using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定上下文的基础接口，定义了约定上下文的核心功能
    /// </summary>
    public interface IConventionalContext
    {
        /// <summary>
        /// 获取依赖注入服务集合
        /// </summary>
        /// <value>服务集合实例 <see cref="IServiceCollection"/></value>
        IServiceCollection Services { get; }

        /// <summary>
        /// 获取当前上下文中筛选后的类型集合
        /// </summary>
        /// <value>类型枚举 <see cref="IEnumerable{Type}"/></value>
        IEnumerable<Type> Types { get; }

        /// <summary>
        /// 获取用于筛选类型的谓词表达式
        /// </summary>
        /// <value>类型筛选谓词表达式 <see cref="Expression{Func}"/></value>
        Expression<Func<Type, bool>> TypePredicate { get; }
    }

    /// <summary>
    /// 泛型约定上下文接口，继承自 <see cref="IConventionalContext"/>
    /// </summary>
    /// <typeparam name="TAction">约定操作类型参数，用于指定特定的约定操作</typeparam>
#pragma warning disable S2326
    public interface IConventionalContext<out TAction> : IConventionalContext
    {

    }
#pragma warning restore S2326 
}
