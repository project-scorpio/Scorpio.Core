using System;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 提供一个不执行任何操作的 <see cref="IDisposable"/> 实现。
    /// </summary>
    /// <remarks>
    /// 此类采用单例模式，可通过 <see cref="Instance"/> 属性获取唯一实例。
    /// 当需要返回 <see cref="IDisposable"/> 对象但不需要执行实际释放操作时，可使用此类。
    /// </remarks>
    public sealed class NullDisposable : IDisposable
    {
        /// <summary>
        /// 获取 <see cref="NullDisposable"/> 类的单例实例。
        /// </summary>
        /// <value>
        /// <see cref="NullDisposable"/> 类的唯一实例。
        /// </value>
        public static NullDisposable Instance { get; } = new NullDisposable();

        /// <summary>
        /// 防止创建 <see cref="NullDisposable"/> 类的新实例的私有构造函数。
        /// </summary>
        private NullDisposable()
        {
            // 私有构造函数，防止外部创建实例
        }

        /// <summary>
        /// 实现 <see cref="IDisposable.Dispose"/> 方法，但不执行任何操作。
        /// </summary>
        /// <remarks>
        /// 此方法故意留空，不执行任何释放资源的操作。
        /// </remarks>
        public void Dispose()
        {
            // Method intentionally left empty.
        }
    }

    /// <summary>
    /// 提供一个不执行任何操作的 <see cref="IAsyncDisposable"/> 实现。
    /// </summary>
    /// <remarks>
    /// 此类采用单例模式，可通过 <see cref="Instance"/> 属性获取唯一实例。
    /// 当需要返回 <see cref="IAsyncDisposable"/> 对象但不需要执行实际异步释放操作时，可使用此类。
    /// </remarks>
    public sealed class NullAsyncDispose : IAsyncDisposable
    {
        /// <summary>
        /// 获取 <see cref="NullAsyncDispose"/> 类的单例实例。
        /// </summary>
        /// <value>
        /// <see cref="NullAsyncDispose"/> 类的唯一实例。
        /// </value>
        public static readonly NullAsyncDispose Instance = new NullAsyncDispose();

        /// <summary>
        /// 实现 <see cref="IAsyncDisposable.DisposeAsync"/> 方法，返回一个已完成的 <see cref="ValueTask"/>。
        /// </summary>
        /// <returns>一个表示异步处理操作的 <see cref="ValueTask"/>，该任务已完成。</returns>
        /// <remarks>
        /// 此方法不执行任何实际的资源释放操作，仅返回一个已完成的任务。
        /// </remarks>
        public ValueTask DisposeAsync() => new ValueTask();
    }
}
