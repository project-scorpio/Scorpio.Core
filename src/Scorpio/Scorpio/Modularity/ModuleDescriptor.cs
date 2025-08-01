using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 模块描述符的实现类，用于描述和管理单个模块的详细信息
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IModuleDescriptor"/> 接口，封装了模块的所有重要信息，
    /// 包括类型定义、程序集引用、实例对象、加载方式和依赖关系等。
    /// 作为模块管理系统的核心数据结构，为模块的生命周期管理、依赖解析和状态跟踪提供支持。
    /// </remarks>
    internal class ModuleDescriptor : IModuleDescriptor
    {
        /// <summary>
        /// 获取模块的类型信息
        /// </summary>
        /// <value>
        /// 表示模块类的 <see cref="Type"/> 对象，该类型必须实现 <see cref="IScorpioModule"/> 接口。
        /// 通过此属性可以获取模块的完整类型信息，包括命名空间、泛型参数等。
        /// </value>
        public Type Type { get; }

        /// <summary>
        /// 获取包含模块类型的程序集
        /// </summary>
        /// <value>
        /// 包含当前模块类型定义的 <see cref="Assembly"/> 对象。
        /// 可用于获取程序集级别的信息，如版本、元数据、资源等。
        /// </value>
        public Assembly Assembly { get; }

        /// <summary>
        /// 获取模块的实例对象
        /// </summary>
        /// <value>
        /// 模块类型的实例化对象，实现了 <see cref="IScorpioModule"/> 接口。
        /// 此实例用于执行模块的生命周期方法，如配置服务、初始化和关闭等操作。
        /// </value>
        public IScorpioModule Instance { get; }

        /// <summary>
        /// 获取一个值，指示模块是否作为插件加载
        /// </summary>
        /// <value>
        /// 如果模块是通过插件机制动态加载的，则为 <c>true</c>；
        /// 如果模块是静态引用或直接注册的，则为 <c>false</c>。
        /// </value>
        public bool IsLoadedAsPlugIn { get; }

        /// <summary>
        /// 获取当前模块依赖的其他模块描述符列表
        /// </summary>
        /// <value>
        /// 包含所有直接依赖模块描述符的只读列表。列表中的每个 <see cref="IModuleDescriptor"/>
        /// 表示当前模块所依赖的一个模块。依赖模块必须在当前模块之前加载和初始化。
        /// </value>
        public IReadOnlyList<IModuleDescriptor> Dependencies => _dependencies.ToImmutableList();

        /// <summary>
        /// 存储模块依赖关系的内部列表
        /// </summary>
        private readonly List<IModuleDescriptor> _dependencies;

        /// <summary>
        /// 初始化 <see cref="ModuleDescriptor"/> 类的新实例
        /// </summary>
        /// <param name="type">模块的类型，必须实现 <see cref="IScorpioModule"/> 接口</param>
        /// <param name="instance">模块的实例对象</param>
        /// <param name="isLoadedAsPlugIn">指示模块是否作为插件加载</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 或 <paramref name="instance"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当 <paramref name="instance"/> 不是 <paramref name="type"/> 的实例时抛出</exception>
        /// <remarks>
        /// 构造函数会验证传入的实例是否与指定的类型匹配，确保类型安全性。
        /// 同时会自动从类型中获取程序集信息，并初始化依赖关系列表。
        /// </remarks>
        public ModuleDescriptor(
             Type type,
             IScorpioModule instance,
            bool isLoadedAsPlugIn)
        {
            // 验证参数不为 null
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            // 验证实例是否为指定类型的实例
            if (!type.GetTypeInfo().IsInstanceOfType(instance))
            {
                throw new ArgumentException($"Given module instance ({instance.GetType().AssemblyQualifiedName}) is not an instance of given module type: {type.AssemblyQualifiedName}");
            }

            // 设置模块的基本属性
            Type = type;
            Assembly = type.Assembly;  // 从类型获取程序集信息
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;

            // 初始化依赖关系列表
            _dependencies = new List<IModuleDescriptor>();
        }


#pragma warning disable CS1574 // XML 注释中有无法解析的 cref 特性
        /// <summary>
        /// 向当前模块添加依赖关系
        /// </summary>
        /// <param name="descriptor">要添加的依赖模块描述符</param>
        /// <remarks>
        /// 该方法使用 <see cref="CollectionExtensions.AddIfNotContains{T}(ICollection{T}, T)"/> 扩展方法，
        /// 确保同一个依赖不会被重复添加。这有助于避免依赖关系列表中的重复项。
        /// </remarks>
        public void AddDependency(IModuleDescriptor descriptor) => _dependencies.AddIfNotContains(descriptor);
#pragma warning restore CS1574 // XML 注释中有无法解析的 cref 特性

        /// <summary>
        /// 返回表示当前模块描述符的字符串
        /// </summary>
        /// <returns>包含模块完整类型名称的字符串表示形式</returns>
        /// <remarks>
        /// 该方法重写了 <see cref="Object.ToString"/> 方法，提供有意义的字符串表示，
        /// 便于调试和日志记录时识别模块。
        /// </remarks>
        public override string ToString() => $"[ModuleDescriptor {Type.FullName}]";
    }
}
