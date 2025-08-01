using System;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 非自动装配特性，用于标记属性不应该被自动注入依赖
    /// </summary>
    /// <remarks>
    /// 应用此特性的属性将被排除在自动依赖注入过程之外，即使该属性类型已在容器中注册
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NotAutowiredAttribute : Attribute
    {
    }
}
