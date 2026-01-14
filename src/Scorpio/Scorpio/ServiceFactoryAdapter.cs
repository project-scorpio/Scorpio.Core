using System;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio
{
    /// <summary>
    /// 服务工厂适配器的泛型实现类。
    /// 实现 <see cref="IServiceFactoryAdapter"/> 接口，为不同类型的依赖注入容器提供统一的适配器接口。
    /// 支持将标准的 <see cref="IServiceProviderFactory{TContainerBuilder}"/> 包装为框架内部使用的适配器。
    /// </summary>
    /// <typeparam name="TContainerBuilder">
    /// 容器构建器的类型，由具体的依赖注入容器实现定义
    /// （如 Autofac 的 ContainerBuilder、Castle Windsor 的 IWindsorContainer 等）
    /// </typeparam>
    internal class ServiceFactoryAdapter<TContainerBuilder> : IServiceFactoryAdapter
    {
        /// <summary>
        /// 底层的服务提供程序工厂实例。
        /// 负责实际的容器构建器创建和服务提供程序生成工作。
        /// </summary>
        private readonly IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory;

        /// <summary>
        /// 初始化 <see cref="ServiceFactoryAdapter{TContainerBuilder}"/> 类的新实例。
        /// 使用指定的服务提供程序工厂来创建适配器。
        /// </summary>
        /// <param name="serviceProviderFactory">
        /// 服务提供程序工厂实例，不能为 null
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="serviceProviderFactory"/> 为 null 时抛出
        /// </exception>
        public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
            => _serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));

        /// <summary>
        /// 使用指定的服务集合创建容器构建器。
        /// 将 <see cref="IServiceCollection"/> 转换为 <typeparamref name="TContainerBuilder"/> 类型的容器构建器。
        /// </summary>
        /// <param name="services">
        /// 包含所有已注册服务的 <see cref="IServiceCollection"/> 实例
        /// </param>
        /// <returns>
        /// 类型为 <typeparamref name="TContainerBuilder"/> 的容器构建器对象，
        /// 以 <see cref="object"/> 类型返回以满足接口契约
        /// </returns>
        public object CreateBuilder(IServiceCollection services) => _serviceProviderFactory.CreateBuilder(services);

        /// <summary>
        /// 使用容器构建器创建服务提供者。
        /// 将容器构建器转换为可用于服务解析的 <see cref="IServiceProvider"/> 实例。
        /// </summary>
        /// <param name="containerBuilder">
        /// 由 <see cref="CreateBuilder(IServiceCollection)"/> 方法创建的容器构建器对象，
        /// 将被强制转换为 <typeparamref name="TContainerBuilder"/> 类型
        /// </param>
        /// <returns>
        /// 配置完成的 <see cref="IServiceProvider"/> 实例，
        /// 用于运行时的服务解析和依赖注入
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// 当 <paramref name="containerBuilder"/> 无法转换为 <typeparamref name="TContainerBuilder"/> 类型时抛出
        /// </exception>
        public IServiceProvider CreateServiceProvider(object containerBuilder)
            => _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}

