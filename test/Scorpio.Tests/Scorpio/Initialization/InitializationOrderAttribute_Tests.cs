using System;
using System.Reflection;

using Shouldly;

using Xunit;

namespace Scorpio.Initialization
{
    public class InitializationOrderAttribute_Tests
    {
        [InitializationOrder(100)]
        private class HighPriorityService { }

        [InitializationOrder(0)]
        private class ZeroPriorityService { }

        private class NoAttributeService { }

        [Fact]
        public void Constructor_ShouldSetOrder()
        {
            var attr = new InitializationOrderAttribute(50);
            attr.Order.ShouldBe(50);
        }

        [Fact]
        public void GetOrder_WithAttribute_ShouldReturnAttributeOrder()
        {
            var order = InitializationOrderAttribute.GetOrder(typeof(HighPriorityService));
            order.ShouldBe(100);
        }

        [Fact]
        public void GetOrder_WithZeroOrder_ShouldReturnZero()
        {
            var order = InitializationOrderAttribute.GetOrder(typeof(ZeroPriorityService));
            order.ShouldBe(0);
        }

        [Fact]
        public void GetOrder_WithoutAttribute_ShouldReturnDefaultOrder()
        {
            var order = InitializationOrderAttribute.GetOrder(typeof(NoAttributeService));
            order.ShouldBe(0);
        }

        [Fact]
        public void GetOrder_WithoutAttribute_CustomDefault_ShouldReturnCustomDefault()
        {
            var order = InitializationOrderAttribute.GetOrder(typeof(NoAttributeService), 99);
            order.ShouldBe(99);
        }

        [Fact]
        public void Attribute_IsInherited()
        {
            var usage = typeof(InitializationOrderAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            usage.ShouldNotBeNull();
            usage.Inherited.ShouldBeTrue();
        }

        [Fact]
        public void Attribute_NotAllowMultiple()
        {
            var usage = typeof(InitializationOrderAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            usage.ShouldNotBeNull();
            usage.AllowMultiple.ShouldBeFalse();
        }
    }
}
