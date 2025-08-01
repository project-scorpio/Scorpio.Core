using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Scorpio;

namespace System.Linq.Async
{
    /// <summary>
    /// 为异步可枚举对象提供扩展方法的静态工具类
    /// </summary>
    /// <remarks>
    /// 此类提供了用于处理 <see cref="IAsyncEnumerable{T}"/> 的各种扩展方法和适配器实现，
    /// 支持将同步可枚举对象转换为异步可枚举对象，以及提供异步 LINQ 操作。
    /// </remarks>
    /// <seealso cref="IAsyncEnumerable{T}"/>
    /// <seealso cref="IEnumerable{T}"/>
    public static partial class AsyncEnumerable
    {
        /// <summary>
        /// 将可枚举序列转换为异步可枚举序列
        /// </summary>
        /// <typeparam name="TSource">源序列中元素的类型</typeparam>
        /// <param name="source">要转换为异步可枚举序列的可枚举序列</param>
        /// <returns>异步可枚举序列，其元素从给定的可枚举序列中提取</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法根据源序列的具体类型选择最优的适配器实现：
        /// - 对于 <see cref="IList{T}"/>，使用 <see cref="AsyncIListEnumerableAdapter{T}"/>
        /// - 对于 <see cref="ICollection{T}"/>，使用 <see cref="AsyncICollectionEnumerableAdapter{T}"/>
        /// - 对于其他类型，使用通用的 <see cref="AsyncEnumerableAdapter{T}"/>
        /// </remarks>
        /// <seealso cref="IList{T}"/>
        /// <seealso cref="ICollection{T}"/>
        public static IAsyncEnumerable<TSource> ToAsyncEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return source switch
            {
                IList<TSource> list => new AsyncIListEnumerableAdapter<TSource>(list),
                ICollection<TSource> collection => new AsyncICollectionEnumerableAdapter<TSource>(collection),
                _ => new AsyncEnumerableAdapter<TSource>(source),
            };
        }

        /// <summary>
        /// 异步可枚举适配器的抽象基类，提供从同步可枚举对象到异步可枚举对象的转换功能
        /// </summary>
        /// <typeparam name="T">序列中元素的类型</typeparam>
        /// <remarks>
        /// 此基类继承自 <see cref="AsyncIterator{T}"/> 并实现 <see cref="IAsyncIListProvider{T}"/>，
        /// 提供了异步迭代的基础功能和优化的集合操作方法。
        /// 内部使用 <see cref="IEnumerator{T}"/> 来枚举源序列，并通过 <see cref="Task.Run{TResult}(Func{TResult})"/> 将同步操作异步化。
        /// </remarks>
        /// <seealso cref="AsyncIterator{T}"/>
        /// <seealso cref="IAsyncIListProvider{T}"/>
        private abstract class AsyncEnumerableAdapterBase<T> : AsyncIterator<T>, IAsyncIListProvider<T>
        {
            /// <summary>
            /// 用于枚举源序列的枚举器
            /// </summary>
            /// <remarks>
            /// 此字段在异步迭代过程中保持对源序列枚举器的引用，
            /// 并在适当的时候进行释放以避免资源泄漏。
            /// </remarks>
            private IEnumerator<T> _enumerator;

            /// <summary>
            /// 初始化 <see cref="AsyncEnumerableAdapterBase{T}"/> 类的新实例
            /// </summary>
            /// <remarks>
            /// 基类构造函数，不执行特定的初始化操作。
            /// </remarks>
            protected AsyncEnumerableAdapterBase()
            {
            }

