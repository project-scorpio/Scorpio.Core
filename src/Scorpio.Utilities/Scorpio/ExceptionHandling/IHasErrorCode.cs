namespace Scorpio.ExceptionHandling
{
    /// <summary>
    /// 定义具有错误代码的异常接口
    /// </summary>
    /// <remarks>
    /// 此接口用于标识那些包含特定错误代码的异常类型。
    /// 实现此接口的异常类可以提供结构化的错误代码，便于错误分类和处理。
    /// </remarks>
    /// <seealso cref="IHasErrorDetails"/>
    public interface IHasErrorCode
    {
        /// <summary>
        /// 获取异常的错误代码
        /// </summary>
        /// <value>
        /// 表示错误类型或分类的字符串代码，通常用于程序化的错误处理和分类
        /// </value>
        /// <remarks>
        /// 错误代码应该是唯一且有意义的标识符，用于区分不同类型的错误。
        /// 建议使用统一的命名规范，如"ERR_001"、"VALIDATION_FAILED"等格式。
        /// </remarks>
        string Code { get; }
    }
}