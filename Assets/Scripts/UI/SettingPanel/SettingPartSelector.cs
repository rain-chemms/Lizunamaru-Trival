using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingPartSelector: MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
    }
    
    [SerializeField] private SettingPanelSettingsContent settingPanelButton;
    public SettingPanelSettingsContent GetSettingContent() => settingPanelButton;
    public void SetSettingContent(SettingPanelSettingsContent settingPanelButton) => this.settingPanelButton = settingPanelButton;
    
    public void TriggerTheDisplayChange()
    {
        SettingPanel.instance.SetDisplayContent(settingPanelButton);//设置显示内容
        //触发显示动画器
        SettingPanel.instance?.GetComponent<Animator>()?.SetTrigger("ChangeDisplay");
    }
}
