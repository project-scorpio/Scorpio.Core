using System.Security.Principal;

namespace Scorpio.Security
{
    /// <summary>
    /// 定义当前主体访问器的接口。
    /// 提供获取当前执行上下文中安全主体信息的能力。
    /// </summary>
    public interface ICurrentPrincipalAccessor
    {
        /// <summary>
        /// 获取当前的安全主体。
        /// 返回代表当前用户身份和角色信息的 <see cref="IPrincipal"/> 实例。
        /// </summary>
        /// <value>
        /// 当前的安全主体，包含用户身份标识和相关的角色权限信息。
        /// 如果没有经过身份验证的用户，可能返回匿名主体或 null。
        /// </value>
        IPrincipal Principal { get; }
    }
}
