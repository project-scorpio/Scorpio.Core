namespace System.Linq.Expressions
{
    /// <summary>
    /// 成员初始化表达式转换器。
    /// 用于将一个类型的成员初始化表达式转换为另一个类型的成员初始化表达式，保持属性映射关系。
    /// </summary>
    /// <typeparam name="TSource">源表达式的类型。</typeparam>
    public sealed class MemberInitTranslation<TSource>
    {
        /// <summary>
        /// 需要被转换的源成员初始化表达式。
        /// </summary>
        private readonly Expression<Func<TSource, TSource>> _predicate;

        /// <summary>
        /// 初始化 <see cref="MemberInitTranslation{TSource}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">要转换的源成员初始化表达式。</param>
        internal MemberInitTranslation(Expression<Func<TSource, TSource>> predicate) => _predicate = predicate;

        /// <summary>
        /// 将源类型的成员初始化表达式转换为目标类型的成员初始化表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource">目标表达式的类型。</typeparam>
        /// <returns>转换后的成员初始化表达式，将源类型的属性映射到目标类型的同名属性。</returns>
        /// <remarks>
        /// 此方法假设目标类型具有与源类型相同名称的属性。它会为每个在源表达式中初始化的属性
        /// 创建相应的绑定，用于初始化目标类型的同名属性。
        /// </remarks>
        public Expression<Func<TTranslatedSource, TTranslatedSource>> To<TTranslatedSource>()
        {
            var type = typeof(TTranslatedSource);
            var s = _predicate.Parameters[0];
            var t = Expression.Parameter(type, s.Name);
            var init = _predicate.Body as MemberInitExpression;
            var binds = init.Bindings
                .OfType<MemberAssignment>()
                .Select(b => (b.Member.Name, b.Expression))
                .Select(b => Expression.Bind(type.GetProperty(b.Name), GetExpression(t, b.Expression, b.Name)))
                .ToList();
            return Expression.Lambda<Func<TTranslatedSource, TTranslatedSource>>(
                 Expression.MemberInit(Expression.New(typeof(TTranslatedSource)), binds), t);

            // <summary>
            // 根据表达式类型获取适当的表达式。
            // </summary>
            // <param name="parameter">目标类型的参数表达式。</param>
            // <param name="expression">源表达式中的成员赋值表达式。</param>
            // <param name="propertyName">属性名称。</param>
            // <returns>
            // 如果源表达式是成员访问表达式，则返回目标类型的相应属性访问表达式；
            // 否则返回原始表达式。
            // </returns>
            static Expression GetExpression(Expression parameter,
                                            Expression expression,
                                            string propertyName)
            {
                return expression.NodeType switch
                {
                    ExpressionType.MemberAccess => Expression.Property(parameter, propertyName),
                    _ => expression
                };
            }
        }
    }
}
