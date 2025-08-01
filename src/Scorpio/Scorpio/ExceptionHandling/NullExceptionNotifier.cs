using System.Threading.Tasks;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 空异常通知器实现，提供无操作的异常通知功能
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类实现了 <see cref="IExceptionNotifier"/> 接口，但不执行任何实际的通知操作。
    /// 采用空对象模式（Null Object Pattern），用于在不需要异常通知或测试场景中
    /// 替代真实的异常通知器实现。
    /// </para>
    /// <para>
    /// 使用单例模式确保整个应用程序中只有一个实例，减少内存开销。
    /// 适用于禁用异常通知、单元测试或性能敏感的场景。
    /// </para>
    /// </remarks>
    /// <seealso cref="IExceptionNotifier"/>
    /// <seealso cref="ExceptionNotificationContext"/>
    /// <seealso cref="ExceptionNotifier"/>
    public class NullExceptionNotifier : IExceptionNotifier
    {
        /// <summary>
        /// 获取 <see cref="NullExceptionNotifier"/> 类的单例实例
        /// </summary>
        /// <value>
        /// 返回全局唯一的空异常通知器实例
        /// </value>
        /// <remarks>
        /// 该属性提供了线程安全的单例访问，确保在整个应用程序生命周期中
        /// 只存在一个 <see cref="NullExceptionNotifier"/> 实例。
        /// </remarks>
        public static NullExceptionNotifier Instance { get; } = new NullExceptionNotifier();

        /// <summary>
        /// 初始化 <see cref="NullExceptionNotifier"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 私有构造函数确保只能通过 <see cref="Instance"/> 属性访问唯一实例，
        /// 防止外部代码创建多个实例。
        /// </remarks>
        private NullExceptionNotifier()
        {
            // 空构造函数，无需初始化任何资源
        }

        /// <summary>
        /// 异步通知异常处理（空实现）
        /// </summary>
        /// <param name="context">
        /// 异常通知上下文，包含异常信息、日志级别和处理状态等数据。
        /// 在此实现中该参数被忽略
        /// </param>
        /// <returns>
        /// 返回已完成的任务对象，表示通知操作立即完成
        /// </returns>
        /// <remarks>
        /// <para>
        /// 此方法实现了 <see cref="IExceptionNotifier.NotifyAsync(ExceptionNotificationContext)"/> 
        /// 接口，但不执行任何实际操作。直接返回 <see cref="Task.CompletedTask"/>，
        /// 表示通知操作已完成。
        /// </para>
        /// <para>
        /// 适用于需要禁用异常通知或在测试环境中避免副作用的场景。
        /// </para>
        /// </remarks>
        /// <seealso cref="IExceptionNotifier.NotifyAsync(ExceptionNotificationContext)"/>
        /// <seealso cref="Task.CompletedTask"/>
        public Task NotifyAsync(ExceptionNotificationContext context) => Task.CompletedTask;
    }
}