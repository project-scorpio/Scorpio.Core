using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scorpio;

namespace System.Linq.Async

{
    static partial class AsyncQueryable
    {
        #region Min

        /// <summary>
        ///     异步返回序列的最小值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要确定最小值的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列中的最小值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TSource> MinAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.MinWithoutSelector, source, cancellationToken);
        }

        /// <summary>
        ///     异步对序列中的每个元素调用投影函数并返回最小的结果值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     <paramref name="selector"/> 函数返回的值的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要确定最小值的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="selector"> 应用于每个元素的投影函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列中的最小值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TResult> MinAsync<TSource, TResult>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<TResult>>(QueryableMethods.MinWithSelector, source, selector, cancellationToken);
        }

        #endregion

        #region Max

        /// <summary>
        ///     异步返回序列的最大值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要确定最大值的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列中的最大值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TSource> MaxAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.MaxWithoutSelector, source, cancellationToken);
        }

        /// <summary>
        ///     异步对序列中的每个元素调用投影函数并返回最大的结果值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     <paramref name="selector"/> 函数返回的值的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要确定最大值的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="selector"> 应用于每个元素的投影函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列中的最大值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TResult> MaxAsync<TSource, TResult>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<TResult>>(QueryableMethods.MaxWithSelector, source, selector, cancellationToken);
        }

        #endregion

    }
}