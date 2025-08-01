namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 代理约定操作接口，定义了动态代理约定操作的核心功能
    /// </summary>
    /// <remarks>
    /// 此接口用于封装动态代理的约定操作逻辑，通过传入代理约定操作上下文来执行具体的代理配置和处理
    /// </remarks>
    public interface IProxyConventionalAction
    {
        /// <summary>
        /// 执行代理约定操作
        /// </summary>
        /// <param name="context">代理约定操作上下文，包含代理操作所需的类型、服务等信息 <see cref="IProxyConventionalActionContext"/></param>
        void Action(IProxyConventionalActionContext context);
    }
}
