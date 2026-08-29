using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class VSyncDropdownSetter : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    void OnEnable()
    {
        if(dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
    }

    public void SetTheVsync()
    {
        SettingPanel instance = SettingPanel.instance;
        if(instance != null)
        {
            SettingConfigue cfg = instance.GetSettingConfigue();
            if(cfg!=null) cfg.vSyncCount = (int)dropdown?.value;//直接应用索引设置
            Debug.Log($"[VSyncAppender]: Set the VSync Count to: {cfg.vSyncCount}");
        }
        instance.ApplyDisplaySettingsToGame();
    }
}
