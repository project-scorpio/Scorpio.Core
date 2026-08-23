#if !NET8_0_OR_GREATER
using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务提供程序工厂。
    /// 先使用底层 <see cref="IServiceCollection"/> 构建普通容器，再包装为支持键控解析的
    /// <see cref="KeyedServiceProvider"/>，并预处理构造函数中使用键控特性的普通服务。
    /// </summary>
    internal sealed class KeyedServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        /// <summary>
        /// 底层普通容器的构建选项。
        /// </summary>
        private readonly ServiceProviderOptions _options;

        /// <summary>
        /// 在 <see cref="CreateBuilder(IServiceCollection)"/> 阶段捕获的键控服务注册表。
        /// </summary>
        private KeyedServiceRegistry _registry;

        /// <summary>
        /// 初始化 <see cref="KeyedServiceProviderFactory"/> 类的新实例。
        /// </summary>
        public KeyedServiceProviderFactory()
            : this(new ServiceProviderOptions())
        {
        }

        /// <summary>
        /// 使用指定选项初始化 <see cref="KeyedServiceProviderFactory"/> 类的新实例。
        /// </summary>
        /// <param name="options">服务提供程序选项</param>
        public KeyedServiceProviderFactory(ServiceProviderOptions options)
            => _options = options ?? throw new ArgumentNullException(nameof(options));

        /// <summary>
        /// 创建容器构建器并捕获附加在服务集合上的键控注册表。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>原服务集合</returns>
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            _registry = KeyedServiceRegistryAccessor.GetOrCreate(services);
            return services;
        }

        /// <summary>
        /// 创建支持键控解析的服务提供程序。
        /// </summary>
        /// <param name="containerBuilder">服务集合</param>
        /// <returns>键控服务提供程序</returns>
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            var registry = _registry ?? KeyedServiceRegistryAccessor.GetOrCreate(containerBuilder);
            var ordinaryServices = OrdinaryServiceRegistry.Create(containerBuilder);
            var activator = new KeyedServiceActivator(ordinaryServices);
            var holder = new KeyedServiceProviderHolder();

            // 预处理普通服务，使其构造函数中的键控注入能够被识别
            PrepareOrdinaryServices(containerBuilder, holder);

            var inner = containerBuilder.BuildServiceProvider(_options);
            var provider = new KeyedServiceProvider(inner, registry, activator, _options);
            holder.Value = provider;
            KeyedServiceProviderAccessor.Register(inner, provider);
            return provider;
        }

        /// <summary>
        /// 预处理构造函数中使用键控特性的普通服务注册，将其替换为键控感知工厂。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="holder">根键控服务提供程序的持有对象</param>
        private static void PrepareOrdinaryServices(IServiceCollection services, KeyedServiceProviderHolder holder)
        {
            for (var i = 0; i < services.Count; i++)
            {
                var descriptor = services[i];
                if (descriptor.ImplementationType == null ||
                    descriptor.ImplementationType.ContainsGenericParameters ||
                    !RequiresKeyedAwareActivation(descriptor.ImplementationType))
                {
                    continue;
                }

                var implementationType = descriptor.ImplementationType;
                var serviceType = descriptor.ServiceType;
                services[i] = ServiceDescriptor.Describe(
                    serviceType,
                    provider => KeyedServiceProviderAccessor
                        .Get(provider, holder.Value)
                        .CreateKeyedAware(implementationType, serviceType, null),
                    descriptor.Lifetime);
            }
        }

        /// <summary>
        /// 判断实现类型的公共构造函数是否使用了键控注入特性。
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <returns>如果使用键控注入特性，则返回 true；否则返回 false</returns>
        private static bool RequiresKeyedAwareActivation(Type implementationType)
            => implementationType
                .GetConstructors()
                .SelectMany(constructor => constructor.GetParameters())
                .Any(parameter =>
                    parameter.IsDefined(typeof(FromKeyedServicesAttribute), true) ||
                    parameter.IsDefined(typeof(ServiceKeyAttribute), true));

        /// <summary>
        /// 保存根键控服务提供程序的持有对象。
        /// 用于在容器构建完成前被普通服务预处理工厂闭包捕获。
        /// </summary>
        private sealed class KeyedServiceProviderHolder
        {
            /// <summary>
            /// 获取或设置根键控服务提供程序。
            /// </summary>
            /// <value>根键控服务提供程序</value>
            public KeyedServiceProvider Value { get; set; }
        }
    }
}
#endif
