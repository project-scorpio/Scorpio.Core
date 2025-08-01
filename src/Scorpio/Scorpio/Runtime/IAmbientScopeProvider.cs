using System;

namespace Scorpio.Runtime
{
    /// <summary>
    /// 定义环境作用域提供程序的接口。
    /// 提供创建和管理特定类型环境作用域的能力，支持作用域嵌套和自动清理。
    /// </summary>
    /// <typeparam name="T">环境作用域中存储的数据类型</typeparam>
    public interface IAmbientScopeProvider<T>
    {
        /// <summary>
        /// 获取指定上下文键对应的当前作用域值。
        /// 从当前活动的环境作用域中检索存储的数据。
        /// </summary>
        /// <param name="contextKey">上下文键，用于标识特定的环境数据</param>
        /// <returns>当前作用域中存储的值，如果不存在则返回 <typeparamref name="T"/> 的默认值</returns>
        T GetValue(string contextKey);

        /// <summary>
        /// 开始一个新的环境作用域，并在其中存储指定的值。
        /// 新作用域会嵌套在当前作用域之上，形成作用域链结构。
        /// </summary>
        /// <param name="contextKey">上下文键，用于标识特定的环境数据</param>
        /// <param name="value">要在新作用域中存储的值</param>
        /// <returns>一个 <see cref="IDisposable"/> 对象，释放时会结束当前作用域并恢复到上一层作用域</returns>
        IDisposable BeginScope(string contextKey, T value);
    }
}
