using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;

namespace Scorpio.DependencyInjection.Conventional
{
    /// <summary>
    /// 基础约定注册器的内部实现类，实现 <see cref="IConventionalRegistrar"/> 接口
    /// </summary>
    /// <remarks>
    /// 提供标准类型的依赖注入约定注册，包括单例、瞬态、作用域生命周期以及服务暴露的自动注册
    /// </remarks>
    internal class BasicConventionalRegistrar : IConventionalRegistrar
    {
        /// <summary>
        /// 执行约定注册逻辑，根据类型特征自动注册依赖注入服务
        /// </summary>
        /// <param name="context">约定注册上下文，包含类型集合和服务集合 <see cref="IConventionalRegistrationContext"/></param>
        public void Register(IConventionalRegistrationContext context)
        {
            // 注册约定依赖注入配置
            context.RegisterConventionalDependencyInject(config =>
            {
                // 注册实现了 ISingletonDependency 接口的标准类型为单例生命周期
                config.Where(t => t.IsStandardType()).Where(t => t.IsAssignableTo<ISingletonDependency>()).AsDefault().Lifetime(ServiceLifetime.Singleton);
                // 注册实现了 ITransientDependency 接口的标准类型为瞬态生命周期
                config.Where(t => t.IsStandardType()).Where(t => t.IsAssignableTo<ITransientDependency>()).AsDefault().Lifetime(ServiceLifetime.Transient);
                // 注册实现了 IScopedDependency 接口的标准类型为作用域生命周期
                config.Where(t => t.IsStandardType()).Where(t => t.IsAssignableTo<IScopedDependency>()).AsDefault().Lifetime(ServiceLifetime.Scoped);
                // 注册标记了 ExposeServicesAttribute 特性的标准类型为暴露服务
                config.Where(t => t.IsStandardType()).Where(t => t.AttributeExists<ExposeServicesAttribute>(false)).AsExposeService();
            });
        }
    }
}
