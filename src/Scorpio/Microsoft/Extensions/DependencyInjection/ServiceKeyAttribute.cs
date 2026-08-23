#if !NET8_0_OR_GREATER
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 指示构造函数参数应注入当前服务注册或解析所使用的键。
    /// 仅在以非空键解析键控服务时生效，与 .NET 8 原生行为一致。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ServiceKeyAttribute : Attribute
    {
    }
}
#endif
