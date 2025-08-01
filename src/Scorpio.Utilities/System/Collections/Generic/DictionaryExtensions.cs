using System.Collections.Concurrent;

using Scorpio;

namespace System.Collections.Generic
{
    /// <summary>
    /// 为字典类型提供扩展方法的静态工具类
    /// </summary>
    /// <remarks>
    /// 此类包含一系列针对 <see cref="IDictionary{TKey, TValue}"/>、<see cref="Dictionary{TKey, TValue}"/>、
    /// <see cref="IReadOnlyDictionary{TKey, TValue}"/> 和 <see cref="ConcurrentDictionary{TKey, TValue}"/> 的扩展方法，
    /// 提供了安全的值获取、条件添加/更新等常用字典操作功能。
    /// </remarks>
    /// <seealso cref="IDictionary{TKey, TValue}"/>
    /// <seealso cref="Dictionary{TKey, TValue}"/>
    /// <seealso cref="IReadOnlyDictionary{TKey, TValue}"/>
    /// <seealso cref="ConcurrentDictionary{TKey, TValue}"/>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 尝试从字符串键对象值字典中获取指定类型的值
        /// </summary>
        /// <typeparam name="T">要获取的值的类型</typeparam>
        /// <param name="dictionary">字典对象</param>
        /// <param name="key">要查找的键</param>
        /// <param name="value">如果键存在且值类型匹配，则为键对应的值；否则为 <typeparamref name="T"/> 的默认值</param>
        /// <returns>如果键存在且值可以转换为指定类型则返回 <c>true</c>，否则返回 <c>false</c></returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="key"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法专门用于处理值类型为 <see cref="object"/> 的字典，通过类型检查来安全地获取特定类型的值。
        /// 如果值无法转换为目标类型，将返回 <c>false</c> 并设置输出参数为默认值。
        /// </remarks>
        /// <seealso cref="IDictionary{TKey, TValue}.TryGetValue"/>
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(key, nameof(key));

