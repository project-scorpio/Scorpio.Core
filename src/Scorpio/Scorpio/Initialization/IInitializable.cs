using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化接口，定义对象初始化的核心契约
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该接口为需要执行初始化操作的对象提供了统一的契约。实现此接口的类
    /// 可以在对象创建后执行必要的初始化逻辑，如资源加载、配置设置、
    /// 依赖项初始化等操作。
    /// </para>
    /// <para>
    /// 通常用于依赖注入容器中，在对象实例化后自动调用初始化方法，
    /// 确保对象在使用前处于正确的初始化状态。
    /// </para>
    /// </remarks>
    public interface IInitializable
    {
        /// <summary>
        /// 执行对象初始化操作
        /// </summary>
        /// <remarks>
        /// <para>
        /// 此方法在对象创建后被调用，用于执行必要的初始化逻辑。
        /// 实现此方法时应确保初始化操作的幂等性，即多次调用不会产生副作用。
        /// </para>
        /// <para>
        /// 实现注意事项：
        /// <list type="bullet">
        /// <item><description>初始化操作应该是幂等的，允许多次调用</description></item>
        /// <item><description>如果初始化失败，应抛出明确的异常</description></item>
        /// <item><description>避免在初始化过程中执行耗时操作，考虑使用异步版本</description></item>
        /// <item><description>确保线程安全，特别是在多线程环境中</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// 当初始化过程中发生错误或对象已处于无效状态时抛出
        /// </exception>
        public void Initialize();
    }
}
