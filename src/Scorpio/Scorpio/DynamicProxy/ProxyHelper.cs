using System;
using System.Reflection;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 代理帮助器静态类，提供代理对象相关的实用方法
    /// </summary>
    /// <remarks>
    /// 提供判断对象是否为代理和获取代理对象原始目标的功能
    /// </remarks>
    public static class ProxyHelper
    {

        /// <summary>
        /// 判断给定对象是否为代理对象
        /// </summary>
        /// <typeparam name="T">对象类型，必须是引用类型</typeparam>
        /// <param name="proxy">要检查的对象实例</param>
        /// <returns>如果是代理对象则返回 true，否则返回 false</returns>
        public static bool IsProxy<T>(this T proxy) where T : class
        {
            // 首先尝试从对象获取代理目标提供者特性
            var provider = proxy.GetAttribute<IProxyTargetProvider>(true);
            if (provider != null)
            {
                return provider.IsProxy(proxy);
            }
            // 如果没有特性，则遍历默认代理目标提供者列表
            foreach (var item in ProxyTargetProvider.Default.Providers)
            {
                var target = item.GetTarget(proxy);
                if (target != null)
                {
                    return item.IsProxy(proxy);
                }
            }
            return false;
        }

        /// <summary>
        /// 从代理对象中获取原始的目标对象，如果不是代理则返回原对象
        /// </summary>
        /// <typeparam name="T">对象类型，必须是引用类型</typeparam>
        /// <param name="proxy">代理对象或普通对象实例</param>
        /// <returns>返回原始目标对象，如果不是代理则返回原对象 <typeparamref name="T"/></returns>
        public static T UnProxy<T>(this T proxy) where T : class
        {
            // 首先尝试从对象获取代理目标提供者特性
            var provider = proxy.GetAttribute<IProxyTargetProvider>(true);
            if (provider != null)
            {
                return provider.GetTarget(proxy).As<T>();
            }
            // 如果没有特性，则遍历默认代理目标提供者列表
            foreach (var item in ProxyTargetProvider.Default.Providers)
            {
                var target = item.GetTarget(proxy);
                if (target != null)
                {
                    return target.As<T>();
                }
            }
            // 如果都没有找到目标对象，则返回原对象
            return proxy;
        }
    }
}
