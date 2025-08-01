using System.Threading.Tasks;

namespace Scorpio.Middleware.Pipeline
{
    /// <summary>
    /// 表示管道请求处理委托，定义了中间件的标准执行签名
    /// </summary>
    /// <typeparam name="TPipelineContext">管道上下文类型，表示在管道中传递的数据上下文</typeparam>
    /// <param name="context">
    /// 管道上下文实例，包含了当前请求的所有相关信息和状态
    /// </param>
    /// <returns>
    /// 返回一个 <see cref="Task"/>，表示异步操作的完成状态
    /// </returns>
    /// <remarks>
    /// 此委托定义了中间件管道中每个组件的标准接口。
    /// 每个中间件都应该实现这个委托签名，接收管道上下文并执行相应的处理逻辑。
    /// 中间件可以：
    /// <list type="bullet">
    /// <item><description>检查和修改传入的上下文</description></item>
    /// <item><description>执行业务逻辑处理</description></item>
    /// <item><description>调用下一个中间件继续管道执行</description></item>
    /// <item><description>短路管道执行，直接返回结果</description></item>
    /// </list>
    /// 使用协变泛型 <c>in</c> 修饰符，允许委托类型的协变转换。
    /// </remarks>
    /// <seealso cref="IPipelineBuilder{TPipelineContext}"/>
    /// <seealso cref="PipelineBuilder{TPipelineContext}"/>
    /// <example>
    /// 以下示例展示了如何创建一个简单的日志记录中间件：
    /// <code>
    /// PipelineRequestDelegate&lt;MyContext&gt; loggingMiddleware = async (context) =>
    /// {
    ///     Console.WriteLine($"处理请求: {context.RequestId}");
    ///     // 在这里可以调用下一个中间件
    ///     await next(context);
    ///     Console.WriteLine($"请求完成: {context.RequestId}");
    /// };
    /// </code>
    /// </example>
    public delegate Task PipelineRequestDelegate<in TPipelineContext>(TPipelineContext context);
}
