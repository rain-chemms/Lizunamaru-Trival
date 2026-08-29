using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class ScreenModeDropdownSetter : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if(dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
    }
    [SerializeField] public SerializableDictionary<int,FullScreenMode> modeDict = new SerializableDictionary<int, FullScreenMode>()
    {
        {0,FullScreenMode.ExclusiveFullScreen},
        {1,FullScreenMode.FullScreenWindow},
        {2,FullScreenMode.MaximizedWindow},
        {3,FullScreenMode.Windowed}
    };//屏幕模式字典
    
    //应用屏幕模式
    public void SetTheScreenMode()
    {
        if(dropdown == null || modeDict == null) return;
        SettingPanel instance = SettingPanel.instance;
        if(instance != null)
        {
            SettingConfigue cfg = instance.GetSettingConfigue();
            if(cfg != null)
            {
                int size = modeDict.Count;
                int index = dropdown.value;
                cfg.fullScreenMode = modeDict[index < 0 ? 0 : index >= size ? size - 1 : index];
            }
            instance.ApplyDisplaySettingsToGame();//应用屏幕模式
        }
    }
}
