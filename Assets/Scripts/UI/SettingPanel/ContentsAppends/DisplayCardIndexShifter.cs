using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]    
public class DisplayCardIndexShifter : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    void OnEnable()
    {
        if(toggle == null) toggle = GetComponent<Toggle>();
    }

    public void SetDisplayCardIndex()
    {
        SettingPanel instance = SettingPanel.instance;
        if(instance != null)
        {
            SettingConfigue cfg = instance.GetSettingConfigue();
            if(cfg!=null) cfg.displayCardIndex = (bool)toggle?.isOn;//直接应用索引设置
            Debug.Log($"[DisplayCardIndexShifter]: Set the CardIndexDisplay State to: {cfg.displayCardIndex}");
        }
        instance.ApplyDisplaySettingsToGame();   
    }
}
