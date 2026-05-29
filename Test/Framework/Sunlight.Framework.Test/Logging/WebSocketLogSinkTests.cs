//-----------------------------------------------------------------------
// <copyright file="WebSocketLogSinkTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Logging
{
    using System;
    using System.Collections.Generic;
    using SunlightUnit;
    using Sunlight.Framework;

    /// <summary>
    /// Pure-logic tests for <see cref="WebSocketLogSink"/>. Uses a
    /// captured-payload <see cref="Action{T}"/>, a manually-ticked
    /// <see cref="IWindowTimer"/>, and a manually-advanced clock so we
    /// never touch a real WebSocket or sleep on wall-clock time. This is
    /// the framework-side counterpart of v2's test strategy.
    /// </summary>
    [TestFixture]
    public class WebSocketLogSinkTests
    {
        /// <summary>
        /// Single-callback timer recorder. Mirrors the existing
        /// <c>ControllableTimer</c> shape from <c>LoggerTests</c> so this
        /// suite stays uniform.
        /// </summary>
        private class ManualWindowTimer : IWindowTimer
        {
            public Action IntervalCallback;
            public int IntervalMs;
            public int ClearedIntervalHandle = -1;

            public int SetImmediate(Action action) { action(); return 0; }
            public int SetTimeout(Action action, int timeoutTime) { action(); return 0; }

            public int SetInterval(Action action, int intervalTime)
            {
                this.IntervalCallback = action;
                this.IntervalMs = intervalTime;
                return 99;
            }

            public void ClearTimeout(int timeoutHandle) { }

            public void ClearInterval(int intervalHandle)
            {
                this.ClearedIntervalHandle = intervalHandle;
                this.IntervalCallback = null;
            }

            public int RequestAnimationFrame(Action action) { action(); return 0; }

            public void Tick()
            {
                if (this.IntervalCallback != null) { this.IntervalCallback(); }
            }
        }

        /// <summary>
        /// Tiny clock the test controls. Avoids any wall-clock dependency.
        /// </summary>
        private class FakeClock
        {
            public int NowMs = 1000;
            public int Read() { return this.NowMs; }
        }

        private static LogEvent MakeEvent(string id, string msg)
        {
            return new LogEvent(id, "ts", LogLevel.Info, string.Empty, msg, null, null);
        }

        private static WebSocketLogSink NewSink(
            List<string> payloads,
            Func<bool> isConnected,
            ManualWindowTimer timer,
            FakeClock clock,
            int batchSize,
            int ackTimeoutMs,
            int maxRetry)
        {
            return new WebSocketLogSink(
                isConnected,
                payload => payloads.Add(payload),
                batchSize: batchSize,
                flushIntervalMs: 5000,
                maxQueueSize: 100,
                ackTimeoutMs: ackTimeoutMs,
                maxRetry: maxRetry,
                timer: timer,
                nowMsOverride: clock.Read);
        }

        [Test]
        public static void TestBatchFlushPopulatesInFlight(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            bool connected = true;
            var sink = NewSink(payloads, () => connected, timer, clock, batchSize: 2, ackTimeoutMs: 1000, maxRetry: 3);

            sink.Handle(MakeEvent("id-a", "a"));
            assert.Equal(0, payloads.Count, "No flush yet at batchSize=2 with only 1 event");

            sink.Handle(MakeEvent("id-b", "b"));
            assert.Equal(1, payloads.Count, "Reached batchSize → one flush");
            assert.Equal(2, sink.InFlightCount, "Both events in flight after flush");
            assert.IsTrue(payloads[0].IndexOf("\"id\":\"id-a\"") >= 0, "Payload contains id-a");
            assert.IsTrue(payloads[0].IndexOf("\"id\":\"id-b\"") >= 0, "Payload contains id-b");
        }

        [Test]
        public static void TestHandleAckRemovesInFlight(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var sink = NewSink(payloads, () => true, timer, clock, batchSize: 1, ackTimeoutMs: 1000, maxRetry: 3);

            sink.Handle(MakeEvent("id-1", "m"));
            assert.Equal(1, sink.InFlightCount, "Event went in flight");

            sink.HandleAck(new string[] { "id-1" });
            assert.Equal(0, sink.InFlightCount, "Ack removed from in-flight map");
        }

        [Test]
        public static void TestTimeoutTriggersResendIncrementingRetryCount(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var sink = NewSink(payloads, () => true, timer, clock, batchSize: 1, ackTimeoutMs: 500, maxRetry: 3);

            sink.Handle(MakeEvent("retry-1", "m"));
            assert.Equal(1, payloads.Count, "First send happened at flush");

            // Advance past the ack timeout, then tick.
            clock.NowMs = clock.NowMs + 600;
            timer.Tick();

            assert.Equal(2, payloads.Count, "Timer tick re-sent the unacked event");
            assert.Equal(1, sink.InFlightCount, "Still tracking the same event after retry");
        }

        [Test]
        public static void TestRetryExhaustedDropsEventAndIncrementsDroppedCount(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var sink = NewSink(payloads, () => true, timer, clock, batchSize: 1, ackTimeoutMs: 100, maxRetry: 2);

            LogEvent giveUp = null;
            sink.RetryExhausted += evt => { giveUp = evt; };

            sink.Handle(MakeEvent("doomed", "m"));
            assert.Equal(1, payloads.Count, "First send");

            // Retry 1
            clock.NowMs = clock.NowMs + 200; timer.Tick();
            assert.Equal(2, payloads.Count, "Retry 1 sent");

            // Retry 2 (= maxRetry)
            clock.NowMs = clock.NowMs + 200; timer.Tick();
            assert.Equal(3, payloads.Count, "Retry 2 sent");

            // Next tick: retryCount > maxRetry → drop + RetryExhausted
            clock.NowMs = clock.NowMs + 200; timer.Tick();

            assert.Equal(0, sink.InFlightCount, "Doomed event removed from in-flight");
            assert.Equal(1, sink.DroppedCount, "Dropped count incremented");
            assert.IsTrue(giveUp != null, "RetryExhausted event raised");
            assert.Equal("doomed", giveUp.Id, "Surfaced event carries original id");
        }

        [Test]
        public static void TestNotifyDisconnectedSurfacesPendingAndInFlight(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            bool connected = true;
            var sink = NewSink(payloads, () => connected, timer, clock, batchSize: 2, ackTimeoutMs: 1000, maxRetry: 3);

            // One event goes in flight (batch of 1 after batchSize=2 + handle once is not enough; force flush with 2)
            sink.Handle(MakeEvent("inflight-1", "a"));
            sink.Handle(MakeEvent("inflight-2", "b"));
            assert.Equal(1, payloads.Count, "Batch flushed");
            assert.Equal(2, sink.InFlightCount, "Two events in flight");

            // Third event sits queued because batchSize=2 and the queue
            // resets after flush — Handle pushes one more without
            // triggering a new flush.
            sink.Handle(MakeEvent("queued-1", "c"));
            assert.Equal(1, sink.QueueCount, "Third event queued, not flushed");

            List<LogEvent> drained = null;
            sink.DisconnectedWithPending += list => { drained = list; };
            connected = false;
            sink.NotifyDisconnected();

            assert.IsTrue(drained != null, "DisconnectedWithPending fired");
            assert.Equal(3, drained.Count, "Both queued and in-flight surfaced together");
            assert.Equal(0, sink.InFlightCount, "Local in-flight cleared after notify");
            assert.Equal(0, sink.QueueCount, "Local queue cleared after notify");
        }

        [Test]
        public static void TestFlushSkippedWhenDisconnected(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            bool connected = false;
            var sink = NewSink(payloads, () => connected, timer, clock, batchSize: 1, ackTimeoutMs: 1000, maxRetry: 3);

            sink.Handle(MakeEvent("queued", "m"));

            assert.Equal(0, payloads.Count, "No flush attempt while disconnected");
            assert.Equal(1, sink.QueueCount, "Event stays in queue waiting for connection");
        }

        [Test]
        public static void TestDetachClearsIntervalAndStopsAccepting(Assert assert)
        {
            var payloads = new List<string>();
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var sink = NewSink(payloads, () => true, timer, clock, batchSize: 100, ackTimeoutMs: 1000, maxRetry: 3);

            sink.Handle(MakeEvent("pre-detach", "m"));

            sink.Detach();
            assert.Equal(99, timer.ClearedIntervalHandle, "Detach cleared the interval");

            sink.Handle(MakeEvent("post-detach", "m"));
            assert.Equal(1, payloads.Count, "Post-detach Handle is a no-op (only the final detach flush shipped)");
        }
    }
}
