using System.Linq;

using Scorpio;

namespace System.Collections.Generic
{
    /// <summary>
    /// 为集合类型提供扩展方法的静态工具类
    /// </summary>
    /// <remarks>
    /// 此类包含一系列针对 <see cref="ICollection{T}"/> 及其派生类型的扩展方法，
    /// 提供了常用的集合操作功能，如条件添加、批量移除、获取或创建元素等。
    /// </remarks>
    /// <seealso cref="ICollection{T}"/>
    /// <seealso cref="IList{T}"/>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 检查指定的集合对象是否为 null 或不包含任何项
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">要检查的集合</param>
        /// <returns>如果集合为 null 或为空则返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了空安全的集合检查，避免了在检查集合状态时抛出空引用异常。
        /// 当集合为 null 或 <see cref="ICollection{T}.Count"/> 小于等于 0 时返回 <c>true</c>。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Count"/>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source) => source == null || source.Count <= 0;

        /// <summary>
        /// 如果项不在集合中，则将其添加到集合中
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">目标集合</param>
        /// <param name="item">要检查和添加的项</param>
        /// <returns>如果添加成功返回 <c>true</c>，如果项已存在返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法使用集合的默认相等比较器来检查项是否已存在。
        /// 只有当项不存在时才会添加到集合中，避免重复项的产生。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Contains"/>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            Check.NotNull(source, nameof(source));

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        /// <summary>
        /// 如果项不在集合中，则将其添加到集合中，使用指定的相等比较器
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">目标集合</param>
        /// <param name="item">要检查和添加的项</param>
        /// <param name="comparer">用于比较元素的相等比较器</param>
        /// <returns>如果添加成功返回 <c>true</c>，如果项已存在返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许使用自定义的相等比较器来确定项是否已存在。
        /// 这在需要特定比较逻辑的场景中非常有用，如忽略大小写的字符串比较。
        /// </remarks>
        /// <seealso cref="IEqualityComparer{T}"/>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item, IEqualityComparer<T> comparer)
        {
            Check.NotNull(source, nameof(source));

            if (source.Contains(item, comparer))
            {
                return false;
            }

            source.Add(item);
            return true;
        }


        /// <summary>
        /// 根据指定的条件，如果项不在集合中则将其添加到集合中
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">目标集合</param>
        /// <param name="predicate">用于判断项是否已在集合中的条件</param>
        /// <param name="itemFactory">返回要添加项的工厂函数</param>
        /// <returns>如果添加成功返回 <c>true</c>，如果满足条件的项已存在返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/>、<paramref name="predicate"/> 或 <paramref name="itemFactory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法提供了基于自定义条件的添加逻辑。
        /// 只有当集合中没有满足指定条件的项时，才会调用工厂函数创建新项并添加到集合中。
        /// 这种延迟创建的方式可以提高性能，避免不必要的对象创建。
        /// </remarks>
        /// <seealso cref="Func{T, TResult}"/>
        public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(itemFactory, nameof(itemFactory));
            
            if (source.Any(predicate))
            {
                return false;
            }

            source.Add(itemFactory());
            return true;
        }

        /// <summary>
        /// 从集合中移除所有满足指定条件的项
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="predicate">用于测试每个元素的条件</param>
        /// <returns>包含所有被移除项的集合</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="predicate"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会移除集合中所有满足指定条件的项，并返回被移除的项的集合。
        /// 对于 <see cref="IList{T}"/> 类型，使用反向遍历以提高性能并避免索引问题。
        /// 对于其他集合类型，先收集满足条件的项，然后逐一移除。
        /// </remarks>
        /// <seealso cref="IList{T}"/>
        /// <seealso cref="Func{T, TResult}"/>
        public static ICollection<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            if (source is IList<T> list)
            {
                var result = new List<T>();
                // 反向遍历以避免索引变化问题
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (predicate(list[i]))
                    {
                        result.Add(list[i]);
                        list.RemoveAt(i);
                    }
                }
                return result;
            }
            
            var items = source.Where(predicate).ToList();
            foreach (var item in items)
            {
                source.Remove(item);
            }

            return items;
        }

        /// <summary>
        /// 根据指定的选择器从集合中获取第一个匹配的项，如果未找到则返回默认值
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="selector">用于测试每个元素的条件</param>
        /// <param name="default">未找到匹配项时返回的默认值</param>
        /// <returns>第一个满足条件的项，如果未找到则返回默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法提供了安全的查找功能，不会在未找到匹配项时抛出异常。
        /// 如果集合中没有满足条件的项，或者找到的项为 null，则返回指定的默认值。
        /// </remarks>
        /// <seealso cref="Enumerable.FirstOrDefault{T}(IEnumerable{T}, Func{T, bool})"/>
        public static T GetOrDefault<T>(this ICollection<T> source, Func<T, bool> selector, T @default = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            
            if (!source.Any(selector))
            {
                return @default;
            }
            var item = source.FirstOrDefault(selector) ?? @default;
            return item;
        }

        /// <summary>
        /// 根据指定的选择器从集合中获取第一个匹配的项，如果未找到则创建并添加新项
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="selector">用于测试每个元素的条件</param>
        /// <param name="factory">未找到匹配项时用于创建新项的工厂函数</param>
        /// <returns>第一个满足条件的项，如果未找到则返回新创建并添加的项</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/>、<paramref name="selector"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法实现了"获取或创建"的模式，常用于缓存和延迟初始化场景。
        /// 如果集合中存在满足条件的项，则直接返回第一个匹配项。
        /// 如果不存在，则调用工厂函数创建新项，将其添加到集合中，然后返回该项。
        /// </remarks>
        /// <seealso cref="Func{T}"/>
        /// <seealso cref="Enumerable.First{T}(IEnumerable{T}, Func{T, bool})"/>
        public static T GetOrAdd<T>(this ICollection<T> source, Func<T, bool> selector, Func<T> factory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(factory, nameof(factory));
            
            if (!source.Any(selector))
            {
                var result = factory();
                source.Add(result);
                return result;
            }
            return source.First(selector);
        }
    }
}
