using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 默认服务作用域工厂的内部实现类，实现 <see cref="IHybridServiceScopeFactory"/> 接口
    /// </summary>
    /// <remarks>
    /// 通过 <see cref="ExposeServicesAttribute"/> 特性暴露为 <see cref="IHybridServiceScopeFactory"/> 和 <see cref="DefaultServiceScopeFactory"/> 服务
    /// </remarks>
    [ExposeServices(
        typeof(IHybridServiceScopeFactory),
        typeof(DefaultServiceScopeFactory)
        )]
    internal class DefaultServiceScopeFactory : IHybridServiceScopeFactory
    {
        /// <summary>
        /// 获取内部使用的服务作用域工厂实例
        /// </summary>
        /// <value>服务作用域工厂 <see cref="IServiceScopeFactory"/></value>
        protected IServiceScopeFactory Factory { get; }

        /// <summary>
        /// 初始化默认服务作用域工厂的新实例
        /// </summary>
        /// <param name="factory">要包装的服务作用域工厂实例</param>
        public DefaultServiceScopeFactory(IServiceScopeFactory factory) => Factory = factory;

        /// <summary>
        /// 创建一个新的服务作用域
        /// </summary>
        /// <returns>返回新创建的服务作用域 <see cref="IServiceScope"/></returns>
        public IServiceScope CreateScope() => Factory.CreateScope();
    }
}
