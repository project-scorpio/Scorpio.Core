using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 代理约定操作上下文的内部实现类，实现 <see cref="IProxyConventionalActionContext"/> 接口
    /// </summary>
    /// <remarks>
    /// 封装了执行代理约定操作时所需的所有上下文信息，包括服务集合、类型集合、类型筛选谓词和拦截器列表
    /// </remarks>
    internal sealed class ProxyConventionalActionContext : IProxyConventionalActionContext
    {
        /// <summary>
        /// 获取依赖注入服务集合
        /// </summary>
        /// <value>服务集合实例 <see cref="IServiceCollection"/></value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取当前上下文中的类型集合
        /// </summary>
        /// <value>类型枚举 <see cref="IEnumerable{Type}"/></value>
        public IEnumerable<Type> Types { get; }

        /// <summary>
        /// 获取用于筛选类型的谓词表达式
        /// </summary>
        /// <value>类型筛选谓词表达式 <see cref="Expression{Func}"/></value>
        public Expression<Func<Type, bool>> TypePredicate { get; }

        /// <summary>
        /// 获取拦截器类型列表
        /// </summary>
        /// <value>包含所有拦截器类型的类型列表 <see cref="ITypeList{IInterceptor}"/></value>
        public ITypeList<IInterceptor> Interceptors { get; }

        /// <summary>
        /// 初始化代理约定操作上下文的新实例
        /// </summary>
        /// <param name="services">依赖注入服务集合</param>
        /// <param name="types">要处理的类型集合</param>
        /// <param name="typePredicate">用于筛选类型的谓词表达式</param>
        /// <param name="interceptors">拦截器类型列表</param>
        public ProxyConventionalActionContext(IServiceCollection services, System.Collections.Generic.IEnumerable<Type> types, Expression<Func<Type, bool>> typePredicate, ITypeList<IInterceptor> interceptors)
        {
            Services = services;
            Types = types;
            TypePredicate = typePredicate;
            Interceptors = interceptors;
        }

    }
}
