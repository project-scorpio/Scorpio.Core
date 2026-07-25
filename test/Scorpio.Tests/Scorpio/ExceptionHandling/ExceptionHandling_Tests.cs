using System;

using Microsoft.Extensions.Logging;

using Shouldly;

using Xunit;

namespace Scorpio.ExceptionHandling
{
    public class ExceptionNotificationContext_Tests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenExceptionIsNull()
        {
            Should.Throw<ArgumentNullException>(() => new ExceptionNotificationContext(null));
        }

        [Fact]
        public void Constructor_ShouldSetProperties_WithDefaults()
        {
            var ex = new ScorpioException("test");
            var context = new ExceptionNotificationContext(ex);
            context.Exception.ShouldBe(ex);
            context.Handled.ShouldBeTrue();
            context.LogLevel.ShouldBe(ex.GetLogLevel());
        }

        [Fact]
        public void Constructor_ShouldSetExplicitLogLevel()
        {
            var ex = new ScorpioException("test");
            var context = new ExceptionNotificationContext(ex, LogLevel.Warning);
            context.LogLevel.ShouldBe(LogLevel.Warning);
        }

        [Fact]
        public void Constructor_ShouldSetHandled_WhenFalse()
        {
            var ex = new ScorpioException("test");
            var context = new ExceptionNotificationContext(ex, handled: false);
            context.Handled.ShouldBeFalse();
        }
    }

    public class NullExceptionNotifier_Tests
    {
        [Fact]
        public void Instance_ShouldBeSingleton()
        {
            NullExceptionNotifier.Instance.ShouldNotBeNull();
            NullExceptionNotifier.Instance.ShouldBeSameAs(NullExceptionNotifier.Instance);
        }

        [Fact]
        public void NotifyAsync_ShouldNotThrow()
        {
            var context = new ExceptionNotificationContext(new ScorpioException("test"));
            Should.NotThrow(() => NullExceptionNotifier.Instance.NotifyAsync(context));
        }
    }

    public class ExceptionSubscriber_Tests
    {
        private class ConcreteSubscriber : ExceptionSubscriber { }

        [Fact]
        public void HandleAsync_ShouldThrow_WhenContextIsNull()
        {
            var subscriber = new ConcreteSubscriber();
            Should.Throw<ArgumentNullException>(() => subscriber.HandleAsync(null));
        }

        [Fact]
        public void HandleAsync_ShouldComplete_WithValidContext()
        {
            var subscriber = new ConcreteSubscriber();
            var context = new ExceptionNotificationContext(new ScorpioException("test"));
            Should.NotThrow(() => subscriber.HandleAsync(context));
        }
    }
}
