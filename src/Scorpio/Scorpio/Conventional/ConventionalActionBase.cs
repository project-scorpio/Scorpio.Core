using System.Collections.Generic;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定动作基类
    /// 为所有约定操作提供统一的执行框架和上下文管理
    /// 通过模板方法模式定义约定操作的执行流程，子类只需实现具体的动作逻辑
    /// </summary>
    /// <seealso cref="IConventionalConfiguration"/>
    /// <seealso cref="IConventionalContext"/>
    /// <remarks>
    /// 此抽象类实现了模板方法模式，定义了约定操作的标准执行流程：
    /// <list type="number">
    /// <item><description>从配置中获取所有上下文</description></item>
    /// <item><description>遍历每个上下文</description></item>
    /// <item><description>对每个上下文执行具体的动作逻辑</description></item>
    /// </list>
    /// 子类需要继承此类并实现 <see cref="Action(IConventionalContext)"/> 方法来定义具体的处理逻辑。
    /// </remarks>
    public abstract class ConventionalActionBase
    {
        /// <summary>
        /// 约定配置对象
        /// 包含执行约定操作所需的配置信息和上下文集合
        /// </summary>
        private readonly IConventionalConfiguration _configuration;

        /// <summary>
        /// 初始化约定动作基类的新实例
        /// </summary>
        /// <param name="configuration">约定配置对象，包含执行操作所需的配置信息和上下文</param>
        /// <exception cref="System.ArgumentNullException">当 <paramref name="configuration"/> 为 null 时抛出</exception>
        /// <seealso cref="IConventionalConfiguration"/>
        protected ConventionalActionBase(IConventionalConfiguration configuration) => _configuration = configuration;

        /// <summary>
        /// 执行约定动作
        /// 从配置中获取所有上下文并依次执行动作逻辑
        /// 此方法实现了模板方法模式的算法骨架
        /// </summary>
        /// <seealso cref="ConventionalConfigurationExtensions.GetContexts(IConventionalConfiguration)"/>
        /// <seealso cref="Action(IConventionalContext)"/>
        /// <remarks>
        /// 此方法的执行流程：
        /// <list type="number">
        /// <item><description>调用配置对象的 <see cref="ConventionalConfigurationExtensions.GetContexts(IConventionalConfiguration)"/> 方法获取所有上下文</description></item>
        /// <item><description>使用 <c>ForEach</c> 遍历所有上下文</description></item>
        /// <item><description>对每个上下文调用抽象方法 <see cref="Action(IConventionalContext)"/></description></item>
        /// </list>
        /// </remarks>
        internal void Action()
        {
            // 从配置中获取所有需要处理的上下文
            var contexts = _configuration.GetContexts();
            // 遍历每个上下文并执行具体的动作逻辑
            contexts.ForEach(Action);
        }

        /// <summary>
        /// 对单个上下文执行约定动作
        /// 子类必须实现此方法来定义具体的约定操作逻辑
        /// </summary>
        /// <param name="context">约定上下文，包含执行操作所需的环境信息和数据</param>
        /// <seealso cref="IConventionalContext"/>
        /// <remarks>
        /// 此方法是模板方法模式中的抽象步骤，由子类实现具体的业务逻辑。
        /// 常见的实现包括：
        /// <list type="bullet">
        /// <item><description>依赖注入注册操作</description></item>
        /// <item><description>拦截器配置操作</description></item>
        /// <item><description>配置验证操作</description></item>
        /// <item><description>其他约定相关的处理逻辑</description></item>
        /// </list>
        /// </remarks>
        protected abstract void Action(IConventionalContext context);
    }
}
