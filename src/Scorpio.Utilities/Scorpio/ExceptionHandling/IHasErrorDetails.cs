namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 定义具有错误详细信息的异常接口
    /// </summary>
    /// <remarks>
    /// 此接口用于标识那些包含详细错误描述信息的异常类型。
    /// 实现此接口的异常类可以提供更详细的错误说明，便于调试和用户理解错误原因。
    /// </remarks>
    /// <seealso cref="IHasErrorCode"/>
    public interface IHasErrorDetails
    {
        /// <summary>
        /// 获取异常的详细错误信息
        /// </summary>
        /// <value>
        /// 包含错误详细描述的字符串，提供比异常消息更丰富的错误信息
        /// </value>
        /// <remarks>
        /// 错误详细信息应该包含足够的上下文信息，帮助开发者或用户理解错误的具体原因。
        /// 可以包含技术细节、建议的解决方案或相关的业务规则说明。
        /// </remarks>
        string Details { get; }
    }
}