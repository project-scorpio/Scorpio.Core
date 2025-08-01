using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 约定拦截器操作类，继承自 <see cref="ConventionalActionBase"/>
    /// </summary>
    /// <remarks>
    /// 用于执行动态代理拦截器的约定操作，通过代理约定操作上下文来处理拦截器的注册和配置
    /// </remarks>
    public sealed class ConventionalInterceptorAction : ConventionalActionBase
    {
        /// <summary>
        /// 拦截器常量键名，用于在上下文中存储和获取拦截器列表
        /// </summary>
        internal const string Interceptors = "Interceptors";

        /// <summary>
        /// 初始化约定拦截器操作的新实例
        /// </summary>
        /// <param name="configuration">约定配置实例 <see cref="IConventionalConfiguration"/></param>
        internal ConventionalInterceptorAction(IConventionalConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// 执行约定拦截器操作的核心逻辑
        /// </summary>
        /// <param name="context">约定上下文实例 <see cref="IConventionalContext"/></param>
        protected override void Action(IConventionalContext context)
        {
            // 从服务集合中获取代理约定操作实例
            var action = context.Services.GetSingletonInstanceOrNull<IProxyConventionalAction>();
            if (action == null)
            {
                return;
            }
            // 从上下文中获取拦截器类型列表
            var typeList = context.GetOrDefault(Interceptors, default(ITypeList<IInterceptor>));
            // 创建代理约定操作上下文，只包含标准类型
            var ctx = new ProxyConventionalActionContext(context.Services, context.Types.Where(t => t.IsStandardType()), context.TypePredicate, typeList);
            // 执行代理约定操作
            action.Action(ctx);
        }
    }
}
