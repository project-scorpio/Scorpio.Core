using System;
using System.Collections.Generic;

using Scorpio.DynamicProxy;

namespace Scorpio.Aspects
{
    /// <summary>
    /// 横切关注点管理器
    /// 提供管理和跟踪横切关注点应用状态的静态方法
    /// 用于避免在AOP场景中重复应用相同的横切关注点，防止无限递归或重复处理
    /// </summary>
    /// <seealso cref="IAvoidDuplicateCrossCuttingConcerns"/>
    /// <seealso cref="ProxyHelper.UnProxy"/>
    public static class CrossCuttingConcerns
    {
        /// <summary>
        /// 添加已应用的横切关注点
        /// 将指定的横切关注点标记为已应用到目标对象上，用于避免重复应用
        /// </summary>
        /// <param name="obj">目标对象，必须实现 <see cref="IAvoidDuplicateCrossCuttingConcerns"/> 接口</param>
        /// <param name="concerns">要添加的横切关注点名称数组</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="obj"/> 或 <paramref name="concerns"/> 为空时抛出</exception>
        /// <seealso cref="IAvoidDuplicateCrossCuttingConcerns.AppliedCrossCuttingConcerns"/>
        /// <seealso cref="ProxyHelper.UnProxy"/>
        public static void AddApplied(object obj, params string[] concerns)
        {

            // 验证参数不为空
            Check.NotNull(obj, nameof(obj));
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            // 获取未代理的原始对象并添加横切关注点
            (obj.UnProxy() as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.AddRange(concerns);
        }

        /// <summary>
        /// 移除已应用的横切关注点
        /// 从目标对象中移除指定的横切关注点标记，表示这些关注点不再应用
        /// </summary>
        /// <param name="obj">目标对象，必须实现 <see cref="IAvoidDuplicateCrossCuttingConcerns"/> 接口</param>
        /// <param name="concerns">要移除的横切关注点名称数组</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="obj"/> 或 <paramref name="concerns"/> 为空时抛出</exception>
        /// <seealso cref="IAvoidDuplicateCrossCuttingConcerns.AppliedCrossCuttingConcerns"/>
        /// <seealso cref="ProxyHelper.UnProxy"/>
        public static void RemoveApplied(object obj, params string[] concerns)
        {
            // 验证参数不为空
            Check.NotNull(obj, nameof(obj));

            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            // 获取未代理的原始对象
            if (obj.UnProxy() is not IAvoidDuplicateCrossCuttingConcerns crossCuttingEnabledObj)
            {
                return;
            }

            // 逐个移除指定的横切关注点
            foreach (var concern in concerns)
            {
                crossCuttingEnabledObj.AppliedCrossCuttingConcerns.RemoveAll(c => c == concern);
            }
        }

        /// <summary>
        /// 检查横切关注点是否已应用
        /// 判断指定的横切关注点是否已经应用到目标对象上
        /// </summary>
        /// <param name="obj">目标对象，必须实现 <see cref="IAvoidDuplicateCrossCuttingConcerns"/> 接口</param>
        /// <param name="concern">要检查的横切关注点名称</param>
        /// <returns>如果横切关注点已应用则返回 true，否则返回 false</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="obj"/> 或 <paramref name="concern"/> 为空时抛出</exception>
        /// <seealso cref="IAvoidDuplicateCrossCuttingConcerns.AppliedCrossCuttingConcerns"/>
        /// <seealso cref="ProxyHelper.UnProxy"/>
        public static bool IsApplied(object obj, string concern)
        {
            // 验证参数不为空
            Check.NotNull(obj, nameof(obj));
            if (concern == null)
            {
                throw new ArgumentNullException(nameof(concern));
            }

            // 检查横切关注点是否存在于已应用列表中
            return (obj.UnProxy() as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.Contains(concern) ?? false;
        }

        /// <summary>
        /// 临时应用横切关注点
        /// 添加横切关注点并返回一个可释放对象，当释放时自动移除这些关注点
        /// 这提供了一种RAII（资源获取即初始化）模式的横切关注点管理方式
        /// </summary>
        /// <param name="obj">目标对象，必须实现 <see cref="IAvoidDuplicateCrossCuttingConcerns"/> 接口</param>
        /// <param name="concerns">要临时应用的横切关注点名称数组</param>
        /// <returns>返回 <see cref="IDisposable"/> 对象，释放时自动移除横切关注点</returns>
        /// <seealso cref="AddApplied"/>
        /// <seealso cref="RemoveApplied"/>
        /// <seealso cref="DisposeAction"/>
        /// <example>
        /// using (CrossCuttingConcerns.Applying(obj, "Logging", "Caching"))
        /// {
        ///     // 在此范围内，横切关注点被标记为已应用
        ///     // 执行业务逻辑
        /// } // 自动移除横切关注点标记
        /// </example>
        public static IDisposable Applying(object obj, params string[] concerns)
        {
            // 添加横切关注点
            AddApplied(obj, concerns);
            // 返回释放操作，当释放时移除横切关注点
            return new DisposeAction(() => RemoveApplied(obj, concerns));
        }

        /// <summary>
        /// 获取所有已应用的横切关注点
        /// 返回目标对象上当前已应用的所有横切关注点名称
        /// </summary>
        /// <param name="obj">目标对象，必须实现 <see cref="IAvoidDuplicateCrossCuttingConcerns"/> 接口</param>
        /// <returns>包含所有已应用横切关注点名称的字符串数组</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="obj"/> 为空时抛出</exception>
        /// <seealso cref="IAvoidDuplicateCrossCuttingConcerns.AppliedCrossCuttingConcerns"/>
        /// <seealso cref="ProxyHelper.UnProxy"/>
        public static string[] GetApplieds(object obj)
        {
            // 验证参数不为空
            Check.NotNull(obj, nameof(obj));

            // 获取未代理的原始对象
            if (!(obj.UnProxy() is IAvoidDuplicateCrossCuttingConcerns crossCuttingEnabledObj))
            {
                // 如果对象不支持横切关注点管理，返回空数组
                return new string[0];
            }

            // 返回已应用的横切关注点数组
            return crossCuttingEnabledObj.AppliedCrossCuttingConcerns.ToArray();
        }
    }
}
