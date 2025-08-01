using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供 <see cref="DateTime"/> 类型的扩展方法。
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 返回自 1970-01-01T00:00:00Z（Unix 纪元）以来经过的秒数。
        /// </summary>
        /// <param name="dateTime">要转换的 <see cref="DateTime"/> 对象</param>
        /// <returns>自 1970-01-01T00:00:00Z 以来经过的秒数。</returns>
        /// <remarks>
        /// 此方法内部使用 <see cref="DateTimeOffset"/> 进行转换，以确保正确处理时区信息。
        /// </remarks>
        public static long ToUnixTimestamp(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }
}
