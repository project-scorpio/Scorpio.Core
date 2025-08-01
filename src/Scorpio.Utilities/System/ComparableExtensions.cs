using System;
using System.Linq;

namespace System
{
    /// <summary>
    /// <see cref="IComparable{T}"/> 的扩展方法集合。
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// 检查一个值是否在指定的最小值和最大值之间（包含边界值）。
        /// </summary>
        /// <typeparam name="T">要比较的值的类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="minInclusiveValue">最小值（包含）</param>
        /// <param name="maxInclusiveValue">最大值（包含）</param>
        /// <returns>如果值在指定范围内（包含边界值），则返回 true；否则返回 false</returns>
        public static bool IsBetween<T>(this T value, T minInclusiveValue, T maxInclusiveValue) where T : IComparable<T> 
            => value.CompareTo(minInclusiveValue) >= 0 && value.CompareTo(maxInclusiveValue) <= 0;

        /// <summary>
        /// 检查一个项是否在指定列表中。
        /// </summary>
        /// <typeparam name="T">项的类型</typeparam>
        /// <param name="item">要检查的项</param>
        /// <param name="list">项的列表</param>
        /// <returns>如果项在列表中，则返回 true；否则返回 false</returns>
        public static bool IsIn<T>(this T item, params T[] list) => list.Contains(item);
    }
}
