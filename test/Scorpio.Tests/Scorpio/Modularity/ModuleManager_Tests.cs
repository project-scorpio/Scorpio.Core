using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Scorpio.Modularity;
using Scorpio.TestBase;

using Shouldly;

using Xunit;

namespace Scorpio.Modularity
{
    public class ModuleManager_Tests : IntegratedTest<ModuleManagerTestModule>
    {
        [Fact]
        public void InitializeModules_ShouldCallAllLifecycleMethods()
        {
            var tracker = ServiceProvider.GetRequiredService<ModuleLifecycleTracker>();
            tracker.PreInitializeCalled.ShouldBeTrue();
            tracker.InitializeCalled.ShouldBeTrue();
            tracker.PostInitializeCalled.ShouldBeTrue();
        }

        [Fact]
        public void ShutdownModules_ShouldCallShutdownOnModules()
        {
            var manager = ServiceProvider.GetRequiredService<IModuleManager>();
            var ctx = new ApplicationShutdownContext(ServiceProvider);
            Should.NotThrow(() => manager.ShutdownModules(ctx));
        }
    }

    public class ModuleLifecycleTracker
    {
        public bool PreInitializeCalled { get; set; }
        public bool InitializeCalled { get; set; }
        public bool PostInitializeCalled { get; set; }
        public bool ShutdownCalled { get; set; }
    }

    public class ModuleManagerTestModule : ScorpioModule
    {
        public override void ConfigureServices(ConfigureServicesContext context)
        {
            context.Services.AddSingleton<ModuleLifecycleTracker>();
        }

        public override void PreInitialize(ApplicationInitializationContext context)
        {
            context.ServiceProvider.GetRequiredService<ModuleLifecycleTracker>().PreInitializeCalled = true;
        }

        public override void Initialize(ApplicationInitializationContext context)
        {
            context.ServiceProvider.GetRequiredService<ModuleLifecycleTracker>().InitializeCalled = true;
        }

        public override void PostInitialize(ApplicationInitializationContext context)
        {
            context.ServiceProvider.GetRequiredService<ModuleLifecycleTracker>().PostInitializeCalled = true;
        }

        public override void Shutdown(ApplicationShutdownContext context)
        {
            context.ServiceProvider.GetRequiredService<ModuleLifecycleTracker>().ShutdownCalled = true;
        }
    }
}
