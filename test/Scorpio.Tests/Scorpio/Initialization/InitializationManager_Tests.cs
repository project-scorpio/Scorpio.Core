using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Scorpio.Modularity;
using Scorpio.TestBase;

using Shouldly;

using Xunit;

namespace Scorpio.Initialization
{
    public class InitializationOptions_Tests
    {
        [Fact]
        public void AddInitializable_Generic_ShouldAddType()
        {
            var options = new InitializationOptions();
            options.AddInitializable<TestInitializable>(10);
            options.Initializables.ShouldContainKey(10);
            options.Initializables[10].ShouldContain(typeof(TestInitializable));
        }

        [Fact]
        public void AddInitializable_ByType_ShouldAddType()
        {
            var options = new InitializationOptions();
            options.AddInitializable(typeof(TestInitializable), 5);
            options.Initializables.ShouldContainKey(5);
            options.Initializables[5].ShouldContain(typeof(TestInitializable));
        }

        [Fact]
        public void AddInitializable_InvalidType_ShouldThrow()
        {
            var options = new InitializationOptions();
            Should.Throw<ArgumentException>(() => options.AddInitializable(typeof(string), 0));
        }

        [Fact]
        public void Initializables_ShouldBeOrderedDescending()
        {
            var options = new InitializationOptions();
            options.AddInitializable<TestInitializable>(1);
            options.AddInitializable<TestInitializable2>(10);

            var keys = options.Initializables.Keys.ToList();
            keys[0].ShouldBe(10);
            keys[1].ShouldBe(1);
        }

        private class TestInitializable : IInitializable
        {
            public void Initialize() { }
        }

        private class TestInitializable2 : IInitializable
        {
            public void Initialize() { }
        }
    }

    public class InitializationManager_Tests : IntegratedTest<InitManagerTestModule>
    {
        [Fact]
        public void Initialize_ShouldCallInitializablesInOrder()
        {
            var tracker = ServiceProvider.GetRequiredService<InitializationTracker>();
            // Initialize() is already called once during bootstrapper startup
            tracker.Order.Count.ShouldBe(2);
            tracker.Order[0].ShouldBe("High");
            tracker.Order[1].ShouldBe("Low");
        }
    }

    public class InitializationTracker
    {
        public System.Collections.Generic.List<string> Order { get; } = new System.Collections.Generic.List<string>();
    }

    [InitializationOrder(100)]
    public class HighPriorityInit : IInitializable
    {
        private readonly InitializationTracker _tracker;
        public HighPriorityInit(InitializationTracker tracker) => _tracker = tracker;
        public void Initialize() => _tracker.Order.Add("High");
    }

    [InitializationOrder(1)]
    public class LowPriorityInit : IInitializable
    {
        private readonly InitializationTracker _tracker;
        public LowPriorityInit(InitializationTracker tracker) => _tracker = tracker;
        public void Initialize() => _tracker.Order.Add("Low");
    }

    public class InitManagerTestModule : ScorpioModule
    {
        public override void ConfigureServices(ConfigureServicesContext context)
        {
            context.Services.AddSingleton<InitializationTracker>();
            context.Services.AddSingleton<HighPriorityInit>();
            context.Services.AddSingleton<LowPriorityInit>();
        }
    }
}
