using UnityEngine;

[RequireComponent(typeof(Animator))] 
[RequireComponent(typeof(SettingPanel))]
public class SettingPanelAnimatorEvent : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SettingPanel settingPanel;
    void OnEnable()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(settingPanel == null) settingPanel = GetComponent<SettingPanel>();
        if(settingPanel == null) settingPanel = SettingPanel.instance;
    }

    public void ClearContent()
    {
        settingPanel?.ClearTheDisplayArea();
    }

    public void SetNewContent()
    {
        settingPanel?.SetContentToDisplayArea();    
    }
}
