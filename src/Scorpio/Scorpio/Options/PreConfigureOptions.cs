using System;
namespace Scorpio.Options
{
    /// <summary>
    /// <see cref="IPreConfigureOptions{TOptions}"/> 接口的实现。
    /// 用于在选项配置之前执行预配置操作，不依赖任何外部依赖项。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    public class PreConfigureOptions<TOptions> : IPreConfigureOptions<TOptions> where TOptions : class
    {
        /// <summary>
        /// 使用指定的名称和配置操作初始化 <see cref="PreConfigureOptions{TOptions}"/> 类的新实例。
        /// </summary>
        /// <param name="name">选项配置的名称标识符，null 表示应用于所有命名选项</param>
        /// <param name="action">用于预配置选项的操作委托</param>
        internal PreConfigureOptions(string name, Action<TOptions> action)
        {
            Name = name;
            Action = action;
        }

        /// <summary>
        /// 获取选项配置的名称标识符。
        /// 如果为 null，则表示此预配置操作应用于所有命名选项。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取用于预配置选项的操作委托。
        /// </summary>
        public Action<TOptions> Action { get; }

        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 只有当名称匹配或 <see cref="Name"/> 为 null 时才会执行配置操作。
        /// </summary>
        /// <param name="name">要配置的选项实例的名称标识符</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public virtual void PreConfigure(string name, TOptions options)
        {
            Check.NotNull(options, nameof(options));

            // 空名称用于初始化所有命名选项。
            if (Name == null || name == Name)
            {
                Action?.Invoke(options);
            }
        }

        /// <summary>
        /// 使用 <see cref="Microsoft.Extensions.Options.Options.DefaultName"/> 对选项实例进行预配置。
        /// </summary>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public void PreConfigure(TOptions options) => PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, options);
    }

    /// <summary>
    /// <see cref="IPreConfigureOptions{TOptions}"/> 接口的实现。
    /// 用于在选项配置之前执行预配置操作，依赖一个外部依赖项。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep">预配置操作所依赖的依赖项类型，必须是引用类型</typeparam>
    public class PreConfigureOptions<TOptions, TDep> : IPreConfigureOptions<TOptions>
        where TOptions : class
        where TDep : class
    {
        /// <summary>
        /// 使用指定的名称、依赖项和配置操作初始化 <see cref="PreConfigureOptions{TOptions, TDep}"/> 类的新实例。
        /// </summary>
        /// <param name="name">选项配置的名称标识符</param>
        /// <param name="dependency">预配置操作所需的依赖项</param>
        /// <param name="action">用于预配置选项的操作委托</param>
        internal PreConfigureOptions(string name, TDep dependency, Action<TOptions, TDep> action)
        {
            Name = name;
            Action = action;
            Dependency = dependency;
        }

        /// <summary>
        /// 获取选项配置的名称标识符。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取用于预配置选项的操作委托。
        /// </summary>
        public Action<TOptions, TDep> Action { get; }

        /// <summary>
        /// 获取预配置操作所需的依赖项。
        /// </summary>
        public TDep Dependency { get; }

        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 只有当名称匹配或 <see cref="Name"/> 为 null 时才会执行配置操作。
        /// </summary>
        /// <param name="name">要配置的选项实例的名称标识符</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public virtual void PreConfigure(string name, TOptions options)
        {
            Check.NotNull(options, nameof(options));

            // 空名称用于配置所有命名选项。
            if (Name == null || name == Name)
            {
                Action?.Invoke(options, Dependency);
            }
        }

        /// <summary>
        /// 使用 <see cref="Microsoft.Extensions.Options.Options.DefaultName"/> 对选项实例进行预配置。
        /// </summary>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public void PreConfigure(TOptions options) => PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, options);
    }

    /// <summary>
    /// <see cref="IPreConfigureOptions{TOptions}"/> 接口的实现。
    /// 用于在选项配置之前执行预配置操作，依赖两个外部依赖项。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep1">预配置操作所依赖的第一个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep2">预配置操作所依赖的第二个依赖项类型，必须是引用类型</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public class PreConfigureOptions<TOptions, TDep1, TDep2> : IPreConfigureOptions<TOptions>
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
    {
        /// <summary>
        /// 使用指定的名称、依赖项和配置操作初始化 <see cref="PreConfigureOptions{TOptions, TDep1, TDep2}"/> 类的新实例。
        /// </summary>
        /// <param name="name">选项配置的名称标识符</param>
        /// <param name="dependency">预配置操作所需的第一个依赖项</param>
        /// <param name="dependency2">预配置操作所需的第二个依赖项</param>
        /// <param name="action">用于预配置选项的操作委托</param>
        internal PreConfigureOptions(string name, TDep1 dependency, TDep2 dependency2, Action<TOptions, TDep1, TDep2> action)
        {
            Name = name;
            Action = action;
            Dependency1 = dependency;
            Dependency2 = dependency2;
        }

        /// <summary>
        /// 获取选项配置的名称标识符。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取用于预配置选项的操作委托。
        /// </summary>
        public Action<TOptions, TDep1, TDep2> Action { get; }

        /// <summary>
        /// 获取预配置操作所需的第一个依赖项。
        /// </summary>
        public TDep1 Dependency1 { get; }

        /// <summary>
        /// 获取预配置操作所需的第二个依赖项。
        /// </summary>
        public TDep2 Dependency2 { get; }

        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 只有当名称匹配或 <see cref="Name"/> 为 null 时才会执行配置操作。
        /// </summary>
        /// <param name="name">要配置的选项实例的名称标识符</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public virtual void PreConfigure(string name, TOptions options)
        {
            Check.NotNull(options, nameof(options));
            // 空名称用于配置所有命名选项。
            if (Name == null || name == Name)
            {
                Action?.Invoke(options, Dependency1, Dependency2);
            }
        }

        /// <summary>
        /// 使用 <see cref="Microsoft.Extensions.Options.Options.DefaultName"/> 对选项实例进行预配置。
        /// </summary>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public void PreConfigure(TOptions options) => PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, options);
    }

