using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Scorpio
{
    /// <summary>
    /// 类型字典的默认实现，提供基于类型的键值对存储和操作
    /// </summary>
    /// <remarks>
    /// 这是一个简化的类型字典实现，继承自 <see cref="TypeDictionary{TBaseKeyType, TBaseValueType}"/>，
    /// 其中键类型和值类型都使用 <see cref="object"/> 作为基类型，提供最大的灵活性。
    /// 所有 .NET 类型都可以作为键或值添加到此字典中。
    /// </remarks>
    /// <seealso cref="TypeDictionary{TBaseKeyType, TBaseValueType}"/>
    /// <seealso cref="ITypeDictionary"/>
    public class TypeDictionary : TypeDictionary<object, object>, ITypeDictionary
    {

    }

    /// <summary>
    /// 泛型类型字典的实现，提供类型安全的键值对存储和操作
    /// </summary>
    /// <typeparam name="TBaseKeyType">键类型的基类型，所有键类型必须继承或实现此类型</typeparam>
    /// <typeparam name="TBaseValueType">值类型的基类型，所有值类型必须继承或实现此类型</typeparam>
    /// <remarks>
    /// 此类实现了 <see cref="ITypeDictionary{TBaseKeyType, TBaseValueType}"/> 接口，
    /// 内部使用 <see cref="Dictionary{Type, Type}"/> 来存储类型映射关系，
    /// 并提供了类型安全的验证和便捷的泛型方法。
    /// </remarks>
    /// <seealso cref="ITypeDictionary{TBaseKeyType, TBaseValueType}"/>
    /// <seealso cref="Dictionary{TKey, TValue}"/>
    public class TypeDictionary<TBaseKeyType, TBaseValueType> : ITypeDictionary<TBaseKeyType, TBaseValueType>
    {
        /// <summary>
        /// 内部字典，用于存储类型到类型的映射关系
        /// </summary>
        /// <remarks>
        /// 使用标准的 <see cref="Dictionary{Type, Type}"/> 来提供高效的键值对存储和查找。
        /// </remarks>
        private readonly Dictionary<Type, Type> _pairs;

        /// <summary>
        /// 初始化 <see cref="TypeDictionary{TBaseKeyType, TBaseValueType}"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数创建一个空的内部字典，准备接受类型映射的添加。
        /// </remarks>
        public TypeDictionary() => _pairs = new Dictionary<Type, Type>();

        /// <summary>
        /// 获取或设置指定键类型对应的值类型
        /// </summary>
        /// <param name="key">作为键的类型</param>
        /// <returns>与指定键关联的值类型</returns>
        /// <exception cref="KeyNotFoundException">当指定的键不存在时抛出</exception>
        /// <exception cref="ArgumentException">当键或值类型不符合基类型约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 设置值时会验证键和值类型是否符合泛型约束。
        /// 获取值时如果键不存在会抛出 <see cref="KeyNotFoundException"/>。
        /// </remarks>
        public Type this[Type key]
        {
            get => _pairs[key];
            set
            {
                TypeDictionary<TBaseKeyType, TBaseValueType>.CheckKeyType(key);
                TypeDictionary<TBaseKeyType, TBaseValueType>.CheckValueType(value);
                _pairs[key] = value;
            }
        }

        /// <summary>
        /// 获取包含字典中所有键类型的集合
        /// </summary>
        /// <value>
        /// 包含字典中所有键类型的 <see cref="ICollection{Type}"/>
        /// </value>
        /// <remarks>
        /// 返回的集合是只读的，反映字典的当前状态。
        /// </remarks>
        public ICollection<Type> Keys => _pairs.Keys;

        /// <summary>
        /// 获取包含字典中所有值类型的集合
        /// </summary>
        /// <value>
        /// 包含字典中所有值类型的 <see cref="ICollection{Type}"/>
        /// </value>
        /// <remarks>
        /// 返回的集合是只读的，反映字典的当前状态。
        /// </remarks>
        public ICollection<Type> Values => _pairs.Values;

        /// <summary>
        /// 获取字典中包含的键值对数量
        /// </summary>
        /// <value>
        /// 字典中键值对的数量
        /// </value>
        public int Count => _pairs.Count;

        /// <summary>
        /// 获取一个值，该值指示字典是否为只读
        /// </summary>
        /// <value>
        /// 始终返回 <c>false</c>，因为此字典是可修改的
        /// </value>
        public bool IsReadOnly => false;

        /// <summary>
        /// 向字典中添加一个类型映射关系
        /// </summary>
        /// <typeparam name="TKeyType">键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <typeparam name="TValueType">值类型，必须继承或实现 <typeparamref name="TBaseValueType"/></typeparam>
        /// <exception cref="ArgumentException">当键类型已存在时抛出</exception>
        /// <remarks>
        /// 此方法提供了类型安全的方式来添加类型映射，避免了手动获取 <see cref="Type"/> 对象。
        /// 如果键类型已存在，将抛出异常。如果需要尝试添加而不抛出异常，请使用 <see cref="TryAdd{TKeyType, TValueType}"/>。
        /// </remarks>
        /// <seealso cref="TryAdd{TKeyType, TValueType}"/>
        public void Add<TKeyType, TValueType>()
            where TKeyType : TBaseKeyType
            where TValueType : TBaseValueType => Add(typeof(TKeyType), typeof(TValueType));

        /// <summary>
        /// 向字典中添加指定的键类型和值类型
        /// </summary>
        /// <param name="key">要添加的键类型</param>
        /// <param name="value">要添加的值类型</param>
        /// <exception cref="ArgumentException">当键类型已存在或类型不符合约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会验证键和值类型是否符合泛型约束，然后添加到内部字典中。
        /// </remarks>
        public void Add(Type key, Type value)
        {
            TypeDictionary<TBaseKeyType, TBaseValueType>.CheckKeyType(key);
            TypeDictionary<TBaseKeyType, TBaseValueType>.CheckValueType(value);
            _pairs.Add(key, value);
        }

        /// <summary>
        /// 向字典中添加指定的键值对
        /// </summary>
        /// <param name="item">要添加的键值对</param>
        /// <exception cref="ArgumentException">当键类型已存在或类型不符合约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当键为 null 时抛出</exception>
        /// <remarks>
        /// 此方法是 <see cref="ICollection{T}"/> 接口的实现，内部调用 <see cref="Add(Type, Type)"/> 方法。
        /// </remarks>
        public void Add(KeyValuePair<Type, Type> item) => Add(item.Key, item.Value);

        /// <summary>
        /// 从字典中移除所有键值对
        /// </summary>
        /// <remarks>
        /// 此操作会清空字典，使其成为空字典。
        /// </remarks>
        public void Clear() => _pairs.Clear();

        /// <summary>
        /// 检查指定的键类型是否存在于字典中
        /// </summary>
        /// <typeparam name="TKeyType">要检查的键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <returns>如果键类型存在返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来检查类型是否已注册。
        /// </remarks>
        public bool Contains<TKeyType>() where TKeyType : TBaseKeyType => ContainsKey(typeof(TKeyType));

        /// <summary>
        /// 确定字典是否包含指定的键值对
        /// </summary>
        /// <param name="item">要在字典中定位的键值对</param>
        /// <returns>如果在字典中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <remarks>
        /// 此方法是 <see cref="ICollection{T}"/> 接口的显式实现。
        /// </remarks>
        bool ICollection<KeyValuePair<Type, Type>>.Contains(KeyValuePair<Type, Type> item) => (_pairs as ICollection<KeyValuePair<Type, Type>>).Contains(item);

        /// <summary>
        /// 确定字典是否包含具有指定键的元素
        /// </summary>
        /// <param name="key">要在字典中定位的键</param>
        /// <returns>如果字典包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 null 时抛出</exception>
        public bool ContainsKey(Type key) => _pairs.ContainsKey(key);

        /// <summary>
        /// 从特定的数组索引开始，将字典的元素复制到数组中
        /// </summary>
        /// <param name="array">目标数组</param>
        /// <param name="arrayIndex">开始复制的索引</param>
        /// <remarks>
        /// 此方法是 <see cref="ICollection{T}"/> 接口的显式实现。
        /// </remarks>
        void ICollection<KeyValuePair<Type, Type>>.CopyTo(KeyValuePair<Type, Type>[] array, int arrayIndex) => (_pairs as ICollection<KeyValuePair<Type, Type>>).CopyTo(array, arrayIndex);

        /// <summary>
        /// 返回循环访问字典的枚举器
        /// </summary>
        /// <returns>用于字典的 <see cref="IEnumerator{T}"/></returns>
        /// <remarks>
        /// 此方法是 <see cref="IEnumerable{T}"/> 接口的显式实现。
        /// </remarks>
        IEnumerator<KeyValuePair<Type, Type>> IEnumerable<KeyValuePair<Type, Type>>.GetEnumerator() => _pairs.GetEnumerator();

        /// <summary>
        /// 获取指定键类型对应的值类型，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="TKeyType">键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <returns>如果找到对应的值类型则返回该类型，否则返回 <c>null</c></returns>
        /// <remarks>
        /// 此方法提供了安全的类型查找方式，不会在键不存在时抛出异常。
        /// 使用扩展方法 <see cref="DictionaryExtensions.GetOrDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey)"/> 进行查找。
        /// </remarks>
        public Type GetOrDefault<TKeyType>() where TKeyType : TBaseKeyType => _pairs.GetOrDefault(typeof(TKeyType));

        /// <summary>
        /// 从字典中移除指定的键类型及其对应的值类型
        /// </summary>
        /// <typeparam name="TKeyType">要移除的键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <returns>如果成功移除返回 <c>true</c>；如果键类型不存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来移除类型映射。
        /// </remarks>
        public bool Remove<TKeyType>() where TKeyType : TBaseKeyType => Remove(typeof(TKeyType));

        /// <summary>
        /// 从字典中移除具有指定键的元素
        /// </summary>
        /// <param name="key">要移除的元素的键</param>
        /// <returns>如果成功移除元素，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentException">当键类型不符合约束时抛出</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会先验证键类型，然后尝试从内部字典中移除。
        /// </remarks>
        public bool Remove(Type key)
        {
            TypeDictionary<TBaseKeyType, TBaseValueType>.CheckKeyType(key);
            return _pairs.Remove(key);
        }

        /// <summary>
        /// 从字典中移除指定的键值对
        /// </summary>
        /// <param name="item">要移除的键值对</param>
        /// <returns>如果成功移除项，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <remarks>
        /// 此方法是 <see cref="ICollection{T}"/> 接口的显式实现。
        /// </remarks>
        bool ICollection<KeyValuePair<Type, Type>>.Remove(KeyValuePair<Type, Type> item) => (_pairs as ICollection<KeyValuePair<Type, Type>>).Remove(item);

        /// <summary>
        /// 尝试向字典中添加一个类型映射关系
        /// </summary>
        /// <typeparam name="TKeyType">键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <typeparam name="TValueType">值类型，必须继承或实现 <typeparamref name="TBaseValueType"/></typeparam>
        /// <returns>如果成功添加返回 <c>true</c>；如果键类型已存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了安全的添加方式，当键类型已存在时不会抛出异常，而是返回 <c>false</c>。
        /// 首先检查键是否已存在，如果不存在则调用 <see cref="Add{TKeyType, TValueType}"/> 方法添加。
        /// </remarks>
        public bool TryAdd<TKeyType, TValueType>()
            where TKeyType : TBaseKeyType
            where TValueType : TBaseValueType
        {
            if (_pairs.ContainsKey(typeof(TKeyType)))
            {
                return false;
            }
            Add<TKeyType, TValueType>();
            return true;
        }

        /// <summary>
        /// 获取与指定键关联的值
        /// </summary>
        /// <param name="key">要获取其值的键</param>
        /// <param name="value">当此方法返回时，包含与指定键关联的值（如果找到该键）；否则为该值类型的默认值</param>
        /// <returns>如果字典包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法是 <see cref="IDictionary{TKey, TValue}"/> 接口的实现，提供安全的值获取方式。
        /// </remarks>
        public bool TryGetValue(Type key, out Type value) => _pairs.TryGetValue(key, out value);

        /// <summary>
        /// 返回循环访问集合的枚举器
        /// </summary>
        /// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象</returns>
        /// <remarks>
        /// 此方法是 <see cref="IEnumerable"/> 接口的显式实现。
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => _pairs.GetEnumerator();

        /// <summary>
        /// 验证值类型是否符合基类型约束
        /// </summary>
        /// <param name="value">要验证的值类型</param>
        /// <exception cref="ArgumentException">当值类型不符合 <typeparamref name="TBaseValueType"/> 约束时抛出</exception>
        /// <remarks>
        /// 使用反射检查类型兼容性，确保类型安全。
        /// </remarks>
        private static void CheckValueType(Type value)
        {
            if (!typeof(TBaseValueType).GetTypeInfo().IsAssignableFrom(value))
            {
                throw new ArgumentException($"给定的类型 ({value.AssemblyQualifiedName}) 应该是 {typeof(TBaseValueType).AssemblyQualifiedName} 的实例", nameof(value));
            }
        }

        /// <summary>
        /// 验证键类型是否符合基类型约束
        /// </summary>
        /// <param name="key">要验证的键类型</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当键类型不符合 <typeparamref name="TBaseKeyType"/> 约束时抛出</exception>
        /// <remarks>
        /// 首先检查键不为 null，然后使用反射检查类型兼容性，确保类型安全。
        /// </remarks>
        private static void CheckKeyType(Type key)
        {
            Check.NotNull(key, nameof(key));
            if (!typeof(TBaseKeyType).GetTypeInfo().IsAssignableFrom(key))
            {
                throw new ArgumentException($"给定的类型 ({key.AssemblyQualifiedName}) 应该是 {typeof(TBaseKeyType).AssemblyQualifiedName} 的实例", nameof(key));
            }
        }
    }
}
