using System;

namespace Scorpio.Middleware.Pipeline
{
    /// <summary>
    /// 定义管道构建器接口，用于构建中间件执行管道
    /// </summary>
    /// <typeparam name="TPipelineContext">管道上下文类型，表示在管道中传递的数据上下文</typeparam>
    /// <remarks>
    /// 管道构建器遵循构建器模式，允许通过链式调用的方式添加多个中间件组件。
    /// 每个中间件可以在请求处理过程中执行特定的逻辑，如验证、日志记录、异常处理等。
    /// 最终通过Build方法构建出完整的管道执行委托。
    /// </remarks>
    /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
    public interface IPipelineBuilder<TPipelineContext>
    {
        /// <summary>
        /// 获取应用程序服务提供者，用于依赖注入和服务解析
        /// </summary>
        /// <value>
        /// <see cref="IServiceProvider"/> 实例，提供对注册服务的访问
        /// </value>
        /// <remarks>
        /// 中间件组件可以通过此属性获取所需的依赖服务，实现松耦合的设计。
        /// </remarks>
        /// <seealso cref="IServiceProvider"/>
        IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// 向管道中添加一个中间件组件
        /// </summary>
        /// <param name="middleware">
        /// 中间件工厂函数，接收下一个中间件的委托并返回当前中间件的委托
        /// </param>
        /// <returns>
        /// 返回当前的 <see cref="IPipelineBuilder{TPipelineContext}"/> 实例，支持链式调用
        /// </returns>
        /// <remarks>
        /// 中间件按照添加顺序执行，每个中间件可以：
        /// 1. 在调用下一个中间件之前执行预处理逻辑
        /// 2. 调用下一个中间件继续管道执行
        /// 3. 在下一个中间件执行完成后执行后处理逻辑
        /// 4. 短路管道执行，不调用后续中间件
        /// </remarks>
        /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
        IPipelineBuilder<TPipelineContext> Use(Func<PipelineRequestDelegate<TPipelineContext>, PipelineRequestDelegate<TPipelineContext>> middleware);

        /// <summary>
        /// 构建完整的管道执行委托
        /// </summary>
        /// <returns>
        /// 返回 <see cref="PipelineRequestDelegate{TPipelineContext}"/> 委托，
        /// 该委托封装了所有已添加的中间件组件
        /// </returns>
        /// <remarks>
        /// 此方法将所有通过 <see cref="Use"/> 方法添加的中间件组合成一个完整的执行管道。
        /// 构建完成后，可以通过返回的委托来执行整个管道流程。
        /// 通常在应用程序启动时调用此方法来完成管道的初始化。
        /// </remarks>
        /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
        /// <seealso cref="Use"/>
        PipelineRequestDelegate<TPipelineContext> Build();
    }
}
