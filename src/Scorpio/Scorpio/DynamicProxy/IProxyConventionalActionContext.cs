using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 代理约定操作上下文接口，定义了动态代理约定操作所需的上下文信息
    /// </summary>
    /// <remarks>
    /// 此接口提供了执行代理约定操作时所需的所有必要信息，包括拦截器列表、服务集合、类型集合和类型筛选谓词
    /// </remarks>
    public interface IProxyConventionalActionContext
    {
        /// <summary>
        /// 获取拦截器类型列表
        /// </summary>
        /// <value>包含所有拦截器类型的类型列表 <see cref="ITypeList{IInterceptor}"/></value>
        ITypeList<IInterceptor> Interceptors { get; }

        /// <summary>
        /// 获取依赖注入服务集合
        /// </summary>
        /// <value>服务集合实例 <see cref="IServiceCollection"/></value>
        IServiceCollection Services { get; }

        /// <summary>
        /// 获取用于筛选类型的谓词表达式
        /// </summary>
        /// <value>类型筛选谓词表达式 <see cref="Expression{Func}"/></value>
        Expression<Func<Type, bool>> TypePredicate { get; }

        /// <summary>
        /// 获取当前上下文中的类型集合
        /// </summary>
        /// <value>类型枚举 <see cref="IEnumerable{Type}"/></value>
        IEnumerable<Type> Types { get; }
    }
}