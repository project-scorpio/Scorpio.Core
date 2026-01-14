using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Scorpio;

namespace System.Linq.Async
{
    /// <summary>
    /// 为异步可枚举对象提供扩展方法的部分类
    /// </summary>
    /// <remarks>
    /// 此部分类专门提供了异步 ForEach 操作的各种重载，
    /// 支持同步和异步操作、索引访问以及取消令牌的传递。
    /// </remarks>
    /// <seealso cref="IAsyncEnumerable{T}"/>
    public partial class AsyncEnumerable
    {
        /// <summary>
        /// 对异步可枚举序列的每个元素执行指定的同步操作
        /// </summary>
        /// <typeparam name="T">异步可枚举序列中元素的类型</typeparam>
        /// <param name="enumerable">要对其元素执行操作的异步可枚举序列</param>
        /// <param name="action">要对每个元素执行的同步操作</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的 <see cref="ValueTask"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="enumerable"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此方法异步枚举序列中的每个元素，并对每个元素同步执行指定的操作。
        /// 操作本身是同步的，但枚举过程是异步的，支持通过取消令牌中断。
        /// 使用 <see cref="TaskAsyncEnumerableExtensions.WithCancellation{T}(IAsyncEnumerable{T}, CancellationToken)"/> 来传递取消令牌。
        /// </remarks>
        /// <seealso cref="Action{T}"/>
        /// <seealso cref="TaskAsyncEnumerableExtensions.WithCancellation{T}(IAsyncEnumerable{T}, CancellationToken)"/>
        public static async ValueTask ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Action<T> action, CancellationToken cancellationToken = default)
        {
            Check.NotNull(enumerable, nameof(enumerable));
            Check.NotNull(action, nameof(action));
            await Core(enumerable, action, cancellationToken);

            // <summary>
            // 核心实现方法，使用本地静态函数以优化性能
            // </summary>
            // <param name="enumerable">异步可枚举序列</param>
            // <param name="action">要执行的操作</param>
            // <param name="cancellationToken">取消令牌</param>
            // <returns>表示异步操作的 <see cref="ValueTask"/></returns>
            static async ValueTask Core(IAsyncEnumerable<T> enumerable, Action<T> action, CancellationToken cancellationToken)
            {
                await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// 对异步可枚举序列的每个元素执行指定的同步操作，并提供元素的索引
        /// </summary>
        /// <typeparam name="T">异步可枚举序列中元素的类型</typeparam>
        /// <param name="enumerable">要对其元素执行操作的异步可枚举序列</param>
        /// <param name="action">要对每个元素执行的同步操作，接收元素和索引作为参数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的 <see cref="ValueTask"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="enumerable"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <exception cref="OverflowException">当索引值超出 <see cref="int"/> 的范围时抛出</exception>
        /// <remarks>
        /// 此重载提供了元素的从零开始的索引，便于在操作中使用位置信息。
        /// 索引使用 checked 算术运算以防止溢出。
        /// </remarks>
        /// <seealso cref="Action{T1, T2}"/>
        public static async ValueTask ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Action<T, int> action, CancellationToken cancellationToken = default)
        {
            Check.NotNull(enumerable, nameof(enumerable));
            Check.NotNull(action, nameof(action));

            await Core(enumerable, action, cancellationToken);

            // <summary>
            // 核心实现方法，提供索引功能
            // </summary>
            // <param name="enumerable">异步可枚举序列</param>
            // <param name="action">要执行的操作，接收元素和索引</param>
            // <param name="cancellationToken">取消令牌</param>
            // <returns>表示异步操作的 <see cref="ValueTask"/></returns>
            static async ValueTask Core(IAsyncEnumerable<T> enumerable, Action<T, int> action, CancellationToken cancellationToken)
            {
                var index = 0;
                await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    action(item, checked(index++));
                }
            }
        }

