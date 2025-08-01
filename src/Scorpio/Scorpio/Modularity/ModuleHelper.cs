using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 模块辅助工具类，提供模块发现和依赖关系解析的静态方法
    /// </summary>
    /// <remarks>
    /// 该类包含用于模块系统核心功能的辅助方法，主要负责：
    /// <list type="bullet">
    /// <item><description>从启动模块开始递归发现所有相关模块</description></item>
    /// <item><description>解析模块的直接依赖关系</description></item>
    /// <item><description>构建完整的模块依赖图</description></item>
    /// </list>
    /// 这些方法为模块加载器提供基础的模块发现和依赖解析功能。
    /// </remarks>
    internal static class ModuleHelper
    {
        /// <summary>
        /// 从指定的启动模块开始，递归查找所有相关的模块类型
        /// </summary>
        /// <param name="startupModuleType">启动模块的类型，作为依赖图的根节点</param>
        /// <returns>
        /// 包含启动模块及其所有直接和间接依赖模块类型的列表。
        /// 列表中不包含重复的模块类型。
        /// </returns>
        /// <remarks>
        /// 该方法实现了深度优先搜索算法，从启动模块开始递归遍历整个依赖图，
        /// 收集所有需要加载的模块类型。这是模块发现过程的入口点。
        /// </remarks>
        /// <exception cref="ArgumentException">当 <paramref name="startupModuleType"/> 不是有效的模块类型时抛出</exception>
        public static List<Type> FindAllModuleTypes(Type startupModuleType)
        {
            // 初始化模块类型列表
            var moduleTypes = new List<Type>();
            // 递归添加模块及其依赖项
            AddModuleAndDependenciesResursively(moduleTypes, startupModuleType);
            return moduleTypes;
        }

        /// <summary>
        /// 查找指定模块类型的直接依赖模块类型
        /// </summary>
        /// <param name="moduleType">要查找依赖关系的模块类型</param>
        /// <returns>
        /// 包含所有直接依赖模块类型的列表。如果模块没有显式依赖且不是内核模块，
        /// 则自动添加 <see cref="KernelModule"/> 作为默认依赖。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法通过反射获取模块上的 <see cref="IDependedTypesProvider"/> 特性（如 <see cref="DependsOnAttribute"/>），
        /// 并调用其 <see cref="IDependedTypesProvider.GetDependedTypes"/> 方法来获取依赖关系。
        /// </para>
        /// <para>
        /// 如果模块没有任何显式依赖，且不是 <see cref="KernelModule"/> 本身，
        /// 则会自动添加 <see cref="KernelModule"/> 作为默认依赖，确保所有模块都依赖于核心模块。
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">当 <paramref name="moduleType"/> 不是有效的模块类型时抛出</exception>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            // 验证模块类型的有效性
            ScorpioModule.CheckModuleType(moduleType);

            // 初始化依赖列表
            var dependencies = new List<Type>();

            // 获取模块上的所有依赖类型提供程序特性
            var dependencyDescriptors = moduleType.GetCustomAttributes().OfType<IDependedTypesProvider>();

            // 遍历每个依赖描述符
            foreach (var descriptor in dependencyDescriptors)
            {
                // 获取依赖类型并添加到列表中
                foreach (var dependedModuleType in descriptor.GetDependedTypes())
                {
                    // 使用扩展方法避免重复添加
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }

            // 如果没有显式依赖且不是内核模块，则添加内核模块作为默认依赖
            if (dependencies.Count == 0 && moduleType != typeof(KernelModule))
            {
                dependencies.Add(typeof(KernelModule));
            }

            return dependencies;
        }

        /// <summary>
        /// 递归地添加模块及其所有依赖项到模块类型列表中
        /// </summary>
        /// <param name="moduleTypes">用于收集模块类型的列表</param>
        /// <param name="moduleType">当前要处理的模块类型</param>
        /// <remarks>
        /// <para>
        /// 该方法实现了深度优先搜索算法的核心逻辑：
        /// <list type="number">
        /// <item><description>验证当前模块类型的有效性</description></item>
        /// <item><description>检查模块是否已被添加，避免重复处理</description></item>
        /// <item><description>将当前模块添加到列表中</description></item>
        /// <item><description>递归处理当前模块的所有依赖项</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 通过递归调用自身来遍历整个依赖图，确保所有相关模块都被发现和收集。
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">当 <paramref name="moduleType"/> 不是有效的模块类型时抛出</exception>
        private static void AddModuleAndDependenciesResursively(List<Type> moduleTypes, Type moduleType)
        {
            // 验证模块类型的有效性
            ScorpioModule.CheckModuleType(moduleType);

            // 如果模块已存在于列表中，直接返回避免重复处理
            if (moduleTypes.Contains(moduleType))
            {
                return;
            }

            // 将当前模块添加到列表中
            moduleTypes.Add(moduleType);

            // 递归处理当前模块的每个依赖项
            foreach (var dependedModuleType in FindDependedModuleTypes(moduleType))
            {
                AddModuleAndDependenciesResursively(moduleTypes, dependedModuleType);
            }
        }
    }
}
