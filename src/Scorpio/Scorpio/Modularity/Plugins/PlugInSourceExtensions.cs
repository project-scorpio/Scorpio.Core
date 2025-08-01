using System;
using System.Linq;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 为 <see cref="IPlugInSource"/> 接口提供扩展方法的静态类
    /// </summary>
    /// <remarks>
    /// 该类包含用于增强插件源功能的扩展方法，特别是处理模块依赖关系的方法。
    /// </remarks>
    internal static class PlugInSourceExtensions
    {
        /// <summary>
        /// 获取插件源中的所有模块类型及其所有依赖模块类型
        /// </summary>
        /// <param name="plugInSource">要获取模块的插件源</param>
        /// <returns>
        /// 包含所有模块类型及其依赖模块类型的数组，已去除重复项。
        /// 返回的数组包括直接模块和所有传递依赖的模块类型。
        /// </returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="plugInSource"/> 为 null 时抛出</exception>
        /// <remarks>
        /// <para>
        /// 此方法不仅返回插件源中直接发现的模块类型，还会递归查找这些模块的所有依赖模块，
        /// 确保返回完整的模块依赖图。这对于确保模块系统能够正确解析和加载所有必需的依赖项非常重要。
        /// </para>
        /// <para>
        /// 方法执行流程：
        /// <list type="number">
        /// <item><description>验证输入参数不为 null</description></item>
        /// <item><description>获取插件源中的直接模块类型</description></item>
        /// <item><description>通过 <see cref="ModuleHelper.FindAllModuleTypes"/> 递归查找每个模块的所有依赖</description></item>
        /// <item><description>展平结果并去除重复的模块类型</description></item>
        /// <item><description>返回最终的模块类型数组</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static Type[] GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            // 验证插件源参数不为 null
            Check.NotNull(plugInSource, nameof(plugInSource));

            // 获取插件源中的所有模块，并递归查找每个模块的依赖关系，
            // 最后去重并转换为数组返回
            return plugInSource
                .GetModules()                           // 获取插件源中的直接模块类型
                .SelectMany(ModuleHelper.FindAllModuleTypes)  // 为每个模块递归查找所有依赖模块
                .Distinct()                             // 去除重复的模块类型
                .ToArray();                             // 转换为数组并返回
        }
    }
}
