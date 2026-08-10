using UnityEngine;
using UnityEditor;
using System.Reflection;
using GridObjectSystem.RoleSystem.AutoSystem;

[CustomEditor(typeof(ScriptableObject), true)]
public class SmartHideEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (ShouldHide(iterator))
                continue;

            EditorGUILayout.PropertyField(iterator, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool ShouldHide(SerializedProperty property)
    {
        var fieldInfo = GetFieldInfo(property);
        if (fieldInfo == null) return false;

        if (fieldInfo.GetCustomAttribute<HideInheritedAttribute>() == null)
            return false;

        // 当前实例类型 ≠ 字段声明类型 → 继承来的 → 隐藏
        return target.GetType() != fieldInfo.DeclaringType;
    }

    private FieldInfo GetFieldInfo(SerializedProperty property)
    {
        string[] pathParts = property.propertyPath.Split('.');
        System.Type currentType = target.GetType();
        FieldInfo field = null;

        foreach (string part in pathParts)
        {
            field = FindFieldInHierarchy(currentType, part);
            if (field == null) return null;
            currentType = field.FieldType;
        }

        return field;
    }

    /// <summary>
    /// 沿继承链逐级向上查找，确保能找到父类的 private 字段
    /// </summary>
    private static FieldInfo FindFieldInHierarchy(System.Type type, string fieldName)
    {
        System.Type current = type;
        while (current != null)
        {
            var field = current.GetField(fieldName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly); // ← DeclaredOnly 确保只在当前层级找

            if (field != null) return field;
            current = current.BaseType; // ← 向父类移动
        }
        return null;
    }
}