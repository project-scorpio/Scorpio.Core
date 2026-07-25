using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;
using Scorpio.DependencyInjection.Conventional;

using Shouldly;

using Xunit;

namespace Scorpio.DependencyInjection
{
    public class BasicConventionalRegistrar_Tests
    {
        private class MySingletonService : ISingletonDependency { }
        private class MyTransientService : ITransientDependency { }
        private class MyScopedService : IScopedDependency { }

        [ExposeServices(typeof(IExposedContract))]
        private class MyExposedService { }
        private interface IExposedContract { }

        [Fact]
        public void Register_ShouldRegisterSingletonDependency()
        {
            var services = new ServiceCollection();
            services.AddConventionalRegistrar(new BasicConventionalRegistrar());
            services.RegisterAssemblyByConvention(typeof(BasicConventionalRegistrar_Tests).Assembly);

            var descriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(MySingletonService));
            descriptor.ShouldNotBeNull();
            descriptor.Lifetime.ShouldBe(ServiceLifetime.Singleton);
        }

        [Fact]
        public void Register_ShouldRegisterTransientDependency()
        {
            var services = new ServiceCollection();
            services.AddConventionalRegistrar(new BasicConventionalRegistrar());
            services.RegisterAssemblyByConvention(typeof(BasicConventionalRegistrar_Tests).Assembly);

            var descriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(MyTransientService));
            descriptor.ShouldNotBeNull();
            descriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
        }

        [Fact]
        public void Register_ShouldRegisterScopedDependency()
        {
            var services = new ServiceCollection();
            services.AddConventionalRegistrar(new BasicConventionalRegistrar());
            services.RegisterAssemblyByConvention(typeof(BasicConventionalRegistrar_Tests).Assembly);

            var descriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(MyScopedService));
            descriptor.ShouldNotBeNull();
            descriptor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        }

        [Fact]
        public void Register_ShouldRegisterExposeServicesAttribute()
        {
            var services = new ServiceCollection();
            services.AddConventionalRegistrar(new BasicConventionalRegistrar());
            services.RegisterAssemblyByConvention(typeof(BasicConventionalRegistrar_Tests).Assembly);

            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IExposedContract));
            descriptor.ShouldNotBeNull();
        }
    }
}
