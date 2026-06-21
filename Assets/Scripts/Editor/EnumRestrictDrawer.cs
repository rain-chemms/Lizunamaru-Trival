using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

// 修改点1: 关联正确的非泛型属性类 EnumRestrictAttribute
[CustomPropertyDrawer(typeof(EnumRestrictAttribute))]
public class EnumRestrictDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 确保字段确实是枚举类型
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            // 修改点2: 强制转换为正确的属性类型
            EnumRestrictAttribute restrictAttr = attribute as EnumRestrictAttribute;

            if (restrictAttr != null)
            {
                // 修改点3: 通过反射获取字段真实的枚举类型
                // 注意：SerializedProperty 不直接暴露字段的 Type，需要通过目标对象和路径获取
                Type fieldType = fieldInfo.FieldType;
                if (!fieldType.IsEnum)
                {
                    // 防御性编程：虽然检查了 propertyType，但最好再确认一下 fieldType
                    EditorGUI.PropertyField(position, property, label);
                    return;
                }

                // 修改点4: 获取所有允许的枚举值名称
                // 将 AllowedValues (object[]) 转换为字符串数组用于显示
                string[] allowedNames = restrictAttr.AllowedValues
                    .Select(val => Enum.GetName(fieldType, val))
                    .Where(name => name != null) // 过滤掉无效的值
                    .ToArray();

                if (allowedNames.Length > 0)
                {
                    // 修改点5: 获取当前值在“允许列表”中的索引
                    // property.enumValueIndex 对应的是所有枚举值的索引，我们需要映射到 allowedNames
                    string currentName = property.enumDisplayNames[property.enumValueIndex];
                    int newIndex = Array.IndexOf(allowedNames, currentName);

                    // 如果当前值不在允许列表中，默认选中第一个（或者你可以设为 -1 表示无）
                    if (newIndex < 0) newIndex = 0;

                    // 修改点6: 绘制下拉菜单
                    EditorGUI.BeginChangeCheck();
                    // 注意：这里直接使用 allowedNames 作为选项
                    newIndex = EditorGUI.Popup(position, label.text, newIndex, allowedNames);

                    if (EditorGUI.EndChangeCheck())
                    {
                        // 将选中的名字转回枚举值，再转成 int 赋值给 property
                        // 注意：这里不能直接用 property.enumValueIndex = newIndex，因为 newIndex 是 allowedNames 的索引
                        Enum selectedEnum = (Enum)Enum.Parse(fieldType, allowedNames[newIndex]);
                        property.enumValueIndex = Convert.ToInt32(selectedEnum);
                    }
                }
                else
                {
                    // 如果没有允许的选项，显示警告或默认绘制
                    EditorGUI.LabelField(position, label.text, "No allowed values");
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
        else
        {
            // 如果不是枚举，按默认方式绘制
            EditorGUI.PropertyField(position, property, label);
        }
    }
}