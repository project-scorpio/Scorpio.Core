using System;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Modularity.Plugins;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 定义模块加载器的接口，用于发现、解析和加载应用程序模块
    /// </summary>
    /// <remarks>
    /// 该接口定义了模块加载的核心功能，负责从不同的来源（如插件源、启动模块等）
    /// 发现模块类型，解析模块依赖关系，并按照正确的顺序加载和初始化模块。
    /// 模块加载器是模块化系统的关键组件，确保应用程序的模块化架构正常工作。
    /// </remarks>
    public interface IModuleLoader
    {
        /// <summary>
        /// 加载应用程序的所有模块，包括启动模块及其依赖项和插件模块
        /// </summary>
        /// <param name="services">服务集合，用于注册模块相关的服务和依赖项</param>
        /// <param name="startupModuleType">启动模块的类型，作为模块加载的入口点</param>
        /// <param name="plugInSources">插件源列表，包含要动态加载的插件模块</param>
        /// <returns>
        /// 包含所有已加载模块描述符的数组，按照依赖关系和加载顺序排列。
        /// 每个 <see cref="IModuleDescriptor"/> 包含模块的详细信息和状态。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法执行完整的模块加载流程：
        /// <list type="number">
        /// <item><description>从启动模块开始，递归发现所有依赖模块</description></item>
        /// <item><description>从插件源中发现和加载插件模块</description></item>
        /// <item><description>解析模块间的依赖关系，构建依赖图</description></item>
        /// <item><description>按照依赖顺序对模块进行拓扑排序</description></item>
        /// <item><description>创建模块实例并执行配置服务阶段</description></item>
        /// <item><description>返回所有已加载的模块描述符</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 加载过程中如果发现循环依赖、缺失依赖或其他错误，将抛出相应的异常。
        /// </para>
        /// </remarks>
        /// <exception cref="ScorpioException">当模块加载过程中发生错误时抛出，如循环依赖、类型错误等</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="services"/>、<paramref name="startupModuleType"/> 或 <paramref name="plugInSources"/> 为 null 时抛出</exception>
        IModuleDescriptor[] LoadModules(
            IServiceCollection services,
            Type startupModuleType,
            IPlugInSourceList plugInSources
            );
    }
}