        /// <summary>
        /// 对异步可枚举序列的每个元素执行指定的异步操作
        /// </summary>
        /// <typeparam name="T">异步可枚举序列中元素的类型</typeparam>
        /// <param name="enumerable">要对其元素执行操作的异步可枚举序列</param>
        /// <param name="action">要对每个元素执行的异步操作</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的 <see cref="ValueTask"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="enumerable"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此方法对每个元素执行异步操作，操作按顺序执行（非并行）。
        /// 每个操作都会被等待完成后才处理下一个元素。
        /// 使用 <see cref="Task.ConfigureAwait(bool)"/> 来避免死锁。
        /// </remarks>
        /// <seealso cref="Func{T, TResult}"/>
        /// <seealso cref="Task"/>
        public static async ValueTask ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, Task> action, CancellationToken cancellationToken = default)
        {
            Check.NotNull(enumerable, nameof(enumerable));
            Check.NotNull(action, nameof(action));

            await Core(enumerable, action, cancellationToken);

            // <summary>
            // 核心实现方法，支持异步操作
            // </summary>
            // <param name="enumerable">异步可枚举序列</param>
            // <param name="action">要执行的异步操作</param>
            // <param name="cancellationToken">取消令牌</param>
            // <returns>表示异步操作的 <see cref="ValueTask"/></returns>
            static async ValueTask Core(IAsyncEnumerable<T> enumerable, Func<T, Task> action, CancellationToken cancellationToken)
            {
                await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await action(item).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// 对异步可枚举序列的每个元素执行指定的异步操作，并传递取消令牌
        /// </summary>
        /// <typeparam name="T">异步可枚举序列中元素的类型</typeparam>
        /// <param name="enumerable">要对其元素执行操作的异步可枚举序列</param>
        /// <param name="action">要对每个元素执行的异步操作，接收元素和取消令牌作为参数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的 <see cref="ValueTask"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="enumerable"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此重载将取消令牌传递给每个异步操作，允许操作内部也能响应取消请求。
        /// 这提供了更细粒度的取消控制。
        /// </remarks>
        /// <seealso cref="Func{T1, T2, TResult}"/>
        /// <seealso cref="CancellationToken"/>
        public static ValueTask ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            Check.NotNull(enumerable, nameof(enumerable));
            Check.NotNull(action, nameof(action));

            return Core(enumerable, action, cancellationToken);

            // <summary>
            // 核心实现方法，支持取消令牌传递
            // </summary>
            // <param name="enumerable">异步可枚举序列</param>
            // <param name="action">要执行的异步操作，接收取消令牌</param>
            // <param name="cancellationToken">取消令牌</param>
            // <returns>表示异步操作的 <see cref="ValueTask"/></returns>
            static async ValueTask Core(IAsyncEnumerable<T> enumerable, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken)
            {
                await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await action(item, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// 对异步可枚举序列的每个元素执行指定的异步操作，并提供元素的索引和取消令牌
        /// </summary>
        /// <typeparam name="T">异步可枚举序列中元素的类型</typeparam>
        /// <param name="enumerable">要对其元素执行操作的异步可枚举序列</param>
        /// <param name="action">要对每个元素执行的异步操作，接收元素、索引和取消令牌作为参数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的 <see cref="ValueTask"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="enumerable"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <exception cref="OverflowException">当索引值超出 <see cref="int"/> 的范围时抛出</exception>
        /// <remarks>
        /// 此重载结合了索引访问和取消令牌传递的功能，提供了最完整的控制能力。
        /// 适用于需要位置信息和取消支持的复杂异步操作场景。
        /// 索引使用 checked 算术运算以防止溢出。
        /// </remarks>
        /// <seealso cref="Func{T1, T2, T3, TResult}"/>
        public static ValueTask ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, int, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            Check.NotNull(enumerable, nameof(enumerable));
            Check.NotNull(action, nameof(action));

            return Core(enumerable, action, cancellationToken);

            // <summary>
            // 核心实现方法，提供索引和取消令牌功能
            // </summary>
            // <param name="enumerable">异步可枚举序列</param>
            // <param name="action">要执行的异步操作，接收元素、索引和取消令牌</param>
            // <param name="cancellationToken">取消令牌</param>
            // <returns>表示异步操作的 <see cref="ValueTask"/></returns>
            static async ValueTask Core(IAsyncEnumerable<T> enumerable, Func<T, int, CancellationToken, Task> action, CancellationToken cancellationToken)
            {
                var index = 0;
                await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await action(item, checked(index++), cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
