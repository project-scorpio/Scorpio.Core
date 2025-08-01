using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Scorpio;

namespace System.Linq.Async

{
    /// <summary>
    /// 提供一组用于异步查询操作的静态方法，支持通过 <see cref="IAsyncQueryProvider"/> 执行异步 LINQ 查询。
    /// </summary>
    public static partial class AsyncQueryable
    {
        #region Impl.

        /// <summary>
        /// 通过指定的方法信息和表达式执行异步查询操作。
        /// </summary>
        /// <typeparam name="TSource">查询源中元素的类型。</typeparam>
        /// <typeparam name="TResult">异步操作的结果类型。</typeparam>
        /// <param name="operatorMethodInfo">表示要执行的操作符方法的 <see cref="MethodInfo"/> 对象。</param>
        /// <param name="source">要对其执行异步操作的 <see cref="IQueryable{T}"/> 实例。</param>
        /// <param name="expression">可选的表达式参数，用于传递给操作符方法。</param>
        /// <param name="cancellationToken">可用于取消异步操作的取消标记。</param>
        /// <returns>异步操作的结果。</returns>
        /// <exception cref="InvalidOperationException">当查询提供程序不是 <see cref="IAsyncQueryProvider"/> 类型时抛出。</exception>
        private static TResult ExecuteAsync<TSource, TResult>(
            MethodInfo operatorMethodInfo,
            IQueryable<TSource> source,
            Expression expression,
            CancellationToken cancellationToken = default)
        {
            if (source.Provider is IAsyncQueryProvider provider)
            {
                if (operatorMethodInfo.IsGenericMethod)
                {
                    operatorMethodInfo
                        = operatorMethodInfo.GetGenericArguments().Length == 2
                            ? operatorMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TResult).GetGenericArguments().Single())
                            : operatorMethodInfo.MakeGenericMethod(typeof(TSource));
                }

                return provider.ExecuteAsync<TResult>(
                    Expression.Call(
                        instance: null,
                        method: operatorMethodInfo,
                        arguments: expression == null
                            ? new[] { source.Expression }
                            : new[] { source.Expression, expression }),
                    cancellationToken);
            }

            throw new InvalidOperationException("IQueryableProviderNotAsync");
        }

        /// <summary>
        /// 通过指定的方法信息和 Lambda 表达式执行异步查询操作。
        /// </summary>
        /// <typeparam name="TSource">查询源中元素的类型。</typeparam>
        /// <typeparam name="TResult">异步操作的结果类型。</typeparam>
        /// <param name="operatorMethodInfo">表示要执行的操作符方法的 <see cref="MethodInfo"/> 对象。</param>
        /// <param name="source">要对其执行异步操作的 <see cref="IQueryable{T}"/> 实例。</param>
        /// <param name="expression">要传递给操作符方法的 Lambda 表达式。</param>
        /// <param name="cancellationToken">可用于取消异步操作的取消标记。</param>
        /// <returns>异步操作的结果。</returns>
        private static TResult ExecuteAsync<TSource, TResult>(
            MethodInfo operatorMethodInfo,
            IQueryable<TSource> source,
            LambdaExpression expression,
            CancellationToken cancellationToken = default)
        {
            return ExecuteAsync<TSource, TResult>(
                           operatorMethodInfo, source, Expression.Quote(expression), cancellationToken);
        }

        /// <summary>
        /// 通过指定的方法信息执行异步查询操作，不需要额外的表达式参数。
        /// </summary>
        /// <typeparam name="TSource">查询源中元素的类型。</typeparam>
        /// <typeparam name="TResult">异步操作的结果类型。</typeparam>
        /// <param name="operatorMethodInfo">表示要执行的操作符方法的 <see cref="MethodInfo"/> 对象。</param>
        /// <param name="source">要对其执行异步操作的 <see cref="IQueryable{T}"/> 实例。</param>
        /// <param name="cancellationToken">可用于取消异步操作的取消标记。</param>
        /// <returns>异步操作的结果。</returns>
        private static TResult ExecuteAsync<TSource, TResult>(
            MethodInfo operatorMethodInfo,
            IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return ExecuteAsync<TSource, TResult>(
                           operatorMethodInfo, source, (Expression)null, cancellationToken);
        }

        #endregion
    }
}
