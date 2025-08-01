using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Async
{
    /// <summary>
    /// 一个可以通过优化路径生成数组或 <see cref="List{TElement}"/> 的迭代器。
    /// </summary>
    /// <typeparam name="TElement">序列中元素的类型。</typeparam>
    public interface IAsyncIListProvider<TElement> : IAsyncEnumerable<TElement>
    {
        /// <summary>
        /// 通过优化路径生成序列的数组。
        /// </summary>
        /// <param name="cancellationToken">
        /// 用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>包含序列元素的数组。</returns>
        ValueTask<TElement[]> ToArrayAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 通过优化路径生成序列的 <see cref="List{TElement}"/>。
        /// </summary>
        /// <param name="cancellationToken">
        /// 用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>包含序列元素的 <see cref="List{TElement}"/>。</returns>
        ValueTask<List<TElement>> ToListAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 返回序列中元素的计数。
        /// </summary>
        /// <param name="onlyIfCheap">
        /// 如果为 true，则仅在计算计数快速（确定或可能是常数时间）的情况下才计算，否则应返回 -1。
        /// </param>
        /// <param name="cancellationToken">
        /// 用于观察任务完成过程的 <see cref="CancellationToken"/> 对象。
        /// </param>
        /// <returns>序列中的元素数量。</returns>
        ValueTask<int> GetCountAsync(bool onlyIfCheap, CancellationToken cancellationToken);
    }
}
