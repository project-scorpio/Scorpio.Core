#if !NET8_0_OR_GREATER
using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务注册表。
    /// 以服务类型和服务键的组合为索引保存所有键控服务描述符，
    /// 是键控服务兼容层解析时的事实来源。
    /// </summary>
    internal sealed class KeyedServiceRegistry
    {
        /// <summary>
        /// 存储服务标识到键控描述符列表的映射。
        /// </summary>
        private readonly Dictionary<ServiceIdentifier, List<KeyedServiceDescriptor>> _descriptors =
            new Dictionary<ServiceIdentifier, List<KeyedServiceDescriptor>>();

        /// <summary>
        /// 按注册顺序保存全部键控描述符，供枚举解析保持原生顺序。
        /// </summary>
        private readonly List<KeyedServiceDescriptor> _orderedDescriptors =
            new List<KeyedServiceDescriptor>();

        /// <summary>
        /// 保护注册表写操作的同步锁。
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        /// 添加键控服务描述符。
        /// 相同服务类型和键的注册按添加顺序保存，解析时最后一个注册生效。
        /// </summary>
        /// <param name="descriptor">要添加的键控服务描述符</param>
        public void Add(KeyedServiceDescriptor descriptor)
        {
            lock (_sync)
            {
                var identifier = new ServiceIdentifier(descriptor.ServiceType, descriptor.ServiceKey);
                if (!_descriptors.TryGetValue(identifier, out var descriptors))
                {
                    descriptors = new List<KeyedServiceDescriptor>();
                    _descriptors.Add(identifier, descriptors);
                }

                descriptors.Add(descriptor);
                _orderedDescriptors.Add(descriptor);
            }
        }

        /// <summary>
        /// 若相同服务类型和键尚未注册，则添加键控服务描述符。
        /// </summary>
        /// <param name="descriptor">要尝试添加的键控服务描述符</param>
        /// <returns>如果添加成功，则返回 true；否则返回 false</returns>
        public bool TryAdd(KeyedServiceDescriptor descriptor)
        {
            lock (_sync)
            {
                if (Contains(descriptor.ServiceType, descriptor.ServiceKey))
                {
                    return false;
                }

                Add(descriptor);
                return true;
            }
        }

        /// <summary>
        /// 移除指定服务类型和键的所有键控注册。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public void RemoveAll(Type serviceType, object serviceKey)
        {
            lock (_sync)
            {
                _descriptors.Remove(new ServiceIdentifier(serviceType, serviceKey));
            }
        }

        /// <summary>
        /// 获取指定服务类型和键的所有键控描述符。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>匹配的键控描述符只读列表，按注册顺序排列</returns>
        public IReadOnlyList<KeyedServiceDescriptor> Get(Type serviceType, object serviceKey)
        {
            lock (_sync)
            {
                return _descriptors.TryGetValue(new ServiceIdentifier(serviceType, serviceKey), out var descriptors)
                    ? descriptors
                    : Array.Empty<KeyedServiceDescriptor>();
            }
        }

        /// <summary>
        /// 获取注册表中的全部键控描述符。
        /// </summary>
        /// <returns>全部键控描述符的只读列表</returns>
        public IReadOnlyList<KeyedServiceDescriptor> GetAll()
        {
            lock (_sync)
            {
                return _orderedDescriptors;
            }
        }

        /// <summary>
        /// 判断指定服务类型和键是否已注册。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>如果已注册，则返回 true；否则返回 false</returns>
        public bool Contains(Type serviceType, object serviceKey)
        {
            lock (_sync)
            {
                return _descriptors.ContainsKey(new ServiceIdentifier(serviceType, serviceKey));
            }
        }
    }
}
#endif
