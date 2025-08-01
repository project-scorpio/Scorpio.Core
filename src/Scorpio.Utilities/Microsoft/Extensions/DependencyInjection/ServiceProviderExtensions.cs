using System;
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceProvider"/> 的扩展方法类。
    /// 提供额外的服务解析功能，增强依赖注入容器的使用便利性。
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// 获取指定类型的服务，如果服务未注册则返回默认值。
        /// 提供比标准 <see cref="ServiceProviderServiceExtensions.GetService{T}(IServiceProvider)"/> 更灵活的服务解析机制。
        /// </summary>
        /// <typeparam name="T">要获取的服务类型</typeparam>
        /// <param name="serviceProvider">服务提供程序实例</param>
        /// <param name="defaultValue">
        /// 当服务未注册或解析失败时调用的默认值提供函数，
        /// 允许延迟计算默认值以提高性能
        /// </param>
        /// <returns>
        /// 解析到的 <typeparamref name="T"/> 类型服务实例，
        /// 如果服务未注册则返回 <paramref name="defaultValue"/> 函数的结果
        /// </returns>
        public static T GetService<T>(this IServiceProvider serviceProvider, Func<T> defaultValue)
        {
            // 尝试从服务提供程序解析服务，如果返回 null 则调用默认值函数
            var result = serviceProvider.GetService<T>() ?? defaultValue();
            return result;
        }
    }
}
