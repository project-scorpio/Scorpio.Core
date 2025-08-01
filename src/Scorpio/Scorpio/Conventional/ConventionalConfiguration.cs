using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定配置抽象基类
    /// 为约定操作提供基础配置管理功能，包含服务集合、类型集合和上下文管理
    /// 作为所有约定配置的基础实现，定义了配置的核心结构和行为
    /// </summary>
    /// <seealso cref="IConventionalConfiguration"/>
    /// <seealso cref="ConventionalConfiguration{TAction}"/>
    /// <seealso cref="ConventionalContext"/>
    internal abstract class ConventionalConfiguration : IConventionalConfiguration
    {
        /// <summary>
        /// 初始化约定配置基类的新实例
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="types">要处理的类型集合，约定操作将应用于这些类型</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="services"/> 或 <paramref name="types"/> 为 null 时抛出</exception>
        /// <seealso cref="IServiceCollection"/>
        protected ConventionalConfiguration(IServiceCollection services, IEnumerable<Type> types)
        {
            // 初始化服务集合和类型集合
            Services = services;
            Types = types;
        }

        /// <summary>
        /// 获取服务集合
        /// 用于注册和管理依赖注入服务的容器
        /// </summary>
        /// <value>服务集合实例，包含所有已注册的服务描述符</value>
        /// <seealso cref="IServiceCollection"/>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取类型集合
        /// 包含需要应用约定操作的所有类型
        /// </summary>
        /// <value>类型集合，约定操作将遍历并处理这些类型</value>
        /// <seealso cref="Type"/>
        public IEnumerable<Type> Types { get; }

        /// <summary>
        /// 约定上下文集合
        /// 存储所有创建的约定上下文实例，用于跟踪和管理约定操作的执行状态
        /// </summary>
        /// <value>包含所有约定上下文的集合</value>
        /// <seealso cref="ConventionalContext"/>
        /// <remarks>
        /// 此集合在约定操作执行过程中动态填充，每次创建新的上下文时都会添加到此集合中
        /// </remarks>
        internal ICollection<ConventionalContext> Contexts { get; } = new List<ConventionalContext>();

        /// <summary>
        /// 获取约定上下文
        /// 抽象方法，由子类实现具体的上下文创建逻辑
        /// </summary>
        /// <returns>约定上下文实例，包含执行约定操作所需的环境信息</returns>
        /// <seealso cref="IConventionalContext"/>
        /// <seealso cref="ConventionalConfiguration{TAction}.GetContext"/>
        internal abstract IConventionalContext GetContext();
    }

    /// <summary>
    /// 泛型约定配置类
    /// 为特定约定动作类型提供配置管理功能，继承自基础配置类并实现泛型接口
    /// </summary>
    /// <typeparam name="TAction">约定动作类型，必须继承自 <see cref="ConventionalActionBase"/></typeparam>
    /// <seealso cref="ConventionalConfiguration"/>
    /// <seealso cref="IConventionalConfiguration{TAction}"/>
    /// <seealso cref="ConventionalContext{TAction}"/>
    internal class ConventionalConfiguration<TAction> : ConventionalConfiguration, IConventionalConfiguration<TAction>
    {
        /// <summary>
        /// 初始化泛型约定配置类的新实例
        /// </summary>
        /// <param name="services">服务集合，用于注册依赖注入服务</param>
        /// <param name="types">要处理的类型集合，约定操作将应用于这些类型</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="services"/> 或 <paramref name="types"/> 为 null 时抛出</exception>
        /// <seealso cref="ConventionalConfiguration(IServiceCollection, IEnumerable{Type})"/>
        internal ConventionalConfiguration(IServiceCollection services, IEnumerable<Type> types) : base(services, types)
        {
        }

        /// <summary>
        /// 获取约定上下文
        /// 重写基类方法，委托给泛型版本的上下文获取方法
        /// </summary>
        /// <returns>约定上下文实例</returns>
        /// <seealso cref="GetInternalContext"/>
        /// <seealso cref="IConventionalContext"/>
        internal override IConventionalContext GetContext() => GetInternalContext();

        /// <summary>
        /// 获取泛型约定上下文
        /// 创建特定约定动作类型的上下文实例，并将其添加到上下文集合中
        /// </summary>
        /// <returns>泛型约定上下文实例，类型安全的上下文对象</returns>
        /// <seealso cref="ConventionalContext{TAction}"/>
        /// <seealso cref="IConventionalContext{TAction}"/>
        /// <remarks>
        /// 此方法执行以下操作：
        /// <list type="number">
        /// <item><description>创建新的泛型约定上下文实例</description></item>
        /// <item><description>将新创建的上下文添加到上下文集合中</description></item>
        /// <item><description>返回类型安全的泛型上下文接口</description></item>
        /// </list>
        /// </remarks>
        internal IConventionalContext<TAction> GetInternalContext()
        {
            // 创建特定动作类型的约定上下文
            var context = new ConventionalContext<TAction>(Services, Types);
            // 将上下文添加到集合中进行跟踪
            Contexts.Add(context);
            return context;
        }
    }
}
