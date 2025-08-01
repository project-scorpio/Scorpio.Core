namespace System.Linq.Expressions
{
    /// <summary>
    /// 提供用于构建和组合表达式谓词的静态方法集合。
    /// 此类可用于动态构建 LINQ 查询的条件表达式。
    /// </summary>
    public static partial class PredicateBuilder
    {
        /// <summary>
        /// 创建一个始终返回 true 的谓词表达式。
        /// </summary>
        /// <typeparam name="T">表达式参数的类型。</typeparam>
        /// <returns>一个接受 <typeparamref name="T"/> 类型参数并始终返回 true 的表达式。</returns>
        public static Expression<Func<T, bool>> True<T>() => f => true;

        /// <summary>
        /// 创建一个始终返回 false 的谓词表达式。
        /// </summary>
        /// <typeparam name="T">表达式参数的类型。</typeparam>
        /// <returns>一个接受 <typeparamref name="T"/> 类型参数并始终返回 false 的表达式。</returns>
        public static Expression<Func<T, bool>> False<T>() => f => false;

        /// <summary>
        /// 使用等于操作符组合两个表达式树。
        /// </summary>
        /// <typeparam name="T">表达式参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示检查两个输入表达式结果是否相等。</returns>
        public static Expression<Func<T, TResult>> Equal<T, TResult>(
            this Expression<Func<T, TResult>> left,
            Expression<Func<T, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.Equal(lft, rit));


        /// <summary>
        /// 使用等于操作符组合两个双参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示检查两个输入表达式结果是否相等。</returns>
        public static Expression<Func<T1, T2, TResult>> Equal<T1, T2, TResult>(
            this Expression<Func<T1, T2, TResult>> left,
                 Expression<Func<T1, T2, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.Equal(lft, rit));

        /// <summary>
        /// 使用等于操作符组合两个三参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示检查两个输入表达式结果是否相等。</returns>
        public static Expression<Func<T1, T2, T3, TResult>> Equal<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> left,
            Expression<Func<T1, T2, T3, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.Equal(lft, rit));

        /// <summary>
        /// 使用等于操作符组合两个四参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="T4">表达式第四个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示检查两个输入表达式结果是否相等。</returns>
        public static Expression<Func<T1, T2, T3, T4, TResult>> Equal<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> left,
            Expression<Func<T1, T2, T3, T4, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.Equal(lft, rit));

        /// <summary>
        /// 使用等于操作符组合两个五参数表达式树。
        /// </summary>
        /// <typeparam name="T1">表达式第一个参数的类型。</typeparam>
        /// <typeparam name="T2">表达式第二个参数的类型。</typeparam>
        /// <typeparam name="T3">表达式第三个参数的类型。</typeparam>
        /// <typeparam name="T4">表达式第四个参数的类型。</typeparam>
        /// <typeparam name="T5">表达式第五个参数的类型。</typeparam>
        /// <typeparam name="TResult">表达式结果的类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <returns>一个新的组合表达式，表示检查两个输入表达式结果是否相等。</returns>
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> Equal<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> left,
            Expression<Func<T1, T2, T3, T4, T5, TResult>> right) => BinaryCombine(left, right, (lft, rit) => Expression.Equal(lft, rit));

        /// <summary>
        /// 通用方法，用于使用指定的二元操作符组合两个表达式树。
        /// </summary>
        /// <typeparam name="TDelegate">表达式的委托类型。</typeparam>
        /// <param name="left">左侧表达式。</param>
        /// <param name="right">右侧表达式。</param>
        /// <param name="combineFunc">用于组合表达式的函数，接收左右两个表达式，返回一个二元表达式。</param>
        /// <returns>组合后的新表达式。</returns>
        public static Expression<TDelegate> BinaryCombine<TDelegate>(
            this Expression<TDelegate> left,
            Expression<TDelegate> right,
            Func<Expression, Expression, BinaryExpression> combineFunc)
        {
            var (lft, rit, parameters) = MergeExpressionAndParameters(left, right);
            return Expression.Lambda<TDelegate>(combineFunc(lft, rit), parameters);
        }

        /// <summary>
        /// 合并两个表达式的参数，创建共享参数的新表达式主体。
        /// </summary>
        /// <param name="left">左侧 Lambda 表达式。</param>
        /// <param name="right">右侧 Lambda 表达式。</param>
        /// <returns>一个包含修改后的左右表达式主体和新参数数组的元组。</returns>
        private static (Expression left, Expression right, ParameterExpression[] parameters) MergeExpressionAndParameters(
            LambdaExpression left,
            LambdaExpression right)
        {
            var parameters = new ParameterExpression[left.Parameters.Count];
            var leftExpression = left.Body;
            var rightExpression = right.Body;
            for (var i = 0; i < left.Parameters.Count; i++)
            {
                parameters[i] = Expression.Parameter(left.Parameters[i].Type);
                var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[i], parameters[i]);
                leftExpression = leftVisitor.Visit(leftExpression);

                var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[i], parameters[i]);
                rightExpression = rightVisitor.Visit(rightExpression);
            }
            return (leftExpression, rightExpression, parameters);
        }

    }
}
