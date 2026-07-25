using System;

using Shouldly;

using Xunit;

namespace Scorpio.DynamicProxy
{
    public class ProxyTargetProvider_Tests
    {
        [Fact]
        public void Default_ShouldReturnSameInstance()
        {
            var a = ProxyTargetProvider.Default;
            var b = ProxyTargetProvider.Default;
            a.ShouldBe(b);
        }

        [Fact]
        public void Add_ShouldRegisterProvider()
        {
            // Create a fresh provider via default and check Add doesn't throw
            var provider = ProxyTargetProvider.Default;
            var fakeProvider = new FakeProxyTargetProvider();
            // No exception expected
            provider.Add(fakeProvider);
        }

        private class FakeProxyTargetProvider : IProxyTargetProvider
        {
            public bool IsProxy(object proxy) => false;
            public object GetTarget(object proxy) => null;
        }
    }

    public class ProxyHelper_Tests
    {
        private class PlainObject { }

        [Fact]
        public void IsProxy_NonProxyObject_ShouldReturnFalse()
        {
            var obj = new PlainObject();
            obj.IsProxy().ShouldBeFalse();
        }

        [Fact]
        public void UnProxy_NonProxyObject_ShouldReturnSelf()
        {
            var obj = new PlainObject();
            obj.UnProxy().ShouldBe(obj);
        }
    }
}
