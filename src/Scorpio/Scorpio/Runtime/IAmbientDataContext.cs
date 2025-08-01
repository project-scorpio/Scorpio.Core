namespace Scorpio.Runtime
{
    /// <summary>
    /// 定义环境数据上下文的接口。
    /// 提供在当前执行上下文中存储和检索键值对数据的能力，
    /// 支持跨异步调用链的数据传递。
    /// </summary>
    public interface IAmbientDataContext
    {
        /// <summary>
        /// 设置指定键的环境数据值。
        /// 将数据存储在当前执行上下文中，使其在相同上下文及其子上下文中可访问。
        /// </summary>
        /// <param name="key">数据的唯一标识符，用于后续检索数据</param>
        /// <param name="value">要存储的数据值，可以为 null</param>
        void SetData(string key, object value);

        /// <summary>
        /// 获取指定键对应的环境数据值。
        /// 从当前执行上下文中检索之前存储的数据。
        /// </summary>
        /// <param name="key">数据的唯一标识符</param>
        /// <returns>存储的数据值，如果指定键不存在则返回 null</returns>
        object GetData(string key);
    }
}
