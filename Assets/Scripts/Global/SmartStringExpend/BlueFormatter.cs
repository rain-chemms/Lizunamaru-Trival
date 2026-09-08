using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace GlobalSystem.SmartStringExpendSystem
{
    /// <summary>
    /// 把内容包裹成 TMP 蓝色富文本。
    /// 用法同 RedFormatter，颜色标签为 <#0000FF>。
    /// </summary>
    public class BlueFormatter : ColoredFormatterBase
    {
        protected override string OpenTag => "<#0000FF>";
        protected override string CloseTag => "</color>";

        public override string[] DefaultNames => new[] { "blue", "Blue", "BLUE" };

        public BlueFormatter() { Names = DefaultNames; }
    }
}