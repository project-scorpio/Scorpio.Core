namespace System.Linq.Expressions
{
    /// <summary>
    /// 提供用于构建和组合表达式谓词的辅助方法。
    /// 该类提供了各种重载方法，用于使用 OR 和 AND 逻辑运算符组合表达式树。
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "<挂起>")]
    public partial class PredicateBuilder
    {

        /// <summary>
        /// 使用逻辑 OR 操作符组合两个表达式树。
        /// </summary>
        /// <typeparam name="T">表达式参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 OR 操作。</returns>
        public static Expression<Func<T, TResult>> OrElse<T, TResult>(
            this Expression<Func<T, TResult>> left,
            Expression<Func<T, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.OrElse(lft, rit));

        /// <summary>
        /// 使用逻辑 OR 操作符组合两个双参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 OR 操作。</returns>
        public static Expression<Func<T1, T2, TResult>> OrElse<T1, T2, TResult>(
            this Expression<Func<T1, T2, TResult>> left,
            Expression<Func<T1, T2, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.OrElse(lft, rit));

        /// <summary>
        /// 使用逻辑 OR 操作符组合两个三参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 OR 操作。</returns>
        public static Expression<Func<T1, T2, T3, TResult>> OrElse<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> left,
            Expression<Func<T1, T2, T3, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.OrElse(lft, rit));

        /// <summary>
        /// 使用逻辑 OR 操作符组合两个四参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="T4">表达式第四个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 OR 操作。</returns>
        public static Expression<Func<T1, T2, T3, T4, TResult>> OrElse<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> left,
            Expression<Func<T1, T2, T3, T4, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.OrElse(lft, rit));

        /// <summary>
        /// 使用逻辑 OR 操作符组合两个五参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="T4">表达式第四个参数的类型。</typeparam>
        /// <typeparam name="T5">表达式第五个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 OR 操作。</returns>
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> OrElse<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> left,
            Expression<Func<T1, T2, T3, T4, T5, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.OrElse(lft, rit));

        /// <summary>
        /// 使用逻辑 AND 操作符组合两个表达式树。
        /// </summary>
        /// <typeparam name="T">表达式参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 AND 操作。</returns>
        public static Expression<Func<T, TResult>> AndAlso<T, TResult>(
            this Expression<Func<T, TResult>> left,
            Expression<Func<T, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.AndAlso(lft, rit));


        /// <summary>
        /// 使用逻辑 AND 操作符组合两个双参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 AND 操作。</returns>
        public static Expression<Func<T1, T2, TResult>> AndAlso<T1, T2, TResult>(
            this Expression<Func<T1, T2, TResult>> left,
                 Expression<Func<T1, T2, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.AndAlso(lft, rit));

        /// <summary>
        /// 使用逻辑 AND 操作符组合两个三参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 AND 操作。</returns>
        public static Expression<Func<T1, T2, T3, TResult>> AndAlso<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> left,
            Expression<Func<T1, T2, T3, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.AndAlso(lft, rit));

        /// <summary>
        /// 使用逻辑 AND 操作符组合两个四参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="T4">表达式第四个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 AND 操作。</returns>
        public static Expression<Func<T1, T2, T3, T4, TResult>> AndAlso<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> left,
            Expression<Func<T1, T2, T3, T4, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.AndAlso(lft, rit));

        /// <summary>
        /// 使用逻辑 AND 操作符组合两个五参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="T4">表达式第四个参数的类型。</typeparam>
        /// <typeparam name="T5">表达式第五个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示两个输入表达式的逻辑 AND 操作。</returns>
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> AndAlso<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> left,
            Expression<Func<T1, T2, T3, T4, T5, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.AndAlso(lft, rit));

    }
}
