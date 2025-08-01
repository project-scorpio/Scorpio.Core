using System.Threading;

namespace Scorpio.Threading
{
    /// <summary>
    /// 提供永不取消的取消令牌的提供程序实现。
    /// 实现 <see cref="ICancellationTokenProvider"/> 接口，始终返回 <see cref="CancellationToken.None"/>。
    /// 使用单例模式确保全局唯一实例。
    /// </summary>
    public class NoneCancellationTokenProvider : ICancellationTokenProvider
    {
        /// <summary>
        /// 获取 <see cref="NoneCancellationTokenProvider"/> 的单例实例。
        /// 提供全局访问点，确保应用程序中只有一个实例。
        /// </summary>
        /// <value>
        /// <see cref="NoneCancellationTokenProvider"/> 的唯一实例
        /// </value>
        public static NoneCancellationTokenProvider Instance { get; } = new NoneCancellationTokenProvider();

        /// <summary>
        /// 获取永不取消的取消令牌。
        /// 始终返回 <see cref="CancellationToken.None"/>，表示永远不会被取消的操作。
        /// </summary>
        /// <value>
        /// <see cref="CancellationToken.None"/>，表示不支持取消的令牌
        /// </value>
        public CancellationToken Token { get; } = CancellationToken.None;

        /// <summary>
        /// 初始化 <see cref="NoneCancellationTokenProvider"/> 类的新实例。
        /// 私有构造函数确保只能通过 <see cref="Instance"/> 属性获取实例。
        /// </summary>
        private NoneCancellationTokenProvider()
        {

        }
    }
}
