using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Scorpio.Threading
{
    public class LockExtensions_Tests
    {
        #region Locking(Action)

        [Fact]
        public void Locking_Action_ShouldExecuteAction()
        {
            var executed = false;
            var obj = new object();
            obj.Locking(() => executed = true);
            executed.ShouldBeTrue();
        }

        [Fact]
        public void Locking_Action_ShouldBeThreadSafe()
        {
            var counter = 0;
            var obj = new object();
            Parallel.For(0, 1000, _ => obj.Locking(() => counter++));
            counter.ShouldBe(1000);
        }

        #endregion

        #region Locking<T>(Action<T>)

        [Fact]
        public void Locking_ActionT_ShouldPassSourceToAction()
        {
            var list = new System.Collections.Generic.List<int>();
            list.Locking(l => l.Add(1));
            list.ShouldHaveSingleItem().ShouldBe(1);
        }

        [Fact]
        public void Locking_ActionT_ShouldBeThreadSafe()
        {
            var list = new System.Collections.Generic.List<int>();
            Parallel.For(0, 500, i => list.Locking(l => l.Add(i)));
            list.Count.ShouldBe(500);
        }

        #endregion

        #region Locking<TResult>(Func<TResult>)

        [Fact]
        public void Locking_Func_ShouldReturnValue()
        {
            var obj = new object();
            var result = obj.Locking(() => 42);
            result.ShouldBe(42);
        }

        [Fact]
        public void Locking_Func_ShouldBeThreadSafe()
        {
            var counter = 0;
            var obj = new object();
            Parallel.For(0, 1000, _ => obj.Locking(() => Interlocked.Increment(ref counter)));
            counter.ShouldBe(1000);
        }

        #endregion

        #region Locking<T, TResult>(Func<T, TResult>)

        [Fact]
        public void Locking_FuncT_ShouldPassSourceAndReturnValue()
        {
            var list = new System.Collections.Generic.List<int> { 10, 20, 30 };
            var result = list.Locking(l => l.Count);
            result.ShouldBe(3);
        }

        [Fact]
        public void Locking_FuncT_ShouldBeThreadSafe()
        {
            var list = new System.Collections.Generic.List<int>();
            Parallel.For(0, 500, i => list.Locking(l => { l.Add(i); return l.Count; }));
            list.Count.ShouldBe(500);
        }

        #endregion
    }
}
