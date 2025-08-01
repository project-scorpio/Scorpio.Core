namespace Scorpio.Modularity
{
    /// <summary>
    /// 定义 Scorpio 模块的核心接口，为模块化应用程序提供标准的生命周期方法
    /// </summary>
    /// <remarks>
    /// 该接口定义了模块在应用程序生命周期中的关键阶段，包括服务配置阶段和应用程序初始化/关闭阶段。
    /// 所有 Scorpio 模块都必须实现此接口，以确保能够正确参与到模块化系统的生命周期管理中。
    /// 模块方法的执行顺序：PreConfigureServices → ConfigureServices → PostConfigureServices → 
    /// PreInitialize → Initialize → PostInitialize → Shutdown
    /// </remarks>
    public interface IScorpioModule
    {
        /// <summary>
        /// 在服务配置之前执行的预配置方法
        /// </summary>
        /// <param name="context">服务配置上下文，包含服务集合、配置信息等</param>
        /// <remarks>
        /// 该方法在主要的服务配置阶段之前执行，适用于：
        /// <list type="bullet">
        /// <item><description>注册约定注册器</description></item>
        /// <item><description>配置全局设置和选项</description></item>
        /// <item><description>设置服务配置的前置条件</description></item>
        /// </list>
        /// 此阶段的配置会影响后续的服务注册过程。
        /// </remarks>
        void PreConfigureServices(ConfigureServicesContext context);

        /// <summary>
        /// 配置模块的服务注册，这是模块服务配置的主要阶段
        /// </summary>
        /// <param name="context">服务配置上下文，包含服务集合、配置信息等</param>
        /// <remarks>
        /// 该方法是模块服务配置的核心阶段，用于：
        /// <list type="bullet">
        /// <item><description>向 DI 容器注册模块的服务</description></item>
        /// <item><description>配置服务的生命周期和行为</description></item>
        /// <item><description>注册接口和实现的映射关系</description></item>
        /// <item><description>配置选项和设置对象</description></item>
        /// </list>
        /// 大部分的服务注册逻辑应该在此方法中实现。
        /// </remarks>
        void ConfigureServices(ConfigureServicesContext context);

        /// <summary>
        /// 在服务配置之后执行的后配置方法
        /// </summary>
        /// <param name="context">服务配置上下文，包含服务集合、配置信息等</param>
        /// <remarks>
        /// 该方法在主要的服务配置阶段之后执行，适用于：
        /// <list type="bullet">
        /// <item><description>覆盖或替换已注册的服务</description></item>
        /// <item><description>添加装饰器或代理服务</description></item>
        /// <item><description>执行服务配置的最终调整</description></item>
        /// <item><description>验证服务配置的完整性</description></item>
        /// </list>
        /// 此阶段可以修改之前阶段注册的服务配置。
        /// </remarks>
        void PostConfigureServices(ConfigureServicesContext context);

        /// <summary>
        /// 在应用程序初始化之前执行的预初始化方法
        /// </summary>
        /// <param name="context">应用程序初始化上下文，包含服务提供程序和初始化参数</param>
        /// <remarks>
        /// 该方法在主要的应用程序初始化阶段之前执行，适用于：
        /// <list type="bullet">
        /// <item><description>准备初始化所需的资源</description></item>
        /// <item><description>设置全局状态或环境</description></item>
        /// <item><description>执行依赖项的预检查</description></item>
        /// </list>
        /// 通过 <paramref name="context"/> 可以访问已构建的服务提供程序。
        /// </remarks>
        void PreInitialize(ApplicationInitializationContext context);

        /// <summary>
        /// 执行模块的主要初始化逻辑
        /// </summary>
        /// <param name="context">应用程序初始化上下文，包含服务提供程序和初始化参数</param>
        /// <remarks>
        /// 该方法是模块初始化的核心阶段，用于：
        /// <list type="bullet">
        /// <item><description>初始化模块的核心功能</description></item>
        /// <item><description>启动后台服务和任务</description></item>
        /// <item><description>配置中间件管道</description></item>
        /// <item><description>建立数据库连接和缓存</description></item>
        /// </list>
        /// 通过 <paramref name="context"/> 可以获取服务实例和初始化参数。
        /// </remarks>
        void Initialize(ApplicationInitializationContext context);

        /// <summary>
        /// 在应用程序初始化之后执行的后初始化方法
        /// </summary>
        /// <param name="context">应用程序初始化上下文，包含服务提供程序和初始化参数</param>
        /// <remarks>
        /// 该方法在主要的应用程序初始化阶段之后执行，适用于：
        /// <list type="bullet">
        /// <item><description>执行需要其他模块完成初始化的操作</description></item>
        /// <item><description>启动依赖于完整应用程序状态的功能</description></item>
        /// <item><description>执行最终的验证和健康检查</description></item>
        /// </list>
        /// 此时所有模块的主要初始化都已完成，可以安全地使用跨模块功能。
        /// </remarks>
        void PostInitialize(ApplicationInitializationContext context);

        /// <summary>
        /// 执行模块的关闭和清理操作
        /// </summary>
        /// <param name="context">应用程序关闭上下文，包含服务提供程序信息</param>
        /// <remarks>
        /// 该方法在应用程序关闭时执行，用于：
        /// <list type="bullet">
        /// <item><description>释放模块占用的资源</description></item>
        /// <item><description>停止后台服务和任务</description></item>
        /// <item><description>关闭数据库连接和网络连接</description></item>
        /// <item><description>保存重要的状态信息</description></item>
        /// </list>
        /// 通过 <paramref name="context"/> 可以访问服务提供程序来获取需要清理的服务。
        /// 关闭方法按照与初始化相反的顺序执行。
        /// </remarks>
        void Shutdown(ApplicationShutdownContext context);
    }
}
