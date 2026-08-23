#if !NET8_0_OR_GREATER
using System;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 键控服务提供程序工厂测试，覆盖作用域校验和普通服务预处理场景。
    /// </summary>
    public class KeyedServiceProviderFactory_Tests
    {
        [Fact]
        public void Should_Throw_When_Resolving_Scoped_From_Root_With_Validation()
        {
            var services = new ServiceCollection();
            services.AddKeyedScoped(typeof(ICache), "sql", typeof(SqlCache));

            var provider = services.BuildKeyedServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true
            });
            using (provider as IDisposable)
            {
                Should.Throw<InvalidOperationException>(() =>
                    provider.GetRequiredKeyedService<ICache>("sql"));
            }
        }

        [Fact]
        public void Should_Cache_Scoped_From_Root_When_Validation_Disabled()
        {
            var services = new ServiceCollection();
            services.AddKeyedScoped(typeof(ICache), "sql", typeof(SqlCache));

            using (var provider = services.BuildKeyedServiceProvider() as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredKeyedService<ICache>("sql")
                    .ShouldBeSameAs(serviceProvider.GetRequiredKeyedService<ICache>("sql"));
            }
        }

        [Fact]
        public void Should_Map_Scoped_Keyed_Resolution_Inside_Prepared_Ordinary_Service()
        {
            var services = new ServiceCollection();
            services.AddKeyedScoped(typeof(ICache), "premium", typeof(PremiumCache));
            services.AddScoped<IConsumer, KeyedConsumer>();

            var provider = services.BuildKeyedServiceProvider();
            try
            {
                ICache firstScopeCache;
                using (var scope = provider.CreateScope())
                {
                    var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();
                    firstScopeCache = scope.ServiceProvider.GetRequiredKeyedService<ICache>("premium");
                    consumer.Cache.ShouldBeSameAs(firstScopeCache);
                }

                using (var scope = provider.CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<IConsumer>()
                        .Cache.ShouldNotBeSameAs(firstScopeCache);
                }
            }
            finally
            {
                ((IDisposable)provider).Dispose();
            }
        }
    }
}
#endif
