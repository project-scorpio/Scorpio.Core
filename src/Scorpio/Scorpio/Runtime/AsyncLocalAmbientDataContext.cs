using System.Collections.Concurrent;
using System.Threading;

using Scorpio.DependencyInjection;

namespace Scorpio.Runtime
{
    /// <summary>
    /// 基于 <see cref="AsyncLocal{T}"/> 的环境数据上下文实现。
    /// 实现 <see cref="IAmbientDataContext"/> 接口，提供异步本地存储功能，
    /// 同时作为单例依赖项注册到依赖注入容器中。
    /// </summary>
    /// <remarks>
    /// 此实现使用 <see cref="AsyncLocal{T}"/> 来确保数据在异步调用链中正确流转，
    /// 每个键对应一个独立的异步本地存储实例。
    /// </remarks>
    internal class AsyncLocalAmbientDataContext : IAmbientDataContext, ISingletonDependency
    {
        /// <summary>
        /// 用于存储异步本地变量的线程安全字典。
        /// 键为数据标识符，值为对应的 <see cref="AsyncLocal{Object}"/> 实例。
        /// </summary>
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> _asyncLocalDictionary = new ConcurrentDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// 设置指定键的环境数据值。
        /// 数据将存储在与键关联的异步本地存储中，在整个异步调用链中保持可见。
        /// </summary>
        /// <param name="key">数据的唯一标识符</param>
        /// <param name="value">要存储的数据值，可以为 null</param>
        public void SetData(string key, object value)
        {
            var asyncLocal = GetAsyncLocal(key);
            asyncLocal.Value = value;
        }

        /// <summary>
        /// 获取指定键对应的环境数据值。
        /// 从与键关联的异步本地存储中检索数据。
        /// </summary>
        /// <param name="key">数据的唯一标识符</param>
        /// <returns>存储的数据值，如果不存在则返回 null</returns>
        public object GetData(string key)
        {
            var asyncLocal = GetAsyncLocal(key);
            return asyncLocal.Value;
        }

        /// <summary>
        /// 获取或创建指定键对应的异步本地存储实例。
        /// 如果键不存在，将创建新的 <see cref="AsyncLocal{Object}"/> 实例并添加到字典中。
        /// </summary>
        /// <param name="key">数据的唯一标识符</param>
        /// <returns>与键关联的 <see cref="AsyncLocal{Object}"/> 实例</returns>
        private static AsyncLocal<object> GetAsyncLocal(string key) => _asyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object>());
    }
}
