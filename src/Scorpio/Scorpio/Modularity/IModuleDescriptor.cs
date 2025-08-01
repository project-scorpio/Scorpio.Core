using System;
using System.Collections.Generic;
using System.Reflection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 定义模块描述符的接口，用于描述和管理单个模块的详细信息
    /// </summary>
    /// <remarks>
    /// 该接口封装了模块的所有重要信息，包括类型定义、程序集引用、实例对象、
    /// 加载方式和依赖关系等。模块描述符是模块管理系统的核心数据结构，
    /// 为模块的生命周期管理、依赖解析和状态跟踪提供必要的信息。
    /// </remarks>
    public interface IModuleDescriptor
    {
        /// <summary>
        /// 获取模块的类型信息
        /// </summary>
        /// <value>
        /// 表示模块类的 <see cref="Type"/> 对象，该类型必须实现 <see cref="IScorpioModule"/> 接口。
        /// 通过此属性可以获取模块的完整类型信息，包括命名空间、泛型参数等。
        /// </value>
        Type Type { get; }

        /// <summary>
        /// 获取包含模块类型的程序集
        /// </summary>
        /// <value>
        /// 包含当前模块类型定义的 <see cref="Assembly"/> 对象。
        /// 可用于获取程序集级别的信息，如版本、元数据、资源等。
        /// </value>
        Assembly Assembly { get; }

        /// <summary>
        /// 获取模块的实例对象
        /// </summary>
        /// <value>
        /// 模块类型的实例化对象，实现了 <see cref="IScorpioModule"/> 接口。
        /// 此实例用于执行模块的生命周期方法，如配置服务、初始化和关闭等操作。
        /// </value>
        IScorpioModule Instance { get; }

        /// <summary>
        /// 获取一个值，指示模块是否作为插件加载
        /// </summary>
        /// <value>
        /// 如果模块是通过插件机制动态加载的，则为 <c>true</c>；
        /// 如果模块是静态引用或直接注册的，则为 <c>false</c>。
        /// </value>
        /// <remarks>
        /// 插件模块通常是在运行时从外部程序集或文件中发现和加载的，
        /// 而非插件模块则是在编译时就确定的静态依赖。
        /// 这个属性有助于区分不同的模块加载策略和管理方式。
        /// </remarks>
        bool IsLoadedAsPlugIn { get; }

        /// <summary>
        /// 获取当前模块依赖的其他模块描述符列表
        /// </summary>
        /// <value>
        /// 包含所有直接依赖模块描述符的只读列表。列表中的每个 <see cref="IModuleDescriptor"/>
        /// 表示当前模块所依赖的一个模块。依赖模块必须在当前模块之前加载和初始化。
        /// </value>
        /// <remarks>
        /// 此属性仅包含直接依赖关系，不包括传递依赖。模块系统会自动解析
        /// 和管理完整的依赖图，确保所有依赖项按正确的顺序加载。
        /// 依赖关系通常通过 <see cref="DependsOnAttribute"/> 特性声明。
        /// </remarks>
        IReadOnlyList<IModuleDescriptor> Dependencies { get; }
    }
}