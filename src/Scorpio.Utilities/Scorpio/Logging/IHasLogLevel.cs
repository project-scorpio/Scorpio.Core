using Microsoft.Extensions.Logging;

namespace Scorpio.Logging
{
    /// <summary>
    /// 定义具有日志级别属性的接口
    /// </summary>
    /// <remarks>
    /// 此接口用于标识那些需要指定日志记录级别的类型。
    /// 实现此接口的类可以控制其日志输出的严重程度级别。
    /// </remarks>
    /// <seealso cref="LogLevel"/>
    public interface IHasLogLevel
    {
        /// <summary>
        /// 获取或设置日志记录的严重程度级别
        /// </summary>
        /// <value>
        /// <see cref="LogLevel"/> 枚举值，用于指定日志消息的重要性和严重程度
        /// </value>
        /// <remarks>
        /// 日志级别用于控制日志输出的详细程度，常见级别包括：
        /// <list type="bullet">
        /// <item><description><see cref="LogLevel.Trace"/> - 最详细的日志信息</description></item>
        /// <item><description><see cref="LogLevel.Debug"/> - 调试信息</description></item>
        /// <item><description><see cref="LogLevel.Information"/> - 一般信息</description></item>
        /// <item><description><see cref="LogLevel.Warning"/> - 警告信息</description></item>
        /// <item><description><see cref="LogLevel.Error"/> - 错误信息</description></item>
        /// <item><description><see cref="LogLevel.Critical"/> - 严重错误信息</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="LogLevel"/>
        LogLevel LogLevel { get; set; }
    }
}
