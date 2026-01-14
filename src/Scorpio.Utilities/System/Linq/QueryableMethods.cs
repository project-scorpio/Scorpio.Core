using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Scorpio;

namespace System.Linq
{

    /// <summary>
    ///     提供可翻译 LINQ 方法的反射元数据的类。
    /// </summary>
    /// <remarks>
    ///     更多信息和示例，请参见 <see href="https://aka.ms/efcore-docs-providers">数据库提供程序和扩展的实现</see>
    ///     和 <see href="https://aka.ms/efcore-docs-how-query-works">EF Core 查询工作原理</see>。
    /// </remarks>
    public static class QueryableMethods
    {

        /// <summary>
        ///     <see cref="Queryable.All{TResult}" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo All { get; }

        /// <summary>
        ///     <see cref="Queryable.Any{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo AnyWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.Any{TSource}(IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo AnyWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.AsQueryable{TElement}" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo AsQueryable { get; }

        /// <summary>
        ///     <see cref="Queryable.Cast{TResult}" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Cast { get; }

        /// <summary>
        ///     <see cref="Queryable.Concat{TSource}" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Concat { get; }

        /// <summary>
        ///     <see cref="Queryable.Contains{TSource}(IQueryable{TSource},TSource)" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Contains { get; }

        /// <summary>
        ///     <see cref="Queryable.Count{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo CountWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.Count{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo CountWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.DefaultIfEmpty{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo DefaultIfEmptyWithoutArgument { get; }

        /// <summary>
        ///     <see cref="Queryable.DefaultIfEmpty{TSource}(IQueryable{TSource},TSource)" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo DefaultIfEmptyWithArgument { get; }

        /// <summary>
        ///     <see cref="Queryable.Distinct{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Distinct { get; }

        /// <summary>
        ///     <see cref="Queryable.ElementAt{TSource}(IQueryable{TSource}, int)" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo ElementAt { get; }

        /// <summary>
        ///     <see cref="Queryable.ElementAtOrDefault{TSource}(IQueryable{TSource}, int)" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo ElementAtOrDefault { get; }

        /// <summary>
        ///     <see cref="Queryable.Except{TSource}(IQueryable{TSource},IEnumerable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Except { get; }

        /// <summary>
        ///     <see cref="Queryable.First{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo FirstWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.First{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo FirstWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.FirstOrDefault{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo FirstOrDefaultWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.FirstOrDefault{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo FirstOrDefaultWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.GroupBy{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo GroupByWithKeySelector { get; }

        /// <summary>
        ///     <see cref="Queryable.GroupBy{TSource,TKey,TElement}(IQueryable{TSource},Expression{Func{TSource,TKey}},Expression{Func{TSource,TElement}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo GroupByWithKeyElementSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.GroupBy{TSource,TKey,TElement,TResult}(IQueryable{TSource},Expression{Func{TSource,TKey}},Expression{Func{TSource,TElement}},Expression{Func{TKey,IEnumerable{TElement},TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo GroupByWithKeyElementResultSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.GroupBy{TSource,TKey,TResult}(IQueryable{TSource},Expression{Func{TSource,TKey}},Expression{Func{TKey,IEnumerable{TSource},TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo GroupByWithKeyResultSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.GroupJoin{TOuter,TInner,TKey,TResult}(IQueryable{TOuter},IEnumerable{TInner},Expression{Func{TOuter,TKey}},Expression{Func{TInner,TKey}},Expression{Func{TOuter,IEnumerable{TInner},TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo GroupJoin { get; }

        /// <summary>
        ///     <see cref="Queryable.Intersect{TSource}(IQueryable{TSource},IEnumerable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Intersect { get; }

        /// <summary>
        ///     <see cref="Queryable.Join{TOuter,TInner,TKey,TResult}(IQueryable{TOuter},IEnumerable{TInner},Expression{Func{TOuter,TKey}},Expression{Func{TInner,TKey}},Expression{Func{TOuter,TInner,TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Join { get; }

