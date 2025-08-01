using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scorpio;

namespace System.Linq.Async

{
    static partial class AsyncQueryable
    {
        #region First/FirstOrDefault

        /// <summary>
        ///     异步返回序列的第一个元素。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回第一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含 <paramref name="source"/> 中的第一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<TSource> FirstAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.FirstWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回满足指定条件的序列的第一个元素。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回第一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含 <paramref name="source"/> 中通过 <paramref name="predicate"/> 中的测试的第一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         没有元素满足 <paramref name="predicate"/> 中的条件
        ///     </para>
        ///     <para>
        ///         -或-
        ///     </para>
        ///     <para>
        ///         <paramref name="source"/> 不包含任何元素。
        ///     </para>
        /// </exception>
        public static Task<TSource> FirstAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.FirstWithPredicate, source, predicate, cancellationToken);
        }

        /// <summary>
        ///     异步返回序列的第一个元素，如果序列不包含元素，则返回默认值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回第一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果 <paramref name="source"/> 为空，则任务结果包含 <see langword="default"/>(<typeparamref name="TSource"/>)；
        ///     否则，包含 <paramref name="source"/> 中的第一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.FirstOrDefaultWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回满足指定条件的序列的第一个元素，如果没有找到这样的元素，则返回默认值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回第一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果 <paramref name="source"/> 为空或者没有元素通过 <paramref name="predicate"/> 指定的测试，则任务结果包含
        ///     <see langword="default"/>(<typeparamref name="TSource"/>)；否则，包含 <paramref name="source"/> 中通过
        ///     <paramref name="predicate"/> 指定的测试的第一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.FirstOrDefaultWithPredicate, source, predicate, cancellationToken);
        }

        #endregion

        #region Last/LastOrDefault

        /// <summary>
        ///     异步返回序列的最后一个元素。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回最后一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含 <paramref name="source"/> 中的最后一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<TSource> LastAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.LastWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回满足指定条件的序列的最后一个元素。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回最后一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含 <paramref name="source"/> 中通过 <paramref name="predicate"/> 中的测试的最后一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         没有元素满足 <paramref name="predicate"/> 中的条件。
        ///     </para>
        ///     <para>
        ///         -或-
        ///     </para>
        ///     <para>
        ///         <paramref name="source"/> 不包含任何元素。
        ///     </para>
        /// </exception>
        public static Task<TSource> LastAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.LastWithPredicate, source, predicate, cancellationToken);
        }

        /// <summary>
        ///     异步返回序列的最后一个元素，如果序列不包含元素，则返回默认值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回最后一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果 <paramref name="source"/> 为空，则任务结果包含 <see langword="default"/>(<typeparamref name="TSource"/>)；
        ///     否则，包含 <paramref name="source"/> 中的最后一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TSource> LastOrDefaultAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.LastOrDefaultWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回满足指定条件的序列的最后一个元素，如果没有找到这样的元素，则返回默认值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回最后一个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试每个元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     如果 <paramref name="source"/> 为空或者没有元素通过 <paramref name="predicate"/> 指定的测试，则任务结果包含
        ///     <see langword="default"/>(<typeparamref name="TSource"/>)；否则，包含 <paramref name="source"/> 中通过
        ///     <paramref name="predicate"/> 指定的测试的最后一个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<TSource> LastOrDefaultAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.LastOrDefaultWithPredicate, source, predicate, cancellationToken);
        }

        #endregion

        #region Single/SingleOrDefault

        /// <summary>
        ///     异步返回序列中的唯一元素，如果序列不正好包含一个元素，则抛出异常。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回单个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含输入序列的单个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         <paramref name="source"/> 包含多个元素。
        ///     </para>
        ///     <para>
        ///         -或-
        ///     </para>
        ///     <para>
        ///         <paramref name="source"/> 不包含任何元素。
        ///     </para>
        /// </exception>
        public static Task<TSource> SingleAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.SingleWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回满足指定条件的序列中的唯一元素，如果有多个元素满足条件，则抛出异常。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回单个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含满足 <paramref name="predicate"/> 条件的输入序列的单个元素。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         没有元素满足 <paramref name="predicate"/> 中的条件。
        ///     </para>
        ///     <para>
        ///         -或-
        ///     </para>
        ///     <para>
        ///         多个元素满足 <paramref name="predicate"/> 中的条件。
        ///     </para>
        ///     <para>
        ///         -或-
        ///     </para>
        ///     <para>
        ///         <paramref name="source"/> 不包含任何元素。
        ///     </para>
        /// </exception>
        public static Task<TSource> SingleAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.SingleWithPredicate, source, predicate, cancellationToken);
        }

        /// <summary>
        ///     异步返回序列中的唯一元素，如果序列为空则返回默认值；如果序列中有多个元素，则抛出异常。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回单个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含输入序列的单个元素，如果序列不包含任何元素，则为 <see langword="default"/>(<typeparamref name="TSource"/>)。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="source"/> 包含多个元素。
        /// </exception>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(
             this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<TSource, Task<TSource>>(QueryableMethods.SingleOrDefaultWithoutPredicate, source, cancellationToken);
        }

        /// <summary>
        ///     异步返回满足指定条件的序列中的唯一元素，如果没有这样的元素则返回默认值；如果有多个元素满足条件，则抛出异常。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source">
        ///     要返回单个元素的 <see cref="IQueryable{T}"/>。
        /// </param>
        /// <param name="predicate"> 用于测试元素是否满足条件的函数。 </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含满足 <paramref name="predicate"/> 条件的输入序列的单个元素，如果没有找到这样的元素，
        ///     则为 <see langword="default"/>(<typeparamref name="TSource"/>)。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     多个元素满足 <paramref name="predicate"/> 中的条件。
        /// </exception>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return ExecuteAsync<TSource, Task<TSource>>(
                QueryableMethods.SingleOrDefaultWithPredicate, source, predicate, cancellationToken);
        }

        #endregion

    }
}