using System;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 暴露服务特性，用于标记类型应该暴露为哪些服务类型
    /// </summary>
    /// <remarks>
    /// 实现 <see cref="IExposedServiceTypesProvider"/> 接口，支持自定义服务生命周期
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExposeServicesAttribute : Attribute, IExposedServiceTypesProvider
    {
        /// <summary>
        /// 获取要暴露的服务类型数组
        /// </summary>
        /// <value>暴露的服务类型集合 <see cref="Type"/>[]</value>
        public Type[] ExposedServiceTypes { get; }

        /// <summary>
        /// 获取或设置服务的生命周期
        /// </summary>
        /// <value>服务生命周期，默认为瞬态 <see cref="ServiceLifetime.Transient"/></value>
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// 初始化暴露服务特性的新实例
        /// </summary>
        /// <param name="exposedServiceTypes">要暴露的服务类型参数数组</param>
        public ExposeServicesAttribute(params Type[] exposedServiceTypes) => ExposedServiceTypes = exposedServiceTypes ?? new Type[0];

        /// <summary>
        /// 获取目标类型要暴露的服务类型集合
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <returns>返回要暴露的服务类型数组 <see cref="Type"/>[]</returns>
        public Type[] GetExposedServiceTypes(Type targetType) => ExposedServiceTypes;
    }
}
