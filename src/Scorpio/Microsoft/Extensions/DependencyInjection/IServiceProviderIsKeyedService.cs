#if !NET8_0_OR_GREATER
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 定义查询服务类型与键组合是否已注册的接口。
    /// </summary>
    public interface IServiceProviderIsKeyedService
    {
        /// <summary>
        /// 判断指定服务类型和键的组合是否已注册。
        /// </summary>
        /// <param name="serviceType">要查询的服务类型</param>
        /// <param name="serviceKey">要查询的键，可以为 null</param>
        /// <returns>如果该键控服务已注册，则返回 true；否则返回 false</returns>
        bool IsKeyedService(Type serviceType, object serviceKey);
    }
}
#endif
