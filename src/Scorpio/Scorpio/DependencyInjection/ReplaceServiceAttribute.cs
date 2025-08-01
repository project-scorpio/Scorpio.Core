using System;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 替换服务特性，用于标记服务是否应该替换容器中已存在的同类型服务
    /// </summary>
    /// <remarks>
    /// 当设置为 true 时，该服务将替换依赖注入容器中已注册的同类型服务；否则将与现有服务共存
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ReplaceServiceAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置是否替换现有服务的标志
        /// </summary>
        /// <value>如果为 true 则替换现有服务，否则与现有服务共存</value>
        public bool ReplaceService { get; set; }
    }
}