#if !NET8_0_OR_GREATER
using System;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务的唯一标识结构，由服务类型和服务键组成。
    /// </summary>
    internal readonly struct ServiceIdentifier : IEquatable<ServiceIdentifier>
    {
        /// <summary>
        /// 缓存服务键的对象相等性比较器。
        /// </summary>
        private static readonly KeyedServiceKeyComparer KeyComparer = new KeyedServiceKeyComparer();

        /// <summary>
        /// 初始化 <see cref="ServiceIdentifier"/> 结构的新实例。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        public ServiceIdentifier(Type serviceType, object serviceKey)
        {
            ServiceType = serviceType;
            ServiceKey = serviceKey;
        }

        /// <summary>
        /// 获取服务类型。
        /// </summary>
        /// <value>服务类型</value>
        public Type ServiceType { get; }

        /// <summary>
        /// 获取服务键。
        /// </summary>
        /// <value>服务键，可以为 null</value>
        public object ServiceKey { get; }

        /// <summary>
        /// 判断当前标识是否与指定标识相等。
        /// 服务类型按引用比较，服务键按对象相等性比较。
        /// </summary>
        /// <param name="other">要比较的标识</param>
        /// <returns>如果相等，则返回 true；否则返回 false</returns>
        public bool Equals(ServiceIdentifier other)
            => ServiceType == other.ServiceType && KeyComparer.Equals(ServiceKey, other.ServiceKey);

        /// <summary>
        /// 判断当前标识是否与指定对象相等。
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果相等，则返回 true；否则返回 false</returns>
        public override bool Equals(object obj)
            => obj is ServiceIdentifier other && Equals(other);

        /// <summary>
        /// 返回当前标识的哈希代码。
        /// </summary>
        /// <returns>基于服务类型和服务键计算的哈希代码</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (ServiceType.GetHashCode() * 397) ^ KeyComparer.GetHashCode(ServiceKey);
            }
        }
    }
}
#endif
