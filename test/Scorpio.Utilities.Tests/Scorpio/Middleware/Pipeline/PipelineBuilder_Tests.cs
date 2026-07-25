using System;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace Scorpio.Middleware.Pipeline
{
    public class PipelineBuilder_Tests
    {
        [Fact]
        public void UseMiddleware()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.Throw<ArgumentNullException>(() => builder.UseMiddleware(null));
            Should.NotThrow(() => builder.UseMiddleware(typeof(FuncResultMiddleware)));
            Should.NotThrow(() => builder.UseMiddleware(typeof(TestMiddleware)));
            var context = new TestPipelineContext();
            context.PipelineInvoked.ShouldBeFalse();
            Should.NotThrow(() => builder.Build()(context));
            context.PipelineInvoked.ShouldBeTrue();
        }

        [Fact]
        public void UseMiddleware_T()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, FuncResultMiddleware>());
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, TestMiddleware>());
            var context = new TestPipelineContext();
            context.PipelineInvoked.ShouldBeFalse();
            Should.NotThrow(() => builder.Build()(context));
            context.PipelineInvoked.ShouldBeTrue();
        }

        [Fact]
        public void UseMiddleware_T_NonServiceProvider()
        {
            var builder = new TestPipelineBuilder(null);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, ManyParametersMiddleware>());
            var context = new TestPipelineContext();
#if NET8_0_OR_GREATER
            Should.Throw<ArgumentNullException>(() => builder.Build()(context));
#else
            Should.Throw<InvalidOperationException>(() => builder.Build()(context));
#endif
        }
        [Fact]
        public void UseMiddleware_T_Exception1()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, DoublyMethodMiddleware>());
            var context = new TestPipelineContext();
            Should.Throw<InvalidOperationException>(() => builder.Build()(context));
        }

        [Fact]
        public void UseMiddleware_T_Exception2()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, NonMethodMiddleware>());
            var context = new TestPipelineContext();
            Should.Throw<InvalidOperationException>(() => builder.Build()(context));
        }

        [Fact]
        public void UseMiddleware_T_Exception3()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, NotTaskMiddleware>());
            var context = new TestPipelineContext();
            Should.Throw<InvalidOperationException>(() => builder.Build()(context));
        }

        [Fact]
        public void UseMiddleware_T_Exception4()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, NonParameterMiddleware>());
            var context = new TestPipelineContext();
            Should.Throw<InvalidOperationException>(() => builder.Build()(context));
        }

        [Fact]
        public void UseMiddleware_T_Exception5()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, ByRefParametersMiddleware>());
            var context = new TestPipelineContext();
            Should.Throw<NotSupportedException>(() => builder.Build()(context));
        }

        [Fact]
        public void UseMiddleware_ManyParameters()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.ApplicationServices.ShouldBe(serviceProvider);
            Should.NotThrow(() => builder.UseMiddleware<TestPipelineContext, ManyParametersMiddleware>());
            var context = new TestPipelineContext();
            context.PipelineInvoked.ShouldBeFalse();
            Should.NotThrow(() => builder.Build()(context));
            context.PipelineInvoked.ShouldBeTrue();
        }

        [Fact]
        public void Use_WithSimpleDelegate_ShouldInvokeMiddleware()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.Use((context, next) =>
            {
                context.PipelineInvoked = true;
                return next();
            });
            var context = new TestPipelineContext();
            context.PipelineInvoked.ShouldBeFalse();
            builder.Build()(context);
            context.PipelineInvoked.ShouldBeTrue();
        }

        [Fact]
        public void Use_WithSimpleDelegate_ShouldChainMiddlewares()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            var log = new System.Collections.Generic.List<int>();
            builder.Use(async (context, next) =>
            {
                log.Add(1);
                await next();
                log.Add(3);
            });
            builder.Use(async (context, next) =>
            {
                log.Add(2);
                await next();
            });
            builder.Build()(new TestPipelineContext());
            log.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void Use_WithRequestDelegate_ShouldInvokeMiddleware()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            builder.Use((context, next) =>
            {
                context.PipelineInvoked = true;
                return next(context);
            });
            var context = new TestPipelineContext();
            context.PipelineInvoked.ShouldBeFalse();
            builder.Build()(context);
            context.PipelineInvoked.ShouldBeTrue();
        }

        [Fact]
        public void Use_WithRequestDelegate_ShouldChainMiddlewares()
        {
            var descriptors = new ServiceCollection();
            var serviceProvider = descriptors.BuildServiceProvider();
            var builder = new TestPipelineBuilder(serviceProvider);
            var log = new System.Collections.Generic.List<int>();
            builder.Use(async (context, next) =>
            {
                log.Add(1);
                await next(context);
                log.Add(3);
            });
            builder.Use(async (context, next) =>
            {
                log.Add(2);
                await next(context);
            });
            builder.Build()(new TestPipelineContext());
            log.ShouldBe(new[] { 1, 2, 3 });
        }
    }
}
