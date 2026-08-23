#if !NET8_0_OR_GREATER
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 定义可按类型和键解析服务的服务提供程序接口。
    /// 与 .NET 8 原生 <c>IKeyedServiceProvider</c> 保持一致的契约。
    /// </summary>
    public interface IKeyedServiceProvider : IServiceProvider
    {
        /// <summary>
        /// 按服务类型和键获取服务实例。
        /// </summary>
        /// <param name="serviceType">要获取的服务类型</param>
        /// <param name="serviceKey">要获取的服务的键，可以为 null</param>
        /// <returns>
        /// 指定类型的服务实例；如果未找到对应的键控服务，则返回 null
        /// </returns>
        object GetKeyedService(Type serviceType, object serviceKey);

        /// <summary>
        /// 按服务类型和键获取服务实例，未找到时抛出异常。
        /// </summary>
        /// <param name="serviceType">要获取的服务类型</param>
        /// <param name="serviceKey">要获取的服务的键，可以为 null</param>
        /// <returns>指定类型的服务实例</returns>
        /// <exception cref="InvalidOperationException">当未注册对应的键控服务时抛出</exception>
        object GetRequiredKeyedService(Type serviceType, object serviceKey);
    }
}
#endif
