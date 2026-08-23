#if !NET8_0_OR_GREATER
using System;

using Scorpio;
using Scorpio.DependencyInjection.KeyedServices;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    /// <summary>
    /// 键控服务描述符扩展方法类。
    /// 提供与 .NET 8 原生一致的 <c>TryAddKeyed*</c> 和 <c>RemoveAllKeyed</c> API。
    /// </summary>
    public static class KeyedServiceCollectionDescriptorExtensions
    {
        #region TryAddKeyedSingleton

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则以自身类型作为实现类型注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedSingleton<TService>(this IServiceCollection services, object serviceKey)
            where TService : class
            => TryAddKeyedSingleton(services, typeof(TService), serviceKey, typeof(TService));

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实例注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        public static void TryAddKeyedSingleton<TService>(this IServiceCollection services, object serviceKey, TService implementationInstance)
            where TService : class
            => TryAddKeyedSingleton(services, typeof(TService), serviceKey, implementationInstance);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedSingleton<TService>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
            where TService : class
            => TryAddKeyedSingleton(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedSingleton<TService, TImplementation>(this IServiceCollection services, object serviceKey)
            where TService : class
            where TImplementation : class, TService
            => TryAddKeyedSingleton(services, typeof(TService), serviceKey, typeof(TImplementation));

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型的实现工厂注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedSingleton<TService, TImplementation>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
            => TryAddKeyedSingleton(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则以自身类型作为实现类型注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey)
            => TryAddKeyedSingleton(services, serviceType, serviceKey, serviceType);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        public static void TryAddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey, Type implementationType)
            => TryAddKeyed(services, serviceType, serviceKey, implementationType, ServiceLifetime.Singleton);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实例注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        public static void TryAddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey, object implementationInstance)
            => TryAddKeyed(services, serviceType, serviceKey, implementationInstance);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> implementationFactory)
            => TryAddKeyed(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Singleton);

        #endregion

        #region TryAddKeyedScoped

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则以自身类型作为实现类型注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedScoped<TService>(this IServiceCollection services, object serviceKey)
            where TService : class
            => TryAddKeyedScoped(services, typeof(TService), serviceKey, typeof(TService));

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedScoped<TService>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
            where TService : class
            => TryAddKeyedScoped(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedScoped<TService, TImplementation>(this IServiceCollection services, object serviceKey)
            where TService : class
            where TImplementation : class, TService
            => TryAddKeyedScoped(services, typeof(TService), serviceKey, typeof(TImplementation));

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型的实现工厂注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedScoped<TService, TImplementation>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
            => TryAddKeyedScoped(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则以自身类型作为实现类型注册作用域键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedScoped(this IServiceCollection services, Type serviceType, object serviceKey)
            => TryAddKeyedScoped(services, serviceType, serviceKey, serviceType);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型注册作用域键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        public static void TryAddKeyedScoped(this IServiceCollection services, Type serviceType, object serviceKey, Type implementationType)
            => TryAddKeyed(services, serviceType, serviceKey, implementationType, ServiceLifetime.Scoped);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册作用域键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedScoped(this IServiceCollection services, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> implementationFactory)
            => TryAddKeyed(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Scoped);

        #endregion

        #region TryAddKeyedTransient

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则以自身类型作为实现类型注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedTransient<TService>(this IServiceCollection services, object serviceKey)
            where TService : class
            => TryAddKeyedTransient(services, typeof(TService), serviceKey, typeof(TService));

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedTransient<TService>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
            where TService : class
            => TryAddKeyedTransient(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedTransient<TService, TImplementation>(this IServiceCollection services, object serviceKey)
            where TService : class
            where TImplementation : class, TService
            => TryAddKeyedTransient(services, typeof(TService), serviceKey, typeof(TImplementation));

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型的实现工厂注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedTransient<TService, TImplementation>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
            => TryAddKeyedTransient(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则以自身类型作为实现类型注册瞬态键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public static void TryAddKeyedTransient(this IServiceCollection services, Type serviceType, object serviceKey)
            => TryAddKeyedTransient(services, serviceType, serviceKey, serviceType);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用指定实现类型注册瞬态键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        public static void TryAddKeyedTransient(this IServiceCollection services, Type serviceType, object serviceKey, Type implementationType)
            => TryAddKeyed(services, serviceType, serviceKey, implementationType, ServiceLifetime.Transient);

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册瞬态键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        public static void TryAddKeyedTransient(this IServiceCollection services, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> implementationFactory)
            => TryAddKeyed(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Transient);

        #endregion

        #region RemoveAllKeyed

        /// <summary>
        /// 移除指定服务类型和键的所有键控注册。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection RemoveAllKeyed<TService>(this IServiceCollection services, object serviceKey)
            => RemoveAllKeyed(services, typeof(TService), serviceKey);

        /// <summary>
        /// 移除指定服务类型和键的所有键控注册。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection RemoveAllKeyed(this IServiceCollection services, Type serviceType, object serviceKey)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(serviceType, nameof(serviceType));
            KeyedServiceRegistryAccessor.GetOrCreate(services).RemoveAll(serviceType, serviceKey);
            return services;
        }

        #endregion

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现类型注册键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        /// <param name="lifetime">服务生命周期</param>
        private static void TryAddKeyed(
            IServiceCollection services,
            Type serviceType,
            object serviceKey,
            Type implementationType,
            ServiceLifetime lifetime)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(serviceType, nameof(serviceType));
            Check.NotNull(implementationType, nameof(implementationType));

            KeyedServiceRegistryAccessor
                .GetOrCreate(services)
                .TryAdd(new KeyedServiceDescriptor(serviceType, serviceKey, implementationType, lifetime));
        }

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实例注册键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        private static void TryAddKeyed(
            IServiceCollection services,
            Type serviceType,
            object serviceKey,
            object implementationInstance)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(serviceType, nameof(serviceType));
            Check.NotNull(implementationInstance, nameof(implementationInstance));

            KeyedServiceRegistryAccessor
                .GetOrCreate(services)
                .TryAdd(new KeyedServiceDescriptor(serviceType, serviceKey, implementationInstance));
        }

        /// <summary>
        /// 若服务类型与键的组合尚未注册，则使用实现工厂注册键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <param name="lifetime">服务生命周期</param>
        private static void TryAddKeyed(
            IServiceCollection services,
            Type serviceType,
            object serviceKey,
            Func<IServiceProvider, object, object> implementationFactory,
            ServiceLifetime lifetime)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(serviceType, nameof(serviceType));
            Check.NotNull(implementationFactory, nameof(implementationFactory));

            KeyedServiceRegistryAccessor
                .GetOrCreate(services)
                .TryAdd(new KeyedServiceDescriptor(serviceType, serviceKey, implementationFactory, lifetime));
        }
    }
}
#endif
