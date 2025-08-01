using System;
using System.Collections.Generic;

namespace Scorpio
{
    /// <summary>
    /// 定义类型列表接口，提供基于类型的列表存储和操作
    /// </summary>
    /// <remarks>
    /// 这是一个简化的类型列表接口，继承自 <see cref="ITypeList{TBaseType}"/>，
    /// 使用 <see cref="object"/> 作为基类型，提供最大的灵活性。
    /// 所有 .NET 类型都可以添加到此列表中。
    /// </remarks>
    /// <seealso cref="ITypeList{TBaseType}"/>
    public interface ITypeList : ITypeList<object>
    {

    }

    /// <summary>
    /// 定义泛型类型列表接口，提供类型安全的类型集合存储和操作
    /// </summary>
    /// <typeparam name="TBaseType">基类型，列表中的所有类型必须继承或实现此类型</typeparam>
    /// <remarks>
    /// 此接口扩展了标准的 <see cref="IList{Type}"/>，提供了更强的类型安全性和便捷性。
    /// 使用协变泛型参数 <c>in</c> 修饰符，允许更灵活的类型转换。
    /// 提供了基于泛型类型的便捷方法，避免了直接操作 <see cref="Type"/> 对象的复杂性。
    /// </remarks>
    /// <seealso cref="IList{T}"/>
    /// <seealso cref="Type"/>
    public interface ITypeList<in TBaseType> : IList<Type>
    {
        /// <summary>
        /// 向列表中添加一个类型
        /// </summary>
        /// <typeparam name="T">要添加的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <remarks>
        /// 此方法提供了类型安全的方式来添加类型，避免了手动获取 <see cref="Type"/> 对象。
        /// 如果类型已存在，可能会导致重复添加。如果需要避免重复，请使用 <see cref="TryAdd{T}"/>。
        /// </remarks>
        /// <seealso cref="TryAdd{T}"/>
        /// <seealso cref="ICollection{T}.Add"/>
        void Add<T>() where T : TBaseType;

        /// <summary>
        /// 尝试向列表中添加一个类型，如果类型不存在则添加
        /// </summary>
        /// <typeparam name="T">要添加的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <returns>如果成功添加返回 <c>true</c>；如果类型已存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了安全的添加方式，当类型已存在时不会重复添加，而是返回 <c>false</c>。
        /// 这在不确定类型是否已存在的情况下非常有用，可以避免重复条目。
        /// </remarks>
        /// <seealso cref="Add{T}"/>
        /// <seealso cref="Contains{T}"/>
        bool TryAdd<T>() where T : TBaseType;

        /// <summary>
        /// 检查指定的类型是否存在于列表中
        /// </summary>
        /// <typeparam name="T">要检查的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <returns>如果类型存在返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来检查类型是否已在列表中。
        /// 等效于调用 <see cref="ICollection{Type}.Contains"/> 方法，但提供了更好的类型安全性。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Contains"/>
        bool Contains<T>() where T : TBaseType;

        /// <summary>
        /// 从列表中移除指定的类型
        /// </summary>
        /// <typeparam name="T">要移除的类型，必须继承或实现 <typeparamref name="TBaseType"/></typeparam>
        /// <returns>如果成功移除返回 <c>true</c>；如果类型不存在返回 <c>false</c></returns>
        /// <remarks>
        /// 此方法提供了类型安全的方式来移除类型。
        /// 如果指定的类型不存在，不会抛出异常，而是返回 <c>false</c>。
        /// 如果列表中有多个相同的类型实例，只会移除第一个找到的实例。
        /// </remarks>
        /// <seealso cref="ICollection{T}.Remove"/>
        bool Remove<T>() where T : TBaseType;
    }
}
