using System;

using Microsoft.Extensions.DependencyInjection;

using Scorpio;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Scorpio 框架的服务提供程序工厂实现类。
    /// 实现 <see cref="IServiceProviderFactory{TContainerBuilder}"/> 接口，负责在 .NET Generic Host 环境中
    /// 创建和配置 Scorpio 引导程序，并管理应用程序的生命周期。
    /// </summary>
    internal class ServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        /// <summary>
        /// 主机构建器上下文，包含配置信息和托管环境信息
        /// </summary>
        private readonly HostBuilderContext _context;
        
        /// <summary>
        /// 启动模块类型，用于初始化 Scorpio 应用程序
        /// </summary>
        private readonly Type _startupModuleType;
        
        /// <summary>
        /// 引导程序创建选项的配置委托
        /// </summary>
        private readonly Action<BootstrapperCreationOptions> _optionsAction;
        
        /// <summary>
        /// Scorpio 内部引导程序实例，负责应用程序的初始化和生命周期管理
        /// </summary>
        private InternalBootstrapper _bootstrapper;

        /// <summary>
        /// 初始化 <see cref="ServiceProviderFactory"/> 类的新实例。
        /// 使用指定的主机上下文、启动模块类型和配置选项创建服务提供程序工厂。
        /// </summary>
        /// <param name="context">
        /// 主机构建器上下文，包含配置信息和托管环境信息
        /// </param>
        /// <param name="startupModuleType">
        /// 启动模块类型，必须实现 <see cref="Scorpio.Modularity.IScorpioModule"/> 接口
        /// </param>
        /// <param name="optionsAction">
        /// 用于配置 <see cref="BootstrapperCreationOptions"/> 的委托，
        /// 允许自定义引导程序的创建行为
        /// </param>
        public ServiceProviderFactory(HostBuilderContext context, Type startupModuleType, Action<BootstrapperCreationOptions> optionsAction)
        {
            _context = context;
            _startupModuleType = startupModuleType;
            _optionsAction = optionsAction;
        }

        /// <summary>
        /// 创建容器构建器（服务集合）。
        /// 使用指定的服务集合创建 Scorpio 引导程序，并配置托管环境信息。
        /// </summary>
        /// <param name="services">
        /// 要配置的服务集合，包含所有已注册的服务
        /// </param>
        /// <returns>
        /// 配置后的 <see cref="IServiceCollection"/> 实例，
        /// 包含 Scorpio 框架和引导程序的服务注册
        /// </returns>
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            // 创建 Scorpio 内部引导程序实例
            _bootstrapper = new InternalBootstrapper(_startupModuleType, services, _context.Configuration, _optionsAction);
            
            // 将托管环境信息添加到引导程序的共享属性中，供模块使用
            _bootstrapper.Properties["HostingEnvironment"] = _context.HostingEnvironment;
            
            // 将引导程序注册为单例服务，使其可被依赖注入
            services.AddSingleton(_bootstrapper);
            
            return services;
        }

        /// <summary>
        /// 使用容器构建器创建服务提供者。
        /// 完成 Scorpio 应用程序的服务提供者创建、生命周期管理配置和应用程序初始化。
        /// </summary>
        /// <param name="containerBuilder">
        /// 由 <see cref="CreateBuilder(IServiceCollection)"/> 方法返回的服务集合
        /// </param>
        /// <returns>
        /// 完全配置的 <see cref="IServiceProvider"/> 实例，
        /// 可用于应用程序运行时的服务解析
        /// </returns>
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            // 使用引导程序创建服务提供者
            var serviceProvider = _bootstrapper.CreateServiceProvider(containerBuilder);
            
            // 设置引导程序的内部服务提供者引用
            _bootstrapper.SetServiceProviderInternal(serviceProvider);
            
            // 获取应用程序生命周期管理服务
            var applicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            
            // 注册应用程序停止时的清理回调
            // 当应用程序开始停止时，关闭 Scorpio 引导程序
            applicationLifetime.ApplicationStopping.Register(() => _bootstrapper.Shutdown());
            
            // 当应用程序完全停止时，释放引导程序资源
            applicationLifetime.ApplicationStopped.Register(() => (_bootstrapper as IDisposable).Dispose());
            
            // 初始化 Scorpio 应用程序和所有模块
            _bootstrapper.Initialize();
            
            return serviceProvider;
        }
    }
}
