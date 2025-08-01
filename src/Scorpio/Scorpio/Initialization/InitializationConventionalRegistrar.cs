using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Scorpio.Conventional;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化约定注册器，负责自动发现和注册可初始化类型
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类实现了 <see cref="IConventionalRegistrar"/> 接口，用于在应用程序启动时
    /// 自动扫描和注册所有实现了 <see cref="IInitializable"/> 接口的标准类型。
    /// </para>
    /// <para>
    /// 通过约定注册机制，无需手动注册每个可初始化类型，系统会自动发现并
    /// 按照类型上标记的 <see cref="InitializationOrderAttribute"/> 进行排序注册。
    /// </para>
    /// </remarks>
    /// <seealso cref="IConventionalRegistrar"/>
    /// <seealso cref="IInitializable"/>
    /// <seealso cref="InitializationConventionalAction"/>
    internal class InitializationConventionalRegistrar : IConventionalRegistrar
    {
        /// <summary>
        /// 执行约定注册逻辑
        /// </summary>
        /// <param name="context">
        /// 约定注册上下文，包含类型扫描和服务注册所需的信息
        /// </param>
        /// <remarks>
        /// 该方法配置约定注册条件，筛选出所有标准类型且实现了 <see cref="IInitializable"/> 
        /// 接口的类型，并通过 <see cref="InitializationConventionalAction"/> 进行处理。
        /// </remarks>
        public void Register(IConventionalRegistrationContext context)
        {
            context.DoConventionalAction<InitializationConventionalAction>(config =>
            {
                // 筛选标准类型且实现了 IInitializable 接口的类型
                config.Where(t => t.IsStandardType()).Where(t => t.IsAssignableTo<IInitializable>());
            });
        }
    }
}
