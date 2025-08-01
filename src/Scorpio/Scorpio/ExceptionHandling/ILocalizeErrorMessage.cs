using Scorpio.Localization;

namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 本地化错误消息接口，定义异常消息本地化的核心契约
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该接口用于为异常处理系统提供多语言支持，允许异常消息根据不同的文化区域
    /// 进行本地化处理。实现此接口的类可以将异常消息转换为用户当前语言环境下
    /// 的本地化文本。
    /// </para>
    /// <para>
    /// 通常与异常处理流程结合使用，在向用户展示错误信息时提供友好的本地化消息，
    /// 提升用户体验和系统的国际化支持能力。
    /// </para>
    /// </remarks>
    /// <seealso cref="Scorpio.Localization.LocalizationContext"/>
    public interface ILocalizeErrorMessage
    {
        /// <summary>
        /// 根据本地化上下文获取本地化的错误消息
        /// </summary>
        /// <param name="context">
        /// 本地化上下文，包含文化区域、资源键和参数等本地化所需的信息。不能为 <c>null</c>
        /// </param>
        /// <returns>
        /// 返回根据指定上下文本地化后的错误消息字符串。如果找不到对应的本地化资源，
        /// 应返回默认消息或原始消息
        /// </returns>
        /// <remarks>
        /// <para>
        /// 实现此方法时应根据 <paramref name="context"/> 中的文化信息和资源键，
        /// 从本地化资源中获取相应的错误消息文本。
        /// </para>
        /// <para>
        /// 实现注意事项：
        /// <list type="bullet">
        /// <item><description>应处理资源缺失的情况，提供合理的回退机制</description></item>
        /// <item><description>支持消息模板中的参数替换功能</description></item>
        /// <item><description>考虑缓存机制以提高本地化性能</description></item>
        /// <item><description>确保返回的消息对用户友好且易于理解</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="Scorpio.Localization.LocalizationContext"/>
        string LocalizeMessage(LocalizationContext context);
    }
}