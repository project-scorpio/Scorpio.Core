using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 为 <see cref="ConfigureServicesContext"/> 类提供扩展方法的静态类
    /// </summary>
    /// <remarks>
    /// 该类包含用于增强服务配置上下文功能的扩展方法，主要提供约定式服务注册相关的功能，
    /// 支持添加约定注册器和按约定注册程序集中的服务。
    /// </remarks>
    public static class ConfigureServicesContextExtensions
    {
        /// <summary>
        /// 向服务集合添加指定类型的约定注册器
        /// </summary>
        /// <typeparam name="T">约定注册器的类型，必须实现 <see cref="IConventionalRegistrar"/> 接口</typeparam>
        /// <param name="context">服务配置上下文实例</param>
        /// <returns>返回当前的 <paramref name="context"/> 实例，支持链式调用</returns>
        /// <remarks>
        /// 该方法为泛型约定注册器添加提供便捷方式，内部调用 <see cref="IServiceCollection"/> 
        /// 的相应扩展方法来注册约定注册器。约定注册器将用于 
        /// <see cref="RegisterAssemblyByConvention(ConfigureServicesContext, Assembly)"/> 方法的自动服务注册。
        /// </remarks>
        public static ConfigureServicesContext AddConventionalRegistrar<T>(this ConfigureServicesContext context)
            where T : IConventionalRegistrar
        {
            // 向服务集合添加泛型约定注册器
            context.Services.AddConventionalRegistrar<T>();
            return context;
        }

        /// <summary>
        /// 向服务集合添加约定注册器实例
        /// </summary>
        /// <param name="context">服务配置上下文实例</param>
        /// <param name="registrar">要添加的约定注册器实例</param>
        /// <returns>返回当前的 <paramref name="context"/> 实例，支持链式调用</returns>
        /// <remarks>
        /// 该方法允许直接添加约定注册器的实例，适用于需要自定义初始化逻辑的场景。
        /// 注册器将用于 <see cref="RegisterAssemblyByConvention(ConfigureServicesContext, Assembly)"/> 
        /// 方法的自动服务注册。
        /// </remarks>
        public static ConfigureServicesContext AddConventionalRegistrar(this ConfigureServicesContext context, IConventionalRegistrar registrar)
        {
            // 向服务集合添加约定注册器实例
            context.Services.AddConventionalRegistrar(registrar);
            return context;
        }

        /// <summary>
        /// 按约定注册调用方程序集中的服务
        /// </summary>
        /// <param name="context">服务配置上下文实例</param>
        /// <returns>返回当前的 <paramref name="context"/> 实例，支持链式调用</returns>
        /// <remarks>
        /// 该方法会自动获取调用方程序集，并使用已注册的约定注册器对程序集中的类型进行自动服务注册。
        /// 这是一个便捷方法，适用于模块需要注册自身程序集中服务的场景。
        /// </remarks>
        public static ConfigureServicesContext RegisterAssemblyByConvention(this ConfigureServicesContext context)
        {
            // 获取调用方程序集
            var assembly = Assembly.GetCallingAssembly();
            // 按约定注册程序集中的服务
            context.Services.RegisterAssemblyByConvention(assembly);
            return context;
        }

        /// <summary>
        /// 按约定注册指定程序集中的服务
        /// </summary>
        /// <param name="context">服务配置上下文实例</param>
        /// <param name="assembly">要注册服务的程序集</param>
        /// <returns>返回当前的 <paramref name="context"/> 实例，支持链式调用</returns>
        /// <remarks>
        /// 该方法使用已注册的约定注册器对指定程序集中的类型进行自动服务注册。
        /// 约定注册器会根据预定义的规则（如命名约定、接口实现等）自动识别和注册服务。
        /// </remarks>
        public static ConfigureServicesContext RegisterAssemblyByConvention(this ConfigureServicesContext context, Assembly assembly)
        {
            // 按约定注册指定程序集中的服务
            context.Services.RegisterAssemblyByConvention(assembly);
            return context;
        }

        /// <summary>
        /// 按约定注册包含指定类型的程序集中的服务
        /// </summary>
        /// <typeparam name="T">用于定位程序集的类型标记</typeparam>
        /// <param name="context">服务配置上下文实例</param>
        /// <returns>返回当前的 <paramref name="context"/> 实例，支持链式调用</returns>
        /// <remarks>
        /// 该方法通过泛型类型参数 <typeparamref name="T"/> 来定位其所在的程序集，
        /// 然后使用已注册的约定注册器对该程序集中的类型进行自动服务注册。
        /// 这是一个类型安全的便捷方法，避免了直接操作程序集对象。
        /// </remarks>
        public static ConfigureServicesContext RegisterAssemblyByConventionOfType<T>(this ConfigureServicesContext context)
        {
            // 按约定注册包含指定类型的程序集中的服务
            context.Services.RegisterAssemblyByConventionOfType<T>();
            return context;
        }
    }
}
