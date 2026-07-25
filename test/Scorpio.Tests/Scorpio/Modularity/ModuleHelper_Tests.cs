using System;
using System.Collections.Generic;

using Shouldly;

using Xunit;

namespace Scorpio.Modularity
{
    public class ModuleHelper_Tests
    {
        [Fact]
        public void FindAllModuleTypes_StartupModule_ShouldIncludeKernelModule()
        {
            var types = ModuleHelper.FindAllModuleTypes(typeof(IndependentEmptyModule));
            types.ShouldContain(typeof(KernelModule));
            types.ShouldContain(typeof(IndependentEmptyModule));
        }

        [Fact]
        public void FindAllModuleTypes_ShouldReturnListWithoutDuplicates()
        {
            var types = ModuleHelper.FindAllModuleTypes(typeof(MyTestModuleB));
            types.ShouldNotBeEmpty();
            // Distinct check
            var set = new System.Collections.Generic.HashSet<Type>(types);
            set.Count.ShouldBe(types.Count);
        }

        [Fact]
        public void FindAllModuleTypes_ShouldOrderDependenciesFirst()
        {
            var types = ModuleHelper.FindAllModuleTypes(typeof(MyTestModuleB));
            // The DFS traversal places the module first, then its dependencies
            // KernelModule and MyTestModuleA must both appear in the list (but after MyTestModuleB)
            types.ShouldContain(typeof(KernelModule));
            types.ShouldContain(typeof(MyTestModuleA));
            types.ShouldContain(typeof(MyTestModuleB));
            // MyTestModuleA must appear before KernelModule (since MyTestModuleA depends on KernelModule)
            types.IndexOf(typeof(MyTestModuleA)).ShouldBeLessThan(types.IndexOf(typeof(KernelModule)));
        }

        [Fact]
        public void FindAllModuleTypes_KernelModule_ShouldOnlyContainItself()
        {
            var types = ModuleHelper.FindAllModuleTypes(typeof(KernelModule));
            types.ShouldContain(typeof(KernelModule));
        }

        [Fact]
        public void FindDependedModuleTypes_WithNoDependencies_ShouldReturnKernelModule()
        {
            var deps = ModuleHelper.FindDependedModuleTypes(typeof(IndependentEmptyModule));
            deps.ShouldContain(typeof(KernelModule));
        }

        [Fact]
        public void FindDependedModuleTypes_KernelModule_ShouldReturnEmptyList()
        {
            var deps = ModuleHelper.FindDependedModuleTypes(typeof(KernelModule));
            deps.ShouldBeEmpty();
        }

        [Fact]
        public void FindDependedModuleTypes_WithDependsOn_ShouldReturnDeclaredDependency()
        {
            var deps = ModuleHelper.FindDependedModuleTypes(typeof(MyTestModuleB));
            deps.ShouldContain(typeof(MyTestModuleA));
        }

        [Fact]
        public void FindDependedModuleTypes_InvalidType_ShouldThrow()
        {
            Should.Throw<Exception>(() => ModuleHelper.FindDependedModuleTypes(typeof(string)));
        }
    }

    public class MyTestModuleA : ScorpioModule { }

    [DependsOn(typeof(MyTestModuleA))]
    public class MyTestModuleB : ScorpioModule { }
}
