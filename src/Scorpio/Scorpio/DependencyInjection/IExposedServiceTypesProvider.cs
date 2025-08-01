using System;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 暴露服务类型提供者接口，用于提供类型应该暴露的服务类型和生命周期信息
    /// </summary>
    public interface IExposedServiceTypesProvider
    {
        /// <summary>
        /// 获取目标类型要暴露的服务类型集合
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <returns>返回要暴露的服务类型数组 <see cref="Type"/>[]</returns>
        Type[] GetExposedServiceTypes(Type targetType);

        /// <summary>
        /// 获取服务的生命周期
        /// </summary>
        /// <value>服务生命周期 <see cref="ServiceLifetime"/></value>
        ServiceLifetime ServiceLifetime { get; }
    }
}
