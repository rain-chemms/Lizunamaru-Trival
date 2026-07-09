using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

/// <summary>
/// Unity 6 + Localization 1.5.12 终极兼容版
/// 支持动态文本：Gain {0}/{1} Defend
/// 注意：1.x 不支持 {Name} 语法，必须用位置参数 {0}, {1}
/// </summary>
[RequireComponent(typeof(LocalizeStringEvent))]
public class DynamicLocalizedText : MonoBehaviour
{
    [Serializable]
    public class VariableEntry
    {
        [Tooltip("位置索引（0, 1, 2...），对应 {0}, {1}, {2}...")]
        public int index;

        [Tooltip("动态数值")]
        public float value;

        [Tooltip("是否作为整数显示")]
        public bool asInteger = true;
    }

    [Header("本地化设置")]
    [Tooltip("本地化字符串引用")]
    public LocalizedString stringReference;

    [Header("动态参数映射")]
    [Tooltip("按位置索引配置参数（{0}, {1}...）")]
    public List<VariableEntry> variables = new List<VariableEntry>();
    public List<VariableEntry> GetVariables()
    {
        return variables;
    }

    private LocalizeStringEvent _localizeEvent;

    void Awake()
    {
        _localizeEvent = GetComponent<LocalizeStringEvent>();
    }

    void OnEnable()
    {
        if (_localizeEvent != null)
        {
            if (stringReference != null)
                _localizeEvent.StringReference = stringReference;

            ApplyVariables();
            _localizeEvent.RefreshString();
        }
    }

    /// <summary>
    /// 设置第 N 个参数（N 从 0 开始）
    /// </summary>
    public void SetParameter(int index, float value, bool asInteger = true)
    {
        var entry = variables.Find(v => v.index == index);
        if (entry != null)
        {
            entry.value = value;
            entry.asInteger = asInteger;
        }
        else
        {
            variables.Add(new VariableEntry { index = index, value = value, asInteger = asInteger });
        }

        ApplyVariables();
        RefreshIfActive();
    }

    /// <summary>
    /// 批量更新所有参数
    /// </summary>
    public void UpdateParameters(List<VariableEntry> newParams)
    {
        variables = newParams ?? new List<VariableEntry>();
        ApplyVariables();
        RefreshIfActive();
    }

    private void ApplyVariables()
    {
        if (variables == null || variables.Count == 0)
        {
            _localizeEvent.StringReference.Arguments = null;
            return;
        }

        // 计算最大索引，确保数组长度足够
        int maxIndex = 0;
        foreach (var v in variables) maxIndex = Mathf.Max(maxIndex, v.index);
        var args = new object[maxIndex + 1];

        // 填充参数
        foreach (var v in variables)
        {
            object val = v.asInteger ? (object)Mathf.RoundToInt(v.value) : v.value;
            if (v.index < args.Length)
                args[v.index] = val;
        }

        _localizeEvent.StringReference.Arguments = args;
    }

    private void RefreshIfActive()
    {
        if (_localizeEvent != null && gameObject.activeInHierarchy)
            _localizeEvent.RefreshString();
    }

#if UNITY_EDITOR
    [ContextMenu("Test Refresh")]
    private void EditorTestRefresh()
    {
        Awake();
        OnEnable();
        Debug.Log($"[DynamicLocalizedText] Refreshed with {variables?.Count ?? 0} parameters.");
    }
#endif
}