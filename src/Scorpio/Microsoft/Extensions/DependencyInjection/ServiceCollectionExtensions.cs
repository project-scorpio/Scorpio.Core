using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Scorpio;
using Scorpio.Conventional;
using Scorpio.DependencyInjection.Conventional;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展类
    /// 提供依赖注入容器的增强功能，包括约定操作、单例管理、服务替换等实用方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 执行约定操作
        /// 根据指定的类型集合和配置操作，创建并执行约定动作实例
        /// </summary>
        /// <typeparam name="TAction">约定动作类型，必须继承自 <see cref="ConventionalActionBase"/></typeparam>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="types">要处理的类型集合</param>
        /// <param name="configureAction">配置约定的操作委托</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ConventionalActionBase"/>
        /// <seealso cref="ConventionalConfiguration{T}"/>
        /// <seealso cref="IConventionalConfiguration{T}"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "需要访问非公共构造函数来创建约定动作实例")]
        public static IServiceCollection DoConventionalAction<TAction>(
            this IServiceCollection services,
            IEnumerable<Type> types,
            Action<IConventionalConfiguration<TAction>> configureAction)
            where TAction : ConventionalActionBase
        {
            // 创建约定配置实例，包含服务集合和类型信息
            var config = new ConventionalConfiguration<TAction>(services, types);
            // 执行配置操作，允许用户自定义约定规则
            configureAction(config);

            // 使用反射创建约定动作实例，支持访问非公共构造函数
            var action = Activator.CreateInstance(
                typeof(TAction),
                BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                null,
                new object[] { config },
                null) as TAction;

            // 执行约定动作的核心逻辑
            action.Action();
            return services;
        }

        /// <summary>
        /// 根据约定注册依赖注入
        /// 自动扫描指定类型并根据约定规则注册到依赖注入容器中
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="types">要扫描注册的类型集合</param>
        /// <param name="configureAction">配置约定的操作委托</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ConventionalDependencyAction"/>
        /// <seealso cref="DoConventionalAction{TAction}"/>
        public static IServiceCollection RegisterConventionalDependencyInject(
            this IServiceCollection services,
            IEnumerable<Type> types,
            Action<IConventionalConfiguration<ConventionalDependencyAction>> configureAction) =>
            // 委托给通用的约定操作方法处理
            services.DoConventionalAction(types, configureAction);

        /// <summary>
        /// 获取单例实例或返回 null
        /// 从服务集合中查找指定类型的单例服务实例
        /// </summary>
        /// <typeparam name="T">要获取的服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>如果找到则返回单例实例，否则返回 null</returns>
        /// <seealso cref="ServiceDescriptor"/>
        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            // 查找匹配服务类型的描述符并返回其实现实例
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }

        /// <summary>
        /// 获取单例实例或添加新实例
        /// 如果服务不存在则通过工厂方法创建并添加到服务集合中
        /// </summary>
        /// <typeparam name="T">服务类型，必须是引用类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="func">用于创建服务实例的工厂方法</param>
        /// <returns>现有的或新创建的服务实例</returns>
        /// <seealso cref="GetSingletonInstanceOrNull{T}"/>
        public static T GetSingletonInstanceOrAdd<T>(this IServiceCollection services, Func<IServiceCollection, T> func) where T : class
        {
            // 尝试获取现有实例
            var service = services.GetSingletonInstanceOrNull<T>();
            if (service == null)
            {
                // 如果不存在则通过工厂方法创建并注册为单例
                service = func(services);
                services.AddSingleton(service);
            }
            return service;
        }

        /// <summary>
        /// 获取单例实例或添加指定实例
        /// 如果服务不存在则将提供的实例添加到服务集合中
        /// </summary>
        /// <typeparam name="T">服务类型，必须是引用类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="instance">要添加的服务实例</param>
        /// <returns>现有的或新添加的服务实例</returns>
        /// <seealso cref="GetSingletonInstanceOrNull{T}"/>
        public static T GetSingletonInstanceOrAdd<T>(this IServiceCollection services, T instance) where T : class
        {
            // 尝试获取现有实例
            var service = services.GetSingletonInstanceOrNull<T>();
            if (service == null)
            {
                // 如果不存在则添加提供的实例
                service = instance;
                services.AddSingleton(service);
            }
            return service;
        }

        /// <summary>
        /// 获取单例实例
        /// 从服务集合中获取指定类型的单例服务实例，如果不存在则抛出异常
        /// </summary>
        /// <typeparam name="T">要获取的服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>单例服务实例</returns>
        /// <exception cref="InvalidOperationException">当找不到指定类型的单例服务时抛出</exception>
        /// <seealso cref="GetSingletonInstanceOrNull{T}"/>
        public static T GetSingletonInstance<T>(this IServiceCollection services)
        {
            var service = services.GetSingletonInstanceOrNull<T>() ?? throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
            return service;
        }

        /// <summary>
        /// 替换单例服务
        /// 移除现有的服务注册并添加新的单例服务注册
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">实现类型，必须实现 <typeparamref name="TService"/></typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="RemoveService{TService}"/>
        public static IServiceCollection ReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            // 先移除现有服务，再添加新的单例注册
            RemoveService<TService>(services);
            return services.AddSingleton<TService, TImplementation>();
        }

        /// <summary>
        /// 替换单例服务实例
        /// 移除现有的服务注册并添加新的单例实例
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="instance">新的服务实例</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="RemoveService{TService}"/>
        public static IServiceCollection ReplaceSingleton<TService>(this IServiceCollection services, TService instance)
            where TService : class
        {
            // 先移除现有服务，再添加新的单例实例
            RemoveService<TService>(services);
            return services.AddSingleton(instance);
        }

        /// <summary>
        /// 替换瞬态服务
        /// 移除现有的服务注册并添加新的瞬态服务注册
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">实现类型，必须实现 <typeparamref name="TService"/></typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="RemoveService{TService}"/>
        public static IServiceCollection ReplaceTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            // 先移除现有服务，再添加新的瞬态注册
            RemoveService<TService>(services);
            return services.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// 替换作用域服务
        /// 移除现有的服务注册并添加新的作用域服务注册
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">实现类型，必须实现 <typeparamref name="TService"/></typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="RemoveService{TService}"/>
        public static IServiceCollection ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            // 先移除现有服务，再添加新的作用域注册
            RemoveService<TService>(services);
            return services.AddScoped<TService, TImplementation>();
        }

        /// <summary>
        /// 移除指定类型的服务
        /// 从服务集合中移除第一个匹配指定服务类型的注册
        /// </summary>
        /// <typeparam name="TService">要移除的服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ServiceDescriptor"/>
        public static IServiceCollection RemoveService<TService>(IServiceCollection services) where TService : class
        {
            // 查找并移除匹配的服务描述符
            var old = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
            if (old != null)
            {
                services.Remove(old);
            }
            return services;
        }

        /// <summary>
        /// 替换或添加服务描述符
        /// 根据指定条件替换现有服务或添加新服务到集合中
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceDescriptor">要添加或替换的服务描述符</param>
        /// <param name="replaceAll">是否替换所有匹配的服务类型，默认为 false 只替换相同实现类型的服务</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ServiceDescriptor"/>
        /// <seealso cref="GetImplementationType"/>
        public static IServiceCollection ReplaceOrAdd(this IServiceCollection services, ServiceDescriptor serviceDescriptor, bool replaceAll = false)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(serviceDescriptor, nameof(serviceDescriptor));

            if (!replaceAll)
            {
                // 只移除相同服务类型和实现类型的注册
                var implementationType = serviceDescriptor.GetImplementationType();
                services.RemoveAll(s => s.ServiceType == serviceDescriptor.ServiceType && s.GetImplementationType() == implementationType);
            }
            else
            {
                // 移除所有相同服务类型的注册
                services.RemoveAll(s => s.ServiceType == serviceDescriptor.ServiceType);
            }

            // 添加新的服务描述符
            services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// 替换枚举服务中的特定实现
        /// 将源实现类型替换为目标实现类型
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TSourceImplementation">要替换的源实现类型</typeparam>
        /// <typeparam name="TDestinationImplementation">新的目标实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="lifetime">服务生命周期，默认为瞬态</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ServiceLifetime"/>
        /// <seealso cref="ReplaceEnumerable(IServiceCollection, ServiceDescriptor, ServiceDescriptor)"/>
        public static IServiceCollection ReplaceEnumerable<TService, TSourceImplementation, TDestinationImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TSourceImplementation : class, TService
            where TDestinationImplementation : class, TService =>
            services.ReplaceEnumerable(
                ServiceDescriptor.Transient<TService, TSourceImplementation>(),
                ServiceDescriptor.Describe(typeof(TService), typeof(TDestinationImplementation), lifetime));

        /// <summary>
        /// 替换枚举服务中的特定服务描述符
        /// 移除与源描述符匹配的服务并添加目标描述符
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="sourcedescriptor">要替换的源服务描述符</param>
        /// <param name="destdescriptor">新的目标服务描述符</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <exception cref="ArgumentException">当实现类型无法区分时抛出</exception>
        /// <seealso cref="ServiceDescriptor"/>
        /// <seealso cref="GetImplementationType"/>
        public static IServiceCollection ReplaceEnumerable(
            this IServiceCollection services,
            ServiceDescriptor sourcedescriptor,
            ServiceDescriptor destdescriptor)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(sourcedescriptor, nameof(sourcedescriptor));
            Check.NotNull(destdescriptor, nameof(destdescriptor));

            var implementationType = sourcedescriptor.GetImplementationType();

            // 验证实现类型的唯一性
            if (implementationType == typeof(object) ||
                implementationType == sourcedescriptor.ServiceType)
            {
                throw new ArgumentException($"Implementation type cannot be '{implementationType}' because it is indistinguishable from other services registered for '{sourcedescriptor.ServiceType}'.", nameof(sourcedescriptor));
            }

            // 查找并移除匹配的服务描述符
            var registeredServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == sourcedescriptor.ServiceType &&
                              s.GetImplementationType() == implementationType);
            if (registeredServiceDescriptor != null)
            {
                services.Remove(registeredServiceDescriptor);
            }

            // 添加新的服务描述符
            services.Add(destdescriptor);
            return services;
        }

        /// <summary>
        /// 从枚举服务中移除特定实现
        /// 移除指定服务类型和实现类型的注册
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">要移除的实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="RemoveEnumerable(IServiceCollection, ServiceDescriptor)"/>
        public static IServiceCollection RemoveEnumerable<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.RemoveEnumerable(ServiceDescriptor.Transient<TService, TImplementation>());

        /// <summary>
        /// 从枚举服务中移除指定的服务描述符
        /// 移除与描述符匹配的服务注册
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="descriptor">要移除的服务描述符</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <exception cref="ArgumentException">当实现类型无法区分时抛出</exception>
        /// <seealso cref="ServiceDescriptor"/>
        /// <seealso cref="GetImplementationType"/>
        public static IServiceCollection RemoveEnumerable(
            this IServiceCollection services,
            ServiceDescriptor descriptor)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(descriptor, nameof(descriptor));

            var implementationType = descriptor.GetImplementationType();

            // 验证实现类型的唯一性
            if (implementationType == typeof(object) ||
                implementationType == descriptor.ServiceType)
            {
                throw new ArgumentException($"Implementation type cannot be '{implementationType}' because it is indistinguishable from other services registered for '{descriptor.ServiceType}'.", nameof(descriptor));
            }

            // 查找并移除匹配的服务描述符
            var registeredServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == descriptor.ServiceType &&
                              s.GetImplementationType() == implementationType);
            if (registeredServiceDescriptor != null)
            {
                services.Remove(registeredServiceDescriptor);
            }
            return services;
        }

        /// <summary>
        /// 获取服务描述符的实现类型
        /// 从服务描述符中提取实际的实现类型信息
        /// </summary>
        /// <param name="serviceDescriptor">服务描述符</param>
        /// <returns>实现类型</returns>
        /// <seealso cref="ServiceDescriptor"/>
        /// <remarks>
        /// 支持三种类型的服务描述符：
        /// 1. 直接指定实现类型的
        /// 2. 通过实例注册的
        /// 3. 通过工厂方法注册的
        /// </remarks>
        internal static Type GetImplementationType(this ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor.ImplementationType != null)
            {
                // 直接返回实现类型
                return serviceDescriptor.ImplementationType;
            }
            else if (serviceDescriptor.ImplementationInstance != null)
            {
                // 从实例中获取类型
                return serviceDescriptor.ImplementationInstance.GetType();
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                // 从工厂方法的泛型参数中获取返回类型
                var typeArguments = serviceDescriptor.ImplementationFactory.GetType().GenericTypeArguments;
                Debug.Assert(typeArguments.Length == 2);
                return typeArguments[1];
            }

            Debug.Assert(false, "ImplementationType, ImplementationInstance or ImplementationFactory must be non null");
            return null;
        }
    }
}
