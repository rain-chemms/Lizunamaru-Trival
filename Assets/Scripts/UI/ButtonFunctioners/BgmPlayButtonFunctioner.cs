using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BgmPlayButtonFunctioner : MonoBehaviour
{
    [SerializeField] private bool nameEmptyToStop = true;//当BGM名称为空时，是否停止播放
    public bool IsNameEmptyToStop()
    {
        return nameEmptyToStop;
    }
    public void SetNameEmptyToStop(bool isNameEmptyToStop)
    {
        nameEmptyToStop = isNameEmptyToStop;
    }

    [SerializeField] private string bgmName;//这个按钮控制的BGM名称
    public void SetBgmName(string name)
    {
        bgmName = name;
    }
    public string GetBgmName()
    {
        return bgmName;
    }
    [SerializeField] private TMP_Text buttonText;//按钮的文本
    [SerializeField] private bool changeButtonText = true;//是否改变按钮的文本
    public bool IsChangeButtonText()
    {
        return changeButtonText;
    }
    public void SetChangeButtonText(bool isChange)
    {
        changeButtonText = isChange;
    }
    [SerializeField] private bool autoLinkFunction = false;//是否自动链接触发功能
    [SerializeField] private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        if(autoLinkFunction) button.onClick.AddListener(OnClick);
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
            buttonText.text = bgmName;
        }
    }
}
