#if !NET8_0_OR_GREATER
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 键控服务的静态辅助类型。
    /// 提供与 .NET 8 原生键控服务一致的任意键哨兵值，用于注册和解析任意键的兜底实现。
    /// </summary>
    public static class KeyedService
    {
        /// <summary>
        /// 存储 <see cref="AnyKey"/> 的单例哨兵对象。
        /// </summary>
        private static readonly object AnyKeyValue = new AnyKeyObject();

        /// <summary>
        /// 获取表示任意键的特殊键值。
        /// 使用该键注册的服务将作为未找到精确键注册时的兜底实现。
        /// </summary>
        /// <value>代表任意键的哨兵对象</value>
        public static object AnyKey => AnyKeyValue;

        /// <summary>
        /// 任意键哨兵对象的私有实现类型。
        /// </summary>
        private sealed class AnyKeyObject
        {
            /// <summary>
            /// 返回任意键哨兵对象的字符串表示形式。
            /// </summary>
            /// <returns>始终返回星号字符</returns>
            public override string ToString() => "*";
        }
    }
}
#endif
