using System;
using System.Collections.Generic;

using Scorpio.ExceptionHandling;
using Scorpio.Logging;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// ILogger的扩展方法类，提供增强的异常日志记录功能
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// 记录异常信息，使用异常自身的消息作为日志消息
        /// </summary>
        /// <param name="logger">日志记录器实例</param>
        /// <param name="ex">要记录的异常</param>
        /// <param name="level">可选的日志级别，如果未指定则使用异常的默认日志级别</param>
        /// <seealso cref="LogException(ILogger, Exception, string, LogLevel?)"/>
        public static void LogException(this ILogger logger, Exception ex, LogLevel? level = null) => LogException(logger, ex, ex.Message, level);

        /// <summary>
        /// 记录异常信息，包括异常的详细属性、自定义数据和自记录功能
        /// </summary>
        /// <param name="logger">日志记录器实例</param>
        /// <param name="ex">要记录的异常</param>
        /// <param name="message">自定义的日志消息</param>
        /// <param name="level">可选的日志级别，如果未指定则使用异常的默认日志级别</param>
        /// <remarks>
        /// 此方法会依次执行以下操作：
        /// 1. 记录基本异常信息
        /// 2. 记录异常的已知属性（如错误代码和详细信息）
        /// 3. 处理实现了自记录接口的异常
        /// 4. 记录异常的Data字典内容
        /// </remarks>
        public static void LogException(this ILogger logger, Exception ex, string message, LogLevel? level = null)
        {
            // 确定要使用的日志级别
            var selectedLevel = level ?? ex.GetLogLevel();

            // 记录基本的异常信息
            logger.Log(selectedLevel, ex, message);
            // 记录异常的已知属性
            LogKnownProperties(logger, ex, selectedLevel);
            // 处理自记录异常
            LogSelfLogging(logger, ex);
            // 记录异常的额外数据
            LogData(logger, ex, selectedLevel);
        }

        /// <summary>
        /// 记录异常的已知属性，如错误代码和错误详细信息
        /// </summary>
        /// <param name="logger">日志记录器实例</param>
        /// <param name="exception">要检查的异常</param>
        /// <param name="logLevel">日志记录级别</param>
        /// <seealso cref="IHasErrorCode"/>
        /// <seealso cref="IHasErrorDetails"/>
        private static void LogKnownProperties(ILogger logger, Exception exception, LogLevel logLevel)
        {
            // 如果异常包含错误代码，则记录
            if (exception is IHasErrorCode exceptionWithErrorCode)
            {
                logger.Log(logLevel, "Code:{Code}", exceptionWithErrorCode.Code);
            }

            // 如果异常包含错误详细信息，则记录
            if (exception is IHasErrorDetails exceptionWithErrorDetails)
            {
                logger.Log(logLevel, "Details:{Details}", exceptionWithErrorDetails.Details);
            }
        }

        /// <summary>
        /// 记录异常Data字典中的所有键值对
        /// </summary>
        /// <param name="logger">日志记录器实例</param>
        /// <param name="exception">要处理的异常</param>
        /// <param name="logLevel">日志记录级别</param>
        /// <remarks>
        /// 如果异常的Data字典为空或null，则不进行任何记录
        /// </remarks>
        private static void LogData(ILogger logger, Exception exception, LogLevel logLevel)
        {
            // 检查异常是否包含额外数据
            if (exception.Data == null || exception.Data.Count <= 0)
            {
                return;
            }

            // 记录数据分隔符
            logger.Log(logLevel, "---------- Exception Data ----------");

            // 遍历并记录所有数据项
            foreach (var key in exception.Data.Keys)
            {
                logger.Log(logLevel, "{Key} = {Value}", key, exception.Data[key]);
            }
        }

        /// <summary>
        /// 处理实现了自记录接口的异常，包括聚合异常的递归处理
        /// </summary>
        /// <param name="logger">日志记录器实例</param>
        /// <param name="exception">要处理的异常</param>
        /// <seealso cref="IExceptionWithSelfLogging"/>
        /// <seealso cref="AggregateException"/>
        private static void LogSelfLogging(ILogger logger, Exception exception)
        {
            // 收集所有需要自记录的异常
            var loggingExceptions = new List<IExceptionWithSelfLogging>();
            LogSelfLogging(loggingExceptions, exception);

            // 执行每个异常的自记录逻辑
            foreach (var ex in loggingExceptions)
            {
                ex.Log(logger);
            }
        }

        /// <summary>
        /// 递归收集实现了自记录接口的异常
        /// </summary>
        /// <param name="selfLoggings">用于收集自记录异常的列表</param>
        /// <param name="exception">要检查的异常</param>
        /// <remarks>
        /// 此方法会递归处理AggregateException中的所有内部异常
        /// </remarks>
        /// <seealso cref="IExceptionWithSelfLogging"/>
        /// <seealso cref="AggregateException"/>
        private static void LogSelfLogging(List<IExceptionWithSelfLogging> selfLoggings, Exception exception)
        {
            switch (exception)
            {
                // 如果异常实现了自记录接口，添加到列表中
                case IExceptionWithSelfLogging ex:
                    selfLoggings.Add(ex);
                    break;
                // 如果是聚合异常，递归处理所有内部异常
                case AggregateException aggException:
                    {
                        aggException.InnerExceptions.ForEach(e => LogSelfLogging(selfLoggings, e));
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
