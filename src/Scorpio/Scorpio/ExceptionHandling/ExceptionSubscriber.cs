using System.Threading.Tasks;

using Scorpio.DependencyInjection;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 异常订阅者抽象基类，为异常处理提供标准化的实现模板
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该抽象类实现了 <see cref="IExceptionSubscriber"/> 接口，并自动注册为瞬态依赖项。
    /// 继承此类的具体实现会被自动发现并注册到依赖注入容器中。
    /// </para>
    /// <para>
    /// 通过 <see cref="ExposeServicesAttribute"/> 特性，确保派生类会被正确地
    /// 注册为 <see cref="IExceptionSubscriber"/> 服务类型。
    /// </para>
    /// </remarks>
    /// <seealso cref="IExceptionSubscriber"/>
    /// <seealso cref="ITransientDependency"/>
    /// <seealso cref="ExposeServicesAttribute"/>
    [ExposeServices(typeof(IExceptionSubscriber))]
    public abstract class ExceptionSubscriber : IExceptionSubscriber, ITransientDependency
    {
        /// <summary>
        /// 异步处理异常通知的抽象方法
        /// </summary>
        /// <param name="context">
        /// 异常通知上下文，包含异常信息、日志级别和处理状态等数据。不能为 <c>null</c>
        /// </param>
        /// <returns>
        /// 表示异步处理操作的任务对象
        /// </returns>
        /// <remarks>
        /// <para>
        /// 派生类必须实现此方法来定义具体的异常处理逻辑。该方法会被
        /// <see cref="IExceptionNotifier"/> 在异常发生时自动调用。
        /// </para>
        /// <para>
        /// 实现此方法时应注意：
        /// <list type="bullet">
        /// <item><description>处理过程中不应抛出异常，否则会影响其他订阅者的执行</description></item>
        /// <item><description>应根据 <paramref name="context"/> 中的信息进行相应的处理</description></item>
        /// <item><description>可以通过 <see cref="ExceptionNotificationContext.Handled"/> 判断异常是否已被处理</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="ExceptionNotificationContext"/>
        /// <seealso cref="IExceptionNotifier"/>
        public virtual Task HandleAsync(ExceptionNotificationContext context)
        {
            Check.NotNull(context, nameof(context));

            // 默认实现不做任何处理，派生类应覆盖此方法以实现具体逻辑
            return Task.CompletedTask;
        }
    }
}