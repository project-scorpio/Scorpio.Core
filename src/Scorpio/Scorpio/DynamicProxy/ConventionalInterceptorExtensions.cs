using Scorpio.Conventional;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 为约定拦截器上下文提供扩展方法的静态类
    /// </summary>
    public static class ConventionalInterceptorExtensions
    {

        /// <summary>
        /// 为约定拦截器上下文添加指定类型的拦截器
        /// </summary>
        /// <typeparam name="TInterceptor">要添加的拦截器类型，必须实现 <see cref="IInterceptor"/> 接口</typeparam>
        /// <param name="context">约定拦截器上下文实例</param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalInterceptorAction}"/></returns>
        public static IConventionalContext<ConventionalInterceptorAction> Intercept<TInterceptor>(this IConventionalContext<ConventionalInterceptorAction> context)
            where TInterceptor : IInterceptor
        {
            // 获取或创建拦截器类型列表，并添加指定的拦截器类型
            var typeList = context.GetOrAdd(ConventionalInterceptorAction.Interceptors, n => new TypeList<IInterceptor>());
            typeList.Add<TInterceptor>();
            return context;
        }
    }
}
