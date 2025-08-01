using System;
using System.Globalization;

namespace Scorpio.Localization
{
    /// <summary>
    /// 文化信息帮助类，提供文化设置和操作的实用方法
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该静态类提供了一系列用于管理和操作 <see cref="CultureInfo"/> 的便捷方法，
    /// 包括临时设置文化、验证文化代码、判断文本方向等功能。
    /// </para>
    /// <para>
    /// 主要用于国际化和本地化场景，帮助应用程序正确处理不同地区的文化设置。
    /// </para>
    /// </remarks>
    /// <seealso cref="CultureInfo"/>
    /// <seealso cref="System.Globalization"/>
    public static class CultureHelper
    {
        /// <summary>
        /// 临时设置指定的文化和 UI 文化，并返回用于恢复原始文化的可释放对象
        /// </summary>
        /// <param name="culture">
        /// 要设置的文化代码（如 "en-US", "zh-CN"）。不能为 <c>null</c> 或空字符串
        /// </param>
        /// <param name="uiCulture">
        /// 要设置的 UI 文化代码。如果为 <c>null</c>，则使用 <paramref name="culture"/> 的值
        /// </param>
        /// <returns>
        /// 返回 <see cref="IDisposable"/> 对象，当释放时会自动恢复之前的文化设置
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="culture"/> 为 <c>null</c> 时抛出
        /// </exception>
        /// <exception cref="CultureNotFoundException">
        /// 当指定的文化代码无效时抛出
        /// </exception>
        /// <remarks>
        /// <para>
        /// 该方法采用了"使用即释放"的模式，通常与 <c>using</c> 语句配合使用：
        /// <code>
        /// using (CultureHelper.Use("zh-CN"))
        /// {
        ///     // 在此作用域内使用中文文化设置
        /// }
        /// // 作用域结束后自动恢复原始文化设置
        /// </code>
        /// </para>
        /// </remarks>
        /// <seealso cref="CultureInfo"/>
        /// <seealso cref="Use(CultureInfo, CultureInfo)"/>
        public static IDisposable Use(string culture, string uiCulture = null)
        {
            Check.NotNull(culture, nameof(culture));

            return Use(
                new CultureInfo(culture),
                uiCulture == null
                    ? null
                    : new CultureInfo(uiCulture)
            );
        }

        /// <summary>
        /// 临时设置指定的文化和 UI 文化，并返回用于恢复原始文化的可释放对象
        /// </summary>
        /// <param name="culture">
        /// 要设置的文化信息对象。不能为 <c>null</c>
        /// </param>
        /// <param name="uiCulture">
        /// 要设置的 UI 文化信息对象。如果为 <c>null</c>，则使用 <paramref name="culture"/> 的值
        /// </param>
        /// <returns>
        /// 返回 <see cref="IDisposable"/> 对象，当释放时会自动恢复之前的文化设置
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="culture"/> 为 <c>null</c> 时抛出
        /// </exception>
        /// <remarks>
        /// <para>
        /// 该方法会临时修改 <see cref="CultureInfo.CurrentCulture"/> 和 
        /// <see cref="CultureInfo.CurrentUICulture"/> 的值，并在返回的对象被释放时
        /// 自动恢复原始值。
        /// </para>
        /// <para>
        /// 该方法是线程安全的，支持嵌套调用。
        /// </para>
        /// </remarks>
        /// <seealso cref="CultureInfo.CurrentCulture"/>
        /// <seealso cref="CultureInfo.CurrentUICulture"/>
        public static IDisposable Use(CultureInfo culture, CultureInfo uiCulture = null)
        {
            Check.NotNull(culture, nameof(culture));

            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture ?? culture;

            return new DisposeAction(() =>
            {
                CultureInfo.CurrentCulture = currentCulture;
                CultureInfo.CurrentUICulture = currentUiCulture;
            });
        }

        /// <summary>
        /// 获取一个值，该值指示当前 UI 文化是否为从右到左的文本方向
        /// </summary>
        /// <value>
        /// 如果当前 UI 文化使用从右到左的文本方向，则为 <c>true</c>；否则为 <c>false</c>
        /// </value>
        /// <remarks>
        /// <para>
        /// 该属性通过检查 <see cref="CultureInfo.CurrentUICulture"/> 的 
        /// <see cref="TextInfo.IsRightToLeft"/> 属性来确定文本方向。
        /// </para>
        /// <para>
        /// 主要用于 UI 布局和文本渲染，帮助应用程序正确处理阿拉伯语、希伯来语等
        /// 从右到左书写的语言。
        /// </para>
        /// </remarks>
        /// <seealso cref="TextInfo.IsRightToLeft"/>
        /// <seealso cref="CultureInfo.CurrentUICulture"/>
        public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

        /// <summary>
        /// 验证指定的文化代码是否有效
        /// </summary>
        /// <param name="cultureCode">
        /// 要验证的文化代码（如 "en-US", "zh-CN"）
        /// </param>
        /// <returns>
        /// 如果文化代码有效，则为 <c>true</c>；否则为 <c>false</c>
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法通过尝试调用 <see cref="CultureInfo.GetCultureInfo(string)"/> 来验证
        /// 文化代码的有效性。如果抛出 <see cref="CultureNotFoundException"/> 异常，
        /// 则认为文化代码无效。
        /// </para>
        /// <para>
        /// 对于 <c>null</c>、空字符串或仅包含空白字符的输入，该方法返回 <c>false</c>。
        /// </para>
        /// </remarks>
        /// <seealso cref="CultureInfo.GetCultureInfo(string)"/>
        /// <seealso cref="CultureNotFoundException"/>
        public static bool IsValidCultureCode(string cultureCode)
        {
            if (cultureCode.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                CultureInfo.GetCultureInfo(cultureCode);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// 从完整的文化名称中提取基础文化名称
        /// </summary>
        /// <param name="cultureName">
        /// 完整的文化名称（如 "en-US", "zh-CN"）
        /// </param>
        /// <returns>
        /// 返回基础文化名称（如 "en", "zh"）。如果输入不包含地区信息，则返回原始输入
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法通过查找连字符（"-"）来分离语言代码和地区代码。
        /// 例如，"en-US" 会返回 "en"，"zh-CN" 会返回 "zh"。
        /// </para>
        /// <para>
        /// 如果输入的文化名称不包含连字符，则认为它本身就是基础文化名称，直接返回。
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// string baseCulture1 = CultureHelper.GetBaseCultureName("en-US"); // 返回 "en"
        /// string baseCulture2 = CultureHelper.GetBaseCultureName("zh");    // 返回 "zh"
        /// </code>
        /// </example>
        public static string GetBaseCultureName(string cultureName)
        {
            return cultureName.Contains("-")
                ? cultureName.Left(cultureName.IndexOf("-", StringComparison.Ordinal))
                : cultureName;
        }
    }
}