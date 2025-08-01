using System.Threading;

namespace Scorpio.Threading
{
    /// <summary>
    /// 为 <see cref="ICancellationTokenProvider"/> 提供扩展方法。
    /// 包含用于处理取消令牌回退逻辑的实用工具方法。
    /// </summary>
    public static class CancellationTokenProviderExtensions
    {
        /// <summary>
        /// 提供取消令牌的回退机制。
        /// 如果首选的取消令牌无效或为默认值，则回退到提供程序的取消令牌。
        /// </summary>
        /// <param name="provider">取消令牌提供程序实例</param>
        /// <param name="prefferedValue">
        /// 首选的取消令牌值，默认为 <see cref="CancellationToken.None"/>
        /// </param>
        /// <returns>
        /// 如果 <paramref name="prefferedValue"/> 有效且不为 <see cref="CancellationToken.None"/>，
        /// 则返回 <paramref name="prefferedValue"/>；
        /// 否则返回 <see cref="ICancellationTokenProvider.Token"/>
        /// </returns>
        public static CancellationToken FallbackToProvider(this ICancellationTokenProvider provider, CancellationToken prefferedValue = default)
        {
            return prefferedValue == default || prefferedValue == CancellationToken.None
                ? provider.Token
                : prefferedValue;
        }
    }
}
