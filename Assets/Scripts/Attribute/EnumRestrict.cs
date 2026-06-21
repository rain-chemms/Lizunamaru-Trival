using UnityEngine;
using System;
using System.Linq;

//Enum参数限制器属性
public class EnumRestrictAttribute : PropertyAttribute
{
    public Type EnumType { get; private set; }
    public object[] AllowedValues { get; private set; }

    public EnumRestrictAttribute(Type enumType, params object[] values)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("[EnumRestrict]:Type must be an <Enum>", nameof(enumType));

        EnumType = enumType;
        AllowedValues = values;
    }
}