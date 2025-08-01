using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 服务配置上下文类，用于在模块配置服务阶段传递必要的上下文信息
    /// </summary>
    /// <remarks>
    /// 该类在模块系统的服务配置阶段使用，为模块提供访问引导程序、服务集合、
    /// 配置信息和共享属性的能力。通过此上下文，模块可以注册服务、
    /// 配置依赖注入容器并共享配置期间的状态信息。
    /// </remarks>
    public class ConfigureServicesContext
    {
        /// <summary>
        /// 初始化 <see cref="ConfigureServicesContext"/> 类的新实例
        /// </summary>
        /// <param name="bootstrapper">引导程序实例，提供应用程序启动和配置的核心功能</param>
        /// <param name="services">服务集合，用于注册和配置依赖注入服务</param>
        /// <param name="configuration">配置对象，包含应用程序的配置信息</param>
        /// <remarks>
        /// 该构造函数具有 protected internal 访问级别，通常由框架内部在服务配置阶段创建上下文实例。
        /// 确保所有必需的组件都可用于模块的服务配置过程。
        /// </remarks>
        protected internal ConfigureServicesContext(IBootstrapper bootstrapper, IServiceCollection services, IConfiguration configuration)
        {
            // 设置引导程序引用
            Bootstrapper = bootstrapper;
            // 设置服务集合引用
            Services = services;
            // 设置配置对象引用
            Configuration = configuration;
        }

        /// <summary>
        /// 获取引导程序实例
        /// </summary>
        /// <value>
        /// 提供对应用程序引导程序的访问，模块可通过此属性获取启动和配置相关的功能
        /// </value>
        public IBootstrapper Bootstrapper { get; }

        /// <summary>
        /// 获取服务集合
        /// </summary>
        /// <value>
        /// 提供对依赖注入服务集合的访问，模块可通过此属性注册服务、配置生命周期等
        /// </value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取应用程序配置信息
        /// </summary>
        /// <value>
        /// 包含应用程序合并后的配置信息的 <see cref="IConfiguration"/> 实例。
        /// 模块可通过此属性读取配置值、绑定配置对象等
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 获取属性字典，用于在主机构建过程中组件间共享状态
        /// </summary>
        /// <value>
        /// 一个中心化的位置，用于在服务配置阶段存储和共享组件间的状态信息。
        /// 键为字符串类型，值为任意对象类型，支持模块间的数据传递和状态共享
        /// </value>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
}
