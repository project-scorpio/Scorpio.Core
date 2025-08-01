using System;

namespace Scorpio.Modularity.Plugins
{
    /// <summary>
    /// 基于类型的插件源实现类，用于直接提供预定义的模块类型
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IPlugInSource"/> 接口，通过直接持有模块类型数组来提供模块。
    /// 这种插件源适用于已知模块类型并需要直接注册的场景，无需从文件或程序集中动态发现模块。
    /// </remarks>
    internal class TypePlugInSource : IPlugInSource
    {
        /// <summary>
        /// 存储要提供的模块类型数组
        /// </summary>
        private readonly Type[] _moduleTypes;

        /// <summary>
        /// 初始化 <see cref="TypePlugInSource"/> 类的新实例
        /// </summary>
        /// <param name="moduleTypes">要提供的模块类型数组</param>
        /// <remarks>
        /// 如果传入的 <paramref name="moduleTypes"/> 为 null，则内部将使用空数组以避免空引用异常。
        /// 该构造函数使用 params 修饰符，支持传入可变数量的类型参数。
        /// </remarks>
        public TypePlugInSource(params Type[] moduleTypes) => _moduleTypes = moduleTypes ?? new Type[0];

        /// <summary>
        /// 获取当前插件源中的所有模块类型
        /// </summary>
        /// <returns>
        /// 包含所有预定义模块类型的数组。如果构造时未提供任何模块类型，则返回空数组。
        /// </returns>
        /// <remarks>
        /// 该方法直接返回构造时提供的模块类型数组，不执行任何动态发现或验证逻辑。
        /// 调用者应确保提供的类型是有效的 Scorpio 模块类型。
        /// </remarks>
        public Type[] GetModules() => _moduleTypes;
    }
}
