using System;
using System.Threading;
using System.Threading.Tasks;
using GeneticSharp.Infrastructure.Framework.Threading;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
    [TestFixture()]
    [Category("Infrastructure")]
    public class ParallelTaskExecutorTest
    {
        [Test()]
        public void Start_ManyTasks_ParallelExecuted()
        {
            var pipeline = "";
            var target = new ParallelTaskExecutor();
            target.Add(() =>
            {
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(100);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "3";
            });

            var actual = target.Start();
            Assert.IsTrue(actual);
            Assert.AreEqual("132", pipeline);
        }

        [Test()]
        public void Start_Timeout_False()
        {
            var pipeline = "1";
            var target = new ParallelTaskExecutor();
            target.Timeout = TimeSpan.FromMilliseconds(2);

            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "2";
            });

            var actual = target.Start();
            Assert.IsFalse(actual);
        }

        [Test()]
        public void Start_AnyTaskWithException_Exception()
        {
            var pipeline = "";
            var target = new ParallelTaskExecutor();

            target.Add(() =>
            {
                throw new Exception("1");
            });
            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "3";
            });

            Assert.Catch<Exception>(() =>
            {
                target.Start();
            }, "1");
        }

        [Test()]
        public void Stop_ManyTasks_StopAll()
        {
            var pipeline = "";
            var target = new ParallelTaskExecutor();
			target.Timeout = TimeSpan.FromMilliseconds(1000);

			target.Add(() =>
			{
				Thread.Sleep(5);
				pipeline += "1";
			});
			target.Add(() =>
			{
				Thread.Sleep(5);
				pipeline += "2";
			});
			target.Add(() =>
			{
				Thread.Sleep(5);
				pipeline += "3";
			});

			Parallel.Invoke(
				() => Assert.IsTrue(target.Start()),
				() =>
				{
					Thread.Sleep(100);
					target.Stop();
				});
			
        }

        [Test()]
        public void Stop_Tasks_ShutdownCalled()
        {
            var pipeline = "";
            var target = new ParallelTaskExecutor();
            target.Timeout = TimeSpan.FromMilliseconds(1000);

            target.Add(() =>
            {
                Thread.Sleep(500);
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(500);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(500);
                pipeline += "3";
            });

            Parallel.Invoke(
                () => target.Start(),
                () =>
                {
                    Thread.Sleep(100);
                    target.Stop();
                });

            Assert.IsFalse(target.IsRunning);
        }
    }
}

