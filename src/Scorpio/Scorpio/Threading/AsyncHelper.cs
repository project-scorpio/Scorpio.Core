using System;
using System.Reflection;
using System.Threading.Tasks;

using Nito.AsyncEx;

namespace Scorpio.Threading
{
    /// <summary>
    /// 提供用于处理异步方法的帮助方法。
    /// 包含检测异步方法、类型转换和同步执行异步操作的实用工具。
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// 检查给定的方法是否为异步方法。
        /// 通过检查方法的返回类型是否为 <see cref="Task"/> 或 <see cref="Task{TResult}"/> 来判断。
        /// </summary>
        /// <param name="method">要检查的方法信息</param>
        /// <returns>如果方法是异步方法则返回 true，否则返回 false</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="method"/> 为 null 时抛出</exception>
        public static bool IsAsync(this MethodInfo method)
        {
            Check.NotNull(method, nameof(method));
            return method.ReturnType.IsTask();
        }

        /// <summary>
        /// 检查给定的委托是否为异步方法。
        /// 通过检查委托指向的方法是否为异步方法来判断。
        /// </summary>
        /// <param name="delegate">要检查的委托</param>
        /// <returns>如果委托是异步方法则返回 true，否则返回 false</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="delegate"/> 为 null 时抛出</exception>
        public static bool IsAsync(this Delegate @delegate)
        {
            Check.NotNull(@delegate, nameof(@delegate));
            return IsAsync(@delegate.Method);
        }

        /// <summary>
        /// 检查给定的类型是否为任务类型。
        /// 支持检测 <see cref="Task"/>、<see cref="ValueTask"/>、<see cref="Task{TResult}"/> 和 <see cref="ValueTask{TResult}"/> 类型。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果类型是任务类型则返回 true，否则返回 false</returns>
        public static bool IsTask(this Type type)
        {
            return type switch
            {
                Type t when t == typeof(Task) => true,
                Type t when t == typeof(ValueTask) => true,
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>) => true,
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ValueTask<>) => true,
                _ => false
            };
        }

        /// <summary>
        /// 解包任务类型，返回其内部的实际类型。
        /// 如果给定类型是 <see cref="Task"/> 或 <see cref="ValueTask"/>，则返回 void 类型；
        /// 如果是 <see cref="Task{TResult}"/> 或 <see cref="ValueTask{TResult}"/>，则返回 TResult 类型；
        /// 否则返回原始类型。
        /// </summary>
        /// <param name="type">要解包的类型</param>
        /// <returns>解包后的实际类型</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出</exception>
        public static Type UnwrapTask(this Type type)
        {
            Check.NotNull(type, nameof(type));
            return type switch
            {
                Type t when t == typeof(Task) => typeof(void),
                Type t when t == typeof(ValueTask) => typeof(void),
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>) => t.GenericTypeArguments[0],
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ValueTask<>) => t.GenericTypeArguments[0],
                Type t => t
            };
        }

        /// <summary>
        /// 同步运行异步方法并返回结果。
        /// 使用 <see cref="AsyncContext.Run{TResult}(Func{Task{TResult}})"/> 来避免死锁。
        /// </summary>
        /// <typeparam name="TResult">异步操作的结果类型</typeparam>
        /// <param name="func">返回 <see cref="Task{TResult}"/> 的函数</param>
        /// <returns>异步操作的结果</returns>
        public static TResult RunSync<TResult>(this Func<Task<TResult>> func) => AsyncContext.Run(func);

        /// <summary>
        /// 同步运行异步方法。
        /// 使用 <see cref="AsyncContext.Run(Func{Task})"/> 来避免死锁。
        /// </summary>
        /// <param name="action">要同步执行的异步操作</param>
        public static void RunSync(this Func<Task> action) => AsyncContext.Run(action);
    }
}
