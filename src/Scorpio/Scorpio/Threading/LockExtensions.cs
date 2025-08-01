using System;

namespace Scorpio.Threading
{
    /// <summary>
    /// 提供使锁定操作更简便的扩展方法。
    /// 为对象提供线程安全的锁定机制，简化同步代码的编写。
    /// </summary>
    public static class LockExtensions
    {
        /// <summary>
        /// 通过锁定指定的源对象来执行给定的操作。
        /// 在执行操作期间，源对象将被独占锁定，确保线程安全。
        /// </summary>
        /// <param name="source">要锁定的源对象</param>
        /// <param name="action">要执行的操作委托</param>
        public static void Locking(this object source, Action action)
        {
            lock (source)
            {
                action();
            }
        }

        /// <summary>
        /// 通过锁定指定的源对象来执行给定的操作。
        /// 在执行操作期间，源对象将被独占锁定，操作可以访问锁定的对象实例。
        /// </summary>
        /// <typeparam name="T">要锁定的对象类型，必须是引用类型</typeparam>
        /// <param name="source">要锁定的源对象</param>
        /// <param name="action">要执行的操作委托，接受锁定的对象作为参数</param>
        public static void Locking<T>(this T source, Action<T> action) where T : class
        {
            lock (source)
            {
                action(source);
            }
        }

        /// <summary>
        /// 通过锁定指定的源对象来执行给定的函数并返回其结果。
        /// 在执行函数期间，源对象将被独占锁定，确保线程安全。
        /// </summary>
        /// <typeparam name="TResult">函数的返回值类型</typeparam>
        /// <param name="source">要锁定的源对象</param>
        /// <param name="func">要执行的函数委托</param>
        /// <returns><paramref name="func"/> 函数的返回值</returns>
        public static TResult Locking<TResult>(this object source, Func<TResult> func)
        {
            lock (source)
            {
                return func();
            }
        }

        /// <summary>
        /// 通过锁定指定的源对象来执行给定的函数并返回其结果。
        /// 在执行函数期间，源对象将被独占锁定，函数可以访问锁定的对象实例。
        /// </summary>
        /// <typeparam name="T">要锁定的对象类型，必须是引用类型</typeparam>
        /// <typeparam name="TResult">函数的返回值类型</typeparam>
        /// <param name="source">要锁定的源对象</param>
        /// <param name="func">要执行的函数委托，接受锁定的对象作为参数</param>
        /// <returns><paramref name="func"/> 函数的返回值</returns>
        public static TResult Locking<T, TResult>(this T source, Func<T, TResult> func) where T : class
        {
            lock (source)
            {
                return func(source);
            }

        }
    }
}
