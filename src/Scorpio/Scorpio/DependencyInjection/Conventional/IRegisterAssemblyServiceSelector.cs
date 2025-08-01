using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.Conventional
{
    /// <summary>
    /// 程序集注册服务选择器接口，用于选择类型应该注册为哪些服务类型
    /// </summary>
    public interface IRegisterAssemblyServiceSelector
    {
        /// <summary>
        /// 根据组件类型选择应该注册的服务类型集合
        /// </summary>
        /// <param name="componentType">要注册的组件类型</param>
        /// <returns>返回该组件类型应该注册为的服务类型集合 <see cref="IEnumerable{Type}"/></returns>
        IEnumerable<Type> Select(Type componentType);
    }

    /// <summary>
    /// 程序集注册生命周期选择器接口，用于确定类型的服务生命周期
    /// </summary>
    public interface IRegisterAssemblyLifetimeSelector
    {
        /// <summary>
        /// 根据组件类型选择其服务生命周期
        /// </summary>
        /// <param name="componentType">要确定生命周期的组件类型</param>
        /// <returns>返回该组件类型的服务生命周期 <see cref="ServiceLifetime"/></returns>
        ServiceLifetime Select(Type componentType);
    }

}
