using System;
using System.Reflection;

using Shouldly;

using Xunit;

namespace Scorpio
{
    public class ScorpioException_Tests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateException()
        {
            var ex = new ScorpioException();
            ex.ShouldBeOfType<ScorpioException>();
            ex.ShouldBeAssignableTo<Exception>();
        }

        [Fact]
        public void MessageConstructor_ShouldSetMessage()
        {
            var ex = new ScorpioException("test message");
            ex.Message.ShouldBe("test message");
        }

        [Fact]
        public void InnerExceptionConstructor_ShouldSetInnerException()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new ScorpioException("outer", inner);
            ex.Message.ShouldBe("outer");
            ex.InnerException.ShouldBe(inner);
        }

        [Fact]
        public void ScorpioException_ShouldBeSerializable()
        {
            typeof(ScorpioException).GetCustomAttribute<SerializableAttribute>().ShouldNotBeNull();
        }
    }
}