            /// <summary>
            /// 异步释放迭代器使用的资源
            /// </summary>
            /// <returns>表示异步释放操作的 <see cref="ValueTask"/></returns>
            /// <remarks>
            /// 此方法确保正确释放内部枚举器，并调用基类的释放方法。
            /// 在异步迭代完成或被中断时自动调用。
            /// </remarks>
            /// <seealso cref="IAsyncDisposable.DisposeAsync"/>
            public override async ValueTask DisposeAsync()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                await base.DisposeAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// 异步移动到序列中的下一个元素
            /// </summary>
            /// <returns>如果成功移动到下一个元素，则为 <c>true</c>；如果已到达序列末尾，则为 <c>false</c></returns>
            /// <remarks>
            /// 此方法实现了异步迭代的核心逻辑：
            /// - 在 <see cref="AsyncIteratorState.Allocated"/> 状态下初始化枚举器
            /// - 在 <see cref="AsyncIteratorState.Iterating"/> 状态下异步移动到下一个元素
            /// - 使用 <see cref="Task.Run{TResult}(Func{TResult})"/> 将同步的 <see cref="IEnumerator.MoveNext"/> 异步化
            /// - 使用 goto 语句优化状态转换性能
            /// </remarks>
            /// <seealso cref="AsyncIteratorState"/>
            [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S907:\"goto\" statement should not be used", Justification = "性能优化：避免不必要的状态检查")]
            protected override async ValueTask<bool> MoveNextCore()
            {
                switch (_state)
                {
                    case AsyncIteratorState.Allocated:
                        // 初始化枚举器并转换到迭代状态
                        _enumerator = GetEnumerator();
                        _state = AsyncIteratorState.Iterating;
                        goto case AsyncIteratorState.Iterating;

                    case AsyncIteratorState.Iterating:
                        // 异步移动到下一个元素
                        if (await Task.Run(_enumerator.MoveNext))
                        {
                            _current = _enumerator.Current;
                            return true;
                        }
                        // 已到达序列末尾，清理资源
                        await DisposeAsync().ConfigureAwait(false);
                        break;
                }
                return false;
            }

