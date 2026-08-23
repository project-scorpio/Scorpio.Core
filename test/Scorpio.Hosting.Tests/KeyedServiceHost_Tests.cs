using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

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
            var builder = new HostBuilder();
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IHostLifetime, ConsoleLifetime>();
                services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
            });
            builder.AddScorpio<KeyedServiceHostTestModule>();

            using var host = builder.Build();
            host.Services.GetRequiredKeyedService<IHostCache>("sql").ShouldBeOfType<HostSqlCache>();
            host.Services.GetRequiredService<IBootstrapper>().ShouldBeOfType<InternalBootstrapper>();
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
