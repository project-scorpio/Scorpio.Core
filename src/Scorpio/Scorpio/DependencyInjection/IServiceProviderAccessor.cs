using System;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 服务提供者访问器接口，用于获取当前的服务提供者实例
    /// </summary>
    public interface IServiceProviderAccessor
    {
        /// <summary>
        /// 获取当前的服务提供者实例
        /// </summary>
        /// <value>服务提供者实例 <see cref="IServiceProvider"/></value>
        IServiceProvider ServiceProvider { get; }
    }
}
