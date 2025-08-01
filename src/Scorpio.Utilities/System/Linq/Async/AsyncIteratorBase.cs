using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Async
{
    // 设计说明：引入下面的基类是为了避免在迭代器值可以从另一个字段轻松推断出来的情况下（例如在 Repeat 中）
    // 存储 TSource 类型字段的开销。它也被 System.Interactive.Async 中的 Defer 操作符使用。
    // 对于一些操作符如 Where、Skip、Take 和 Concat，它可以用来从底层枚举器检索值。
    // 然而，这种方法的性能在某些情况下稍差，所以我们暂时不采用。
    // 一个需要做出的决定是当 MoveNextAsync 返回 false 时，Current 是否可以抛出异常，
    // 例如通过省略对枚举器字段的空检查。

    /// <summary>
    /// 异步迭代器的抽象基类，提供异步可枚举序列的基础实现
    /// </summary>
    /// <typeparam name="TSource">序列中元素的类型</typeparam>
    /// <remarks>
    /// 此类同时实现了 <see cref="IAsyncEnumerable{T}"/> 和 <see cref="IAsyncEnumerator{T}"/> 接口，
    /// 采用了迭代器模式来支持异步序列的延迟求值和状态管理。
    /// 基类设计考虑了性能优化，通过线程ID检查来复用迭代器实例。
    /// </remarks>
    /// <seealso cref="IAsyncEnumerable{T}"/>
    /// <seealso cref="IAsyncEnumerator{T}"/>
    /// <seealso cref="AsyncIterator{TSource}"/>
    internal abstract partial class AsyncIteratorBase<TSource> : IAsyncEnumerable<TSource>, IAsyncEnumerator<TSource>
    {
        /// <summary>
        /// 创建此迭代器时的托管线程标识符
        /// </summary>
        /// <remarks>
        /// 用于检查是否可以复用当前迭代器实例，避免不必要的克隆开销。
        /// 当在相同线程上且迭代器处于新建状态时，可以直接复用实例。
        /// </remarks>
        private readonly int _threadId;

        /// <summary>
        /// 迭代器的当前状态
        /// </summary>
        /// <remarks>
        /// 状态用于控制迭代器的生命周期，从新建到分配、迭代再到释放。
        /// 状态转换确保了迭代器的正确行为和资源管理。
        /// </remarks>
        /// <seealso cref="AsyncIteratorState"/>
        protected AsyncIteratorState _state = AsyncIteratorState.New;

        /// <summary>
        /// 用于取消异步操作的取消令牌
        /// </summary>
        /// <remarks>
        /// 取消令牌在迭代器创建时设置，并在整个迭代过程中使用。
        /// 支持协作式取消，允许优雅地中断长时间运行的异步序列操作。
        /// </remarks>
        /// <seealso cref="CancellationToken"/>
        protected CancellationToken _cancellationToken;

        /// <summary>
        /// 初始化 <see cref="AsyncIteratorBase{TSource}"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数记录当前托管线程的ID，用于后续的线程检查和实例复用优化。
        /// </remarks>
        protected AsyncIteratorBase() => _threadId = Environment.CurrentManagedThreadId;

        /// <summary>
        /// 返回循环访问异步可枚举集合的异步枚举器
        /// </summary>
        /// <param name="cancellationToken">用于取消异步操作的取消令牌</param>
        /// <returns>可用于循环访问异步可枚举集合的异步枚举器</returns>
        /// <exception cref="OperationCanceledException">当 <paramref name="cancellationToken"/> 已被取消时抛出</exception>
        /// <remarks>
        /// 此方法实现了线程安全的枚举器获取逻辑：
        /// - 如果在相同线程上且迭代器处于新建状态，则复用当前实例
        /// - 否则创建迭代器的克隆以支持多线程或多次枚举
        /// - 设置迭代器状态为已分配并保存取消令牌
        /// 
        /// 注意：根据 LDM-2018-11-28 的决定，此行为等效于 async iterator 的行为。
        /// </remarks>
        /// <seealso cref="Clone"/>
        /// <seealso cref="AsyncIteratorState.New"/>
        /// <seealso cref="AsyncIteratorState.Allocated"/>
        public IAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested(); // 注意：[LDM-2018-11-28] 等效于 async iterator 行为

            var enumerator = _state == AsyncIteratorState.New && _threadId == Environment.CurrentManagedThreadId
                ? this
                : Clone();

            enumerator._state = AsyncIteratorState.Allocated;
            enumerator._cancellationToken = cancellationToken;

            // 设计考虑：如果最终接口在这里包含 CancellationToken，我们是否应该在这里
            // 或在第一次调用 MoveNextAsync 时检查取消请求？

            return enumerator;
        }

        /// <summary>
        /// 异步释放迭代器使用的资源
        /// </summary>
        /// <returns>表示异步释放操作的 <see cref="ValueTask"/></returns>
        /// <remarks>
        /// 此虚方法设置迭代器状态为已释放，子类可以重写以执行额外的清理操作。
        /// 默认实现不执行实际的异步操作，直接返回已完成的 ValueTask。
        /// </remarks>
        /// <seealso cref="IAsyncDisposable.DisposeAsync"/>
        /// <seealso cref="AsyncIteratorState.Disposed"/>
        public virtual ValueTask DisposeAsync()
        {
            _state = AsyncIteratorState.Disposed;

            return default;
        }

        /// <summary>
        /// 获取枚举器的当前位置的元素
        /// </summary>
        /// <value>序列中枚举器当前位置的元素</value>
        /// <remarks>
        /// 此抽象属性必须由子类实现以提供当前元素的访问。
        /// 当迭代器尚未开始或已结束时，此属性的行为由具体实现定义。
        /// </remarks>
        /// <seealso cref="IAsyncEnumerator{T}.Current"/>
        public abstract TSource Current { get; }

        /// <summary>
        /// 异步推进枚举器到集合的下一个元素
        /// </summary>
        /// <returns>
        /// 一个任务，其结果为：如果枚举器成功推进到下一个元素，则为 <c>true</c>；
        /// 如果枚举器越过集合的末尾，则为 <c>false</c>
        /// </returns>
        /// <remarks>
        /// 此方法必须实现为异步方法，以确保从 MoveNextCore 调用抛出的任何异常都被
        /// try/catch 处理，无论它们是同步还是异步异常。
        /// 
        /// 方法行为：
        /// - 如果迭代器已释放，直接返回 false
        /// - 调用子类实现的 MoveNextCore 方法执行实际的移动逻辑
        /// - 如果发生异常，自动释放迭代器并重新抛出异常
        /// </remarks>
        /// <seealso cref="MoveNextCore"/>
        /// <seealso cref="DisposeAsync"/>
        /// <seealso cref="AsyncIteratorState.Disposed"/>
        public async ValueTask<bool> MoveNextAsync()
        {
            // 注意：MoveNext *必须* 实现为异步方法，以确保
            // 从 MoveNextCore 调用抛出的任何异常都被 try/catch 处理，
            // 无论它们是同步还是异步异常

            if (_state == AsyncIteratorState.Disposed)
            {
                return false;
            }

            try
            {
                return await MoveNextCore().ConfigureAwait(false);
            }
            catch
            {
                await DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// 创建此迭代器的副本
        /// </summary>
        /// <returns>此异步迭代器的新实例</returns>
        /// <remarks>
        /// 此抽象方法必须由子类实现，用于支持多次枚举或多线程访问。
        /// 克隆的迭代器应该具有相同的配置但独立的状态。
        /// </remarks>
        /// <seealso cref="GetAsyncEnumerator"/>
        public abstract AsyncIteratorBase<TSource> Clone();

        /// <summary>
        /// 执行移动到下一个元素的核心逻辑
        /// </summary>
        /// <returns>
        /// 一个任务，其结果为：如果成功移动到下一个元素，则为 <c>true</c>；
        /// 如果已到达序列末尾，则为 <c>false</c>
        /// </returns>
        /// <remarks>
        /// 此抽象方法包含子类特定的迭代逻辑，由 <see cref="MoveNextAsync"/> 调用。
        /// 子类应在此方法中实现具体的元素获取和状态更新逻辑。
        /// </remarks>
        /// <seealso cref="MoveNextAsync"/>
        protected abstract ValueTask<bool> MoveNextCore();
    }

    /// <summary>
    /// 具有当前元素存储的异步迭代器抽象类
    /// </summary>
    /// <typeparam name="TSource">序列中元素的类型</typeparam>
    /// <remarks>
    /// 此类扩展了 <see cref="AsyncIteratorBase{TSource}"/>，为大多数异步迭代器提供了
    /// 标准的当前元素存储和访问实现。它维护了一个 <typeparamref name="TSource"/> 类型的字段
    /// 来存储当前元素，并在释放时正确清理该字段。
    /// </remarks>
    /// <seealso cref="AsyncIteratorBase{TSource}"/>
    internal abstract class AsyncIterator<TSource> : AsyncIteratorBase<TSource>
    {
        /// <summary>
        /// 存储迭代器当前位置的元素
        /// </summary>
        /// <remarks>
        /// 此字段由子类在 MoveNextCore 实现中设置，通过 Current 属性对外公开。
        /// 在迭代器释放时会被重置为默认值以避免内存泄漏。
        /// </remarks>
        protected TSource _current = default!;

        /// <summary>
        /// 获取枚举器的当前位置的元素
        /// </summary>
        /// <value>序列中枚举器当前位置的元素</value>
        /// <remarks>
        /// 此实现直接返回 _current 字段的值。当迭代器尚未开始迭代或已完成时，
        /// 该值为 <typeparamref name="TSource"/> 类型的默认值。
        /// </remarks>
        /// <seealso cref="AsyncIteratorBase{TSource}.Current"/>
        public override TSource Current => _current;

        /// <summary>
        /// 异步释放迭代器使用的资源
        /// </summary>
        /// <returns>表示异步释放操作的 <see cref="ValueTask"/></returns>
        /// <remarks>
        /// 此重写实现在调用基类的释放方法之前，将当前元素重置为默认值。
        /// 这有助于防止对大对象的意外引用保持，从而避免潜在的内存泄漏。
        /// </remarks>
        /// <seealso cref="AsyncIteratorBase{TSource}.DisposeAsync"/>
        public override ValueTask DisposeAsync()
        {
            _current = default!;

            return base.DisposeAsync();
        }
    }

    /// <summary>
    /// 定义异步迭代器的状态枚举
    /// </summary>
    /// <remarks>
    /// 此枚举用于跟踪异步迭代器的生命周期状态，确保正确的状态转换和资源管理。
    /// 状态转换通常按以下顺序进行：New → Allocated → Iterating → Disposed
    /// </remarks>
    /// <seealso cref="AsyncIteratorBase{TSource}._state"/>
    internal enum AsyncIteratorState
    {
        /// <summary>
        /// 迭代器已创建但尚未分配给枚举操作
        /// </summary>
        /// <remarks>
        /// 这是迭代器的初始状态。在此状态下，迭代器可以被复用或克隆。
        /// </remarks>
        New = 0,

        /// <summary>
        /// 迭代器已分配给枚举操作但尚未开始迭代
        /// </summary>
        /// <remarks>
        /// 在调用 GetAsyncEnumerator 后，迭代器进入此状态。
        /// 取消令牌已设置，但尚未开始实际的元素枚举。
        /// </remarks>
        Allocated = 1,

        /// <summary>
        /// 迭代器正在进行元素枚举
        /// </summary>
        /// <remarks>
        /// 当第一次调用 MoveNextAsync 后，迭代器进入此状态。
        /// 在此状态下，迭代器正在积极地产生序列元素。
        /// </remarks>
        Iterating = 2,

        /// <summary>
        /// 迭代器已释放，不能再使用
        /// </summary>
        /// <remarks>
        /// 这是迭代器的终端状态。一旦进入此状态，迭代器就不能再执行任何操作。
        /// 任何后续的 MoveNextAsync 调用都将返回 false。
        /// </remarks>
        Disposed = -1,
    }
}
