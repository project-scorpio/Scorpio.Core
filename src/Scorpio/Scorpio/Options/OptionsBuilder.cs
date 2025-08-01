using System;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Options
{
    /// <summary>
    /// 选项构建器，用于配置和构建特定类型的选项。
    /// 继承自 Microsoft.Extensions.Options.OptionsBuilder，提供了预配置功能的扩展。
    /// </summary>
    /// <typeparam name="TOptions">要配置的选项类型，必须是引用类型</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public class OptionsBuilder<TOptions> : Microsoft.Extensions.Options.OptionsBuilder<TOptions> where TOptions : class
    {
        /// <summary>
        /// 初始化 <see cref="OptionsBuilder{TOptions}"/> 类的新实例。
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖项</param>
        /// <param name="name">选项配置的名称标识符</param>
        public OptionsBuilder(IServiceCollection services, string name) : base(services, name)
        {
        }

        /// <summary>
        /// 注册用于预配置特定类型选项的操作。
        /// 注意：这些操作在所有 <see cref="Microsoft.Extensions.Options.OptionsBuilder{TOptions}.Configure(Action{TOptions})"/> 之前执行。
        /// </summary>
        /// <param name="configureOptions">用于配置选项的操作委托</param>
        /// <returns>当前的 <see cref="OptionsBuilder{TOptions}"/> 实例，支持链式调用</returns>
        public virtual OptionsBuilder<TOptions> PreConfigure(Action<TOptions> configureOptions)
        {
            Check.NotNull(configureOptions, nameof(configureOptions));
            Services.AddSingleton<IPreConfigureOptions<TOptions>>(new PreConfigureOptions<TOptions>(Name, configureOptions));
            return this;
        }

        /// <summary>
        /// 注册用于预配置特定类型选项的操作，该操作需要一个依赖项。
        /// 注意：这些操作在所有 <see cref="Microsoft.Extensions.Options.OptionsBuilder{TOptions}.Configure(Action{TOptions})"/> 之前执行。
        /// </summary>
        /// <typeparam name="TDep">操作所使用的依赖项类型，必须是引用类型</typeparam>
        /// <param name="configureOptions">用于配置选项的操作委托，接受选项实例和依赖项作为参数</param>
        /// <returns>当前的 <see cref="OptionsBuilder{TOptions}"/> 实例，支持链式调用</returns>
        public virtual OptionsBuilder<TOptions> PreConfigure<TDep>(Action<TOptions, TDep> configureOptions)
            where TDep : class
        {
            Check.NotNull(configureOptions, nameof(configureOptions));

            Services.AddTransient<IPreConfigureOptions<TOptions>>(sp =>
                new PreConfigureOptions<TOptions, TDep>(Name, sp.GetRequiredService<TDep>(), configureOptions));
            return this;
        }

        /// <summary>
        /// 注册用于预配置特定类型选项的操作，该操作需要两个依赖项。
        /// 注意：这些操作在所有 <see cref="Microsoft.Extensions.Options.OptionsBuilder{TOptions}.Configure(Action{TOptions})"/> 之前执行。
        /// </summary>
        /// <typeparam name="TDep1">操作所使用的第一个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep2">操作所使用的第二个依赖项类型，必须是引用类型</typeparam>
        /// <param name="configureOptions">用于配置选项的操作委托，接受选项实例和两个依赖项作为参数</param>
        /// <returns>当前的 <see cref="OptionsBuilder{TOptions}"/> 实例，支持链式调用</returns>
        public virtual OptionsBuilder<TOptions> PreConfigure<TDep1, TDep2>(Action<TOptions, TDep1, TDep2> configureOptions)
            where TDep1 : class
            where TDep2 : class
        {
            Check.NotNull(configureOptions, nameof(configureOptions));

            Services.AddTransient<IPreConfigureOptions<TOptions>>(sp =>
                new PreConfigureOptions<TOptions, TDep1, TDep2>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), configureOptions));
            return this;
        }

        /// <summary>
        /// 注册用于预配置特定类型选项的操作，该操作需要三个依赖项。
        /// 注意：这些操作在所有 <see cref="Microsoft.Extensions.Options.OptionsBuilder{TOptions}.Configure(Action{TOptions})"/> 之前执行。
        /// </summary>
        /// <typeparam name="TDep1">操作所使用的第一个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep2">操作所使用的第二个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep3">操作所使用的第三个依赖项类型，必须是引用类型</typeparam>
        /// <param name="configureOptions">用于配置选项的操作委托，接受选项实例和三个依赖项作为参数</param>
        /// <returns>当前的 <see cref="OptionsBuilder{TOptions}"/> 实例，支持链式调用</returns>
        public virtual OptionsBuilder<TOptions> PreConfigure<TDep1, TDep2, TDep3>(Action<TOptions, TDep1, TDep2, TDep3> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
        {
            Check.NotNull(configureOptions, nameof(configureOptions));

            Services.AddTransient<IPreConfigureOptions<TOptions>>(
                sp => new PreConfigureOptions<TOptions, TDep1, TDep2, TDep3>(
                    Name,
                    sp.GetRequiredService<TDep1>(),
                    sp.GetRequiredService<TDep2>(),
                    sp.GetRequiredService<TDep3>(),
                    configureOptions));
            return this;
        }

        /// <summary>
        /// 注册用于预配置特定类型选项的操作，该操作需要四个依赖项。
        /// 注意：这些操作在所有 <see cref="Microsoft.Extensions.Options.OptionsBuilder{TOptions}.Configure(Action{TOptions})"/> 之前执行。
        /// </summary>
        /// <typeparam name="TDep1">操作所使用的第一个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep2">操作所使用的第二个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep3">操作所使用的第三个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep4">操作所使用的第四个依赖项类型，必须是引用类型</typeparam>
        /// <param name="configureOptions">用于配置选项的操作委托，接受选项实例和四个依赖项作为参数</param>
        /// <returns>当前的 <see cref="OptionsBuilder{TOptions}"/> 实例，支持链式调用</returns>
        public virtual OptionsBuilder<TOptions> PreConfigure<TDep1, TDep2, TDep3, TDep4>(Action<TOptions, TDep1, TDep2, TDep3, TDep4> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
            where TDep4 : class
        {
            Check.NotNull(configureOptions, nameof(configureOptions));

            Services.AddTransient<IPreConfigureOptions<TOptions>>(
                sp => new PreConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(
                    Name,
                    sp.GetRequiredService<TDep1>(),
                    sp.GetRequiredService<TDep2>(),
                    sp.GetRequiredService<TDep3>(),
                    sp.GetRequiredService<TDep4>(),
                    configureOptions));
            return this;
        }

        /// <summary>
        /// 注册用于预配置特定类型选项的操作，该操作需要五个依赖项。
        /// 注意：这些操作在所有 <see cref="Microsoft.Extensions.Options.OptionsBuilder{TOptions}.Configure(Action{TOptions})"/> 之前执行。
        /// </summary>
        /// <typeparam name="TDep1">操作所使用的第一个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep2">操作所使用的第二个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep3">操作所使用的第三个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep4">操作所使用的第四个依赖项类型，必须是引用类型</typeparam>
        /// <typeparam name="TDep5">操作所使用的第五个依赖项类型，必须是引用类型</typeparam>
        /// <param name="configureOptions">用于配置选项的操作委托，接受选项实例和五个依赖项作为参数</param>
        /// <returns>当前的 <see cref="OptionsBuilder{TOptions}"/> 实例，支持链式调用</returns>
        public virtual OptionsBuilder<TOptions> PreConfigure<TDep1, TDep2, TDep3, TDep4, TDep5>(Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
            where TDep4 : class
            where TDep5 : class
        {
            Check.NotNull(configureOptions, nameof(configureOptions));

            Services.AddTransient<IPreConfigureOptions<TOptions>>(
                sp => new PreConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(
                    Name,
                    sp.GetRequiredService<TDep1>(),
                    sp.GetRequiredService<TDep2>(),
                    sp.GetRequiredService<TDep3>(),
                    sp.GetRequiredService<TDep4>(),
                    sp.GetRequiredService<TDep5>(),
                    configureOptions));
            return this;
        }
    }
}