        /// <summary>
        ///     <see cref="Queryable.Last{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo LastWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.Last{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo LastWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.LastOrDefault{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo LastOrDefaultWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.LastOrDefault{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo LastOrDefaultWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.LongCount{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo LongCountWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.LongCount{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo LongCountWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.Max{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo MaxWithoutSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.Max{TSource,TResult}(IQueryable{TSource},Expression{Func{TSource,TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo MaxWithSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.Min{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo MinWithoutSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.Min{TSource,TResult}(IQueryable{TSource},Expression{Func{TSource,TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo MinWithSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.OfType{TResult}" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo OfType { get; }

        /// <summary>
        ///     <see cref="Queryable.OrderBy{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo OrderBy { get; }

        /// <summary>
        ///     <see cref="Queryable.OrderByDescending{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo OrderByDescending { get; }

        /// <summary>
        ///     <see cref="Queryable.Reverse{TSource}" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Reverse { get; }

        /// <summary>
        ///     <see cref="Queryable.Select{TSource,TResult}(IQueryable{TSource},Expression{Func{TSource,TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Select { get; }


        /// <summary>
        ///     <see cref="Queryable.SelectMany{TSource,TResult}(IQueryable{TSource},Expression{Func{TSource,IEnumerable{TResult}}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SelectManyWithoutCollectionSelector { get; }


        /// <summary>
        ///     <see cref="Queryable.SelectMany{TSource,TCollection,TResult}(IQueryable{TSource},Expression{Func{TSource,IEnumerable{TCollection}}},Expression{Func{TSource,TCollection,TResult}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SelectManyWithCollectionSelector { get; }

        /// <summary>
        ///     <see cref="Queryable.Single{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SingleWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.Single{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SingleWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.SingleOrDefault{TSource}(IQueryable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SingleOrDefaultWithoutPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.SingleOrDefault{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SingleOrDefaultWithPredicate { get; }

        /// <summary>
        ///     <see cref="Queryable.Skip{TSource}(IQueryable{TSource}, int)" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Skip { get; }

        /// <summary>
        ///     <see cref="Queryable.SkipWhile{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo SkipWhile { get; }

        /// <summary>
        ///     <see cref="Queryable.Take{TSource}(IQueryable{TSource}, int)" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Take { get; }

        /// <summary>
        ///     <see cref="Queryable.TakeWhile{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo TakeWhile { get; }

        /// <summary>
        ///     <see cref="Queryable.ThenBy{TSource,TKey}(IOrderedQueryable{TSource},Expression{Func{TSource,TKey}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo ThenBy { get; }

        /// <summary>
        ///     <see cref="Queryable.ThenByDescending{TSource,TKey}(IOrderedQueryable{TSource},Expression{Func{TSource,TKey}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo ThenByDescending { get; }

        /// <summary>
        ///     <see cref="Queryable.Union{TSource}(IQueryable{TSource},IEnumerable{TSource})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Union { get; }

        /// <summary>
        ///     <see cref="Queryable.Where{TSource}(IQueryable{TSource},Expression{Func{TSource,bool}})" /> 的 <see cref="MethodInfo" />
        /// </summary>
        public static MethodInfo Where { get; }

        /// <summary>
        ///     检查给定的 <see cref="MethodInfo"/> 是否为无选择器的 <see cref="O:Queryable.Average"/> 方法之一。
        /// </summary>
        /// <param name="methodInfo">要检查的方法</param>
        /// <returns>如果方法匹配则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsAverageWithoutSelector(MethodInfo methodInfo)
            => AverageWithoutSelectorMethods.ContainsValue(methodInfo);

        /// <summary>
        ///     检查给定的 <see cref="MethodInfo"/> 是否为有选择器的 <see cref="O:Queryable.Average"/> 方法之一。
        /// </summary>
        /// <param name="methodInfo">要检查的方法</param>
        /// <returns>如果方法匹配则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsAverageWithSelector(MethodInfo methodInfo)
            => methodInfo.IsGenericMethod
                && AverageWithSelectorMethods.ContainsValue(methodInfo.GetGenericMethodDefinition());

        /// <summary>
        ///     检查给定的 <see cref="MethodInfo"/> 是否为无选择器的 <see cref="O:Queryable.Sum"/> 方法之一。
        /// </summary>
        /// <param name="methodInfo">要检查的方法</param>
        /// <returns>如果方法匹配则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsSumWithoutSelector(MethodInfo methodInfo)
            => SumWithoutSelectorMethods.ContainsValue(methodInfo);

