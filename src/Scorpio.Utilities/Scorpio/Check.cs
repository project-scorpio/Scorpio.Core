using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Scorpio
{
    /// <summary>
    /// 提供参数验证和断言检查的静态工具类
    /// </summary>
    /// <remarks>
    /// 此类包含一系列静态方法，用于在运行时验证方法参数的有效性。
    /// 当参数不满足预期条件时，会抛出相应的异常。
    /// 使用 <see cref="DebuggerStepThroughAttribute"/> 特性，调试时不会进入此类的方法内部。
    /// </remarks>
    /// <seealso cref="ArgumentNullException"/>
    /// <seealso cref="ArgumentException"/>
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// 检查指定的值不为 null
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <returns>如果值不为 null，则返回原始值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="value"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 这是一个泛型方法，可以用于检查任何类型的参数是否为 null。
        /// 如果检查通过，返回原始值以支持链式调用。
        /// </remarks>
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的值不为 null，并提供自定义错误消息
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <param name="message">当值为 null 时显示的自定义错误消息</param>
        /// <returns>如果值不为 null，则返回原始值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="value"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许指定自定义的错误消息，提供更详细的错误信息。
        /// </remarks>
        public static T NotNull<T>(T value, string parameterName, string message) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的值不为 null 或默认值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <returns>如果值不为 null 或默认值，则返回原始值</returns>
        /// <exception cref="ArgumentException">当 <paramref name="value"/> 为 null 或默认值时抛出</exception>
        /// <remarks>
        /// 此方法用于检查值是否为 null 或默认值，并提供自定义错误消息。
        /// </remarks>
        public static T NotNullOrDefault<T>(T value, string parameterName)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                throw new ArgumentException($"{parameterName} can not be null or default!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的值不为 null 或默认值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <param name="message">当值为 null 或默认值时显示的自定义错误消息</param>
        /// <returns>如果值不为 null 或默认值，则返回原始值</returns>
        /// <exception cref="ArgumentException">当 <paramref name="value"/> 为 null 或默认值时抛出</exception>
        /// <remarks>
        /// 此方法用于检查值是否为 null 或默认值，并提供自定义错误消息。
        /// </remarks>
        public static T NotNullOrDefault<T>(T value, string parameterName, string message)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                throw new ArgumentException(message, parameterName);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的字符串不为 null、空字符串或仅包含空白字符
        /// </summary>
        /// <param name="value">要检查的字符串值</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <returns>如果字符串有效，则返回原始字符串</returns>
        /// <exception cref="ArgumentException">当 <paramref name="value"/> 为 null、空字符串或仅包含空白字符时抛出</exception>
        /// <remarks>
        /// 此方法比 <see cref="NotNullOrEmpty"/> 更严格，因为它还会检查空白字符。
        /// 使用 <see cref="string.IsNullOrWhiteSpace"/> 扩展方法进行检查。
        /// </remarks>
        /// <seealso cref="NotNullOrEmpty"/>
        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的字符串不为 null 或空字符串
        /// </summary>
        /// <param name="value">要检查的字符串值</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <returns>如果字符串不为 null 或空，则返回原始字符串</returns>
        /// <exception cref="ArgumentException">当 <paramref name="value"/> 为 null 或空字符串时抛出</exception>
        /// <remarks>
        /// 此方法允许包含空白字符的字符串通过验证。
        /// 如果需要更严格的验证，请使用 <see cref="NotNullOrWhiteSpace"/>。
        /// 使用 <see cref="string.IsNullOrEmpty"/> 扩展方法进行检查。
        /// </remarks>
        /// <seealso cref="NotNullOrWhiteSpace"/>
        public static string NotNullOrEmpty(string value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的集合不为 null 或空集合
        /// </summary>
        /// <typeparam name="T">集合元素的类型</typeparam>
        /// <param name="value">要检查的集合</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <returns>如果集合不为 null 或空，则返回原始集合</returns>
        /// <exception cref="ArgumentException">当 <paramref name="value"/> 为 null 或空集合时抛出</exception>
        /// <remarks>
        /// 此方法适用于任何实现了 <see cref="ICollection{T}"/> 接口的集合类型。
        /// 使用扩展方法进行 null 和空集合的检查。
        /// </remarks>
        /// <seealso cref="ICollection{T}"/>
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// 检查指定的类型是否可以分配给基类型
        /// </summary>
        /// <typeparam name="TBaseType">基类型或接口类型</typeparam>
        /// <param name="type">要检查的类型</param>
        /// <param name="parameterName">参数名称，用于异常消息</param>
        /// <returns>如果类型可以分配给基类型，则返回原始类型</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当 <paramref name="type"/> 不能分配给 <typeparamref name="TBaseType"/> 时抛出</exception>
        /// <remarks>
        /// 此方法用于验证类型继承或接口实现关系。
        /// 首先检查类型不为 null，然后验证类型兼容性。
        /// 使用反射扩展方法 <see cref="TypeExtensions.IsAssignableTo{T}"/> 进行检查。
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        public static Type AssignableTo<TBaseType>(
                    Type type,
                    string parameterName)
        {
            NotNull(type, parameterName);

            if (!type.IsAssignableTo<TBaseType>())
            {
                throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
            }

            return type;
        }
    }
}
