using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownVector2IntLinker : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    void OnEnable()
    {
        if(dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
    }

    // 在 Inspector 中配置所有可选的 Vector2Int 值
    [SerializeField] private List<Vector2Int> data = new List<Vector2Int>(4);

    private void Start()
    {
        // 动态生成下拉选项文本
        dropdown.ClearOptions();
        foreach (var size in data)
            dropdown.options.Add(new TMP_Dropdown.OptionData($"{size.x} x {size.y}"));

        dropdown.RefreshShownValue();
        // 监听选择变化
        dropdown.onValueChanged.AddListener(OnChanged);
    }

    private void OnChanged(int index)
    {
        Vector2Int selectedSize = data[index];
        Debug.Log($"[DropdownVector2IntLinker]: {selectedSize}");
        // TODO: 使用 selectedSize
    }

    // 外部获取当前值的公共方法
    public Vector2Int GetCurrentValue()
    {
        return data[dropdown.value];
    }
}