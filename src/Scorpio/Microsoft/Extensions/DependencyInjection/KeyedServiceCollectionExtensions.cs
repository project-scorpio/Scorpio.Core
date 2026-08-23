#if !NET8_0_OR_GREATER
using System;

using Scorpio;
using Scorpio.DependencyInjection.KeyedServices;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 键控服务注册的扩展方法类。
    /// 提供与 .NET 8 原生一致的 <c>AddKeyedSingleton</c>、<c>AddKeyedScoped</c> 和
    /// <c>AddKeyedTransient</c> 注册 API。
    /// </summary>
    public static class KeyedServiceCollectionExtensions
    {
        #region AddKeyedSingleton

        /// <summary>
        /// 以自身类型作为实现类型注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton<TService>(this IServiceCollection services, object serviceKey)
            where TService : class
            => AddKeyedSingleton(services, typeof(TService), serviceKey, typeof(TService));

        /// <summary>
        /// 使用指定实例注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton<TService>(this IServiceCollection services, object serviceKey, TService implementationInstance)
            where TService : class
            => AddKeyedSingleton(services, typeof(TService), serviceKey, implementationInstance);

        /// <summary>
        /// 使用实现工厂注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton<TService>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
            where TService : class
            => AddKeyedSingleton(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 使用指定实现类型注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton<TService, TImplementation>(this IServiceCollection services, object serviceKey)
            where TService : class
            where TImplementation : class, TService
            => AddKeyedSingleton(services, typeof(TService), serviceKey, typeof(TImplementation));

        /// <summary>
        /// 使用指定实现类型的实现工厂注册单例键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton<TService, TImplementation>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
            => AddKeyedSingleton(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 以自身类型作为实现类型注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey)
            => AddKeyedSingleton(services, serviceType, serviceKey, serviceType);

        /// <summary>
        /// 使用指定实现类型注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey, Type implementationType)
            => AddKeyed(services, serviceType, serviceKey, implementationType, ServiceLifetime.Singleton);

        /// <summary>
        /// 使用指定实例注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey, object implementationInstance)
            => AddKeyed(services, serviceType, serviceKey, implementationInstance);

        /// <summary>
        /// 使用实现工厂注册单例键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedSingleton(this IServiceCollection services, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> implementationFactory)
            => AddKeyed(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Singleton);

        #endregion

        #region AddKeyedScoped

        /// <summary>
        /// 以自身类型作为实现类型注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped<TService>(this IServiceCollection services, object serviceKey)
            where TService : class
            => AddKeyedScoped(services, typeof(TService), serviceKey, typeof(TService));

        /// <summary>
        /// 使用实现工厂注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped<TService>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
            where TService : class
            => AddKeyedScoped(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 使用指定实现类型注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped<TService, TImplementation>(this IServiceCollection services, object serviceKey)
            where TService : class
            where TImplementation : class, TService
            => AddKeyedScoped(services, typeof(TService), serviceKey, typeof(TImplementation));

        /// <summary>
        /// 使用指定实现类型的实现工厂注册作用域键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped<TService, TImplementation>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
            => AddKeyedScoped(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 以自身类型作为实现类型注册作用域键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped(this IServiceCollection services, Type serviceType, object serviceKey)
            => AddKeyedScoped(services, serviceType, serviceKey, serviceType);

        /// <summary>
        /// 使用指定实现类型注册作用域键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped(this IServiceCollection services, Type serviceType, object serviceKey, Type implementationType)
            => AddKeyed(services, serviceType, serviceKey, implementationType, ServiceLifetime.Scoped);

        /// <summary>
        /// 使用实现工厂注册作用域键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedScoped(this IServiceCollection services, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> implementationFactory)
            => AddKeyed(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Scoped);

        #endregion

        #region AddKeyedTransient

        /// <summary>
        /// 以自身类型作为实现类型注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient<TService>(this IServiceCollection services, object serviceKey)
            where TService : class
            => AddKeyedTransient(services, typeof(TService), serviceKey, typeof(TService));

        /// <summary>
        /// 使用实现工厂注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient<TService>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
            where TService : class
            => AddKeyedTransient(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 使用指定实现类型注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient<TService, TImplementation>(this IServiceCollection services, object serviceKey)
            where TService : class
            where TImplementation : class, TService
            => AddKeyedTransient(services, typeof(TService), serviceKey, typeof(TImplementation));

        /// <summary>
        /// 使用指定实现类型的实现工厂注册瞬态键控服务。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient<TService, TImplementation>(this IServiceCollection services, object serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
            => AddKeyedTransient(services, typeof(TService), serviceKey, implementationFactory);

        /// <summary>
        /// 以自身类型作为实现类型注册瞬态键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient(this IServiceCollection services, Type serviceType, object serviceKey)
            => AddKeyedTransient(services, serviceType, serviceKey, serviceType);

        /// <summary>
        /// 使用指定实现类型注册瞬态键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient(this IServiceCollection services, Type serviceType, object serviceKey, Type implementationType)
            => AddKeyed(services, serviceType, serviceKey, implementationType, ServiceLifetime.Transient);

        /// <summary>
        /// 使用实现工厂注册瞬态键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddKeyedTransient(this IServiceCollection services, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> implementationFactory)
            => AddKeyed(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Transient);

        #endregion

        /// <summary>
        /// 构建支持键控服务解析的服务提供程序。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>支持键控解析的 <see cref="IServiceProvider"/> 实例</returns>
        public static IServiceProvider BuildKeyedServiceProvider(this IServiceCollection services)
            => BuildKeyedServiceProvider(services, new ServiceProviderOptions());

        /// <summary>
        /// 使用指定选项构建支持键控服务解析的服务提供程序。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="options">服务提供程序选项</param>
        /// <returns>支持键控解析的 <see cref="IServiceProvider"/> 实例</returns>
        public static IServiceProvider BuildKeyedServiceProvider(this IServiceCollection services, ServiceProviderOptions options)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(options, nameof(options));

            var factory = new KeyedServiceProviderFactory(options);
            var builder = factory.CreateBuilder(services);
            return factory.CreateServiceProvider(builder);
        }

        /// <summary>
        /// 使用实现类型注册键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <returns>服务集合，支持链式调用</returns>
        private static IServiceCollection AddKeyed(
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
                .Add(new KeyedServiceDescriptor(serviceType, serviceKey, implementationType, lifetime));
            return services;
        }

        /// <summary>
        /// 使用实例注册键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        /// <returns>服务集合，支持链式调用</returns>
        private static IServiceCollection AddKeyed(
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
                .Add(new KeyedServiceDescriptor(serviceType, serviceKey, implementationInstance));
            return services;
        }

        /// <summary>
        /// 使用实现工厂注册键控服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <returns>服务集合，支持链式调用</returns>
        private static IServiceCollection AddKeyed(
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
                .Add(new KeyedServiceDescriptor(serviceType, serviceKey, implementationFactory, lifetime));
            return services;
        }
    }
}
#endif
