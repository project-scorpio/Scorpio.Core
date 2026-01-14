using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Scorpio.DependencyInjection;

namespace Scorpio.Linq
{
    /// <summary>
    /// 默认异步查询执行器实现，提供基本的异步查询功能
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类实现了 <see cref="IAsyncQueryableExecuter"/> 接口，作为单例依赖项注册到容器中。
    /// 提供了将同步 LINQ 查询包装为异步操作的默认实现。
    /// </para>
    /// <para>
    /// 此实现主要用于内存查询或不支持真正异步操作的数据源。对于支持异步的数据源
    /// （如 Entity Framework Core），应使用专门的实现类以获得更好的性能。
    /// </para>
    /// </remarks>
    /// <seealso cref="IAsyncQueryableExecuter"/>
    /// <seealso cref="ISingletonDependency"/>
    internal class DefaultAsyncQueryableExecuter : IAsyncQueryableExecuter, ISingletonDependency
    {
        /// <summary>
        /// 获取 <see cref="DefaultAsyncQueryableExecuter"/> 类的单例实例
        /// </summary>
        /// <value>
        /// 返回全局唯一的默认异步查询执行器实例
        /// </value>
        /// <remarks>
        /// 该属性提供了线程安全的单例访问，确保在整个应用程序生命周期中
        /// 只存在一个 <see cref="DefaultAsyncQueryableExecuter"/> 实例。
        /// </remarks>
        public static DefaultAsyncQueryableExecuter Instance { get; } = new DefaultAsyncQueryableExecuter();

        /// <summary>
        /// 异步获取查询结果的元素数量
        /// </summary>
        /// <typeparam name="T">
        /// 查询元素的类型
        /// </typeparam>
        /// <param name="queryable">
        /// 要执行计数操作的查询对象。不能为 <c>null</c>
        /// </param>
        /// <param name="cancellationToken">
        /// 用于取消异步操作的令牌。在此实现中被忽略
        /// </param>
        /// <returns>
        /// 表示异步计数操作的任务，任务结果为查询结果中的元素数量
        /// </returns>
        /// <remarks>
        /// 该实现内部调用同步的 <see cref="Queryable.Count{TSource}(IQueryable{TSource})"/> 方法，
        /// 并使用 <see cref="Task.FromResult{TResult}(TResult)"/> 包装结果。
        /// 对于真正的异步数据源，建议使用专门的实现类。
        /// </remarks>
        public Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default) =>
            Task.FromResult(queryable.Count());


#pragma warning disable CS1574 // XML 注释中有无法解析的 cref 特性
        /// <summary>
        /// 异步将查询结果转换为列表
        /// </summary>
        /// <typeparam name="T">
        /// 查询元素的类型
        /// </typeparam>
        /// <param name="queryable">
        /// 要转换为列表的查询对象。不能为 <c>null</c>
        /// </param>
        /// <param name="cancellationToken">
        /// 用于取消异步操作的令牌。在此实现中被忽略
        /// </param>
        /// <returns>
        /// 表示异步转换操作的任务，任务结果为包含查询结果的 <see cref="List{T}"/>
        /// </returns>
        /// <remarks>
        /// 该实现内部调用同步的 <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> 方法，
        /// 并使用 <see cref="Task.FromResult{TResult}(TResult)"/> 包装结果。
        /// </remarks>
        public Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default) =>
            Task.FromResult(queryable.ToList());
#pragma warning restore CS1574 // XML 注释中有无法解析的 cref 特性

        /// <summary>
        /// 异步获取查询结果的第一个元素，如果没有元素则返回默认值
        /// </summary>
        /// <typeparam name="T">
        /// 查询元素的类型
        /// </typeparam>
        /// <param name="queryable">
        /// 要获取第一个元素的查询对象。不能为 <c>null</c>
        /// </param>
        /// <param name="cancellationToken">
        /// 用于取消异步操作的令牌。在此实现中被忽略
        /// </param>
        /// <returns>
        /// 表示异步获取操作的任务，任务结果为查询结果的第一个元素，如果序列为空则返回 <typeparamref name="T"/> 的默认值
        /// </returns>
        /// <remarks>
        /// 该实现内部调用同步的 <see cref="Queryable.FirstOrDefault{TSource}(IQueryable{TSource})"/> 方法，
        /// 并使用 <see cref="Task.FromResult{TResult}(TResult)"/> 包装结果。
        /// </remarks>
        public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default) =>
            Task.FromResult(queryable.FirstOrDefault());

        /// <summary>
        /// 将查询对象转换为异步可枚举序列
        /// </summary>
        /// <typeparam name="TSource">
        /// 序列元素的类型
        /// </typeparam>
        /// <param name="sources">
        /// 要转换的查询对象。不能为 <c>null</c>
        /// </param>
        /// <returns>
        /// 支持异步枚举的 <see cref="IAsyncEnumerable{T}"/> 序列
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// 当 <paramref name="sources"/> 不实现 <see cref="IAsyncEnumerable{T}"/> 接口时抛出
        /// </exception>
        /// <remarks>
        /// <para>
        /// 该实现首先检查查询对象是否已经实现了 <see cref="IAsyncEnumerable{TSource}"/> 接口，
        /// 如果是则直接返回；否则抛出 <see cref="InvalidOperationException"/> 异常。
        /// </para>
        /// <para>
        /// 对于不支持异步枚举的查询源，需要使用专门的实现类来提供转换逻辑。
        /// </para>
        /// </remarks>
        /// <seealso cref="IAsyncEnumerable{T}"/>
        public IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(IQueryable<TSource> sources)
        {
            // 检查查询对象是否已实现异步可枚举接口
            if (sources is IAsyncEnumerable<TSource> asyncEnumerable)
            {
                return asyncEnumerable;
            }

            // 如果不支持异步枚举，抛出异常
            throw new InvalidOperationException($"查询对象 {sources.GetType().Name} 不支持异步枚举操作。请使用支持异步操作的数据源或专门的执行器实现。");
        }
    }
}
