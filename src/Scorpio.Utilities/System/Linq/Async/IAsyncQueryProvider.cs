using System.Linq.Expressions;
using System.Threading;

namespace System.Linq.Async
{
    /// <summary>
    ///     <para>
    ///         定义用于异步执行由 IQueryable 对象描述的查询的方法。
    ///     </para>
    /// </summary>
    public interface IAsyncQueryProvider : IQueryProvider
    {
        /// <summary>
        ///     异步执行由指定表达式树表示的强类型查询。
        /// </summary>
        /// <typeparam name="TResult">
        ///     查询返回的元素的类型。
        /// </typeparam>
        /// <param name="expression">
        ///     表示查询的表达式树。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     查询执行的结果。
        /// </returns>
        TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default);
    }

}
