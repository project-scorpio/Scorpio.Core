using System;

using Scorpio;
using Scorpio.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Scorpio选项服务集合扩展类，提供配置选项的便捷方法
    /// </summary>
    public static class ScorpioOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// 获取一个选项构建器，将相同类型 <typeparamref name="TOptions"/> 的配置调用转发到底层服务集合
        /// </summary>
        /// <typeparam name="TOptions">要配置的选项类型</typeparam>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/></param>
        /// <returns><see cref="OptionsBuilder{TOptions}"/>，以便可以在其中链式调用配置方法</returns>
        public static OptionsBuilder<TOptions> Options<TOptions>(this IServiceCollection services) where TOptions : class => services.Options<TOptions>(Microsoft.Extensions.Options.Options.DefaultName);

        /// <summary>
        /// 获取一个选项构建器，将相同命名的 <typeparamref name="TOptions"/> 配置调用转发到底层服务集合
        /// </summary>
        /// <typeparam name="TOptions">要配置的选项类型</typeparam>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/></param>
        /// <param name="name">选项实例的名称</param>
        /// <returns><see cref="OptionsBuilder{TOptions}"/>，以便可以在其中链式调用配置方法</returns>
        public static OptionsBuilder<TOptions> Options<TOptions>(this IServiceCollection services, string name)
            where TOptions : class
        {
            Check.NotNull(services, nameof(services));
            services.AddOptions();
            return new OptionsBuilder<TOptions>(services, name);
        }

        /// <summary>
        /// 注册用于初始化特定类型选项的操作
        /// 注意：这些操作在所有 <seealso cref="OptionsServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, Action{TOptions})"/> 之后运行
        /// </summary>
        /// <typeparam name="TOptions">要配置的选项类型</typeparam>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/></param>
        /// <param name="configureOptions">用于配置选项的操作</param>
        /// <returns><see cref="IServiceCollection"/>，以便可以链式调用其他方法</returns>
        public static IServiceCollection PreConfigure<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class => services.PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, configureOptions);

        /// <summary>
        /// 注册用于配置特定类型选项的操作
        /// 注意：这些操作在所有 <seealso cref="OptionsServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, Action{TOptions})"/> 之后运行
        /// </summary>
        /// <typeparam name="TOptions">要配置的选项类型</typeparam>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/></param>
        /// <param name="name">选项实例的名称</param>
        /// <param name="configureOptions">用于配置选项的操作</param>
        /// <returns><see cref="IServiceCollection"/>，以便可以链式调用其他方法</returns>
        public static IServiceCollection PreConfigure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(configureOptions, nameof(configureOptions));
            services.AddOptions();
            services.AddSingleton<IPreConfigureOptions<TOptions>>(new PreConfigureOptions<TOptions>(name, configureOptions));
            return services;
        }

        /// <summary>
        /// 注册用于后置配置特定类型选项所有实例的操作
        /// 注意：这些操作在所有 <seealso cref="OptionsServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, Action{TOptions})"/> 之后运行
        /// </summary>
        /// <typeparam name="TOptions">要配置的选项类型</typeparam>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/></param>
        /// <param name="configureOptions">用于配置选项的操作</param>
        /// <returns><see cref="IServiceCollection"/>，以便可以链式调用其他方法</returns>
        public static IServiceCollection PreConfigureAll<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class => services.PreConfigure(name: null, configureOptions: configureOptions);
    }
}
