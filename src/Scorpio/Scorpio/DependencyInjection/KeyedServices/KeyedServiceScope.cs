#if !NET8_0_OR_GREATER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务作用域。
    /// 包装底层 <see cref="IServiceScope"/>，为作用域内的键控解析提供缓存和释放边界。
    /// </summary>
    internal sealed class KeyedServiceScope : IServiceScope
    {
        /// <summary>
        /// 底层的普通服务作用域。
        /// </summary>
        private readonly IServiceScope _inner;

        /// <summary>
        /// 缓存作用域内创建的键控服务实例。
        /// </summary>
        private readonly ConcurrentDictionary<ServiceIdentifier, Lazy<object>> _scopedInstances =
            new ConcurrentDictionary<ServiceIdentifier, Lazy<object>>();

        /// <summary>
        /// 保护可释放实例列表和释放状态的对象锁。
        /// </summary>
        private readonly object _disposeLock = new object();

        /// <summary>
        /// 按创建顺序保存由当前作用域创建的键控可释放实例。
        /// </summary>
        private List<object> _disposables;

        /// <summary>
        /// 指示当前作用域是否已释放。
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 初始化 <see cref="KeyedServiceScope"/> 类的新实例。
        /// </summary>
        /// <param name="root">根键控服务提供程序</param>
        /// <param name="inner">底层的普通服务作用域</param>
        /// <param name="registry">键控服务注册表</param>
        /// <param name="activator">键控感知实例激活器</param>
        public KeyedServiceScope(
            KeyedServiceProvider root,
            IServiceScope inner,
            KeyedServiceRegistry registry,
            KeyedServiceActivator activator)
        {
            _inner = inner;
            ServiceProvider = new KeyedServiceProvider(root, inner.ServiceProvider, registry, activator, this);
        }

        /// <summary>
        /// 获取作用域内的服务提供程序，支持键控解析。
        /// </summary>
        /// <value>作用域内的键控服务提供程序</value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 获取或创建作用域内的键控服务实例。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="factory">创建服务实例的工厂</param>
        /// <returns>作用域内缓存的或新创建的服务实例</returns>
        internal object GetOrAdd(Type serviceType, object serviceKey, Func<object> factory)
        {
            var identifier = new ServiceIdentifier(serviceType, serviceKey);
            var lazy = _scopedInstances.GetOrAdd(
                identifier,
                key => new Lazy<object>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
            return lazy.Value;
        }

        /// <summary>
        /// 捕获作用域创建的可释放实例，使其随作用域一起释放。
        /// </summary>
        /// <param name="instance">要跟踪的实例</param>
        internal void CaptureDisposable(object instance)
        {
            if (!(instance is IDisposable disposable))
            {
                return;
            }

            lock (_disposeLock)
            {
                if (_disposed)
                {
                    disposable.Dispose();
                    throw new ObjectDisposedException(nameof(KeyedServiceScope));
                }

                if (_disposables == null)
                {
                    _disposables = new List<object>();
                }

                _disposables.Add(instance);
            }
        }

        /// <summary>
        /// 释放当前作用域及其创建的键控服务实例。
        /// </summary>
        public void Dispose()
        {
            List<object> disposables;
            lock (_disposeLock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                disposables = _disposables;
            }

            if (disposables != null)
            {
                for (var i = disposables.Count - 1; i >= 0; i--)
                {
                    ((IDisposable)disposables[i]).Dispose();
                }
            }

            _inner.Dispose();
        }
    }
}
#endif
