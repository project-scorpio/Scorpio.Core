using Scorpio;

namespace System.Collections.Generic
{
    /// <summary>
    /// 为 <see cref="IList{T}"/> 提供扩展方法的静态工具类
    /// </summary>
    /// <remarks>
    /// 此类包含一系列针对 <see cref="IList{T}"/> 和 <see cref="List{T}"/> 的扩展方法，
    /// 提供了查找索引、插入元素、替换元素、移动元素等常用的列表操作功能。
    /// </remarks>
    /// <seealso cref="IList{T}"/>
    /// <seealso cref="List{T}"/>
    public static class ListExtensions
    {
        /// <summary>
        /// 搜索与指定谓词定义的条件匹配的元素，并返回第一个匹配元素的从零开始的索引
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要搜索的列表</param>
        /// <param name="selector">用于定义搜索条件的谓词</param>
        /// <returns>如果找到与 <paramref name="selector"/> 定义的条件匹配的第一个元素，则为该元素的从零开始的索引；否则为 -1</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法从索引 0 开始向前搜索，并返回第一个满足条件的元素的索引。
        /// 类似于 <see cref="List{T}.FindIndex(Predicate{T})"/> 方法，但适用于任何 <see cref="IList{T}"/> 实现。
        /// </remarks>
        /// <seealso cref="List{T}.FindIndex(Predicate{T})"/>
        /// <seealso cref="Predicate{T}"/>
        public static int FindIndex<T>(this IList<T> source, Predicate<T> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            for (var i = 0; i < source.Count; ++i)
            {
                if (selector(source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 将元素插入到列表的开头（索引 0 处）
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要插入元素的列表</param>
        /// <param name="item">要插入的元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此方法相当于调用 <see cref="IList{T}.Insert(int, T)"/> 并传入索引 0。
        /// 插入后，原有的所有元素的索引都会增加 1。
        /// </remarks>
        /// <seealso cref="AddLast{T}(IList{T}, T)"/>
        /// <seealso cref="IList{T}.Insert"/>
        public static void AddFirst<T>(this IList<T> source, T item)
        {
            Check.NotNull(source, nameof(source));
            source.Insert(0, item);
        }

        /// <summary>
        /// 将元素添加到列表的末尾
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要添加元素的列表</param>
        /// <param name="item">要添加的元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此方法相当于调用 <see cref="IList{T}.Insert(int, T)"/> 并传入列表的当前长度作为索引。
        /// 等效于 <see cref="ICollection{T}.Add"/> 方法。
        /// </remarks>
        /// <seealso cref="AddFirst{T}(IList{T}, T)"/>
        /// <seealso cref="ICollection{T}.Add"/>
        public static void AddLast<T>(this IList<T> source, T item)
        {
            Check.NotNull(source, nameof(source));
            source.Insert(source.Count, item);
        }

        /// <summary>
        /// 在满足指定条件的第一个元素之后插入新元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于查找插入位置的谓词</param>
        /// <param name="item">要插入的元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 如果找到满足条件的元素，新元素将插入到该元素之后。
        /// 如果未找到满足条件的元素，新元素将插入到列表的开头。
        /// </remarks>
        /// <seealso cref="InsertBefore{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="FindIndex{T}(IList{T}, Predicate{T})"/>
        public static void InsertAfter<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            var index = source.FindIndex(selector);
            if (index < 0)
            {
                source.AddFirst(item);
                return;
            }

            source.Insert(index + 1, item);
        }

        /// <summary>
        /// 在满足指定条件的第一个元素之前插入新元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于查找插入位置的谓词</param>
        /// <param name="item">要插入的元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 如果找到满足条件的元素，新元素将插入到该元素之前。
        /// 如果未找到满足条件的元素，新元素将添加到列表的末尾。
        /// </remarks>
        /// <seealso cref="InsertAfter{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="FindIndex{T}(IList{T}, Predicate{T})"/>
        public static void InsertBefore<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            var index = source.FindIndex(selector);
            if (index < 0)
            {
                source.AddLast(item);
                return;
            }

            source.Insert(index, item);
        }

        /// <summary>
        /// 将列表中所有满足指定条件的元素替换为指定的元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于确定要替换的元素的谓词</param>
        /// <param name="item">用于替换的新元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此方法会遍历整个列表，将所有满足条件的元素都替换为指定的新元素。
        /// 如果需要只替换第一个匹配的元素，请使用 <see cref="ReplaceOne{T}(IList{T}, Predicate{T}, T)"/>。
        /// </remarks>
        /// <seealso cref="ReplaceOne{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="ReplaceWhile{T}(IList{T}, Predicate{T}, Func{T, T})"/>
        public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            for (var i = 0; i < source.Count; i++)
            {
                if (selector(source[i]))
                {
                    source[i] = item;
                }
            }
        }

        /// <summary>
        /// 使用工厂函数将列表中所有满足指定条件的元素替换为新元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于确定要替换的元素的谓词</param>
        /// <param name="itemFactory">接收原始元素并返回替换元素的工厂函数</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/>、<paramref name="selector"/> 或 <paramref name="itemFactory"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此重载允许基于原始元素的值来生成替换元素，提供了更大的灵活性。
        /// 工厂函数接收当前元素作为参数，并返回用于替换的新元素。
        /// </remarks>
        /// <seealso cref="ReplaceWhile{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="Func{T, TResult}"/>
        public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(itemFactory, nameof(itemFactory));

            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (selector(item))
                {
                    source[i] = itemFactory(item);
                }
            }
        }

