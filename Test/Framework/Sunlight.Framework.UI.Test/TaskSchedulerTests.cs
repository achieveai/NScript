namespace Sunlight.Framework.UI.Test
{
    using System;
    using System.Collections.Generic;
    using SunlightUnit;

    [TestFixture]
    public class TaskSchedulerTests
    {
        [Test]
        public static void TestQueuedTaskFailuresBubbleToUnhandledPath(Assert assert)
        {
            var timer = new QueuedWindowTimer();
            var scheduler = new TaskScheduler(timer, 10, 10);

            scheduler.EnqueueTask(
                delegate
                {
                    throw new Exception("queued task boom");
                },
                "queued-task-failure");

            assert.Equal(timer.PendingCount, 1, "Queued work should schedule a scheduler callback");

            timer.RunNext();
            assert.Equal(timer.PendingCount, 1, "A failed task should queue an unhandled exception callback");

            string message = "";
            try
            {
                timer.RunNext();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            assert.Equal(message, "queued task boom", "Unhandled task callback should rethrow the original exception");
            assert.Equal(timer.PendingCount, 0, "Unhandled exception callback should be drained after it runs");
        }

        [Test]
        public static void TestTimerTaskFailuresBubbleToUnhandledPath(Assert assert)
        {
            var timer = new QueuedWindowTimer();
            var scheduler = new TaskScheduler(timer, 10, 10);

            scheduler.EnqueueOnTimeout(
                delegate
                {
                    throw new Exception("timer task boom");
                },
                "timer-task-failure",
                1);

            assert.Equal(timer.PendingCount, 1, "Timer work should queue the timer callback");

            timer.RunNext();
            assert.Equal(timer.PendingCount, 1, "A failed timer callback should queue an unhandled exception callback");

            string message = "";
            try
            {
                timer.RunNext();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            assert.Equal(message, "timer task boom", "Unhandled timer callback should rethrow the original exception");
            assert.Equal(timer.PendingCount, 0, "Unhandled exception callback should be drained after it runs");
        }

        [Test]
        public static void TestLowPriTaskFailuresBubbleToUnhandledPath(Assert assert)
        {
            var timer = new QueuedWindowTimer();
            var scheduler = new TaskScheduler(timer, 10, 10);

            scheduler.EnqueueLowPriTask(
                delegate
                {
                    throw new Exception("low-pri task boom");
                },
                "low-pri-task-failure");

            assert.Equal(timer.PendingCount, 1, "Low-pri work should schedule a scheduler callback");

            timer.RunNext();
            assert.Equal(timer.PendingCount, 1, "A failed low-pri task should queue an unhandled exception callback");

            string message = "";
            try
            {
                timer.RunNext();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            assert.Equal(message, "low-pri task boom", "Unhandled low-pri callback should rethrow the original exception");
            assert.Equal(timer.PendingCount, 0, "Unhandled exception callback should be drained after it runs");
        }

        private class QueuedWindowTimer : IWindowTimer
        {
            private readonly Queue<Action> pendingActions = new Queue<Action>();
            private int nextHandle = 1;

            public int PendingCount
            {
                get { return this.pendingActions.Count; }
            }

            public int SetImmediate(Action action)
            {
                this.pendingActions.Enqueue(action);
                return this.nextHandle++;
            }

            public int SetTimeout(Action action, int timoutTime)
            {
                this.pendingActions.Enqueue(action);
                return this.nextHandle++;
            }

            public int SetInterval(Action action, int intervalTime)
            {
                throw new NotImplementedException();
            }

            public void ClearTimeout(int timeoutHandle)
            {
            }

            public void ClearInterval(int intervalHandle)
            {
            }

            public int RequestAnimationFrame(Action action)
            {
                this.pendingActions.Enqueue(action);
                return this.nextHandle++;
            }

            public void RunNext()
            {
                Action action = this.pendingActions.Dequeue();
                action();
            }
        }
    }
}
