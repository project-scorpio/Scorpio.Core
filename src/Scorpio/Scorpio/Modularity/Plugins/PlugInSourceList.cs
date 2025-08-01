using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Microsoft.Extensions.FileProviders;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 插件源列表的实现类，用于管理和操作多个插件源
    /// </summary>
    /// <remarks>
    /// 该类继承自 <see cref="List{T}"/>，其中 T 为 <see cref="IPlugInSource"/>，
    /// 同时实现了 <see cref="IPlugInSourceList"/> 接口。
    /// 提供了文件提供程序和程序集加载上下文，用于插件源的统一管理和模块发现。
    /// </remarks>
    internal class PlugInSourceList : List<IPlugInSource>, IPlugInSourceList
    {
        /// <summary>
        /// 初始化 <see cref="PlugInSourceList"/> 类的新实例
        /// </summary>
        /// <param name="fileProvider">文件提供程序，用于访问文件系统资源</param>
        /// <param name="assemblyLoadContext">程序集加载上下文，用于加载程序集</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="fileProvider"/> 或 <paramref name="assemblyLoadContext"/> 为 null 时抛出</exception>
        public PlugInSourceList(IFileProvider fileProvider, AssemblyLoadContext assemblyLoadContext)
        {
            FileProvider = fileProvider;
            AssemblyLoadContext = assemblyLoadContext;
        }

        /// <summary>
        /// 获取文件提供程序，用于访问文件系统资源
        /// </summary>
        /// <value>
        /// 提供对文件系统的抽象访问，支持物理文件系统、嵌入式资源等不同的文件源
        /// </value>
        public IFileProvider FileProvider { get; }

        /// <summary>
        /// 获取程序集加载上下文，用于加载程序集
        /// </summary>
        /// <value>
        /// 控制程序集的加载行为，提供程序集解析和隔离功能
        /// </value>
        public AssemblyLoadContext AssemblyLoadContext { get; }

        /// <summary>
        /// 获取所有插件源中的所有模块类型，包括它们的依赖模块
        /// </summary>
        /// <returns>
        /// 包含所有发现的模块类型的数组，已去除重复项。
        /// 结果包括直接模块和所有传递依赖的模块类型。
        /// </returns>
        /// <remarks>
        /// 该方法会遍历列表中的每个插件源，调用 <see cref="PlugInSourceExtensions.GetModulesWithAllDependencies"/> 
        /// 扩展方法来获取模块及其依赖，然后将结果合并并去重。
        /// </remarks>
        internal Type[] GetAllModules()
        {
            return this
                .SelectMany(pluginSource => pluginSource.GetModulesWithAllDependencies())  // 从每个插件源获取模块及其所有依赖
                .Distinct()                                                                  // 去除重复的模块类型
                .ToArray();                                                                  // 转换为数组并返回
        }
    }

    /// <summary>
    /// 定义插件源列表的接口，用于标识和约束插件源列表的行为
    /// </summary>
    /// <remarks>
    /// 此接口作为插件源列表的抽象，可用于依赖注入和接口隔离。
    /// 当前版本为标记接口，未定义具体成员，主要用于类型标识和未来扩展。
    /// </remarks>
    public interface IPlugInSourceList
    {
        // 标记接口，用于类型标识和未来功能扩展
    }
}