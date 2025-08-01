using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

using Scorpio;

namespace System.Reflection
{
    /// <summary>
    /// <see cref="MemberInfo"/> 的扩展方法集合。
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// 获取成员元数据的 <see cref="DescriptionAttribute"/> 特性描述信息。
        /// </summary>
        /// <param name="member">成员元数据对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DescriptionAttribute"/> 特性描述信息，如不存在则返回 <see cref="DisplayAttribute"/> 的描述，再不存在则返回成员名称</returns>
        public static string GetDescription(this MemberInfo member, bool inherit = false)
        {
            var desc = member.GetAttribute<DescriptionAttribute>(inherit: inherit);
            return desc?.Description ?? member.GetDisplayAttribute()?.Description ?? member.Name;
        }

        /// <summary>
        /// 获取类型元数据的 <see cref="DescriptionAttribute"/> 特性描述信息。
        /// </summary>
        /// <param name="object">类型实例对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DescriptionAttribute"/> 特性描述信息，如不存在则返回 <see cref="DisplayAttribute"/> 的描述，再不存在则返回成员名称</returns>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> 为 null 时抛出</exception>
        public static string GetDescription(this object @object, bool inherit = false)
        {
            Check.NotNull(@object, nameof(@object));
            return @object.GetType().GetDescription(inherit);
        }

        /// <summary>
        /// 获取指定属性的 <see cref="DescriptionAttribute"/> 特性描述信息。
        /// </summary>
        /// <typeparam name="TModel">模型类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="object">模型实例对象</param>
        /// <param name="propertyExpression">属性表达式</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DescriptionAttribute"/> 特性描述信息，如不存在则返回 <see cref="DisplayAttribute"/> 的描述，再不存在则返回成员名称</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyExpression"/> 为 null 时抛出</exception>
        public static string GetDescription<TModel, TProperty>(this TModel @object, Expression<Func<TModel, TProperty>> propertyExpression, bool inherit = false)
        {
            Check.NotNull(propertyExpression, nameof(propertyExpression));
            var member = ((MemberExpression)(propertyExpression).Body).Member;
            return member.GetDescription(inherit);
        }

        /// <summary>
        /// 获取枚举值的 <see cref="DescriptionAttribute"/> 特性描述信息。
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="object">枚举值</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DescriptionAttribute"/> 特性描述信息，如不存在则返回 <see cref="DisplayAttribute"/> 的描述，再不存在则返回成员名称</returns>
        public static string GetDescription<TEnum>(this TEnum @object, bool inherit = false) where TEnum : Enum
        {
            var type = @object.GetType();//先获取这个枚举的类型
            var field = type.GetField(@object.ToString());//通过这个类型获取到值
            return field.GetDescription(inherit);
        }

        /// <summary>
        /// 获取成员元数据的 <see cref="DisplayNameAttribute"/> 特性显示名称。
        /// </summary>
        /// <param name="member">成员元数据对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DisplayNameAttribute"/> 特性显示名称，如不存在则返回 <see cref="DisplayAttribute"/> 的名称，再不存在则返回成员名称</returns>
        public static string GetDisplayName(this MemberInfo member, bool inherit = false)
        {
            var desc = member.GetAttribute<DisplayNameAttribute>(inherit: inherit);
            return desc?.DisplayName ?? member.GetDisplayAttribute()?.Name ?? member.Name;
        }

        /// <summary>
        /// 获取类型元数据的 <see cref="DisplayNameAttribute"/> 特性显示名称。
        /// </summary>
        /// <param name="object">类型实例对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DisplayNameAttribute"/> 特性显示名称，如不存在则返回 <see cref="DisplayAttribute"/> 的名称，再不存在则返回成员名称</returns>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> 为 null 时抛出</exception>
        public static string GetDisplayName(this object @object, bool inherit = false)
        {
            Check.NotNull(@object, nameof(@object));
            return @object.GetType().GetDisplayName(inherit);
        }

        /// <summary>
        /// 获取枚举值的 <see cref="DisplayNameAttribute"/> 特性显示名称。
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="object">枚举值</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DisplayNameAttribute"/> 特性显示名称，如不存在则返回 <see cref="DisplayAttribute"/> 的名称，再不存在则返回成员名称</returns>
        public static string GetDisplayName<TEnum>(this TEnum @object, bool inherit = false) where TEnum : Enum
        {
            var type = @object.GetType();//先获取这个枚举的类型
            var field = type.GetField(@object.ToString());//通过这个类型获取到值
            return field.GetDisplayName(inherit);
        }

