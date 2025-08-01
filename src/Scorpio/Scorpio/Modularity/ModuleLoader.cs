using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Modularity.Plugins;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 模块加载器的默认实现类，负责发现、解析和加载应用程序模块
    /// </summary>
    /// <remarks>
    /// 该类实现了 <see cref="IModuleLoader"/> 接口，提供模块加载的核心功能：
    /// <list type="bullet">
    /// <item><description>从启动模块开始递归发现所有依赖模块</description></item>
    /// <item><description>从插件源中发现和加载插件模块</description></item>
    /// <item><description>解析模块间的依赖关系并构建依赖图</description></item>
    /// <item><description>按照依赖顺序对模块进行拓扑排序</description></item>
    /// <item><description>创建模块实例并注册到依赖注入容器</description></item>
    /// </list>
    /// </remarks>
    internal class ModuleLoader : IModuleLoader
    {
        /// <summary>
        /// 加载应用程序的所有模块，包括启动模块及其依赖项和插件模块
        /// </summary>
        /// <param name="services">服务集合，用于注册模块相关的服务和依赖项</param>
        /// <param name="startupModuleType">启动模块的类型，作为模块加载的入口点</param>
        /// <param name="plugInSources">插件源列表，包含要动态加载的插件模块</param>
        /// <returns>
        /// 包含所有已加载模块描述符的数组，按照依赖关系和加载顺序排列。
        /// 每个 <see cref="IModuleDescriptor"/> 包含模块的详细信息和状态。
        /// </returns>
        /// <exception cref="ArgumentNullException">当任何参数为 null 时抛出</exception>
        /// <exception cref="ScorpioException">当模块加载过程中发生错误时抛出</exception>
        public IModuleDescriptor[] LoadModules(
            IServiceCollection services,
            Type startupModuleType,
            IPlugInSourceList plugInSources)
        {
            // 验证参数有效性
            Check.NotNull(services, nameof(services));
            Check.NotNull(startupModuleType, nameof(startupModuleType));
            Check.NotNull(plugInSources, nameof(plugInSources));

            // 获取所有模块描述符
            var modules = GetDescriptors(services, startupModuleType, plugInSources);

            // 按依赖关系排序模块
            modules = SortByDependency(modules, startupModuleType);

            // 返回模块描述符数组
            return modules.ToArray();
        }

        /// <summary>
        /// 获取所有模块的描述符，包括创建模块实例和设置依赖关系
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="startupModuleType">启动模块类型</param>
        /// <param name="plugInSources">插件源列表</param>
        /// <returns>包含所有模块描述符的列表</returns>
        /// <remarks>
        /// 该方法协调模块填充和依赖关系设置的整个过程，确保所有模块都被正确创建和配置。
        /// </remarks>
        private List<IModuleDescriptor> GetDescriptors(
            IServiceCollection services,
            Type startupModuleType,
            IPlugInSourceList plugInSources)
        {
            // 创建模块描述符列表
            var modules = new List<ModuleDescriptor>();

            // 填充模块列表
            FillModules(modules, services, startupModuleType, plugInSources);
            // 设置模块间的依赖关系
            SetDependencies(modules);

            // 转换为接口类型并返回
            return modules.Cast<IModuleDescriptor>().ToList();
        }

        /// <summary>
        /// 填充模块列表，包括从启动模块发现的依赖模块和插件模块
        /// </summary>
        /// <param name="modules">要填充的模块描述符列表</param>
        /// <param name="services">服务集合</param>
        /// <param name="startupModuleType">启动模块类型</param>
        /// <param name="plugInSources">插件源列表</param>
        /// <remarks>
        /// <para>
        /// 该方法分两个阶段填充模块：
        /// <list type="number">
        /// <item><description>从启动模块开始递归发现所有依赖模块</description></item>
        /// <item><description>从插件源中发现插件模块，跳过已存在的模块</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 派生类可以重写此方法来自定义模块发现逻辑。
        /// </para>
        /// </remarks>
        protected virtual void FillModules(
            List<ModuleDescriptor> modules,
            IServiceCollection services,
            Type startupModuleType,
            IPlugInSourceList plugInSources)
        {
            // 添加从启动模块开始的所有模块
            foreach (var moduleType in ModuleHelper.FindAllModuleTypes(startupModuleType))
            {
                modules.Add(CreateModuleDescriptor(services, moduleType));
            }

            // 添加插件模块
            foreach (var moduleType in plugInSources.As<PlugInSourceList>().GetAllModules())
            {
                // 跳过已存在的模块
                if (modules.Any(m => m.Type == moduleType))
                {
                    continue;
                }

                // 创建插件模块描述符
                modules.Add(CreateModuleDescriptor(services, moduleType, isLoadedAsPlugIn: true));
            }
        }

        /// <summary>
        /// 按照依赖关系对模块进行排序，确保正确的加载顺序
        /// </summary>
        /// <param name="modules">要排序的模块列表</param>
        /// <param name="startupModuleType">启动模块类型</param>
        /// <returns>按依赖关系排序后的模块列表</returns>
        /// <remarks>
        /// <para>
        /// 排序规则：
        /// <list type="bullet">
        /// <item><description>使用拓扑排序算法按依赖关系排序</description></item>
        /// <item><description><see cref="KernelModule"/> 始终排在第一位</description></item>
        /// <item><description>启动模块始终排在最后一位</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 派生类可以重写此方法来自定义排序逻辑。
        /// </para>
        /// </remarks>
        protected virtual List<IModuleDescriptor> SortByDependency(List<IModuleDescriptor> modules, Type startupModuleType)
        {
            // 使用拓扑排序算法按依赖关系排序
            var sortedModules = modules.SortByDependencies(m => m.Dependencies);
            // 将启动模块移动到最后
            sortedModules.MoveItem(m => m.Type == startupModuleType, modules.Count - 1);
            // 将内核模块移动到第一位
            sortedModules.MoveItem(m => m.Type == typeof(KernelModule), 0);
            return sortedModules;
        }

        /// <summary>
        /// 创建模块描述符
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="moduleType">模块类型</param>
        /// <param name="isLoadedAsPlugIn">是否作为插件加载，默认为 false</param>
        /// <returns>创建的模块描述符实例</returns>
        /// <remarks>
        /// 该方法创建模块实例并将其注册到服务容器，然后创建对应的模块描述符。
        /// 派生类可以重写此方法来自定义模块描述符的创建逻辑。
        /// </remarks>
        protected virtual ModuleDescriptor CreateModuleDescriptor(IServiceCollection services, Type moduleType, bool isLoadedAsPlugIn = false) => 
            new ModuleDescriptor(moduleType, CreateAndRegisterModule(services, moduleType), isLoadedAsPlugIn);

        /// <summary>
        /// 创建模块实例并将其注册到服务集合中
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="moduleType">要创建的模块类型</param>
        /// <returns>创建的模块实例</returns>
        /// <remarks>
        /// <para>
        /// 该方法执行以下操作：
        /// <list type="number">
        /// <item><description>使用 <see cref="Activator.CreateInstance(Type)"/> 创建模块实例</description></item>
        /// <item><description>将模块实例作为单例注册到服务容器</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 派生类可以重写此方法来自定义模块实例的创建和注册逻辑。
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">当模块类型无法实例化时抛出</exception>
        protected virtual IScorpioModule CreateAndRegisterModule(IServiceCollection services, Type moduleType)
        {
            // 创建模块实例
            var module = (IScorpioModule)Activator.CreateInstance(moduleType);
            // 将模块注册为单例服务
            services.AddSingleton(moduleType, module);
            return module;
        }

        /// <summary>
        /// 为所有模块设置依赖关系
        /// </summary>
        /// <param name="modules">模块描述符列表</param>
        /// <remarks>
        /// 该方法遍历所有模块，为每个模块设置其依赖关系。
        /// 派生类可以重写此方法来自定义依赖关系设置逻辑。
        /// </remarks>
        protected virtual void SetDependencies(List<ModuleDescriptor> modules)
        {
            // 为每个模块设置依赖关系
            foreach (var module in modules)
            {
                SetDependencies(modules, module);
            }
        }

        /// <summary>
        /// 为指定模块设置其依赖关系
        /// </summary>
        /// <param name="modules">所有模块的列表，用于查找依赖模块</param>
        /// <param name="module">要设置依赖关系的目标模块</param>
        /// <remarks>
        /// 该方法通过 <see cref="ModuleHelper.FindDependedModuleTypes"/> 获取模块的依赖类型，
        /// 然后在模块列表中查找对应的模块描述符，并建立依赖关系。
        /// 派生类可以重写此方法来自定义单个模块的依赖关系设置逻辑。
        /// </remarks>
        /// <exception cref="NullReferenceException">当找不到依赖的模块时抛出</exception>
        protected virtual void SetDependencies(List<ModuleDescriptor> modules, ModuleDescriptor module)
        {
            // 遍历模块的每个依赖类型
            foreach (var dependedModuleType in ModuleHelper.FindDependedModuleTypes(module.Type))
            {
                #pragma warning disable S112 
                // 在模块列表中查找依赖模块
                var dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType) 
                    ?? throw new NullReferenceException($"Could not find a depended module {dependedModuleType.AssemblyQualifiedName} for { module.Type.AssemblyQualifiedName}");
                #pragma warning restore S112
                // 添加依赖关系
                module.AddDependency(dependedModule);
            }
        }
    }
}