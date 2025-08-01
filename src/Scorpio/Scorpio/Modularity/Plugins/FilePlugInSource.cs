using System;
using System.Collections.Generic;
using System.Linq;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 基于文件的插件源实现类，用于从指定的文件路径中加载模块
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IPlugInSource"/> 接口，通过读取指定文件路径中的程序集来发现和加载模块
    /// </remarks>
    internal class FilePlugInSource : IPlugInSource
    {
        /// <summary>
        /// 存储要加载的文件路径数组
        /// </summary>
        private readonly string[] _filePaths;

        /// <summary>
        /// 插件源列表的引用，用于访问程序集加载上下文和文件提供程序
        /// </summary>
        private readonly PlugInSourceList _plugInSourceLists;

        /// <summary>
        /// 初始化 <see cref="FilePlugInSource"/> 类的新实例
        /// </summary>
        /// <param name="plugInSourceLists">插件源列表，提供程序集加载上下文和文件提供程序</param>
        /// <param name="filePaths">要加载的文件路径数组</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="plugInSourceLists"/> 或 <paramref name="filePaths"/> 为 null 时抛出</exception>
        public FilePlugInSource(PlugInSourceList plugInSourceLists, string[] filePaths)
        {
            _plugInSourceLists = plugInSourceLists;
            _filePaths = filePaths;
        }

        /// <summary>
        /// 从指定的文件路径中获取所有模块类型
        /// </summary>
        /// <returns>包含所有发现的模块类型的数组</returns>
        /// <exception cref="ScorpioException">当无法从程序集中获取模块类型时抛出</exception>
        /// <remarks>
        /// 该方法会遍历所有指定的文件路径，加载相应的程序集，并从中提取实现了模块接口的类型
        /// </remarks>
        public Type[] GetModules()
        {
            // 创建模块类型列表用于存储发现的模块
            var modules = new List<Type>();

            // 遍历所有指定的文件路径
            foreach (var filePath in _filePaths)
            {
                // 通过程序集加载上下文从文件流中加载程序集
                var assembly = _plugInSourceLists.AssemblyLoadContext.LoadFromStream(
                    _plugInSourceLists.FileProvider.GetFileInfo(filePath)
                                                   .CreateReadStream());
                try
                {
                    // 遍历程序集中的所有类型
                    foreach (var type in assembly.GetTypes().Where(t => ScorpioModule.IsModule(t)))
                    {
                        // 如果模块尚未存在于列表中，则添加到模块列表
                        modules.AddIfNotContains(type);
                    }
                }
                catch (Exception ex)
                {
                    // 如果获取类型失败，抛出包含详细信息的 ScorpioException
                    throw new ScorpioException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            // 返回发现的所有模块类型数组
            return modules.ToArray();
        }
    }
}
