using System;
using System.Collections.Generic;

using Scorpio.Threading;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 代理目标提供者管理类，管理和维护所有已注册的代理目标提供者实例
    /// </summary>
    /// <remarks>
    /// 此类使用单例模式，提供对代理目标提供者集合的线程安全管理功能
    /// </remarks>
    public class ProxyTargetProvider
    {
        /// <summary>
        /// 存储代理目标提供者的内部哈希集合
        /// </summary>
        private readonly HashSet<IProxyTargetProvider> _providers;

        /// <summary>
        /// 获取代理目标提供者的只读集合
        /// </summary>
        /// <value>包含所有已注册代理目标提供者的只读集合 <see cref="IReadOnlyCollection{IProxyTargetProvider}"/></value>
        internal IReadOnlyCollection<IProxyTargetProvider> Providers => _providers;

        /// <summary>
        /// 延迟初始化的单例实例字段，保证线程安全
        /// </summary>
        private static readonly Lazy<ProxyTargetProvider> _provider = new Lazy<ProxyTargetProvider>(() => new ProxyTargetProvider(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// 获取代理目标提供者管理器的默认单例实例
        /// </summary>
        /// <value>默认的代理目标提供者管理器实例 <see cref="ProxyTargetProvider"/></value>
        public static ProxyTargetProvider Default => _provider.Value;

        /// <summary>
        /// 初始化代理目标提供者管理器的新实例
        /// </summary>
        /// <remarks>
        /// 私有构造函数，确保只能通过单例模式访问
        /// </remarks>
        private ProxyTargetProvider() => _providers = new HashSet<IProxyTargetProvider>();

        /// <summary>
        /// 添加代理目标提供者到管理器中
        /// </summary>
        /// <param name="provider">要添加的代理目标提供者实例 <see cref="IProxyTargetProvider"/></param>
        /// <remarks>
        /// 此方法是线程安全的，使用锁定机制确保集合操作的原子性
        /// </remarks>
        public void Add(IProxyTargetProvider provider) => _providers.Locking(p => p.Add(provider));
    }
}
