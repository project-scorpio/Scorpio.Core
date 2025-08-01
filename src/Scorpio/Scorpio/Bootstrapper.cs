using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Scorpio.DependencyInjection;
using Scorpio.Localization;
using Scorpio.Modularity;

namespace Scorpio
{
    /// <summary>
    /// 应用程序引导程序基类，负责初始化模块系统和应用程序。
    /// 实现 <see cref="IBootstrapper"/>、<see cref="IModuleContainer"/> 和 <see cref="IServiceProviderAccessor"/> 接口，
    /// 提供完整的应用程序生命周期管理功能。
    /// </summary>
    public abstract class Bootstrapper : IBootstrapper, IModuleContainer, IServiceProviderAccessor
    {
        /// <summary>
        /// 指示应用程序是否已关闭的标志
        /// </summary>
        private bool _isShutdown = false;
        
        /// <summary>
        /// 服务工厂适配器的延迟初始化实例
        /// </summary>
        private readonly Lazy<IServiceFactoryAdapter> _serviceFactory;

        /// <summary>
        /// 引导程序创建时的配置选项
        /// </summary>
        private readonly BootstrapperCreationOptions _options;

        /// <summary>
        /// 获取应用程序启动模块类型。
        /// 这是应用程序的入口模块，定义了应用程序的基本配置和依赖关系。
        /// </summary>
        /// <value>实现 <see cref="IScorpioModule"/> 接口的启动模块类型</value>
        public Type StartupModuleType { get; }

        /// <summary>
        /// 获取服务集合，包含应用程序所有注册的服务。
        /// 用于依赖注入容器的服务注册和配置。
        /// </summary>
        /// <value>包含所有已注册服务的 <see cref="IServiceCollection"/> 实例</value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取应用程序合并后的配置对象。
        /// 包含来自各种配置源的合并配置信息。
        /// </summary>
        /// <value>应用程序的 <see cref="IConfiguration"/> 配置实例</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 获取用于在主机构建过程中在组件之间共享状态的中心位置。
        /// 提供键值对存储，用于模块间的数据共享和通信。
        /// </summary>
        /// <value>用于存储共享属性的字典</value>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// 获取应用程序的服务提供者，用于解析服务实例。
        /// 在应用程序初始化完成后可用于获取已注册的服务。
        /// </summary>
        /// <value>用于服务解析的 <see cref="IServiceProvider"/> 实例</value>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 获取应用程序加载的所有模块的描述符列表。
        /// 包含模块的元数据信息和实例引用。
        /// </summary>
        /// <value>模块描述符的只读列表</value>
        public IReadOnlyList<IModuleDescriptor> Modules { get; }

        /// <summary>
        /// 获取模块加载器，负责加载应用程序模块。
        /// 处理模块的发现、依赖解析和加载顺序。
        /// </summary>
        protected internal IModuleLoader ModuleLoader { get; }

        /// <summary>
        /// 获取服务工厂适配器，用于创建服务容器和提供者。
        /// 支持不同的依赖注入容器实现。
        /// </summary>
        internal IServiceFactoryAdapter ServiceFactoryAdapter => _serviceFactory.Value;

        /// <summary>
        /// 创建服务提供者。
        /// 使用配置的服务工厂适配器来创建依赖注入容器。
        /// </summary>
        /// <param name="services">包含所有服务注册的服务集合</param>
        /// <returns>配置完成的 <see cref="IServiceProvider"/> 实例</returns>
        protected IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var containerBuilder = ServiceFactoryAdapter.CreateBuilder(services);

            return ServiceFactoryAdapter.CreateServiceProvider(containerBuilder);
        }

        /// <summary>
        /// 初始化引导程序的新实例。
        /// 执行完整的引导程序初始化流程，包括模块加载和服务配置。
        /// </summary>
        /// <param name="startupModuleType">启动模块类型，必须实现 <see cref="IScorpioModule"/> 接口</param>
        /// <param name="services">用于依赖注入的服务集合</param>
        /// <param name="configuration">应用程序配置对象，可以为 null</param>
        /// <param name="optionsAction">用于配置引导程序创建选项的委托</param>
        protected Bootstrapper(Type startupModuleType, IServiceCollection services, IConfiguration configuration, Action<BootstrapperCreationOptions> optionsAction)
        {
            Services = services;
            StartupModuleType = startupModuleType;
            Properties = new Dictionary<string, object>();
            _options = new BootstrapperCreationOptions();
            ModuleLoader = new ModuleLoader();
            optionsAction(_options);
            _serviceFactory = new Lazy<IServiceFactoryAdapter>(() => _options.ServiceFactory());
            var configBuilder = new ConfigurationBuilder();
            if (configuration != null)
            {
                configBuilder.AddConfiguration(configuration);
            }
            _options.ConfigureConfiguration(configBuilder);
            Configuration = configBuilder.Build();
            ConfigureCoreService(services);
            Modules = LoadModules();
            ConfigureServices();
        }

