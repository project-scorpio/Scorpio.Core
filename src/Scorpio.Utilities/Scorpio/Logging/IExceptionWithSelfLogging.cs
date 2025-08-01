using Microsoft.Extensions.Logging;

namespace Scorpio.Logging
{
    /// <summary>
    /// 定义具有自记录功能的异常接口
    /// </summary>
    /// <remarks>
    /// 此接口用于标识那些可以自行执行日志记录逻辑的异常类型。
    /// 实现此接口的异常类可以定义自己的日志记录行为，包括记录特定的属性、格式化输出或执行复杂的日志逻辑。
    /// 这种设计模式允许异常类封装其特定的日志记录需求，提供更灵活和可扩展的异常处理机制。
    /// </remarks>
    /// <seealso cref="ILogger"/>
    public interface IExceptionWithSelfLogging
    {
        /// <summary>
        /// 执行异常的自定义日志记录逻辑
        /// </summary>
        /// <param name="logger">用于记录日志的日志记录器实例</param>
        /// <remarks>
        /// 实现此方法时，异常类可以：
        /// 1. 记录异常特有的属性和数据
        /// 2. 使用特定的日志格式和级别
        /// 3. 执行复杂的日志记录逻辑
        /// 4. 记录与异常相关的上下文信息
        /// 此方法通常由异常处理框架自动调用，无需手动调用。
        /// </remarks>
        void Log(ILogger logger);
    }
}
