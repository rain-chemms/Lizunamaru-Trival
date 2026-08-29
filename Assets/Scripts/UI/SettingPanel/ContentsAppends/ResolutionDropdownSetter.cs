using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(DropdownVector2IntLinker))]
public class ResolutionDropdownSetter : MonoBehaviour
{ 
    [SerializeField] private DropdownVector2IntLinker linker;
    void OnEnable()
    {
        if(linker == null) linker = GetComponent<DropdownVector2IntLinker>();
    }

    public void SetTheResolution()
    {
        Vector2Int value = linker.GetCurrentValue();//获取当前的分辨率
        SettingPanel instance = SettingPanel.instance;
        if(instance != null)
        {
            SettingConfigue cfg = instance.GetSettingConfigue();
            if(cfg != null)
            {
                cfg.resolutionWidth = value.x;
                cfg.resolutionHeight = value.y;
            }
        }
        instance.ApplyDisplaySettingsToGame();
        Debug.Log($"[ResolutionDropdownSetter]: Set the resolution to: {value.x}x{value.y}");
    }
}