using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace Scorpio.DependencyInjection
{
    public class DefaultServiceScopeFactory_Tests
    {
        [Fact]
        public void CreateScope_ShouldReturnNewScope()
        {
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            var root = services.BuildServiceProvider();
            var innerFactory = root.GetRequiredService<IServiceScopeFactory>();
            var factory = new DefaultServiceScopeFactory(innerFactory);
            using var scope = factory.CreateScope();
            scope.ShouldNotBeNull();
            scope.ServiceProvider.ShouldNotBeNull();
            scope.ServiceProvider.GetRequiredService<ITestService>().ShouldNotBeNull();
        }

        [Fact]
        public void CreateScope_ShouldReturnDistinctScopes()
        {
            var services = new ServiceCollection();
            var root = services.BuildServiceProvider();
            var innerFactory = root.GetRequiredService<IServiceScopeFactory>();
            var factory = new DefaultServiceScopeFactory(innerFactory);
            using var scope1 = factory.CreateScope();
            using var scope2 = factory.CreateScope();
            scope1.ShouldNotBeSameAs(scope2);
        }

        public interface ITestService { }
        private class TestService : ITestService { }
    }
}
