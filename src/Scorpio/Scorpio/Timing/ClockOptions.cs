using System;

namespace Scorpio.Timing
{
    /// <summary>
    /// 时钟配置选项类。
    /// 定义了时钟服务的行为配置，包括时间类型和时区处理方式。
    /// </summary>
    public class ClockOptions
    {
        /// <summary>
        /// 获取或设置时钟的时间类型。
        /// 指示时钟返回的日期时间是本地时间、UTC 时间还是未指定类型。
        /// 默认值：<see cref="DateTimeKind.Unspecified"/>
        /// </summary>
        /// <value>
        /// 一个 <see cref="DateTimeKind"/> 枚举值，用于确定时钟的时间类型：
        /// <list type="bullet">
        /// <item><description><see cref="DateTimeKind.Local"/> - 本地时间</description></item>
        /// <item><description><see cref="DateTimeKind.Utc"/> - UTC 时间</description></item>
        /// <item><description><see cref="DateTimeKind.Unspecified"/> - 未指定时间类型</description></item>
        /// </list>
        /// </value>
        public DateTimeKind Kind { get; set; }

        /// <summary>
        /// 初始化 <see cref="ClockOptions"/> 类的新实例。
        /// 将 <see cref="Kind"/> 属性设置为默认值 <see cref="DateTimeKind.Unspecified"/>。
        /// </summary>
        public ClockOptions() => Kind = DateTimeKind.Unspecified;
    }
}
