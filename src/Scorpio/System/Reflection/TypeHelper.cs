using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

using Scorpio;
using Scorpio.Localization;

namespace System.Reflection
{
    /// <summary>
    /// 类型帮助器类，提供类型判断、转换和处理的实用工具方法。
    /// 包含对基元类型、可空类型、泛型类型、集合类型和字典类型的检测和处理功能。
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// 浮点数类型的集合。
        /// 包含 <see cref="float"/>、<see cref="double"/> 和 <see cref="decimal"/> 类型。
        /// </summary>
        private static readonly HashSet<Type> _floatingTypes = new HashSet<Type>
        {
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        /// <summary>
        /// 非可空基元类型的集合。
        /// 包含所有不可为空的基础数据类型，如数值类型、布尔类型和日期时间类型等。
        /// </summary>
        private static readonly HashSet<Type> _nonNullablePrimitiveTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(bool),
            typeof(float),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };

        /// <summary>
        /// 检查指定类型是否为非可空基元类型。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果类型是非可空基元类型则返回 true；否则返回 false</returns>
        public static bool IsNonNullablePrimitiveType(Type type)
        {
            return _nonNullablePrimitiveTypes.Contains(type);
        }

        /// <summary>
        /// 检查指定对象是否为无参数的 <see cref="Func{TResult}"/> 委托。
        /// </summary>
        /// <param name="obj">要检查的对象</param>
        /// <returns>如果对象是 <see cref="Func{TResult}"/> 委托则返回 true；否则返回 false</returns>
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        /// <summary>
        /// 检查指定对象是否为返回 <typeparamref name="TReturn"/> 类型的 <see cref="Func{TResult}"/> 委托。
        /// </summary>
        /// <typeparam name="TReturn">委托的返回值类型</typeparam>
        /// <param name="obj">要检查的对象</param>
        /// <returns>如果对象是指定返回类型的 <see cref="Func{TResult}"/> 委托则返回 true；否则返回 false</returns>
        public static bool IsFunc<TReturn>(object obj) => obj is Func<TReturn>;

        /// <summary>
        /// 检查指定类型是否为扩展的基元类型。
        /// 扩展的基元类型包括 .NET 基元类型以及常用的值类型如 <see cref="string"/>、<see cref="decimal"/>、<see cref="DateTime"/> 等。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <param name="includeNullables">是否包含可空类型，默认为 true</param>
        /// <param name="includeEnums">是否包含枚举类型，默认为 false</param>
        /// <returns>如果类型是扩展基元类型则返回 true；否则返回 false</returns>
        public static bool IsPrimitiveExtended(Type type, bool includeNullables = true, bool includeEnums = false)
        {
            if (IsPrimitiveExtendedInternal(type, includeEnums))
            {
                return true;
            }

            if (includeNullables && IsNullable(type) && type.GenericTypeArguments.Any())
            {
                return IsPrimitiveExtendedInternal(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        /// <summary>
        /// 检查指定类型是否为可空类型（<see cref="Nullable{T}"/>）。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果类型是 <see cref="Nullable{T}"/> 则返回 true；否则返回 false</returns>
        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 如果类型是可空类型，则获取其第一个泛型参数；否则返回原类型。
        /// 用于从 <see cref="Nullable{T}"/> 中提取 T 类型。
        /// </summary>
        /// <param name="t">要处理的类型</param>
        /// <returns>可空类型的底层类型或原类型本身</returns>
        public static Type GetFirstGenericArgumentIfNullable(this Type t)
        {
            if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments().FirstOrDefault();
            }

            return t;
        }

        /// <summary>
        /// 检查指定类型是否为可枚举类型，并输出元素类型。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <param name="itemType">输出参数，表示集合元素的类型</param>
        /// <param name="includePrimitives">是否包含基元类型，默认为 true</param>
        /// <returns>如果类型是可枚举类型则返回 true；否则返回 false</returns>
        public static bool IsEnumerable(Type type, out Type itemType, bool includePrimitives = true)
        {
            if (!includePrimitives && IsPrimitiveExtended(type))
            {
                itemType = null;
                return false;
            }

            var enumerableTypes = type.GetAssignableToGenericTypes(typeof(IEnumerable<>));
            if (enumerableTypes.Count == 1)
            {
                itemType = enumerableTypes[0].GenericTypeArguments[0];
                return true;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                itemType = typeof(object);
                return true;
            }

            itemType = null;
            return false;
        }

        /// <summary>
        /// 检查指定类型是否为字典类型，并输出键和值的类型。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <param name="keyType">输出参数，表示字典键的类型</param>
        /// <param name="valueType">输出参数，表示字典值的类型</param>
        /// <returns>如果类型是字典类型则返回 true；否则返回 false</returns>
        public static bool IsDictionary(Type type, out Type keyType, out Type valueType)
        {
            var dictionaryTypes = type.GetAssignableToGenericTypes(typeof(IDictionary<,>));

            if (dictionaryTypes.Count == 1)
            {
                keyType = dictionaryTypes[0].GenericTypeArguments[0];
                valueType = dictionaryTypes[0].GenericTypeArguments[1];
                return true;
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                keyType = typeof(object);
                valueType = typeof(object);
                return true;
            }

            keyType = null;
            valueType = null;

            return false;
        }

        /// <summary>
        /// 内部方法：检查类型是否为扩展基元类型。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <param name="includeEnums">是否包含枚举类型</param>
        /// <returns>如果类型是扩展基元类型则返回 true；否则返回 false</returns>
        private static bool IsPrimitiveExtendedInternal(Type type, bool includeEnums)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        /// <summary>
        /// 获取指定类型的默认值。
        /// </summary>
        /// <typeparam name="T">要获取默认值的类型</typeparam>
        /// <returns><typeparamref name="T"/> 类型的默认值</returns>
        public static T GetDefaultValue<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取指定类型的默认值。
        /// </summary>
        /// <param name="type">要获取默认值的类型</param>
        /// <returns>指定类型的默认值，值类型返回其默认实例，引用类型返回 null</returns>
        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// 获取类型的完整名称，包括对可空类型和泛型类型的特殊处理。
        /// 可空类型会显示为"类型?"，泛型类型会显示完整的泛型参数。
        /// </summary>
        /// <param name="type">要获取名称的类型</param>
        /// <returns>处理后的类型完整名称</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出</exception>
        public static string GetFullNameHandlingNullableAndGenerics(Type type)
        {
            Check.NotNull(type, nameof(type));

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GenericTypeArguments[0].FullName + "?";
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                return $"{genericType.FullName.Left(genericType.FullName.IndexOf('`'))}<{type.GenericTypeArguments.Select(GetFullNameHandlingNullableAndGenerics).ExpandToString(",")}>";
            }

            return type.FullName ?? type.Name;
        }

        /// <summary>
        /// 获取类型的简化名称，将常用的 .NET 类型映射为简化的字符串表示。
        /// 主要用于生成更友好的类型名称显示，如将 <see cref="int"/> 映射为 "number"。
        /// </summary>
        /// <param name="type">要获取简化名称的类型</param>
        /// <returns>类型的简化名称</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出</exception>
        public static string GetSimplifiedName(Type type)
        {
            Check.NotNull(type, nameof(type));

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return GetSimplifiedName(type.GenericTypeArguments[0]) + "?";
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                return $"{genericType.FullName.Left(genericType.FullName.IndexOf('`'))}<{type.GenericTypeArguments.Select(GetSimplifiedName).ExpandToString(",")}>";
            }
            
            // 将常用类型映射为简化名称
            if (type == typeof(string)
                || type == typeof(char)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid))
            {
                return "string";
            }
            else if (type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(IntPtr)
                || type == typeof(UIntPtr)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(bool))
            {
                return "number";
            }
            else if (type == typeof(object))
            {
                return "object";
            }

