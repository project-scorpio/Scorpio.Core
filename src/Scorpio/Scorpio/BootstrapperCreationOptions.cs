using System;
using System.Collections.Generic;
using System.Runtime.Loader;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using Scorpio.Modularity;
using Scorpio.Modularity.Plugins;

namespace Scorpio
{
    /// <summary>
    /// 引导程序创建选项类。
    /// 提供配置引导程序创建过程的各种选项，包括服务配置、插件源和依赖注入容器工厂的设置。
    /// </summary>
    public sealed class BootstrapperCreationOptions
    {
        /// <summary>
        /// 存储预配置服务操作的集合。
        /// 这些操作将在服务配置的第一阶段执行。
        /// </summary>
        private readonly HashSet<Action<ConfigureServicesContext>> _preConfigureServiceActions = new HashSet<Action<ConfigureServicesContext>>();

        /// <summary>
        /// 存储服务配置操作的集合。
        /// 这些操作将在服务配置的主要阶段执行。
        /// </summary>
        private readonly HashSet<Action<ConfigureServicesContext>> _configureServiceActions = new HashSet<Action<ConfigureServicesContext>>();

        /// <summary>
        /// 存储后配置服务操作的集合。
        /// 这些操作将在服务配置的最后阶段执行。
        /// </summary>
        private readonly HashSet<Action<ConfigureServicesContext>> _postConfigureServiceActions = new HashSet<Action<ConfigureServicesContext>>();

        /// <summary>
        /// 存储配置构建操作的集合。
        /// 这些操作用于配置应用程序的 <see cref="IConfigurationBuilder"/>。
        /// </summary>
        private readonly ICollection<Action<IConfigurationBuilder>> _configurationActions = new HashSet<Action<IConfigurationBuilder>>();

        /// <summary>
        /// 获取插件源列表，用于加载外部插件模块。
        /// 定义了从哪些位置和程序集加载上下文中搜索插件。
        /// </summary>
        /// <value>包含所有插件源的 <see cref="IPlugInSourceList"/> 实例</value>
        public IPlugInSourceList PlugInSources { get; }

        /// <summary>
        /// 获取或设置服务工厂适配器的创建函数。
        /// 默认使用 <see cref="DefaultServiceProviderFactory"/> 创建标准的依赖注入容器。
        /// </summary>
        /// <value>返回 <see cref="IServiceFactoryAdapter"/> 实例的函数</value>
        internal Func<IServiceFactoryAdapter> ServiceFactory { get; set; } = () => new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

        /// <summary>
        /// 初始化 <see cref="BootstrapperCreationOptions"/> 类的新实例。
        /// 使用应用程序基目录的物理文件提供程序和默认程序集加载上下文初始化插件源。
        /// </summary>
        internal BootstrapperCreationOptions() => PlugInSources = new PlugInSourceList(new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory), AssemblyLoadContext.Default);

        /// <summary>
        /// 添加预配置服务操作。
        /// 在服务配置的第一阶段执行，用于注册和配置基础服务。
        /// </summary>
        /// <param name="configureDelegate">配置服务的委托，接收 <see cref="ConfigureServicesContext"/> 参数</param>
        /// <returns>当前 <see cref="BootstrapperCreationOptions"/> 实例，支持链式调用</returns>
        public BootstrapperCreationOptions PreConfigureServices(Action<ConfigureServicesContext> configureDelegate)
        {
            _preConfigureServiceActions.Add(configureDelegate);
            return this;
        }

        /// <summary>
        /// 添加服务配置操作。
        /// 在服务配置的主要阶段执行，用于注册应用程序的核心服务。
        /// </summary>
        /// <param name="configureDelegate">配置服务的委托，接收 <see cref="ConfigureServicesContext"/> 参数</param>
        /// <returns>当前 <see cref="BootstrapperCreationOptions"/> 实例，支持链式调用</returns>
        public BootstrapperCreationOptions ConfigureServices(Action<ConfigureServicesContext> configureDelegate)
        {
            _configureServiceActions.Add(configureDelegate);
            return this;
        }

        /// <summary>
        /// 添加后配置服务操作。
        /// 在服务配置的最后阶段执行，用于调整和优化已注册的服务。
        /// </summary>
        /// <param name="configureDelegate">配置服务的委托，接收 <see cref="ConfigureServicesContext"/> 参数</param>
        /// <returns>当前 <see cref="BootstrapperCreationOptions"/> 实例，支持链式调用</returns>
        public BootstrapperCreationOptions PostConfigureServices(Action<ConfigureServicesContext> configureDelegate)
        {
            _postConfigureServiceActions.Add(configureDelegate);
            return this;
        }

        /// <summary>
        /// 添加配置构建操作。
        /// 用于向应用程序配置添加配置源、设置和其他配置相关的操作。
        /// </summary>
        /// <param name="action">配置 <see cref="IConfigurationBuilder"/> 的操作委托</param>
        public void Configuration(Action<IConfigurationBuilder> action) => _configurationActions.Add(action);

        /// <summary>
        /// 设置自定义的服务提供程序工厂。
        /// 允许使用第三方依赖注入容器（如 Autofac、Castle Windsor 等）替代默认的 <see cref="IServiceCollection"/>。
        /// </summary>
        /// <typeparam name="TContainerBuilder">容器构建器的类型</typeparam>
        /// <param name="factory">服务提供程序工厂实例</param>
        public void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) => ServiceFactory = () => new ServiceFactoryAdapter<TContainerBuilder>(factory);

        /// <summary>
        /// 执行所有配置构建操作。
        /// 按照添加顺序依次执行所有配置操作来构建应用程序配置。
        /// </summary>
        /// <param name="configurationBuilder">要配置的 <see cref="IConfigurationBuilder"/> 实例</param>
        internal void ConfigureConfiguration(IConfigurationBuilder configurationBuilder) => _configurationActions.ForEach(a => a(configurationBuilder));

        /// <summary>
        /// 执行所有预配置服务操作。
        /// 在服务配置的第一阶段依次执行所有预配置操作。
        /// </summary>
        /// <param name="context">服务配置上下文，包含引导程序、服务集合和配置信息</param>
        internal void PreConfigureServices(ConfigureServicesContext context) => _preConfigureServiceActions.ForEach(a => a(context));

        /// <summary>
        /// 执行所有服务配置操作。
        /// 在服务配置的主要阶段依次执行所有配置操作。
        /// </summary>
        /// <param name="context">服务配置上下文，包含引导程序、服务集合和配置信息</param>
        internal void ConfigureServices(ConfigureServicesContext context) => _configureServiceActions.ForEach(a => a(context));

        /// <summary>
        /// 执行所有后配置服务操作。
        /// 在服务配置的最后阶段依次执行所有后配置操作。
        /// </summary>
        /// <param name="context">服务配置上下文，包含引导程序、服务集合和配置信息</param>
        internal void PostConfigureServices(ConfigureServicesContext context) => _postConfigureServiceActions.ForEach(a => a(context));
    }
}
