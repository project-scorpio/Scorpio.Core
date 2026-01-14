namespace System.Linq.Expressions
{
    /// <summary>
    /// 表达式翻译构建器。
    /// 提供用于创建各种类型表达式翻译器的静态扩展方法。
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public static class TranslationBuilder
    {
        /// <summary>
        /// 创建单参数表达式的翻译器。
        /// </summary>
        /// <typeparam name="TSource">源表达式的参数类型。</typeparam>
        /// <typeparam name="TResult">表达式的返回类型。</typeparam>
        /// <param name="predicate">要翻译的源表达式。</param>
        /// <returns>
        /// 一个 <see cref="IExpressionTranslation{TSource, TResult}"/> 接口实例，
        /// 可用于将表达式从 <typeparamref name="TSource"/> 类型参数转换为其他类型参数。
        /// </returns>
        public static IExpressionTranslation<TSource, TResult> Translate<TSource, TResult>(this Expression<Func<TSource, TResult>> predicate) => new ExpressionTranslation<TSource, TResult>(predicate);

        /// <summary>
        /// 创建双参数表达式的翻译器。
        /// </summary>
        /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
        /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TResult">表达式的返回类型。</typeparam>
        /// <param name="predicate">要翻译的源表达式。</param>
        /// <returns>
        /// 一个 <see cref="IExpressionTranslation{T1, T2, TResult}"/> 接口实例，
        /// 可用于将表达式从 <typeparamref name="T1"/> 和 <typeparamref name="T2"/> 类型参数转换为其他类型参数。
        /// </returns>
        public static IExpressionTranslation<T1, T2, TResult> Translate<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> predicate) => new ExpressionTranslation<T1, T2, TResult>(predicate);

        /// <summary>
        /// 创建三参数表达式的翻译器。
        /// </summary>
        /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
        /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
        /// <typeparam name="T3">源表达式的第三个参数类型。</typeparam>
        /// <typeparam name="TResult">表达式的返回类型。</typeparam>
        /// <param name="predicate">要翻译的源表达式。</param>
        /// <returns>
        /// 一个 <see cref="IExpressionTranslation{T1, T2, T3, TResult}"/> 接口实例，
        /// 可用于将表达式从 <typeparamref name="T1"/>、<typeparamref name="T2"/> 和 <typeparamref name="T3"/> 类型参数转换为其他类型参数。
        /// </returns>
        public static IExpressionTranslation<T1, T2, T3, TResult> Translate<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> predicate) => new ExpressionTranslation<T1, T2, T3, TResult>(predicate);

        /// <summary>
        /// 创建四参数表达式的翻译器。
        /// </summary>
        /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
        /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
        /// <typeparam name="T3">源表达式的第三个参数类型。</typeparam>
        /// <typeparam name="T4">源表达式的第四个参数类型。</typeparam>
        /// <typeparam name="TResult">表达式的返回类型。</typeparam>
        /// <param name="predicate">要翻译的源表达式。</param>
        /// <returns>
        /// 一个 <see cref="IExpressionTranslation{T1, T2, T3, T4, TResult}"/> 接口实例，
        /// 可用于将表达式从 <typeparamref name="T1"/>、<typeparamref name="T2"/>、<typeparamref name="T3"/> 和 <typeparamref name="T4"/> 类型参数转换为其他类型参数。
        /// </returns>
        public static IExpressionTranslation<T1, T2, T3, T4, TResult> Translate<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> predicate) => new ExpressionTranslation<T1, T2, T3, T4, TResult>(predicate);

        /// <summary>
        /// 创建成员初始化表达式的翻译器。
        /// </summary>
        /// <typeparam name="TSource">源表达式类型和目标表达式类型。</typeparam>
        /// <param name="predicate">要翻译的成员初始化表达式。</param>
        /// <returns>
        /// 一个 <see cref="MemberInitTranslation{TSource}"/> 实例，
        /// 可用于将一个类型的成员初始化表达式转换为另一个类型的成员初始化表达式，保持属性映射关系。
        /// </returns>
        public static MemberInitTranslation<TSource> Translate<TSource>(this Expression<Func<TSource, TSource>> predicate) => new MemberInitTranslation<TSource>(predicate);

    }
}
