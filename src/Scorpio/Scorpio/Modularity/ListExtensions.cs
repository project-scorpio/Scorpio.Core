using System;
using System.Collections.Generic;

namespace Scorpio.Modularity
{
    /// <summary>
    /// 为 <see cref="IList{T}"/> 提供扩展方法的静态类
    /// </summary>
    /// <remarks>
    /// 该类包含用于增强列表功能的扩展方法，主要提供基于依赖关系的拓扑排序功能，
    /// 用于解决模块加载顺序等需要考虑依赖关系的排序问题。
    /// </remarks>
    internal static class ListExtensions
    {
        /// <summary>
        /// 使用拓扑排序算法对列表进行排序，考虑元素间的依赖关系
        /// </summary>
        /// <typeparam name="T">列表元素的类型</typeparam>
        /// <param name="source">要排序的对象集合</param>
        /// <param name="getDependencies">用于解析依赖关系的函数，返回指定项的所有依赖项</param>
        /// <returns>
        /// 按照依赖关系排序后的列表。依赖项会排在被依赖项之前，确保加载顺序的正确性。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 该方法实现了拓扑排序算法，用于解决有向无环图（DAG）的排序问题。
        /// 在模块化系统中，这个方法用于确定模块的加载顺序，确保所有依赖项在被依赖项之前加载。
        /// </para>
        /// <para>
        /// 算法参考：
        /// <list type="bullet">
        /// <item><description>http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp</description></item>
        /// <item><description>http://en.wikipedia.org/wiki/Topological_sorting</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">当检测到循环依赖时抛出</exception>
        internal static List<T> SortByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            /* 参考资料: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
             *          http://en.wikipedia.org/wiki/Topological_sorting
             */

            // 存储排序结果的列表
            var sorted = new List<T>();
            // 记录访问状态的字典：true表示正在处理中，false表示已完成处理
            var visited = new Dictionary<T, bool>();

            // 遍历源集合中的每个项目
            foreach (var item in source)
            {
                // 对每个项目进行深度优先访问
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        /// <summary>
        /// 拓扑排序的递归访问方法，使用深度优先搜索算法处理依赖关系
        /// </summary>
        /// <typeparam name="T">列表元素的类型</typeparam>
        /// <param name="item">当前要处理的项目</param>
        /// <param name="getDependencies">用于解析依赖关系的函数</param>
        /// <param name="sorted">存储排序结果的列表</param>
        /// <param name="visited">记录访问状态的字典，用于检测循环依赖</param>
        /// <remarks>
        /// <para>
        /// 该方法实现了深度优先搜索（DFS）算法的核心逻辑：
        /// <list type="number">
        /// <item><description>检查当前项目是否已被访问</description></item>
        /// <item><description>如果正在处理中，说明存在循环依赖，抛出异常</description></item>
        /// <item><description>标记当前项目为正在处理状态</description></item>
        /// <item><description>递归处理所有依赖项</description></item>
        /// <item><description>标记当前项目为已完成状态并添加到结果列表</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">当检测到循环依赖时抛出，包含导致循环的项目信息</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S4143:Collection elements should not be replaced unconditionally", Justification = "字典值的更新是算法逻辑的必要部分，用于跟踪访问状态")]
        private static void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            // 检查当前项目是否已被访问过
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                // 如果项目正在处理中，说明存在循环依赖
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
                // 如果项目已完成处理，直接返回
            }
            else
            {
                // 标记当前项目为正在处理状态
                visited[item] = true;

                // 获取当前项目的所有依赖项
                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    // 递归处理每个依赖项
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                // 标记当前项目为已完成状态
                visited[item] = false;
                // 将当前项目添加到排序结果中
                sorted.Add(item);
            }
        }
    }
}
