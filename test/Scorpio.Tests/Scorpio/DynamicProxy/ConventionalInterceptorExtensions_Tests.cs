using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Scorpio.Conventional;

using Shouldly;

using Xunit;

namespace Scorpio.DynamicProxy
{
    public class ConventionalInterceptorExtensions_Tests
    {
        private class TestInterceptor : IInterceptor
        {
            public System.Threading.Tasks.Task InterceptAsync(IMethodInvocation invocation) => System.Threading.Tasks.Task.CompletedTask;
        }

        private class TestInterceptor2 : IInterceptor
        {
            public System.Threading.Tasks.Task InterceptAsync(IMethodInvocation invocation) => System.Threading.Tasks.Task.CompletedTask;
        }

        private static IConventionalContext<ConventionalInterceptorAction> GetContext()
        {
            var services = new ServiceCollection();
            var config = new ConventionalConfiguration<ConventionalInterceptorAction>(services, Array.Empty<Type>());
            return config.GetInternalContext();
        }

        [Fact]
        public void Intercept_ShouldAddInterceptorToTypeList()
        {
            var context = GetContext();
            var result = context.Intercept<TestInterceptor>();
            result.ShouldBe(context);
            var typeList = context.Get<ITypeList<IInterceptor>>(ConventionalInterceptorAction.Interceptors);
            typeList.ShouldNotBeNull();
            typeList.ShouldContain(typeof(TestInterceptor));
        }

        [Fact]
        public void Intercept_CalledTwice_ShouldAddBothInterceptors()
        {
            var context = GetContext();
            context.Intercept<TestInterceptor>().Intercept<TestInterceptor2>();
            var typeList = context.Get<ITypeList<IInterceptor>>(ConventionalInterceptorAction.Interceptors);
            typeList.ShouldNotBeNull();
            typeList.Count.ShouldBe(2);
        }
    }

    public class ConventionalInterceptorAction_Tests
    {
        private class TestInterceptor : IInterceptor
        {
            public System.Threading.Tasks.Task InterceptAsync(IMethodInvocation invocation) => System.Threading.Tasks.Task.CompletedTask;
        }

        [Fact]
        public void Action_ShouldDoNothing_WhenNoProxyConventionalAction()
        {
            var services = new ServiceCollection();
            // No IProxyConventionalAction registered — early return path
            Should.NotThrow(() =>
                services.RegisterConventionalInterceptor(
                    new[] { typeof(ConventionalInterceptorAction_Tests) },
                    config => config.Where(t => true).Intercept<TestInterceptor>()));
        }

        [Fact]
        public void Action_ShouldCallProxyConventionalAction_WhenRegistered()
        {
            IProxyConventionalActionContext capturedCtx = null;
            var mockAction = new Mock<IProxyConventionalAction>();
            mockAction
                .Setup(a => a.Action(It.IsAny<IProxyConventionalActionContext>()))
                .Callback<IProxyConventionalActionContext>(ctx => capturedCtx = ctx);

            var services = new ServiceCollection();
            services.AddSingleton(mockAction.Object);

            services.RegisterConventionalInterceptor(
                new[] { typeof(ConventionalInterceptorAction_Tests) },
                config => config.Where(t => true).Intercept<TestInterceptor>());

            mockAction.Verify(a => a.Action(It.IsAny<IProxyConventionalActionContext>()), Times.Once);
            capturedCtx.ShouldNotBeNull();
            capturedCtx.Interceptors.ShouldContain(typeof(TestInterceptor));
        }
    }
}
