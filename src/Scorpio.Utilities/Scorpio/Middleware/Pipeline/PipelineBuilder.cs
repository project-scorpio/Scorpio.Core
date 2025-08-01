using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.Middleware.Pipeline
{
    /// <summary>
    /// 管道构建器的扩展方法类，提供便捷的中间件添加和配置功能
    /// </summary>
    /// <remarks>
    /// 此静态类为 <see cref="IPipelineBuilder{TPipelineContext}"/> 提供了多种扩展方法，
    /// 简化了中间件的注册和配置过程，支持多种中间件定义模式。
    /// </remarks>
    /// <seealso cref="IPipelineBuilder{TPipelineContext}"/>
    /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
    public static class PipelineBuilder
    {
        /// <summary>
        /// 缓存的GetService方法信息，用于表达式树编译
        /// </summary>
        /// <remarks>
        /// 预先获取GetService方法的反射信息，避免在运行时重复反射，提高性能。
        /// </remarks>
        private static readonly MethodInfo _getServiceInfo = ((Func<IServiceProvider, Type, object>)PipelineBuilder.GetService).Method!;

        /// <summary>
        /// 添加一个简化的中间件，使用简单的委托签名
        /// </summary>
        /// <typeparam name="TPipelineContext">管道上下文类型</typeparam>
        /// <param name="app">管道构建器实例</param>
        /// <param name="middleware">
        /// 中间件委托，接收上下文和下一个中间件的简化委托
        /// </param>
        /// <returns>
        /// 返回当前的 <see cref="IPipelineBuilder{TPipelineContext}"/> 实例，支持链式调用
        /// </returns>
        /// <remarks>
        /// 此重载简化了中间件的定义，将下一个中间件包装为简单的 <see cref="Func{Task}"/> 委托，
        /// 使中间件的编写更加直观和易于理解。
        /// </remarks>
        /// <seealso cref="IPipelineBuilder{TPipelineContext}.Use"/>
        public static IPipelineBuilder<TPipelineContext> Use<TPipelineContext>(this IPipelineBuilder<TPipelineContext> app, Func<TPipelineContext, Func<Task>, Task> middleware)
        {
            return app.Use(next =>
            {
                return context =>
                {
                    // 将下一个中间件包装为简单的Task委托
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }

        /// <summary>
        /// 添加一个中间件，使用完整的管道委托签名
        /// </summary>
        /// <typeparam name="TPipelineContext">管道上下文类型</typeparam>
        /// <param name="app">管道构建器实例</param>
        /// <param name="middleware">
        /// 中间件委托，接收上下文和下一个中间件的完整委托
        /// </param>
        /// <returns>
        /// 返回当前的 <see cref="IPipelineBuilder{TPipelineContext}"/> 实例，支持链式调用
        /// </returns>
        /// <remarks>
        /// 此重载提供了对管道委托的直接访问，适用于需要完全控制下一个中间件调用的场景。
        /// </remarks>
        /// <seealso cref="PipelineRequestDelegate{TPipelineContext}"/>
        public static IPipelineBuilder<TPipelineContext> Use<TPipelineContext>(this IPipelineBuilder<TPipelineContext> app, Func<TPipelineContext, PipelineRequestDelegate<TPipelineContext>, Task> middleware)
        {
            return app.Use(next => context => middleware(context, next));
        }

        /// <summary>
        /// 添加一个基于类型的中间件，支持依赖注入
        /// </summary>
        /// <typeparam name="TPipelineContext">管道上下文类型</typeparam>
        /// <typeparam name="TMiddleware">中间件类型</typeparam>
        /// <param name="builder">管道构建器实例</param>
        /// <param name="args">传递给中间件构造函数的额外参数</param>
        /// <returns>
        /// 返回当前的 <see cref="IPipelineBuilder{TPipelineContext}"/> 实例，支持链式调用
        /// </returns>
        /// <remarks>
        /// 此方法是 <see cref="UseMiddleware{TPipelineContext}(IPipelineBuilder{TPipelineContext}, Type, object[])"/> 的泛型重载，
        /// 提供了编译时类型安全性。
        /// </remarks>
        /// <seealso cref="UseMiddleware{TPipelineContext}(IPipelineBuilder{TPipelineContext}, Type, object[])"/>
        public static IPipelineBuilder<TPipelineContext> UseMiddleware<TPipelineContext, TMiddleware>(this IPipelineBuilder<TPipelineContext> builder, params object[] args) => UseMiddleware(builder, typeof(TMiddleware), args);

        /// <summary>
        /// 添加一个基于类型的中间件，支持依赖注入和参数传递
        /// </summary>
        /// <typeparam name="TPipelineContext">管道上下文类型</typeparam>
        /// <param name="builder">管道构建器实例</param>
        /// <param name="middlewareType">中间件类型</param>
        /// <param name="args">传递给中间件构造函数的额外参数</param>
        /// <returns>
        /// 返回当前的 <see cref="IPipelineBuilder{TPipelineContext}"/> 实例，支持链式调用
        /// </returns>
        /// <remarks>
        /// 此方法通过反射和表达式树编译技术，自动处理中间件的实例化和方法调用。
        /// 中间件类必须包含名为"Invoke"或"InvokeAsync"的公共方法，该方法必须：
        /// 1. 返回 <see cref="Task"/> 类型
        /// 2. 第一个参数必须是 <typeparamref name="TPipelineContext"/> 类型
        /// 3. 其他参数将通过依赖注入自动解析
        /// </remarks>
        /// <exception cref="ArgumentNullException">当 <paramref name="builder"/> 或 <paramref name="middlewareType"/> 为 null 时抛出</exception>
        /// <exception cref="InvalidOperationException">当中间件类型不符合约定时抛出</exception>
        /// <seealso cref="ActivatorUtilities"/>
        public static IPipelineBuilder<TPipelineContext> UseMiddleware<TPipelineContext>(this IPipelineBuilder<TPipelineContext> builder, Type middlewareType, params object[] args)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(middlewareType, nameof(middlewareType));
            
            builder.Use(next =>
            {
                // 查找Invoke或InvokeAsync方法
                var methods = middlewareType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => m.Name.IsIn("Invoke", "InvokeAsync")).ToArray();
                
                // 验证方法数量
                if (methods.Length > 1)
                {
                    throw new InvalidOperationException($"中间件类型 {middlewareType.Name} 包含多个Invoke/InvokeAsync方法");
                }

                if (methods.Length == 0)
                {
                    throw new InvalidOperationException($"中间件类型 {middlewareType.Name} 必须包含Invoke或InvokeAsync方法");
                }
                
                var methodinfo = methods[0];
                
                // 验证返回类型
                if (!typeof(Task).IsAssignableFrom(methodinfo.ReturnType))
                {
                    throw new InvalidOperationException($"中间件方法 {methodinfo.Name} 必须返回Task类型");
                }
                
                var parameters = methodinfo.GetParameters();
                
                // 验证第一个参数
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(TPipelineContext))
                {
                    throw new InvalidOperationException($"中间件方法的第一个参数必须是 {typeof(TPipelineContext).Name} 类型");
                }

                // 准备构造函数参数
                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                
                // 创建中间件实例
                var instance = ActivatorUtilities.CreateInstance(builder.ApplicationServices, middlewareType, ctorArgs);
                
                // 如果只有一个参数，直接创建委托
                if (parameters.Length == 1)
                {
                    return (PipelineRequestDelegate<TPipelineContext>)methodinfo.CreateDelegate(typeof(PipelineRequestDelegate<TPipelineContext>), instance);
                }

                // 编译表达式树以处理依赖注入参数
                var factory = Compile<object, TPipelineContext>(methodinfo, parameters);

                return context =>
                {
                    var serviceProvider = builder.ApplicationServices;
                    if (serviceProvider == null)
                    {
                        throw new InvalidOperationException("服务提供者不能为null");
                    }

                    return factory(instance, context, serviceProvider);
                };
            });
            
            return builder;
        }

        /// <summary>
        /// 编译中间件方法的表达式树，用于支持依赖注入参数
        /// </summary>
        /// <typeparam name="T">中间件实例类型</typeparam>
        /// <typeparam name="TPipelineContext">管道上下文类型</typeparam>
        /// <param name="methodinfo">中间件方法信息</param>
        /// <param name="parameters">方法参数信息数组</param>
        /// <returns>
        /// 编译后的委托，用于调用中间件方法并自动注入依赖
        /// </returns>
        /// <remarks>
        /// 此方法使用表达式树技术动态生成高性能的方法调用委托，
        /// 自动处理依赖注入参数的解析和类型转换。
        /// </remarks>
        /// <exception cref="NotSupportedException">当方法参数包含引用类型时抛出</exception>
        private static Func<T, TPipelineContext, IServiceProvider, Task> Compile<T, TPipelineContext>(MethodInfo methodinfo, ParameterInfo[] parameters)
        {
            var middleware = typeof(T);
            var httpContextArg = Expression.Parameter(typeof(TPipelineContext), "context");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middleware, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = httpContextArg;
            
            // 为每个依赖注入参数生成服务解析表达式
            for (var i = 1; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    throw new NotSupportedException($"不支持引用类型参数: {parameterType.Name}");
                }

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type))
                };

                var getServiceCall = Expression.Call(_getServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression middlewareInstanceArg = instanceArg;
            if (methodinfo.DeclaringType != typeof(T))
            {
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodinfo.DeclaringType);
            }

            var body = Expression.Call(middlewareInstanceArg, methodinfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, TPipelineContext, IServiceProvider, Task>>(body, instanceArg, httpContextArg, providerArg);

            return lambda.Compile();
        }

        /// <summary>
        /// 从服务提供者获取必需的服务实例
        /// </summary>
        /// <param name="sp">服务提供者</param>
        /// <param name="type">要获取的服务类型</param>
        /// <returns>服务实例</returns>
        /// <remarks>
        /// 此方法用于表达式树编译中的服务解析，确保获取到必需的服务实例。
        /// </remarks>
        /// <exception cref="InvalidOperationException">当无法解析指定类型的服务时抛出</exception>
        /// <seealso cref="ServiceProviderServiceExtensions.GetRequiredService(IServiceProvider, Type)"/>
        private static object GetService(IServiceProvider sp, Type type)
        {
            var service = sp.GetRequiredService(type);
            return service;
        }
    }
}
