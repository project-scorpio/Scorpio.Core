using System;

using Scorpio.DependencyInjection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 应用程序关闭上下文类，用于在模块关闭过程中传递必要的上下文信息
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IServiceProviderAccessor"/> 接口，提供对服务提供程序的访问。
    /// 在应用程序和模块的关闭阶段，此上下文对象会被传递给各个模块，
    /// 使模块能够在关闭时访问依赖注入容器中的服务，执行必要的清理操作。
    /// </remarks>
    public class ApplicationShutdownContext : IServiceProviderAccessor
    {
        /// <summary>
        /// 获取服务提供程序，用于访问依赖注入容器中的服务
        /// </summary>
        /// <value>
        /// 提供对应用程序依赖注入容器的访问，模块可通过此属性在关闭时获取所需的服务实例，
        /// 以便执行资源释放、状态保存等清理操作
        /// </value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 初始化 <see cref="ApplicationShutdownContext"/> 类的新实例
        /// </summary>
        /// <param name="serviceProvider">服务提供程序，用于访问依赖注入容器</param>
        /// <remarks>
        /// 该构造函数为内部使用，通常由应用程序框架在关闭阶段创建上下文实例。
        /// 关闭上下文相对简单，仅包含服务提供程序，因为关闭操作通常不需要额外的参数。
        /// </remarks>
        internal ApplicationShutdownContext(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    }
}