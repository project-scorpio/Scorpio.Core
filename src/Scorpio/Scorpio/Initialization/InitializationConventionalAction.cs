
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化约定操作类，负责将发现的可初始化类型注册到初始化配置中
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类继承自 <see cref="ConventionalActionBase"/>，实现了将扫描到的
    /// <see cref="IInitializable"/> 类型注册到 <see cref="InitializationOptions"/> 
    /// 配置中的逻辑。
    /// </para>
    /// <para>
    /// 注册过程中会自动读取类型上的 <see cref="InitializationOrderAttribute"/> 
    /// 特性以确定初始化顺序，未标记特性的类型使用默认顺序值。
    /// </para>
    /// </remarks>
    /// <seealso cref="ConventionalActionBase"/>
    /// <seealso cref="InitializationOptions"/>
    /// <seealso cref="InitializationOrderAttribute"/>
    internal class InitializationConventionalAction : ConventionalActionBase
    {
        /// <summary>
        /// 初始化 <see cref="InitializationConventionalAction"/> 类的新实例
        /// </summary>
        /// <param name="configuration">
        /// 约定配置信息，包含类型筛选条件和注册规则
        /// </param>
        public InitializationConventionalAction(IConventionalConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// 执行约定注册操作
        /// </summary>
        /// <param name="context">
        /// 约定上下文，包含要处理的类型集合和服务注册容器
        /// </param>
        /// <remarks>
        /// <para>
        /// 该方法遍历上下文中的所有类型，将每个类型及其初始化顺序添加到
        /// <see cref="InitializationOptions"/> 配置中。
        /// </para>
        /// <para>
        /// 初始化顺序通过 <see cref="InitializationOrderAttribute.GetOrder(Type, int)"/> 
        /// 方法获取，如果类型未标记特性则使用默认值 0。
        /// </para>
        /// </remarks>
        protected override void Action(IConventionalContext context) => 
            context.Types.ForEach(t => 
                context.Services.Configure<InitializationOptions>(opts => 
                    opts.AddInitializable(t, InitializationOrderAttribute.GetOrder(t))));
    }
}