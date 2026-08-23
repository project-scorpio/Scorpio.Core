using Microsoft.Extensions.DependencyInjection;

using Scorpio.Modularity;

using Shouldly;

using Xunit;

namespace Scorpio
{
    /// <summary>
    /// 键控服务与 Bootstrapper 集成的测试。
    /// 验证 Bootstrapper 路径和模块初始化阶段的键控解析。
    /// </summary>
    public class KeyedServiceBootstrapper_Tests
    {
        [Fact]
        public void Should_Resolve_Keyed_Service_From_Bootstrapper()
        {
            using (var bootstrapper = Bootstrapper.Create<KeyedServiceTestModule>())
            {
                bootstrapper.ServiceProvider
                    .GetRequiredKeyedService<ICache>("sql")
                    .ShouldBeOfType<SqlCache>();
            }
        }

        [Fact]
        public void Should_Resolve_Keyed_Service_During_Initialize()
        {
            using (var bootstrapper = Bootstrapper.Create<KeyedServiceTestModule>())
            {
                bootstrapper.Initialize();
                bootstrapper.ServiceProvider
                    .GetRequiredService<KeyedServiceTestModule>()
                    .KeyedServiceResolvedInInitialize
                    .ShouldBeTrue();
            }
        }
    }

    /// <summary>
    /// 键控服务 Bootstrapper 测试模块。
    /// </summary>
    public class KeyedServiceTestModule : ScorpioModule
    {
        /// <summary>
        /// 获取一个值，指示初始化阶段是否成功解析键控服务。
        /// </summary>
        /// <value>如果解析成功，则为 true；否则为 false</value>
        public bool KeyedServiceResolvedInInitialize { get; private set; }

        /// <summary>
        /// 配置键控服务注册。
        /// </summary>
        /// <param name="context">服务配置上下文</param>
        public override void ConfigureServices(ConfigureServicesContext context)
        {
            context.Services.AddKeyedSingleton<ICache>("sql", new SqlCache());
            context.Services.AddKeyedSingleton<ICache>("premium", new PremiumCache());
            context.Services.AddKeyedSingleton<IConsumer, KeyedConsumer>("consumer");
        }

        /// <summary>
        /// 在初始化阶段解析键控服务。
        /// </summary>
        /// <param name="context">应用初始化上下文</param>
        public override void Initialize(ApplicationInitializationContext context)
            => KeyedServiceResolvedInInitialize =
                context.ServiceProvider.GetRequiredKeyedService<IConsumer>("consumer") != null;
    }
}
