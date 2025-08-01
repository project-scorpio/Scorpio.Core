using System;
using System.Collections.Generic;

namespace Scorpio
{
    /// <summary>
    /// 定义类型字典接口，提供基于类型的键值对存储和操作
    /// </summary>
    /// <remarks>
    /// 这是一个简化的类型字典接口，继承自 <see cref="ITypeDictionary{TBaseKeyType, TBaseValueType}"/>，
    /// 其中键类型和值类型都使用 <see cref="object"/> 作为基类型，提供最大的灵活性。
    /// </remarks>
    /// <seealso cref="ITypeDictionary{TBaseKeyType, TBaseValueType}"/>
    public interface ITypeDictionary : ITypeDictionary<object, object>
    {

    }

    /// <summary>
    /// 定义泛型类型字典接口，提供类型安全的键值对存储和操作
    /// </summary>
    /// <typeparam name="TBaseKeyType">键类型的基类型，所有键类型必须继承或实现此类型</typeparam>
    /// <typeparam name="TBaseValueType">值类型的基类型，所有值类型必须继承或实现此类型</typeparam>
    /// <remarks>
    /// 此接口扩展了标准的 <see cref="IDictionary{Type, Type}"/>，提供了更强的类型安全性。
    /// 使用协变泛型参数 <c>in</c> 修饰符，允许更灵活的类型转换。
    /// 提供了基于泛型类型的便捷方法，避免了直接操作 <see cref="Type"/> 对象的复杂性。
    /// </remarks>
    /// <seealso cref="IDictionary{TKey, TValue}"/>
    /// <seealso cref="Type"/>
    public interface ITypeDictionary<in TBaseKeyType, in TBaseValueType> : IDictionary<Type, Type>
    {
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
        void Add<TKeyType, TValueType>()
            where TKeyType : TBaseKeyType where TValueType : TBaseValueType;

        /// <summary>
        /// 尝试向字典中添加一个类型映射关系
        /// </summary>
        /// <typeparam name="TKeyType">键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <typeparam name="TValueType">值类型，必须继承或实现 <typeparamref name="TBaseValueType"/></typeparam>
        /// <returns>如果成功添加返回 <c>true</c>；如果键类型已存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了安全的添加方式，当键类型已存在时不会抛出异常，而是返回 <c>false</c>。
        /// 这在不确定键是否已存在的情况下非常有用。
        /// </remarks>
        /// <seealso cref="Add{TKeyType, TValueType}"/>
        bool TryAdd<TKeyType, TValueType>()
            where TKeyType : TBaseKeyType where TValueType : TBaseValueType;

        /// <summary>
        /// 检查指定的键类型是否存在于字典中
        /// </summary>
        /// <typeparam name="TKeyType">要检查的键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <returns>如果键类型存在返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来检查类型是否已注册。
        /// 等效于调用 <see cref="IDictionary{TKey, TValue}.ContainsKey"/> 方法。
        /// </remarks>
        /// <seealso cref="IDictionary{TKey, TValue}.ContainsKey"/>
        bool Contains<TKeyType>() where TKeyType : TBaseKeyType;

        /// <summary>
        /// 从字典中移除指定的键类型及其对应的值类型
        /// </summary>
        /// <typeparam name="TKeyType">要移除的键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <returns>如果成功移除返回 <c>true</c>；如果键类型不存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来移除类型映射。
        /// 如果指定的键类型不存在，不会抛出异常，而是返回 <c>false</c>。
        /// </remarks>
        /// <seealso cref="IDictionary{TKey, TValue}.Remove"/>
        bool Remove<TKeyType>() where TKeyType : TBaseKeyType;

        /// <summary>
        /// 获取指定键类型对应的值类型，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="TKeyType">键类型，必须继承或实现 <typeparamref name="TBaseKeyType"/></typeparam>
        /// <returns>如果找到对应的值类型则返回该类型，否则返回 <c>null</c></returns>
        /// <remarks>
        /// 此方法提供了安全的类型查找方式，不会在键不存在时抛出异常。
        /// 返回 <c>null</c> 表示指定的键类型未在字典中注册。
        /// </remarks>
        /// <seealso cref="IDictionary{TKey, TValue}.TryGetValue"/>
        Type GetOrDefault<TKeyType>() where TKeyType : TBaseKeyType;
    }
}
