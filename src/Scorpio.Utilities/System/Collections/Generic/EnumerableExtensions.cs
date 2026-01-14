using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Scorpio;

namespace System.Collections.Generic
{
    /// <summary> 
    /// 为 <see cref="IEnumerable{T}"/> 提供扩展方法的静态工具类
    /// </summary>
    /// <remarks>
    /// 此类包含一系列针对 <see cref="IEnumerable{T}"/> 的扩展方法，
    /// 提供了条件过滤、字符串连接、循环操作、异步查询等常用的枚举操作功能。
    /// </remarks>
    /// <seealso cref="IEnumerable{T}"/>
    /// <seealso cref="Enumerable"/>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 检查指定的集合对象是否为 null 或不包含任何项
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">要检查的集合</param>
        /// <returns>如果集合为 null 或为空则返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了空安全的集合检查，避免了在检查集合状态时抛出空引用异常。
        /// 当集合为 null 或不包含任何元素时返回 <c>true</c>。
        /// </remarks>
        /// <seealso cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

        /// <summary>
        /// 使用指定的分隔符连接构造的字符串集合的成员
        /// </summary>
        /// <param name="source">包含要连接的字符串的集合</param>
        /// <param name="separator">用作分隔符的字符串。仅当集合有多个元素时，分隔符才包含在返回的字符串中</param>
        /// <returns>由分隔符字符串分隔的集合成员组成的字符串。如果集合没有成员，则该方法返回 <see cref="string.Empty"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 这是 <see cref="string.Join(string, IEnumerable{string})"/> 方法的快捷方式。
        /// 专门用于字符串集合的连接操作。
        /// </remarks>
        /// <seealso cref="string.Join(string, IEnumerable{string})"/>
        /// <seealso cref="ExpandToString{T}(IEnumerable{T}, string)"/>
        public static string ExpandToString(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        /// <summary>
        /// 使用指定的分隔符连接集合的成员
        /// </summary>
        /// <typeparam name="T">集合成员的类型</typeparam>
        /// <param name="source">包含要连接的对象的集合</param>
        /// <param name="separator">用作分隔符的字符串。仅当集合有多个元素时，分隔符才包含在返回的字符串中</param>
        /// <returns>由分隔符字符串分隔的集合成员组成的字符串。如果集合没有成员，则该方法返回 <see cref="string.Empty"/></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 这是 <see cref="string.Join{T}(string, IEnumerable{T})"/> 方法的快捷方式。
        /// 可以用于任何类型的集合，每个元素会调用其 <see cref="object.ToString"/> 方法。
        /// </remarks>
        /// <seealso cref="string.Join{T}(string, IEnumerable{T})"/>
        /// <seealso cref="ExpandToString(IEnumerable{string}, string)"/>
        public static string ExpandToString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }

        /// <summary>
        /// 如果给定条件为 true，则使用指定的谓词过滤 <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">要应用过滤的可枚举对象</param>
        /// <param name="condition">一个布尔值，决定是否应用过滤</param>
        /// <param name="predicate">用于过滤可枚举对象的谓词函数</param>
        /// <returns>根据 <paramref name="condition"/> 返回过滤或未过滤的可枚举对象</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法提供了条件过滤的便捷方式。只有当条件为 <c>true</c> 时才会应用过滤，
        /// 否则返回原始集合。这避免了在代码中使用条件语句来决定是否过滤。
        /// </remarks>
        /// <seealso cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }

        /// <summary>
        /// 如果给定条件为 true，则使用指定的谓词过滤 <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">要应用过滤的可枚举对象</param>
        /// <param name="condition">一个布尔值，决定是否应用过滤</param>
        /// <param name="predicate">用于过滤可枚举对象的谓词函数，接收元素和索引作为参数</param>
        /// <returns>根据 <paramref name="condition"/> 返回过滤或未过滤的可枚举对象</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许谓词函数访问元素的索引位置，适用于需要基于位置进行过滤的场景。
        /// </remarks>
        /// <seealso cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }

        /// <summary>
        /// 对 <see cref="IEnumerable{T}"/> 的每个元素执行指定的操作
        /// </summary>
        /// <typeparam name="T">可枚举对象中元素的类型</typeparam>
        /// <param name="source">要应用操作的 <see cref="IEnumerable{T}"/></param>
        /// <param name="action">要对 <see cref="IEnumerable{T}"/> 的每个元素执行的 <see cref="Action{T}"/> 委托</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法类似于 <see cref="List{T}.ForEach"/> 方法，但适用于任何 <see cref="IEnumerable{T}"/>。
        /// 如果源集合为空或操作为 null，方法会安全地返回而不执行任何操作。
        /// </remarks>
        /// <seealso cref="List{T}.ForEach"/>
        /// <seealso cref="Action{T}"/>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {

