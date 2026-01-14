using System.Collections.Generic;

namespace Scorpio.Options
{
    /// <summary>
    /// 为 <see cref="ExtensibleOptions"/> 提供扩展方法的静态类
    /// </summary>
    /// <remarks>
    /// 该类包含用于简化可扩展选项操作的扩展方法，提供类型安全的方式来获取和设置扩展配置项。
    /// 这些方法隐藏了底层字典操作的复杂性，为开发者提供更直观和易用的 API。
    /// </remarks>
    public static class ExtensibleOptionsExtensions
    {

        /// <summary>
        /// 从可扩展选项中获取指定名称的配置项
        /// </summary>
        /// <typeparam name="TOption">配置项的类型</typeparam>
        /// <param name="options">可扩展选项实例</param>
        /// <param name="name">配置项的名称</param>
        /// <returns>
        /// 指定类型的配置项值。如果配置项不存在，则返回 <typeparamref name="TOption"/> 类型的默认值。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法使用 <see cref="CollectionExtensions.GetOrAdd{T}(ICollection{T}, System.Func{T, bool}, System.Func{T})"/> 
        /// 扩展方法来获取或添加配置项。如果指定名称的配置项不存在，会自动创建一个默认值并添加到字典中。
        /// </para>
        /// <para>
        /// 返回值会被强制转换为 <typeparamref name="TOption"/> 类型，因此调用者需要确保类型匹配，
        /// 否则可能引发 <see cref="System.InvalidCastException"/> 异常。
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">当 <paramref name="options"/> 或 <paramref name="name"/> 为 null 时抛出</exception>
        /// <exception cref="System.InvalidCastException">当存储的值无法转换为 <typeparamref name="TOption"/> 类型时抛出</exception>
        public static TOption GetOption<TOption>(this ExtensibleOptions options, string name) =>
            (TOption)options.ExtendedOption.GetOrAdd(name, () => default(TOption));

        /// <summary>
        /// 向可扩展选项中设置指定名称的配置项
        /// </summary>
        /// <typeparam name="TOption">配置项的类型</typeparam>
        /// <param name="options">可扩展选项实例</param>
        /// <param name="name">配置项的名称</param>
        /// <param name="option">要设置的配置项值</param>
        /// <remarks>
        /// <para>
        /// 该方法直接向扩展选项字典中设置指定名称的配置项。如果配置项已存在，则会覆盖原有值。
        /// </para>
        /// <para>
        /// 配置项名称建议使用有意义且唯一的字符串，避免与其他组件的配置项发生命名冲突。
        /// 可以考虑使用命名空间或前缀来确保唯一性。
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">当 <paramref name="options"/> 或 <paramref name="name"/> 为 null 时抛出</exception>
        public static void SetOption<TOption>(this ExtensibleOptions options, string name, TOption option) =>
            options.ExtendedOption[name] = option;
    }
}
