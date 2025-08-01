using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化顺序特性，用于标记类的初始化优先级
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该特性用于控制实现了 <see cref="IInitializable"/> 接口的类的初始化顺序。
    /// 具有较高数值的类将优先被初始化。
    /// </para>
    /// <para>
    /// 特性只能应用于类，不允许多重应用，但支持继承。如果子类未显式标记此特性，
    /// 将继承父类的初始化顺序。
    /// </para>
    /// </remarks>
    /// <seealso cref="IInitializable"/>
    /// <seealso cref="InitializationManager"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class InitializationOrderAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="InitializationOrderAttribute"/> 类的新实例
        /// </summary>
        /// <param name="order">
        /// 初始化顺序值。数值越大，初始化优先级越高
        /// </param>
        /// <remarks>
        /// 初始化顺序按降序排列，即数值较大的类将优先被初始化。
        /// 建议使用有意义的间隔值（如10、20、30）以便后续插入新的优先级。
        /// </remarks>
        public InitializationOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        /// 获取初始化顺序值
        /// </summary>
        /// <value>
        /// 表示初始化优先级的整数值，数值越大优先级越高
        /// </value>
        public int Order { get; }

        /// <summary>
        /// 获取指定类型的初始化顺序值
        /// </summary>
        /// <param name="type">要检查的类型，不能为 <c>null</c></param>
        /// <param name="defaultOrder">
        /// 当类型未标记 <see cref="InitializationOrderAttribute"/> 时返回的默认顺序值。默认为 0
        /// </param>
        /// <returns>
        /// 返回类型的初始化顺序值。如果类型未标记此特性，则返回 <paramref name="defaultOrder"/>
        /// </returns>
        /// <remarks>
        /// 此方法通过反射检查类型是否标记了 <see cref="InitializationOrderAttribute"/> 特性，
        /// 并返回相应的顺序值。用于初始化管理器确定初始化顺序。
        /// </remarks>
        internal static int GetOrder(Type type, int defaultOrder = 0)
        {
            var attr = type.GetAttribute<InitializationOrderAttribute>();
            if (attr == null)
            {
                return defaultOrder;
            }
            return attr.Order;
        }
    }
}
