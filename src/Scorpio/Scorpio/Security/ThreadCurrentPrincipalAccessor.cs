using System.Security.Principal;
using System.Threading;

using Scorpio.DependencyInjection;

namespace Scorpio.Security
{
    /// <summary>
    /// 基于线程的当前主体访问器实现。
    /// 实现 <see cref="ICurrentPrincipalAccessor"/> 接口，通过 <see cref="Thread.CurrentPrincipal"/> 获取当前线程的安全主体，
    /// 同时作为单例依赖项注册到依赖注入容器中。
    /// </summary>
    /// <remarks>
    /// 此实现依赖于 .NET Framework 的线程主体模型，适用于传统的同步应用程序场景。
    /// 在异步应用程序中，可能需要考虑使用其他实现方式来确保主体信息的正确传递。
    /// </remarks>
    internal class ThreadCurrentPrincipalAccessor : ICurrentPrincipalAccessor, ISingletonDependency
    {
        /// <summary>
        /// 获取当前线程的安全主体。
        /// 通过 <see cref="Thread.CurrentPrincipal"/> 属性返回与当前执行线程关联的主体信息。
        /// </summary>
        /// <value>
        /// 当前线程的安全主体，包含用户身份标识和相关的角色权限信息。
        /// 如果当前线程没有设置主体，将返回默认的匿名主体。
        /// </value>
        public virtual IPrincipal Principal => Thread.CurrentPrincipal;
    }
}
