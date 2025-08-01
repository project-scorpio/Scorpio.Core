namespace Scorpio.Modularity
{
    /// <summary>
    /// 定义模块管理器的接口，用于管理应用程序模块的生命周期
    /// </summary>
    /// <remarks>
    /// 该接口提供了模块生命周期管理的核心功能，负责协调所有已加载模块的初始化和关闭过程。
    /// 模块管理器确保模块按照正确的依赖顺序进行初始化和关闭，维护应用程序的稳定运行。
    /// </remarks>
    public interface IModuleManager
    {
        /// <summary>
        /// 初始化应用程序中的所有模块
        /// </summary>
        /// <param name="applicationInitializationContext">
        /// 应用程序初始化上下文，包含初始化过程中所需的服务提供程序和参数信息
        /// </param>
        /// <remarks>
        /// <para>
        /// 该方法按照模块的依赖顺序逐个调用每个模块的初始化方法。
        /// 初始化过程包括：
        /// <list type="bullet">
        /// <item><description>按依赖顺序遍历所有已加载的模块</description></item>
        /// <item><description>调用每个模块的 <see cref="IScorpioModule.Initialize(ApplicationInitializationContext)"/> 方法</description></item>
        /// <item><description>传递 <paramref name="applicationInitializationContext"/> 给模块使用</description></item>
        /// <item><description>处理初始化过程中的异常并确保系统稳定性</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 如果某个模块初始化失败，管理器会采取适当的错误处理策略。
        /// </para>
        /// </remarks>
        /// <exception cref="ScorpioException">当模块初始化过程中发生错误时抛出</exception>
        void InitializeModules(ApplicationInitializationContext applicationInitializationContext);

        /// <summary>
        /// 关闭应用程序中的所有模块
        /// </summary>
        /// <param name="applicationShutdownContext">
        /// 应用程序关闭上下文，包含关闭过程中所需的服务提供程序信息
        /// </param>
        /// <remarks>
        /// <para>
        /// 该方法按照与初始化相反的顺序逐个调用每个模块的关闭方法。
        /// 关闭过程包括：
        /// <list type="bullet">
        /// <item><description>按依赖顺序的逆序遍历所有已加载的模块</description></item>
        /// <item><description>调用每个模块的 <see cref="IScorpioModule.Shutdown(ApplicationShutdownContext)"/> 方法</description></item>
        /// <item><description>传递 <paramref name="applicationShutdownContext"/> 给模块使用</description></item>
        /// <item><description>确保所有模块都有机会执行清理操作</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 关闭过程中的异常不会阻止其他模块的关闭，以确保应用程序能够正常退出。
        /// </para>
        /// </remarks>
        void ShutdownModules(ApplicationShutdownContext applicationShutdownContext);
    }
}