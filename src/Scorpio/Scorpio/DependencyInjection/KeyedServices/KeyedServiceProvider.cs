#if !NET8_0_OR_GREATER
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务提供程序。
    /// 包装普通 <see cref="IServiceProvider"/>，在普通解析之外提供键控服务解析、
    /// 键控生命周期缓存和键控感知构造注入能力。
    /// </summary>
    internal sealed class KeyedServiceProvider :
        IServiceProvider,
        IKeyedServiceProvider,
        IServiceProviderIsKeyedService,
        IServiceScopeFactory,
        ISupportRequiredService,
        IDisposable
    {
        /// <summary>
        /// 根键控服务提供程序。
        /// </summary>
        private readonly KeyedServiceProvider _root;

        /// <summary>
        /// 底层普通服务提供程序。
        /// </summary>
        private readonly IServiceProvider _inner;

        /// <summary>
        /// 键控服务注册表。
        /// </summary>
        private readonly KeyedServiceRegistry _registry;

        /// <summary>
        /// 键控感知实例激活器。
        /// </summary>
        private readonly KeyedServiceActivator _activator;

        /// <summary>
        /// 当前提供程序所属的作用域，根提供程序为 null。
        /// </summary>
        private readonly KeyedServiceScope _scope;

        /// <summary>
        /// 是否启用作用域校验。
        /// </summary>
        private readonly bool _validateScopes;

        /// <summary>
        /// 缓存键控单例以及从根解析的作用域服务实例。
        /// </summary>
        private readonly ConcurrentDictionary<ServiceIdentifier, Lazy<object>> _singletons =
            new ConcurrentDictionary<ServiceIdentifier, Lazy<object>>();

        /// <summary>
        /// 保护根容器可释放实例列表和释放状态的对象锁。
        /// </summary>
        private readonly object _disposeLock = new object();

        /// <summary>
        /// 按创建顺序保存根容器创建的键控可释放实例。
        /// </summary>
        private List<object> _disposables;

        /// <summary>
        /// 指示根容器是否已释放。
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 初始化根 <see cref="KeyedServiceProvider"/> 类的新实例。
        /// </summary>
        /// <param name="inner">底层普通服务提供程序</param>
        /// <param name="registry">键控服务注册表</param>
        /// <param name="activator">键控感知实例激活器</param>
        /// <param name="options">服务提供程序选项</param>
        public KeyedServiceProvider(
            IServiceProvider inner,
            KeyedServiceRegistry registry,
            KeyedServiceActivator activator,
            ServiceProviderOptions options)
        {
            _root = this;
            _inner = inner;
            _registry = registry;
            _activator = activator;
            _scope = null;
            _validateScopes = options != null && options.ValidateScopes;
        }

        /// <summary>
        /// 初始化作用域绑定的 <see cref="KeyedServiceProvider"/> 类的新实例。
        /// </summary>
        /// <param name="root">根键控服务提供程序</param>
        /// <param name="inner">作用域内的底层普通服务提供程序</param>
        /// <param name="registry">键控服务注册表</param>
        /// <param name="activator">键控感知实例激活器</param>
        /// <param name="scope">当前提供程序所属的作用域</param>
        internal KeyedServiceProvider(
            KeyedServiceProvider root,
            IServiceProvider inner,
            KeyedServiceRegistry registry,
            KeyedServiceActivator activator,
            KeyedServiceScope scope)
        {
            _root = root;
            _inner = inner;
            _registry = registry;
            _activator = activator;
            _scope = scope;
            _validateScopes = root._validateScopes;
        }

        /// <summary>
        /// 获取普通服务实例。
        /// 对服务提供程序自身相关的内置类型返回当前实例，其余委托给底层提供程序。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务实例；如果未找到，则返回 null</returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider) ||
                serviceType == typeof(IKeyedServiceProvider) ||
                serviceType == typeof(IServiceProviderIsKeyedService) ||
                serviceType == typeof(IServiceScopeFactory))
            {
                return this;
            }

            return _inner.GetService(serviceType);
        }

        /// <summary>
        /// 获取普通服务实例，未找到时抛出异常。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务实例</returns>
        /// <exception cref="InvalidOperationException">当服务未注册时抛出</exception>
        public object GetRequiredService(Type serviceType)
        {
            Check.NotNull(serviceType, nameof(serviceType));
            return GetService(serviceType) ??
                throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");
        }

        /// <summary>
        /// 按键获取服务实例，未找到时返回 null。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务实例；如果未找到，则返回 null</returns>
        public object GetKeyedService(Type serviceType, object serviceKey)
            => ResolveKeyed(serviceType, serviceKey, false);

        /// <summary>
        /// 按键获取服务实例，未找到时抛出异常。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>服务实例</returns>
        /// <exception cref="InvalidOperationException">当键控服务未注册时抛出</exception>
        public object GetRequiredKeyedService(Type serviceType, object serviceKey)
            => ResolveKeyed(serviceType, serviceKey, true);

        /// <summary>
        /// 判断指定服务类型和键的组合是否已注册。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>如果已注册，则返回 true；否则返回 false</returns>
        public bool IsKeyedService(Type serviceType, object serviceKey)
        {
            Check.NotNull(serviceType, nameof(serviceType));

            // 开放泛型定义本身不可直接解析，与原生行为一致
            if (serviceType.IsGenericTypeDefinition)
            {
                return false;
            }

            if (_registry.Contains(serviceType, serviceKey))
            {
                return true;
            }

            if (serviceKey != null &&
                !ReferenceEquals(serviceKey, KeyedService.AnyKey) &&
                _registry.Contains(serviceType, KeyedService.AnyKey))
            {
                return true;
            }

            if (serviceType.IsConstructedGenericType)
            {
                var genericDefinition = serviceType.GetGenericTypeDefinition();
                if (_registry.Contains(genericDefinition, serviceKey))
                {
                    return true;
                }

                if (serviceKey != null &&
                    !ReferenceEquals(serviceKey, KeyedService.AnyKey) &&
                    _registry.Contains(genericDefinition, KeyedService.AnyKey))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 创建新的键控服务作用域。
        /// </summary>
        /// <returns>支持键控解析的服务作用域</returns>
        public IServiceScope CreateScope()
        {
            var innerScopeFactory = _inner.GetService(typeof(IServiceScopeFactory)) as IServiceScopeFactory;
            if (innerScopeFactory == null)
            {
                throw new InvalidOperationException(
                    $"No service for type '{typeof(IServiceScopeFactory)}' has been registered.");
            }

            var innerScope = innerScopeFactory.CreateScope();
            var scope = new KeyedServiceScope(_root, innerScope, _registry, _activator);
            KeyedServiceProviderAccessor.Register(innerScope.ServiceProvider, (KeyedServiceProvider)scope.ServiceProvider);
            return scope;
        }

        /// <summary>
        /// 供普通服务预处理工厂调用，执行键控感知实例创建。
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <returns>创建完成的实例</returns>
        internal object CreateKeyedAware(Type implementationType, Type serviceType, object serviceKey)
            => _activator.CreateInstance(implementationType, serviceType, serviceKey, this);

        /// <summary>
        /// 释放根容器及其创建的键控服务实例。
        /// </summary>
        public void Dispose()
        {
            if (_scope != null)
            {
                return;
            }

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

            (_inner as IDisposable)?.Dispose();
        }

        /// <summary>
        /// 按键解析单个服务实例。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="throwIfNotFound">未找到时是否抛出异常</param>
        /// <returns>服务实例；未找到且不要求抛异常时返回 null</returns>
        private object ResolveKeyed(Type serviceType, object serviceKey, bool throwIfNotFound)
        {
            Check.NotNull(serviceType, nameof(serviceType));

            // 与原生一致，通过 IEnumerable<T> 的键控解析提供 GetKeyedServices 能力
            if (serviceType.IsConstructedGenericType &&
                serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var itemType = serviceType.GetGenericArguments()[0];
                return CreateEnumerable(itemType, serviceKey);
            }

            var descriptor = FindDescriptor(serviceType, serviceKey);
            if (descriptor == null)
            {
                if (throwIfNotFound)
                {
                    throw new InvalidOperationException(
                        $"No service for type '{serviceType}' has been registered.");
                }

                return null;
            }

            return CreateKeyedInstance(descriptor, serviceType, serviceKey);
        }

        /// <summary>
        /// 查找指定服务类型和键的键控描述符。
        /// 依次尝试精确键、任意键兜底、开放泛型精确键和开放泛型任意键兜底。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>匹配的键控描述符；未找到时返回 null</returns>
        private KeyedServiceDescriptor FindDescriptor(Type serviceType, object serviceKey)
        {
            var descriptors = _registry.Get(serviceType, serviceKey);
            if (descriptors.Count > 0)
            {
                return descriptors[descriptors.Count - 1];
            }

            if (serviceKey != null && !ReferenceEquals(serviceKey, KeyedService.AnyKey))
            {
                descriptors = _registry.Get(serviceType, KeyedService.AnyKey);
                if (descriptors.Count > 0)
                {
                    return descriptors[descriptors.Count - 1];
                }
            }

            if (serviceType.IsConstructedGenericType)
            {
                var genericDefinition = serviceType.GetGenericTypeDefinition();
                descriptors = _registry.Get(genericDefinition, serviceKey);
                if (descriptors.Count > 0)
                {
                    return descriptors[descriptors.Count - 1];
                }

                if (serviceKey != null && !ReferenceEquals(serviceKey, KeyedService.AnyKey))
                {
                    descriptors = _registry.Get(genericDefinition, KeyedService.AnyKey);
                    if (descriptors.Count > 0)
                    {
                        return descriptors[descriptors.Count - 1];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 创建指定元素类型和键的强类型服务序列。
        /// </summary>
        /// <param name="itemType">服务元素类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>匹配的服务实例序列</returns>
        private object CreateEnumerable(Type itemType, object serviceKey)
        {
            var results = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
            AddResolvedServices(results, itemType, serviceKey);
            return results;
        }

        /// <summary>
        /// 向结果集合添加指定类型和键的所有键控服务实例。
        /// </summary>
        /// <param name="results">结果集合</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        private void AddResolvedServices(IList results, Type serviceType, object serviceKey)
        {
            // 与原生一致：非泛型类型存在精确注册时只返回精确注册，否则回退到 AnyKey
            if (!serviceType.IsConstructedGenericType)
            {
                var exactDescriptors = _registry.Get(serviceType, serviceKey);
                if (exactDescriptors.Count > 0)
                {
                    foreach (var descriptor in exactDescriptors)
                    {
                        results.Add(CreateKeyedInstance(descriptor, serviceType, serviceKey));
                    }

                    return;
                }

                if (serviceKey != null && !ReferenceEquals(serviceKey, KeyedService.AnyKey))
                {
                    foreach (var descriptor in _registry.Get(serviceType, KeyedService.AnyKey))
                    {
                        results.Add(CreateKeyedInstance(descriptor, serviceType, serviceKey));
                    }
                }

                return;
            }

            // 闭合泛型按原生顺序处理：先匹配精确闭合类型，再匹配开放泛型定义
            var genericDefinition = serviceType.GetGenericTypeDefinition();
            foreach (var descriptor in _registry.GetAll())
            {
                if (KeysMatch(descriptor.ServiceKey, serviceKey) && descriptor.ServiceType == serviceType)
                {
                    results.Add(CreateKeyedInstance(descriptor, serviceType, serviceKey));
                }
            }

            foreach (var descriptor in _registry.GetAll())
            {
                if (KeysMatch(descriptor.ServiceKey, serviceKey) &&
                    descriptor.ServiceType.IsGenericTypeDefinition &&
                    descriptor.ServiceType == genericDefinition)
                {
                    results.Add(CreateKeyedInstance(descriptor, serviceType, serviceKey));
                }
            }
        }

        /// <summary>
        /// 判断注册键与请求键是否匹配。
        /// 与原生 <c>KeysMatch</c> 语义一致：两个空键匹配，非空键相等或注册键为
        /// <see cref="KeyedService.AnyKey"/> 时匹配。
        /// </summary>
        /// <param name="registeredKey">注册时使用的键</param>
        /// <param name="requestedKey">请求解析使用的键</param>
        /// <returns>如果匹配，则返回 true；否则返回 false</returns>
        private static bool KeysMatch(object registeredKey, object requestedKey)
        {
            if (registeredKey == null && requestedKey == null)
            {
                return true;
            }

            if (registeredKey != null && requestedKey != null)
            {
                return registeredKey.Equals(KeyedService.AnyKey) || registeredKey.Equals(requestedKey);
            }

            return false;
        }

        /// <summary>
        /// 根据生命周期创建或缓存键控服务实例。
        /// </summary>
        /// <param name="descriptor">键控服务描述符</param>
        /// <param name="serviceType">服务类型，开放泛型描述符解析时为闭合类型</param>
        /// <param name="serviceKey">调用方请求的键，可以为 null</param>
        /// <returns>服务实例</returns>
        private object CreateKeyedInstance(
            KeyedServiceDescriptor descriptor,
            Type serviceType,
            object serviceKey)
        {
            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    return GetOrCreateSingleton(descriptor, serviceType, serviceKey);

                case ServiceLifetime.Scoped:
                    return CreateScopedInstance(descriptor, serviceType, serviceKey);

                default:
                    return CreateCore(descriptor, serviceType, serviceKey);
            }
        }

        /// <summary>
        /// 获取或创建键控单例实例。
        /// </summary>
        /// <param name="descriptor">键控服务描述符</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">调用方请求的键</param>
        /// <returns>缓存的或新创建的单例实例</returns>
        private object GetOrCreateSingleton(
            KeyedServiceDescriptor descriptor,
            Type serviceType,
            object serviceKey)
        {
            var identifier = new ServiceIdentifier(serviceType, serviceKey);
            var lazy = _root._singletons.GetOrAdd(
                identifier,
                key => new Lazy<object>(
                    () => CreateCore(descriptor, serviceType, serviceKey),
                    LazyThreadSafetyMode.ExecutionAndPublication));
            return lazy.Value;
        }

        /// <summary>
        /// 创建或缓存作用域键控服务实例。
        /// </summary>
        /// <param name="descriptor">键控服务描述符</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">调用方请求的键</param>
        /// <returns>服务实例</returns>
        private object CreateScopedInstance(
            KeyedServiceDescriptor descriptor,
            Type serviceType,
            object serviceKey)
        {
            if (_scope == null)
            {
                if (_validateScopes)
                {
                    throw new InvalidOperationException(
                        $"Cannot resolve scoped service '{serviceType}' from root provider.");
                }

                // 与原生一致：未启用作用域校验时，从根解析的作用域服务缓存在根容器
                return GetOrCreateSingleton(descriptor, serviceType, serviceKey);
            }

            return _scope.GetOrAdd(serviceType, serviceKey, () => CreateCore(descriptor, serviceType, serviceKey));
        }

        /// <summary>
        /// 创建键控服务实例并跟踪可释放实例。
        /// </summary>
        /// <param name="descriptor">键控服务描述符</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">调用方请求的键</param>
        /// <returns>创建完成的服务实例</returns>
        private object CreateCore(
            KeyedServiceDescriptor descriptor,
            Type serviceType,
            object serviceKey)
        {
            // 使用当前作用域绑定的提供程序，保证普通依赖按作用域解析
            var provider = _scope?.ServiceProvider ?? this;
            object instance;

            if (descriptor.KeyedImplementationInstance != null)
            {
                instance = descriptor.KeyedImplementationInstance;
            }
            else if (descriptor.KeyedImplementationFactory != null)
            {
                instance = descriptor.KeyedImplementationFactory(provider, serviceKey);
            }
            else
            {
                var implementationType = descriptor.KeyedImplementationType;
                if (implementationType != null &&
                    serviceType != descriptor.ServiceType &&
                    serviceType.IsConstructedGenericType)
                {
                    implementationType = implementationType.MakeGenericType(serviceType.GetGenericArguments());
                }

                instance = _activator.CreateInstance(implementationType, serviceType, serviceKey, provider);
            }

            // 与原生一致，用户注册的实例不由容器释放
            if (descriptor.KeyedImplementationInstance == null)
            {
                CaptureDisposable(instance);
            }

            return instance;
        }

        /// <summary>
        /// 跟踪容器创建的可释放实例。
        /// </summary>
        /// <param name="instance">要跟踪的实例</param>
        private void CaptureDisposable(object instance)
        {
            if (!(instance is IDisposable disposable))
            {
                return;
            }

            if (_scope != null)
            {
                _scope.CaptureDisposable(instance);
                return;
            }

            lock (_root._disposeLock)
            {
                if (_root._disposed)
                {
                    disposable.Dispose();
                    throw new ObjectDisposedException(nameof(KeyedServiceProvider));
                }

                if (_root._disposables == null)
                {
                    _root._disposables = new List<object>();
                }

                _root._disposables.Add(instance);
            }
        }
    }
}
#endif