        /// <summary>
        ///     检查给定的 <see cref="MethodInfo"/> 是否为有选择器的 <see cref="O:Queryable.Sum"/> 方法之一。
        /// </summary>
        /// <param name="methodInfo">要检查的方法</param>
        /// <returns>如果方法匹配则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsSumWithSelector(MethodInfo methodInfo)
            => methodInfo.IsGenericMethod
                && SumWithSelectorMethods.ContainsValue(methodInfo.GetGenericMethodDefinition());

        /// <summary>
        ///     返回指定类型的无选择器 <see cref="O:Queryable.Average"/> 方法的 <see cref="MethodInfo"/>。
        /// </summary>
        /// <param name="type">要创建的方法的泛型类型</param>
        /// <returns><see cref="MethodInfo"/> 对象</returns>
        public static MethodInfo GetAverageWithoutSelector(Type type)
            => AverageWithoutSelectorMethods[type];

        /// <summary>
        ///     返回指定类型的有选择器 <see cref="O:Queryable.Average"/> 方法的 <see cref="MethodInfo"/>。
        /// </summary>
        /// <param name="type">要创建的方法的泛型类型</param>
        /// <returns><see cref="MethodInfo"/> 对象</returns>
        public static MethodInfo GetAverageWithSelector(Type type)
            => AverageWithSelectorMethods[type];

        /// <summary>
        ///     返回指定类型的无选择器 <see cref="O:Queryable.Sum"/> 方法的 <see cref="MethodInfo"/>。
        /// </summary>
        /// <param name="type">要创建的方法的泛型类型</param>
        /// <returns><see cref="MethodInfo"/> 对象</returns>
        public static MethodInfo GetSumWithoutSelector(Type type)
            => SumWithoutSelectorMethods[type];

        /// <summary>
        ///     返回指定类型的有选择器 <see cref="O:Queryable.Sum"/> 方法的 <see cref="MethodInfo"/>。
        /// </summary>
        /// <param name="type">要创建的方法的泛型类型</param>
        /// <returns><see cref="MethodInfo"/> 对象</returns>
        public static MethodInfo GetSumWithSelector(Type type)
            => SumWithSelectorMethods[type];

        /// <summary>
        ///     存储无选择器 Average 方法的字典
        /// </summary>
        private static Dictionary<Type, MethodInfo> AverageWithoutSelectorMethods { get; }

        /// <summary>
        ///     存储有选择器 Average 方法的字典
        /// </summary>
        private static Dictionary<Type, MethodInfo> AverageWithSelectorMethods { get; }

        /// <summary>
        ///     存储无选择器 Sum 方法的字典
        /// </summary>
        private static Dictionary<Type, MethodInfo> SumWithoutSelectorMethods { get; }

        /// <summary>
        ///     存储有选择器 Sum 方法的字典
        /// </summary>
        private static Dictionary<Type, MethodInfo> SumWithSelectorMethods { get; }

