using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

//该命名空间用于添加有关智能字符的全局扩展
namespace GlobalSystem.SmartStringExpendSystem
{
    /// <summary>
    /// 自定义 SmartFormat 格式化器，支持数值乘法运算。
    /// 用法：{0:multiply(100)}  或  {0:mul(2.5)}
    /// </summary>
    public class MultiplyFormatter : FormatterBase
    {
        /// <summary>
        /// 格式化器的名称，可通过这些名称在 Smart String 中显式调用。
        /// "multiply" 是完整名，"mul" 是缩写。
        /// </summary>
        public override string[] DefaultNames => new[] { "multiply", "mul" };
        public MultiplyFormatter() { Names = DefaultNames; } 

        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            if (!TryConvertToDouble(formattingInfo.CurrentValue, out double number))
                return false;

            // 关键：{0:mul(100)} 的 "100" 在 FormatterOptions 里
            if (!double.TryParse(
                formattingInfo.FormatterOptions,
                NumberStyles.Any,
                CultureInfo.InvariantCulture, 
                out double multiplier
                )
            ) {
                Debug.LogWarning("[MultiplyFormatter]: Invalid multiplier format. multiplier Set To 1!");
                multiplier = 1.0;
            }
            formattingInfo.Write(FormatValue(number * multiplier));
            return true;
        }

        /// <summary>
        /// 尝试将对象转换为 double
        /// </summary>
        private static bool TryConvertToDouble(object value, out double result)
        {
            if (value == null)
            {
                result = 0;
                return false;
            }

            if (value is IConvertible convertible)
            {
                try
                {
                    result = Convert.ToDouble(convertible, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    result = 0;
                    return false;
                }
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// 智能格式化数字输出：整数不显示小数点，小数保留合理位数
        /// </summary>
        private static string FormatValue(double value)
        {
            // 如果是整数，不显示小数点
            if (Math.Abs(value - Math.Round(value)) < 1e-10)
            {
                return ((long)Math.Round(value)).ToString(CultureInfo.InvariantCulture);
            }

            // 否则最多保留 4 位小数，去掉末尾零
            return value.ToString("0.####", CultureInfo.InvariantCulture);
        }
    }
}