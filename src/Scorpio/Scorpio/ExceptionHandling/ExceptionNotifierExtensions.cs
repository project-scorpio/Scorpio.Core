using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 异常通知器扩展方法类，为 <see cref="IExceptionNotifier"/> 提供便捷的调用方法
    /// </summary>
    /// <remarks>
    /// 该静态类提供了扩展方法，简化了异常通知的调用过程，允许直接传递异常对象
    /// 而无需手动创建 <see cref="ExceptionNotificationContext"/> 实例。
    /// </remarks>
    /// <seealso cref="IExceptionNotifier"/>
    /// <seealso cref="ExceptionNotificationContext"/>
    public static class ExceptionNotifierExtensions
    {
        /// <summary>
        /// 异步通知异常处理，提供简化的调用方式
        /// </summary>
        /// <param name="exceptionNotifier">
        /// 异常通知器实例，用于处理异常通知。不能为 <c>null</c>
        /// </param>
        /// <param name="exception">
        /// 需要通知的异常对象。不能为 <c>null</c>
        /// </param>
        /// <param name="logLevel">
        /// 可选的日志记录级别。如果为 <c>null</c>，将从 <paramref name="exception"/> 
        /// 自动推断适当的日志级别
        /// </param>
        /// <param name="handled">
        /// 指示异常是否已被处理。默认值为 <c>true</c>，表示异常已被处理
        /// </param>
        /// <returns>
        /// 表示异步通知操作的任务对象
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="exceptionNotifier"/> 为 <c>null</c> 时抛出
        /// </exception>
        /// <remarks>
        /// <para>
        /// 此扩展方法简化了异常通知的调用过程，内部会自动创建 
        /// <see cref="ExceptionNotificationContext"/> 实例并调用
        /// <see cref="IExceptionNotifier.NotifyAsync(ExceptionNotificationContext)"/> 方法。
        /// </para>
        /// <para>
        /// 该方法特别适用于需要快速通知异常而不想手动构造上下文对象的场景。
        /// </para>
        /// </remarks>
        /// <seealso cref="IExceptionNotifier.NotifyAsync(ExceptionNotificationContext)"/>
        /// <seealso cref="ExceptionNotificationContext"/>
        public static Task NotifyAsync(
            this IExceptionNotifier exceptionNotifier,
            Exception exception,
            LogLevel? logLevel = null,
            bool handled = true)
        {
            Check.NotNull(exceptionNotifier, nameof(exceptionNotifier));

            // 创建异常通知上下文并调用异常通知器的核心方法
            return exceptionNotifier.NotifyAsync(
                new ExceptionNotificationContext(
                    exception,
                    logLevel,
                    handled
                )
            );
        }
    }
}