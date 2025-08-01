using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scorpio.Modularity;

namespace Scorpio
{
    /// <summary>
    /// 内部引导程序实现类。
    /// 继承自 <see cref="Bootstrapper"/> 基类，提供引导程序的具体实现，
    /// 主要用于 Scorpio 框架内部的应用程序引导和资源管理。
    /// </summary>
    internal class InternalBootstrapper : Bootstrapper
    {
        /// <summary>
        /// 初始化 <see cref="InternalBootstrapper"/> 类的新实例。
        /// 使用指定的启动模块类型、服务集合、配置和选项来创建引导程序。
        /// </summary>
        /// <param name="startupModuleType">
        /// 启动模块类型，必须实现 <see cref="IScorpioModule"/> 接口，
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
        /// 释放 <see cref="InternalBootstrapper"/> 使用的资源。
        /// 重写基类的释放方法，确保配置和服务提供者被正确释放。
        /// </summary>
        /// <param name="disposing">
        /// 如果为 true，则释放托管和非托管资源；
        /// 如果为 false，则仅释放非托管资源
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // 首先调用基类的释放方法
            base.Dispose(disposing);
            
            // 安全地释放配置对象
            Configuration.SafelyDispose();
            
            // 安全地释放服务提供者
            ServiceProvider.SafelyDispose();
        }
    }
}
