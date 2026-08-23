#if !NET8_0_OR_GREATER
using System;

using Scorpio;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 键控服务描述符。
    /// 继承自 <see cref="ServiceDescriptor"/>，用于在旧版
    /// <c>Microsoft.Extensions.DependencyInjection</c> 中表达带键的服务注册。
    /// </summary>
    /// <remarks>
    /// 基类构造仅用于满足 <see cref="ServiceDescriptor"/> 的类型要求，
    /// 实际解析统一使用 <c>KeyedImplementationType</c>、<c>KeyedImplementationInstance</c>
    /// 和 <c>KeyedImplementationFactory</c> 属性，不应将本类型直接交给旧版容器作为普通服务解析。
    /// </remarks>
    public sealed class KeyedServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// 使用实现类型初始化 <see cref="KeyedServiceDescriptor"/> 类的新实例。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationType">实现类型</param>
        /// <param name="lifetime">服务生命周期</param>
        public KeyedServiceDescriptor(
            Type serviceType,
            object serviceKey,
            Type implementationType,
            ServiceLifetime lifetime)
            : base(
                  Check.NotNull(serviceType, nameof(serviceType)),
                  Check.NotNull(implementationType, nameof(implementationType)),
                  lifetime)
        {
            ServiceKey = serviceKey;
            KeyedImplementationType = implementationType;
            IsKeyedService = true;
        }

        /// <summary>
        /// 使用实例初始化 <see cref="KeyedServiceDescriptor"/> 类的新实例。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationInstance">作为服务实现的实例</param>
        public KeyedServiceDescriptor(
            Type serviceType,
            object serviceKey,
            object implementationInstance)
            : base(
                  Check.NotNull(serviceType, nameof(serviceType)),
                  Check.NotNull(implementationInstance, nameof(implementationInstance)))
        {
            ServiceKey = serviceKey;
            KeyedImplementationInstance = implementationInstance;
            IsKeyedService = true;
        }

        /// <summary>
        /// 使用实现工厂初始化 <see cref="KeyedServiceDescriptor"/> 类的新实例。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <param name="implementationFactory">创建服务实例的工厂</param>
        /// <param name="lifetime">服务生命周期</param>
        public KeyedServiceDescriptor(
            Type serviceType,
            object serviceKey,
            Func<IServiceProvider, object, object> implementationFactory,
            ServiceLifetime lifetime)
            : base(
                  Check.NotNull(serviceType, nameof(serviceType)),
                  CreateBaseFactory(implementationFactory, serviceKey),
                  lifetime)
        {
            ServiceKey = serviceKey;
            KeyedImplementationFactory = implementationFactory;
            IsKeyedService = true;
        }

        /// <summary>
        /// 获取该键控服务注册所使用的键。
        /// </summary>
        /// <value>服务键，可以为 null</value>
        public object ServiceKey { get; }

        /// <summary>
        /// 获取键控服务注册的实现类型。
        /// </summary>
        /// <value>实现类型；如果使用实例或工厂注册，则为 null</value>
        public Type KeyedImplementationType { get; }

        /// <summary>
        /// 获取键控服务注册的实现实例。
        /// </summary>
        /// <value>实现实例；如果使用实现类型或工厂注册，则为 null</value>
        public object KeyedImplementationInstance { get; }

        /// <summary>
        /// 获取键控服务注册的实现工厂。
        /// </summary>
        /// <value>实现工厂；如果使用实现类型或实例注册，则为 null</value>
        public Func<IServiceProvider, object, object> KeyedImplementationFactory { get; }

        /// <summary>
        /// 获取一个值，指示该描述符是否为键控服务描述符。
        /// </summary>
        /// <value>始终返回 true</value>
        public bool IsKeyedService { get; }

        /// <summary>
        /// 创建基类 <see cref="ServiceDescriptor"/> 使用的普通工厂委托。
        /// </summary>
        /// <param name="implementationFactory">键控服务实现工厂</param>
        /// <param name="serviceKey">服务键</param>
        /// <returns>适配基类构造函数签名的工厂委托</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="implementationFactory"/> 为 null 时抛出</exception>
        private static Func<IServiceProvider, object> CreateBaseFactory(
            Func<IServiceProvider, object, object> implementationFactory,
            object serviceKey)
        {
            Check.NotNull(implementationFactory, nameof(implementationFactory));
            return provider => implementationFactory(provider, serviceKey);
        }
    }
}
#endif
