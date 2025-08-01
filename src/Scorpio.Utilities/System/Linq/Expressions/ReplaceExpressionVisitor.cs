namespace System.Linq.Expressions
{
    /// <summary>
    /// 表达式替换访问器。
    /// 用于在表达式树中将一个表达式节点替换为另一个表达式节点。
    /// </summary>
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 需要被替换的原始表达式节点。
        /// </summary>
        private readonly Expression _oldValue;

        /// <summary>
        /// 用于替换的新表达式节点。
        /// </summary>
        private readonly Expression _newValue;

        /// <summary>
        /// 初始化 <see cref="ReplaceExpressionVisitor"/> 类的新实例。
        /// </summary>
        /// <param name="oldValue">需要被替换的原始表达式节点。</param>
        /// <param name="newValue">用于替换的新表达式节点。</param>
        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        /// <summary>
        /// 访问参数表达式节点。
        /// 如果参数表达式与需要被替换的原始表达式匹配，则返回新的表达式节点。
        /// </summary>
        /// <param name="node">要访问的参数表达式。</param>
        /// <returns>
        /// 如果参数表达式与需要被替换的表达式相同，则返回新的表达式节点；
        /// 否则返回原始参数表达式或其修改版本。
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == _oldValue)
            {
                return _newValue;
            }
            return base.VisitParameter(node);
        }

        /// <summary>
        /// 访问调用表达式节点。
        /// 处理当被调用的表达式需要被替换，且替换表达式是 Lambda 表达式的特殊情况。
        /// </summary>
        /// <param name="node">要访问的调用表达式。</param>
        /// <returns>
        /// 如果调用表达式的被调用对象与需要被替换的表达式相同，且新表达式是 Lambda 表达式，
        /// 则将 Lambda 表达式的参数替换为调用表达式的参数后返回 Lambda 表达式的主体；
        /// 否则返回原始调用表达式或其修改版本。
        /// </returns>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            if (node.Expression == _oldValue && _newValue is LambdaExpression lambda)
            {
                var buiders = lambda.Parameters.Zip(node.Arguments, (p, a) => new ReplaceExpressionVisitor(p, a));
                return buiders.Aggregate(lambda.Body, (e, b) => b.Visit(e));
            }
            return base.VisitInvocation(node);
        }
    }
}
