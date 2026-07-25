using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RectTransform))]
public class SpellAttackDisplayer : MonoBehaviour
{
    public static SpellAttackDisplayer instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private TMP_Text spellText;
    public TMP_Text GetSpellText()
    {
        return spellText;
    }
    [SerializeField] private Image roleImage;
    public Image GetRoleImage()
    {
        return roleImage;
    }
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    [SerializeField] private Animator animator;
    public Animator GetAnimator()
    {
        return animator;
    }
    public List<AudioSource> GetAudioSources()
    {
        return audioSources;
    }
    void Start()
    {
        //自动获取全部的音频源
        List<AudioSource> AS = GetComponentsInChildren<AudioSource>().ToList();
        foreach (AudioSource aS in AS)
        {
            if(aS == null) continue;
            if(audioSources.Contains(aS)) continue;
            audioSources.Add(aS);
        }
        //尝试自动获取角色spell图片容器
        if(roleImage == null)
        {
            foreach(Image img in GetComponentsInChildren<Image>())
            {
                if(img == null) continue;
                if(img.name.Equals("RoleImage")) 
                {
                    roleImage = img;
                    break;
                }
            }
        }
        //尝试自动获取技能文本容器
        if(spellText == null)
        {
            foreach(TMP_Text txt in GetComponentsInChildren<TMP_Text>())
            {
                if(txt == null) continue;
                if(txt.name.Equals("SpellText"))
                {
                    spellText = txt;
                    break;
                }
            }
        }
        //尝试自动获取动画控制器
        if(animator == null) animator = GetComponent<Animator>();
    }

    /*
        附加功能: 触发一次符卡显示
    */
    public IEnumerator WakeDisplayer(Sprite sprite,bool leftOrRight = true,string textKey = "Spell_Debug",string searchTable = "SpellDisplayTexts")
    {
        //SpellAttackDisplayer instance = SpellAttackDisplayer.instance;
        //设置图像
        Image img = instance?.GetRoleImage();
        if (img != null)
        {
            img.sprite = sprite;
        }
        //设置文本
        yield return LocalizationSettings.InitializationOperation;
        string keyName = textKey;
        // 异步获取
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(searchTable, keyName);
        yield return operation;
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            TMP_Text txt = instance?.GetSpellText();
            if (txt != null)
            {
                txt.text = operation.Result;
            }
        }
        else
        {
            Debug.LogError($"[CardSpellAttackWaker]: Display the Keywords Failed: {operation.OperationException}");
        }
        //播放动画
        Animator animator = instance?.GetAnimator();
        if (animator != null)
        {
            animator?.SetTrigger(leftOrRight ? "Left" : "Right");
            yield return null;
            AnimatorStateInfo stateInfo =(AnimatorStateInfo)animator?.GetCurrentAnimatorStateInfo(0);
            float length = stateInfo.length;
            yield return new WaitForSeconds(length / animator.speed); //等待动画播放完毕 
        }
    }
}
