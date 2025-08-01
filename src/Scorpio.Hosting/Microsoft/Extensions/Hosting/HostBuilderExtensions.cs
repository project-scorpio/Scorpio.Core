using System;

using Scorpio;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// <see cref="IHostBuilder"/> 的扩展方法类。
    /// 提供将 Scorpio 框架集成到 .NET Generic Host 的扩展方法，
    /// 简化 Scorpio 应用程序在托管环境中的配置和启动过程。
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// 将 Scorpio 框架添加到主机构建器中，使用默认配置。
        /// 使用泛型参数指定启动模块类型，提供类型安全的模块配置。
        /// </summary>
        /// <typeparam name="TStartupModule">
        /// 启动模块类型，必须实现 <see cref="Scorpio.Modularity.IScorpioModule"/> 接口
        /// </typeparam>
        /// <param name="hostBuilder">要配置的 <see cref="IHostBuilder"/> 实例</param>
        /// <returns>配置后的 <see cref="IHostBuilder"/> 实例，支持链式调用</returns>
        public static IHostBuilder AddScorpio<TStartupModule>(this IHostBuilder hostBuilder)
                 where TStartupModule : Scorpio.Modularity.IScorpioModule => AddScorpio<TStartupModule>(hostBuilder, o => { });

        /// <summary>
        /// 将 Scorpio 框架添加到主机构建器中，使用自定义配置。
        /// 使用泛型参数指定启动模块类型，并允许通过委托配置引导程序选项。
        /// </summary>
        /// <typeparam name="TStartupModule">
        /// 启动模块类型，必须实现 <see cref="Scorpio.Modularity.IScorpioModule"/> 接口
        /// </typeparam>
        /// <param name="hostBuilder">要配置的 <see cref="IHostBuilder"/> 实例</param>
        /// <param name="optionsAction">
        /// 用于配置 <see cref="BootstrapperCreationOptions"/> 的委托，
        /// 允许自定义引导程序的创建行为
        /// </param>
        /// <returns>配置后的 <see cref="IHostBuilder"/> 实例，支持链式调用</returns>
        public static IHostBuilder AddScorpio<TStartupModule>(this IHostBuilder hostBuilder, Action<BootstrapperCreationOptions> optionsAction)
            where TStartupModule : Scorpio.Modularity.IScorpioModule => AddScorpio(hostBuilder, typeof(TStartupModule), optionsAction);

        /// <summary>
        /// 将 Scorpio 框架添加到主机构建器中，使用指定的启动模块类型和默认配置。
        /// 通过 <see cref="Type"/> 参数指定启动模块，适用于运行时动态确定模块类型的场景。
        /// </summary>
        /// <param name="builder">要配置的 <see cref="IHostBuilder"/> 实例</param>
        /// <param name="startupModuleType">
        /// 启动模块的类型，必须实现 <see cref="Scorpio.Modularity.IScorpioModule"/> 接口
        /// </param>
        /// <returns>配置后的 <see cref="IHostBuilder"/> 实例，支持链式调用</returns>
        public static IHostBuilder AddScorpio(this IHostBuilder builder, Type startupModuleType) => AddScorpio(builder, startupModuleType, o => { });

        /// <summary>
        /// 将 Scorpio 框架添加到主机构建器中，使用指定的启动模块类型和自定义配置。
        /// 这是核心实现方法，负责创建和配置 Scorpio 服务提供程序工厂。
        /// </summary>
        /// <param name="builder">要配置的 <see cref="IHostBuilder"/> 实例</param>
        /// <param name="startupModuleType">
        /// 启动模块的类型，必须实现 <see cref="Scorpio.Modularity.IScorpioModule"/> 接口
        /// </param>
        /// <param name="optionsAction">
        /// 用于配置 <see cref="BootstrapperCreationOptions"/> 的委托，
        /// 允许自定义引导程序的创建行为，包括服务配置、插件源等
        /// </param>
        /// <returns>配置后的 <see cref="IHostBuilder"/> 实例，支持链式调用</returns>
        public static IHostBuilder AddScorpio(this IHostBuilder builder, Type startupModuleType, Action<BootstrapperCreationOptions> optionsAction)
        {
            // 使用 Scorpio 的服务提供程序工厂替换默认的依赖注入容器
            // ServiceProviderFactory 将处理 Scorpio 框架的初始化和服务配置
            builder.UseServiceProviderFactory(context => new ServiceProviderFactory(context, startupModuleType, optionsAction));
            return builder;
        }
    }
}
