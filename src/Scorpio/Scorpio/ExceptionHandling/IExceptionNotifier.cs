using System.Threading.Tasks;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 异常通知器接口，定义异常通知的核心契约
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该接口定义了异常通知系统的核心功能，用于将异常信息分发给所有已注册的
    /// 异常订阅者进行处理。实现类负责管理异常订阅者的生命周期和通知流程。
    /// </para>
    /// <para>
    /// 异常通知器采用发布-订阅模式，允许多个 <see cref="IExceptionSubscriber"/> 
    /// 实现同时订阅和处理异常事件，提供了灵活的异常处理机制。
    /// </para>
    /// </remarks>
    /// <seealso cref="IExceptionSubscriber"/>
    /// <seealso cref="ExceptionNotificationContext"/>
    /// <seealso cref="ExceptionNotifier"/>
    public interface IExceptionNotifier
    {
        /// <summary>
        /// 异步通知所有异常订阅者处理指定的异常
        /// </summary>
        /// <param name="context">
        /// 异常通知上下文，包含异常信息、日志级别和处理状态等数据。不能为 <c>null</c>
        /// </param>
        /// <returns>
        /// 表示异步通知操作的任务对象，当所有订阅者处理完成后任务完成
        /// </returns>
        /// <remarks>
        /// <para>
        /// 此方法会将异常通知分发给系统中所有已注册的 <see cref="IExceptionSubscriber"/> 
        /// 实现，每个订阅者都会收到相同的 <paramref name="context"/> 进行处理。
        /// </para>
        /// <para>
        /// 通知过程是异步执行的，即使某个订阅者处理失败，也不会影响其他订阅者的执行。
        /// 实现类应确保所有订阅者都能被正确调用。
        /// </para>
        /// </remarks>
        /// <seealso cref="ExceptionNotificationContext"/>
        /// <seealso cref="IExceptionSubscriber.HandleAsync(ExceptionNotificationContext)"/>
        Task NotifyAsync(ExceptionNotificationContext context);
    }
}