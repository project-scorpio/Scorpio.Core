using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scorpio.Linq
{
    /// <summary>
    /// 异步查询执行器接口，定义异步查询操作的核心契约
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该接口专为 Scorpio 框架设计，提供了对 <see cref="IQueryable{T}"/> 的异步执行支持。
    /// 实现此接口的类可以将同步的 LINQ 查询转换为异步操作，提高应用程序的响应性和性能。
    /// </para>
    /// <para>
    /// 主要用于数据访问层，特别是与 Entity Framework Core 等 ORM 框架集成时，
    /// 提供统一的异步查询抽象。
    /// </para>
    /// </remarks>
    /// <seealso cref="IQueryable{T}"/>
    /// <seealso cref="DefaultAsyncQueryableExecuter"/>
    public interface IAsyncQueryableExecuter
    {
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
        /// 用于取消异步操作的令牌。默认为 <see cref="CancellationToken.None"/>
        /// </param>
        /// <returns>
        /// 表示异步计数操作的任务，任务结果为查询结果中的元素数量
        /// </returns>
        /// <remarks>
        /// 该方法等效于同步版本的 <see cref="Queryable.Count{TSource}(IQueryable{TSource})"/> 方法，
        /// 但提供了异步执行能力，避免阻塞调用线程。
        /// </remarks>
        /// <seealso cref="Queryable.Count{TSource}(IQueryable{TSource})"/>
        Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

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
        /// 用于取消异步操作的令牌。默认为 <see cref="CancellationToken.None"/>
        /// </param>
        /// <returns>
        /// 表示异步转换操作的任务，任务结果为包含查询结果的 <see cref="List{T}"/>
        /// </returns>
        /// <remarks>
        /// 该方法等效于同步版本的 <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> 方法，
        /// 但提供了异步执行能力，特别适用于数据库查询等 I/O 密集型操作。
        /// </remarks>
        /// <seealso cref="List{T}"/>
        /// <seealso cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/>
        Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

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
        /// 用于取消异步操作的令牌。默认为 <see cref="CancellationToken.None"/>
        /// </param>
        /// <returns>
        /// 表示异步获取操作的任务，任务结果为查询结果的第一个元素，如果序列为空则返回 <typeparamref name="T"/> 的默认值
        /// </returns>
        /// <remarks>
        /// 该方法等效于同步版本的 <see cref="Queryable.FirstOrDefault{TSource}(IQueryable{TSource})"/> 方法，
        /// 但提供了异步执行能力。对于引用类型，默认值为 <c>null</c>；对于值类型，默认值为该类型的默认值。
        /// </remarks>
        /// <seealso cref="Queryable.FirstOrDefault{TSource}(IQueryable{TSource})"/>
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

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
        /// <remarks>
        /// <para>
        /// 该方法将 <see cref="IQueryable{T}"/> 转换为 <see cref="IAsyncEnumerable{T}"/>，
        /// 允许使用 <c>await foreach</c> 语句进行异步枚举。
        /// </para>
        /// <para>
        /// 如果查询对象本身已经实现了 <see cref="IAsyncEnumerable{T}"/>，则直接返回；
        /// 否则需要由具体实现提供转换逻辑。
        /// </para>
        /// </remarks>
        /// <seealso cref="IAsyncEnumerable{T}"/>
        /// <seealso cref="IQueryable{T}"/>
        IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(IQueryable<TSource> sources);
    }
}