            if (source.IsNullOrEmpty() || action == null)
            {
                return;
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// 对 <see cref="IEnumerable{T}"/> 的每个元素异步执行指定的操作
        /// </summary>
        /// <typeparam name="T">可枚举对象中元素的类型</typeparam>
        /// <param name="source">要应用操作的 <see cref="IEnumerable{T}"/></param>
        /// <param name="action">要对 <see cref="IEnumerable{T}"/> 的每个元素异步执行的操作</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法按顺序对每个元素执行异步操作，支持通过取消令牌中断操作。
        /// 当检测到取消请求时，会立即停止处理后续元素。
        /// </remarks>
        /// <seealso cref="CancellationToken"/>
        /// <seealso cref="Task"/>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, CancellationToken cancellationToken = default)
        {
            if (source.IsNullOrEmpty() || action == null)
            {
                return;
            }

            foreach (var item in source)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                await action(item);
            }
        }

        /// <summary>
        /// 对 <see cref="IEnumerable{T}"/> 的每个元素异步执行指定的操作
        /// </summary>
        /// <typeparam name="T">可枚举对象中元素的类型</typeparam>
        /// <param name="source">要应用操作的 <see cref="IEnumerable{T}"/></param>
        /// <param name="action">要对 <see cref="IEnumerable{T}"/> 的每个元素异步执行的操作，接收元素和取消令牌作为参数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="action"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许操作函数接收取消令牌，以便在操作内部也能响应取消请求。
        /// </remarks>
        /// <seealso cref="CancellationToken"/>
        /// <seealso cref="Task"/>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            if (source.IsNullOrEmpty() || action == null)
            {
                return;
            }

            foreach (var item in source)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                await action(item, cancellationToken);
            }
        }

        /// <summary>
        /// 异步确定序列中是否有任何元素满足指定的条件
        /// </summary>
        /// <typeparam name="TSource">源序列中元素的类型</typeparam>
        /// <param name="source">要检查元素的 <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">用于测试每个元素的异步函数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>如果源序列中的任何元素通过指定谓词中的测试，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此方法是 <see cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> 的异步版本。
        /// 一旦找到满足条件的元素，就会立即返回 <c>true</c>，不会继续检查剩余元素。
        /// </remarks>
        /// <seealso cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// <seealso cref="AllAsync{TSource}(IEnumerable{TSource}, Func{TSource, Task{bool}}, CancellationToken)"/>
        public static async Task<bool> AnyAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await predicate(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 异步确定序列中是否有任何元素满足指定的条件
        /// </summary>
        /// <typeparam name="TSource">源序列中元素的类型</typeparam>
        /// <param name="source">要检查元素的 <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">用于测试每个元素的异步函数，接收元素和取消令牌作为参数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>如果源序列中的任何元素通过指定谓词中的测试，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此重载允许谓词函数接收取消令牌，以便在谓词内部也能响应取消请求。
        /// </remarks>
        /// <seealso cref="AnyAsync{TSource}(IEnumerable{TSource}, Func{TSource, Task{bool}}, CancellationToken)"/>
        public static async Task<bool> AnyAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, CancellationToken, Task<bool>> predicate, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await predicate(item, cancellationToken))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 异步确定序列中的所有元素是否都满足指定的条件
        /// </summary>
        /// <typeparam name="TSource">源序列中元素的类型</typeparam>
        /// <param name="source">要检查元素的 <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">用于测试每个元素的异步函数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>如果源序列中的每个元素都通过指定谓词中的测试，或者如果序列为空，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此方法是 <see cref="Enumerable.All{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> 的异步版本。
        /// 一旦找到不满足条件的元素，就会立即返回 <c>false</c>，不会继续检查剩余元素。
        /// 对于空序列，此方法返回 <c>true</c>。
        /// </remarks>
        /// <seealso cref="Enumerable.All{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// <seealso cref="AnyAsync{TSource}(IEnumerable{TSource}, Func{TSource, Task{bool}}, CancellationToken)"/>
        public static async Task<bool> AllAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!(await predicate(item)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 异步确定序列中的所有元素是否都满足指定的条件
        /// </summary>
        /// <typeparam name="TSource">源序列中元素的类型</typeparam>
        /// <param name="source">要检查元素的 <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">用于测试每个元素的异步函数，接收元素和取消令牌作为参数</param>
        /// <param name="cancellationToken">用于取消操作的取消令牌</param>
        /// <returns>如果源序列中的每个元素都通过指定谓词中的测试，或者如果序列为空，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
        /// <remarks>
        /// 此重载允许谓词函数接收取消令牌，以便在谓词内部也能响应取消请求。
        /// </remarks>
        /// <seealso cref="AllAsync{TSource}(IEnumerable{TSource}, Func{TSource, Task{bool}}, CancellationToken)"/>
        public static async Task<bool> AllAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, CancellationToken, Task<bool>> predicate, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!(await predicate(item, cancellationToken)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
