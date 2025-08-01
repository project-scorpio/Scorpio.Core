using System;

using Microsoft.Extensions.Options;

using Scorpio.DependencyInjection;

namespace Scorpio.Timing
{
    /// <summary>
    /// 时钟服务的实现类。
    /// 实现 <see cref="IClock"/> 接口，提供可配置的日期时间访问功能，
    /// 支持本地时间和 UTC 时间的处理。
    /// </summary>
    public class Clock : IClock, ITransientDependency
    {
        /// <summary>
        /// 获取时钟配置选项。
        /// 定义了时钟的行为，包括时间类型和时区处理方式。
        /// </summary>
        /// <value>包含时钟配置的 <see cref="ClockOptions"/> 实例</value>
        protected ClockOptions Options { get; }

        /// <summary>
        /// 使用指定的配置选项初始化 <see cref="Clock"/> 类的新实例。
        /// </summary>
        /// <param name="options">时钟配置选项，用于确定时钟的行为</param>
        public Clock(IOptions<ClockOptions> options) => Options = options.Value;

        /// <summary>
        /// 获取当前的日期和时间。
        /// 根据配置的 <see cref="Kind"/> 返回 UTC 时间或本地时间。
        /// </summary>
        /// <value>
        /// 如果 <see cref="Kind"/> 为 <see cref="DateTimeKind.Utc"/>，则返回 <see cref="DateTime.UtcNow"/>；
        /// 否则返回 <see cref="DateTime.Now"/>
        /// </value>
        public virtual DateTime Now => Options.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

        /// <summary>
        /// 获取时钟的时间类型。
        /// 指示时钟返回的日期时间是本地时间、UTC 时间还是未指定类型。
        /// </summary>
        /// <value>配置的 <see cref="DateTimeKind"/> 值</value>
        public virtual DateTimeKind Kind => Options.Kind;

        /// <summary>
        /// 获取一个值，该值指示时钟是否支持多时区操作。
        /// 只有当时钟配置为 UTC 时间时才支持多时区。
        /// </summary>
        /// <value>
        /// 如果 <see cref="Kind"/> 为 <see cref="DateTimeKind.Utc"/> 则为 true；
        /// 否则为 false
        /// </value>
        public virtual bool SupportsMultipleTimezone => Options.Kind == DateTimeKind.Utc;

        /// <summary>
        /// 将给定的日期时间规范化为时钟的时间类型。
        /// 根据时钟的配置和输入日期时间的类型进行适当的时区转换。
        /// </summary>
        /// <param name="dateTime">要规范化的日期时间</param>
        /// <returns>规范化后的日期时间，其 <see cref="DateTime.Kind"/> 与时钟的 <see cref="Kind"/> 匹配</returns>
        public virtual DateTime Normalize(DateTime dateTime)
        {
            // 如果时钟类型为未指定或与输入的日期时间类型相同，直接返回
            if (Kind == DateTimeKind.Unspecified || Kind == dateTime.Kind)
            {
                return dateTime;
            }

            // 如果时钟为本地时间且输入为 UTC 时间，转换为本地时间
            if (Kind == DateTimeKind.Local && dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }

            // 如果时钟为 UTC 时间且输入为本地时间，转换为 UTC 时间
            if (Kind == DateTimeKind.Utc && dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            // 其他情况下，仅更改日期时间的 Kind 属性
            return DateTime.SpecifyKind(dateTime, Kind);
        }
    }
}
