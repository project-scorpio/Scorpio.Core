using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.DependencyInjection.Conventional;
using Scorpio.DynamicProxy;

using Shouldly;

using Xunit;

namespace Scorpio.Conventional
{
    public class ConventionalRegistrationContextExtensions_Tests
    {
        private static IConventionalRegistrationContext GetContext()
        {
            var services = new ServiceCollection();
            var assembly = typeof(ConventionalRegistrationContextExtensions_Tests).Assembly;
            return new ConventionalRegistrationContext(assembly, services);
        }

        [Fact]
        public void DoConventionalAction_ShouldReturnSameContext()
        {
            var context = GetContext();
            var result = context.DoConventionalAction<ConventionalDependencyAction>(_ => { });
            result.ShouldBe(context);
        }

        [Fact]
        public void RegisterConventionalDependencyInject_ShouldReturnSameContext()
        {
            var context = GetContext();
            var result = context.RegisterConventionalDependencyInject(_ => { });
            result.ShouldBe(context);
        }

        [Fact]
        public void RegisterConventionalInterceptor_ShouldReturnSameContext()
        {
            var context = GetContext();
            var result = context.RegisterConventionalInterceptor(_ => { });
            result.ShouldBe(context);
        }

        [Fact]
        public void RegisterConventionalDependencyInject_ShouldRegisterServices()
        {
            var services = new ServiceCollection();
            var assembly = typeof(ConventionalRegistrationContextExtensions_Tests).Assembly;
            var context = new ConventionalRegistrationContext(assembly, services);

            context.RegisterConventionalDependencyInject(c =>
                c.Where(t => t == typeof(MySelfService))
                 .AsSelf()
                 .Lifetime(ServiceLifetime.Transient));

            services.ShouldContain(sd => sd.ServiceType == typeof(MySelfService));
        }

        private class MySelfService { }
    }

    public class ConventionalConfigurationExtensions_Tests
    {
        [Fact]
        public void Where_ShouldFilterTypes()
        {
            var services = new ServiceCollection();
            var types = new Type[] { typeof(string), typeof(int), typeof(List<>) };
            var config = new ConventionalConfiguration<ConventionalDependencyAction>(services, types);

            var context = config.Where(t => t == typeof(string));
            context.Types.ShouldHaveSingleItem().ShouldBe(typeof(string));
        }

        [Fact]
        public void CreateContext_ShouldReturnNewContext()
        {
            var services = new ServiceCollection();
            var config = new ConventionalConfiguration<ConventionalDependencyAction>(services, new Type[] { });
            var ctx = config.CreateContext();
            ctx.ShouldNotBeNull();
        }
    }
}
