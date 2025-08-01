using System.Collections.Generic;

namespace System.Linq.Expressions
{
    /// <summary>
    /// 表达式翻译器的抽象基类。
    /// 提供将一种类型的表达式转换为另一种类型表达式的基本功能。
    /// </summary>
    /// <typeparam name="TDelegate">源表达式的委托类型。</typeparam>
    internal abstract class ExpressionTranslation<TDelegate>
    {
        /// <summary>
        /// 需要被转换的源表达式。
        /// </summary>
        private readonly Expression<TDelegate> _predicate;

        /// <summary>
        /// 初始化 <see cref="ExpressionTranslation{TDelegate}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">需要转换的源表达式。</param>
        protected ExpressionTranslation(Expression<TDelegate> predicate) => _predicate = predicate;

        /// <summary>
        /// 将源表达式转换为使用指定参数类型的新表达式。
        /// </summary>
        /// <typeparam name="TTranslatedDelegate">目标表达式的委托类型。</typeparam>
        /// <param name="parameterTypes">新表达式中参数的类型数组。</param>
        /// <returns>转换后的表达式。</returns>
        protected Expression<TTranslatedDelegate> To<TTranslatedDelegate>(params Type[] parameterTypes) => To<TTranslatedDelegate>((IEnumerable<Type>)parameterTypes);

        /// <summary>
        /// 将源表达式转换为使用指定参数类型的新表达式。
        /// </summary>
        /// <typeparam name="TTranslatedDelegate">目标表达式的委托类型。</typeparam>
        /// <param name="parameterTypes">新表达式中参数的类型集合。</param>
        /// <returns>转换后的表达式。</returns>
        protected Expression<TTranslatedDelegate> To<TTranslatedDelegate>(IEnumerable<Type> parameterTypes)
        {
            var (expression, parameters) = MergeExpressionAndParameters(parameterTypes);
            return Expression.Lambda<TTranslatedDelegate>(expression, parameters);
        }

        /// <summary>
        /// 合并源表达式的主体和新创建的参数。
        /// </summary>
        /// <param name="parameterTypes">新表达式中参数的类型集合。</param>
        /// <returns>一个包含转换后的表达式主体和参数数组的元组。</returns>
        private (Expression expression, ParameterExpression[] parameters) MergeExpressionAndParameters(IEnumerable<Type> parameterTypes)
        {
            var parameters = new ParameterExpression[_predicate.Parameters.Count];
            _predicate.Parameters.CopyTo(parameters, 0);
            var expression = _predicate.Body;
            var types = parameterTypes.GetEnumerator();
            for (var i = 0; i < _predicate.Parameters.Count; i++)
            {
                types.MoveNext();
                var s = _predicate.Parameters[i];
                parameters[i] = Expression.Parameter(types.Current, s.Name);
                var binder = new ReplaceExpressionVisitor(s, parameters[i]);
                expression = binder.Visit(expression);
            }
            return (expression, parameters);
        }

        /// <summary>
        /// 使用自定义映射规则将源表达式转换为新表达式。
        /// </summary>
        /// <typeparam name="TTranslatedDelegate">目标表达式的委托类型。</typeparam>
        /// <param name="action">用于配置表达式转换映射规则的委托。</param>
        /// <returns>转换后的表达式。</returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="action"/> 为 <see langword="null"/> 时抛出。
        /// </exception>
        public Expression<TTranslatedDelegate> To<TTranslatedDelegate>(Action<TranslatePathMapper<TDelegate>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var mapper = new TranslatePathMapper<TDelegate>(_predicate);
            action(mapper);
            var (expression, parameters) = mapper.MergeExpressionAndParameters<TTranslatedDelegate>();

            return Expression.Lambda<TTranslatedDelegate>(expression, parameters);
        }
    }

}
