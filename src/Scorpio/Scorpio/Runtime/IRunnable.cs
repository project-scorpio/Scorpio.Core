using System.Threading;
using System.Threading.Tasks;

namespace Scorpio.Runtime
{
    /// <summary>
    /// 定义可运行服务的接口。
    /// 提供启动和停止自管理线程服务的能力，支持异步操作和取消令牌。
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// 异步启动服务。
        /// 开始执行服务的主要功能，通常会创建和管理后台线程或异步任务。
        /// </summary>
        /// <param name="cancellationToken">
        /// 用于通知取消启动操作的 <see cref="CancellationToken"/>，
        /// 默认值为 <see cref="CancellationToken.None"/>
        /// </param>
        /// <returns>表示异步启动操作的 <see cref="Task"/></returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步停止服务。
        /// 优雅地关闭服务，清理资源并停止所有相关的后台操作。
        /// </summary>
        /// <param name="cancellationToken">
        /// 用于通知取消停止操作的 <see cref="CancellationToken"/>，
        /// 默认值为 <see cref="CancellationToken.None"/>
        /// </param>
        /// <returns>表示异步停止操作的 <see cref="Task"/></returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
