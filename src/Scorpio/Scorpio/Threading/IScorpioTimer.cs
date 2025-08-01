using System;

namespace Scorpio.Threading
{
    /// <summary>
    /// 定义健壮的计时器接口，确保不会发生重叠执行。
    /// 在每次触发之间精确等待指定的 <see cref="Period"/> 时间间隔。
    /// </summary>
    public interface IScorpioTimer
    {
        /// <summary>
        /// 获取或设置计时器的任务执行周期（以毫秒为单位）。
        /// 定义了 <see cref="Elapsed"/> 事件触发的时间间隔。
        /// </summary>
        /// <value>计时器的执行周期，单位为毫秒</value>
        int Period { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示计时器是否在调用 <see cref="Start"/> 方法时立即触发一次 <see cref="Elapsed"/> 事件。
        /// </summary>
        /// <value>
        /// 如果计时器在启动时立即执行一次则为 true；否则为 false。
        /// 默认值为 false。
        /// </value>
        bool RunOnStart { get; set; }

        /// <summary>
        /// 根据计时器的 <see cref="Period"/> 周期性触发的事件。
        /// 此事件在每个时间间隔结束时被引发，用于执行定时任务。
        /// </summary>
        event EventHandler Elapsed;

        /// <summary>
        /// 开始触发 <see cref="Elapsed"/> 事件。
        /// 启动计时器，使其按照指定的 <see cref="Period"/> 周期性地引发事件。
        /// </summary>
        void Start();

        /// <summary>
        /// 停止触发 <see cref="Elapsed"/> 事件。
        /// 停止计时器的运行，不再引发周期性事件。
        /// </summary>
        void Stop();
    }
}