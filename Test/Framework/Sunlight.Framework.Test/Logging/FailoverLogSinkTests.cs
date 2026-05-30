//-----------------------------------------------------------------------
// <copyright file="FailoverLogSinkTests.cs" company="">
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
    /// Pure-logic tests for <see cref="FailoverLogSink"/>. The HTTP sink
    /// is real (with a transportOverride capture) so we can assert the
    /// HTTP path's serialization; the WS sink is real with fake
    /// callbacks so we can drive its IsConnected / disconnect events.
    /// </summary>
    [TestFixture]
    public class FailoverLogSinkTests
    {
        /// <summary>
        /// Multi-callback timer — both WS and HTTP sinks call SetInterval
        /// on it during construction, so we must retain BOTH callbacks
        /// to drive the failover dance. Tick fires every registered
        /// callback.
        /// </summary>
        private class ManualWindowTimer : IWindowTimer
        {
            public List<Action> IntervalCallbacks = new List<Action>();
            public int SetImmediate(Action action) { action(); return 0; }
            public int SetTimeout(Action action, int timeoutTime) { action(); return 0; }
            public int SetInterval(Action action, int intervalTime) { this.IntervalCallbacks.Add(action); return this.IntervalCallbacks.Count - 1; }
            public void ClearTimeout(int timeoutHandle) { }
            public void ClearInterval(int intervalHandle)
            {
                if (intervalHandle >= 0 && intervalHandle < this.IntervalCallbacks.Count)
                {
                    this.IntervalCallbacks[intervalHandle] = null;
                }
            }
            public int RequestAnimationFrame(Action action) { action(); return 0; }

            public void Tick()
            {
                for (int i = 0; i < this.IntervalCallbacks.Count; i++)
                {
                    var cb = this.IntervalCallbacks[i];
                    if (cb != null) { cb(); }
                }
            }
        }

        private class FakeClock { public int NowMs = 1000; public int Read() { return this.NowMs; } }

        private static LogEvent MakeEvent(string id, string msg)
        {
            return new LogEvent(id, "ts", LogLevel.Info, string.Empty, msg, null, null);
        }

        [Test]
        public static void TestHandleRoutesToWsWhenConnected(Assert assert)
        {
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var wsPayloads = new List<string>();
            var httpPayloads = new List<string>();
            bool connected = true;

            var ws = new WebSocketLogSink(
                () => connected, p => wsPayloads.Add(p),
                batchSize: 1, flushIntervalMs: 5000, maxQueueSize: 100,
                ackTimeoutMs: 1000, maxRetry: 3, timer: timer, nowMsOverride: clock.Read);
            var http = new HttpLogSink(
                "/log", 1, 5000, 100, timer,
                (endpoint, payload) => httpPayloads.Add(payload));

            var failover = new FailoverLogSink(ws, http, () => connected);

            failover.Handle(MakeEvent("evt-1", "hello"));

            assert.Equal(1, wsPayloads.Count, "Event went to WS sink");
            assert.Equal(0, httpPayloads.Count, "HTTP sink untouched while connected");
        }

        [Test]
        public static void TestHandleRoutesToHttpWhenDisconnected(Assert assert)
        {
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var wsPayloads = new List<string>();
            var httpPayloads = new List<string>();
            bool connected = false;

            var ws = new WebSocketLogSink(
                () => connected, p => wsPayloads.Add(p),
                batchSize: 1, flushIntervalMs: 5000, maxQueueSize: 100,
                ackTimeoutMs: 1000, maxRetry: 3, timer: timer, nowMsOverride: clock.Read);
            var http = new HttpLogSink(
                "/log", 1, 5000, 100, timer,
                (endpoint, payload) => httpPayloads.Add(payload));

            var failover = new FailoverLogSink(ws, http, () => connected);

            failover.Handle(MakeEvent("evt-1", "fallback"));

            assert.Equal(0, wsPayloads.Count, "WS sink skipped while disconnected");
            assert.Equal(1, httpPayloads.Count, "HTTP sink received the event");
        }

        [Test]
        public static void TestMidBatchDisconnectDrainsPendingToHttp(Assert assert)
        {
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var wsPayloads = new List<string>();
            var httpPayloads = new List<string>();
            bool connected = true;

            var ws = new WebSocketLogSink(
                () => connected, p => wsPayloads.Add(p),
                batchSize: 2, flushIntervalMs: 5000, maxQueueSize: 100,
                ackTimeoutMs: 1000, maxRetry: 3, timer: timer, nowMsOverride: clock.Read);
            var http = new HttpLogSink(
                "/log", 1, 5000, 100, timer,
                (endpoint, payload) => httpPayloads.Add(payload));

            var failover = new FailoverLogSink(ws, http, () => connected);

            // Two events flush as a batch via WS.
            failover.Handle(MakeEvent("a", "a"));
            failover.Handle(MakeEvent("b", "b"));
            assert.Equal(1, wsPayloads.Count, "WS batched + flushed");
            assert.Equal(0, httpPayloads.Count, "HTTP untouched");

            // Third event queued (batchSize=2, queue empty after flush).
            failover.Handle(MakeEvent("c", "c"));

            // Disconnect: surface pending + in-flight events to HTTP.
            connected = false;
            ws.NotifyDisconnected();

            // a, b were in flight; c was queued. All three end up on HTTP.
            assert.Equal(3, httpPayloads.Count, "All pending events drained to HTTP");
        }

        [Test]
        public static void TestRetryExhaustedReroutesToHttp(Assert assert)
        {
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var wsPayloads = new List<string>();
            var httpPayloads = new List<string>();

            var ws = new WebSocketLogSink(
                () => true, p => wsPayloads.Add(p),
                batchSize: 1, flushIntervalMs: 5000, maxQueueSize: 100,
                ackTimeoutMs: 100, maxRetry: 1, timer: timer, nowMsOverride: clock.Read);
            var http = new HttpLogSink(
                "/log", 1, 5000, 100, timer,
                (endpoint, payload) => httpPayloads.Add(payload));

            var failover = new FailoverLogSink(ws, http, () => true);

            failover.Handle(MakeEvent("doomed", "m"));
            assert.Equal(1, wsPayloads.Count, "WS sent first attempt");

            // Tick 1: first retry — RetryCount goes 0→1.
            clock.NowMs = clock.NowMs + 200; timer.Tick();
            assert.Equal(2, wsPayloads.Count, "WS resent on tick 1");

            // Tick 2: RetryCount=1 >= maxRetry=1 → RetryExhausted fires.
            clock.NowMs = clock.NowMs + 200; timer.Tick();
            assert.Equal(2, wsPayloads.Count, "No additional WS send on giveup tick");
            assert.Equal(0, ws.InFlightCount, "Doomed event removed from WS in-flight");

            assert.Equal(1, httpPayloads.Count, "Exhausted retry rerouted to HTTP");
        }

        [Test]
        public static void TestDetachForwardsToBothSinks(Assert assert)
        {
            var timer = new ManualWindowTimer();
            var clock = new FakeClock();
            var wsPayloads = new List<string>();
            var httpPayloads = new List<string>();
            bool connected = true;

            var ws = new WebSocketLogSink(
                () => connected, p => wsPayloads.Add(p),
                batchSize: 100, flushIntervalMs: 5000, maxQueueSize: 100,
                ackTimeoutMs: 1000, maxRetry: 3, timer: timer, nowMsOverride: clock.Read);
            var http = new HttpLogSink(
                "/log", 100, 5000, 100, timer,
                (endpoint, payload) => httpPayloads.Add(payload));

            var failover = new FailoverLogSink(ws, http, () => connected);

            // Queue one event on each path before detaching.
            failover.Handle(MakeEvent("ws-q", "ws"));        // WS queue
            connected = false;
            failover.Handle(MakeEvent("http-q", "http"));    // HTTP queue
            connected = true;

            failover.Detach();
            // HTTP Detach flushes residual via sendBeacon path → captured.
            assert.IsTrue(httpPayloads.Count >= 1, "HTTP detach drained its residual");
        }
    }
}
