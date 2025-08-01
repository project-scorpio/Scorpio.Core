using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Scorpio.DependencyInjection;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 异常通知器，负责将异常通知分发给所有已注册的异常订阅者
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IExceptionNotifier"/> 接口，通过依赖注入容器获取所有
    /// <see cref="IExceptionSubscriber"/> 实现，并逐一调用它们处理异常通知。
    /// 作为瞬态依赖项注册到容器中。
    /// </remarks>
    /// <seealso cref="IExceptionNotifier"/>
    /// <seealso cref="IExceptionSubscriber"/>
    /// <seealso cref="ITransientDependency"/>
    public class ExceptionNotifier : IExceptionNotifier, ITransientDependency
    {
        /// <summary>
        /// 获取或设置日志记录器
        /// </summary>
        /// <value>
        /// 用于记录异常通知过程中的日志信息，默认为空日志记录器实例
        /// </value>
        /// <seealso cref="ILogger{T}"/>
        public ILogger<ExceptionNotifier> Logger { get; set; }

        /// <summary>
        /// 获取混合服务作用域工厂
        /// </summary>
        /// <value>
        /// 用于创建服务作用域以解析异常订阅者实例
        /// </value>
        /// <seealso cref="IHybridServiceScopeFactory"/>
        protected IHybridServiceScopeFactory ServiceScopeFactory { get; }

        /// <summary>
        /// 初始化 <see cref="ExceptionNotifier"/> 类的新实例
        /// </summary>
        /// <param name="serviceScopeFactory">
        /// 混合服务作用域工厂，用于创建依赖注入作用域。不能为 <c>null</c>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="serviceScopeFactory"/> 为 <c>null</c> 时抛出
        /// </exception>
        /// <remarks>
        /// 构造函数会将日志记录器初始化为 <see cref="NullLogger{T}"/> 实例，
        /// 确保在没有注入实际日志记录器时不会出现空引用异常。
        /// </remarks>
        public ExceptionNotifier(IHybridServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            Logger = NullLogger<ExceptionNotifier>.Instance;
        }

        /// <summary>
        /// 异步通知所有异常订阅者处理异常
        /// </summary>
        /// <param name="context">
        /// 异常通知上下文，包含异常信息、日志级别和处理状态等数据。不能为 <c>null</c>
        /// </param>
        /// <returns>
        /// 表示异步操作的任务对象
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="context"/> 为 <c>null</c> 时抛出
        /// </exception>
        /// <remarks>
        /// <para>
        /// 该方法会创建新的服务作用域，从依赖注入容器中获取所有已注册的
        /// <see cref="IExceptionSubscriber"/> 实现，并逐一调用它们的
        /// <see cref="IExceptionSubscriber.HandleAsync"/> 方法。
        /// </para>
        /// <para>
        /// 如果某个异常订阅者在处理过程中抛出异常，该异常会被捕获并记录为警告日志，
        /// 不会影响其他订阅者的执行。
        /// </para>
        /// </remarks>
        /// <seealso cref="ExceptionNotificationContext"/>
        /// <seealso cref="IExceptionSubscriber"/>
        public virtual async Task NotifyAsync(ExceptionNotificationContext context)
        {
            Check.NotNull(context, nameof(context));

            // 创建服务作用域以获取异常订阅者实例
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                // 从依赖注入容器中获取所有异常订阅者
                var exceptionSubscribers = scope.ServiceProvider
                    .GetServices<IExceptionSubscriber>();

                // 逐一通知每个订阅者处理异常
                foreach (var exceptionSubscriber in exceptionSubscribers)
                {
                    try
                    {
                        // 异步调用订阅者的异常处理方法
                        await exceptionSubscriber.HandleAsync(context);
                    }
                    catch (Exception e)
                    {
                        // 记录订阅者处理异常时发生的错误
                        Logger.LogError(e, "Exception subscriber of type {AssemblyQualifiedName} has thrown an exception!", exceptionSubscriber.GetType().AssemblyQualifiedName);
                    }
                }
            }
        }
    }
}
