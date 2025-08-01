using System;
using System.Reflection;

namespace Scorpio.Modularity
{
    /// <summary>
    /// Scorpio 模块的抽象基类，所有模块定义类都必须继承此类
    /// </summary>
    /// <remarks>
    /// <para>
    /// 模块定义类通常位于其自己的程序集中，在应用程序启动和关闭时实现模块事件中的特定操作。
    /// 它还定义了依赖的模块关系。
    /// </para>
    /// <para>
    /// 该类实现了 <see cref="IScorpioModule"/> 接口，为所有 Scorpio 模块提供标准的生命周期方法实现。
    /// 派生类可以根据需要重写这些虚方法来实现特定的模块逻辑。
    /// </para>
    /// </remarks>
    public abstract class ScorpioModule : IScorpioModule
    {
        /// <summary>
        /// 获取或设置一个值，指示是否跳过自动服务注册
        /// </summary>
        /// <value>
        /// 如果为 <c>true</c>，则跳过模块的自动服务注册过程；
        /// 如果为 <c>false</c>（默认值），则执行自动服务注册。
        /// </value>
        /// <remarks>
        /// 当模块需要完全控制服务注册过程，或者不需要自动服务发现时，
        /// 可以将此属性设置为 <c>true</c>。
        /// </remarks>
        protected internal bool SkipAutoServiceRegistration { get; set; } = false;

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
        /// 派生类可以重写此方法来实现特定的预配置逻辑。
        /// </remarks>
        public virtual void PreConfigureServices(ConfigureServicesContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }

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
        public virtual void ConfigureServices(ConfigureServicesContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }

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
        /// 派生类可以重写此方法来实现后配置逻辑。
        /// </remarks>
        public virtual void PostConfigureServices(ConfigureServicesContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }

        /// <summary>
        /// 检查指定类型是否为有效的 Scorpio 模块类型
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>
        /// 如果类型是有效的 Scorpio 模块，则为 <c>true</c>；否则为 <c>false</c>。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 有效的 Scorpio 模块类型必须满足以下条件：
        /// <list type="bullet">
        /// <item><description>必须是类类型（不能是接口、结构体等）</description></item>
        /// <item><description>必须是非抽象类</description></item>
        /// <item><description>必须是非泛型类型</description></item>
        /// <item><description>必须实现 <see cref="IScorpioModule"/> 接口</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 该方法主要用于模块加载过程中的类型验证。
        /// </para>
        /// </remarks>
        public static bool IsModule(Type type)
        {
            // 获取类型信息
            var typeInfo = type.GetTypeInfo();

            return
                typeInfo.IsClass &&                                                    // 必须是类
                !typeInfo.IsAbstract &&                                              // 必须是非抽象类
                !typeInfo.IsGenericType &&                                           // 必须是非泛型类型
                typeof(IScorpioModule).GetTypeInfo().IsAssignableFrom(type);         // 必须实现 IScorpioModule 接口
        }

        /// <summary>
        /// 检查模块类型的有效性，如果无效则抛出异常
        /// </summary>
        /// <param name="moduleType">要检查的模块类型</param>
        /// <exception cref="ArgumentException">当 <paramref name="moduleType"/> 不是有效的 Scorpio 模块类型时抛出</exception>
        /// <remarks>
        /// 该方法内部调用 <see cref="IsModule(Type)"/> 方法进行验证，
        /// 如果类型无效则抛出包含详细错误信息的异常。
        /// 主要用于模块加载器中的类型验证。
        /// </remarks>
        internal static void CheckModuleType(Type moduleType)
        {
            // 检查模块类型，如果无效则抛出异常
            if (!IsModule(moduleType))
            {
                throw new ArgumentException("Given type is not an Scorpio module: " + moduleType.AssemblyQualifiedName);
            }
        }

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
        /// 派生类可以重写此方法来实现特定的预初始化逻辑。
        /// </remarks>
        public virtual void PreInitialize(ApplicationInitializationContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }

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
        /// 派生类应该重写此方法来实现具体的初始化逻辑。
        /// </remarks>
        public virtual void Initialize(ApplicationInitializationContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }

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
        /// 派生类可以重写此方法来实现后初始化逻辑。
        /// </remarks>
        public virtual void PostInitialize(ApplicationInitializationContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }

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
        /// 派生类可以重写此方法来实现特定的清理逻辑。
        /// </remarks>
        public virtual void Shutdown(ApplicationShutdownContext context)
        {
            // 基类提供空实现，派生类可根据需要重写
        }
    }
}
