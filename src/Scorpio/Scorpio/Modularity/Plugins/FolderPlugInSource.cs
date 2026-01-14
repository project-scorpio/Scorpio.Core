using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.FileSystemGlobbing;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 基于文件夹的插件源实现类，用于从指定的文件夹路径中加载程序集并发现模块
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IPlugInSource"/> 接口，通过扫描指定文件夹中的 .dll 和 .exe 文件来发现和加载模块。
    /// 支持通过文件过滤器来控制哪些程序集文件会被加载。
    /// </remarks>
    internal class FolderPlugInSource : IPlugInSource
    {
        /// <summary>
        /// 要扫描的文件夹路径
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// 插件源列表的引用，用于访问程序集加载上下文和文件提供程序
        /// </summary>
        private readonly PlugInSourceList _plugInSourceLists;

        /// <summary>
        /// 获取或设置文件过滤器，用于控制哪些文件会被加载
        /// </summary>
        /// <value>
        /// 一个委托，接受文件名作为参数，返回布尔值表示是否应该加载该文件。
        /// 如果为 null，则加载所有匹配的程序集文件。
        /// </value>
        public Func<string, bool> Filter { get; set; }

        /// <summary>
        /// 初始化 <see cref="FolderPlugInSource"/> 类的新实例
        /// </summary>
        /// <param name="plugInSourceLists">插件源列表，提供程序集加载上下文和文件提供程序</param>
        /// <param name="path">要扫描的文件夹路径</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="plugInSourceLists"/> 或 <paramref name="path"/> 为 null 时抛出</exception>
        public FolderPlugInSource(PlugInSourceList plugInSourceLists, string path)
        {
            _plugInSourceLists = plugInSourceLists;
            _path = path;
        }

        /// <summary>
        /// 从指定的文件夹路径中获取所有模块类型
        /// </summary>
        /// <returns>包含所有发现的模块类型的数组</returns>
        /// <exception cref="ScorpioException">当无法从程序集中获取模块类型时抛出</exception>
        /// <remarks>
        /// 该方法会扫描指定文件夹中的所有程序集文件（.dll 和 .exe），加载它们并从中提取实现了模块接口的类型。
        /// 如果程序集无法正确加载类型（<see cref="ReflectionTypeLoadException"/>），该程序集将被跳过。
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3626:Jump statements should not be redundant", Justification = "异常处理中的 continue 语句用于跳过无法加载的程序集")]
        public Type[] GetModules()
        {
            // 创建模块类型列表用于存储发现的模块
            var modules = new List<Type>();

            // 遍历文件夹中的所有程序集
            foreach (var assembly in GetAssemblies())
            {
                try
                {
                    // 遍历程序集中的所有类型，筛选出模块类型
                    foreach (var type in assembly.GetTypes().Where(type => ScorpioModule.IsModule(type)))
                    {
                        // 如果模块尚未存在于列表中，则添加到模块列表
                        modules.AddIfNotContains(type);
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // 如果程序集无法正确加载类型，跳过该程序集继续处理下一个
                    continue;
                }
                catch (Exception ex)
                {
                    // 如果获取类型失败（非类型加载异常），抛出包含详细信息的 ScorpioException
                    throw new ScorpioException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            // 返回发现的所有模块类型数组
            return modules.ToArray();
        }

        /// <summary>
        /// 从指定文件夹中获取所有匹配的程序集
        /// </summary>
        /// <returns>程序集的可枚举集合</returns>
        /// <remarks>
        /// 该方法使用文件系统匹配器搜索 .dll 和 .exe 文件，并可选地应用文件过滤器。
        /// 通过程序集加载上下文从文件流中加载每个匹配的程序集。
        /// </remarks>
        internal IEnumerable<Assembly> GetAssemblies()
        {
            // 创建文件匹配器，忽略大小写
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            // 添加匹配模式：递归搜索所有 .dll 和 .exe 文件
            matcher.AddInclude("./**/*.dll").AddInclude("./**/*.exe");

            // 获取目录中所有匹配的程序集文件
            var assemblyFiles = _plugInSourceLists.FileProvider.GetDirectoryContents(_path)
                .Where(f => matcher.Match(f.PhysicalPath).HasMatches);

            // 如果存在文件过滤器，则应用过滤器
            if (Filter != null)
            {
                assemblyFiles = assemblyFiles.Where(f => Filter(f.Name));
            }

            // 通过程序集加载上下文从文件流中加载每个程序集并返回
            return assemblyFiles.Select(f => _plugInSourceLists.AssemblyLoadContext.LoadFromStream(f.CreateReadStream()));
        }
    }
}
