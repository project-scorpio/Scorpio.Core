using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

using Moq;

using Scorpio.Modularity;

using Shouldly;

using Xunit;

namespace Scorpio.Hosting.Tests
{
    /// <summary>
    /// 键控服务与 Generic Host 集成的测试。
    /// </summary>
    public class KeyedServiceHost_Tests
    {
        [Fact]
        public void Should_Resolve_Keyed_Service_From_Generic_Host()
        {
            var context = new HostBuilderContext(new Dictionary<object, object>());
            var services = new ServiceCollection();
            var mock = new Mock<IHostBuilder>();
            var factory = default(IServiceProviderFactory<IServiceCollection>);
            mock.Setup(builder => builder.UseServiceProviderFactory(
                    It.IsAny<Func<HostBuilderContext, IServiceProviderFactory<IServiceCollection>>>()))
                .Callback<Func<HostBuilderContext, IServiceProviderFactory<IServiceCollection>>>(
                    factorySelector => factory = factorySelector(context));
            mock.Object.AddScorpio<KeyedServiceHostTestModule>();

            if (factory is null)
            {
                throw new InvalidOperationException("The service provider factory was not configured.");
            }

            factory.CreateBuilder(services);
            services.AddSingleton<IHostLifetime, ConsoleLifetime>();
            services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();

            var serviceProvider = factory.CreateServiceProvider(services);
            try
            {
                serviceProvider
                    .GetRequiredKeyedService<IHostCache>("sql")
                    .ShouldBeOfType<HostSqlCache>();
                serviceProvider.GetRequiredService<IBootstrapper>().ShouldBeOfType<InternalBootstrapper>();
            }
            finally
            {
                (serviceProvider as IDisposable)?.Dispose();
            }
        }
    }

    /// <summary>
    /// 键控服务宿主测试模块。
    /// </summary>
    public class KeyedServiceHostTestModule : ScorpioModule
    {
        /// <summary>
        /// 配置键控服务注册。
        /// </summary>
        /// <param name="context">服务配置上下文</param>
        public override void ConfigureServices(ConfigureServicesContext context)
            => context.Services.AddKeyedSingleton<IHostCache>("sql", new HostSqlCache());
    }

    /// <summary>
    /// 宿主缓存测试接口。
    /// </summary>
    public interface IHostCache
    {
    }

    /// <summary>
    /// SQL 宿主缓存实现。
    /// </summary>
    public class HostSqlCache : IHostCache
    {
    }
}
