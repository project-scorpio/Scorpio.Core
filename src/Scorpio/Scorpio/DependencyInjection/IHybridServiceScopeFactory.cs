using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 混合服务作用域工厂接口，继承自 <see cref="IServiceScopeFactory"/>
    /// </summary>
    /// <remarks>
    /// 提供了增强的服务作用域创建功能，用于在不同的上下文中创建服务作用域
    /// </remarks>
    public interface IHybridServiceScopeFactory : IServiceScopeFactory
    {

    }
}
