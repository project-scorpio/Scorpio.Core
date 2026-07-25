using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Conventional;

using Shouldly;

using Xunit;

namespace Scorpio.DependencyInjection.Conventional
{
    public class DependencyInjectionConventionalContextExtensions_Tests
    {
        private static IConventionalContext<ConventionalDependencyAction> GetContext()
        {
            var services = new ServiceCollection();
            var config = new ConventionalConfiguration<ConventionalDependencyAction>(services, new Type[] { });
            return config.GetContext().As<IConventionalContext<ConventionalDependencyAction>>();
        }

        [Theory]
        [InlineData(ServiceLifetime.Transient)]
        [InlineData(ServiceLifetime.Scoped)]
        [InlineData(ServiceLifetime.Singleton)]
        public void Lifetime_WithServiceLifetime_ShouldSetSelector(ServiceLifetime lifetime)
        {
            var context = GetContext();
            context.Lifetime(lifetime);
            context.Get<IRegisterAssemblyLifetimeSelector>("Lifetime").ShouldNotBeNull();
        }

        [Fact]
        public void Lifetime_WithCustomSelector_ShouldSetSelector()
        {
            var context = GetContext();
            var selector = LifetimeSelector.Transient;
            context.Lifetime(selector);
            context.Get<IRegisterAssemblyLifetimeSelector>("Lifetime").ShouldBe(selector);
        }

        [Fact]
        public void As_WithCustomSelector_ShouldAddToCollection()
        {
            var context = GetContext();
            context.As(SelfSelector.Instance);
            var col = context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service");
            col.ShouldContain(SelfSelector.Instance);
        }

        [Fact]
        public void AsGeneric_ShouldAddTypeSelector()
        {
            var context = GetContext();
            context.As<IDisposable>();
            var col = context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service");
            col.ShouldNotBeEmpty();
        }

        [Fact]
        public void AsDefault_ShouldAddDefaultInterfaceSelector()
        {
            var context = GetContext();
            context.AsDefault();
            var col = context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service");
            col.ShouldContain(DefaultInterfaceSelector.Instance);
        }

        [Fact]
        public void AsAll_ShouldAddAllInterfaceSelector()
        {
            var context = GetContext();
            context.AsAll();
            var col = context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service");
            col.ShouldContain(AllInterfaceSelector.Instance);
        }

        [Fact]
        public void AsSelf_ShouldAddSelfSelector()
        {
            var context = GetContext();
            context.AsSelf();
            var col = context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service");
            col.ShouldContain(SelfSelector.Instance);
        }

        [Fact]
        public void AsExposeService_ShouldAddExposeServicesSelector()
        {
            var context = GetContext();
            context.AsExposeService();
            var col = context.Get<ICollection<IRegisterAssemblyServiceSelector>>("Service");
            col.ShouldContain(ExposeServicesSelector.Instance);
            context.Get<IRegisterAssemblyLifetimeSelector>("Lifetime").ShouldBe(ExposeLifetimeSelector.Instance);
        }

        [Fact]
        public void ChainCalls_ShouldReturnSameContext()
        {
            var context = GetContext();
            var result = context.AsDefault().AsSelf().Lifetime(ServiceLifetime.Singleton);
            result.ShouldBe(context);
        }
    }
}
