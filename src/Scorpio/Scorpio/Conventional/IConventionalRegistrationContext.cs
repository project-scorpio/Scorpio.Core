using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定注册上下文接口，用于在约定注册过程中传递所需的对象
    /// </summary>
    public interface IConventionalRegistrationContext
    {

        /// <summary>
        /// 获取程序集中的所有类型集合
        /// </summary>
        /// <value>类型枚举 <see cref="IEnumerable{Type}"/></value>
        IEnumerable<Type> Types { get; }

        /// <summary>
        /// 获取用于注册类型的IOC容器服务集合
        /// </summary>
        /// <value>依赖注入服务集合 <see cref="IServiceCollection"/></value>
        IServiceCollection Services { get; }
        /// <inheritdoc/>
        Assembly Assembly { get; }
    }
}