using System;
using System.Runtime.Serialization;

namespace Scorpio
{
    /// <summary>
    /// Scorpio 框架的基础异常类。
    /// 继承自 <see cref="Exception"/>，用于表示 Scorpio 框架中发生的各种异常情况。
    /// 提供完整的异常构造函数重载，支持序列化和内部异常链。
    /// </summary>
    [Serializable]
    public class ScorpioException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="ScorpioException"/> 类的新实例。
        /// 创建一个具有默认错误消息的异常实例。
        /// </summary>
        public ScorpioException() : base() { }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="ScorpioException"/> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public ScorpioException(string message) : base(message) { }

        /// <summary>
        /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="ScorpioException"/> 类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息</param>
        /// <param name="innerException">
        /// 导致当前异常的异常；如果未指定内部异常，则为 null 引用（在 Visual Basic 中为 Nothing）
        /// </param>
        public ScorpioException(string message, Exception innerException) : base(message, innerException) { }

#if !NET8_0_OR_GREATER
        /// <summary>
        /// 使用序列化数据初始化 <see cref="ScorpioException"/> 类的新实例。
        /// 此构造函数在反序列化过程中被调用，以重新构造通过流传输的异常对象。
        /// </summary>
        /// <param name="info">
        /// 包含有关所引发异常的序列化对象数据的 <see cref="SerializationInfo"/>
        /// </param>
        /// <param name="context">
        /// 包含有关源或目标的上下文信息的 <see cref="StreamingContext"/>
        /// </param>
        /// <exception cref="ArgumentNullException">当 <paramref name="info"/> 参数为 null 时抛出</exception>
        /// <exception cref="SerializationException">
        /// 当类名为 null 或 <see cref="Exception.HResult"/> 为零 (0) 时抛出
        /// </exception>
        protected ScorpioException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    #endif
    }
}