        /// <summary>
        /// 将列表中第一个满足指定条件的元素替换为指定的元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于确定要替换的元素的谓词</param>
        /// <param name="item">用于替换的新元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此方法只替换第一个满足条件的元素，然后立即返回。
        /// 如果没有找到满足条件的元素，列表不会发生任何改变。
        /// 如果需要替换所有匹配的元素，请使用 <see cref="ReplaceWhile{T}(IList{T}, Predicate{T}, T)"/>。
        /// </remarks>
        /// <seealso cref="ReplaceWhile{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="ReplaceOne{T}(IList{T}, Predicate{T}, Func{T, T})"/>
        public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            for (var i = 0; i < source.Count; i++)
            {
                if (selector(source[i]))
                {
                    source[i] = item;
                    return;
                }
            }
        }

        /// <summary>
        /// 使用工厂函数将列表中第一个满足指定条件的元素替换为新元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于确定要替换的元素的谓词</param>
        /// <param name="itemFactory">接收原始元素并返回替换元素的工厂函数</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/>、<paramref name="selector"/> 或 <paramref name="itemFactory"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此重载允许基于原始元素的值来生成替换元素。
        /// 工厂函数只对第一个满足条件的元素调用，提供了更好的性能。
        /// </remarks>
        /// <seealso cref="ReplaceOne{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="Func{T, TResult}"/>
        public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(itemFactory, nameof(itemFactory));

            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (selector(item))
                {
                    source[i] = itemFactory(item);
                    return;
                }
            }
        }

        /// <summary>
        /// 将列表中第一个与指定元素相等的元素替换为新元素
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="item">要被替换的元素</param>
        /// <param name="replaceWith">用于替换的新元素</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <exception cref="NotSupportedException">当列表为只读时抛出</exception>
        /// <remarks>
        /// 此方法使用 <see cref="Comparer{T}.Default"/> 来比较元素的相等性。
        /// 只有第一个匹配的元素会被替换，如果没有找到匹配的元素，列表保持不变。
        /// </remarks>
        /// <seealso cref="ReplaceOne{T}(IList{T}, Predicate{T}, T)"/>
        /// <seealso cref="Comparer{T}.Default"/>
        public static void ReplaceOne<T>(this IList<T> source, T item, T replaceWith)
        {
            Check.NotNull(source, nameof(source));

            for (var i = 0; i < source.Count; i++)
            {
                if (Comparer<T>.Default.Compare(source[i], item) == 0)
                {
                    source[i] = replaceWith;
                    return;
                }
            }
        }

        /// <summary>
        /// 将满足指定条件的元素移动到列表中的指定位置
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <param name="source">要操作的列表</param>
        /// <param name="selector">用于查找要移动的元素的谓词</param>
        /// <param name="targetIndex">目标索引位置</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="selector"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="targetIndex"/> 超出有效范围时抛出</exception>
        /// <exception cref="InvalidOperationException">当找不到满足条件的元素时抛出</exception>
        /// <remarks>
        /// 此方法会查找第一个满足条件的元素，将其从当前位置移除，然后插入到目标位置。
        /// 目标索引必须在 0 到 <c>Count - 1</c> 之间。
        /// 如果元素已经在目标位置，则不执行任何操作。
        /// </remarks>
        /// <seealso cref="List{T}.RemoveAt"/>
        /// <seealso cref="List{T}.Insert"/>
        /// <seealso cref="FindIndex{T}(IList{T}, Predicate{T})"/>
        public static void MoveItem<T>(this List<T> source, Predicate<T> selector, int targetIndex)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            if (!targetIndex.IsBetween(0, source.Count - 1))
            {
                throw new ArgumentOutOfRangeException(nameof(targetIndex), $"targetIndex should be between 0 and {source.Count - 1}");
            }

            var currentIndex = source.FindIndex(0, selector);
            if (currentIndex < 0)
            {
                throw new InvalidOperationException("未找到满足条件的元素");
            }

            if (currentIndex == targetIndex)
            {
                return;
            }

            var item = source[currentIndex];
            source.RemoveAt(currentIndex);
            source.Insert(targetIndex, item);
        }
    }
}