            /// <summary>
            /// 异步将序列转换为数组
            /// </summary>
            /// <param name="cancellationToken">用于取消操作的取消令牌</param>
            /// <returns>包含序列元素的数组</returns>
            /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
            /// <remarks>
            /// 此方法提供了优化的数组转换实现，直接使用同步方法并包装为 <see cref="ValueTask{TResult}"/>。
            /// 依赖于 System.Linq 实现的优化和短路逻辑。
            /// </remarks>
            /// <seealso cref="IAsyncIListProvider{T}.ToArrayAsync"/>
            public ValueTask<T[]> ToArrayAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return new ValueTask<T[]>(ToArray());
            }

            /// <summary>
            /// 异步将序列转换为列表
            /// </summary>
            /// <param name="cancellationToken">用于取消操作的取消令牌</param>
            /// <returns>包含序列元素的 <see cref="List{T}"/></returns>
            /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
            /// <remarks>
            /// 此方法提供了优化的列表转换实现，直接使用同步方法并包装为 <see cref="ValueTask{TResult}"/>。
            /// </remarks>
            /// <seealso cref="IAsyncIListProvider{T}.ToListAsync"/>
            public ValueTask<List<T>> ToListAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return new ValueTask<List<T>>(ToList());
            }

            /// <summary>
            /// 异步获取序列中元素的数量
            /// </summary>
            /// <param name="onlyIfCheap">指示是否仅在操作开销较小时获取计数</param>
            /// <param name="cancellationToken">用于取消操作的取消令牌</param>
            /// <returns>序列中元素的数量</returns>
            /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
            /// <remarks>
            /// 此方法提供了优化的计数实现。<paramref name="onlyIfCheap"/> 参数被忽略，
            /// 因为适配器可以高效地获取源序列的计数。
            /// </remarks>
            /// <seealso cref="IAsyncIListProvider{T}.GetCountAsync"/>
            public ValueTask<int> GetCountAsync(bool onlyIfCheap, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return new ValueTask<int>(Count());
            }

            /// <summary>
            /// 将序列转换为数组的抽象方法
            /// </summary>
            /// <returns>包含序列元素的数组</returns>
            /// <remarks>
            /// 子类必须实现此方法以提供特定于源序列类型的数组转换逻辑。
            /// </remarks>
            protected abstract T[] ToArray();

            /// <summary>
            /// 将序列转换为列表的抽象方法
            /// </summary>
            /// <returns>包含序列元素的 <see cref="List{T}"/></returns>
            /// <remarks>
            /// 子类必须实现此方法以提供特定于源序列类型的列表转换逻辑。
            /// </remarks>
            protected abstract List<T> ToList();

            /// <summary>
            /// 获取序列中元素数量的抽象方法
            /// </summary>
            /// <returns>序列中元素的数量</returns>
            /// <remarks>
            /// 子类必须实现此方法以提供特定于源序列类型的计数逻辑。
            /// </remarks>
            protected abstract int Count();

            /// <summary>
            /// 获取用于枚举序列的枚举器的抽象方法
            /// </summary>
            /// <returns>用于枚举序列的 <see cref="IEnumerator{T}"/></returns>
            /// <remarks>
            /// 子类必须实现此方法以提供特定于源序列类型的枚举器。
            /// </remarks>
            protected abstract IEnumerator<T> GetEnumerator();
        }

        /// <summary>
        /// 基于 <see cref="ICollection{T}"/> 的异步可枚举适配器抽象基类
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <remarks>
        /// 此类扩展了 <see cref="AsyncEnumerableAdapterBase{T}"/>，并实现了 <see cref="ICollection{T}"/> 接口，
        /// 提供了针对集合类型的优化实现。所有集合操作都委托给源集合。
        /// </remarks>
        /// <seealso cref="AsyncEnumerableAdapterBase{T}"/>
        /// <seealso cref="ICollection{T}"/>
        private abstract class AsyncICollectionEnumerableAdapterBase<T> : AsyncEnumerableAdapterBase<T>, ICollection<T>
        {
            /// <summary>
            /// 源集合的引用
            /// </summary>
            /// <remarks>
            /// 保持对源集合的引用，用于委托所有集合操作。
            /// </remarks>
            private readonly ICollection<T> _source;

            /// <summary>
            /// 初始化 <see cref="AsyncICollectionEnumerableAdapterBase{T}"/> 类的新实例
            /// </summary>
            /// <param name="source">要适配的源集合</param>
            /// <remarks>
            /// 构造函数接收源集合并保存其引用，用于后续的所有操作委托。
            /// </remarks>
            protected AsyncICollectionEnumerableAdapterBase(ICollection<T> source) => _source = source;

            /// <summary>
            /// 获取集合中元素的数量
            /// </summary>
            /// <returns>集合中元素的数量</returns>
            /// <remarks>
            /// 此实现直接返回源集合的 <see cref="ICollection{T}.Count"/> 属性值。
            /// </remarks>
            protected override int Count() => _source.Count;

            /// <summary>
            /// 获取用于枚举集合的枚举器
            /// </summary>
            /// <returns>用于枚举集合的 <see cref="IEnumerator{T}"/></returns>
            /// <remarks>
            /// 此实现直接返回源集合的枚举器。
            /// </remarks>
            protected override IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

            /// <summary>
            /// 将集合转换为数组
            /// </summary>
            /// <returns>包含集合元素的数组</returns>
            /// <remarks>
            /// 此实现使用 LINQ 的 <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/> 方法。
            /// </remarks>
            protected override T[] ToArray() => _source.ToArray();

            /// <summary>
            /// 将集合转换为列表
            /// </summary>
            /// <returns>包含集合元素的 <see cref="List{T}"/></returns>
            /// <remarks>
            /// 此实现使用 LINQ 的 <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> 方法。
            /// </remarks>
            protected override List<T> ToList() => _source.ToList();

            // IEnumerable<T> 接口的显式实现
            /// <summary>
            /// 返回循环访问集合的枚举器
            /// </summary>
            /// <returns>用于集合的 <see cref="IEnumerator{T}"/></returns>
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => _source.GetEnumerator();

            /// <summary>
            /// 返回循环访问集合的枚举器
            /// </summary>
            /// <returns>用于集合的 <see cref="IEnumerator"/></returns>
            IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();

            // ICollection<T> 接口的显式实现
            /// <summary>
            /// 将项添加到集合中
            /// </summary>
            /// <param name="item">要添加的项</param>
            void ICollection<T>.Add(T item) => _source.Add(item);

            /// <summary>
            /// 从集合中移除所有项
            /// </summary>
            void ICollection<T>.Clear() => _source.Clear();

            /// <summary>
            /// 确定集合是否包含特定值
            /// </summary>
            /// <param name="item">要在集合中定位的对象</param>
            /// <returns>如果在集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c></returns>
            bool ICollection<T>.Contains(T item) => _source.Contains(item);

            /// <summary>
            /// 从特定的数组索引开始，将集合的元素复制到数组中
            /// </summary>
            /// <param name="array">作为从集合复制的元素的目标的一维数组</param>
            /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
            void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _source.CopyTo(array, arrayIndex);

            /// <summary>
            /// 从集合中移除特定对象的第一个匹配项
            /// </summary>
            /// <param name="item">要从集合中移除的对象</param>
            /// <returns>如果成功移除 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c></returns>
            bool ICollection<T>.Remove(T item) => _source.Remove(item);

            /// <summary>
            /// 获取集合中包含的元素数
            /// </summary>
            /// <value>集合中包含的元素数</value>
            int ICollection<T>.Count => _source.Count;

            /// <summary>
            /// 获取一个值，该值指示集合是否为只读
            /// </summary>
            /// <value>如果集合为只读，则为 <c>true</c>；否则为 <c>false</c></value>
            bool ICollection<T>.IsReadOnly => _source.IsReadOnly;
        }

        /// <summary>
        /// 通用可枚举序列的异步适配器实现
        /// </summary>
        /// <typeparam name="T">序列中元素的类型</typeparam>
        /// <remarks>
        /// 此类是最通用的适配器实现，适用于任何 <see cref="IEnumerable{T}"/> 类型。
        /// 它不提供额外的集合或列表接口支持，但能处理所有可枚举序列。
        /// </remarks>
        /// <seealso cref="AsyncEnumerableAdapterBase{T}"/>
        private sealed class AsyncEnumerableAdapter<T> : AsyncEnumerableAdapterBase<T>
        {
            /// <summary>
            /// 源可枚举序列的引用
            /// </summary>
            /// <remarks>
            /// 保持对源序列的引用，用于获取枚举器和执行 LINQ 操作。
            /// </remarks>
            private readonly IEnumerable<T> _source;

            /// <summary>
            /// 初始化 <see cref="AsyncEnumerableAdapter{T}"/> 类的新实例
            /// </summary>
            /// <param name="source">要适配的源可枚举序列</param>
            /// <remarks>
            /// 构造函数接收源序列并保存其引用。
            /// </remarks>
            public AsyncEnumerableAdapter(IEnumerable<T> source) => _source = source;

            /// <summary>
            /// 创建此迭代器的副本
            /// </summary>
            /// <returns>此异步迭代器的新实例</returns>
            /// <remarks>
            /// 此方法用于支持多次枚举同一个异步序列。
            /// 每次枚举都会创建一个新的适配器实例。
            /// </remarks>
            /// <seealso cref="AsyncIteratorBase{T}.Clone"/>
            public override AsyncIteratorBase<T> Clone() => new AsyncEnumerableAdapter<T>(_source);

            /// <summary>
            /// 获取序列中元素的数量
            /// </summary>
            /// <returns>序列中元素的数量</returns>
            /// <remarks>
            /// 此实现使用 LINQ 的 <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/> 方法。
            /// </remarks>
            protected override int Count() => _source.Count();

            /// <summary>
            /// 获取用于枚举序列的枚举器
            /// </summary>
            /// <returns>用于枚举序列的 <see cref="IEnumerator{T}"/></returns>
            /// <remarks>
            /// 此实现直接返回源序列的枚举器。
            /// </remarks>
            protected override IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

            /// <summary>
            /// 将序列转换为数组
            /// </summary>
            /// <returns>包含序列元素的数组</returns>
            /// <remarks>
            /// 此实现使用 LINQ 的 <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/> 方法。
            /// </remarks>
            protected override T[] ToArray() => _source.ToArray();

            /// <summary>
            /// 将序列转换为列表
            /// </summary>
            /// <returns>包含序列元素的 <see cref="List{T}"/></returns>
            /// <remarks>
            /// 此实现使用 LINQ 的 <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> 方法。
            /// </remarks>
            protected override List<T> ToList() => _source.ToList();
        }

        /// <summary>
        /// 基于 <see cref="IList{T}"/> 的异异步可枚举适配器实现
        /// </summary>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <remarks>
        /// 此类是针对列表类型的专门适配器，继承自 <see cref="AsyncICollectionEnumerableAdapterBase{T}"/>
        /// 并额外实现了 <see cref="IList{T}"/> 接口，提供索引访问和列表特定操作。
        /// </remarks>
        /// <seealso cref="AsyncICollectionEnumerableAdapterBase{T}"/>
        /// <seealso cref="IList{T}"/>
        private sealed class AsyncIListEnumerableAdapter<T> : AsyncICollectionEnumerableAdapterBase<T>, IList<T>
        {
            /// <summary>
            /// 源列表的引用
            /// </summary>
            /// <remarks>
            /// 保持对源列表的引用，用于委托所有列表操作。
            /// </remarks>
            private readonly IList<T> _source;

            /// <summary>
            /// 初始化 <see cref="AsyncIListEnumerableAdapter{T}"/> 类的新实例
            /// </summary>
            /// <param name="source">要适配的源列表</param>
            /// <remarks>
            /// 构造函数接收源列表并将其同时传递给基类构造函数和保存为字段引用。
            /// </remarks>
            public AsyncIListEnumerableAdapter(IList<T> source) : base(source) => _source = source;

            /// <summary>
            /// 创建此迭代器的副本
            /// </summary>
            /// <returns>此异步迭代器的新实例</returns>
            /// <remarks>
            /// 此方法用于支持多次枚举同一个异步序列。
            /// 每次枚举都会创建一个新的适配器实例。
            /// </remarks>
            /// <seealso cref="AsyncIteratorBase{T}.Clone"/>
            public override AsyncIteratorBase<T> Clone() => new AsyncIListEnumerableAdapter<T>(_source);

            // IList<T> 接口的显式实现
            /// <summary>
            /// 确定列表中特定项的索引
            /// </summary>
            /// <param name="item">要在列表中定位的对象</param>
            /// <returns><paramref name="item"/> 在列表中的索引（如果找到）；否则为 -1</returns>
            int IList<T>.IndexOf(T item) => _source.IndexOf(item);

            /// <summary>
            /// 在列表中的指定索引处插入项
            /// </summary>
            /// <param name="index">应插入 <paramref name="item"/> 的从零开始的索引</param>
            /// <param name="item">要插入的对象</param>
            void IList<T>.Insert(int index, T item) => _source.Insert(index, item);

            /// <summary>
            /// 移除指定索引处的列表项
            /// </summary>
            /// <param name="index">要移除的项的从零开始的索引</param>
            void IList<T>.RemoveAt(int index) => _source.RemoveAt(index);

            /// <summary>
            /// 获取或设置指定索引处的元素
            /// </summary>
            /// <param name="index">要获取或设置的元素的从零开始的索引</param>
            /// <returns>指定索引处的元素</returns>
            T IList<T>.this[int index]
            {
                get => _source[index];
                set => _source[index] = value;
            }
        }

        /// <summary>
        /// 基于 <see cref="ICollection{T}"/> 的异步可枚举适配器实现
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <remarks>
        /// 此类是针对集合类型的专门适配器，继承自 <see cref="AsyncICollectionEnumerableAdapterBase{T}"/>。
        /// 它为非列表的集合类型提供优化的异步枚举支持。
        /// </remarks>
        /// <seealso cref="AsyncICollectionEnumerableAdapterBase{T}"/>
        private sealed class AsyncICollectionEnumerableAdapter<T> : AsyncICollectionEnumerableAdapterBase<T>
        {
            /// <summary>
            /// 源集合的引用
            /// </summary>
            /// <remarks>
            /// 保持对源集合的引用，主要用于克隆操作。
            /// </remarks>
            private readonly ICollection<T> _source;

            /// <summary>
            /// 初始化 <see cref="AsyncICollectionEnumerableAdapter{T}"/> 类的新实例
            /// </summary>
            /// <param name="source">要适配的源集合</param>
            /// <remarks>
            /// 构造函数接收源集合并将其同时传递给基类构造函数和保存为字段引用。
            /// </remarks>
            public AsyncICollectionEnumerableAdapter(ICollection<T> source) : base(source) => _source = source;

            /// <summary>
            /// 创建此迭代器的副本
            /// </summary>
            /// <returns>此异步迭代器的新实例</returns>
            /// <remarks>
            /// 此方法用于支持多次枚举同一个异步序列。
            /// 每次枚举都会创建一个新的适配器实例。
            /// </remarks>
            /// <seealso cref="AsyncIteratorBase{T}.Clone"/>
            public override AsyncIteratorBase<T> Clone() => new AsyncICollectionEnumerableAdapter<T>(_source);
        }
    }
}
