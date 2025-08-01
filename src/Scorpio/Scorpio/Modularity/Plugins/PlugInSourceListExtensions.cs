using System;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 为 <see cref="IPlugInSourceList"/> 接口提供扩展方法的静态类
    /// </summary>
    /// <remarks>
    /// 该类包含用于向插件源列表添加不同类型插件源的便捷扩展方法，
    /// 支持类型插件源、文件插件源和文件夹插件源的添加。
    /// </remarks>
    public static class PlugInSourceListExtensions
    {
        /// <summary>
        /// 向插件源列表添加指定类型的模块插件源
        /// </summary>
        /// <typeparam name="TModule">要添加的模块类型，必须实现 <see cref="IScorpioModule"/> 接口</typeparam>
        /// <param name="plugs">插件源列表实例</param>
        /// <remarks>
        /// 这是一个泛型便捷方法，内部调用 <see cref="AddType(IPlugInSourceList, Type[])"/> 方法。
        /// 通过泛型约束确保只能添加有效的 Scorpio 模块类型。
        /// </remarks>
        public static void AddType<TModule>(this IPlugInSourceList plugs) where TModule : IScorpioModule => plugs.AddType(typeof(TModule));

        /// <summary>
        /// 向插件源列表添加一个或多个指定类型的模块插件源
        /// </summary>
        /// <param name="plugs">插件源列表实例</param>
        /// <param name="moduleType">要添加的模块类型数组</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="plugs"/> 或 <paramref name="moduleType"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 该方法创建一个 <see cref="TypePlugInSource"/> 实例并将其添加到插件源列表中。
        /// 支持同时添加多个模块类型，所有类型将包含在同一个插件源中。
        /// </remarks>
        public static void AddType(this IPlugInSourceList plugs, params Type[] moduleType) => plugs.Add(new TypePlugInSource(moduleType));

        /// <summary>
        /// 向插件源列表添加基于文件的插件源
        /// </summary>
        /// <param name="plugs">插件源列表实例</param>
        /// <param name="filePaths">要加载的程序集文件路径数组</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="plugs"/> 或 <paramref name="filePaths"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 该方法创建一个 <see cref="FilePlugInSource"/> 实例并将其添加到插件源列表中。
        /// 支持同时添加多个程序集文件，插件源将从这些文件中发现和加载模块。
        /// </remarks>
        public static void AddFile(this IPlugInSourceList plugs, params string[] filePaths) => plugs.Add(new FilePlugInSource(plugs.As<PlugInSourceList>(), filePaths));

        /// <summary>
        /// 向插件源列表添加基于文件夹的插件源
        /// </summary>
        /// <param name="plugs">插件源列表实例</param>
        /// <param name="path">要扫描的文件夹路径</param>
        /// <param name="predicate">
        /// 可选的文件过滤器委托，用于控制哪些文件会被加载。
        /// 如果为 null 或 default，则加载文件夹中所有匹配的程序集文件。
        /// </param>
        /// <exception cref="ArgumentNullException">当 <paramref name="plugs"/> 或 <paramref name="path"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 该方法创建一个 <see cref="FolderPlugInSource"/> 实例并将其添加到插件源列表中。
        /// 插件源将扫描指定文件夹中的 .dll 和 .exe 文件，并可选地应用文件过滤器。
        /// </remarks>
        public static void AddFolder(this IPlugInSourceList plugs, string path, Func<string, bool> predicate = default) => plugs.Add(new FolderPlugInSource(plugs.As<PlugInSourceList>(), path) { Filter = predicate });

        /// <summary>
        /// 将插件源添加到插件源列表中的私有辅助方法
        /// </summary>
        /// <param name="plugs">插件源列表实例</param>
        /// <param name="plug">要添加的插件源</param>
        /// <remarks>
        /// 该方法执行实际的添加操作，通过类型转换将接口实例转换为具体的 <see cref="PlugInSourceList"/> 类型。
        /// 如果转换失败（plugs 不是 PlugInSourceList 的实例），则不执行任何操作。
        /// </remarks>
        private static void Add(this IPlugInSourceList plugs, IPlugInSource plug) => (plugs as PlugInSourceList)?.Add(plug);
    }
}
