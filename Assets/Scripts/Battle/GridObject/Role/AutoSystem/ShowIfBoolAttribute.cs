using UnityEngine;

public class ShowIfBoolAttribute : PropertyAttribute
{
    public string BoolFieldName { get; }
    public bool Invert { get; } // true = 当bool为false时显示

    public ShowIfBoolAttribute(string boolFieldName, bool invert = false)
    {
        BoolFieldName = boolFieldName;
        Invert = invert;
    }
}