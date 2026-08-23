#if !NET8_0_OR_GREATER
using System.Collections.Generic;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控服务键的对象相等性比较器。
    /// 对 null 键做统一处理，并对非 null 键调用实例的 <c>Equals</c> 方法进行比较。
    /// </summary>
    internal sealed class KeyedServiceKeyComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// 判断两个服务键是否相等。
        /// </summary>
        /// <param name="x">第一个服务键</param>
        /// <param name="y">第二个服务键</param>
        /// <returns>如果两个服务键相等，则返回 true；否则返回 false</returns>
        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// 返回服务键的哈希代码。
        /// </summary>
        /// <param name="obj">服务键</param>
        /// <returns>服务键的哈希代码；如果服务键为 null，则返回 0</returns>
        public int GetHashCode(object obj)
            => obj?.GetHashCode() ?? 0;
    }
}
#endif
