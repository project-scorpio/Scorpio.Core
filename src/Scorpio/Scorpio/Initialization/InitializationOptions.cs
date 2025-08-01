using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Initialization
{
    /// <summary>
    /// 初始化配置选项类，管理所有可初始化类型的注册和排序
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该类用于配置和管理系统中所有实现了 <see cref="IInitializable"/> 接口的类型，
    /// 并按照指定的优先级顺序进行排序。
    /// </para>
    /// <para>
    /// 内部使用 <see cref="SortedDictionary{TKey, TValue}"/> 按降序存储初始化类型，
    /// 确保高优先级的类型优先被初始化。
    /// </para>
    /// </remarks>
    /// <seealso cref="IInitializable"/>
    /// <seealso cref="InitializationOrderAttribute"/>
    public sealed class InitializationOptions
    {
        /// <summary>
        /// 初始化 <see cref="InitializationOptions"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数创建一个按降序排列的 <see cref="SortedDictionary{TKey, TValue}"/>，
        /// 确保初始化顺序值较大的类型优先被处理。
        /// </remarks>
        public InitializationOptions()
        {
            Initializables = new SortedDictionary<int, ITypeList<IInitializable>>(Comparer<int>.Create((x, y) => y.CompareTo(x)));
        }

        /// <summary>
        /// 获取按优先级排序的可初始化类型集合
        /// </summary>
        /// <value>
        /// 键为初始化顺序值，值为该优先级下的类型列表的有序字典
        /// </value>
        /// <remarks>
        /// 该字典按键值降序排列，确保高优先级（大数值）的类型组优先被初始化。
        /// </remarks>
        internal SortedDictionary<int, ITypeList<IInitializable>> Initializables { get; }

        /// <summary>
        /// 添加指定类型的可初始化类型到配置中
        /// </summary>
        /// <typeparam name="T">
        /// 要添加的类型，必须实现 <see cref="IInitializable"/> 接口
        /// </typeparam>
        /// <param name="order">
        /// 该类型的初始化顺序值，数值越大优先级越高
        /// </param>
        /// <remarks>
        /// 如果指定的顺序值不存在，将创建新的类型列表；
        /// 如果已存在，则将类型添加到现有列表中。
        /// </remarks>
        public void AddInitializable<T>(int order) where T : IInitializable
        {
            Initializables.GetOrAdd(order, i => new TypeList<IInitializable>()).Add<T>();
        }

        /// <summary>
        /// 添加指定类型的可初始化类型到配置中
        /// </summary>
        /// <param name="type">
        /// 要添加的类型，必须实现 <see cref="IInitializable"/> 接口。不能为 <c>null</c>
        /// </param>
        /// <param name="order">
        /// 该类型的初始化顺序值，数值越大优先级越高
        /// </param>
        /// <exception cref="ArgumentException">
        /// 当 <paramref name="type"/> 未实现 <see cref="IInitializable"/> 接口时抛出
        /// </exception>
        /// <remarks>
        /// 此方法会验证类型是否实现了 <see cref="IInitializable"/> 接口，
        /// 如果验证失败将抛出异常。
        /// </remarks>
        public void AddInitializable(Type type, int order)
        {
            _ = Check.AssignableTo<IInitializable>(type, nameof(type));
            Initializables.GetOrAdd(order, i => new TypeList<IInitializable>()).Add(type);
        }
    }
}
