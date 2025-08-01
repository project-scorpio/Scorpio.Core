namespace System.Linq.Expressions
{
    /// <summary>
    /// 表示单参数表达式转换器接口。
    /// 提供将一种参数类型的表达式转换为另一种参数类型的表达式的功能。
    /// </summary>
    /// <typeparam name="TSource">源表达式的参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    public interface IExpressionTranslation<TSource, TResult>
    {
        /// <summary>
        /// 将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource">目标表达式的参数类型。</typeparam>
        /// <returns>
        /// 转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource"/>，
        /// 但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource, TResult>> To<TTranslatedSource>();

        /// <summary>
        /// 使用自定义映射规则将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource">目标表达式的参数类型。</typeparam>
        /// <param name="action">
        /// 用于配置转换映射规则的委托。
        /// 通过此委托可以指定源表达式中的路径如何映射到目标表达式中的路径。
        /// </param>
        /// <returns>
        /// 根据自定义映射规则转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource"/>，
        /// 但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource, TResult>> To<TTranslatedSource>(Action<TranslatePathMapper<Func<TSource, TResult>>> action);
    }

    /// <summary>
    /// 表示双参数表达式转换器接口。
    /// 提供将一种参数类型的双参数表达式转换为另一种参数类型的表达式的功能。
    /// </summary>
    /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
    /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public interface IExpressionTranslation<T1, T2, TResult>
    {
        /// <summary>
        /// 将双参数表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <returns>
        /// 转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource1"/> 和 
        /// <typeparamref name="TTranslatedSource2"/>，但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TResult>> To<TTranslatedSource1, TTranslatedSource2>();

        /// <summary>
        /// 使用自定义映射规则将双参数表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <param name="action">
        /// 用于配置转换映射规则的委托。
        /// 通过此委托可以指定源表达式中的路径如何映射到目标表达式中的路径。
        /// </param>
        /// <returns>
        /// 根据自定义映射规则转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource1"/> 和 
        /// <typeparamref name="TTranslatedSource2"/>，但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TResult>> To<TTranslatedSource1, TTranslatedSource2>(Action<TranslatePathMapper<Func<T1, T2, TResult>>> action);
    }

    /// <summary>
    /// 表示三参数表达式转换器接口。
    /// 提供将一种参数类型的三参数表达式转换为另一种参数类型的表达式的功能。
    /// </summary>
    /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
    /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
    /// <typeparam name="T3">源表达式的第三个参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic Parameters", Justification = "<挂起>")]
    public interface IExpressionTranslation<T1, T2, T3, TResult>
    {
        /// <summary>
        /// 将三参数表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <returns>
        /// 转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource1"/>、
        /// <typeparamref name="TTranslatedSource2"/> 和 <typeparamref name="TTranslatedSource3"/>，
        /// 但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TResult>> To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3>();

        /// <summary>
        /// 使用自定义映射规则将三参数表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <param name="action">
        /// 用于配置转换映射规则的委托。
        /// 通过此委托可以指定源表达式中的路径如何映射到目标表达式中的路径。
        /// </param>
        /// <returns>
        /// 根据自定义映射规则转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource1"/>、
        /// <typeparamref name="TTranslatedSource2"/> 和 <typeparamref name="TTranslatedSource3"/>，
        /// 但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TResult>> To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3>(Action<TranslatePathMapper<Func<T1, T2, T3, TResult>>> action);
    }

    /// <summary>
    /// 表示四参数表达式转换器接口。
    /// 提供将一种参数类型的四参数表达式转换为另一种参数类型的表达式的功能。
    /// </summary>
    /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
    /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
    /// <typeparam name="T3">源表达式的第三个参数类型。</typeparam>
    /// <typeparam name="T4">源表达式的第四个参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public interface IExpressionTranslation<T1, T2, T3, T4, TResult>
    {
        /// <summary>
        /// 将四参数表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource4">目标表达式的第四个参数类型。</typeparam>
        /// <returns>
        /// 转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource1"/>、
        /// <typeparamref name="TTranslatedSource2"/>、<typeparamref name="TTranslatedSource3"/> 和 
        /// <typeparamref name="TTranslatedSource4"/>，但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4, TResult>> To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4>();

        /// <summary>
        /// 使用自定义映射规则将四参数表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource4">目标表达式的第四个参数类型。</typeparam>
        /// <param name="action">
        /// 用于配置转换映射规则的委托。
        /// 通过此委托可以指定源表达式中的路径如何映射到目标表达式中的路径。
        /// </param>
        /// <returns>
        /// 根据自定义映射规则转换后的表达式，参数类型变为 <typeparamref name="TTranslatedSource1"/>、
        /// <typeparamref name="TTranslatedSource2"/>、<typeparamref name="TTranslatedSource3"/> 和 
        /// <typeparamref name="TTranslatedSource4"/>，但保留原始表达式的结构和返回类型 <typeparamref name="TResult"/>。
        /// </returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4, TResult>> To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4>(Action<TranslatePathMapper<Func<T1, T2, T3, T4, TResult>>> action);
    }
}