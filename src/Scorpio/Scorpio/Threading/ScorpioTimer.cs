using System;
using System.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Scorpio.DependencyInjection;

namespace Scorpio.Threading
{
    /// <summary>
    /// 健壮的计时器实现，确保不会发生重叠执行。
    /// 实现 <see cref="IScorpioTimer"/> 接口，在每次触发之间精确等待指定的 <see cref="Period"/> 时间间隔。
    /// </summary>
    public class ScorpioTimer : ITransientDependency, IDisposable, IScorpioTimer
    {
        /// <summary>
        /// 根据计时器的 <see cref="Period"/> 周期性触发的事件。
        /// 此事件在每个时间间隔结束时被引发，用于执行定时任务。
        /// </summary>
        public event EventHandler Elapsed;

        /// <summary>
        /// 获取或设置计时器的任务执行周期（以毫秒为单位）。
        /// 定义了 <see cref="Elapsed"/> 事件触发的时间间隔。
        /// </summary>
        /// <value>计时器的执行周期，单位为毫秒</value>
        public int Period { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示计时器是否在调用 <see cref="Start"/> 方法时立即触发一次 <see cref="Elapsed"/> 事件。
        /// </summary>
        /// <value>
        /// 如果计时器在启动时立即执行一次则为 true；否则为 false。
        /// 默认值为 false。
        /// </value>
        public bool RunOnStart { get; set; }

        /// <summary>
        /// 获取或设置用于记录计时器消息的 <see cref="ILogger{TCategoryName}"/>。
        /// </summary>
        /// <value>用于记录计时器消息的 <see cref="ILogger{TCategoryName}"/> 实例</value>
        public ILogger<ScorpioTimer> Logger { get; set; }

        /// <summary>
        /// 内部使用的 .NET <see cref="Timer"/> 实例
        /// </summary>
        private readonly Timer _taskTimer;

        /// <summary>
        /// 指示计时器是否正在执行任务的易失性标志
        /// </summary>
        private volatile bool _performingTasks;

        /// <summary>
        /// 指示计时器是否正在运行的易失性标志
        /// </summary>
        private volatile bool _isRunning;

        /// <summary>
        /// 指示对象是否已被释放的标志
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        /// 使用无限期间和无限到期时间初始化 <see cref="ScorpioTimer"/> 类的新实例。
        /// </summary>
        public ScorpioTimer()
        {
            Logger = NullLogger<ScorpioTimer>.Instance;

            _taskTimer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 开始触发 <see cref="Elapsed"/> 事件。
        /// 启动计时器，使其按照指定的 <see cref="Period"/> 周期性地引发事件。
        /// </summary>
        /// <exception cref="ScorpioException">当 <see cref="Period"/> 小于或等于 0 时抛出</exception>
        public void Start()
        {
            if (Period <= 0)
            {
                throw new ScorpioException("Period should be set before starting the timer!");
            }

            // 使用锁定扩展方法确保线程安全
            _taskTimer.Locking(t =>
            {
                // 根据 RunOnStart 属性决定首次触发时间
                t.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
                _isRunning = true;
            });
        }

        /// <summary>
        /// 停止触发 <see cref="Elapsed"/> 事件。
        /// 停止计时器的运行，并等待当前正在执行的任务完成。
        /// </summary>
        public void Stop()
        {
            _taskTimer.Locking(t =>
            {
                // 停止计时器
                t.Change(Timeout.Infinite, Timeout.Infinite);

                // 等待当前任务完成
                while (_performingTasks)
                {
                    Monitor.Wait(t);
                }

                _isRunning = false;
            });
        }

        /// <summary>
        /// 计时器回调方法，由内部 <see cref="_taskTimer"/> 调用。
        /// 负责触发 <see cref="Elapsed"/> 事件并确保不重叠执行。
        /// </summary>
        /// <param name="state">未使用的参数</param>
        private void TimerCallBack(object state)
        {
            // 检查是否应该执行任务
            _taskTimer.Locking(t =>
            {
                if (!_isRunning || _performingTasks)
                {
                    return;
                }

                // 暂停计时器并标记正在执行任务
                t.Change(Timeout.Infinite, Timeout.Infinite);
                _performingTasks = true;
            });

            try
            {
                // 触发 Elapsed 事件
                Elapsed?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // 忽略事件处理程序中的异常
                Logger.LogError(ex, "An error occurred while executing the Elapsed event handlers.");
            }
            finally
            {
                // 重置状态并重新启动计时器
                _taskTimer.Locking(t =>
                {
                    _performingTasks = false;
                    if (_isRunning)
                    {
                        // 重新设置计时器以等待下一个周期
                        t.Change(Period, Timeout.Infinite);
                    }

                    // 通知等待的线程
                    Monitor.Pulse(_taskTimer);
                });
            }
        }

        /// <summary>
        /// 释放 <see cref="ScorpioTimer"/> 使用的非托管资源，
        /// 并可选择释放托管资源。
        /// </summary>
        /// <param name="disposing">
        /// 如果为 true，则释放托管和非托管资源；
        /// 如果为 false，则仅释放非托管资源
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // 停止计时器并释放托管资源
                    Stop();
                    _taskTimer.Dispose();
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// 释放当前 <see cref="ScorpioTimer"/> 实例使用的所有资源。
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入 'Dispose(bool disposing)' 方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
