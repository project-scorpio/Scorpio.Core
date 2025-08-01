using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;

using Scorpio.DependencyInjection;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化管理器实现类，负责按优先级顺序初始化所有注册的可初始化对象
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类实现了 <see cref="IInitializationManager"/> 接口，作为单例依赖项注册到容器中。
    /// 通过依赖注入获取服务实例并执行初始化操作。
    /// </para>
    /// <para>
    /// 初始化过程按照 <see cref="InitializationOptions"/> 中定义的优先级顺序执行，
    /// 确保系统组件按正确的顺序启动。
    /// </para>
    /// </remarks>
    /// <seealso cref="IInitializationManager"/>
    /// <seealso cref="IInitializable"/>
    /// <seealso cref="InitializationOptions"/>
    internal class InitializationManager : IInitializationManager, IServiceProviderAccessor, ISingletonDependency
    {
        /// <summary>
        /// 初始化配置选项
        /// </summary>
        private readonly InitializationOptions _options;

        /// <summary>
        /// 初始化 <see cref="InitializationManager"/> 类的新实例
        /// </summary>
        /// <param name="serviceProvider">
        /// 服务提供者，用于解析可初始化对象的实例。不能为 <c>null</c>
        /// </param>
        /// <param name="options">
        /// 初始化配置选项，包含所有需要初始化的类型信息。不能为 <c>null</c>
        /// </param>
        public InitializationManager(IServiceProvider serviceProvider, IOptions<InitializationOptions> options)
        {
            _options = options.Value;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取服务提供者实例
        /// </summary>
        /// <value>
        /// 用于解析依赖注入容器中服务实例的服务提供者
        /// </value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 执行系统初始化操作
        /// </summary>
        /// <remarks>
        /// <para>
        /// 该方法按照配置的优先级顺序遍历所有注册的可初始化类型，
        /// 从依赖注入容器中获取实例并调用其 <see cref="IInitializable.Initialize"/> 方法。
        /// </para>
        /// <para>
        /// 初始化过程按以下步骤执行：
        /// <list type="number">
        /// <item><description>按优先级降序遍历所有类型组</description></item>
        /// <item><description>对每个类型组中的类型依次处理</description></item>
        /// <item><description>从服务容器获取类型实例</description></item>
        /// <item><description>如果实例实现了 <see cref="IInitializable"/>，则调用初始化方法</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public void Initialize()
        {
            // 按优先级顺序遍历所有可初始化类型组
            _options.Initializables.ForEach(kv =>
            {
                // 遍历当前优先级组中的所有类型
                kv.Value.ForEach(t =>
                {
                    // 从依赖注入容器中获取类型实例
                    if (ServiceProvider.GetService(t) is IInitializable initializable)
                    {
                        // 执行初始化操作
                        initializable.Initialize();
                    }
                });
            });
        }
    }
}
