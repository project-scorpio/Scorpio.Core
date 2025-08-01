using System;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 提供一种在对象被释放时执行指定操作的机制。
    /// </summary>
    /// <remarks>
    /// 此类实现 <see cref="IDisposable"/> 接口，允许在 using 语句块结束时自动执行指定的操作。
    /// </remarks>
    public sealed class DisposeAction : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        /// 初始化 <see cref="DisposeAction"/> 类的新实例。
        /// </summary>
        /// <param name="action">在对象被释放时要执行的操作</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> 为 null 时抛出</exception>
        public DisposeAction(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        #region IDisposable Support
        private bool _disposedValue = false; // 用于检测冗余的调用

        /// <summary>
        /// 释放由 <see cref="DisposeAction"/> 使用的非托管资源，并执行构造时指定的操作。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，则为 true；否则为 false</param>
        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _action();
                }
                _disposedValue = true;
            }
        }
        
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() => Dispose(true);
        #endregion
    }

    /// <summary>
    /// 提供一种在对象被异步释放时执行指定异步操作的机制。
    /// </summary>
    /// <remarks>
    /// 此类实现 <see cref="IAsyncDisposable"/> 接口，允许在 await using 语句块结束时自动执行指定的异步操作。
    /// </remarks>
    public sealed class AsyncDisposeAction : IAsyncDisposable
    {
        private readonly Func<ValueTask> _action;

        /// <summary>
        /// 初始化 <see cref="AsyncDisposeAction"/> 类的新实例。
        /// </summary>
        /// <param name="action">在对象被异步释放时要执行的异步操作</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> 为 null 时抛出</exception>
        public AsyncDisposeAction(Func<ValueTask> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        #region IDisposable Support
        private bool _disposedValue = false; // 用于检测冗余的调用

        /// <summary>
        /// 异步释放由 <see cref="AsyncDisposeAction"/> 使用的非托管资源，并执行构造时指定的异步操作。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，则为 true；否则为 false</param>
        /// <returns>表示异步处理操作的任务</returns>
        private async ValueTask DisposeCoreAsync(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    await _action();
                }
                _disposedValue = true;
            }
        }
        
        /// <summary>
        /// 异步执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <returns>表示异步处理操作的任务</returns>
        public ValueTask DisposeAsync() => DisposeCoreAsync(true);
        #endregion
    }
}
