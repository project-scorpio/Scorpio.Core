using System;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 定义依赖类型提供程序的接口，用于获取类型依赖关系
    /// </summary>
    /// <remarks>
    /// 该接口定义了获取类型依赖关系的标准协议。实现此接口的类或特性可以提供
    /// 当前类型所依赖的其他类型信息，用于依赖解析、加载顺序管理等场景。
    /// 常见的实现包括 <see cref="DependsOnAttribute"/> 等依赖关系特性。
    /// </remarks>
    public interface IDependedTypesProvider
    {
        /// <summary>
        /// 获取当前类型依赖的所有类型
        /// </summary>
        /// <returns>
        /// 包含所有依赖类型的数组。如果没有依赖关系，则返回空数组。
        /// 返回的类型将用于确定加载或初始化的顺序，确保依赖项在当前类型之前被处理。
        /// </returns>
        /// <remarks>
        /// 此方法应返回当前类型直接依赖的所有类型。依赖关系可以是：
        /// <list type="bullet">
        /// <item><description>模块依赖：当前模块需要其他模块先加载</description></item>
        /// <item><description>服务依赖：当前服务需要其他服务先注册</description></item>
        /// <item><description>组件依赖：当前组件需要其他组件先初始化</description></item>
        /// </list>
        /// 实现时应确保返回值不为 null，如无依赖则返回空数组。
        /// </remarks>
        Type[] GetDependedTypes();
    }
}
