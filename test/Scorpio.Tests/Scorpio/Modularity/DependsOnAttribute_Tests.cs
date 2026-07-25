using System;
using System.Linq;
using System.Reflection;

using Shouldly;

using Xunit;

namespace Scorpio.Modularity
{
    public class DependsOnAttribute_Tests
    {
        [DependsOn(typeof(IndependentEmptyModule))]
        private class ModuleWithDep : ScorpioModule { }

        [DependsOn]
        private class ModuleWithEmptyDep : ScorpioModule { }

        [Fact]
        public void Constructor_ShouldSetDependedTypes()
        {
            var attr = new DependsOnAttribute(typeof(IndependentEmptyModule), typeof(KernelModule));
            attr.DependedTypes.ShouldContain(typeof(IndependentEmptyModule));
            attr.DependedTypes.ShouldContain(typeof(KernelModule));
        }

        [Fact]
        public void Constructor_NullParam_ShouldResultInEmptyArray()
        {
            var attr = new DependsOnAttribute(null);
            attr.DependedTypes.ShouldBeEmpty();
        }

        [Fact]
        public void Constructor_NoDependencies_ShouldResultInEmptyArray()
        {
            var attr = new DependsOnAttribute();
            attr.DependedTypes.ShouldBeEmpty();
        }

        [Fact]
        public void GetDependedTypes_ShouldReturnDeclaredTypes()
        {
            var attr = new DependsOnAttribute(typeof(IndependentEmptyModule));
            attr.GetDependedTypes().ShouldContain(typeof(IndependentEmptyModule));
        }

        [Fact]
        public void Attribute_OnClass_ShouldBeReadable()
        {
            var attrs = typeof(ModuleWithDep).GetCustomAttributes<DependsOnAttribute>().ToArray();
            attrs.ShouldNotBeEmpty();
            attrs[0].DependedTypes.ShouldContain(typeof(IndependentEmptyModule));
        }

        [Fact]
        public void Attribute_AllowMultiple()
        {
            var usage = typeof(DependsOnAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            usage.ShouldNotBeNull();
            usage.AllowMultiple.ShouldBeTrue();
        }
    }
}
