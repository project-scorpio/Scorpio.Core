using System.Linq.Expressions;

using Scorpio;

namespace System.Linq
{
    /// <summary>
    /// 为 <see cref="IQueryable{T}"/> 提供一些有用的扩展方法。
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 用于分页。可以作为 Skip(...).Take(...) 链式调用的替代方案。
        /// </summary>
        /// <typeparam name="T">查询序列中的元素类型。</typeparam>
        /// <param name="query">要应用分页的查询。</param>
        /// <param name="skipCount">要跳过的元素数量。</param>
        /// <param name="maxResultCount">要获取的最大元素数量。</param>
        /// <returns>应用了分页的查询。</returns>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            Check.NotNullOrDefault(query, nameof(query));

            return query.Skip(skipCount).Take(maxResultCount);
        }

        /// <summary>
        /// 用于分页。可以作为 Skip(...).Take(...) 链式调用的替代方案。
        /// </summary>
        /// <typeparam name="T">查询序列中的元素类型。</typeparam>
        /// <typeparam name="TQueryable">查询的具体类型，必须实现 <see cref="IQueryable{T}"/> 接口。</typeparam>
        /// <param name="query">要应用分页的查询。</param>
        /// <param name="skipCount">要跳过的元素数量。</param>
        /// <param name="maxResultCount">要获取的最大元素数量。</param>
        /// <returns>应用了分页的查询，保留原始查询类型。</returns>
        public static TQueryable PageBy<T, TQueryable>(this TQueryable query, int skipCount, int maxResultCount)
            where TQueryable : IQueryable<T>
        {
            Check.NotNullOrDefault(query, nameof(query));

            return (TQueryable)query.Skip(skipCount).Take(maxResultCount);
        }

        /// <summary>
        /// 如果给定条件为真，则按给定谓词筛选 <see cref="IQueryable{T}"/>。
        /// </summary>
        /// <typeparam name="T">查询序列中的元素类型。</typeparam>
        /// <param name="query">要应用筛选的查询。</param>
        /// <param name="condition">布尔值条件。</param>
        /// <param name="predicate">用于筛选查询的谓词表达式。</param>
        /// <returns>根据 <paramref name="condition"/> 条件决定是否筛选的查询结果。</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            Check.NotNull(query, nameof(query));

            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 如果给定条件为真，则按给定谓词筛选 <see cref="IQueryable{T}"/>。
        /// </summary>
        /// <typeparam name="T">查询序列中的元素类型。</typeparam>
        /// <typeparam name="TQueryable">查询的具体类型，必须实现 <see cref="IQueryable{T}"/> 接口。</typeparam>
        /// <param name="query">要应用筛选的查询。</param>
        /// <param name="condition">布尔值条件。</param>
        /// <param name="predicate">用于筛选查询的谓词表达式。</param>
        /// <returns>根据 <paramref name="condition"/> 条件决定是否筛选的查询结果，保留原始查询类型。</returns>
        public static TQueryable WhereIf<T, TQueryable>(this TQueryable query, bool condition, Expression<Func<T, bool>> predicate)
            where TQueryable : IQueryable<T>
        {
            Check.NotNullOrDefault(query, nameof(query));

            return condition
                ? (TQueryable)query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 如果给定条件为真，则按给定谓词筛选 <see cref="IQueryable{T}"/>。
        /// 此重载使用带索引的筛选谓词。
        /// </summary>
        /// <typeparam name="T">查询序列中的元素类型。</typeparam>
        /// <param name="query">要应用筛选的查询。</param>
        /// <param name="condition">布尔值条件。</param>
        /// <param name="predicate">用于筛选查询的谓词表达式，包含元素索引参数。</param>
        /// <returns>根据 <paramref name="condition"/> 条件决定是否筛选的查询结果。</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
        {
            Check.NotNull(query, nameof(query));

            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 如果给定条件为真，则按给定谓词筛选 <see cref="IQueryable{T}"/>。
        /// 此重载使用带索引的筛选谓词。
        /// </summary>
        /// <typeparam name="T">查询序列中的元素类型。</typeparam>
        /// <typeparam name="TQueryable">查询的具体类型，必须实现 <see cref="IQueryable{T}"/> 接口。</typeparam>
        /// <param name="query">要应用筛选的查询。</param>
        /// <param name="condition">布尔值条件。</param>
        /// <param name="predicate">用于筛选查询的谓词表达式，包含元素索引参数。</param>
        /// <returns>根据 <paramref name="condition"/> 条件决定是否筛选的查询结果，保留原始查询类型。</returns>
        public static TQueryable WhereIf<T, TQueryable>(this TQueryable query, bool condition, Expression<Func<T, int, bool>> predicate)
            where TQueryable : IQueryable<T>
        {
            Check.NotNullOrDefault(query, nameof(query));

            return condition
                ? (TQueryable)query.Where(predicate)
                : query;
        }
    }
}
