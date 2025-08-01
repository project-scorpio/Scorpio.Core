using System;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 定义插件源的接口，用于从不同来源发现和加载模块类型
    /// </summary>
    /// <remarks>
    /// 插件源可以是文件、文件夹、程序集或其他任何可以提供模块类型的来源。
    /// 实现此接口的类负责从特定来源中发现并返回所有可用的模块类型。
    /// </remarks>
    public interface IPlugInSource
    {
        /// <summary>
        /// 获取当前插件源中的所有模块类型
        /// </summary>
        /// <returns>
        /// 包含所有发现的模块类型的数组。如果没有找到任何模块，则返回空数组。
        /// 返回的类型必须是实现了模块接口的有效模块类型。
        /// </returns>
        /// <remarks>
        /// 此方法会扫描插件源中的所有可用资源，识别并返回实现了模块接口的类型。
        /// 具体的发现逻辑取决于插件源的实现方式，例如：
        /// <list type="bullet">
        /// <item><description>文件插件源会从指定的程序集文件中加载模块</description></item>
        /// <item><description>文件夹插件源会扫描目录中的所有程序集文件</description></item>
        /// <item><description>类型插件源会直接返回预定义的模块类型</description></item>
        /// </list>
        /// </remarks>
        Type[] GetModules();
    }
}