            return type.FullName ?? type.Name;
        }

        /// <summary>
        /// 将字符串值转换为指定的目标类型。
        /// </summary>
        /// <typeparam name="TTargetType">目标类型</typeparam>
        /// <param name="value">要转换的字符串值</param>
        /// <returns>转换后的 <typeparamref name="TTargetType"/> 类型对象</returns>
        public static object ConvertFromString<TTargetType>(string value)
        {
            return ConvertFromString(typeof(TTargetType), value);
        }

        /// <summary>
        /// 将字符串值转换为指定的目标类型。
        /// 对于浮点数类型，使用不变区域性进行转换以确保数值格式的一致性。
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="value">要转换的字符串值</param>
        /// <returns>转换后的目标类型对象，如果输入为 null 则返回 null</returns>
        public static object ConvertFromString(Type targetType, string value)
        {
            if (value == null)
            {
                return null;
            }

            var converter = TypeDescriptor.GetConverter(targetType);

            if (IsFloatingType(targetType))
            {
                using (CultureHelper.Use(CultureInfo.InvariantCulture))
                {
                    return converter.ConvertFromString(value.Replace(',', '.'));
                }
            }

            return converter.ConvertFromString(value);
        }

        /// <summary>
        /// 检查指定类型是否为浮点数类型。
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <param name="includeNullable">是否包含可空的浮点数类型，默认为 true</param>
        /// <returns>如果类型是浮点数类型则返回 true；否则返回 false</returns>
        public static bool IsFloatingType(Type type, bool includeNullable = true)
        {
            if (_floatingTypes.Contains(type))
            {
                return true;
            }

            if (includeNullable &&
                IsNullable(type) &&
                _floatingTypes.Contains(type.GenericTypeArguments[0]))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 将对象值转换为指定的目标类型。
        /// </summary>
        /// <typeparam name="TTargetType">目标类型</typeparam>
        /// <param name="value">要转换的对象值</param>
        /// <returns>转换后的 <typeparamref name="TTargetType"/> 类型对象</returns>
        public static object ConvertFrom<TTargetType>(object value)
        {
            return ConvertFrom(typeof(TTargetType), value);
        }

        /// <summary>
        /// 将对象值转换为指定的目标类型。
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="value">要转换的对象值</param>
        /// <returns>转换后的目标类型对象</returns>
        public static object ConvertFrom(Type targetType, object value)
        {
            return TypeDescriptor
                .GetConverter(targetType)
                .ConvertFrom(value);
        }

        /// <summary>
        /// 移除可空类型的包装，返回底层类型。
        /// 如果类型不是可空类型，则返回原类型。
        /// </summary>
        /// <param name="type">要处理的类型</param>
        /// <returns>去除可空包装后的类型</returns>
        public static Type StripNullable(Type type)
        {
            return IsNullable(type)
                ? type.GenericTypeArguments[0]
                : type;
        }

        /// <summary>
        /// 检查对象是否为其类型的默认值。
        /// </summary>
        /// <param name="obj">要检查的对象</param>
        /// <returns>如果对象是其类型的默认值则返回 true；否则返回 false</returns>
        public static bool IsDefaultValue(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            return obj.Equals(GetDefaultValue(obj.GetType()));
        }
    }
}
