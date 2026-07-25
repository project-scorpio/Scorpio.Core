using Shouldly;

using Xunit;

namespace Scorpio.Runtime
{
    public class AsyncLocalAmbientDataContext_Tests
    {
        [Fact]
        public void SetData_And_GetData_ShouldRoundtrip()
        {
            var ctx = new AsyncLocalAmbientDataContext();
            ctx.SetData("key1", "value1");
            ctx.GetData("key1").ShouldBe("value1");
        }

        [Fact]
        public void GetData_ShouldReturnNull_WhenKeyNotSet()
        {
            var ctx = new AsyncLocalAmbientDataContext();
            ctx.GetData("nonexistent_key_xyz").ShouldBeNull();
        }

        [Fact]
        public void SetData_ShouldOverwritePreviousValue()
        {
            var ctx = new AsyncLocalAmbientDataContext();
            ctx.SetData("key2", "first");
            ctx.SetData("key2", "second");
            ctx.GetData("key2").ShouldBe("second");
        }

        [Fact]
        public void SetData_ShouldAcceptNullValue()
        {
            var ctx = new AsyncLocalAmbientDataContext();
            ctx.SetData("key3", "something");
            ctx.SetData("key3", null);
            ctx.GetData("key3").ShouldBeNull();
        }
    }
}
