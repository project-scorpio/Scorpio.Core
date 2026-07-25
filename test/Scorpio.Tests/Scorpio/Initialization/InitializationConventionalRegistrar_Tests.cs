using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Scorpio.Conventional;

using Shouldly;

using Xunit;

namespace Scorpio.Initialization
{
    public class InitializationConventionalRegistrar_Tests
    {
        private class ConcreteInitializable : IInitializable
        {
            public void Initialize() { }
        }

        [InitializationOrder(5)]
        private class OrderedInitializable : IInitializable
        {
            public void Initialize() { }
        }

        [Fact]
        public void Register_ShouldAddInitializableTypes()
        {
            var services = new ServiceCollection();
            services.AddConventionalRegistrar(new InitializationConventionalRegistrar());
            services.RegisterAssemblyByConvention(typeof(InitializationConventionalRegistrar_Tests).Assembly);

            var sp = services.BuildServiceProvider();
            var opts = sp.GetRequiredService<IOptions<InitializationOptions>>().Value;
            var allTypes = opts.Initializables.Values.SelectMany(v => v).ToList();
            allTypes.ShouldContain(typeof(ConcreteInitializable));
        }

        [Fact]
        public void Register_ShouldRespectInitializationOrder()
        {
            var services = new ServiceCollection();
            services.AddConventionalRegistrar(new InitializationConventionalRegistrar());
            services.RegisterAssemblyByConvention(typeof(InitializationConventionalRegistrar_Tests).Assembly);

            var sp = services.BuildServiceProvider();
            var opts = sp.GetRequiredService<IOptions<InitializationOptions>>().Value;
            var allTypes = opts.Initializables.Values.SelectMany(v => v).ToList();
            allTypes.ShouldContain(typeof(OrderedInitializable));
            opts.Initializables.ShouldContainKey(5);
            opts.Initializables[5].ShouldContain(typeof(OrderedInitializable));
        }
    }
}
