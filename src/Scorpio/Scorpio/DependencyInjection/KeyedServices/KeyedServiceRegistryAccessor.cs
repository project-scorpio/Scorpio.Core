#if !NET8_0_OR_GREATER
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务注册表的附加访问器。
    /// 通过 <see cref="ConditionalWeakTable{TKey, TValue}"/> 将注册表附加到服务集合，
    /// 避免在服务集合中暴露内部对象，也不影响普通容器的解析。
    /// </summary>
    internal static class KeyedServiceRegistryAccessor
    {
        /// <summary>
        /// 存储服务集合与其键控注册表之间映射的弱表。
        /// </summary>
        private static readonly ConditionalWeakTable<IServiceCollection, KeyedServiceRegistry> Registries =
            new ConditionalWeakTable<IServiceCollection, KeyedServiceRegistry>();

        /// <summary>
        /// 获取或创建服务集合对应的键控服务注册表。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>附加到指定服务集合的键控服务注册表</returns>
        public static KeyedServiceRegistry GetOrCreate(IServiceCollection services)
            => Registries.GetValue(services, collection => new KeyedServiceRegistry());
    }
}
#endif
