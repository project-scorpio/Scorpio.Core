using System;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Xunit;

namespace System
{
    public class DisposeAction_Tests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenActionIsNull()
        {
            Should.Throw<ArgumentNullException>(() => new DisposeAction(null));
        }

        [Fact]
        public void Dispose_ShouldInvokeAction()
        {
            var invoked = false;
            var sut = new DisposeAction(() => invoked = true);
            invoked.ShouldBeFalse();
            sut.Dispose();
            invoked.ShouldBeTrue();
        }

        [Fact]
        public void Dispose_ShouldOnlyInvokeActionOnce_WhenCalledMultipleTimes()
        {
            var count = 0;
            var sut = new DisposeAction(() => count++);
            sut.Dispose();
            sut.Dispose();
            count.ShouldBe(1);
        }

        [Fact]
        public void Dispose_ShouldWork_WithUsingStatement()
        {
            var invoked = false;
            using (new DisposeAction(() => invoked = true))
            {
                invoked.ShouldBeFalse();
            }
            invoked.ShouldBeTrue();
        }
    }

    public class AsyncDisposeAction_Tests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenActionIsNull()
        {
            Should.Throw<ArgumentNullException>(() => new AsyncDisposeAction(null));
        }

        [Fact]
        public async Task DisposeAsync_ShouldInvokeAction()
        {
            var invoked = false;
            var sut = new AsyncDisposeAction(() =>
            {
                invoked = true;
                return new ValueTask(Task.CompletedTask);
            });
            invoked.ShouldBeFalse();
            await sut.DisposeAsync();
            invoked.ShouldBeTrue();
        }

        [Fact]
        public async Task DisposeAsync_ShouldOnlyInvokeActionOnce_WhenCalledMultipleTimes()
        {
            var count = 0;
            var sut = new AsyncDisposeAction(() =>
            {
                count++;
                return new ValueTask(Task.CompletedTask);
            });
            await sut.DisposeAsync();
            await sut.DisposeAsync();
            count.ShouldBe(1);
        }

        [Fact]
        public async Task DisposeAsync_ShouldWork_WithAwaitUsing()
        {
            var invoked = false;
            await using (new AsyncDisposeAction(() =>
            {
                invoked = true;
                return new ValueTask(Task.CompletedTask);
            }))
            {
                invoked.ShouldBeFalse();
            }
            invoked.ShouldBeTrue();
        }
    }
}
