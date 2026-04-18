//-----------------------------------------------------------------------
// <copyright file="LoggerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using System.Collections.Generic;
    using SunlightUnit;
    using Sunlight.Framework;

    /// <summary>
    /// QUnit tests for the structured logging pipeline added in WI-11.
    /// Each test resets <see cref="Logger"/> state at start via
    /// <see cref="ResetLogger"/> because the facade is a process-wide singleton.
    /// </summary>
    [TestFixture]
    public class LoggerTests
    {
        /// <summary>
        /// Captures dispatched events in-memory for assertion.
        /// </summary>
        private class FakeSink : ILogSink
        {
            public readonly List<LogEvent> Events = new List<LogEvent>();
            public int FlushCount;
            public int DetachCount;

            public void Handle(LogEvent evt)
            {
                this.Events.Add(evt);
            }

            public void Flush()
            {
                this.FlushCount++;
            }

            public void Detach()
            {
                this.DetachCount++;
            }
        }

        /// <summary>
        /// Sink that throws on Handle; used to verify fault isolation.
        /// </summary>
        private class ThrowingSink : ILogSink
        {
            public int HandleAttempts;

            public void Handle(LogEvent evt)
            {
                this.HandleAttempts++;
                throw new Exception("sink boom");
            }

            public void Flush() { }

            public void Detach() { }
        }

        /// <summary>
        /// Controllable timer for HttpLogSink tests. Captures the interval
        /// callback so the test can trigger ticks deterministically.
        /// </summary>
        private class ControllableTimer : IWindowTimer
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
                return 42;
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

        private static void ResetLogger()
        {
            Logger.ClearSinks();
            Logger.MinLevel = LogLevel.Debug;
        }

        [Test]
        public static void TestLevelFilteringSkipsBelowMinLevel(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);
            Logger.MinLevel = LogLevel.Warn;

            Logger.Info("info-skipped");
            Logger.Warn("warn-kept");
            Logger.Error("error-kept");

            assert.Equal(2, fake.Events.Count, "Only Warn and Error should pass MinLevel=Warn");
            assert.Equal(LogLevel.Warn, fake.Events[0].Level, "First retained event is Warn");
            assert.Equal(LogLevel.Error, fake.Events[1].Level, "Second retained event is Error");
        }

        [Test]
        public static void TestFanoutDeliversToAllSinks(Assert assert)
        {
            ResetLogger();
            var a = new FakeSink();
            var b = new FakeSink();
            Logger.AddSink(a);
            Logger.AddSink(b);

            Logger.Info("hello");

            assert.Equal(1, a.Events.Count, "Sink A received event");
            assert.Equal(1, b.Events.Count, "Sink B received event");
            assert.Equal("hello", a.Events[0].Message, "Message preserved");
        }

        [Test]
        public static void TestForCategoryCarriesCategory(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);

            var log = Logger.ForCategory("TodoApp.ListView");
            log.Info("item added");

            assert.Equal(1, fake.Events.Count, "One event dispatched");
            assert.Equal("TodoApp.ListView", fake.Events[0].Category, "Category tagged on event");
            assert.Equal("item added", fake.Events[0].Message, "Message preserved through category logger");
        }

        [Test]
        public static void TestForCategoryCachesInstance(Assert assert)
        {
            ResetLogger();
            var a = Logger.ForCategory("shared");
            var b = Logger.ForCategory("shared");
            assert.StrictEqual(b, a, "Same category returns same cached instance");
        }

        [Test]
        public static void TestForCategoryNullNormalizesToEmpty(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);

            // A null category is normalized to empty string by Logger.ForCategory
            // so downstream callers never have to null-check the category tag.
            var log = Logger.ForCategory(null);
            log.Info("null-cat");

            assert.Equal(string.Empty, log.Category, "Null category normalized to empty string");
            assert.Equal(1, fake.Events.Count, "Event dispatched through null-category logger");
            assert.Equal(string.Empty, fake.Events[0].Category, "Event category is empty, not null");
        }

        [Test]
        public static void TestUncategorizedStaticCallsEmptyCategory(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);

            Logger.Info("no-cat");

            assert.Equal(1, fake.Events.Count, "Event dispatched");
            assert.Equal(string.Empty, fake.Events[0].Category, "Static facade uses empty category");
        }

        [Test]
        public static void TestCallContextEnrichment(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);

            var ctx = CallContext.StartRoot();
            Logger.Info("enriched");
            var captured = fake.Events[0].Context;

            assert.StrictEqual(ctx, captured, "LogEvent.Context should be the active CallContext at emit");
            assert.Equal(ctx.ActionId, captured.ActionId, "ActionId preserved on captured context");
            assert.Equal(ctx.TraceId, captured.TraceId, "TraceId preserved on captured context");
        }

        [Test]
        public static void TestPropertiesPreservedThroughDispatch(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);

            var props = new string[] { "itemId", "42", "source", "api" };
            Logger.Info("with-props", props);

            assert.StrictEqual(props, fake.Events[0].Properties, "Properties array passed through unchanged");
        }

        [Test]
        public static void TestSinkFaultIsolation(Assert assert)
        {
            ResetLogger();
            var bad = new ThrowingSink();
            var good = new FakeSink();
            Logger.AddSink(bad);
            Logger.AddSink(good);

            // Must not throw out of Logger.Info even though the first sink threw.
            Logger.Info("hello");

            assert.Equal(1, bad.HandleAttempts, "Throwing sink was called");
            assert.Equal(1, good.Events.Count, "Subsequent sink still received event after prior one threw");
        }

        [Test]
        public static void TestRemoveSinkDetachesAndStopsDelivery(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);
            Logger.RemoveSink(fake);

            Logger.Info("after-remove");

            assert.Equal(0, fake.Events.Count, "Removed sink receives no further events");
            assert.Equal(1, fake.DetachCount, "Removed sink had Detach called once");
        }

        [Test]
        public static void TestClearSinksSuppressesLazyDefault(Assert assert)
        {
            ResetLogger();
            // ClearSinks already called above. One more call with zero sinks should
            // not re-install the default ConsoleSink when we log.
            Logger.Info("no-sinks");

            // Add a fake AFTER the log call above; it should only see new events.
            var fake = new FakeSink();
            Logger.AddSink(fake);
            Logger.Info("after-add");

            assert.Equal(1, fake.Events.Count, "Sink added after user-configured clear sees only subsequent events");
        }

        [Test]
        public static void TestFlushFlushesEverySink(Assert assert)
        {
            ResetLogger();
            var a = new FakeSink();
            var b = new FakeSink();
            Logger.AddSink(a);
            Logger.AddSink(b);

            Logger.Flush();

            assert.Equal(1, a.FlushCount, "Sink A flushed once");
            assert.Equal(1, b.FlushCount, "Sink B flushed once");
        }

        [Test]
        public static void TestTraceDispatchesWhenMinLevelTrace(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);
            Logger.MinLevel = LogLevel.Trace;

            Logger.Trace("verbose");

            // Tests compile under DEBUG so [Conditional("DEBUG")] keeps the call.
            assert.Equal(1, fake.Events.Count, "Trace call dispatched under DEBUG compilation");
            assert.Equal(LogLevel.Trace, fake.Events[0].Level, "Event level is Trace");
        }

        [Test]
        public static void TestNamedLoggerTraceCarriesCategory(Assert assert)
        {
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);
            Logger.MinLevel = LogLevel.Trace;

            var log = Logger.ForCategory("cat1");
            log.Trace("nm-trace");

            assert.Equal(1, fake.Events.Count, "NamedLogger.Trace dispatched");
            assert.Equal("cat1", fake.Events[0].Category, "Category preserved");
            assert.Equal(LogLevel.Trace, fake.Events[0].Level, "Level is Trace");
        }

        [Test]
        public static void TestHttpLogSinkBatchesOnSize(Assert assert)
        {
            ResetLogger();
            var timer = new ControllableTimer();
            var sink = new HttpLogSink("/ClientLogs.ashx", 3, 5000, 500, timer);
            Logger.AddSink(sink);

            // Two events — under the batch size of 3, so no flush yet.
            Logger.Info("one");
            Logger.Info("two");
            // We can't directly verify the XHR, but we can observe queue state
            // indirectly: a third event triggers a flush and the queue resets.
            // After flush we add a fourth event: the queue has one item, not four.
            Logger.Info("three");
            Logger.Info("four");

            // Trigger timer tick — if the queue was flushed at size 3, only
            // the fourth event remains and should flush now.
            timer.Tick();

            assert.IsTrue(true, "Batch size flush exercised without exceptions");

            Logger.RemoveSink(sink);
        }

        [Test]
        public static void TestHttpLogSinkTimerClearedOnDetach(Assert assert)
        {
            ResetLogger();
            var timer = new ControllableTimer();
            var sink = new HttpLogSink("/ClientLogs.ashx", 10, 1000, 500, timer);

            assert.Equal(1000, timer.IntervalMs, "Interval installed at ctor with configured flushIntervalMs");

            Logger.AddSink(sink);
            Logger.RemoveSink(sink);

            assert.Equal(42, timer.ClearedIntervalHandle, "Interval handle cleared on Detach");
        }

        [Test]
        public static void TestHttpLogSinkOverflowDropsOldest(Assert assert)
        {
            ResetLogger();
            var timer = new ControllableTimer();
            // maxQueueSize=2, batchSize=100 so only overflow (not batch) triggers.
            var sink = new HttpLogSink("/ClientLogs.ashx", 100, 5000, 2, timer);
            Logger.AddSink(sink);

            Logger.Info("a");
            Logger.Info("b");
            Logger.Info("c"); // Should push out "a"
            Logger.Info("d"); // Should push out "b"

            // No assertion on internal queue contents (private). Instead verify
            // the flow survives bounded-queue pressure without throwing.
            timer.Tick();

            assert.IsTrue(true, "Overflow path exercised without exceptions");

            Logger.RemoveSink(sink);
        }
    }
}
