using System.Threading.Tasks;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 异常订阅者接口，定义异常处理的核心契约
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该接口定义了异常订阅者的基本功能，用于接收和处理来自 <see cref="IExceptionNotifier"/> 
    /// 的异常通知。实现此接口的类会被自动注册到依赖注入容器中，并在异常发生时被调用。
    /// </para>
    /// <para>
    /// 异常订阅者采用异步处理模式，允许执行耗时的异常处理操作（如发送邮件、记录日志、
    /// 上报监控系统等）而不阻塞主线程。
    /// </para>
    /// </remarks>
    /// <seealso cref="IExceptionNotifier"/>
    /// <seealso cref="ExceptionNotificationContext"/>
    /// <seealso cref="ExceptionSubscriber"/>
    public interface IExceptionSubscriber
    {
        /// <summary>
        /// 异步处理异常通知
        /// </summary>
        /// <param name="context">
        /// 异常通知上下文，包含异常信息、日志级别和处理状态等数据。不能为 <c>null</c>
        /// </param>
        /// <returns>
        /// 表示异步处理操作的任务对象，当异常处理完成后任务完成
        /// </returns>
        /// <remarks>
        /// <para>
        /// 实现此方法时应根据 <paramref name="context"/> 中的异常信息进行相应的处理，
        /// 例如记录日志、发送通知、清理资源等操作。
        /// </para>
        /// <para>
        /// 处理过程中应注意：
        /// <list type="bullet">
        /// <item><description>避免在处理过程中抛出异常，否则会影响其他订阅者的执行</description></item>
        /// <item><description>可以通过 <see cref="ExceptionNotificationContext.LogLevel"/> 判断异常的严重程度</description></item>
        /// <item><description>可以通过 <see cref="ExceptionNotificationContext.Handled"/> 判断异常是否已被处理</description></item>
        /// <item><description>处理操作应该是幂等的，以防止重复处理同一异常</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="ExceptionNotificationContext"/>
        /// <seealso cref="IExceptionNotifier.NotifyAsync(ExceptionNotificationContext)"/>
        Task HandleAsync(ExceptionNotificationContext context);
    }
}
