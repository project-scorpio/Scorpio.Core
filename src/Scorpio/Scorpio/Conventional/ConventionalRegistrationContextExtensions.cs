using System;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.DependencyInjection.Conventional;
using Scorpio.DynamicProxy;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 为约定注册上下文提供扩展方法的静态类
    /// </summary>
    public static class ConventionalRegistrationContextExtensions
    {
        /// <summary>
        /// 在约定注册上下文中执行指定的约定操作
        /// </summary>
        /// <typeparam name="TAction">约定操作类型，必须继承自 <see cref="ConventionalActionBase"/></typeparam>
        /// <param name="context">约定注册上下文实例</param>
        /// <param name="configureAction">用于配置约定操作的委托</param>
        /// <returns>返回约定注册上下文实例以支持链式调用 <see cref="IConventionalRegistrationContext"/></returns>
        public static IConventionalRegistrationContext DoConventionalAction<TAction>(this IConventionalRegistrationContext context, Action<IConventionalConfiguration<TAction>> configureAction) where TAction : ConventionalActionBase
        {
            // 在服务集合中对指定类型执行约定操作
            context.Services.DoConventionalAction(context.Types, configureAction);
            return context;
        }

        /// <summary>
        /// 在约定注册上下文中注册约定依赖注入
        /// </summary>
        /// <param name="context">约定注册上下文实例</param>
        /// <param name="configureAction">用于配置依赖注入约定的委托</param>
        /// <returns>返回约定注册上下文实例以支持链式调用 <see cref="IConventionalRegistrationContext"/></returns>
        public static IConventionalRegistrationContext RegisterConventionalDependencyInject(this IConventionalRegistrationContext context, Action<IConventionalConfiguration<ConventionalDependencyAction>> configureAction) => context.DoConventionalAction(configureAction);

        /// <summary>
        /// 在约定注册上下文中注册约定拦截器
        /// </summary>
        /// <param name="context">约定注册上下文实例</param>
        /// <param name="configureAction">用于配置拦截器约定的委托</param>
        /// <returns>返回约定注册上下文实例以支持链式调用 <see cref="IConventionalRegistrationContext"/></returns>
        public static IConventionalRegistrationContext RegisterConventionalInterceptor(
            this IConventionalRegistrationContext context,
            Action<IConventionalConfiguration<ConventionalInterceptorAction>> configureAction)
        {
            // 在服务集合中注册约定拦截器
            context.Services.RegisterConventionalInterceptor(context.Types, configureAction);
            return context;
        }
    }
}
