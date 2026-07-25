using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Scorpio.Modularity;

using Shouldly;

using Xunit;

namespace Scorpio.Threading
{
    public class ScorpioTimer_Tests : TestBase.IntegratedTest<IndependentEmptyModule>
    {
        #region Constructor & Properties

        [Fact]
        public void Constructor_ShouldInitializeDefaults()
        {
            using var timer = new ScorpioTimer();
            timer.Period.ShouldBe(0);
            timer.RunOnStart.ShouldBeFalse();
            timer.Logger.ShouldNotBeNull();
        }

        [Fact]
        public void Logger_ShouldBeAssignable()
        {
            using var timer = new ScorpioTimer();
            var logger = ServiceProvider.GetRequiredService<ILogger<ScorpioTimer>>();
            timer.Logger = logger;
            timer.Logger.ShouldBeSameAs(logger);
        }

        [Fact]
        public void IsTransientDependency_ShouldResolveNewInstanceEachTime()
        {
            var t1 = ServiceProvider.GetService<ScorpioTimer>();
            var t2 = ServiceProvider.GetService<ScorpioTimer>();
            try
            {
                t1.ShouldNotBeSameAs(t2);
            }
            finally
            {
                t1?.Dispose();
                t2?.Dispose();
            }
        }

        #endregion

        #region Start

        [Fact]
        public void Start_WhenPeriodIsZero_ShouldThrowScorpioException()
        {
            using var timer = new ScorpioTimer();
            timer.Period = 0;
            Should.Throw<ScorpioException>(() => timer.Start())
                  .Message.ShouldContain("Period");
        }

        [Fact]
        public void Start_WhenPeriodIsNegative_ShouldThrowScorpioException()
        {
            using var timer = new ScorpioTimer();
            timer.Period = -1;
            Should.Throw<ScorpioException>(() => timer.Start());
        }

        [Fact]
        public void Start_WithValidPeriod_ShouldNotThrow()
        {
            using var timer = new ScorpioTimer { Period = 500 };
            Should.NotThrow(() => timer.Start());
            timer.Stop();
        }

        #endregion

        #region Elapsed event & timing behaviour

        [Fact]
        public async Task Elapsed_ShouldFireAfterPeriod()
        {
            using var timer = new ScorpioTimer { Period = 50 };
            var fired = false;
            timer.Elapsed += (_, _) => fired = true;
            timer.Start();
            await Task.Delay(300);
            timer.Stop();
            fired.ShouldBeTrue();
        }

        [Fact]
        public async Task Elapsed_ShouldFireMultipleTimes()
        {
            using var timer = new ScorpioTimer { Period = 50 };
            var count = 0;
            timer.Elapsed += (_, _) => Interlocked.Increment(ref count);
            timer.Start();
            await Task.Delay(500);
            timer.Stop();
            count.ShouldBeGreaterThan(1);
        }

        [Fact]
        public async Task RunOnStart_True_ShouldFireImmediately()
        {
            using var timer = new ScorpioTimer { Period = 10_000, RunOnStart = true };
            var fired = false;
            timer.Elapsed += (_, _) => fired = true;
            timer.Start();
            await Task.Delay(300);
            timer.Stop();
            fired.ShouldBeTrue();
        }

        [Fact]
        public async Task RunOnStart_False_ShouldNotFireBeforePeriod()
        {
            using var timer = new ScorpioTimer { Period = 10_000, RunOnStart = false };
            var fired = false;
            timer.Elapsed += (_, _) => fired = true;
            timer.Start();
            await Task.Delay(100);
            timer.Stop();
            fired.ShouldBeFalse();
        }

