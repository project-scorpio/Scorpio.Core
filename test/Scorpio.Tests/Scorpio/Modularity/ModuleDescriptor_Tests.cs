using System;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace Scorpio.Modularity
{
    public class ModuleDescriptor_Tests
    {
        private class TestModule : ScorpioModule { }

        [Fact]
        public void Constructor_ShouldSetProperties()
        {
            var instance = new TestModule();
            var descriptor = new ModuleDescriptor(typeof(TestModule), instance, false);

            descriptor.Type.ShouldBe(typeof(TestModule));
            descriptor.Assembly.ShouldBe(typeof(TestModule).Assembly);
            descriptor.Instance.ShouldBe(instance);
            descriptor.IsLoadedAsPlugIn.ShouldBeFalse();
            descriptor.Dependencies.ShouldBeEmpty();
        }

        [Fact]
        public void Constructor_IsLoadedAsPlugIn_ShouldBeTrue()
        {
            var instance = new TestModule();
            var descriptor = new ModuleDescriptor(typeof(TestModule), instance, true);
            descriptor.IsLoadedAsPlugIn.ShouldBeTrue();
        }

        [Fact]
        public void Constructor_NullType_ShouldThrow()
        {
            var instance = new TestModule();
            Should.Throw<ArgumentNullException>(() => new ModuleDescriptor(null, instance, false));
        }

        [Fact]
        public void Constructor_NullInstance_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => new ModuleDescriptor(typeof(TestModule), null, false));
        }

        [Fact]
        public void Constructor_MismatchedInstance_ShouldThrow()
        {
            var services = new ServiceCollection();
            var wrongInstance = new KernelModule();
            Should.Throw<ArgumentException>(() => new ModuleDescriptor(typeof(TestModule), wrongInstance, false));
        }

        [Fact]
        public void AddDependency_ShouldBeReflectedInDependencies()
        {
            var instanceA = new TestModule();
            var descriptorA = new ModuleDescriptor(typeof(TestModule), instanceA, false);

            var kernelDescriptor = new ModuleDescriptor(typeof(KernelModule), new KernelModule(), false);
            descriptorA.AddDependency(kernelDescriptor);

            descriptorA.Dependencies.ShouldContain(kernelDescriptor);
        }
    }
}
