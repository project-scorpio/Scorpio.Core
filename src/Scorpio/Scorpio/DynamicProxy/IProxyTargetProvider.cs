namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 代理目标提供者接口，用于获取代理对象的原始目标对象和判断对象是否为代理
    /// </summary>
    /// <remarks>
    /// 此接口提供了访问代理对象内部目标对象的能力，以及判断给定对象是否为代理对象的功能
    /// </remarks>
    public interface IProxyTargetProvider
    {
        /// <summary>
        /// 从代理对象中获取原始的目标对象
        /// </summary>
        /// <param name="proxy">代理对象实例</param>
        /// <returns>返回代理对象包装的原始目标对象</returns>
        object GetTarget(object proxy);

        /// <summary>
        /// 判断给定对象是否为代理对象
        /// </summary>
        /// <param name="proxy">要检查的对象实例</param>
        /// <returns>如果是代理对象则返回 true，否则返回 false</returns>
        bool IsProxy(object proxy);
    }
}
