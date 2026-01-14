using System.Threading.Tasks;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 拦截器接口，定义了方法拦截的基础功能
    /// </summary>
    /// <remarks>
    /// 实现此接口的类可以在方法调用前后执行自定义逻辑，用于AOP（面向切面编程）场景
    /// </remarks>
    public interface IInterceptor
    {
        /// <summary>
        /// 异步拦截方法调用
        /// </summary>
        /// <param name="invocation">方法调用信息，包含方法参数、返回值等详细信息 <see cref="IMethodInvocation"/></param>
        /// <returns>表示异步操作的任务 <see cref="Task"/></returns>
        Task InterceptAsync(IMethodInvocation invocation);
    }
}