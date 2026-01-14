using System;
using System.Reflection;

using Scorpio.Conventional;
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 依赖注入服务集合扩展类，提供基于约定的自动服务注册功能
    /// 通过预定义的注册器（<see cref="IConventionalRegistrar"/>）自动扫描和注册程序集中的类型
    /// </summary>
    public static class DependencyInjectionServiceCollectionExtensions
    {
        /// <summary>
        /// 向服务集合添加约定注册器实例
        /// 注册器用于定义如何自动注册程序集中的类型到依赖注入容器
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="registrar">约定注册器实例，包含具体的类型注册逻辑</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        public static IServiceCollection AddConventionalRegistrar(this IServiceCollection services, IConventionalRegistrar registrar)
        {
            // 获取或创建注册器列表，并将新的注册器添加到列表中
            GetOrCreateRegistrarList(services).Add(registrar);
            return services;
        }

        /// <summary>
        /// 通过泛型方式添加约定注册器
        /// 自动创建指定类型的注册器实例并添加到服务集合中
        /// </summary>
        /// <typeparam name="T">注册器类型，必须实现 <see cref="IConventionalRegistrar"/> 接口且具有无参构造函数</typeparam>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <exception cref="MissingMethodException">当类型 <typeparamref name="T"/> 没有公共无参构造函数时抛出</exception>
        public static IServiceCollection AddConventionalRegistrar<T>(this IServiceCollection services)
          where T : IConventionalRegistrar =>
            // 使用 Activator.CreateInstance 创建注册器实例，要求 T 类型必须有公共无参构造函数
            services.AddConventionalRegistrar(Activator.CreateInstance<T>());

        /// <summary>
        /// 使用约定方式注册指定程序集中的所有符合条件的类型
        /// 遍历所有已注册的约定注册器，让它们根据各自的规则来注册类型
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="assembly">要扫描和注册类型的目标程序集</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="ConventionalRegistrationContext"/>
        /// <seealso cref="IConventionalRegistrar.Register"/>
        public static IServiceCollection RegisterAssemblyByConvention(this IServiceCollection services, Assembly assembly)
        {
            // 创建约定注册上下文，包含程序集和服务集合信息
            var context = new ConventionalRegistrationContext(assembly, services);

            // 获取所有注册器并依次执行注册逻辑
            GetOrCreateRegistrarList(services).ForEach(registrar => registrar.Register(context));
            return services;
        }

        /// <summary>
        /// 注册调用方程序集中的所有符合约定的类型
        /// 自动获取调用此方法的代码所在的程序集进行类型注册
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <remarks>
        /// 注意：在某些反射调用场景下，<see cref="Assembly.GetCallingAssembly()"/> 可能不会返回预期的程序集
        /// </remarks>
        /// <seealso cref="Assembly.GetCallingAssembly"/>
        /// <seealso cref="RegisterAssemblyByConvention(IServiceCollection, Assembly)"/>
        public static IServiceCollection RegisterAssemblyByConvention(this IServiceCollection services)
        {
            // 获取调用此方法的代码所在的程序集
            var assembly = Assembly.GetCallingAssembly();
            // 委托给重载方法处理实际的注册逻辑
            return RegisterAssemblyByConvention(services, assembly);
        }

        /// <summary>
        /// 通过指定类型注册该类型所在程序集中的所有符合约定的类型
        /// 这是一种便捷的方式来确定目标程序集，避免直接操作 <see cref="Assembly"/> 对象
        /// </summary>
        /// <typeparam name="T">用于确定目标程序集的类型标记</typeparam>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <returns>返回 <see cref="IServiceCollection"/>，支持链式调用</returns>
        /// <seealso cref="RegisterAssemblyByConvention(IServiceCollection, Assembly)"/>
        public static IServiceCollection RegisterAssemblyByConventionOfType<T>(this IServiceCollection services) =>
            // 通过类型 T 获取其所在的程序集，然后进行约定注册
            services.RegisterAssemblyByConvention(typeof(T).GetTypeInfo().Assembly);

        /// <summary>
        /// 获取或创建约定注册器列表
        /// 如果服务集合中已存在注册器列表则返回现有实例，否则创建新的列表并注册为单例
        /// 确保整个应用程序中约定注册器列表的唯一性和可复用性
        /// </summary>
        /// <param name="services">服务集合，用于存储和获取注册器列表</param>
        /// <returns>约定注册器列表，包含所有已注册的约定注册器</returns>
        /// <seealso cref="ConventionalRegistrarList"/>
        private static ConventionalRegistrarList GetOrCreateRegistrarList(IServiceCollection services) =>
            // 使用扩展方法获取单例实例，如果不存在则通过工厂方法创建新实例
            services.GetSingletonInstanceOrAdd(s => new ConventionalRegistrarList());
    }
}
