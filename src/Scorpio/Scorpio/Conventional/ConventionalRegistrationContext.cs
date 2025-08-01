using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定注册上下文的内部实现类，实现 <see cref="IConventionalRegistrationContext"/> 接口
    /// </summary>
    internal class ConventionalRegistrationContext : IConventionalRegistrationContext
    {
        /// <summary>
        /// 获取服务集合实例
        /// </summary>
        /// <value>依赖注入服务集合 <see cref="IServiceCollection"/></value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取程序集中的所有类型集合
        /// </summary>
        /// <value>类型枚举 <see cref="IEnumerable{Type}"/></value>
        public IEnumerable<Type> Types { get; }

        /// <summary>
        /// 初始化约定注册上下文的新实例
        /// </summary>
        /// <param name="assembly">要扫描类型的程序集</param>
        /// <param name="services">依赖注入服务集合</param>
        internal ConventionalRegistrationContext(Assembly assembly, IServiceCollection services)
        {
            // 获取程序集中的所有类型
            Types = assembly.GetTypes();
            Services = services;
        }
    }
}
