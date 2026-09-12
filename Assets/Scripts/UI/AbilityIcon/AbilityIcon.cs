using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;           // AsyncOperationHandle 定义
using UnityEngine.ResourceManagement.AsyncOperations; // GetAwaiter 扩展方法所在命名空间
using TMPro;
using GridObjectSystem.AbilitySystem;//能力系统命名空间
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

//用于显示角色能力的UI物体
[RequireComponent(typeof(RectTransform))]
public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private Image abilityImage;
    public Image GetAbilityImage() => abilityImage;
    [SerializeField] private TMP_Text abilityLayer;//能力层数
    public TMP_Text GetAbilityLayer() => abilityLayer;

    [SerializeField] private Sprite debugSprite;//测试用的默认图标
    [SerializeField] private SerializableDictionary<string, Sprite> abilityImageDict;//能力图标映射字典
    public SerializableDictionary<string, Sprite> GetAbilityImageDict() => abilityImageDict;

    [SerializeField] private TMP_Text discription;
    public TMP_Text GetDiscription() => discription;

    [SerializeField] private string searchLocaleTableName = "AbilityDiscription";
    [Header("本地化设置")]
    [Tooltip("本地化字符串引用")]
    [SerializeField] private LocalizeStringEvent localizeEvent;

    void OnEnable()
    {
        //关闭abilityLayer的射线检测
        if(abilityLayer != null) abilityLayer.raycastTarget = false;
    }
    /*
    private async void SetDiscriptionWithAbility(Ability ability)
    {
        string abilityName = "AbilityDiscribe_"+ ability.GetType().Name.ToString();
        Debug.Log("[AbilityIcon]: 要搜索的本地化表:"+searchLocaleTableName+"; 键值:"+abilityName);
        //在本地化系统中寻找对应的本地化表的键值
        // 异步获取条目，不会阻塞主线程
        // 现在可以正常 await 了
        var handle = LocalizationSettings.StringDatabase.GetTableEntryAsync(searchLocaleTableName, abilityName);
    
        var result = await handle.Task; // ✅ 推荐用 .Task 属性代替直接 await

        if (result.Entry != null)
        {
            localizeEvent.StringReference = new LocalizedString(searchLocaleTableName, abilityName);
        }
        else
        {
            Debug.LogWarning($"[AbilityIcon]: Localization key not found: [{searchLocaleTableName}] {abilityName}");
            localizeEvent.StringReference = new LocalizedString(searchLocaleTableName, "AbilityDiscribe_Debug");
        }
        localizeEvent.RefreshString();
    }
    */
    private IEnumerator SetDiscriptionWithAbility(Ability ability)
    {
        string abilityName = "AbilityDiscribe_" + ability.GetType().Name;
        Debug.Log($"[AbilityIcon]: 表:{searchLocaleTableName}; 键:{abilityName}");

        //等待初始化完成
        yield return LocalizationSettings.InitializationOperation;
        //异步获取指定表和Key的本地化字符串
        localizeEvent.StringReference.SetReference(searchLocaleTableName, abilityName);
        //刷新显示
        localizeEvent.OnUpdateString?.Invoke(localizeEvent.StringReference.GetLocalizedString());
        
    }

    private void SetSpriteWithAbility(Ability ability)
    {
        string abilityName = ability.GetType().Name.ToString();
        if (abilityImageDict.ContainsKey(abilityName))
        {
            if (abilityImageDict[abilityName] != null)
            {
                abilityImage.sprite = abilityImageDict[abilityName];
            }
            else
            {
                abilityImage.sprite = debugSprite;
            }
        }
        else
        {
            abilityImage.sprite = debugSprite;
        }
    }
    //刷新层数的显示
    public void RefreshLayerDisplay(int layer, Ability ability = null)
    {
        if ((bool)ability?.CanStack) abilityLayer.text = layer.ToString();
        else abilityLayer.text = "";//不可叠加的能力不显示堆叠数
    }

    //在创建新的图标时调用
    public void SetIconDisplay(Ability ability, int layer)
    {
        SetSpriteWithAbility(ability);
        RefreshLayerDisplay(layer, ability);
        StartCoroutine(SetDiscriptionWithAbility(ability));
    }

}
