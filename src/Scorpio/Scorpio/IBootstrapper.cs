using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scorpio.Modularity;

namespace Scorpio
{
    /// <summary>
    /// 定义应用程序引导程序的接口。
    /// 提供应用程序初始化、配置管理、服务容器和生命周期管理的核心功能。
    /// 继承 <see cref="IDisposable"/> 接口以支持资源释放。
    /// </summary>
    public interface IBootstrapper : IDisposable
    {
        /// <summary>
        /// 获取应用程序的启动（入口）模块类型。
        /// 这是应用程序的根模块，定义了应用程序的基本结构和依赖关系。
        /// </summary>
        /// <value>实现 <see cref="IScorpioModule"/> 接口的启动模块类型</value>
        Type StartupModuleType { get; }

        /// <summary>
        /// 获取注册到此应用程序的服务列表。
        /// 应用程序初始化后不能向此集合添加新服务。
        /// </summary>
        /// <value>包含所有已注册服务的 <see cref="IServiceCollection"/> 实例</value>
        IServiceCollection Services { get; }

        /// <summary>
        /// 获取包含应用程序合并配置的 <see cref="IConfiguration"/> 对象。
        /// 包含来自各种配置源的合并配置信息。
        /// </summary>
        /// <value>应用程序的配置实例</value>
        IConfiguration Configuration { get; }

        /// <summary>
        /// 获取用于在主机构建过程中在组件之间共享状态的中心位置。
        /// 提供键值对存储，用于模块间的数据共享和通信。
        /// </summary>
        /// <value>用于存储共享属性的字典</value>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// 获取应用程序使用的根服务提供者的引用。
        /// 在应用程序初始化之前不能使用此属性。
        /// </summary>
        /// <value>用于服务解析的 <see cref="IServiceProvider"/> 实例</value>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 用于优雅地关闭应用程序和所有模块。
        /// 按照相反的加载顺序停止所有模块并清理资源。
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 初始化应用程序。
        /// 启动所有已加载的模块并完成应用程序初始化过程。
        /// </summary>
        /// <param name="initializeParams">传递给模块初始化方法的参数数组</param>
        void Initialize(params object[] initializeParams);
    }
}
