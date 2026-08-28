using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingPanelCloseButton : MonoBehaviour
{
    [SerializeField] private SettingPanel panel;
    //点击关闭按钮时的操作
    public void CloseThePanel()
    {
        panel?.SaveSettingsToFile();//保存设置
        panel?.GetComponent<Animator>()?.SetBool("IsOpen",false);//关闭面板
    }
}
