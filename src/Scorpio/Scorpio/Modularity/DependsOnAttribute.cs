using System;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 用于定义类型依赖关系的特性类
    /// </summary>
    /// <remarks>
    /// 该特性实现了 <see cref="IDependedTypesProvider"/> 接口，用于标记模块或类型的依赖关系。
    /// 通过此特性，可以明确指定当前类型所依赖的其他类型，确保依赖项在当前类型之前被加载和初始化。
    /// 支持多重应用，可以为同一个类型定义多个依赖关系。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute, IDependedTypesProvider
    {
        /// <summary>
        /// 获取当前类型依赖的类型数组
        /// </summary>
        /// <value>
        /// 包含所有依赖类型的数组。这些类型在当前类型加载或初始化之前必须先被处理。
        /// </value>
        public Type[] DependedTypes { get; }

        /// <summary>
        /// 初始化 <see cref="DependsOnAttribute"/> 类的新实例
        /// </summary>
        /// <param name="dependedTypes">当前类型所依赖的类型数组</param>
        /// <remarks>
        /// 如果传入的 <paramref name="dependedTypes"/> 为 null，则内部将使用空数组以避免空引用异常。
        /// 该构造函数使用 params 修饰符，支持传入可变数量的依赖类型参数。
        /// </remarks>
        public DependsOnAttribute(params Type[] dependedTypes) => DependedTypes = dependedTypes ?? new Type[0];

        /// <summary>
        /// 获取当前类型依赖的所有类型
        /// </summary>
        /// <returns>包含所有依赖类型的数组</returns>
        /// <remarks>
        /// 该方法实现了 <see cref="IDependedTypesProvider.GetDependedTypes"/> 接口方法，
        /// 直接返回构造时提供的依赖类型数组。派生类可以重写此方法以提供动态的依赖关系逻辑。
        /// </remarks>
        public virtual Type[] GetDependedTypes() => DependedTypes;
    }

#if NET7_0_OR_GREATER

    /// <summary>
    /// 泛型版本的依赖关系特性类，提供类型安全的依赖声明
    /// </summary>
    /// <typeparam name="TModule">依赖的模块类型，必须实现 <see cref="IScorpioModule"/> 接口</typeparam>
    /// <remarks>
    /// 该类是 <see cref="DependsOnAttribute"/> 的泛型版本，通过泛型约束确保只能依赖有效的 Scorpio 模块类型。
    /// 相比于基类，这个版本提供了编译时类型检查，避免了运行时的类型错误。
    /// 仅在 .NET 7.0 及更高版本中可用。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DependsOnAttribute<TModule> : DependsOnAttribute
        where TModule : IScorpioModule
    {
        /// <summary>
        /// 初始化 <see cref="DependsOnAttribute{TModule}"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 该构造函数自动将泛型类型参数 <typeparamref name="TModule"/> 传递给基类构造函数，
        /// 创建对指定模块类型的依赖关系。通过泛型约束确保依赖的类型是有效的 Scorpio 模块。
        /// </remarks>
        public DependsOnAttribute() : base(typeof(TModule))
        {
            // 无需额外初始化逻辑，基类构造函数已处理类型注册
        }
    }

#endif

}
