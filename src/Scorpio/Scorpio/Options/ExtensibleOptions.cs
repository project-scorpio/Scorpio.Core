using System.Collections.Generic;

namespace Scorpio.Options
{
    /// <summary>
    /// 可扩展选项的抽象基类，为配置选项提供动态扩展功能
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类为选项对象提供了扩展机制，允许在运行时动态添加额外的配置属性。
    /// 这种设计模式特别适用于框架或插件系统，其中基础选项可能需要被第三方组件扩展。
    /// </para>
    /// <para>
    /// 派生类可以继承此基类来获得扩展配置的能力，同时保持类型安全和良好的封装性。
    /// 扩展属性通过键值对的形式存储，支持任意类型的值。
    /// </para>
    /// </remarks>
    public abstract class ExtensibleOptions
    {
        /// <summary>
        /// 获取扩展选项的字典集合，用于存储动态添加的配置属性
        /// </summary>
        /// <value>
        /// 包含扩展配置项的字典，其中键为配置项名称（字符串），值为配置项的值（任意对象类型）。
        /// 该字典在构造函数中初始化，确保始终可用。
        /// </value>
        /// <remarks>
        /// <para>
        /// 此属性使用 <c>protected internal</c> 访问修饰符，允许：
        /// <list type="bullet">
        /// <item><description>派生类访问和操作扩展选项</description></item>
        /// <item><description>同一程序集内的其他类访问扩展选项</description></item>
        /// <item><description>防止外部程序集的直接访问，维护封装性</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 扩展选项的键建议使用有意义的名称，避免命名冲突。
        /// 值可以是任意类型的对象，使用时需要进行适当的类型转换。
        /// </para>
        /// </remarks>
        protected internal IDictionary<string, object> ExtendedOption { get; }

        /// <summary>
        /// 初始化 <see cref="ExtensibleOptions"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// <para>
        /// 构造函数使用表达式体语法初始化 <see cref="ExtendedOption"/> 属性，
        /// 创建一个新的 <see cref="Dictionary{TKey, TValue}"/> 实例来存储扩展配置。
        /// </para>
        /// <para>
        /// 由于这是一个抽象类，此构造函数只能被派生类调用。
        /// 派生类的构造函数会自动调用此基类构造函数来确保扩展选项字典的正确初始化。
        /// </para>
        /// </remarks>
        protected ExtensibleOptions() => ExtendedOption = new Dictionary<string, object>();
    }
}
