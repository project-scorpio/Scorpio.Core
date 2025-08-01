using System;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio
{
    /// <summary>
    /// 定义服务工厂适配器的内部接口。
    /// 提供抽象层以支持不同的依赖注入容器实现，
    /// 允许在 Microsoft.Extensions.DependencyInjection 和第三方容器之间进行适配。
    /// </summary>
    internal interface IServiceFactoryAdapter
    {
        /// <summary>
        /// 使用指定的服务集合创建容器构建器。
        /// 将 <see cref="IServiceCollection"/> 转换为特定容器的构建器对象。
        /// </summary>
        /// <param name="services">
        /// 包含所有已注册服务的 <see cref="IServiceCollection"/> 实例
        /// </param>
        /// <returns>
        /// 特定依赖注入容器的构建器对象，
        /// 具体类型取决于使用的容器实现（如 Autofac、Castle Windsor 等）
        /// </returns>
        object CreateBuilder(IServiceCollection services);

        /// <summary>
        /// 使用容器构建器创建服务提供者。
        /// 将容器构建器转换为可用于服务解析的 <see cref="IServiceProvider"/> 实例。
        /// </summary>
        /// <param name="containerBuilder">
        /// 由 <see cref="CreateBuilder(IServiceCollection)"/> 方法创建的容器构建器对象
        /// </param>
        /// <returns>
        /// 配置完成的 <see cref="IServiceProvider"/> 实例，
        /// 用于运行时的服务解析和依赖注入
        /// </returns>
        IServiceProvider CreateServiceProvider(object containerBuilder);
    }
}
