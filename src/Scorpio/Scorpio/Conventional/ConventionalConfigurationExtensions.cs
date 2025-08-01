using System;
using System.Collections.Generic;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 为约定配置提供扩展方法的静态类
    /// </summary>
    public static class ConventionalConfigurationExtensions
    {
        /// <summary>
        /// 根据指定的谓词筛选类型，创建约定上下文
        /// </summary>
        /// <typeparam name="TAction">操作类型参数</typeparam>
        /// <param name="configuration">约定配置实例</param>
        /// <param name="predicate">用于筛选类型的谓词函数</param>
        /// <returns>返回筛选后的约定上下文 <see cref="IConventionalContext{TAction}"/></returns>
        public static IConventionalContext<TAction> Where<TAction>(this IConventionalConfiguration<TAction> configuration, Predicate<Type> predicate) => configuration.CreateContext().Where(predicate);

        /// <summary>
        /// 为指定的约定配置创建新的约定上下文
        /// </summary>
        /// <typeparam name="TAction">操作类型参数</typeparam>
        /// <param name="configuration">约定配置实例</param>
        /// <returns>返回新创建的约定上下文 <see cref="IConventionalContext{TAction}"/></returns>
        public static IConventionalContext<TAction> CreateContext<TAction>(this IConventionalConfiguration<TAction> configuration)
        {
            // 将配置转换为具体实现类型并获取内部上下文
            var context = (configuration as ConventionalConfiguration<TAction>).GetInternalContext();
            return context;
        }

        /// <summary>
        /// 获取约定配置中的所有上下文集合（内部方法）
        /// </summary>
        /// <param name="configuration">约定配置实例</param>
        /// <returns>返回所有约定上下文的可枚举集合 <see cref="IEnumerable{IConventionalContext}"/></returns>
        internal static IEnumerable<IConventionalContext> GetContexts(this IConventionalConfiguration configuration) => (configuration as ConventionalConfiguration).Contexts;
    }
}
