using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BgmListButton : MonoBehaviour
{
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
    // Update is called once per frame
    void Update()
    {
        ChangeTheButtonText();
    }

    public void OnClick()
    {
        //播放相应的BGM
        BgmController.instance?.PlayBgm(bgmName);
    }

    private void ChangeTheButtonText()
    {
        if(buttonText == null) return;
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
