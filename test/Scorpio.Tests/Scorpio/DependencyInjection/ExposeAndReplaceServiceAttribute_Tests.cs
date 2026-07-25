using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace Scorpio.DependencyInjection
{
    public class ExposeServicesAttribute_Tests
    {
        [ExposeServices(typeof(IMyService))]
        private class MyServiceImpl : IMyService { }

        private interface IMyService { }

        [Fact]
        public void Constructor_ShouldSetExposedServiceTypes()
        {
            var attr = new ExposeServicesAttribute(typeof(IMyService), typeof(string));
            attr.ExposedServiceTypes.ShouldContain(typeof(IMyService));
            attr.ExposedServiceTypes.ShouldContain(typeof(string));
        }

        [Fact]
        public void Constructor_NullParam_ShouldResultInEmptyArray()
        {
            var attr = new ExposeServicesAttribute(null);
            attr.ExposedServiceTypes.ShouldBeEmpty();
        }

        [Fact]
        public void GetExposedServiceTypes_ShouldReturnDeclaredTypes()
        {
            var attr = new ExposeServicesAttribute(typeof(IMyService));
            var types = attr.GetExposedServiceTypes(typeof(MyServiceImpl));
            types.ShouldContain(typeof(IMyService));
        }

        [Fact]
        public void ServiceLifetime_DefaultShouldBeTransient()
        {
            var attr = new ExposeServicesAttribute(typeof(IMyService));
            attr.ServiceLifetime.ShouldBe(ServiceLifetime.Transient);
        }

        [Fact]
        public void ServiceLifetime_CanBeChanged()
        {
            var attr = new ExposeServicesAttribute(typeof(IMyService)) { ServiceLifetime = ServiceLifetime.Singleton };
            attr.ServiceLifetime.ShouldBe(ServiceLifetime.Singleton);
        }

        [Fact]
        public void Attribute_OnClass_ShouldBeReadable()
        {
            var attr = typeof(MyServiceImpl).GetCustomAttribute<ExposeServicesAttribute>();
            attr.ShouldNotBeNull();
            attr.ExposedServiceTypes.ShouldContain(typeof(IMyService));
        }
    }

    public class ReplaceServiceAttribute_Tests
    {
        [ReplaceService(ReplaceService = true)]
        private class ReplacingService { }

        [ReplaceService]
        private class DefaultReplaceService { }

        [Fact]
        public void ReplaceService_WhenSetToTrue_ShouldBeTrue()
        {
            var attr = typeof(ReplacingService).GetCustomAttribute<ReplaceServiceAttribute>();
            attr.ShouldNotBeNull();
            attr.ReplaceService.ShouldBeTrue();
        }

        [Fact]
        public void ReplaceService_WhenDefault_ShouldBeFalse()
        {
            var attr = typeof(DefaultReplaceService).GetCustomAttribute<ReplaceServiceAttribute>();
            attr.ShouldNotBeNull();
            attr.ReplaceService.ShouldBeFalse();
        }

        [Fact]
        public void Constructor_ShouldCreateAttributeWithFalseDefault()
        {
            var attr = new ReplaceServiceAttribute();
            attr.ReplaceService.ShouldBeFalse();
        }
    }
}
