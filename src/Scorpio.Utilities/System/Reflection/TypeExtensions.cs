using System.Collections.Generic;

using Scorpio;

namespace System.Reflection
{
    /// <summary>
    /// 扩展 <see cref="Type"/> 类的方法集合。
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取类型的完整名称，包括程序集名称。
        /// </summary>
        /// <param name="type">要获取名称的类型</param>
        /// <returns>格式为 "完整类型名称, 程序集名称" 的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> 为 null 时抛出</exception>
        public static string GetFullNameWithAssemblyName(this Type type)
        {
            Check.NotNull(type, nameof(type));
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        /// <summary>
        /// 判断指定类型是否位于给定命名空间或其子命名空间中。
        /// </summary>
        /// <param name="this">要测试的类型</param>
        /// <param name="namespace">要测试的命名空间</param>
        /// <returns>如果类型位于指定命名空间或其子命名空间中，则返回 true；否则返回 false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="this"/> 或 <paramref name="namespace"/> 为 null 时抛出</exception>
        public static bool IsInNamespace(this Type @this, string @namespace)
        {
            Check.NotNull(@this, nameof(@this));
            Check.NotNull(@namespace, nameof(@namespace));

            return @this.Namespace != null &&
                (@this.Namespace == @namespace || @this.Namespace.StartsWith(@namespace + ".", StringComparison.Ordinal));
        }

        /// <summary>
        /// 判断指定类型是否与 <typeparamref name="T"/> 位于相同命名空间或其子命名空间中。
        /// </summary>
        /// <typeparam name="T">用于获取命名空间的参考类型</typeparam>
        /// <param name="this">要测试的类型</param>
        /// <returns>如果类型与 <typeparamref name="T"/> 位于相同命名空间或其子命名空间中，则返回 true；否则返回 false</returns>
        public static bool IsInNamespaceOf<T>(this Type @this) => IsInNamespace(@this, typeof(T).Namespace);

        /// <summary>
        /// 判断指定类型是否可分配给 <typeparamref name="T"/> 类型。
        /// </summary>
        /// <typeparam name="T">要测试分配兼容性的目标类型</typeparam>
        /// <param name="this">要测试的类型</param>
        /// <returns>如果类型可分配给 <typeparamref name="T"/> 类型的引用，则返回 true；否则返回 false</returns>
        public static bool IsAssignableTo<T>(this Type @this) => @this.IsAssignableTo(typeof(T));

#if NETSTANDARD2_0
        /// <summary>
        /// 判断指定类型是否可分配给指定的目标类型。
        /// </summary>
        /// <param name="this">要测试的类型</param>
        /// <param name="type">要测试分配兼容性的目标类型</param>
        /// <returns>如果类型可分配给指定目标类型的引用，则返回 true；否则返回 false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="this"/> 或 <paramref name="type"/> 为 null 时抛出</exception>
        public static bool IsAssignableTo(this Type @this, Type type)
        {
            Check.NotNull(@this, nameof(@this));
            Check.NotNull(type, nameof(type));
            return type.GetTypeInfo().IsAssignableFrom(@this.GetTypeInfo());
        }
#endif

        /// <summary>
        /// 判断指定类型是否可分配给指定的泛型类型。
        /// </summary>
        /// <param name="this">要测试的类型</param>
        /// <param name="genericType">要测试分配兼容性的泛型类型</param>
        /// <returns>如果类型可分配给指定泛型类型的引用，则返回 true；否则返回 false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="this"/> 或 <paramref name="genericType"/> 为 null 时抛出</exception>
        public static bool IsAssignableToGenericType(this Type @this, Type genericType)
        {
            Check.NotNull(@this, nameof(@this));
            Check.NotNull(genericType, nameof(genericType));
            var givenTypeInfo = @this.GetTypeInfo();

            // 检查当前类型是否是目标泛型类型
            if (givenTypeInfo.IsGenericType && @this.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            // 检查接口实现
            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            // 递归检查基类
            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }

        /// <summary>
        /// 获取可分配给指定泛型类型的所有类型列表。
        /// </summary>
        /// <param name="this">要测试的类型</param>
        /// <param name="genericType">要测试分配兼容性的泛型类型</param>
        /// <returns>可分配给指定泛型类型的所有类型列表</returns>
        /// <exception cref="ArgumentNullException"><paramref name="this"/> 或 <paramref name="genericType"/> 为 null 时抛出</exception>
        public static List<Type> GetAssignableToGenericTypes(this Type @this, Type genericType)
        {
            Check.NotNull(@this, nameof(@this));
            Check.NotNull(genericType, nameof(genericType));
            var result = new List<Type>();
            AddImplementedGenericTypes(result, @this, genericType);
            return result;
        }

        /// <summary>
        /// 向结果列表中添加给定类型中实现的指定泛型类型。
        /// </summary>
        /// <param name="result">存储结果的列表</param>
        /// <param name="givenType">要检查的类型</param>
        /// <param name="genericType">要查找的泛型类型</param>
        private static void AddImplementedGenericTypes(List<Type> result, Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            // 检查当前类型是否是目标泛型类型
            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                result.AddIfNotContains(givenType);
            }

            // 检查接口实现
            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    result.AddIfNotContains(interfaceType);
                }
            }

            // 递归检查基类
            if (givenTypeInfo.BaseType == null)
            {
                return;
            }

            AddImplementedGenericTypes(result, givenTypeInfo.BaseType, genericType);
        }

        /// <summary>
        /// 判断指定类型是否为标准类型（非抽象类、非泛型定义）。
        /// </summary>
        /// <param name="this">要测试的类型</param>
        /// <returns>如果类型是标准类型，则返回 true；否则返回 false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="this"/> 为 null 时抛出</exception>
        public static bool IsStandardType(this Type @this)
        {
            Check.NotNull(@this, nameof(@this));
            return @this.IsClass && !@this.IsAbstract && !@this.IsGenericTypeDefinition;
        }
    }
}

