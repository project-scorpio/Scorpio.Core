#if !NET8_0_OR_GREATER
using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 普通服务类型索引。
    /// 从服务集合快照普通服务的服务类型，供构造器选择阶段判断参数是否可解析。
    /// 独立于键控注册表，保证键控服务不会被误判为普通服务。
    /// </summary>
    internal sealed class OrdinaryServiceRegistry
    {
        /// <summary>
        /// 已注册的普通服务类型集合。
        /// </summary>
        private readonly HashSet<Type> _serviceTypes;

        /// <summary>
        /// 已注册的开放泛型普通服务类型集合。
        /// </summary>
        private readonly HashSet<Type> _openGenericServiceTypes;

        /// <summary>
        /// 初始化 <see cref="OrdinaryServiceRegistry"/> 类的新实例。
        /// </summary>
        /// <param name="serviceTypes">普通服务类型集合</param>
        /// <param name="openGenericServiceTypes">开放泛型普通服务类型集合</param>
        private OrdinaryServiceRegistry(HashSet<Type> serviceTypes, HashSet<Type> openGenericServiceTypes)
        {
            _serviceTypes = serviceTypes;
            _openGenericServiceTypes = openGenericServiceTypes;
        }

        /// <summary>
        /// 从服务集合创建普通服务类型索引。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>普通服务类型索引</returns>
        public static OrdinaryServiceRegistry Create(IServiceCollection services)
        {
            var serviceTypes = new HashSet<Type>();
            var openGenericServiceTypes = new HashSet<Type>();

            foreach (var descriptor in services)
            {
                if (descriptor.ServiceType.IsGenericTypeDefinition)
                {
                    openGenericServiceTypes.Add(descriptor.ServiceType);
                }
                else
                {
                    serviceTypes.Add(descriptor.ServiceType);
                }
            }

            return new OrdinaryServiceRegistry(serviceTypes, openGenericServiceTypes);
        }

        /// <summary>
        /// 判断指定服务类型是否可作为普通服务解析。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>如果可解析，则返回 true；否则返回 false</returns>
        public bool IsService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider) ||
                serviceType == typeof(IKeyedServiceProvider) ||
                serviceType == typeof(IServiceProviderIsKeyedService) ||
                serviceType == typeof(IServiceScopeFactory)
#if NET6_0_OR_GREATER
                || serviceType == typeof(IServiceProviderIsService)
#endif
                )
            {
                return true;
            }

            // 与原生一致，IEnumerable<T> 即使没有注册也始终可解析为空序列
            if (serviceType.IsConstructedGenericType &&
                serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }

            if (_serviceTypes.Contains(serviceType))
            {
                return true;
            }

            return serviceType.IsConstructedGenericType &&
                _openGenericServiceTypes.Contains(serviceType.GetGenericTypeDefinition());
        }
    }
}
#endif
