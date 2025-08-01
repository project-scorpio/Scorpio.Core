using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// 表达式路径映射器。
    /// 用于在表达式转换过程中配置源类型路径到目标类型路径的映射规则。
    /// </summary>
    /// <typeparam name="TDelegate">要映射的表达式的委托类型。</typeparam>
    public sealed class TranslatePathMapper<TDelegate>
    {
        /// <summary>
        /// 源表达式。
        /// </summary>
        private readonly Expression<TDelegate> _predicate;

        /// <summary>
        /// 存储所有映射路径表达式的集合。
        /// </summary>
        private readonly List<LambdaExpression> _expressions = new List<LambdaExpression>();

        /// <summary>
        /// 初始化 <see cref="TranslatePathMapper{TDelegate}"/> 类的新实例。
        /// </summary>
        /// <param name="predicate">源表达式。</param>
        internal TranslatePathMapper(Expression<TDelegate> predicate) => _predicate = predicate;

        /// <summary>
        /// 添加从目标类型到源类型的映射路径。
        /// </summary>
        /// <typeparam name="TSource">源类型。</typeparam>
        /// <typeparam name="TTranslatedSource">目标类型。</typeparam>
        /// <param name="path">定义从目标类型到源类型转换的表达式路径。</param>
        /// <returns>当前映射器实例，支持链式调用。</returns>
        public TranslatePathMapper<TDelegate> Map<TSource, TTranslatedSource>(Expression<Func<TTranslatedSource, TSource>> path)
        {
            _expressions.Add(path);
            return this;
        }

        /// <summary>
        /// 合并源表达式和目标参数，创建新的表达式。
        /// </summary>
        /// <typeparam name="TTranslatedDelegate">转换后表达式的委托类型。</typeparam>
        /// <returns>包含转换后表达式主体和新参数数组的元组。</returns>
        internal (Expression expression, ParameterExpression[] parameters) MergeExpressionAndParameters<TTranslatedDelegate>()
        {
            var parameters = new ParameterExpression[_predicate.Parameters.Count];
            _predicate.Parameters.CopyTo(parameters, 0);
            var expression = _predicate.Body;
            var translatedParameters = (typeof(TTranslatedDelegate)).GenericTypeArguments;
            for (var i = 0; i < _predicate.Parameters.Count; i++)
            {
                var s = _predicate.Parameters[i];
                var path = _expressions.Find(e => e.ReturnType == s.Type && e.Type.GenericTypeArguments[0] == translatedParameters[i]);
                if (path == null)
                {
                    continue;
                }
                path = TranslatePathMapper.UpdateExpression(path, s.Name);
                parameters[i] = path.Parameters[0];
                var binder = new ReplaceExpressionVisitor(s, path.Body);
                expression = binder.Visit(expression);
            }
            return (expression, parameters);
        }
    }

    /// <summary>
    /// 提供用于处理表达式路径映射的静态工具方法。
    /// </summary>
    internal static class TranslatePathMapper
    {
        /// <summary>
        /// 存储通用 Update 方法的方法信息，用于通过反射调用。
        /// </summary>
        private static readonly MethodInfo _method = ((Func<LambdaExpression, string, LambdaExpression>)Update<object, object>).Method.GetGenericMethodDefinition();

        /// <summary>
        /// 更新 Lambda 表达式，将其参数名称修改为指定的名称。
        /// </summary>
        /// <param name="expression">要更新的 Lambda 表达式。</param>
        /// <param name="name">新的参数名称。</param>
        /// <returns>更新后的 Lambda 表达式。</returns>
        public static LambdaExpression UpdateExpression(LambdaExpression expression, string name)
        {
            var method = _method.MakeGenericMethod(expression.Parameters[0].Type, expression.ReturnType);
            return method.Invoke(null, new object[] { expression, name }) as LambdaExpression;
        }

        /// <summary>
        /// 使用新的参数名称更新特定类型的 Lambda 表达式。
        /// </summary>
        /// <typeparam name="TTranslatedSource">Lambda 表达式的参数类型。</typeparam>
        /// <typeparam name="TSource">Lambda 表达式的返回类型。</typeparam>
        /// <param name="path">要更新的 Lambda 表达式。</param>
        /// <param name="name">新的参数名称。</param>
        /// <returns>更新后的 Lambda 表达式。</returns>
        private static LambdaExpression Update<TTranslatedSource, TSource>(LambdaExpression path, string name)
        {
            var expression = path as Expression<Func<TTranslatedSource, TSource>>;
            var para = Expression.Parameter(typeof(TTranslatedSource), name);
            var body = (expression.Body as UnaryExpression).Update(para);
            return expression.Update(body, new ParameterExpression[] { para });
        }
    }

}
