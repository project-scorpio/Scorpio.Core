using System;

namespace Scorpio.Timing
{
    /// <summary>
    /// 定义时钟服务的接口。
    /// 提供日期时间访问、时区处理和时间规范化功能。
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// 获取当前的日期和时间。
        /// 根据时钟的配置返回相应时区的当前时间。
        /// </summary>
        /// <value>当前的日期和时间</value>
        DateTime Now { get; }

        /// <summary>
        /// 获取时钟的时间类型。
        /// 指示时钟返回的日期时间是本地时间、UTC 时间还是未指定类型。
        /// </summary>
        /// <value>
        /// 一个 <see cref="DateTimeKind"/> 枚举值，表示时钟的时间类型
        /// </value>
        DateTimeKind Kind { get; }

        /// <summary>
        /// 获取一个值，该值指示时钟是否支持多时区操作。
        /// 通常只有 UTC 时钟才支持多时区处理。
        /// </summary>
        /// <value>
        /// 如果时钟支持多时区操作则为 true；否则为 false
        /// </value>
        bool SupportsMultipleTimezone { get; }

        /// <summary>
        /// 将给定的日期时间规范化为时钟的时间类型。
        /// 根据时钟的配置进行必要的时区转换，确保返回的日期时间与时钟的时间类型一致。
        /// </summary>
        /// <param name="dateTime">要规范化的日期时间</param>
        /// <returns>
        /// 规范化后的日期时间，其 <see cref="DateTime.Kind"/> 与时钟的 <see cref="Kind"/> 匹配
        /// </returns>
        DateTime Normalize(DateTime dateTime);
    }
}