        /// <summary>
        /// 获取指定属性的 <see cref="DisplayNameAttribute"/> 特性显示名称。
        /// </summary>
        /// <typeparam name="TModel">模型类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="object">模型实例对象</param>
        /// <param name="propertyExpression">属性表达式</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回 <see cref="DisplayNameAttribute"/> 特性显示名称，如不存在则返回 <see cref="DisplayAttribute"/> 的名称，再不存在则返回成员名称</returns>
        [Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static string GetDisplayName<TModel, TProperty>(this TModel @object, Expression<Func<TModel, TProperty>> propertyExpression, bool inherit = false)
        {
            var member = ((MemberExpression)propertyExpression.Body).Member;
            return member.GetDisplayName(inherit);
        }

        /// <summary>
        /// 获取成员元数据的 <see cref="DisplayAttribute"/> 特性。
        /// </summary>
        /// <param name="member">成员元数据对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找特性</param>
        /// <returns>返回 <see cref="DisplayAttribute"/> 特性，如不存在则返回 null</returns>
        private static DisplayAttribute GetDisplayAttribute(this MemberInfo member, bool inherit = false) 
            => member.GetAttribute<DisplayAttribute>(inherit: inherit);

        /// <summary>
        /// 获取指定对象类型上应用的指定类型的第一个自定义特性。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="object">要检查的对象</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <param name="defaultValue">如果未找到特性，则返回的默认值（默认为 default）</param>
        /// <returns>匹配 <typeparamref name="TAttribute"/> 的自定义特性，如果未找到则返回 <paramref name="defaultValue"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> 为 null 时抛出</exception>
        public static TAttribute GetAttribute<TAttribute>(this object @object, bool inherit = false, TAttribute defaultValue = default) 
            => @object.GetAttributes<TAttribute>(inherit).FirstOrDefault() ?? defaultValue;

        /// <summary>
        /// 获取应用于指定成员的指定类型的第一个自定义特性。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="memberInfo">要检查的成员</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <param name="defaultValue">如果未找到特性，则返回的默认值（默认为 default）</param>
        /// <returns>匹配 <typeparamref name="TAttribute"/> 的自定义特性，如果未找到则返回 <paramref name="defaultValue"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberInfo"/> 为 null 时抛出</exception>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo memberInfo, bool inherit = false, TAttribute defaultValue = default) 
            => memberInfo.GetAttributes<TAttribute>(inherit).FirstOrDefault() ?? defaultValue;

        /// <summary>
        /// 获取应用于指定成员或其声明类型的指定类型的第一个自定义特性。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="memberInfo">要检查的成员</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <param name="defaultValue">如果未找到特性，则返回的默认值（默认为 default）</param>
        /// <returns>匹配 <typeparamref name="TAttribute"/> 的自定义特性，如果未找到则返回 <paramref name="defaultValue"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberInfo"/> 为 null 时抛出</exception>
        public static TAttribute GetAttributeOfMemberOrDeclaringType<TAttribute>(this MemberInfo memberInfo, bool inherit = false, TAttribute defaultValue = default)
            => memberInfo.GetAttributesOfMemberOrDeclaringType<TAttribute>(inherit).FirstOrDefault() ?? defaultValue;

        /// <summary>
        /// 获取应用于指定对象类型的指定类型的所有自定义特性的集合。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="object">要检查的对象</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <returns>应用于对象类型并匹配 <typeparamref name="TAttribute"/> 的自定义特性集合，如果不存在则返回空集合</returns>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> 为 null 时抛出</exception>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this object @object, bool inherit = false)
        {
            Check.NotNull(@object, nameof(@object));
            return @object.GetType().GetAttributes<TAttribute>(inherit);
        }

        /// <summary>
        /// 获取应用于指定成员的指定类型的所有自定义特性的集合。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="memberInfo">要检查的成员</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <returns>应用于成员并匹配 <typeparamref name="TAttribute"/> 的自定义特性集合，如果不存在则返回空集合</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberInfo"/> 为 null 时抛出</exception>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
        {
            Check.NotNull(memberInfo, nameof(memberInfo));
            return memberInfo.GetCustomAttributes(inherit).OfType<TAttribute>();
        }

        /// <summary>
        /// 获取应用于指定成员及其声明类型的指定类型的所有自定义特性的集合。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="memberInfo">要检查的成员</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <returns>应用于成员及其声明类型并匹配 <typeparamref name="TAttribute"/> 的自定义特性集合，如果不存在则返回空集合</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberInfo"/> 为 null 时抛出</exception>
        public static IEnumerable<TAttribute> GetAttributesOfMemberOrDeclaringType<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
        {
            Check.NotNull(memberInfo, nameof(memberInfo));
            return memberInfo.GetCustomAttributes(inherit).OfType<TAttribute>().Concat(memberInfo.DeclaringType.GetCustomAttributes(inherit).OfType<TAttribute>());
        }

        /// <summary>
        /// 检查指定对象类型上是否应用了指定类型的自定义特性。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="object">要检查的对象</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <returns>如果找到匹配 <typeparamref name="TAttribute"/> 的特性则返回 true，否则返回 false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> 为 null 时抛出</exception>
        public static bool AttributeExists<TAttribute>(this object @object, bool inherit = false)
        where TAttribute : Attribute
        {
            Check.NotNull(@object, nameof(@object));
            return @object.GetType().AttributeExists<TAttribute>(inherit);
        }

        /// <summary>
        /// 检查指定成员上是否应用了指定类型的自定义特性。
        /// </summary>
        /// <typeparam name="TAttribute">要搜索的特性类型</typeparam>
        /// <param name="memberInfo">要检查的成员</param>
        /// <param name="inherit">是否搜索继承链以查找特性</param>
        /// <returns>如果找到匹配 <typeparamref name="TAttribute"/> 的特性则返回 true，否则返回 false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberInfo"/> 为 null 时抛出</exception>
        public static bool AttributeExists<TAttribute>(this MemberInfo memberInfo, bool inherit = false) where TAttribute : Attribute 
            => memberInfo.GetAttribute<TAttribute>(inherit:inherit) != null;

        /// <summary>
        /// 通过表达式获取指定属性的 <see cref="MemberInfo"/> 对象。
        /// </summary>
        /// <typeparam name="TModel">模型类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="_">模型实例对象（不使用）</param>
        /// <param name="propertyExpression">属性表达式</param>
        /// <returns>表达式对应的 <see cref="MemberInfo"/> 对象</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyExpression"/> 为 null 时抛出</exception>
        public static MemberInfo Member<TModel, TProperty>(this TModel _, Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var member = ((MemberExpression)propertyExpression.Body).Member;
            return member;
        }

    }
}