        /// <summary>
        ///     静态构造函数，初始化所有 Queryable 方法的 MethodInfo
        /// </summary>
        static QueryableMethods()
        {
            // 获取所有 Queryable 类的公共静态方法并按名称分组
            var queryableMethodGroups = typeof(Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .GroupBy(mi => mi.Name)
                .ToDictionary(e => e.Key, l => l.ToList());

            All = GetMethod(
                nameof(Queryable.All), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            AnyWithoutPredicate = GetMethod(
                nameof(Queryable.Any), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            AnyWithPredicate = GetMethod(
                nameof(Queryable.Any), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            AsQueryable = GetMethod(
                nameof(Queryable.AsQueryable), 1,
                types => new[] { typeof(IEnumerable<>).MakeGenericType(types[0]) });

            Cast = GetMethod(nameof(Queryable.Cast), 1, types => new[] { typeof(IQueryable) });

            Concat = GetMethod(
                nameof(Queryable.Concat), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(IEnumerable<>).MakeGenericType(types[0]) });

            Contains = GetMethod(
                nameof(Queryable.Contains), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), types[0] });

            CountWithoutPredicate = GetMethod(
                nameof(Queryable.Count), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            CountWithPredicate = GetMethod(
                nameof(Queryable.Count), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            DefaultIfEmptyWithoutArgument = GetMethod(
                nameof(Queryable.DefaultIfEmpty), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            DefaultIfEmptyWithArgument = GetMethod(
                nameof(Queryable.DefaultIfEmpty), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), types[0] });

            Distinct = GetMethod(nameof(Queryable.Distinct), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            ElementAt = GetMethod(
                nameof(Queryable.ElementAt), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(int) });

            ElementAtOrDefault = GetMethod(
                nameof(Queryable.ElementAtOrDefault), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(int) });

            Except = GetMethod(
                nameof(Queryable.Except), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(IEnumerable<>).MakeGenericType(types[0]) });

            FirstWithoutPredicate = GetMethod(
                nameof(Queryable.First), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            FirstWithPredicate = GetMethod(
                nameof(Queryable.First), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            FirstOrDefaultWithoutPredicate = GetMethod(
                nameof(Queryable.FirstOrDefault), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            FirstOrDefaultWithPredicate = GetMethod(
                nameof(Queryable.FirstOrDefault), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            GroupByWithKeySelector = GetMethod(
                nameof(Queryable.GroupBy), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            GroupByWithKeyElementSelector = GetMethod(
                nameof(Queryable.GroupBy), 3,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1])),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[2]))
                });

