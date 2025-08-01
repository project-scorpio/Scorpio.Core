using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 模块管理器的默认实现类，负责管理应用程序模块的生命周期
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IModuleManager"/> 接口，提供模块生命周期管理的核心功能：
    /// <list type="bullet">
    /// <item><description>按照依赖顺序初始化所有已加载的模块</description></item>
    /// <item><description>按照与初始化相反的顺序关闭所有模块</description></item>
    /// <item><description>记录模块加载和初始化的详细日志信息</description></item>
    /// </list>
    /// 该类确保模块按照正确的顺序进行初始化和关闭，维护应用程序的稳定运行。
    /// </remarks>
    internal class ModuleManager : IModuleManager
    {
        /// <summary>
        /// 模块容器，用于访问所有已加载的模块
        /// </summary>
        private readonly IModuleContainer _moduleContainer;

        /// <summary>
        /// 日志记录器，用于记录模块管理过程中的信息
        /// </summary>
        private readonly ILogger<ModuleManager> _logger;

        /// <summary>
        /// 初始化 <see cref="ModuleManager"/> 类的新实例
        /// </summary>
        /// <param name="moduleContainer">模块容器，提供对已加载模块的访问</param>
        /// <param name="logger">日志记录器，用于记录操作信息</param>
        /// <remarks>
        /// 该构造函数通过依赖注入获取模块容器和日志记录器实例，
        /// 为模块生命周期管理提供必要的依赖项。
        /// </remarks>
        public ModuleManager(
            IModuleContainer moduleContainer,
            ILogger<ModuleManager> logger)
        {
            // 设置模块容器引用
            _moduleContainer = moduleContainer;
            // 设置日志记录器引用
            _logger = logger;
        }

        /// <summary>
        /// 初始化应用程序中的所有模块
        /// </summary>
        /// <param name="applicationInitializationContext">
        /// 应用程序初始化上下文，包含初始化过程中所需的服务提供程序和参数信息
        /// </param>
        /// <remarks>
        /// <para>
        /// 该方法按照模块的依赖顺序执行三个阶段的初始化：
        /// <list type="number">
        /// <item><description>PreInitialize - 预初始化阶段</description></item>
        /// <item><description>Initialize - 主初始化阶段</description></item>
        /// <item><description>PostInitialize - 后初始化阶段</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 在初始化开始前会记录所有已加载模块的列表，便于调试和监控。
        /// 所有模块完成初始化后会记录完成信息。
        /// </para>
        /// </remarks>
        public void InitializeModules(ApplicationInitializationContext applicationInitializationContext)
        {
            // 记录已加载的模块列表
            LogListOfModules();

            // 执行所有模块的预初始化阶段
            _moduleContainer.Modules.ForEach(d => d.Instance.PreInitialize(applicationInitializationContext));
            // 执行所有模块的主初始化阶段
            _moduleContainer.Modules.ForEach(d => d.Instance.Initialize(applicationInitializationContext));
            // 执行所有模块的后初始化阶段
            _moduleContainer.Modules.ForEach(d => d.Instance.PostInitialize(applicationInitializationContext));

            // 记录初始化完成信息
            _logger.LogInformation("Initialized all modules.");
        }

        /// <summary>
        /// 记录已加载模块的列表信息
        /// </summary>
        /// <remarks>
        /// 该方法将所有已加载模块的完整类型名称记录到日志中，
        /// 便于调试和监控模块加载情况。每个模块会以单独的日志条目记录。
        /// </remarks>
        private void LogListOfModules()
        {
            // 记录模块列表标题
            _logger.LogInformation("Loaded modules:");

            // 遍历所有模块并记录其类型信息
            foreach (var module in _moduleContainer.Modules)
            {
                _logger.LogInformation("- ({FullName})", module.Type.FullName);
            }
        }

        /// <summary>
        /// 关闭应用程序中的所有模块
        /// </summary>
        /// <param name="applicationShutdownContext">
        /// 应用程序关闭上下文，包含关闭过程中所需的服务提供程序信息
        /// </param>
        /// <remarks>
        /// <para>
        /// 该方法按照与初始化相反的顺序关闭所有模块，确保依赖关系的正确处理。
        /// 通过 <see cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/> 方法
        /// 将模块列表反转，使得被依赖的模块在依赖它们的模块之后关闭。
        /// </para>
        /// <para>
        /// 关闭过程中即使某个模块出现异常，也不会阻止其他模块的关闭操作，
        /// 确保应用程序能够正常退出。
        /// </para>
        /// </remarks>
        public void ShutdownModules(ApplicationShutdownContext applicationShutdownContext)
        {
            // 获取模块列表的反向副本，按照与初始化相反的顺序关闭
            var modules = _moduleContainer.Modules.Reverse().ToList();
            // 执行所有模块的关闭操作
            modules.ForEach(d => d.Instance.Shutdown(applicationShutdownContext));
        }
    }
}
