using System.Collections.Generic;

namespace Scorpio.Aspects
{
    /// <summary>
    /// 避免重复横切关注点接口
    /// 定义了跟踪和管理横切关注点应用状态的契约
    /// 实现此接口的对象可以记录已应用的横切关注点，防止AOP场景中的重复处理和无限递归
    /// </summary>
    /// <seealso cref="CrossCuttingConcerns"/>
    /// <remarks>
    /// 此接口通常由需要支持AOP功能的实体类实现，用于：
    /// <list type="bullet">
    /// <item><description>记录已应用的拦截器或切面</description></item>
    /// <item><description>避免在嵌套调用中重复应用相同的横切关注点</description></item>
    /// <item><description>防止AOP拦截器的无限递归调用</description></item>
    /// <item><description>提供横切关注点的状态查询能力</description></item>
    /// </list>
    /// </remarks>
    public interface IAvoidDuplicateCrossCuttingConcerns
    {
        /// <summary>
        /// 已应用的横切关注点列表
        /// 存储当前对象上已经应用的所有横切关注点名称
        /// 用于跟踪哪些AOP功能（如日志记录、缓存、事务等）已经被应用到此对象上
        /// </summary>
        /// <value>
        /// 包含横切关注点名称的字符串列表，每个名称代表一个已应用的AOP功能
        /// </value>
        /// <seealso cref="CrossCuttingConcerns.AddApplied"/>
        /// <seealso cref="CrossCuttingConcerns.RemoveApplied"/>
        /// <seealso cref="CrossCuttingConcerns.IsApplied"/>
        /// <seealso cref="CrossCuttingConcerns.GetApplieds"/>
        /// <remarks>
        /// 此列表应该支持以下操作：
        /// <list type="bullet">
        /// <item><description>添加新的横切关注点名称</description></item>
        /// <item><description>移除已存在的横切关注点名称</description></item>
        /// <item><description>查询特定横切关注点是否存在</description></item>
        /// <item><description>获取所有已应用的横切关注点</description></item>
        /// </list>
        /// 实现时建议使用线程安全的集合，以支持多线程环境下的并发访问。
        /// </remarks>
        List<string> AppliedCrossCuttingConcerns { get; }
    }
}
