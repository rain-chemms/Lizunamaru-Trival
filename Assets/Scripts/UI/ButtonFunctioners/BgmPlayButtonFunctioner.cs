using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

[RequireComponent(typeof(Button))]
public class BgmPlayButtonFunctioner : MonoBehaviour
{
    [SerializeField] private bool nameEmptyToStop = true;//当BGM名称为空时，是否停止播放
    public bool IsNameEmptyToStop() => nameEmptyToStop;
    public void SetNameEmptyToStop(bool isNameEmptyToStop) => nameEmptyToStop = isNameEmptyToStop;
    
    [SerializeField] private string bgmName;//这个按钮控制的BGM名称
    public void SetBgmName(string name) => bgmName = name;
    public string GetBgmName() => bgmName;
    
    [SerializeField] private TMP_Text buttonText;//按钮的文本
    [SerializeField] private bool changeButtonText = true;//是否改变按钮的文本
    public bool IsChangeButtonText() => changeButtonText;
    public void SetChangeButtonText(bool isChange) => changeButtonText = isChange;
    
    [SerializeField] private bool autoLinkFunction = false;//是否自动链接触发功能
    [SerializeField] private Button button;
    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        if(autoLinkFunction) button.onClick.AddListener(OnClick);
        if(localizeEvent == null) localizeEvent = buttonText?.GetComponent<LocalizeStringEvent>();
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTheButtonText();
    }

    public void OnClick()
    {
        //播放相应的BGM
        if(nameEmptyToStop && string.IsNullOrEmpty(bgmName)) BgmController.instance?.GetBgm(BgmController.instance?.GetNowBgm())?.Stop();
        else BgmController.instance?.PlayBgm(bgmName);
    }

    [SerializeField] private LocalizeStringEvent localizeEvent;
    [SerializeField] private string searchTable = "MusicNameTexts";
    public string GetLocalizeName() => buttonText.text;//获取按钮的文本=>即本地化键值

    private void ChangeTheButtonText()
    {
        if(buttonText == null) return;
        if(!changeButtonText) return;
        if(string.IsNullOrEmpty(bgmName))
        {
            buttonText.text = "...";
        }
        else
        {
            //依据当前的BgmName寻找本地化语言
            string key = "MusicName_" + bgmName;
            StartCoroutine(FreshTextName(key));
        }
    }

    private IEnumerator FreshTextName(string musicName)
    {
        //等待初始化完成
        yield return LocalizationSettings.InitializationOperation;
        //异步获取指定表和Key的本地化字符串
        localizeEvent.StringReference.SetReference(searchTable, musicName);
        //刷新显示
        localizeEvent.OnUpdateString?.Invoke(localizeEvent.StringReference.GetLocalizedString());
    }
}