    /// <summary>
    /// <see cref="IPreConfigureOptions{TOptions}"/> 接口的实现。
    /// 用于在选项配置之前执行预配置操作，依赖三个外部依赖项。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep1">预配置操作所依赖的第一个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep2">预配置操作所依赖的第二个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep3">预配置操作所依赖的第三个依赖项类型，必须是引用类型</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public class PreConfigureOptions<TOptions, TDep1, TDep2, TDep3> : IPreConfigureOptions<TOptions>
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
        where TDep3 : class
    {
        /// <summary>
        /// 使用指定的名称、依赖项和配置操作初始化 <see cref="PreConfigureOptions{TOptions, TDep1, TDep2, TDep3}"/> 类的新实例。
        /// </summary>
        /// <param name="name">选项配置的名称标识符</param>
        /// <param name="dependency">预配置操作所需的第一个依赖项</param>
        /// <param name="dependency2">预配置操作所需的第二个依赖项</param>
        /// <param name="dependency3">预配置操作所需的第三个依赖项</param>
        /// <param name="action">用于预配置选项的操作委托</param>
        internal PreConfigureOptions(string name, TDep1 dependency, TDep2 dependency2, TDep3 dependency3, Action<TOptions, TDep1, TDep2, TDep3> action)
        {
            Name = name;
            Action = action;
            Dependency1 = dependency;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
        }

        /// <summary>
        /// 获取选项配置的名称标识符。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取用于预配置选项的操作委托。
        /// </summary>
        public Action<TOptions, TDep1, TDep2, TDep3> Action { get; }

        /// <summary>
        /// 获取预配置操作所需的第一个依赖项。
        /// </summary>
        public TDep1 Dependency1 { get; }

        /// <summary>
        /// 获取预配置操作所需的第二个依赖项。
        /// </summary>
        public TDep2 Dependency2 { get; }

        /// <summary>
        /// 获取预配置操作所需的第三个依赖项。
        /// </summary>
        public TDep3 Dependency3 { get; }

        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 只有当名称匹配或 <see cref="Name"/> 为 null 时才会执行配置操作。
        /// </summary>
        /// <param name="name">要配置的选项实例的名称标识符</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public virtual void PreConfigure(string name, TOptions options)
        {
            Check.NotNull(options, nameof(options));

            // 空名称用于配置所有命名选项。
            if (Name == null || name == Name)
            {
                Action?.Invoke(options, Dependency1, Dependency2, Dependency3);
            }
        }

        /// <summary>
        /// 使用 <see cref="Microsoft.Extensions.Options.Options.DefaultName"/> 对选项实例进行预配置。
        /// </summary>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public void PreConfigure(TOptions options) => PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, options);
    }

    /// <summary>
    /// <see cref="IPreConfigureOptions{TOptions}"/> 接口的实现。
    /// 用于在选项配置之前执行预配置操作，依赖四个外部依赖项。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep1">预配置操作所依赖的第一个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep2">预配置操作所依赖的第二个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep3">预配置操作所依赖的第三个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep4">预配置操作所依赖的第四个依赖项类型，必须是引用类型</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public class PreConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4> : IPreConfigureOptions<TOptions>
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
        where TDep3 : class
        where TDep4 : class
    {
        /// <summary>
        /// 使用指定的名称、依赖项和配置操作初始化 <see cref="PreConfigureOptions{TOptions, TDep1, TDep2, TDep3, TDep4}"/> 类的新实例。
        /// </summary>
        /// <param name="name">选项配置的名称标识符</param>
        /// <param name="dependency1">预配置操作所需的第一个依赖项</param>
        /// <param name="dependency2">预配置操作所需的第二个依赖项</param>
        /// <param name="dependency3">预配置操作所需的第三个依赖项</param>
        /// <param name="dependency4">预配置操作所需的第四个依赖项</param>
        /// <param name="action">用于预配置选项的操作委托</param>
        internal PreConfigureOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, TDep4 dependency4, Action<TOptions, TDep1, TDep2, TDep3, TDep4> action)
        {
            Name = name;
            Action = action;
            Dependency1 = dependency1;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
            Dependency4 = dependency4;
        }

