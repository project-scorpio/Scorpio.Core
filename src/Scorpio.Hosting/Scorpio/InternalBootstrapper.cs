using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scorpio
{
    /// <summary>
    /// 用于托管环境的内部引导程序实现类。
    /// 继承自 <see cref="Bootstrapper"/> 基类，专门为 .NET Generic Host 环境提供的引导程序实现，
    /// 公开了一些内部方法以支持托管环境的服务提供程序工厂模式。
    /// </summary>
    internal class InternalBootstrapper : Bootstrapper
    {
        /// <summary>
        /// 初始化 <see cref="InternalBootstrapper"/> 类的新实例。
        /// 使用指定的启动模块类型、服务集合、配置和选项来创建托管环境专用的引导程序。
        /// </summary>
        /// <param name="startupModuleType">
        /// 启动模块类型，必须实现 <see cref="Scorpio.Modularity.IScorpioModule"/> 接口，
        /// 作为应用程序的入口模块
        /// </param>
        /// <param name="services">
        /// 用于依赖注入的服务集合，包含应用程序的所有服务注册
        /// </param>
        /// <param name="configuration">
        /// 应用程序配置对象，包含来自各种配置源的设置信息，
        /// 可以为 null
        /// </param>
        /// <param name="optionsAction">
        /// 用于配置引导程序创建选项的委托，
        /// 允许自定义引导程序的行为和设置
        /// </param>
        public InternalBootstrapper(Type startupModuleType, IServiceCollection services, IConfiguration configuration, Action<BootstrapperCreationOptions> optionsAction) 
            : base(startupModuleType, services, configuration, optionsAction)
        {
            // 构造函数主体为空，所有初始化逻辑由基类处理
        }

        /// <summary>
        /// 设置引导程序的服务提供程序（内部方法）。
        /// 公开基类的受保护方法，使托管环境的服务提供程序工厂能够设置服务提供程序实例。
        /// </summary>
        /// <param name="serviceProvider">
        /// 已配置的服务提供程序实例，用于应用程序运行时的服务解析
        /// </param>
        internal void SetServiceProviderInternal(IServiceProvider serviceProvider) => SetServiceProvider(serviceProvider);

        /// <summary>
        /// 创建服务提供程序（内部方法）。
        /// 公开基类的受保护方法，使托管环境的服务提供程序工厂能够创建服务提供程序实例。
        /// </summary>
        /// <param name="services">
        /// 包含所有服务注册的服务集合
        /// </param>
        /// <returns>
        /// 配置完成的 <see cref="IServiceProvider"/> 实例，
        /// 可用于运行时的服务解析和依赖注入
        /// </returns>
        internal new IServiceProvider CreateServiceProvider(IServiceCollection services) => base.CreateServiceProvider(services);
    }
}
