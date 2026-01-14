using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Scorpio.DependencyInjection.Conventional;
using Scorpio.Initialization;
using Scorpio.Modularity;
using Scorpio.Runtime;
using Scorpio.Threading;

namespace Scorpio
{
    /// <summary>
    /// Scorpio 框架的核心模块。
    /// 负责注册框架的基础服务和核心功能，包括依赖注入约定、环境上下文、
    /// 取消令牌提供程序和初始化管理器等核心组件。
    /// 这是框架启动时最先加载的模块之一。
    /// </summary>
    internal sealed class KernelModule : ScorpioModule
    {
        /// <summary>
        /// 预配置服务阶段的配置方法。
        /// 在服务注册的第一阶段执行，用于注册框架的基础服务和约定注册器。
        /// </summary>
        /// <param name="context">
        /// 服务配置上下文，包含服务集合、配置和引导程序信息
        /// </param>
        public override void PreConfigureServices(ConfigureServicesContext context)
        {
            // 替换默认的选项工厂实现，使用 Scorpio 定制的选项工厂
            context.Services.ReplaceOrAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(Options.OptionsFactory<>)), true);

            // 添加基础约定注册器，用于处理标准的依赖注入约定
            context.AddConventionalRegistrar(new BasicConventionalRegistrar());

            // 添加初始化约定注册器，用于处理初始化相关的服务注册
            context.AddConventionalRegistrar<InitializationConventionalRegistrar>();
        }

        /// <summary>
        /// 配置服务阶段的配置方法。
        /// 在服务注册的主要阶段执行，用于注册框架的核心服务。
        /// </summary>
        /// <param name="context">
        /// 服务配置上下文，包含服务集合、配置和引导程序信息
        /// </param>
        public override void ConfigureServices(ConfigureServicesContext context)
        {
            // 注册环境作用域提供程序的泛型实现
            // 使用 AmbientDataContextAmbientScopeProvider 作为默认实现
            context.Services.TryAddSingleton(typeof(IAmbientScopeProvider<>), typeof(AmbientDataContextAmbientScopeProvider<>));

            // 注册取消令牌提供程序，使用不支持取消的单例实例
            // NoneCancellationTokenProvider.Instance 提供永不取消的令牌
            context.Services.TryAddSingleton<ICancellationTokenProvider>(NoneCancellationTokenProvider.Instance);
        }

        /// <summary>
        /// 初始化应用程序的方法。
        /// 在应用程序启动时执行，负责触发框架的初始化管理器。
        /// </summary>
        /// <param name="context">
        /// 应用程序初始化上下文，包含服务提供者和初始化参数
        /// </param>
        public override void Initialize(ApplicationInitializationContext context)
        {
            // 获取初始化管理器服务并执行初始化
            // 使用可选调用（?.）确保即使服务未注册也不会抛出异常
            context.ServiceProvider.GetService<IInitializationManager>()?.Initialize();
        }
    }
}
