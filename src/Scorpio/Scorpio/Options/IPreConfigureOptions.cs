namespace Scorpio.Options
{
    /// <summary>
    /// 表示用于在配置选项之前进行预配置的接口。
    /// 此接口允许在选项被完全配置之前对其进行初始化或预设值。
    /// </summary>
    /// <typeparam name="TOptions">要预配置的选项类型，必须是引用类型</typeparam>
    public interface IPreConfigureOptions<in TOptions> where TOptions : class
    {
        /// <summary>
        /// 对指定名称的选项实例进行预配置。
        /// 此方法在选项的正式配置流程之前被调用，用于设置默认值或执行初始化逻辑。
        /// </summary>
        /// <param name="name">选项配置的名称标识符，用于区分不同的配置实例</param>
        /// <param name="options">要进行预配置的选项实例，类型为 <typeparamref name="TOptions"/></param>
        void PreConfigure(string name, TOptions options);
    }
}
