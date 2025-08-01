using System;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 为约定上下文提供扩展方法的静态类
    /// </summary>
    public static class ConventionalContextExtensions
    {
        /// <summary>
        /// 在约定上下文中设置指定键的值
        /// </summary>
        /// <param name="context">约定上下文实例</param>
        /// <param name="name">项目的键名</param>
        /// <param name="value">要设置的值</param>
        public static void Set(this IConventionalContext context, string name, object value) => (context as ConventionalContext).SetItem(name, value);

        /// <summary>
        /// 从约定上下文中获取指定键的值
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="context">约定上下文实例</param>
        /// <param name="name">项目的键名</param>
        /// <returns>返回指定类型的值，如果不存在则返回默认值</returns>
        public static T Get<T>(this IConventionalContext context, string name) => (context as ConventionalContext).GetItem<T>(name);

        /// <summary>
        /// 获取指定键的值，如果不存在则添加指定的值并返回
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="context">约定上下文实例</param>
        /// <param name="name">项目的键名</param>
        /// <param name="value">当键不存在时要添加的值</param>
        /// <returns>返回获取到的值或新添加的值</returns>
        public static T GetOrAdd<T>(this IConventionalContext context, string name, T value) => GetOrAdd<T>(context, name, key => value);

        /// <summary>
        /// 获取指定键的值，如果不存在则通过工厂方法创建值并添加后返回
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="context">约定上下文实例</param>
        /// <param name="name">项目的键名</param>
        /// <param name="factory">用于创建值的工厂方法</param>
        /// <returns>返回获取到的值或通过工厂方法创建的新值</returns>
        public static T GetOrAdd<T>(this IConventionalContext context, string name, Func<string, T> factory)
        {
            // 尝试获取现有值
            var result = (context as ConventionalContext).GetItem<T>(name);
            if (Equals(result, default(T)))
            {
                // 如果值不存在，通过工厂方法创建新值并存储
                var value = factory(name);
                context.Set(name, value);
                result = value;
            }
            return result;
        }


        /// <summary>
        /// 获取指定键的值，如果不存在则返回指定的默认值（不存储）
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="context">约定上下文实例</param>
        /// <param name="name">项目的键名</param>
        /// <param name="value">当键不存在时返回的默认值</param>
        /// <returns>返回获取到的值或指定的默认值</returns>
        public static T GetOrDefault<T>(this IConventionalContext context, string name, T value) => GetOrDefault<T>(context, name, key => value);

        /// <summary>
        /// 获取指定键的值，如果不存在则通过工厂方法创建默认值返回（不存储）
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="context">约定上下文实例</param>
        /// <param name="name">项目的键名</param>
        /// <param name="factory">用于创建默认值的工厂方法</param>
        /// <returns>返回获取到的值或通过工厂方法创建的默认值</returns>
        public static T GetOrDefault<T>(this IConventionalContext context, string name, Func<string, T> factory)
        {
            // 尝试获取现有值
            var result = (context as ConventionalContext).GetItem<T>(name);
            if (Equals(result, default(T)))
            {
                // 如果值不存在，通过工厂方法创建默认值（不存储到上下文中）
                result = factory(name);
            }
            return result;
        }

        /// <summary>
        /// 为泛型约定上下文添加类型筛选谓词
        /// </summary>
        /// <typeparam name="TAction">操作类型参数</typeparam>
        /// <param name="context">泛型约定上下文实例</param>
        /// <param name="predicate">类型筛选谓词函数</param>
        /// <returns>返回经过筛选的约定上下文 <see cref="IConventionalContext{TAction}"/></returns>
        public static IConventionalContext<TAction> Where<TAction>(this IConventionalContext<TAction> context, Predicate<Type> predicate)
        {
            // 将上下文转换为具体实现并添加谓词
            (context as ConventionalContext).AddPredicate(predicate);
            return context;
        }

    }
}