        /// <summary>
        /// 配置核心服务。
        /// 注册引导程序运行所需的基础服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        private void ConfigureCoreService(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<IBootstrapper>(this);
            services.AddSingleton<IModuleContainer>(this);
            services.AddSingleton(ModuleLoader);
            services.AddSingleton<IModuleManager, ModuleManager>();
        }

        /// <summary>
        /// 配置应用程序服务。
        /// 按照预配置、配置、后配置的顺序执行模块的服务配置。
        /// </summary>
        private void ConfigureServices()
        {
            var context = new ConfigureServicesContext(this, Services, Configuration);
            Services.AddSingleton(context);
            // 预配置阶段
            Modules.ForEach(m => m.Instance.PreConfigureServices(context));
            _options.PreConfigureServices(context);
            // 配置阶段
            Modules.ForEach(m =>
            {
                if (m.Instance is ScorpioModule module && !module.SkipAutoServiceRegistration)
                {
                    Services.RegisterAssemblyByConvention(m.Type.Assembly);
                }
                m.Instance.ConfigureServices(context);
            });
            _options.ConfigureServices(context);
            // 后配置阶段
            Modules.ForEach(m => m.Instance.PostConfigureServices(context));
            _options.PostConfigureServices(context);
        }

        /// <summary>
        /// 加载应用程序模块。
        /// 使用模块加载器发现和加载所有相关模块。
        /// </summary>
        /// <returns>加载的模块描述符列表</returns>
        private IReadOnlyList<IModuleDescriptor> LoadModules() => ModuleLoader.LoadModules(Services, StartupModuleType, _options.PlugInSources);

        /// <summary>
        /// 设置服务提供者。
        /// 在引导程序初始化完成后设置服务提供者实例。
        /// </summary>
        /// <param name="serviceProvider">已配置的服务提供者实例</param>
        protected internal void SetServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

        /// <summary>
        /// 初始化应用程序。
        /// 启动所有已加载的模块并完成应用程序初始化过程。
        /// </summary>
        /// <param name="initializeParams">传递给模块初始化方法的参数数组</param>
        public virtual void Initialize(params object[] initializeParams) => InitializeModules(initializeParams);

        /// <summary>
        /// 初始化应用程序模块。
        /// 按照正确的顺序初始化所有已加载的模块。
        /// </summary>
        /// <param name="initializeParams">传递给模块初始化方法的参数数组</param>
        protected virtual void InitializeModules(params object[] initializeParams)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<IModuleManager>()
                    .InitializeModules(new ApplicationInitializationContext(scope.ServiceProvider, initializeParams));
            }
        }

        /// <summary>
        /// 关闭应用程序。
        /// 按照相反的顺序关闭所有模块并清理资源。
        /// </summary>
        public virtual void Shutdown()
        {
            if (_isShutdown)
            {
                return;
            }
            _isShutdown = true;
            using (var scope = ServiceProvider.CreateScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<IModuleManager>()
                    .ShutdownModules(new ApplicationShutdownContext(scope.ServiceProvider));
            }
        }

        /// <summary>
        /// 创建引导程序实例。
        /// 使用泛型参数指定启动模块类型。
        /// </summary>
        /// <typeparam name="TStartupModule">启动模块类型，必须实现 <see cref="IScorpioModule"/> 接口</typeparam>
        /// <param name="optionsAction">用于配置引导程序创建选项的委托</param>
        /// <returns>已配置的 <see cref="IBootstrapper"/> 实例</returns>
        public static IBootstrapper Create<TStartupModule>(Action<BootstrapperCreationOptions> optionsAction) where TStartupModule : IScorpioModule => Create(typeof(TStartupModule), optionsAction);

        /// <summary>
        /// 创建引导程序实例。
        /// 使用指定的启动模块类型和配置选项。
        /// </summary>
        /// <param name="startupModuleType">启动模块类型，必须实现 <see cref="IScorpioModule"/> 接口</param>
        /// <param name="optionsAction">用于配置引导程序创建选项的委托</param>
        /// <returns>已配置的 <see cref="IBootstrapper"/> 实例</returns>
        /// <exception cref="ArgumentException">当 <paramref name="startupModuleType"/> 不实现 <see cref="IScorpioModule"/> 接口时抛出</exception>
        public static IBootstrapper Create(Type startupModuleType, Action<BootstrapperCreationOptions> optionsAction)
        {
            if (!startupModuleType.IsAssignableTo<IScorpioModule>())
            {
                throw new ArgumentException($"{nameof(startupModuleType)} should be derived from {typeof(IScorpioModule)}");
            }
            var services = new ServiceCollection();
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.Build();
            var bootstrapper = new InternalBootstrapper(startupModuleType, services, config, optionsAction);
            bootstrapper.SetServiceProvider(bootstrapper.CreateServiceProvider(services));
            return bootstrapper;
        }

        /// <summary>
        /// 使用默认配置创建引导程序实例。
        /// </summary>
        /// <param name="startupModuleType">启动模块类型，必须实现 <see cref="IScorpioModule"/> 接口</param>
        /// <returns>使用默认配置的 <see cref="IBootstrapper"/> 实例</returns>
        public static IBootstrapper Create(Type startupModuleType) => Create(startupModuleType, o => { });

        /// <summary>
        /// 使用默认配置创建引导程序实例。
        /// 使用泛型参数指定启动模块类型。
        /// </summary>
        /// <typeparam name="TStartupModule">启动模块类型，必须实现 <see cref="IScorpioModule"/> 接口</typeparam>
        /// <returns>使用默认配置的 <see cref="IBootstrapper"/> 实例</returns>
        public static IBootstrapper Create<TStartupModule>() where TStartupModule : IScorpioModule => Create(typeof(TStartupModule));

        #region IDisposable Support
        /// <summary>
        /// 指示对象是否已被释放的标志，用于检测重复释放调用
        /// </summary>
        private bool _disposedValue = false;

        /// <summary>
        /// 释放 <see cref="Bootstrapper"/> 使用的资源。
        /// </summary>
        /// <param name="disposing">
        /// 如果为 true，则释放托管和非托管资源；
        /// 如果为 false，则仅释放非托管资源
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // 释放托管资源：关闭应用程序
                    Shutdown();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// 释放当前 <see cref="Bootstrapper"/> 实例使用的所有资源。
        /// 实现 <see cref="IDisposable"/> 接口。
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的Dispose(bool disposing)中
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
