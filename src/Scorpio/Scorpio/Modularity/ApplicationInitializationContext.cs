using System;
using System.Collections.Generic;

using Scorpio.DependencyInjection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 应用程序初始化上下文类，用于在模块初始化过程中传递必要的上下文信息
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IServiceProviderAccessor"/> 接口，提供对服务提供程序的访问。
    /// 在应用程序和模块的初始化阶段，此上下文对象会被传递给各个模块，
    /// 使模块能够访问依赖注入容器和初始化参数。
    /// </remarks>
    public class ApplicationInitializationContext : IServiceProviderAccessor
    {
        /// <summary>
        /// 获取服务提供程序，用于访问依赖注入容器中的服务
        /// </summary>
        /// <value>
        /// 提供对应用程序依赖注入容器的访问，模块可通过此属性获取所需的服务实例
        /// </value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 获取初始化参数集合
        /// </summary>
        /// <value>
        /// 包含应用程序初始化时传递的参数对象集合。
        /// 这些参数可以是配置信息、环境变量或其他初始化所需的数据
        /// </value>
        public IEnumerable<object> Parameters { get; }

        /// <summary>
        /// 初始化 <see cref="ApplicationInitializationContext"/> 类的新实例
        /// </summary>
        /// <param name="serviceProvider">服务提供程序，用于访问依赖注入容器</param>
        /// <param name="initializeParams">初始化参数数组，包含应用程序初始化所需的参数</param>
        /// <remarks>
        /// 该构造函数为内部使用，通常由应用程序框架在初始化阶段创建上下文实例。
        /// 使用 params 修饰符支持传入可变数量的初始化参数。
        /// </remarks>
        internal ApplicationInitializationContext(IServiceProvider serviceProvider, params object[] initializeParams)
        {
            // 设置初始化参数集合
            Parameters = initializeParams;
            // 设置服务提供程序引用
            ServiceProvider = serviceProvider;
        }
    }
}
