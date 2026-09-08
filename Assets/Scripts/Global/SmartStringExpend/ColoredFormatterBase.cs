using System;
using System.Globalization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

//该命名空间用于添加有关智能字符的全局扩展
namespace GlobalSystem.SmartStringExpendSystem
{
    /// <summary>
    /// 颜色格式化器公共基类，把任意值包裹成 TMP 富文本。
    ///
    /// 支持三种写法：
    ///   {0:green()}                        → 直接着色当前值
    ///   {0:green():{1} 点生命}             → 先递归渲染嵌套内容，再整体着色
    ///   {0:green(mul(100))}               → 先执行 options 里的内层格式化器，再着色
    ///
    /// 第三种写法的原理：SmartFormat 解析器把 Green(mul(100)) 拆成
    ///   FormatterName = "Green", FormatterOptions = "mul(100)"
    /// 基类在 options 非空时构造 "{0:mul(100)}" 交给 SmartFormatter 递归求值，
    /// 得到中间结果字符串后再包颜色标签。
    ///
    /// 注意：TMP 的颜色闭合标签只有 </color> 一种形式，
    /// 写成 </#FF0000> 不会被识别，会导致颜色栈只入不出。
    /// </summary>
    public abstract class ColoredFormatterBase : FormatterBase
    {
        protected abstract string OpenTag { get; }
        protected abstract string CloseTag { get; }

        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            var options = formattingInfo.FormatterOptions;
            var value = formattingInfo.CurrentValue;
            var format = formattingInfo.Format;

            // 如果有 options（如 mul(100)），先递归求值内层格式化器。
            // 构造 "{0:mul(100)}" 交给 SmartFormatter，它会按完整流程
            // 解析→选择器求值→格式化器调用，返回格式化后的字符串。
            if (!string.IsNullOrEmpty(options))
            {
                value = formattingInfo.FormatDetails.Formatter.Format(
                    "{0:" + options + "}", value);
            }

            formattingInfo.Write(OpenTag);

            if (format != null && format.Items.Count > 0)
            {
                //递归渲染：Format 里的字面文本被写出，嵌套占位符各自再走一遍
                //Source + Formatter 链，所以子格式化器同样生效。
                //value 作为子占位符求值失败时的回退值向下传递。
                formattingInfo.Write(format, value);
            }
            else if (!string.IsNullOrEmpty(options))
            {
                //没有嵌套内容，但有 options — 直接输出内层格式化器的结果
                formattingInfo.Write((string)value);
            }
            else
            {
                //既没有 options 也没有嵌套内容（例如 {0:green()}），直接输出当前值
                formattingInfo.Write(FormatValue(value));
            }

            formattingInfo.Write(CloseTag);

            //始终返回 true：着色对任意类型都成立。
            //返回 false 会让整条词条抛 "No suitable Formatter could be found"。
            return true;
        }

        /// <summary>
        /// 把值转成显示文本。null 输出空串，数值统一用不变文化，
        /// 避免小数点在某些语言环境下变成逗号。
        /// </summary>
        protected static string FormatValue(object value)
        {
            if (value == null) return string.Empty;
            if (value is IFormattable formattable)
                return formattable.ToString(null, CultureInfo.InvariantCulture);
            return value.ToString();
        }
    }
}
