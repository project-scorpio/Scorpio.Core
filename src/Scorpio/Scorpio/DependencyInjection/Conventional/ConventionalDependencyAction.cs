using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;

namespace Scorpio.DependencyInjection.Conventional
{
    /// <summary>
    /// 约定依赖注入操作类，继承自 <see cref="ConventionalActionBase"/>
    /// </summary>
    /// <remarks>
    /// 用于执行依赖注入的约定操作，根据服务选择器和生命周期选择器自动注册服务
    /// </remarks>
    public sealed class ConventionalDependencyAction : ConventionalActionBase
    {

        /// <summary>
        /// 初始化约定依赖注入操作的新实例
        /// </summary>
        /// <param name="configuration">约定配置实例 <see cref="IConventionalConfiguration"/></param>
        internal ConventionalDependencyAction(IConventionalConfiguration configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// 执行约定依赖注入操作的核心逻辑
        /// </summary>
        /// <param name="context">约定上下文实例 <see cref="IConventionalContext"/></param>
        protected override void Action(IConventionalContext context)
        {
            // 遍历上下文中的所有类型
            context.Types.ForEach(
                t => context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service").ForEach(
                    selector => selector.Select(t).ForEach(
                    s => context.Services.ReplaceOrAdd(
                        // 创建服务描述符，包含服务类型、实现类型和生命周期
                        ServiceDescriptor.Describe(s, t,
                        context.GetOrAdd<IRegisterAssemblyLifetimeSelector>("Lifetime",
                       t => LifetimeSelector.Transient).Select(t)),
                        // 检查是否需要替换现有服务
                        t.GetAttribute<ReplaceServiceAttribute>()?.ReplaceService ?? false
                        ))));
        }
    }
}
