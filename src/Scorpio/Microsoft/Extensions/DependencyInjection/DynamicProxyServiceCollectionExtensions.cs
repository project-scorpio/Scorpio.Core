using System;
using System.Collections.Generic;

using Scorpio.Conventional;
using Scorpio.DynamicProxy;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 动态代理服务集合扩展类
    /// 提供基于约定的拦截器注册功能，支持AOP（面向切面编程）场景
    /// </summary>
    public static class DynamicProxyServiceCollectionExtensions
    {
        /// <summary>
        /// 根据约定注册拦截器
        /// 扫描指定的类型集合，根据配置的约定规则自动注册相关的拦截器到依赖注入容器
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="types">要扫描的类型集合，这些类型将被检查是否需要应用拦截器</param>
        /// <param name="configureAction">配置约定的操作，用于定义拦截器的应用规则和配置</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ConventionalInterceptorAction"/>
        /// <seealso cref="IConventionalConfiguration{T}"/>
        public static IServiceCollection RegisterConventionalInterceptor(
            this IServiceCollection services,
            IEnumerable<Type> types,
            Action<IConventionalConfiguration<ConventionalInterceptorAction>> configureAction) =>
            // 委托给通用的约定操作处理方法，实现拦截器的自动注册
            services.DoConventionalAction(types, configureAction);
    }
}
