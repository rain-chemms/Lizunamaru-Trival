using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
public class ShowIfBoolDrawer : PropertyDrawer
{
    private ShowIfBoolAttribute Attr => (ShowIfBoolAttribute)attribute;

    /// <summary>
    /// 获取当前属性的父级路径。
    /// 例如：
    /// "parent.child.target" -> "parent.child"
    /// "list.Array.data[0].target" -> "list.Array.data[0]"
    /// "target" (根级) -> ""
    /// </summary>
    private string GetParentPath(SerializedProperty property)
    {
        string path = property.propertyPath;
        int lastDot = path.LastIndexOf('.');
        return lastDot > 0 ? path.Substring(0, lastDot) : "";
    }

    /// <summary>
    /// 根据父级路径和字段名，安全地找到同级的 bool 属性。
    /// </summary>
    private SerializedProperty GetBoolProperty(SerializedProperty property)
    {
        string parentPath = GetParentPath(property);
        string boolPath = string.IsNullOrEmpty(parentPath) 
            ? Attr.BoolFieldName 
            : $"{parentPath}.{Attr.BoolFieldName}";

        return property.serializedObject.FindProperty(boolPath);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var boolProp = GetBoolProperty(property);

        // 如果找不到目标 bool 字段，降级处理：显示错误提示并正常绘制原字段（防止 Inspector 崩溃）
        if (boolProp == null || boolProp.propertyType != SerializedPropertyType.Boolean)
        {
            EditorGUI.HelpBox(position, $"[ShowIfBool] 找不到同级 bool 字段: '{Attr.BoolFieldName}'", MessageType.Warning);
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        bool shouldShow = Attr.Invert ? !boolProp.boolValue : boolProp.boolValue;

        if (shouldShow)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        // 如果不显示，则什么都不画（配合 GetPropertyHeight 返回 0 实现隐藏）
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var boolProp = GetBoolProperty(property);

        // 找不到字段时，返回默认高度（避免 NullReferenceException 导致 Inspector 空白）
        if (boolProp == null || boolProp.propertyType != SerializedPropertyType.Boolean)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        bool shouldShow = Attr.Invert ? !boolProp.boolValue : boolProp.boolValue;

        return shouldShow
            ? EditorGUI.GetPropertyHeight(property, label, true)
            : 0f; // ⭐ 核心：隐藏时高度归零
    }
}