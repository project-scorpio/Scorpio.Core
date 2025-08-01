using System;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Scorpio.DependencyInjection;

namespace Scorpio.Localization
{
    /// <summary>
    /// 本地化上下文类，提供基于异步本地存储的本地化服务访问
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类实现了 <see cref="IServiceProviderAccessor"/> 接口，用于在异步上下文中
    /// 管理和访问本地化相关的服务。通过 <see cref="AsyncLocal{T}"/> 确保在异步
    /// 操作中维持正确的本地化上下文。
    /// </para>
    /// <para>
    /// 提供了类似于 CallContext 的功能，但专门针对异步编程模型进行了优化，
    /// 确保本地化上下文能够正确地在异步调用链中传递。
    /// </para>
    /// </remarks>
    /// <seealso cref="IServiceProviderAccessor"/>
    /// <seealso cref="IStringLocalizerFactory"/>
    /// <seealso cref="AsyncLocal{T}"/>
    public class LocalizationContext : IServiceProviderAccessor
    {
        /// <summary>
        /// 异步本地存储，用于在异步上下文中维护当前的本地化上下文
        /// </summary>
        /// <remarks>
        /// 使用 <see cref="AsyncLocal{T}"/> 确保本地化上下文能够正确地
        /// 在异步操作中传递，而不会受到线程切换的影响。
        /// </remarks>
        private static readonly AsyncLocal<LocalizationContext> _currnetContext = new AsyncLocal<LocalizationContext>();

        /// <summary>
        /// 临时设置指定的本地化上下文，并返回用于恢复原始上下文的可释放对象
        /// </summary>
        /// <param name="context">
        /// 要设置为当前上下文的本地化上下文实例。可以为 <c>null</c>
        /// </param>
        /// <returns>
        /// 返回 <see cref="IDisposable"/> 对象，当释放时会自动恢复之前的上下文
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法采用了"使用即释放"的模式，通常与 <c>using</c> 语句配合使用：
        /// <code>
        /// using (LocalizationContext.Use(newContext))
        /// {
        ///     // 在此作用域内使用新的本地化上下文
        /// }
        /// // 作用域结束后自动恢复原始上下文
        /// </code>
        /// </para>
        /// <para>
        /// 该方法是线程安全的，支持嵌套调用。
        /// </para>
        /// </remarks>
        /// <seealso cref="IDisposable"/>
        /// <seealso cref="DisposeAction"/>
        public static IDisposable Use(LocalizationContext context)
        {
            var current = _currnetContext.Value;
            _currnetContext.Value = context;
            return new DisposeAction(() => _currnetContext.Value = current);
        }

        /// <summary>
        /// 获取当前异步上下文中的本地化上下文实例
        /// </summary>
        /// <value>
        /// 返回当前的 <see cref="LocalizationContext"/> 实例，如果未设置则为 <c>null</c>
        /// </value>
        /// <remarks>
        /// 该属性通过 <see cref="AsyncLocal{T}"/> 获取当前异步上下文中的本地化上下文，
        /// 确保在异步操作中能够正确访问当前的本地化设置。
        /// </remarks>
        public static LocalizationContext Current => _currnetContext.Value;

        /// <summary>
        /// 获取服务提供者实例
        /// </summary>
        /// <value>
        /// 用于解析依赖注入容器中服务实例的服务提供者
        /// </value>
        /// <remarks>
        /// 通过该属性可以访问依赖注入容器中的各种服务，
        /// 为本地化功能提供必要的依赖项支持。
        /// </remarks>
        /// <seealso cref="IServiceProvider"/>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 获取字符串本地化工厂实例
        /// </summary>
        /// <value>
        /// 用于创建字符串本地化器的工厂实例
        /// </value>
        /// <remarks>
        /// 该工厂用于创建各种类型的 <see cref="IStringLocalizer"/> 实例，
        /// 为应用程序提供字符串本地化功能。
        /// </remarks>
        /// <seealso cref="IStringLocalizerFactory"/>
        /// <seealso cref="IStringLocalizer"/>
        public IStringLocalizerFactory LocalizerFactory { get; }

        /// <summary>
        /// 初始化 <see cref="LocalizationContext"/> 类的新实例
        /// </summary>
        /// <param name="serviceProvider">
        /// 服务提供者实例，用于解析依赖注入容器中的服务。不能为 <c>null</c>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="serviceProvider"/> 为 <c>null</c> 时抛出
        /// </exception>
        /// <remarks>
        /// <para>
        /// 构造函数会自动从服务提供者中获取 <see cref="IStringLocalizerFactory"/> 服务，
        /// 如果该服务未注册将抛出异常。
        /// </para>
        /// <para>
        /// 通常由依赖注入容器自动创建实例，无需手动调用。
        /// </para>
        /// </remarks>
        public LocalizationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = Check.NotNull(serviceProvider, nameof(serviceProvider));
            LocalizerFactory = ServiceProvider.GetRequiredService<IStringLocalizerFactory>();
        }
    }
}