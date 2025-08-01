using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Scorpio;
using System.Linq;

namespace System
{
    /// <summary>
    /// <see cref="string"/> 类的扩展方法集合。
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 确保字符串以指定字符结尾，如果不是则添加该字符。
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <param name="c">应该出现在字符串末尾的字符</param>
        /// <param name="comparisonType">字符串比较类型</param>
        /// <returns>确保以指定字符结尾的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull(str, nameof(str));

            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        /// <summary>
        /// 确保字符串以指定字符串结尾，如果不是则添加该字符串。
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <param name="c">应该出现在字符串末尾的字符串</param>
        /// <param name="comparisonType">字符串比较类型</param>
        /// <returns>确保以指定字符串结尾的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string EnsureEndsWith(this string str, string c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull(str, nameof(str));

            if (str.EndsWith(c, comparisonType))
            {
                return str;
            }

            return str + c;
        }

        /// <summary>
        /// 确保字符串以指定字符串开头，如果不是则添加该字符串。
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <param name="c">应该出现在字符串开头的字符串</param>
        /// <param name="comparisonType">字符串比较类型</param>
        /// <returns>确保以指定字符串开头的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string EnsureStartsWith(this string str, string c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull(str, nameof(str));

            if (str.StartsWith(c, comparisonType))
            {
                return str;
            }

            return c + str;
        }

        /// <summary>
        /// 指示字符串是否为 null 或 <see cref="string.Empty"/>。
        /// </summary>
        /// <param name="str">要测试的字符串</param>
        /// <returns>如果字符串为 null 或空字符串 ("")，则为 true；否则为 false</returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// 指示字符串是否为 null、空或仅由空白字符组成。
        /// </summary>
        /// <param name="str">要测试的字符串</param>
        /// <returns>如果字符串为 null、空或仅由空白字符组成，则为 true；否则为 false</returns>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// 从字符串的开头获取指定长度的子字符串。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="len">要获取的子字符串长度</param>
        /// <returns>指定长度的子字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentException"><paramref name="len"/> 大于字符串长度时抛出</exception>
        public static string Left(this string str, int len)
        {
            Check.NotNull(str, nameof(str));
            if (len <= 0)
            {
                throw new ArgumentException("len 参数不能小于 0", nameof(len));
            }
            if (str.Length < len)
            {
                throw new ArgumentException("len 参数不能大于给定字符串的长度！");
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// 将字符串中的行结束符转换为 <see cref="Environment.NewLine"/>。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string NormalizeLineEndings(this string str) => str.Replace("\n\r", "\n").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);

        /// <summary>
        /// 获取字符在字符串中第 n 次出现的索引。
        /// </summary>
        /// <param name="str">要搜索的源字符串</param>
        /// <param name="c">要在 <paramref name="str"/> 中搜索的字符</param>
        /// <param name="n">出现次数</param>
        /// <returns>字符第 n 次出现的索引，如果没有找到则返回 -1</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static int NthIndexOf(this string str, char c, int n)
        {
            Check.NotNull(str, nameof(str));

            var count = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != c)
                {
                    continue;
                }

                if (++count == n)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 获取字符在字符串中第 n 次出现的索引，使用指定的字符串比较类型。
        /// </summary>
        /// <param name="str">要搜索的源字符串</param>
        /// <param name="c">要在 <paramref name="str"/> 中搜索的字符</param>
        /// <param name="n">出现次数</param>
        /// <param name="comparisonType">字符串比较类型</param>
        /// <returns>字符第 n 次出现的索引，如果没有找到则返回 -1</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static int NthIndexOf(this string str, char c, int n, StringComparison comparisonType)
        {
            Check.NotNull(str, nameof(str));

            var count = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (!str[i].ToString().Equals(c.ToString(), comparisonType))
                {
                    continue;
                }
                if (++count == n)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从字符串末尾移除指定的后缀（首次出现）。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="postFixes">一个或多个后缀</param>
        /// <returns>修改后的字符串，如果不存在任何指定的后缀则返回原字符串</returns>
        public static string RemovePostFix(this string str, params string[] postFixes) => str.RemovePostFix(StringComparison.Ordinal, postFixes);

        /// <summary>
        /// 从字符串末尾移除指定的后缀（首次出现），使用指定的字符串比较类型。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="comparisonType">字符串比较类型</param>
        /// <param name="postFixes">一个或多个后缀</param>
        /// <returns>修改后的字符串，如果不存在任何指定的后缀则返回原字符串</returns>
        public static string RemovePostFix(this string str, StringComparison comparisonType, params string[] postFixes)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            var postFix = postFixes.FirstOrDefault(postFix => str.EndsWith(postFix, comparisonType));
            if (postFix != null)
            {
                return str.Left(str.Length - postFix.Length);
            }
            return str;
        }

        /// <summary>
        /// 从字符串开头移除指定的前缀（首次出现）。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="preFixes">一个或多个前缀</param>
        /// <returns>修改后的字符串，如果不存在任何指定的前缀则返回原字符串</returns>
        public static string RemovePreFix(this string str, params string[] preFixes) => str.RemovePreFix(StringComparison.Ordinal, preFixes);

        /// <summary>
        /// 从字符串开头移除指定的前缀（首次出现），使用指定的字符串比较类型。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="comparisonType">字符串比较类型</param>
        /// <param name="preFixes">一个或多个前缀</param>
        /// <returns>修改后的字符串，如果不存在任何指定的前缀则返回原字符串</returns>
        public static string RemovePreFix(this string str, StringComparison comparisonType, params string[] preFixes)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }

            if (preFixes.IsNullOrEmpty())
            {
                return str;
            }

            var preFix = preFixes.FirstOrDefault(preFix => str.StartsWith(preFix, comparisonType));
            if (preFix != null)
            {
                // 如果字符串以指定的前缀开头，则返回去除前缀后的字符串
                return str.Right(str.Length - preFix.Length);
            }

            return str;
        }

        /// <summary>
        /// 从字符串的末尾获取指定长度的子字符串。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="len">要获取的子字符串长度</param>
        /// <returns>指定长度的子字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentException"><paramref name="len"/> 大于字符串长度时抛出</exception>
        public static string Right(this string str, int len)
        {
            Check.NotNull(str, nameof(str));
            if (len <= 0)
            {
                throw new ArgumentException("len 参数不能小于 0", nameof(len));
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len 参数不能大于给定字符串的长度！");
            }

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// 使用 string.Split 方法按指定分隔符拆分字符串。
        /// </summary>
        /// <param name="str">要拆分的字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns>拆分后的字符串数组</returns>
        public static string[] Split(this string str, string separator) => str.Split(new[] { separator }, StringSplitOptions.None);

        /// <summary>
        /// 使用 string.Split 方法按指定分隔符和选项拆分字符串。
        /// </summary>
        /// <param name="str">要拆分的字符串</param>
        /// <param name="separator">分隔符</param>
        /// <param name="options">拆分选项</param>
        /// <returns>拆分后的字符串数组</returns>
        public static string[] Split(this string str, string separator, StringSplitOptions options) => str.Split(new[] { separator }, options);

        /// <summary>
        /// 使用 string.Split 方法按 <see cref="Environment.NewLine"/> 拆分字符串为行。
        /// </summary>
        /// <param name="str">要拆分的字符串</param>
        /// <returns>拆分后的字符串行数组</returns>
        public static string[] SplitToLines(this string str) => str.Split(Environment.NewLine);

        /// <summary>
        /// 使用 string.Split 方法按 <see cref="Environment.NewLine"/> 和指定选项拆分字符串为行。
        /// </summary>
        /// <param name="str">要拆分的字符串</param>
        /// <param name="options">拆分选项</param>
        /// <returns>拆分后的字符串行数组</returns>
        public static string[] SplitToLines(this string str, StringSplitOptions options) => str.Split(Environment.NewLine, options);

        /// <summary>
        /// 将 PascalCase 格式的字符串转换为 camelCase 格式。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="useCurrentCulture">设置为 true 使用当前文化，否则使用不变文化</param>
        /// <returns>camelCase 格式的字符串</returns>
        public static string ToCamelCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();
            }

            return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str.Substring(1);
        }

        /// <summary>
        /// 将给定的 PascalCase/camelCase 字符串转换为句子格式（按空格分隔单词）。
        /// 例如："ThisIsASampleSentence" 转换为 "This is a sample sentence"。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="useCurrentCulture">设置为 true 使用当前文化，否则使用不变文化</param>
        /// <returns>句子格式的字符串</returns>
        public static string ToSentenceCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return useCurrentCulture
                ? MyRegex().Replace(str, m => $"{m.Value[0]} {char.ToLower(m.Value[1])}")
                : MyRegex().Replace(str, m => $"{m.Value[0]} {char.ToLowerInvariant(m.Value[1])}");
        }

        /// <summary>
        /// 将给定的 PascalCase/camelCase 字符串转换为连字符格式（按连字符字符分隔单词）。
        /// 例如："ThisIsASampleHyphen" 转换为 "this-is-a-sample-hyphen"。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="useCurrentCulture">设置为 true 使用当前文化，否则使用不变文化</param>
        /// <returns>连字符格式的字符串</returns>
        public static string ToHyphen(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return useCurrentCulture
                ? Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLower(m.Value[1])).ToLower()
                : Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLowerInvariant(m.Value[1])).ToLowerInvariant();
        }

        /// <summary>
        /// 将字符串转换为枚举值。
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的字符串值</param>
        /// <returns>返回枚举对象</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 为 null 时抛出</exception>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            Check.NotNull(value, nameof(value));
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// 将字符串转换为枚举值。
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的字符串值</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>返回枚举对象</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 为 null 时抛出</exception>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            Check.NotNull(value, nameof(value));
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// 将字符串转换为 MD5 哈希值。
        /// </summary>
        /// <param name="str">要哈希的字符串</param>
        /// <returns>MD5 哈希的十六进制字符串表示</returns>
        public static string ToMd5(this string str)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 将 camelCase 格式的字符串转换为 PascalCase 格式。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="useCurrentCulture">设置为 true 使用当前文化，否则使用不变文化</param>
        /// <returns>PascalCase 格式的字符串</returns>
        public static string ToPascalCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return useCurrentCulture ? str.ToUpper() : str.ToUpperInvariant();
            }

            return (useCurrentCulture ? char.ToUpper(str[0]) : char.ToUpperInvariant(str[0])) + str.Substring(1);
        }

        /// <summary>
        /// 如果字符串超过最大长度，则从字符串开头获取子字符串。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>截断后的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Left(maxLength);
        }

        /// <summary>
        /// 如果字符串超过最大长度，则从字符串末尾获取子字符串。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>截断后的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string TruncateFromBeginning(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Right(maxLength);
        }

        /// <summary>
        /// 如果字符串超过最大长度，则从字符串开头获取子字符串。
        /// 如果被截断，则在字符串末尾添加"..."后缀。
        /// 返回的字符串长度不会超过最大长度。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>截断后的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string TruncateWithPostfix(this string str, int maxLength) => str.TruncateWithPostfix(maxLength, "...");

        /// <summary>
        /// 如果字符串超过最大长度，则从字符串开头获取子字符串。
        /// 如果被截断，则在字符串末尾添加指定的 <paramref name="postfix"/> 后缀。
        /// 返回的字符串长度不会超过最大长度。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="postfix">要添加的后缀</param>
        /// <returns>截断后的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 null 时抛出</exception>
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty || maxLength == 0)
            {
                return string.Empty;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            if (maxLength <= postfix.Length)
            {
                return postfix.Left(maxLength);
            }

            return str.Left(maxLength - postfix.Length) + postfix;
        }

        /// <summary>
        /// 使用 <see cref="Encoding.UTF8"/> 编码将给定字符串转换为字节数组。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>表示字符串的字节数组</returns>
        public static byte[] GetBytes(this string str) => str.GetBytes(Encoding.UTF8);

        /// <summary>
        /// 使用指定的 <paramref name="encoding"/> 将给定字符串转换为字节数组。
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="encoding">要使用的编码</param>
        /// <returns>表示字符串的字节数组</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 或 <paramref name="encoding"/> 为 null 时抛出</exception>
        public static byte[] GetBytes(this string str, Encoding encoding)
        {
            Check.NotNull(str, nameof(str));
            Check.NotNull(encoding, nameof(encoding));

            return encoding.GetBytes(str);
        }

        [GeneratedRegex("[a-z][A-Z]")]
        private static partial Regex MyRegex();
    }
}
