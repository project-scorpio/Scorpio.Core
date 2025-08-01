using System.Collections.Generic;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 定义模块容器的接口，用于管理和访问应用程序中的模块集合
    /// </summary>
    /// <remarks>
    /// 该接口为模块管理系统提供统一的访问入口，允许查询和获取已加载的模块信息。
    /// 模块容器负责维护模块的生命周期和状态，为依赖注入和模块协调提供基础支持。
    /// </remarks>
    public interface IModuleContainer
    {
        /// <summary>
        /// 获取容器中所有模块的只读列表
        /// </summary>
        /// <value>
        /// 包含所有模块描述符的只读集合。每个 <see cref="IModuleDescriptor"/> 实例
        /// 包含模块的类型信息、依赖关系、状态等详细信息。
        /// 集合按照模块的加载顺序或依赖关系进行排序。
        /// </value>
        /// <remarks>
        /// 此属性提供对模块集合的只读访问，确保外部代码无法直接修改模块容器的内部状态。
        /// 模块的添加、移除和状态变更应通过容器提供的专门方法进行。
        /// </remarks>
        IReadOnlyList<IModuleDescriptor> Modules { get; }
    }
}
