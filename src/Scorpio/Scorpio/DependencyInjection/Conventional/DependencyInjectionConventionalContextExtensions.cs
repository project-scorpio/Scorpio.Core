using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Scorpio.Conventional;

namespace Scorpio.DependencyInjection.Conventional
{
    /// <summary>
    /// 为依赖注入约定上下文提供扩展方法的静态类
    /// </summary>
    public static class DependencyInjectionConventionalContextExtensions
    {
        /// <summary>
        /// 设置服务的生命周期
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <param name="serviceLifetime">服务生命周期枚举值 <see cref="ServiceLifetime"/></param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> Lifetime(this IConventionalContext<ConventionalDependencyAction> context, ServiceLifetime serviceLifetime)
        {
            // 根据生命周期枚举获取对应的生命周期选择器并设置到上下文中
            context.Set("Lifetime", LifetimeSelector.GetSelector(serviceLifetime));
            return context;
        }

        /// <summary>
        /// 使用自定义生命周期选择器设置服务的生命周期
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <param name="lifetimeSelector">自定义生命周期选择器 <see cref="IRegisterAssemblyLifetimeSelector"/></param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> Lifetime(this IConventionalContext<ConventionalDependencyAction> context, IRegisterAssemblyLifetimeSelector lifetimeSelector)
        {
            // 将自定义生命周期选择器设置到上下文中
            context.Set("Lifetime", lifetimeSelector);
            return context;
        }

        /// <summary>
        /// 添加服务选择器来指定服务的注册方式
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <param name="serviceSelector">服务选择器实例 <see cref="IRegisterAssemblyServiceSelector"/></param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> As(this IConventionalContext<ConventionalDependencyAction> context, IRegisterAssemblyServiceSelector serviceSelector)
        {
            // 获取或创建服务选择器集合，并添加新的选择器
            context.GetOrAdd<ICollection<IRegisterAssemblyServiceSelector>>("Service", new HashSet<IRegisterAssemblyServiceSelector>()).Add(serviceSelector);
            return context;
        }

        /// <summary>
        /// 将服务注册为指定的泛型类型
        /// </summary>
        /// <typeparam name="T">要注册的服务类型</typeparam>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> As<T>(this IConventionalContext<ConventionalDependencyAction> context)
        {
            // 使用类型选择器注册为指定的泛型类型
            context.As(new TypeSelector<T>());
            return context;
        }

        /// <summary>
        /// 将服务注册为其默认接口
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> AsDefault(this IConventionalContext<ConventionalDependencyAction> context)
        {
            // 使用默认接口选择器进行注册
            context.As(DefaultInterfaceSelector.Instance);
            return context;
        }

        /// <summary>
        /// 将服务注册为其实现的所有接口
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> AsAll(this IConventionalContext<ConventionalDependencyAction> context)
        {
            // 使用所有接口选择器进行注册
            context.As(AllInterfaceSelector.Instance);
            return context;
        }

        /// <summary>
        /// 将服务注册为其自身类型
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> AsSelf(this IConventionalContext<ConventionalDependencyAction> context)
        {
            // 使用自身选择器进行注册
            context.As(SelfSelector.Instance);
            return context;
        }

        /// <summary>
        /// 将服务注册为通过 <see cref="ExposeServicesAttribute"/> 特性暴露的服务
        /// </summary>
        /// <param name="context">约定依赖注入上下文实例</param>
        /// <returns>返回约定上下文实例以支持链式调用 <see cref="IConventionalContext{ConventionalDependencyAction}"/></returns>
        public static IConventionalContext<ConventionalDependencyAction> AsExposeService(this IConventionalContext<ConventionalDependencyAction> context)
        {
            // 使用暴露服务选择器和暴露生命周期选择器进行注册
            context.As(ExposeServicesSelector.Instance).Lifetime(ExposeLifetimeSelector.Instance);
            return context;
        }
    }
}
