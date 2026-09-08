using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace GlobalSystem.SmartStringExpendSystem
{
    /// <summary>
    /// 把内容包裹成 TMP 红色富文本。
    /// 用法：
    ///   {0:red()}                  → 直接着色当前值
    ///   {0:red():{1} 点生命}        → 先递归渲染嵌套内容，再整体着色
    ///   {0:red(mul(100))}         → 先执行 options 里的内层格式化器，再着色
    ///   {0:red(mul(100)):{1} 点}   → 组合：内层运算 + 嵌套模板
    /// </summary>
    public class RedFormatter : ColoredFormatterBase
    {
        protected override string OpenTag => "<#FF0000>";
        protected override string CloseTag => "</color>";

        public override string[] DefaultNames => new[] { "red", "Red", "RED" };

        public RedFormatter() { Names = DefaultNames; }
    }
}