using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定上下文的内部实现类，提供类型筛选和项目存储功能
    /// </summary>
    internal class ConventionalContext : IConventionalContext
    {
        /// <summary>
        /// 用于存储键值对项目的字典
        /// </summary>
        private readonly Dictionary<string, object> _items = new Dictionary<string, object>();
        
        /// <summary>
        /// 所有可用的类型集合
        /// </summary>
        private readonly IEnumerable<Type> _types;

        /// <summary>
        /// 获取服务集合实例
        /// </summary>
        /// <value>依赖注入服务集合 <see cref="IServiceCollection"/></value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取或设置类型筛选谓词表达式
        /// </summary>
        /// <value>用于筛选类型的表达式 <see cref="Expression{Func}"/></value>
        public Expression<Func<Type, bool>> TypePredicate { get; private set; }

        /// <summary>
        /// 获取经过谓词筛选后的类型集合
        /// </summary>
        /// <value>筛选后的类型枚举 <see cref="IEnumerable{Type}"/></value>
        public IEnumerable<Type> Types => _types.Where(TypePredicate.Compile());

        /// <summary>
        /// 初始化约定上下文的新实例
        /// </summary>
        /// <param name="services">依赖注入服务集合</param>
        /// <param name="types">可用的类型集合</param>
        public ConventionalContext(IServiceCollection services, IEnumerable<Type> types)
        {
            Services = services;
            _types = types;
        }

        /// <summary>
        /// 添加类型筛选谓词表达式，与现有表达式进行AND逻辑组合
        /// </summary>
        /// <param name="expression">要添加的谓词表达式</param>
        public void AddPredicateExpression(Expression<Func<Type, bool>> expression) => TypePredicate = TypePredicate == null ? expression : TypePredicate.AndAlso(expression);

        /// <summary>
        /// 添加类型筛选谓词，转换为表达式后进行组合
        /// </summary>
        /// <param name="predicate">类型筛选谓词函数</param>
        public void AddPredicate(Predicate<Type> predicate) => AddPredicateExpression(t => predicate(t));

        /// <summary>
        /// 根据键获取指定类型的项目
        /// </summary>
        /// <typeparam name="T">项目的类型</typeparam>
        /// <param name="key">项目的键</param>
        /// <returns>如果找到则返回对应类型的项目，否则返回默认值</returns>
        public T GetItem<T>(string key)
        {
            // 尝试从字典中获取指定键的值
            if (!_items.TryGetValue(key, out var value))
            {
                return default;
            }
            // 将值转换为指定类型并返回
            return (T)value;
        }

        /// <summary>
        /// 设置指定键的项目值
        /// </summary>
        /// <typeparam name="T">项目的类型</typeparam>
        /// <param name="key">项目的键</param>
        /// <param name="item">要存储的项目值</param>
        public void SetItem<T>(string key, T item) => _items[key] = item;
    }

    /// <summary>
    /// 泛型约定上下文的内部实现类，继承自 <see cref="ConventionalContext"/>
    /// </summary>
    /// <typeparam name="TAction">操作类型参数</typeparam>
    internal class ConventionalContext<TAction> : ConventionalContext, IConventionalContext<TAction>
    {
        /// <summary>
        /// 初始化泛型约定上下文的新实例
        /// </summary>
        /// <param name="services">依赖注入服务集合</param>
        /// <param name="types">可用的类型集合</param>
        public ConventionalContext(IServiceCollection services, IEnumerable<Type> types) : base(services, types)
        {
        }
    }
}
