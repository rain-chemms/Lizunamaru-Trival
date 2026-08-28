using UnityEngine;
using UnityEngine.UI;
using System.Collections;


//该脚本存储具体的SettingPanelSelector要显示在PartSettingsView中的内容
//必须是UI Canvas
[RequireComponent(typeof(RectTransform))]
public class SettingPanelSettingsContent : MonoBehaviour
{
    [SerializeField] private RectTransform rtf;
    void OnEnable()
    {
        if(rtf == null) rtf = GetComponent<RectTransform>();
    }
}

