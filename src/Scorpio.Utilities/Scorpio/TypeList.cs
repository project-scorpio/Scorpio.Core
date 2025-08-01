using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace Scorpio
{
    /// <summary>
    /// 类型列表的默认实现，提供基于类型的列表存储和操作
    /// </summary>
    /// <remarks>
    /// 这是一个简化的类型列表实现，继承自 <see cref="TypeList{TBaseType}"/>，
    /// 使用 <see cref="object"/> 作为基类型，提供最大的灵活性。
    /// 所有 .NET 类型都可以添加到此列表中。
    /// </remarks>
    /// <seealso cref="TypeList{TBaseType}"/>
    /// <seealso cref="ITypeList"/>
    public class TypeList : TypeList<object>, ITypeList
    {
    }

    /// <summary>
    /// 泛型类型列表的实现，提供类型安全的类型集合存储和操作
    /// </summary>
    /// <typeparam name="TBaseType">基类型，列表中的所有类型必须继承或实现此类型</typeparam>
    /// <remarks>
    /// 此类扩展了 <see cref="List{Type}"/> 的功能，添加了对特定基类型的限制。
    /// 内部使用 <see cref="List{Type}"/> 来存储类型，并提供了类型安全的验证和便捷的泛型方法。
    /// </remarks>
    /// <seealso cref="ITypeList{TBaseType}"/>
    /// <seealso cref="List{T}"/>
    public class TypeList<TBaseType> : ITypeList<TBaseType>
    {
        /// <summary>
        /// 获取列表中包含的类型数量
        /// </summary>
        /// <value>
        /// 列表中类型的数量
        /// </value>
        public int Count => _typeList.Count;

        /// <summary>
        /// 获取一个值，该值指示列表是否为只读
        /// </summary>
        /// <value>
        /// 始终返回 <c>false</c>，因为此列表是可修改的
        /// </value>
        public bool IsReadOnly => false;

        /// <summary>
        /// 获取或设置指定索引处的类型
        /// </summary>
        /// <param name="index">要获取或设置的类型的从零开始的索引</param>
        /// <returns>指定索引处的类型</returns>
        /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="index"/> 小于 0 或大于等于 <see cref="Count"/> 时抛出</exception>
        /// <exception cref="ArgumentException">当设置的类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当设置的类型为 null 时抛出</exception>
        /// <remarks>
        /// 设置值时会验证类型是否符合泛型约束。
        /// </remarks>
        public Type this[int index]
        {
            get => _typeList[index];
            set
            {
                CheckType(value);
                _typeList[index] = value;
            }
        }

        /// <summary>
        /// 内部列表，用于存储类型集合
        /// </summary>
        /// <remarks>
        /// 使用标准的 <see cref="List{Type}"/> 来提供高效的类型存储和操作。
        /// </remarks>
        private readonly List<Type> _typeList;

        /// <summary>
        /// 初始化 <see cref="TypeList{TBaseType}"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数创建一个空的内部列表，准备接受类型的添加。
        /// </remarks>
        public TypeList() => _typeList = new List<Type>();

        /// <summary>
        /// 向列表中添加一个类型
        /// </summary>
        /// <typeparam name="T">要添加的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <remarks>
        /// 此方法提供了类型安全的方式来添加类型，避免了手动获取 <see cref="Type"/> 对象。
        /// 如果类型已存在，可能会导致重复添加。如果需要避免重复，请使用 <see cref="TryAdd{T}"/>。
        /// </remarks>
        /// <seealso cref="TryAdd{T}"/>
        public void Add<T>() where T : TBaseType => _typeList.Add(typeof(T));

        /// <summary>
        /// 向列表中添加指定的类型
        /// </summary>
        /// <param name="item">要添加的类型</param>
        /// <exception cref="ArgumentException">当类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会验证类型是否符合泛型约束，然后添加到内部列表中。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Add"/>
        public void Add(Type item)
        {
            CheckType(item);
            _typeList.Add(item);
        }

        /// <summary>
        /// 尝试向列表中添加一个类型，如果类型不存在则添加
        /// </summary>
        /// <typeparam name="T">要添加的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <returns>如果成功添加返回 <c>true</c>；如果类型已存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了安全的添加方式，当类型已存在时不会重复添加，而是返回 <c>false</c>。
        /// 这在不确定类型是否已存在的情况下非常有用，可以避免重复条目。
        /// 首先检查类型是否已存在，如果不存在则调用 <see cref="Add{T}"/> 方法添加。
        /// </remarks>
        /// <seealso cref="Add{T}"/>
        /// <seealso cref="Contains{T}"/>
        public bool TryAdd<T>() where T : TBaseType
        {
            if (Contains<T>())
            {
                return false;
            }

            Add<T>();
            return true;
        }

        /// <summary>
        /// 将类型插入列表中指定索引处
        /// </summary>
        /// <param name="index">应插入 <paramref name="item"/> 的从零开始的索引</param>
        /// <param name="item">要插入的类型</param>
        /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="index"/> 小于 0 或大于 <see cref="Count"/> 时抛出</exception>
        /// <exception cref="ArgumentException">当类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会先验证类型，然后将其插入到指定位置。
        /// 插入点之后的类型索引会增加 1。
        /// </remarks>
        /// <seealso cref="IList{T}.Insert"/>
        public void Insert(int index, Type item)
        {
            CheckType(item);
            _typeList.Insert(index, item);
        }

        /// <summary>
        /// 搜索指定的类型，并返回整个列表中第一个匹配项的从零开始的索引
        /// </summary>
        /// <param name="item">要在列表中定位的类型</param>
        /// <returns>如果找到 <paramref name="item"/>，则为整个列表中第一个匹配项的从零开始的索引；否则为 -1</returns>
        /// <exception cref="ArgumentException">当类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会先验证类型，然后在内部列表中搜索。
        /// </remarks>
        /// <seealso cref="IList{T}.IndexOf"/>
        public int IndexOf(Type item)
        {
            CheckType(item);
            return _typeList.IndexOf(item);
        }

        /// <summary>
        /// 检查指定的类型是否存在于列表中
        /// </summary>
        /// <typeparam name="T">要检查的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <returns>如果类型存在返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来检查类型是否已在列表中。
        /// 内部调用 <see cref="Contains(Type)"/> 方法。
        /// </remarks>
        /// <seealso cref="Contains(Type)"/>
        public bool Contains<T>() where T : TBaseType => Contains(typeof(T));

        /// <summary>
        /// 确定列表是否包含指定的类型
        /// </summary>
        /// <param name="item">要在列表中定位的类型</param>
        /// <returns>如果在列表中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentException">当类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会先验证类型，然后在内部列表中搜索。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Contains"/>
        public bool Contains(Type item)
        {
            CheckType(item);
            return _typeList.Contains(item);
        }

        /// <summary>
        /// 从列表中移除指定的类型
        /// </summary>
        /// <typeparam name="T">要移除的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <returns>如果成功移除返回 <c>true</c>；如果类型不存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来移除类型。
        /// 如果列表中有多个相同的类型实例，只会移除第一个找到的实例。
        /// 内部调用 <see cref="ICollection{T}.Remove"/> 方法。
        /// </remarks>
        /// <seealso cref="Remove(Type)"/>
        public bool Remove<T>() where T : TBaseType => _typeList.Remove(typeof(T));

        /// <summary>
        /// 从列表中移除特定类型的第一个匹配项
        /// </summary>
        /// <param name="item">要从列表中移除的类型</param>
        /// <returns>如果成功移除 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentException">当类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会先验证类型，然后尝试从内部列表中移除。
        /// 如果有多个相同类型的实例，只移除第一个找到的。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Remove"/>
        public bool Remove(Type item)
        {
            CheckType(item);
            return _typeList.Remove(item);
        }

        /// <summary>
        /// 移除指定索引处的类型
        /// </summary>
        /// <param name="index">要移除的类型的从零开始的索引</param>
        /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="index"/> 小于 0 或大于等于 <see cref="Count"/> 时抛出</exception>
        /// <remarks>
        /// 移除指定索引的类型后，后续类型的索引会减少 1。
        /// </remarks>
        /// <seealso cref="IList{T}.RemoveAt"/>
        public void RemoveAt(int index) => _typeList.RemoveAt(index);

        /// <summary>
        /// 从列表中移除所有类型
        /// </summary>
        /// <remarks>
        /// 此操作会清空列表，使其成为空列表。
        /// <see cref="Count"/> 将变为 0。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Clear"/>
        public void Clear() => _typeList.Clear();

        /// <summary>
        /// 从特定的数组索引开始，将列表的元素复制到数组中
        /// </summary>
        /// <param name="array">作为从列表复制的元素的目标的一维数组</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="array"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="arrayIndex"/> 小于 0 时抛出</exception>
        /// <exception cref="ArgumentException">当目标数组空间不足时抛出</exception>
        /// <remarks>
        /// 此方法将列表中的所有类型复制到指定的数组中。
        /// </remarks>
        /// <seealso cref="ICollection{T}.CopyTo"/>
        public void CopyTo(Type[] array, int arrayIndex) => _typeList.CopyTo(array, arrayIndex);

        /// <summary>
        /// 返回循环访问列表的枚举器
        /// </summary>
        /// <returns>用于列表的 <see cref="IEnumerator{Type}"/></returns>
        /// <remarks>
        /// 此方法支持在 foreach 语句中使用列表。
        /// </remarks>
        /// <seealso cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<Type> GetEnumerator() => _typeList.GetEnumerator();

        /// <summary>
        /// 返回循环访问集合的枚举器
        /// </summary>
        /// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象</returns>
        /// <remarks>
        /// 此方法是 <see cref="IEnumerable"/> 接口的显式实现。
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => _typeList.GetEnumerator();

        /// <summary>
        /// 验证类型是否符合基类型约束
        /// </summary>
        /// <param name="item">要验证的类型</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当类型不符合 <typeparamref name="TBaseType"/> 约束时抛出</exception>
        /// <remarks>
        /// 此方法使用反射检查类型兼容性，确保类型安全。
        /// 所有对外公开的方法在操作类型前都会调用此方法进行验证。
        /// </remarks>
        private static void CheckType(Type item)
        {
            Check.NotNull(item, nameof(item));
            if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
            {
                throw new ArgumentException($"给定的类型 ({item.AssemblyQualifiedName}) 应该是 {typeof(TBaseType).AssemblyQualifiedName} 的实例", nameof(item));
            }
        }
    }
}
