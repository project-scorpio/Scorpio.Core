using System.Runtime.ExceptionServices;

using Microsoft.Extensions.Logging;

using Scorpio.Logging;

namespace System
{
    /// <summary>
    /// <see cref="Exception"/> 类的扩展方法集合。
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 使用 <see cref="ExceptionDispatchInfo.Capture"/> 方法重新抛出异常，
        /// 同时保留原始的堆栈跟踪信息。
        /// </summary>
        /// <param name="exception">要重新抛出的异常</param>
        public static void ReThrow(this Exception exception) => ExceptionDispatchInfo.Capture(exception).Throw();

        /// <summary>
        /// 尝试从给定的 <paramref name="exception"/> 获取日志级别，
        /// 如果异常实现了 <see cref="IHasLogLevel"/> 接口。
        /// 否则，返回 <paramref name="defaultLevel"/>。
        /// </summary>
        /// <param name="exception">要检查的异常</param>
        /// <param name="defaultLevel">如果异常未指定日志级别，则使用的默认日志级别</param>
        /// <returns>异常的日志级别或默认级别</returns>
        public static LogLevel GetLogLevel(this Exception exception, LogLevel defaultLevel = LogLevel.Error) => (exception as IHasLogLevel)?.LogLevel ?? defaultLevel;
    }
}
