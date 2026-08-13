using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
public class ShowIfBoolDrawer : PropertyDrawer
{
    private ShowIfBoolAttribute Attr => (ShowIfBoolAttribute)attribute;

    private string GetParentPath(SerializedProperty property)
    {
        int lastDot = property.propertyPath.LastIndexOf('.');
        return lastDot > 0 ? property.propertyPath.Substring(0, lastDot) : "";
    }

    private SerializedProperty GetBoolProperty(SerializedProperty property)
    {
        string boolPath = string.IsNullOrEmpty(GetParentPath(property))
            ? Attr.BoolFieldName
            : $"{GetParentPath(property)}.{Attr.BoolFieldName}";
        return property.serializedObject.FindProperty(boolPath);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        var boolProp = GetBoolProperty(property);
        if (boolProp == null || boolProp.propertyType != SerializedPropertyType.Boolean)
            return true; // 降级显示
        return Attr.Invert ? !boolProp.boolValue : boolProp.boolValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ✅ 关键：如果不应显示，直接 return —— 不绘制任何东西（包括标题！）
        if (!ShouldShow(property))
            return;

        // ✅ 正常绘制：显式传 includeChildren=true 确保列表完整展开
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // ✅ 隐藏时必须返回 0，但更重要的是 OnGUI 不绘制标题
        if (!ShouldShow(property))
            return 0f;

        // ⚠️ 注意：对于列表，必须用 includeChildren=true 获取真实高度
        // 否则可能返回单行高度（如 16px），导致布局错位
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}