            GroupByWithKeyElementResultSelector = GetMethod(
                nameof(Queryable.GroupBy), 4,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1])),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[2])),
                typeof(Expression<>).MakeGenericType(
                    typeof(Func<,,>).MakeGenericType(
                        types[1], typeof(IEnumerable<>).MakeGenericType(types[2]), types[3]))
                });

            GroupByWithKeyResultSelector = GetMethod(
                nameof(Queryable.GroupBy), 3,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1])),
                typeof(Expression<>).MakeGenericType(
                    typeof(Func<,,>).MakeGenericType(
                        types[1], typeof(IEnumerable<>).MakeGenericType(types[0]), types[2]))
                });

            GroupJoin = GetMethod(
                nameof(Queryable.GroupJoin), 4,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(IEnumerable<>).MakeGenericType(types[1]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[2])),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[1], types[2])),
                typeof(Expression<>).MakeGenericType(
                    typeof(Func<,,>).MakeGenericType(
                        types[0], typeof(IEnumerable<>).MakeGenericType(types[1]), types[3]))
                });

            Intersect = GetMethod(
                nameof(Queryable.Intersect), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(IEnumerable<>).MakeGenericType(types[0]) });

            Join = GetMethod(
                nameof(Queryable.Join), 4,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(IEnumerable<>).MakeGenericType(types[1]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[2])),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[1], types[2])),
                typeof(Expression<>).MakeGenericType(typeof(Func<,,>).MakeGenericType(types[0], types[1], types[3]))
                });

            LastWithoutPredicate = GetMethod(nameof(Queryable.Last), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            LastWithPredicate = GetMethod(
                nameof(Queryable.Last), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            LastOrDefaultWithoutPredicate = GetMethod(
                nameof(Queryable.LastOrDefault), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            LastOrDefaultWithPredicate = GetMethod(
                nameof(Queryable.LastOrDefault), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            LongCountWithoutPredicate = GetMethod(
                nameof(Queryable.LongCount), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            LongCountWithPredicate = GetMethod(
                nameof(Queryable.LongCount), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            MaxWithoutSelector = GetMethod(nameof(Queryable.Max), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            MaxWithSelector = GetMethod(
                nameof(Queryable.Max), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            MinWithoutSelector = GetMethod(nameof(Queryable.Min), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            MinWithSelector = GetMethod(
                nameof(Queryable.Min), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            OfType = GetMethod(nameof(Queryable.OfType), 1, types => new[] { typeof(IQueryable) });

            OrderBy = GetMethod(
                nameof(Queryable.OrderBy), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            OrderByDescending = GetMethod(
                nameof(Queryable.OrderByDescending), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            Reverse = GetMethod(nameof(Queryable.Reverse), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            Select = GetMethod(
                nameof(Queryable.Select), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            SelectManyWithoutCollectionSelector = GetMethod(
                nameof(Queryable.SelectMany), 2,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(
                    typeof(Func<,>).MakeGenericType(
                        types[0], typeof(IEnumerable<>).MakeGenericType(types[1])))
                });

            SelectManyWithCollectionSelector = GetMethod(
                nameof(Queryable.SelectMany), 3,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(
                    typeof(Func<,>).MakeGenericType(
                        types[0], typeof(IEnumerable<>).MakeGenericType(types[1]))),
                typeof(Expression<>).MakeGenericType(typeof(Func<,,>).MakeGenericType(types[0], types[1], types[2]))
                });

            SingleWithoutPredicate = GetMethod(
                nameof(Queryable.Single), 1, types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            SingleWithPredicate = GetMethod(
                nameof(Queryable.Single), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            SingleOrDefaultWithoutPredicate = GetMethod(
                nameof(Queryable.SingleOrDefault), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]) });

            SingleOrDefaultWithPredicate = GetMethod(
                nameof(Queryable.SingleOrDefault), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            Skip = GetMethod(
                nameof(Queryable.Skip), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(int) });

            SkipWhile = GetMethod(
                nameof(Queryable.SkipWhile), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            Take = GetMethod(
                nameof(Queryable.Take), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(int) });

            TakeWhile = GetMethod(
                nameof(Queryable.TakeWhile), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            ThenBy = GetMethod(
                nameof(Queryable.ThenBy), 2,
                types => new[]
                {
                typeof(IOrderedQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            ThenByDescending = GetMethod(
                nameof(Queryable.ThenByDescending), 2,
                types => new[]
                {
                typeof(IOrderedQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], types[1]))
                });

            Union = GetMethod(
                nameof(Queryable.Union), 1,
                types => new[] { typeof(IQueryable<>).MakeGenericType(types[0]), typeof(IEnumerable<>).MakeGenericType(types[0]) });

            Where = GetMethod(
                nameof(Queryable.Where), 1,
                types => new[]
                {
                typeof(IQueryable<>).MakeGenericType(types[0]),
                typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], typeof(bool)))
                });

            var numericTypes = new[]
            {
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?)
        };

            AverageWithoutSelectorMethods = new Dictionary<Type, MethodInfo>();
            AverageWithSelectorMethods = new Dictionary<Type, MethodInfo>();
            SumWithoutSelectorMethods = new Dictionary<Type, MethodInfo>();
            SumWithSelectorMethods = new Dictionary<Type, MethodInfo>();

            foreach (var type in numericTypes)
            {
                AverageWithoutSelectorMethods[type] = GetMethod(
                    nameof(Queryable.Average), 0, types => new[] { typeof(IQueryable<>).MakeGenericType(type) });
                AverageWithSelectorMethods[type] = GetMethod(
                    nameof(Queryable.Average), 1,
                    types => new[]
                    {
                    typeof(IQueryable<>).MakeGenericType(types[0]),
                    typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], type))
                    });
                SumWithoutSelectorMethods[type] = GetMethod(
                    nameof(Queryable.Sum), 0, types => new[] { typeof(IQueryable<>).MakeGenericType(type) });
                SumWithSelectorMethods[type] = GetMethod(
                    nameof(Queryable.Sum), 1,
                    types => new[]
                    {
                    typeof(IQueryable<>).MakeGenericType(types[0]),
                    typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(types[0], type))
                    });
            }

            // <summary>
            //     获取指定名称和参数类型的方法
            // </summary>
            // <param name="name">方法名称</param>
            // <param name="genericParameterCount">泛型参数数量</param>
            // <param name="parameterGenerator">参数类型生成函数</param>
            // <returns>找到的方法信息</returns>
            MethodInfo GetMethod(string name, int genericParameterCount, Func<Type[], Type[]> parameterGenerator)
                => queryableMethodGroups[name].Single(
                    mi => ((genericParameterCount == 0 && !mi.IsGenericMethod)
                            || (mi.IsGenericMethod && mi.GetGenericArguments().Length == genericParameterCount))
                        && mi.GetParameters().Select(e => e.ParameterType).SequenceEqual(
                            parameterGenerator(mi.IsGenericMethod ? mi.GetGenericArguments() : Array.Empty<Type>())));
        }
    }
}
