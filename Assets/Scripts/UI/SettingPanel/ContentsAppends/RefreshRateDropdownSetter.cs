using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 自动获取系统支持的刷新率并绑定到 TMP_Dropdown
/// 挂载到包含 TMP_Dropdown 的 GameObject 上即可使用
/// </summary>
[RequireComponent(typeof(TMP_Dropdown))]
public class RefreshRateDropdownSetter : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private TMP_Dropdown dropdown;
    void OnEnable()
    {
        if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        PopulateRefreshRates();
        dropdown.onValueChanged.AddListener(OnRefreshRateSelected);
    }

    void OnDisable()
    {
        dropdown?.onValueChanged.RemoveListener(OnRefreshRateSelected);
    }

    [Header("配置")]
    [Tooltip("是否只显示与当前屏幕分辨率匹配的刷新率")]
    [SerializeField] private bool matchCurrentResolution = true;

    [Tooltip("默认选中的刷新率（0 = 最高可用刷新率）")]
    [SerializeField] int defaultIndex = 0;

    // 缓存实际有效的 RefreshRate 列表，与 Dropdown 选项索引一一对应
    private List<RefreshRate> _availableRates = new List<RefreshRate>();

    /// <summary>
    /// 查询系统支持的刷新率并填充 Dropdown
    /// </summary>
    public void PopulateRefreshRates()
    {
        var allResolutions = Screen.resolutions;

        // 1.筛选:仅保留匹配当前分辨率的模式(或全部)
        var filtered = matchCurrentResolution
            ? allResolutions.Where(r => r.width == Screen.currentResolution.width
                                     && r.height == Screen.currentResolution.height)
            : allResolutions.AsEnumerable();

        // 2.去重+按刷新率升序排列
        _availableRates = filtered
            .Select(r => r.refreshRateRatio)
            .Distinct()
            .OrderBy(r => (float)r.value)
            .ToList();

        // 3.生成显示文本并填充Dropdown
        var options = new List<string>(_availableRates.Count);
        foreach (var rate in _availableRates)
        {
            float hz = (float)rate.value;
            // 整数显示为 "60 Hz"，小数保留两位如 "59.94 Hz"
            string label = Mathf.Approximately(hz, Mathf.Round(hz))
                ? $"{Mathf.RoundToInt(hz)} Hz"
                : $"{hz:F2} Hz";
            options.Add(label);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        // 4. 设置默认选中项
        int targetIndex = Mathf.Clamp(defaultIndex, 0, _availableRates.Count - 1);
        dropdown.SetValueWithoutNotify(targetIndex);

        Debug.Log($"[RefreshRateDropdown]: Load In {_availableRates.Count} useable refresh rates.");
    }

    /// <summary>
    /// 用户选择新刷新率时的回调
    /// </summary>
    private void OnRefreshRateSelected(int index)
    {
        if (index < 0 || index >= _availableRates.Count) return;

        var selectedRate = _availableRates[index];
        float hz = (float)selectedRate.value;

        Debug.Log($"[RefreshRateDropdown] Change refresh rate to {(float)selectedRate.value:F2} Hz");

        //设置当前分辨率到
        SettingPanel instance = SettingPanel.instance;
        if(instance != null)
        {
            SettingConfigue cfg = instance.GetSettingConfigue();//获取设置
            if(cfg != null)
            {
                cfg.refreshRate = hz;//设置刷新率
            }
            instance.ApplyDisplaySettingsToGame();//应用设置
        }
    }
}