        /// <summary>
        /// 获取选项配置的名称标识符。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取用于预配置选项的操作委托。
        /// </summary>
        public Action<TOptions, TDep1, TDep2, TDep3, TDep4> Action { get; }

        /// <summary>
        /// 获取预配置操作所需的第一个依赖项。
        /// </summary>
        public TDep1 Dependency1 { get; }

        /// <summary>
        /// 获取预配置操作所需的第二个依赖项。
        /// </summary>
        public TDep2 Dependency2 { get; }

        /// <summary>
        /// 获取预配置操作所需的第三个依赖项。
        /// </summary>
        public TDep3 Dependency3 { get; }

        /// <summary>
        /// 获取预配置操作所需的第四个依赖项。
        /// </summary>
        public TDep4 Dependency4 { get; }

        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 只有当名称匹配或 <see cref="Name"/> 为 null 时才会执行配置操作。
        /// </summary>
        /// <param name="name">要配置的选项实例的名称标识符</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public virtual void PreConfigure(string name, TOptions options)
        {
            Check.NotNull(options, nameof(options));
            // 空名称用于配置所有命名选项。
            if (Name == null || name == Name)
            {
                Action?.Invoke(options, Dependency1, Dependency2, Dependency3, Dependency4);
            }
        }

        /// <summary>
        /// 使用 <see cref="Microsoft.Extensions.Options.Options.DefaultName"/> 对选项实例进行预配置。
        /// </summary>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public void PreConfigure(TOptions options) => PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, options);
    }


    /// <summary>
    /// <see cref="IPreConfigureOptions{TOptions}"/> 接口的实现。
    /// 用于在选项配置之前执行预配置操作，依赖五个外部依赖项。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep1">预配置操作所依赖的第一个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep2">预配置操作所依赖的第二个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep3">预配置操作所依赖的第三个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep4">预配置操作所依赖的第四个依赖项类型，必须是引用类型</typeparam>
    /// <typeparam name="TDep5">预配置操作所依赖的第五个依赖项类型，必须是引用类型</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public class PreConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> : IPreConfigureOptions<TOptions>
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
        where TDep3 : class
        where TDep4 : class
        where TDep5 : class
    {
        /// <summary>
        /// 使用指定的名称、依赖项和配置操作初始化 <see cref="PreConfigureOptions{TOptions, TDep1, TDep2, TDep3, TDep4, TDep5}"/> 类的新实例。
        /// </summary>
        /// <param name="name">选项配置的名称标识符</param>
        /// <param name="dependency1">预配置操作所需的第一个依赖项</param>
        /// <param name="dependency2">预配置操作所需的第二个依赖项</param>
        /// <param name="dependency3">预配置操作所需的第三个依赖项</param>
        /// <param name="dependency4">预配置操作所需的第四个依赖项</param>
        /// <param name="dependency5">预配置操作所需的第五个依赖项</param>
        /// <param name="action">用于预配置选项的操作委托</param>
        internal PreConfigureOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, TDep4 dependency4, TDep5 dependency5, Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> action)
        {
            Name = name;
            Action = action;
            Dependency1 = dependency1;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
            Dependency4 = dependency4;
            Dependency5 = dependency5;
        }

        /// <summary>
        /// 获取选项配置的名称标识符。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取用于预配置选项的操作委托。
        /// </summary>
        public Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> Action { get; }

        /// <summary>
        /// 获取预配置操作所需的第一个依赖项。
        /// </summary>
        public TDep1 Dependency1 { get; }

        /// <summary>
        /// 获取预配置操作所需的第二个依赖项。
        /// </summary>
        public TDep2 Dependency2 { get; }

        /// <summary>
        /// 获取预配置操作所需的第三个依赖项。
        /// </summary>
        public TDep3 Dependency3 { get; }

        /// <summary>
        /// 获取预配置操作所需的第四个依赖项。
        /// </summary>
        public TDep4 Dependency4 { get; }

        /// <summary>
        /// 获取预配置操作所需的第五个依赖项。
        /// </summary>
        public TDep5 Dependency5 { get; }

        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 只有当名称匹配或 <see cref="Name"/> 为 null 时才会执行配置操作。
        /// </summary>
        /// <param name="name">要配置的选项实例的名称标识符</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public virtual void PreConfigure(string name, TOptions options)
        {
            Check.NotNull(options, nameof(options));

            // 空名称用于配置所有命名选项。
            if (Name == null || name == Name)
            {
                Action?.Invoke(options, Dependency1, Dependency2, Dependency3, Dependency4, Dependency5);
            }
        }

        /// <summary>
        /// 使用 <see cref="Microsoft.Extensions.Options.Options.DefaultName"/> 对选项实例进行预配置。
        /// </summary>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        public void PreConfigure(TOptions options) => PreConfigure(Microsoft.Extensions.Options.Options.DefaultName, options);
    }

}
