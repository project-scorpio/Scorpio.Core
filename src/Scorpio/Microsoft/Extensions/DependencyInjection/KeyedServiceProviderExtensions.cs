#if !NET8_0_OR_GREATER
using System;
using System.Collections.Generic;

using Scorpio;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 键控服务解析的扩展方法类。
    /// 提供与 .NET 8 原生一致的 <c>GetKeyedService</c>、<c>GetRequiredKeyedService</c>
    /// 和 <c>GetKeyedServices</c> 解析 API。
    /// </summary>
    public static class KeyedServiceProviderExtensions
    {
        /// <summary>
        /// 按键获取指定类型的服务实例，未找到时返回默认值。
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="provider">服务提供程序</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务实例；如果未找到，则返回 <typeparamref name="T"/> 的默认值</returns>
        public static T GetKeyedService<T>(this IServiceProvider provider, object serviceKey)
            => (T)GetKeyedService(provider, typeof(T), serviceKey);

        /// <summary>
        /// 按键获取指定类型的服务实例，未找到时返回 null。
        /// </summary>
        /// <param name="provider">服务提供程序</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务实例；如果未找到，则返回 null</returns>
        public static object GetKeyedService(this IServiceProvider provider, Type serviceType, object serviceKey)
        {
            Check.NotNull(provider, nameof(provider));
            Check.NotNull(serviceType, nameof(serviceType));

            if (provider is IKeyedServiceProvider keyedServiceProvider)
            {
                return keyedServiceProvider.GetKeyedService(serviceType, serviceKey);
            }

            throw new InvalidOperationException("This service provider doesn't support keyed services.");
        }

        /// <summary>
        /// 按键获取指定类型的服务实例，未找到时抛出异常。
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="provider">服务提供程序</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务实例</returns>
        /// <exception cref="InvalidOperationException">当未注册对应的键控服务时抛出</exception>
        public static T GetRequiredKeyedService<T>(this IServiceProvider provider, object serviceKey)
            => (T)GetRequiredKeyedService(provider, typeof(T), serviceKey);

        /// <summary>
        /// 按键获取指定类型的服务实例，未找到时抛出异常。
        /// </summary>
        /// <param name="provider">服务提供程序</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务实例</returns>
        /// <exception cref="InvalidOperationException">当未注册对应的键控服务时抛出</exception>
        public static object GetRequiredKeyedService(this IServiceProvider provider, Type serviceType, object serviceKey)
        {
            Check.NotNull(provider, nameof(provider));
            Check.NotNull(serviceType, nameof(serviceType));

            if (provider is IKeyedServiceProvider keyedServiceProvider)
            {
                return keyedServiceProvider.GetRequiredKeyedService(serviceType, serviceKey);
            }

            throw new InvalidOperationException("This service provider doesn't support keyed services.");
        }

        /// <summary>
        /// 按键获取指定类型的所有服务实例。
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="provider">服务提供程序</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>匹配该键的服务实例序列，按注册顺序返回</returns>
        public static IEnumerable<T> GetKeyedServices<T>(this IServiceProvider provider, object serviceKey)
        {
            Check.NotNull(provider, nameof(provider));
            return provider.GetRequiredKeyedService<IEnumerable<T>>(serviceKey);
        }

        /// <summary>
        /// 按键获取指定类型的所有服务实例。
        /// </summary>
        /// <param name="provider">服务提供程序</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>匹配该键的服务实例序列，按注册顺序返回</returns>
        public static IEnumerable<object> GetKeyedServices(this IServiceProvider provider, Type serviceType, object serviceKey)
        {
            Check.NotNull(provider, nameof(provider));
            Check.NotNull(serviceType, nameof(serviceType));

            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(serviceType);
            return (IEnumerable<object>)provider.GetRequiredKeyedService(genericEnumerable, serviceKey);
        }
    }
}
#endif
