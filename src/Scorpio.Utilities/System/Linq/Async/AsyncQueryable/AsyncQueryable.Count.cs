using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scorpio;

namespace System.Linq.Async

{
    static partial class AsyncQueryable
    {
        #region Any/All

        /// <summary>
        ///     异步确定序列是否包含任何元素。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要检查是否为空的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果源序列包含任何元素，则任务结果包含 <see langword="true"/>；否则为 <see langword="false"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<bool> AnyAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<bool>>(QueryableMethods.AnyWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步确定序列的任何元素是否满足条件。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要对其元素进行条件测试的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果源序列中有任何元素通过指定谓词中的测试，则任务结果包含 <see langword="true"/>；否则为 <see langword="false"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<bool> AnyAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<bool>>(QueryableMethods.AnyWithPredicate, source, predicate, cancellationToken);
        }

        /// <summary>
        ///     异步确定序列的所有元素是否都满足条件。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要对其元素进行条件测试的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果源序列的每个元素都通过指定谓词中的测试，则任务结果包含 <see langword="true"/>；否则为 <see langword="false"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<bool> AllAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<bool>>(QueryableMethods.All, source, predicate, cancellationToken);
        }

        #endregion

        #region Count/LongCount

        /// <summary>
        ///     异步返回序列中的元素数量。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     包含要计数的元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含输入序列中的元素数量。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<int> CountAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<int>>(QueryableMethods.CountWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回序列中满足条件的元素数量。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     包含要计数的元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列中满足谓词函数中条件的元素数量。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<int> CountAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<int>>(QueryableMethods.CountWithPredicate, source, predicate, cancellationToken);
        }

        /// <summary>
        ///     异步返回一个 <see cref="long"/> 类型的值，表示序列中元素的总数。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     包含要计数的元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含输入序列中的元素数量。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<long> LongCountAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<long>>(QueryableMethods.LongCountWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回一个 <see cref="long"/> 类型的值，表示序列中满足条件的元素数量。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     包含要计数的元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列中满足谓词函数中条件的元素数量。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<long> LongCountAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<long>>(QueryableMethods.LongCountWithPredicate, source, predicate, cancellationToken);
        }

        #endregion

        #region Contains

        /// <summary>
        ///     异步确定序列是否包含指定的元素，使用默认的相等性比较器。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回其中单个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="item"> 要在序列中定位的对象。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果输入序列包含指定的值，则任务结果包含 <see langword="true"/>；否则为 <see langword="false"/>。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<bool> ContainsAsync<TSource>(
             this IQueryable<TSource> source,
             TSource item,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<bool>>(
                QueryableMethods.Contains,
                source,
                Expression.Constant(item, typeof(TSource)),
                cancellationToken);
        }

        #endregion
    }
}