using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 为方法调用接口提供运行时扩展方法的静态类
    /// </summary>
    /// <remarks>
    /// 提供异步方法检测功能，用于判断方法调用是否为异步操作
    /// </remarks>
    public static class IMethodInvocationRuntimeExtensions
    {
        /// <summary>
        /// 用于缓存方法异步状态的并发字典，提高性能避免重复检测
        /// </summary>
        private static readonly ConcurrentDictionary<MethodInfo, bool> _isAsyncCache = new ConcurrentDictionary<MethodInfo, bool>();

        /// <summary>
        /// 判断方法调用是否为异步操作
        /// </summary>
        /// <param name="invocation">方法调用上下文实例 <see cref="IMethodInvocation"/></param>
        /// <returns>如果是异步方法则返回 true，否则返回 false</returns>
        public static bool IsAsync(this IMethodInvocation invocation)
        {
            // 检查参数不为空
            Check.NotNull(invocation, nameof(invocation));

            // 从缓存中获取或计算方法的异步状态
            var isAsyncFromMetaData = _isAsyncCache.GetOrAdd(invocation.Method, IsAsyncFromMetaData);
            if (isAsyncFromMetaData)
            {
                return true;
            }

            // 检查返回值是否为异步类型
            if (invocation.ReturnValue != null)
            {
                return IsAsyncType(invocation.ReturnValue.GetType().GetTypeInfo());
            }

            return false;
        }

        /// <summary>
        /// 根据方法元数据判断是否为异步方法
        /// </summary>
        /// <param name="method">要检查的方法信息 <see cref="MethodInfo"/></param>
        /// <returns>如果方法返回类型是异步类型则返回 true，否则返回 false</returns>
        private static bool IsAsyncFromMetaData(MethodInfo method)
        {
            if (IsAsyncType(method.ReturnType.GetTypeInfo()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断类型是否为异步类型
        /// </summary>
        /// <param name="typeInfo">要检查的类型信息 <see cref="TypeInfo"/></param>
        /// <returns>如果是异步类型（Task、Task&lt;T&gt;、ValueTask、ValueTask&lt;T&gt;）则返回 true，否则返回 false</returns>
        private static bool IsAsyncType(TypeInfo typeInfo)
        {
            // 检查是否为 Task 类型
            if (typeInfo.AsType() == typeof(Task))
            {
                return true;
            }

            // 检查是否为泛型 Task<T> 类型
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return true;
            }

            // 检查是否为 ValueTask 类型
            if (typeInfo.AsType() == typeof(ValueTask))
            {
                return true;
            }

            // 检查是否为泛型 ValueTask<T> 类型
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                return true;
            }

            return false;
        }
    }
}