        [Fact]
        public async Task Elapsed_ShouldNotOverlap_WhenHandlerIsSlowerThanPeriod()
        {
            using var timer = new ScorpioTimer { Period = 50 };
            var concurrent = 0;
            var maxConcurrent = 0;
            timer.Elapsed += (_, _) =>
            {
                var c = Interlocked.Increment(ref concurrent);
                if (c > maxConcurrent) maxConcurrent = c;
                Thread.Sleep(200);
                Interlocked.Decrement(ref concurrent);
            };
            timer.Start();
            await Task.Delay(600);
            timer.Stop();
            maxConcurrent.ShouldBe(1);
        }

        #endregion

        #region Stop

        [Fact]
        public async Task Stop_ShouldPreventFurtherFiring()
        {
            using var timer = new ScorpioTimer { Period = 50 };
            var count = 0;
            timer.Elapsed += (_, _) => Interlocked.Increment(ref count);
            timer.Start();
            await Task.Delay(200);
            timer.Stop();
            var countAfterStop = count;
            await Task.Delay(200);
            count.ShouldBe(countAfterStop);
        }

        [Fact]
        public void Stop_WhenNotStarted_ShouldNotThrow()
        {
            using var timer = new ScorpioTimer { Period = 100 };
            Should.NotThrow(() => timer.Stop());
        }

        [Fact]
        public async Task Stop_ShouldWaitForRunningHandler_ToComplete()
        {
            using var timer = new ScorpioTimer { Period = 50, RunOnStart = true };
            var handlerCompleted = false;
            timer.Elapsed += (_, _) =>
            {
                Thread.Sleep(200);
                handlerCompleted = true;
            };
            timer.Start();
            await Task.Delay(80);   // let handler start
            timer.Stop();
            handlerCompleted.ShouldBeTrue();
        }

        #endregion

        #region Exception handling

        [Fact]
        public async Task Elapsed_ExceptionInHandler_ShouldNotStopTimer()
        {
            using var timer = new ScorpioTimer { Period = 50 };
            var countAfterException = 0;
            var threw = false;
            timer.Elapsed += (_, _) =>
            {
                if (!threw)
                {
                    threw = true;
                    throw new InvalidOperationException("test error");
                }
                Interlocked.Increment(ref countAfterException);
            };
            timer.Start();
            await Task.Delay(500);
            timer.Stop();
            countAfterException.ShouldBeGreaterThan(0);
        }

        #endregion

        #region Dispose

        [Fact]
        public void Dispose_ShouldNotThrow()
        {
            var timer = new ScorpioTimer { Period = 100 };
            Should.NotThrow(() => timer.Dispose());
        }

        [Fact]
        public void Dispose_CalledMultipleTimes_ShouldNotThrow()
        {
            var timer = new ScorpioTimer { Period = 100 };
            Should.NotThrow(() =>
            {
                timer.Dispose();
                timer.Dispose();
            });
        }

        [Fact]
        public async Task Dispose_WhileRunning_ShouldStopTimer()
        {
            var timer = new ScorpioTimer { Period = 50 };
            var count = 0;
            timer.Elapsed += (_, _) => Interlocked.Increment(ref count);
            timer.Start();
            await Task.Delay(200);
            timer.Dispose();
            var countAtDispose = count;
            await Task.Delay(200);
            count.ShouldBe(countAtDispose);
        }

        [Fact]
        public void Dispose_ViaUsing_ShouldNotThrow()
        {
            Should.NotThrow(() =>
            {
                using var timer = new ScorpioTimer { Period = 100 };
                timer.Start();
                timer.Stop();
            });
        }

        #endregion

        #region DI integration

        [Fact]
        public void ServiceProvider_ShouldResolve_ScorpioTimer()
        {
            using var timer = ServiceProvider.GetService<ScorpioTimer>();
            timer.ShouldNotBeNull();
        }

        [Fact]
        public void ServiceProvider_ShouldResolve_IScorpioTimer()
        {
            using var timer = ServiceProvider.GetService<ScorpioTimer>();
            (timer as IScorpioTimer).ShouldNotBeNull();
        }

        #endregion
    }
}

