using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scorpio;

namespace System.Linq.Async
{
    /// <summary>
    /// 提供异步查询操作的扩展方法集合。
    /// </summary>
    static partial class AsyncQueryable
    {
        #region Average

        /// <summary>
        ///     异步计算序列中值的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<decimal> AverageAsync(
             this IQueryable<decimal> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<decimal, Task<decimal>>(
                QueryableMethods.GetAverageWithoutSelector(typeof(decimal)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算可空类型序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<decimal?> AverageAsync(
             this IQueryable<decimal?> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<decimal?, Task<decimal?>>(
                QueryableMethods.GetAverageWithoutSelector(typeof(decimal?)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的值序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<decimal> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, decimal>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<decimal>>(
                QueryableMethods.GetAverageWithSelector(typeof(decimal)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的可空值序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<decimal?> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, decimal?>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<decimal?>>(
                QueryableMethods.GetAverageWithSelector(typeof(decimal?)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算整数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<double> AverageAsync(
             this IQueryable<int> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<int, Task<double>>(QueryableMethods.GetAverageWithoutSelector(typeof(int)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算可空整数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<double?> AverageAsync(
             this IQueryable<int?> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<int?, Task<double?>>(QueryableMethods.GetAverageWithoutSelector(typeof(int?)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的整数值序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<double> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, int>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<double>>(
                QueryableMethods.GetAverageWithSelector(typeof(int)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的可空整数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<double?> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, int?>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<double?>>(
                QueryableMethods.GetAverageWithSelector(typeof(int?)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算长整型序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<double> AverageAsync(
             this IQueryable<long> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<long, Task<double>>(QueryableMethods.GetAverageWithoutSelector(typeof(long)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算可空长整型序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<double?> AverageAsync(
             this IQueryable<long?> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<long?, Task<double?>>(QueryableMethods.GetAverageWithoutSelector(typeof(long?)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的长整型值序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<double> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, long>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<double>>(
                QueryableMethods.GetAverageWithSelector(typeof(long)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的可空长整型序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<double?> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, long?>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<double?>>(
                QueryableMethods.GetAverageWithSelector(typeof(long?)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算双精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<double> AverageAsync(
             this IQueryable<double> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<double, Task<double>>(
                QueryableMethods.GetAverageWithoutSelector(typeof(double)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算可空双精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<double?> AverageAsync(
             this IQueryable<double?> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<double?, Task<double?>>(
                QueryableMethods.GetAverageWithoutSelector(typeof(double?)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的双精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<double> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, double>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<double>>(
                QueryableMethods.GetAverageWithSelector(typeof(double)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的可空双精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<double?> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, double?>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<double?>>(
                QueryableMethods.GetAverageWithSelector(typeof(double?)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算单精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<float> AverageAsync(
             this IQueryable<float> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<float, Task<float>>(QueryableMethods.GetAverageWithoutSelector(typeof(float)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算可空单精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <param name="source">
        ///     要计算平均值的值序列。
        /// </param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含序列值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<float?> AverageAsync(
             this IQueryable<float?> source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<float?, Task<float?>>(
                QueryableMethods.GetAverageWithoutSelector(typeof(float?)), source, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的单精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当 <paramref name="source"/> 不包含任何元素时抛出。
        /// </exception>
        public static Task<float> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, float>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<float>>(
                QueryableMethods.GetAverageWithSelector(typeof(float)), source, selector, cancellationToken);
        }

        /// <summary>
        ///     异步计算通过对输入序列的每个元素调用投影函数获得的可空单精度浮点数序列的平均值。
        /// </summary>
        /// <remarks>
        ///     同一上下文实例上不支持多个活动操作。请使用 'await' 确保在调用此上下文的另一个方法之前已完成任何异步操作。
        /// </remarks>
        /// <typeparam name="TSource">
        ///     <paramref name="source"/> 中元素的类型。
        /// </typeparam>
        /// <param name="source"> <typeparamref name="TSource"/> 类型的值序列。</param>
        /// <param name="selector"> 应用于每个元素的投影函数。</param>
        /// <param name="cancellationToken">
        ///     用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>
        ///     表示异步操作的任务。
        ///     任务结果包含投影值的平均值。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="source"/> 或 <paramref name="selector"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public static Task<float?> AverageAsync<TSource>(
             this IQueryable<TSource> source,
             Expression<Func<TSource, float?>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return ExecuteAsync<TSource, Task<float?>>(
                QueryableMethods.GetAverageWithSelector(typeof(float?)), source, selector, cancellationToken);
        }

        #endregion

    }
}