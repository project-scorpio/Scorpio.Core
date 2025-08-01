using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scorpio.DependencyInjection.Conventional
{
    /// <summary>
    /// 默认接口选择器的内部实现类，实现 <see cref="IRegisterAssemblyServiceSelector"/> 接口
    /// </summary>
    /// <remarks>
    /// 选择与类型名称匹配的接口以及类型本身作为服务
    /// </remarks>
    internal class DefaultInterfaceSelector : IRegisterAssemblyServiceSelector
    {
        /// <summary>
        /// 获取默认接口选择器的静态实例
        /// </summary>
        /// <value>默认接口选择器实例 <see cref="DefaultInterfaceSelector"/></value>
        public static DefaultInterfaceSelector Instance { get; } = new DefaultInterfaceSelector();

        /// <summary>
        /// 初始化默认接口选择器的新实例
        /// </summary>
        public DefaultInterfaceSelector()
        {
        }

        /// <summary>
        /// 根据组件类型选择应该注册的服务类型集合
        /// </summary>
        /// <param name="componentType">要注册的组件类型</param>
        /// <returns>返回匹配的接口类型和组件类型本身的集合 <see cref="IEnumerable{Type}"/></returns>
        public IEnumerable<Type> Select(Type componentType)
        {
            // 筛选出类型名称与接口名称匹配的接口（移除接口名称的"I"前缀后匹配）
            var services = componentType.GetInterfaces().Where(s => componentType.Name.EndsWith(s.Name.RemovePreFix("I"))).ToList();
            // 添加组件类型本身
            services.Add(componentType);
            return services;
        }
    }

    /// <summary>
    /// 全部接口选择器的内部实现类，实现 <see cref="IRegisterAssemblyServiceSelector"/> 接口
    /// </summary>
    /// <remarks>
    /// 选择类型实现的所有公共接口以及类型本身作为服务
    /// </remarks>
    internal class AllInterfaceSelector : IRegisterAssemblyServiceSelector
    {
        /// <summary>
        /// 获取全部接口选择器的静态实例
        /// </summary>
        /// <value>全部接口选择器实例 <see cref="AllInterfaceSelector"/></value>
        public static AllInterfaceSelector Instance { get; } = new AllInterfaceSelector();

        /// <summary>
        /// 初始化全部接口选择器的新实例
        /// </summary>
        public AllInterfaceSelector()
        {
        }

        /// <summary>
        /// 根据组件类型选择应该注册的服务类型集合
        /// </summary>
        /// <param name="componentType">要注册的组件类型</param>
        /// <returns>返回所有公共接口类型和组件类型本身的集合 <see cref="IEnumerable{Type}"/></returns>
        public IEnumerable<Type> Select(Type componentType)
        {
            // 获取所有公共接口
            var services = componentType.GetInterfaces().Where(s => s.IsPublic).ToList();
            // 添加组件类型本身
            services.Add(componentType);
            return services;
        }
    }

    /// <summary>
    /// 自身选择器的内部实现类，实现 <see cref="IRegisterAssemblyServiceSelector"/> 接口
    /// </summary>
    /// <remarks>
    /// 只选择类型本身作为服务，不包含任何接口
    /// </remarks>
    internal class SelfSelector : IRegisterAssemblyServiceSelector
    {
        /// <summary>
        /// 获取自身选择器的静态实例
        /// </summary>
        /// <value>自身选择器实例 <see cref="SelfSelector"/></value>
        public static SelfSelector Instance { get; } = new SelfSelector();

        /// <summary>
        /// 根据组件类型选择应该注册的服务类型集合
        /// </summary>
        /// <param name="componentType">要注册的组件类型</param>
        /// <returns>返回只包含组件类型本身的集合 <see cref="IEnumerable{Type}"/></returns>
        public IEnumerable<Type> Select(Type componentType) => new Type[] { componentType };
    }

    /// <summary>
    /// 泛型类型选择器的内部实现类，实现 <see cref="IRegisterAssemblyServiceSelector"/> 接口
    /// </summary>
    /// <typeparam name="T">要注册为的服务类型</typeparam>
    /// <remarks>
    /// 将组件类型注册为指定的泛型类型 <typeparamref name="T"/>
    /// </remarks>
    internal class TypeSelector<T> : IRegisterAssemblyServiceSelector
    {
        /// <summary>
        /// 根据组件类型选择应该注册的服务类型集合
        /// </summary>
        /// <param name="componentType">要注册的组件类型</param>
        /// <returns>返回只包含泛型类型 <typeparamref name="T"/> 的集合 <see cref="IEnumerable{Type}"/></returns>
        public IEnumerable<Type> Select(Type componentType) => new Type[] { typeof(T) };
    }

    /// <summary>
    /// 暴露服务选择器的内部实现类，实现 <see cref="IRegisterAssemblyServiceSelector"/> 接口
    /// </summary>
    /// <remarks>
    /// 根据类型上的 <see cref="ExposeServicesAttribute"/> 特性来确定要暴露的服务类型
    /// </remarks>
    internal class ExposeServicesSelector : IRegisterAssemblyServiceSelector
    {
        /// <summary>
        /// 获取暴露服务选择器的静态实例
        /// </summary>
        /// <value>暴露服务选择器实例 <see cref="ExposeServicesSelector"/></value>
        public static ExposeServicesSelector Instance { get; } = new ExposeServicesSelector();

        /// <summary>
        /// 根据组件类型选择应该注册的服务类型集合
        /// </summary>
        /// <param name="componentType">要注册的组件类型</param>
        /// <returns>返回通过 <see cref="ExposeServicesAttribute"/> 特性暴露的服务类型集合 <see cref="IEnumerable{Type}"/></returns>
        public IEnumerable<Type> Select(Type componentType)
        {
            // 获取类型上的 ExposeServicesAttribute 特性
            var attr = componentType.GetAttribute<ExposeServicesAttribute>(inherit: true);
            // 返回特性中定义的暴露服务类型
            return attr.GetExposedServiceTypes(componentType);
        }
    }
}
