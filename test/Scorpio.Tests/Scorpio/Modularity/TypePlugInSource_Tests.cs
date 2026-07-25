using System;
using System.Linq;

using Scorpio.Modularity.Plugins;

using Shouldly;

using Xunit;

namespace Scorpio.Modularity
{
    public class TypePlugInSource_Tests
    {
        private class FakeModule : ScorpioModule { }
        private class FakeModule2 : ScorpioModule { }

        [Fact]
        public void GetModules_ShouldReturnProvidedTypes()
        {
            var source = new TypePlugInSource(typeof(FakeModule), typeof(FakeModule2));
            var result = source.GetModules();
            result.ShouldContain(typeof(FakeModule));
            result.ShouldContain(typeof(FakeModule2));
        }

        [Fact]
        public void GetModules_ShouldReturnEmpty_WhenNoTypes()
        {
            var source = new TypePlugInSource();
            source.GetModules().ShouldBeEmpty();
        }

        [Fact]
        public void GetModules_ShouldReturnEmpty_WhenNullPassed()
        {
            var source = new TypePlugInSource(null);
            source.GetModules().ShouldBeEmpty();
        }
    }

    public class PlugInSourceExtensions_Tests
    {
        private class RootModule : ScorpioModule { }

        [Fact]
        public void GetModulesWithAllDependencies_ShouldReturnRootAndDependencies()
        {
            var source = new TypePlugInSource(typeof(RootModule));
            var result = source.GetModulesWithAllDependencies();
            result.ShouldContain(typeof(RootModule));
        }

        [Fact]
        public void GetModulesWithAllDependencies_ShouldThrow_WhenNull()
        {
            IPlugInSource source = null;
            Should.Throw<ArgumentNullException>(() => source.GetModulesWithAllDependencies());
        }

        [Fact]
        public void GetModulesWithAllDependencies_ShouldReturnDistinctTypes()
        {
            var source = new TypePlugInSource(typeof(RootModule), typeof(RootModule));
            var result = source.GetModulesWithAllDependencies();
            result.ShouldContain(typeof(RootModule));
            result.GroupBy(t => t).ShouldAllBe(g => g.Count() == 1);
        }
    }
}
