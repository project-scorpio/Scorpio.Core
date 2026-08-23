#if !NET8_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务提供程序映射访问器。
    /// 将底层普通服务提供程序映射到对应的键控服务提供程序，
    /// 供普通服务预处理工厂在运行时定位作用域感知的键控提供程序。
    /// </summary>
    internal static class KeyedServiceProviderAccessor
    {
        /// <summary>
        /// 存储底层服务提供程序与键控服务提供程序之间映射的弱表。
        /// </summary>
        private static readonly ConditionalWeakTable<IServiceProvider, KeyedServiceProvider> Providers =
            new ConditionalWeakTable<IServiceProvider, KeyedServiceProvider>();

        /// <summary>
        /// 注册底层服务提供程序与键控服务提供程序的映射。
        /// </summary>
        /// <param name="innerProvider">底层普通服务提供程序</param>
        /// <param name="keyedProvider">对应的键控服务提供程序</param>
        public static void Register(IServiceProvider innerProvider, KeyedServiceProvider keyedProvider)
        {
            Providers.Remove(innerProvider);
            Providers.Add(innerProvider, keyedProvider);
        }

        /// <summary>
        /// 查找底层服务提供程序对应的键控服务提供程序。
        /// </summary>
        /// <param name="innerProvider">底层普通服务提供程序</param>
        /// <param name="fallback">未找到映射时使用的根键控服务提供程序</param>
        /// <returns>对应的键控服务提供程序</returns>
        public static KeyedServiceProvider Get(IServiceProvider innerProvider, KeyedServiceProvider fallback)
        {
            if (Providers.TryGetValue(innerProvider, out var keyedProvider))
            {
                return keyedProvider;
            }

            if (fallback == null)
            {
                throw new InvalidOperationException("The keyed service provider has not been initialized.");
            }

            return fallback;
        }
    }
}
#endif
