using System.Globalization;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 为所有对象提供的扩展方法集合。
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 用于简化和美化将对象转换为指定类型的过程。
        /// </summary>
        /// <typeparam name="T">要转换到的目标类型</typeparam>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的对象，如果转换失败则返回 null</returns>
        public static T As<T>(this object obj)
            where T : class => obj as T;

        /// <summary>
        /// 使用 <see cref="Convert.ChangeType(object,System.Type)"/> 方法将给定对象转换为值类型。
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <typeparam name="T">目标值类型</typeparam>
        /// <returns>转换后的值类型对象</returns>
        /// <exception cref="InvalidCastException">如果对象不能转换为指定类型时抛出</exception>
        /// <exception cref="FormatException">如果对象的值不是目标类型的有效格式时抛出</exception>
        /// <exception cref="OverflowException">如果对象表示的值超出目标类型的范围时抛出</exception>
        public static T To<T>(this object obj)
            where T : struct => (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);

        /// <summary>
        /// 对给定对象执行指定操作，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>操作后的原始对象</returns>
        public static T Action<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }

        /// <summary>
        /// 对给定对象执行指定的异步操作，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="action">要执行的异步操作</param>
        /// <returns>包含操作后的原始对象的异步任务</returns>
        public static async ValueTask<T> Action<T>(this T obj, Func<T, ValueTask> action)
        {
            await action(obj);
            return obj;
        }

        /// <summary>
        /// 根据条件对给定对象执行指定操作，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定是否执行操作的条件</param>
        /// <param name="action">条件为 true 时要执行的操作</param>
        /// <returns>操作后的原始对象</returns>
        public static T Action<T>(this T obj, bool condition, Action<T> action)
        {
            if (condition)
            {
                action(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据条件对给定对象执行指定的异步操作，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定是否执行操作的条件</param>
        /// <param name="action">条件为 true 时要执行的异步操作</param>
        /// <returns>包含操作后的原始对象的异步任务</returns>
        public static async ValueTask<T> Action<T>(this T obj, bool condition, Func<T, ValueTask> action)
        {
            if (condition)
            {
                await action(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据条件对给定对象执行两种不同操作之一，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定执行哪个操作的条件</param>
        /// <param name="trueAction">条件为 true 时要执行的操作</param>
        /// <param name="falseAction">条件为 false 时要执行的操作</param>
        /// <returns>操作后的原始对象</returns>
        public static T Action<T>(this T obj, bool condition, Action<T> trueAction, Action<T> falseAction)
        {
            if (condition)
            {
                trueAction(obj);
            }
            else
            {
                falseAction(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据条件对给定对象执行两种不同的异步操作之一，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定执行哪个操作的条件</param>
        /// <param name="trueAction">条件为 true 时要执行的异步操作</param>
        /// <param name="falseAction">条件为 false 时要执行的异步操作</param>
        /// <returns>包含操作后的原始对象的异步任务</returns>
        public static async ValueTask<T> Action<T>(this T obj, bool condition, Func<T, ValueTask> trueAction, Func<T, ValueTask> falseAction)
        {
            if (condition)
            {
                await trueAction(obj);
            }
            else
            {
                await falseAction(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据基于对象本身的条件判断，对给定对象执行指定操作，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定是否执行操作的条件函数</param>
        /// <param name="action">条件为 true 时要执行的操作</param>
        /// <returns>操作后的原始对象</returns>
        public static T Action<T>(this T obj, Func<T, bool> condition, Action<T> action)
        {
            if (condition(obj))
            {
                action(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据基于对象本身的条件判断，对给定对象执行指定的异步操作，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定是否执行操作的条件函数</param>
        /// <param name="action">条件为 true 时要执行的异步操作</param>
        /// <returns>包含操作后的原始对象的异步任务</returns>
        public static async ValueTask<T> Action<T>(this T obj, Func<T, bool> condition, Func<T, ValueTask> action)
        {
            if (condition(obj))
            {
                await action(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据基于对象本身的条件判断，对给定对象执行两种不同操作之一，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定执行哪个操作的条件函数</param>
        /// <param name="trueAction">条件为 true 时要执行的操作</param>
        /// <param name="falseAction">条件为 false 时要执行的操作</param>
        /// <returns>操作后的原始对象</returns>
        public static T Action<T>(this T obj, Func<T, bool> condition, Action<T> trueAction, Action<T> falseAction)
        {
            if (condition(obj))
            {
                trueAction(obj);
            }
            else
            {
                falseAction(obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据基于对象本身的条件判断，对给定对象执行两种不同的异步操作之一，然后返回该对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要操作的对象</param>
        /// <param name="condition">确定执行哪个操作的条件函数</param>
        /// <param name="trueAction">条件为 true 时要执行的异步操作</param>
        /// <param name="falseAction">条件为 false 时要执行的异步操作</param>
        /// <returns>包含操作后的原始对象的异步任务</returns>
        public static async ValueTask<T> Action<T>(this T obj, Func<T, bool> condition, Func<T, ValueTask> trueAction, Func<T, ValueTask> falseAction)
        {
            if (condition(obj))
            {
                await trueAction(obj);
            }
            else
            {
                await falseAction(obj);
            }
            return obj;
        }

        /// <summary>
        /// 如果对象实现了 <see cref="IDisposable"/> 接口，则调用其 <see cref="IDisposable.Dispose"/> 方法；否则不执行任何操作。
        /// </summary>
        /// <param name="obj">要安全释放的对象</param>
        public static void SafelyDispose(this object obj)
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 如果对象实现了 <see cref="IAsyncDisposable"/> 接口，则调用其 <see cref="IAsyncDisposable.DisposeAsync"/> 方法；否则不执行任何操作。
        /// </summary>
        /// <param name="obj">要安全异步释放的对象</param>
        /// <returns>表示异步释放操作的任务</returns>
        public static async ValueTask SafelyDisposeAsync(this object obj)
        {
            if (obj is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }
        }
    }
}
