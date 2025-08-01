using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定配置的基础接口，定义了约定配置的核心功能
    /// </summary>
    public interface IConventionalConfiguration
    {
        /// <summary>
        /// 获取依赖注入服务集合
        /// </summary>
        /// <value>服务集合实例 <see cref="IServiceCollection"/></value>
        IServiceCollection Services { get; }

    }

    /// <summary>
    /// 泛型约定配置接口，继承自 <see cref="IConventionalConfiguration"/>
    /// </summary>
    /// <typeparam name="TAction">约定操作类型参数，用于指定特定的约定操作</typeparam>
#pragma warning disable S2326
    public interface IConventionalConfiguration<out TAction> : IConventionalConfiguration
    {


    }
#pragma warning restore S2326
}
