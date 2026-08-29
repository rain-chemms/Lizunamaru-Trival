using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


[RequireComponent(typeof(TMP_Dropdown))]
public class ManualLanguageDropdownCoroutine : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    // 缓存排序后的语言列表,与 Dropdown 索引严格对应
    private List<Locale> _locales = new List<Locale>();
    
    void OnEnable()
    {
        if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnLanguageSelected);    
    }

    void OnDisable()
    {
        if (dropdown != null) dropdown.onValueChanged.RemoveListener(OnLanguageSelected);
    }

    private IEnumerator Start()
    {
        // 等待 Localization 系统初始化完成
        yield return LocalizationSettings.InitializationOperation;

        // 初始化完成后判空:未配置 Locale 资产时 AvailableLocales 可能为 null
        if (LocalizationSettings.AvailableLocales == null)
        {
            Debug.LogError("[LanguageDropdown]: Localization have been Initialized, But AvailableLocales is null. Please Check the Locale Asset Create or Not, and add it to the Available Locales Table.");
            yield break;
        }

        PopulateDropdown();
    }

    /// <summary>
    /// 构建并填充 Dropdown 选项
    /// </summary>
    private void PopulateDropdown()
    {
        // 关键修复:AvailableLocales 返回 ILocalesProvider，
        // 该接口没有 Count 属性,需显式转换为 IReadOnlyList<Locale>
        ILocalesProvider localesProvider = LocalizationSettings.AvailableLocales;
        
        //获取_locales列表
        _locales =  localesProvider.Locales.ToList();

        // 按显示名称排序，保证 UI 顺序稳定
        _locales = Enumerable.Range(0, _locales.Count)
            .Select(i => _locales[i])
            .OrderBy(l => l.LocaleName)
            .ToList();

        dropdown.ClearOptions();
        dropdown.AddOptions(_locales.Select(l => l.LocaleName).ToList());

        // 同步当前激活语言到 Dropdown 选中项
        var currentLocale = LocalizationSettings.SelectedLocale;
        int currentIndex = _locales.FindIndex(l => l == currentLocale);
        if (currentIndex >= 0)
        {
            dropdown.SetValueWithoutNotify(currentIndex);
        }
        else if (currentLocale != null)
        {
            Debug.LogWarning($"[LanguageDropdown]: The Language '{currentLocale.LocaleName}'not In Available Language List," + "it may be Removed or not Not Loaded.");
        }
    }

    /// <summary>
    /// 用户选择新语言时的回调
    /// </summary>
    private void OnLanguageSelected(int index)
    {
        if (index < 0 || index >= _locales.Count) return;

        Locale selectedLocale = _locales[index];
        Debug.Log($"[LanguageDropdown]: Shift Language to → {selectedLocale.LocaleName} ({selectedLocale.Identifier.Code}).");
        //LocalizationSettings.SelectedLocale = selectedLocale;
        //保存当前的设置信息
        SettingPanel instance = SettingPanel.instance;
        if(instance != null)
        {
            SettingConfigue cfg = instance.GetSettingConfigue();
            if(cfg!=null) cfg.localeCode = selectedLocale.Identifier.Code;
            //触发设置应用设置
            instance.ApplyLanguageSettingsToGame();
        }
        
    }
}