            if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则返回类型的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回 <typeparamref name="TValue"/> 的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法为 <see cref="Dictionary{TKey, TValue}"/> 提供安全的值获取功能，
        /// 避免了直接访问可能不存在的键时抛出的 <see cref="KeyNotFoundException"/>。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey, TValue)"/>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) => dictionary.GetOrDefault(key, default(TValue));

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则返回类型的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回 <typeparamref name="TValue"/> 的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法为 <see cref="IDictionary{TKey, TValue}"/> 提供安全的值获取功能。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue)"/>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) => dictionary.GetOrDefault(key, default(TValue));

        /// <summary>
        /// 从只读字典中获取指定键的值，如果键不存在则返回类型的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的只读字典</param>
        /// <param name="key">要查找值的键</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回 <typeparamref name="TValue"/> 的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法为 <see cref="IReadOnlyDictionary{TKey, TValue}"/> 提供安全的值获取功能。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(IReadOnlyDictionary{TKey, TValue}, TKey, TValue)"/>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) => dictionary.GetOrDefault(key, default(TValue));

        /// <summary>
        /// 从并发字典中获取指定键的值，如果键不存在则返回类型的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的并发字典</param>
        /// <param name="key">要查找值的键</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回 <typeparamref name="TValue"/> 的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法为 <see cref="ConcurrentDictionary{TKey, TValue}"/> 提供安全的值获取功能。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(ConcurrentDictionary{TKey, TValue}, TKey, TValue)"/>
        public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key) => dictionary.GetOrDefault(key, default(TValue));

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则返回指定的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="default">键不存在时返回的默认值</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回指定的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许指定自定义的默认值，而不是使用类型的默认值。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey)"/>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue @default) => GetOrDefault(dictionary, key, k => @default);

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则返回指定的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="default">键不存在时返回的默认值</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回指定的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许指定自定义的默认值，而不是使用类型的默认值。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(IDictionary{TKey, TValue}, TKey)"/>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default) => GetOrDefault(dictionary, key, k => @default);

        /// <summary>
        /// 从只读字典中获取指定键的值，如果键不存在则返回指定的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的只读字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="default">键不存在时返回的默认值</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回指定的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许指定自定义的默认值，而不是使用类型的默认值。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(IReadOnlyDictionary{TKey, TValue}, TKey)"/>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue @default) => GetOrDefault(dictionary, key, k => @default);

        /// <summary>
        /// 从并发字典中获取指定键的值，如果键不存在则返回指定的默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的并发字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="default">键不存在时返回的默认值</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回指定的默认值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许指定自定义的默认值，而不是使用类型的默认值。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(ConcurrentDictionary{TKey, TValue}, TKey)"/>
        public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue @default) => GetOrDefault(dictionary, key, k => @default);

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则使用工厂函数创建默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="factory">用于创建默认值的工厂函数，接收键作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回工厂函数创建的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许使用工厂函数动态创建默认值，适用于需要根据键生成特定默认值的场景。
        /// 工厂函数只有在键不存在时才会被调用，提供了延迟计算的优势。
        /// </remarks>
        /// <seealso cref="Func{T, TResult}"/>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(factory, nameof(factory));

            if (dictionary.TryGetValue(key, out var obj))
            {
                return obj;
            }
            return factory(key);
        }

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则使用工厂函数创建默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="factory">用于创建默认值的工厂函数，接收键作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回工厂函数创建的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许使用工厂函数动态创建默认值，适用于需要根据键生成特定默认值的场景。
        /// </remarks>
        /// <seealso cref="Func{T, TResult}"/>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(factory, nameof(factory));

            if (dictionary.TryGetValue(key, out var obj))
            {
                return obj;
            }
            return factory(key);
        }

        /// <summary>
        /// 从只读字典中获取指定键的值，如果键不存在则使用工厂函数创建默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的只读字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="factory">用于创建默认值的工厂函数，接收键作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回工厂函数创建的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许使用工厂函数动态创建默认值，适用于需要根据键生成特定默认值的场景。
        /// </remarks>
        /// <seealso cref="Func{T, TResult}"/>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(factory, nameof(factory));

            if (dictionary.TryGetValue(key, out var obj))
            {
                return obj;
            }
            return factory(key);
        }

        /// <summary>
        /// 从并发字典中获取指定键的值，如果键不存在则使用工厂函数创建默认值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的并发字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="factory">用于创建默认值的工厂函数，接收键作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回工厂函数创建的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载允许使用工厂函数动态创建默认值，适用于需要根据键生成特定默认值的场景。
        /// </remarks>
        /// <seealso cref="Func{T, TResult}"/>
        public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(factory, nameof(factory));

            if (dictionary.TryGetValue(key, out var obj))
            {
                return obj;
            }
            return factory(key);
        }

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则创建、添加并返回新值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="factory">用于创建新值的工厂函数，接收键作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回新创建并添加的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法实现了"获取或添加"的模式，常用于缓存和延迟初始化场景。
        /// 如果键存在，直接返回现有值；如果不存在，则创建新值、添加到字典中并返回。
        /// 这种模式确保了字典中始终存在所需的键值对。
        /// </remarks>
        /// <seealso cref="GetOrDefault{TKey, TValue}(IDictionary{TKey, TValue}, TKey, Func{TKey, TValue})"/>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(factory, nameof(factory));

            if (dictionary.TryGetValue(key, out var obj))
            {
                return obj;
            }
            return dictionary[key] = factory(key);
        }

        /// <summary>
        /// 从字典中获取指定键的值，如果键不存在则创建、添加并返回新值
        /// </summary>
        /// <param name="dictionary">要检查和获取值的字典</param>
        /// <param name="key">要查找值的键</param>
        /// <param name="factory">用于创建新值的工厂函数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>如果找到键则返回对应的值，否则返回新创建并添加的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载的工厂函数不接收键作为参数，适用于创建值时不需要键信息的场景。
        /// </remarks>
        /// <seealso cref="GetOrAdd{TKey, TValue}(IDictionary{TKey, TValue}, TKey, Func{TKey, TValue})"/>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory) => dictionary.GetOrAdd(key, k => factory());

        /// <summary>
        /// 如果键不存在则添加键值对，如果键已存在则使用指定的函数更新值
        /// </summary>
        /// <param name="dictionary">要操作的字典</param>
        /// <param name="key">要添加或更新的键</param>
        /// <param name="addFactory">键不存在时用于生成新值的函数</param>
        /// <param name="updateFactory">键已存在时用于更新值的函数，接收键和当前值作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>键对应的新值，可能是添加的新值或更新后的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/>、<paramref name="addFactory"/> 或 <paramref name="updateFactory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 它提供了原子性的"添加或更新"操作，确保键值对的一致性。
        /// 更新函数接收当前值作为参数，允许基于现有值计算新值。
        /// </remarks>
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> addFactory, Func<TKey, TValue, TValue> updateFactory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(addFactory, nameof(addFactory));
            Check.NotNull(updateFactory, nameof(updateFactory));

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = updateFactory(key, dictionary[key]);
            }
            else
            {
                dictionary.Add(key, addFactory(key));
            }
            return dictionary[key];
        }

        /// <summary>
        /// 使用指定的工厂函数添加或更新键值对
        /// </summary>
        /// <param name="dictionary">要操作的字典</param>
        /// <param name="key">要添加或更新的键</param>
        /// <param name="factory">用于生成值的工厂函数，接收键作为参数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>键对应的新值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法无论键是否存在都会使用工厂函数生成新值并覆盖现有值。
        /// 这种方式适用于需要根据键重新计算值的场景。
        /// </remarks>
        /// <seealso cref="AddOrUpdate{TKey, TValue}(IDictionary{TKey, TValue}, TKey, Func{TKey, TValue}, Func{TKey, TValue, TValue})"/>
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(dictionary, nameof(dictionary));
            Check.NotNull(factory, nameof(factory));

            return dictionary[key] = factory(key);
        }

        /// <summary>
        /// 使用指定的工厂函数添加或更新键值对
        /// </summary>
        /// <param name="dictionary">要操作的字典</param>
        /// <param name="key">要添加或更新的键</param>
        /// <param name="factory">用于生成值的工厂函数</param>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns>键对应的新值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 或 <paramref name="factory"/> 为 null 时抛出</exception>
        /// <remarks>
        /// 此重载的工厂函数不接收键作为参数，适用于生成值时不需要键信息的场景。
        /// </remarks>
        /// <seealso cref="AddOrUpdate{TKey, TValue}(IDictionary{TKey, TValue}, TKey, Func{TKey, TValue})"/>
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory) => dictionary.AddOrUpdate(key, k => factory());
    }
}
