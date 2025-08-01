using System;

using Microsoft.Extensions.Logging;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 异常通知上下文类，封装异常处理过程中的相关信息
    /// </summary>
    /// <remarks>
    /// 该类用于在异常处理流程中传递异常信息、日志级别和处理状态等数据，
    /// 为异常通知处理器提供完整的上下文信息。
    /// </remarks>
    public class ExceptionNotificationContext
    {
        /// <summary>
        /// 获取异常对象
        /// </summary>
        /// <value>
        /// 表示发生的异常实例
        /// </value>
        public Exception Exception { get; }

        /// <summary>
        /// 获取日志记录级别
        /// </summary>
        /// <value>
        /// 用于记录此异常的日志级别，如果构造时未指定则从异常对象自动推断
        /// </value>
        /// <seealso cref="Microsoft.Extensions.Logging.LogLevel"/>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// 获取异常是否已被处理的标识
        /// </summary>
        /// <value>
        /// 如果为 <c>true</c>，表示异常已被处理；否则表示异常未被处理
        /// </value>
        public bool Handled { get; }

        /// <summary>
        /// 初始化 <see cref="ExceptionNotificationContext"/> 类的新实例
        /// </summary>
        /// <param name="exception">发生的异常对象，不能为 <c>null</c></param>
        /// <param name="logLevel">
        /// 可选的日志记录级别。如果为 <c>null</c>，将通过 <paramref name="exception"/> 
        /// 调用扩展方法自动确定适当的日志级别
        /// </param>
        /// <param name="handled">
        /// 指示异常是否已被处理。默认值为 <c>true</c>，表示异常已被处理
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="exception"/> 为 <c>null</c> 时抛出
        /// </exception>
        public ExceptionNotificationContext(
            Exception exception,
            LogLevel? logLevel = null,
            bool handled = true)
        {
            Exception = Check.NotNull(exception, nameof(exception));
            LogLevel = logLevel ?? exception.GetLogLevel();
            Handled = handled;
        }
    }
}