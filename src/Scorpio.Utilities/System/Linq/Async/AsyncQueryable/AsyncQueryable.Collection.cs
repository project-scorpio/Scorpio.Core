using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scorpio;

namespace System.Linq.Async

{
    static partial class AsyncQueryable
    {
        #region ToList/Array

        /// <summary>
        ///     通过异步枚举 <see cref="IQueryable{T}"/> 创建 <see cref="List{T}"/>。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     用于创建列表的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含一个包含输入序列中元素的 <see cref="List{T}"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static async Task<List<TSource>> ToListAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            var list = new List<TSource>();
            await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                list.Add(element);
            }

            return list;
        }

        /// <summary>
        ///     通过异步枚举 <see cref="IQueryable{T}"/> 创建数组。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     用于创建数组的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含一个包含输入序列中元素的数组。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static async Task<TSource[]> ToArrayAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default) => (await source.ToListAsync(cancellationToken).ConfigureAwait(false)).ToArray();

        #endregion

        #region ToDictionary

        /// <summary>
        ///     通过根据指定的键选择器函数异步枚举 <see cref="IQueryable{T}"/> 创建 <see cref="Dictionary{TKey, TValue}"/>。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     <paramref name="keySelector"/> 返回的键的类型。
        /// </typeparam>
        /// <param name="source">
        ///     用于创建 <see cref="Dictionary{TKey, TValue}"/> 的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="keySelector"> 从每个元素提取键的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含一个包含选定键和值的 <see cref="Dictionary{TKey, TSource}"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="keySelector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
             this IQueryable<TSource> source,
             Func<TSource, TKey> keySelector,
            CancellationToken cancellationToken = default) => ToDictionaryAsync(source, keySelector, e => e, comparer: null, cancellationToken);

        /// <summary>
        ///     通过根据指定的键选择器函数和比较器异步枚举 <see cref="IQueryable{T}"/> 创建 <see cref="Dictionary{TKey, TValue}"/>。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     <paramref name="keySelector"/> 返回的键的类型。
        /// </typeparam>
        /// <param name="source">
        ///     用于创建 <see cref="Dictionary{TKey, TValue}"/> 的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="keySelector"> 从每个元素提取键的函数。 </param>
        /// <param name="comparer">
        ///     用于比较键的 <see cref="IEqualityComparer{TKey}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含一个包含选定键和值的 <see cref="Dictionary{TKey, TSource}"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="keySelector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
             this IQueryable<TSource> source,
             Func<TSource, TKey> keySelector,
             IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default) => ToDictionaryAsync(source, keySelector, e => e, comparer, cancellationToken);

        /// <summary>
        ///     通过根据指定的键选择器和元素选择器函数异步枚举 <see cref="IQueryable{T}"/> 创建 <see cref="Dictionary{TKey, TValue}"/>。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     <paramref name="keySelector"/> 返回的键的类型。
        /// </typeparam>
        /// <typeparam name="TElement">
        ///     <paramref name="elementSelector"/> 返回的值的类型。
        /// </typeparam>
        /// <param name="source">
        ///     用于创建 <see cref="Dictionary{TKey, TValue}"/> 的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="keySelector"> 从每个元素提取键的函数。 </param>
        /// <param name="elementSelector"> 从每个元素生成结果元素值的转换函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含一个包含从输入序列中选择的 <typeparamref name="TElement"/> 类型值的 <see cref="Dictionary{TKey, TElement}"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="keySelector"/> 或 <paramref name="elementSelector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
             this IQueryable<TSource> source,
             Func<TSource, TKey> keySelector,
             Func<TSource, TElement> elementSelector,
            CancellationToken cancellationToken = default) => ToDictionaryAsync(source, keySelector, elementSelector, comparer: null, cancellationToken);

        /// <summary>
        ///     通过根据指定的键选择器函数、比较器和元素选择器函数异步枚举 <see cref="IQueryable{T}"/> 创建 <see cref="Dictionary{TKey, TValue}"/>。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     <paramref name="keySelector"/> 返回的键的类型。
        /// </typeparam>
        /// <typeparam name="TElement">
        ///     <paramref name="elementSelector"/> 返回的值的类型。
        /// </typeparam>
        /// <param name="source">
        ///     用于创建 <see cref="Dictionary{TKey, TValue}"/> 的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="keySelector"> 从每个元素提取键的函数。 </param>
        /// <param name="elementSelector"> 从每个元素生成结果元素值的转换函数。 </param>
        /// <param name="comparer">
        ///     用于比较键的 <see cref="IEqualityComparer{TKey}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含一个包含从输入序列中选择的 <typeparamref name="TElement"/> 类型值的 <see cref="Dictionary{TKey, TElement}"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="keySelector"/> 或 <paramref name="elementSelector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
             this IQueryable<TSource> source,
             Func<TSource, TKey> keySelector,
             Func<TSource, TElement> elementSelector,
             IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(elementSelector, nameof(elementSelector));

            var d = new Dictionary<TKey, TElement>(comparer);
            await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                d.Add(keySelector(element), elementSelector(element));
            }

            return d;
        }

        #endregion

        #region ForEach

        /// <summary>
        ///     异步枚举查询结果并对每个元素执行指定的操作。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="T">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要枚举的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="action"> 对每个元素执行的操作。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns> 表示异步操作的任务。 </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="action"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static async Task ForEachAsync<T>(
             this IQueryable<T> source,
             Action<T> action,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(action, nameof(action));

            await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                action(element);
            }
        }

        #endregion

        #region AsAsyncEnumerable

        /// <summary>
        ///     返回可以异步枚举的 <see cref="IAsyncEnumerable{T}"/>。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要枚举的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <returns> 查询结果。 </returns>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 不是 <see cref="IAsyncEnumerable{T}"/> 时抛出。
        /// </exception>
        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
             this IQueryable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            if (source is IAsyncEnumerable<TSource> asyncEnumerable)
            {
                return asyncEnumerable;
            }

            throw new InvalidOperationException("IQueryableNotAsync(typeof(TSource))");
        }

        #endregion

    }
}