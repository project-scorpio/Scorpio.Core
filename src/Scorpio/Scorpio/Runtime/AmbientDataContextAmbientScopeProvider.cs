using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Scorpio.Runtime
{
    /// <summary>
    /// 基于环境数据上下文的环境作用域提供程序。
    /// 实现 <see cref="IAmbientScopeProvider{T}"/> 接口，用于管理特定类型的环境作用域数据。
    /// </summary>
    /// <typeparam name="T">环境作用域中存储的数据类型</typeparam>
    internal class AmbientDataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
    {
        /// <summary>
        /// 用于存储作用域项的线程安全字典。
        /// 键为作用域项的唯一标识符，值为作用域项实例。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "<挂起>")]
        private static readonly ConcurrentDictionary<string, ScopeItem> _scopeDictionary = new ConcurrentDictionary<string, ScopeItem>();

        /// <summary>
        /// 环境数据上下文，用于在当前执行上下文中存储和检索数据。
        /// </summary>
        private readonly IAmbientDataContext _dataContext;

        /// <summary>
        /// 初始化 <see cref="AmbientDataContextAmbientScopeProvider{T}"/> 类的新实例。
        /// </summary>
        /// <param name="dataContext">环境数据上下文实例，用于管理上下文数据</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="dataContext"/> 为 null 时抛出</exception>
        public AmbientDataContextAmbientScopeProvider(IAmbientDataContext dataContext)
        {
            Check.NotNull(dataContext, nameof(dataContext));

            _dataContext = dataContext;

        }

        /// <summary>
        /// 获取指定上下文键对应的当前作用域值。
        /// </summary>
        /// <param name="contextKey">上下文键，用于标识特定的环境数据</param>
        /// <returns>当前作用域中存储的值，如果不存在则返回 <typeparamref name="T"/> 的默认值</returns>
        public T GetValue(string contextKey)
        {
            var item = GetCurrentItem(contextKey);
            if (item == null)
            {
                return default;
            }

            return item.Value;
        }

        /// <summary>
        /// 开始一个新的环境作用域，并在其中存储指定的值。
        /// 新作用域会嵌套在当前作用域之上，形成作用域链。
        /// </summary>
        /// <param name="contextKey">上下文键，用于标识特定的环境数据</param>
        /// <param name="value">要在新作用域中存储的值</param>
        /// <returns>一个 <see cref="IDisposable"/> 对象，释放时会结束当前作用域并恢复到上一层作用域</returns>
        /// <exception cref="ScorpioException">当无法将作用域项添加到字典中时抛出</exception>
        public IDisposable BeginScope(string contextKey, T value)
        {
            // 创建新的作用域项，将当前项设为外层作用域
            var item = new ScopeItem(value, GetCurrentItem(contextKey));
            if (!_scopeDictionary.TryAdd(item.Id, item))
            {
                throw new ScorpioException("Can not add item! ScopeDictionary.TryAdd returns false!");
            }

            // 在数据上下文中设置当前作用域项的ID
            _dataContext.SetData(contextKey, item.Id);

            // 返回释放操作，用于清理作用域
            return new DisposeAction(() =>
            {
                // 从字典中移除当前作用域项
                _scopeDictionary.TryRemove(item.Id, out item);

                // 如果没有外层作用域，清空上下文数据
                if (item.Outer == null)
                {
                    _dataContext.SetData(contextKey, null);
                    return;
                }

                // 恢复到外层作用域
                _dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        /// <summary>
        /// 获取指定上下文键对应的当前作用域项。
        /// </summary>
        /// <param name="contextKey">上下文键，用于标识特定的环境数据</param>
        /// <returns>当前作用域项，如果不存在则返回 null</returns>
        private ScopeItem GetCurrentItem(string contextKey) => _dataContext.GetData(contextKey) is string objKey ? _scopeDictionary.GetOrDefault(objKey) : null;

        /// <summary>
        /// 表示环境作用域中的一个数据项。
        /// 包含值、唯一标识符以及对外层作用域的引用，形成作用域链结构。
        /// </summary>
        private sealed class ScopeItem
        {
            /// <summary>
            /// 获取作用域项的唯一标识符。
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// 获取外层作用域项的引用。
            /// 如果为 null，表示这是最外层的作用域。
            /// </summary>
            public ScopeItem Outer { get; }

            /// <summary>
            /// 获取存储在此作用域项中的值。
            /// </summary>
            public T Value { get; }

            /// <summary>
            /// 初始化 <see cref="ScopeItem"/> 类的新实例。
            /// </summary>
            /// <param name="value">要存储在作用域中的值</param>
            /// <param name="outer">外层作用域项，如果为 null 则表示这是顶层作用域</param>
            public ScopeItem(T value, ScopeItem outer = null)
            {
                // 生成唯一标识符
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }
        }
    }
}
