using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.Conventional
{
    /// <summary>
    /// 生命周期选择器的内部实现类，实现 <see cref="IRegisterAssemblyLifetimeSelector"/> 接口
    /// </summary>
    internal class LifetimeSelector : IRegisterAssemblyLifetimeSelector
    {
        /// <summary>
        /// 获取瞬态生命周期选择器的静态实例
        /// </summary>
        /// <value>瞬态生命周期选择器 <see cref="LifetimeSelector"/></value>
        public static LifetimeSelector Transient { get; } = new LifetimeSelector(ServiceLifetime.Transient);

        /// <summary>
        /// 获取作用域生命周期选择器的静态实例
        /// </summary>
        /// <value>作用域生命周期选择器 <see cref="LifetimeSelector"/></value>
        public static LifetimeSelector Scoped { get; } = new LifetimeSelector(ServiceLifetime.Scoped);

        /// <summary>
        /// 获取单例生命周期选择器的静态实例
        /// </summary>
        /// <value>单例生命周期选择器 <see cref="LifetimeSelector"/></value>
        public static LifetimeSelector Singleton { get; } = new LifetimeSelector(ServiceLifetime.Singleton);

        /// <summary>
        /// 根据服务生命周期枚举获取对应的生命周期选择器
        /// </summary>
        /// <param name="serviceLifetime">服务生命周期枚举值 <see cref="ServiceLifetime"/></param>
        /// <returns>返回对应的生命周期选择器实例 <see cref="LifetimeSelector"/></returns>
        /// <exception cref="NotImplementedException">当传入未支持的生命周期枚举值时抛出</exception>
        public static LifetimeSelector GetSelector(ServiceLifetime serviceLifetime) => serviceLifetime switch
        {
            ServiceLifetime.Singleton => Singleton,
            ServiceLifetime.Scoped => Scoped,
            ServiceLifetime.Transient => Transient,
            _ => throw new NotImplementedException(),
        };

        /// <summary>
        /// 存储生命周期枚举值的私有字段
        /// </summary>
        private readonly ServiceLifetime _lifetime;

        /// <summary>
        /// 初始化生命周期选择器的新实例
        /// </summary>
        /// <param name="lifetime">要使用的服务生命周期 <see cref="ServiceLifetime"/></param>
        public LifetimeSelector(ServiceLifetime lifetime) => _lifetime = lifetime;

        /// <summary>
        /// 根据组件类型选择其服务生命周期
        /// </summary>
        /// <param name="componentType">要确定生命周期的组件类型</param>
        /// <returns>返回预设的服务生命周期 <see cref="ServiceLifetime"/></returns>
        public ServiceLifetime Select(Type componentType) => _lifetime;


    }

    /// <summary>
    /// 暴露服务生命周期选择器的内部实现类，实现 <see cref="IRegisterAssemblyLifetimeSelector"/> 接口
    /// </summary>
    /// <remarks>
    /// 根据类型上的 <see cref="ExposeServicesAttribute"/> 特性来确定服务生命周期
    /// </remarks>
    internal class ExposeLifetimeSelector : IRegisterAssemblyLifetimeSelector
    {
        /// <summary>
        /// 获取暴露服务生命周期选择器的静态实例
        /// </summary>
        /// <value>暴露服务生命周期选择器实例 <see cref="ExposeLifetimeSelector"/></value>
        public static ExposeLifetimeSelector Instance { get; } = new ExposeLifetimeSelector();

        /// <summary>
        /// 根据组件类型上的 <see cref="ExposeServicesAttribute"/> 特性选择其服务生命周期
        /// </summary>
        /// <param name="componentType">要确定生命周期的组件类型</param>
        /// <returns>返回特性中指定的服务生命周期，如果未指定则返回瞬态生命周期 <see cref="ServiceLifetime"/></returns>
        public ServiceLifetime Select(Type componentType)
        {
            // 获取类型上的 ExposeServicesAttribute 特性
            var attr = componentType.GetAttribute<ExposeServicesAttribute>(true);
            // 返回特性中指定的生命周期，如果特性不存在则默认为瞬态
            return attr?.ServiceLifetime ?? ServiceLifetime.Transient;
        }
    }
}
