#if !NET8_0_OR_GREATER
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 指示构造函数的参数应使用指定键解析键控服务。
    /// 与 .NET 8 原生 <c>FromKeyedServicesAttribute</c> 的行为保持一致。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromKeyedServicesAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="FromKeyedServicesAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="key">用于解析依赖服务的键</param>
        public FromKeyedServicesAttribute(object key) => Key = key;

        /// <summary>
        /// 获取用于解析依赖服务的键。
        /// </summary>
        /// <value>依赖服务对应的键，可以为 null</value>
        public object Key { get; }
    }
}
#endif
