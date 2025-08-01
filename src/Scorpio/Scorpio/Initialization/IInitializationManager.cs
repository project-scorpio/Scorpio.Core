using System;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化管理器接口，定义系统初始化管理的核心契约
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该接口定义了系统初始化管理器的基本功能，用于协调和管理所有实现了
    /// <see cref="IInitializable"/> 接口的组件或服务的初始化过程。
    /// </para>
    /// <para>
    /// 初始化管理器负责按照预定义的优先级顺序执行系统中各个组件的初始化操作，
    /// 确保系统在启动时所有依赖项都能正确初始化。
    /// </para>
    /// </remarks>
    /// <seealso cref="IInitializable"/>
    /// <seealso cref="InitializationOrderAttribute"/>
    public interface IInitializationManager
    {
        /// <summary>
        /// 执行系统组件或服务的初始化过程
        /// </summary>
        /// <remarks>
        /// <para>
        /// 该方法启动系统的初始化流程，按照预配置的优先级顺序初始化所有
        /// 注册的可初始化组件。通常在应用程序启动时调用。
        /// </para>
        /// <para>
        /// 初始化过程应该是确定性的和可重复的，如果某个组件初始化失败，
        /// 应当抛出明确的异常以便诊断问题。
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// 当初始化过程中发生错误时抛出
        /// </exception>
        public void Initialize();
    }
}