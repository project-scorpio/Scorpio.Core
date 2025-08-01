namespace System.Linq.Expressions
{
    /// <summary>
    /// 表达式翻译器的单参数实现。
    /// 用于转换单参数 Lambda 表达式到另一个相似但具有不同参数类型的表达式。
    /// </summary>
    /// <typeparam name="TSource">源表达式的参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    internal sealed class ExpressionTranslation<TSource, TResult> : ExpressionTranslation<Func<TSource, TResult>>, IExpressionTranslation<TSource, TResult>
    {
        /// <summary>
        /// 初始化 <see cref="ExpressionTranslation{TSource, TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">要翻译的源表达式。</param>
        public ExpressionTranslation(Expression<Func<TSource, TResult>> predicate) : base(predicate)
        {
        }

        /// <summary>
        /// 将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource">目标表达式的参数类型。</typeparam>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource, TResult>> IExpressionTranslation<TSource, TResult>.To<TTranslatedSource>() => To<Func<TTranslatedSource, TResult>>(typeof(TTranslatedSource));

        /// <summary>
        /// 使用自定义映射规则将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource">目标表达式的参数类型。</typeparam>
        /// <param name="action">配置映射规则的委托。</param>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource, TResult>> IExpressionTranslation<TSource, TResult>.To<TTranslatedSource>(Action<TranslatePathMapper<Func<TSource, TResult>>> action) => base.To<Func<TTranslatedSource, TResult>>(action);
    }

    /// <summary>
    /// 表达式翻译器的双参数实现。
    /// 用于转换双参数 Lambda 表达式到另一个相似但具有不同参数类型的表达式。
    /// </summary>
    /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
    /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    internal sealed class ExpressionTranslation<T1, T2, TResult> : ExpressionTranslation<Func<T1, T2, TResult>>, IExpressionTranslation<T1, T2, TResult>
    {
        /// <summary>
        /// 初始化 <see cref="ExpressionTranslation{T1, T2, TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">要翻译的源表达式。</param>
        public ExpressionTranslation(Expression<Func<T1, T2, TResult>> predicate) : base(predicate)
        {
        }

        /// <summary>
        /// 将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TResult>> IExpressionTranslation<T1, T2, TResult>.To<TTranslatedSource1, TTranslatedSource2>() => To<Func<TTranslatedSource1, TTranslatedSource2, TResult>>(typeof(TTranslatedSource1), typeof(TTranslatedSource2));

        /// <summary>
        /// 使用自定义映射规则将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <param name="action">配置映射规则的委托。</param>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TResult>> IExpressionTranslation<T1, T2, TResult>.To<TTranslatedSource1, TTranslatedSource2>(Action<TranslatePathMapper<Func<T1, T2, TResult>>> action) => base.To<Func<TTranslatedSource1, TTranslatedSource2, TResult>>(action);
    }

    /// <summary>
    /// 表达式翻译器的三参数实现。
    /// 用于转换三参数 Lambda 表达式到另一个相似但具有不同参数类型的表达式。
    /// </summary>
    /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
    /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
    /// <typeparam name="T3">源表达式的第三个参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    internal sealed class ExpressionTranslation<T1, T2, T3, TResult> : ExpressionTranslation<Func<T1, T2, T3, TResult>>, IExpressionTranslation<T1, T2, T3, TResult>
    {
        /// <summary>
        /// 初始化 <see cref="ExpressionTranslation{T1, T2, T3, TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">要翻译的源表达式。</param>
        public ExpressionTranslation(Expression<Func<T1, T2, T3, TResult>> predicate) : base(predicate)
        {
        }

        /// <summary>
        /// 将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TResult>> IExpressionTranslation<T1, T2, T3, TResult>.To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3>() => To<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TResult>>(typeof(TTranslatedSource1), typeof(TTranslatedSource2), typeof(TTranslatedSource3));

        /// <summary>
        /// 使用自定义映射规则将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <param name="action">配置映射规则的委托。</param>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TResult>> IExpressionTranslation<T1, T2, T3, TResult>.To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3>(Action<TranslatePathMapper<Func<T1, T2, T3, TResult>>> action) => base.To<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TResult>>(action);
    }

    /// <summary>
    /// 表达式翻译器的四参数实现。
    /// 用于转换四参数 Lambda 表达式到另一个相似但具有不同参数类型的表达式。
    /// </summary>
    /// <typeparam name="T1">源表达式的第一个参数类型。</typeparam>
    /// <typeparam name="T2">源表达式的第二个参数类型。</typeparam>
    /// <typeparam name="T3">源表达式的第三个参数类型。</typeparam>
    /// <typeparam name="T4">源表达式的第四个参数类型。</typeparam>
    /// <typeparam name="TResult">表达式的返回类型。</typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    internal sealed class ExpressionTranslation<T1, T2, T3, T4, TResult> : ExpressionTranslation<Func<T1, T2, T3, T4, TResult>>, IExpressionTranslation<T1, T2, T3, T4, TResult>
    {
        /// <summary>
        /// 初始化 <see cref="ExpressionTranslation{T1, T2, T3, T4, TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">要翻译的源表达式。</param>
        public ExpressionTranslation(Expression<Func<T1, T2, T3, T4, TResult>> predicate) : base(predicate)
        {
        }

        /// <summary>
        /// 将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource4">目标表达式的第四个参数类型。</typeparam>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4, TResult>> IExpressionTranslation<T1, T2, T3, T4, TResult>.To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4>() => To<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4, TResult>>(typeof(TTranslatedSource1), typeof(TTranslatedSource2), typeof(TTranslatedSource3), typeof(TTranslatedSource4));

        /// <summary>
        /// 使用自定义映射规则将表达式转换为使用新参数类型的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource1">目标表达式的第一个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource2">目标表达式的第二个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource3">目标表达式的第三个参数类型。</typeparam>
        /// <typeparam name="TTranslatedSource4">目标表达式的第四个参数类型。</typeparam>
        /// <param name="action">配置映射规则的委托。</param>
        /// <returns>转换后的表达式。</returns>
        Expression<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4, TResult>> IExpressionTranslation<T1, T2, T3, T4, TResult>.To<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4>(Action<TranslatePathMapper<Func<T1, T2, T3, T4, TResult>>> action) => base.To<Func<TTranslatedSource1, TTranslatedSource2, TTranslatedSource3, TTranslatedSource4, TResult>>(action);
    }

}
