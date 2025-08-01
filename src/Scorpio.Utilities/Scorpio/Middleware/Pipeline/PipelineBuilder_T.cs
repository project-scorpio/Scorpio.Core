using System;
using System.Collections.Generic;
using System.Linq;

namespace Scorpio.Middleware.Pipeline
{
    /// <summary>
    /// 管道构建器的抽象基类，提供中间件管道构建的核心实现
    /// </summary>
    /// <typeparam name="TPipelineContext">管道上下文类型，表示在管道中传递的数据上下文</typeparam>
    /// <remarks>
    /// 此抽象类实现了 <see cref="IPipelineBuilder{TPipelineContext}"/> 接口的通用逻辑。
    /// 子类需要实现 <see cref="TailDelegate"/> 属性来定义管道末尾的处理逻辑。
    /// 中间件按照添加的相反顺序执行，实现了洋葱模型的执行方式。
    /// </remarks>
    /// <seealso cref="IPipelineBuilder{TPipelineContext}"/>
    /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
    public abstract class PipelineBuilder<TPipelineContext> : IPipelineBuilder<TPipelineContext>
    {
        /// <summary>
        /// 存储中间件工厂函数的列表
        /// </summary>
        /// <remarks>
        /// 每个中间件工厂函数接收下一个中间件的委托并返回当前中间件的委托，
        /// 从而形成链式的中间件调用结构。
        /// </remarks>
        private readonly IList<Func<PipelineRequestDelegate<TPipelineContext>, PipelineRequestDelegate<TPipelineContext>>> _middlewares;

        /// <summary>
        /// 初始化管道构建器实例
        /// </summary>
        /// <param name="serviceProvider">应用程序服务提供者，用于依赖注入</param>
        /// <remarks>
        /// 构造函数初始化服务提供者和中间件列表，为后续的中间件添加和管道构建做准备。
        /// </remarks>
        /// <exception cref="ArgumentNullException">当 <paramref name="serviceProvider"/> 为 null 时抛出</exception>
        protected PipelineBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
            _middlewares = new List<Func<PipelineRequestDelegate<TPipelineContext>, PipelineRequestDelegate<TPipelineContext>>>();
        }

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
        public IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// 构建完整的管道执行委托
        /// </summary>
        /// <returns>
        /// 返回 <see cref="PipelineRequestDelegate{TPipelineContext}"/> 委托，
        /// 该委托封装了所有已添加的中间件组件
        /// </returns>
        /// <remarks>
        /// 此方法使用 LINQ 的 Aggregate 方法将所有中间件组合成一个完整的执行管道。
        /// 中间件按照添加的相反顺序执行（LIFO），最后执行的是 <see cref="TailDelegate"/>。
        /// 这种设计实现了洋葱模型：第一个添加的中间件最外层，最后添加的中间件最内层。
        /// </remarks>
        /// <seealso cref="TailDelegate"/>
        /// <seealso cref="Use"/>
        public PipelineRequestDelegate<TPipelineContext> Build() => _middlewares.Reverse().Aggregate(TailDelegate, (d, f) => f(d));

        /// <summary>
        /// 获取管道末尾的执行委托
        /// </summary>
        /// <value>
        /// 管道执行链的最后一个委托，当所有中间件执行完毕后调用
        /// </value>
        /// <remarks>
        /// 子类必须实现此属性来定义管道的终端行为。
        /// 这个委托通常包含核心的业务逻辑处理，或者是一个空操作。
        /// </remarks>
        /// <seealso cref="Build"/>
        protected abstract PipelineRequestDelegate<TPipelineContext> TailDelegate { get; }

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
        /// 中间件按照添加顺序存储，但在执行时按相反顺序执行。
        /// 每个中间件可以在调用下一个中间件前后执行自定义逻辑，实现横切关注点的处理。
        /// </remarks>
        /// <exception cref="ArgumentNullException">当 <paramref name="middleware"/> 为 null 时抛出</exception>
        /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
        /// <seealso cref="Build"/>
        public IPipelineBuilder<TPipelineContext> Use(Func<PipelineRequestDelegate<TPipelineContext>, PipelineRequestDelegate<TPipelineContext>> middleware)
        {
            Check.NotNull(middleware, nameof(middleware));
            _middlewares.Add(middleware);
            return this;
        }
    }
}
