using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace GlobalSystem.SmartStringExpendSystem
{
    /// <summary>
    /// 把内容包裹成 TMP 绿色富文本。
    /// 用法同 RedFormatter，颜色标签为 <#00FF00>。
    /// </summary>
    public class GreenFormatter : ColoredFormatterBase
    {
        protected override string OpenTag => "<#00FF00>";
        protected override string CloseTag => "</color>";

        public override string[] DefaultNames => new[] { "green", "Green", "GREEN" };

        public GreenFormatter() { Names = DefaultNames; }
    }
}