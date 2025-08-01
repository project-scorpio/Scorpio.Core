using System.Threading;

namespace Scorpio.Threading
{
    /// <summary>
    /// 定义取消令牌提供程序的接口。
    /// 提供获取用于协作取消操作的 <see cref="CancellationToken"/> 的能力。
    /// </summary>
    public interface ICancellationTokenProvider
    {
        /// <summary>
        /// 获取用于协作取消的令牌。
        /// 该令牌可用于通知长时间运行的操作应该被取消。
        /// </summary>
        /// <value>
        /// 一个 <see cref="CancellationToken"/>，用于监视取消请求。
        /// 当操作需要被取消时，此令牌的 <see cref="CancellationToken.IsCancellationRequested"/> 属性将变为 true。
        /// </value>
        CancellationToken Token { get; }
    }
}
