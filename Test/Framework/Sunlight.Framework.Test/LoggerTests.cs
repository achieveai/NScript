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
            var payloads = new List<string>();
            // Inject a transport override via the internal test-only constructor
            // so we can observe the serialized envelopes and verify batch-size
            // triggering behaviorally, not just "did not throw".
            var sink = new HttpLogSink(
                "/ClientLogs.ashx", 3, 5000, 500, timer,
                (endpoint, payload) => payloads.Add(payload));
            Logger.AddSink(sink);

            // Two events — under the batch size of 3, so no flush yet.
            Logger.Info("one");
            Logger.Info("two");
            assert.Equal(0, payloads.Count, "No flush before reaching batchSize");

            // Third event crosses the batchSize threshold and triggers flush.
            Logger.Info("three");
            assert.Equal(1, payloads.Count, "Batch flushed when queue reaches batchSize");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"one\"") >= 0, "First batch includes 'one'");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"three\"") >= 0, "First batch includes 'three'");

            // Queue is drained; the next event should accumulate, not flush.
            Logger.Info("four");
            assert.Equal(1, payloads.Count, "Queue reset after flush — no additional flush yet");

            // Timer tick drains the residual event.
            timer.Tick();
            assert.Equal(2, payloads.Count, "Timer tick flushes the residual event");
            assert.IsTrue(payloads[1].IndexOf("\"msg\":\"four\"") >= 0, "Second batch contains 'four'");

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
            var payloads = new List<string>();
            // maxQueueSize=2, batchSize=100 so only overflow (not batch) triggers.
            var sink = new HttpLogSink(
                "/ClientLogs.ashx", 100, 5000, 2, timer,
                (endpoint, payload) => payloads.Add(payload));
            Logger.AddSink(sink);

            Logger.Info("a");
            Logger.Info("b");
            Logger.Info("c"); // Pushes out "a"
            Logger.Info("d"); // Pushes out "b"

            // Timer tick forces the current queue to be serialized so we can
            // inspect the envelope's dropped count and retained messages.
            timer.Tick();

            assert.Equal(1, payloads.Count, "One batch produced on timer flush after overflow");
            assert.IsTrue(payloads[0].IndexOf("\"dropped\":2") >= 0,
                "Envelope reports the two overflow drops");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"c\"") >= 0, "Retained 'c'");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"d\"") >= 0, "Retained 'd'");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"a\"") < 0, "Dropped 'a'");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"b\"") < 0, "Dropped 'b'");

            Logger.RemoveSink(sink);
        }

        [Test]
        public static void TestReentrancyGuardDropsNestedDispatch(Assert assert)
        {
            // Anchors the Logger.dispatching guard against accidental removal:
            // a sink that logs from its own Handle() must not recursively
            // fan-out the nested event to any sink (including itself).
            ResetLogger();
            var reentrant = new ReentrantSink();
            var observer = new FakeSink();
            Logger.AddSink(reentrant);
            Logger.AddSink(observer);

            Logger.Info("outer");

            assert.Equal(1, reentrant.HandleCount,
                "Re-entrant sink receives only the outer event; nested call is dropped");
            assert.Equal(1, observer.Events.Count,
                "Observer sink receives only the outer event");
        }

        /// <summary>
        /// Sink whose Handle() re-enters <see cref="Logger"/>. Verifies the
        /// dispatching guard short-circuits the nested call rather than
        /// fanning it out (which would recurse indefinitely or duplicate
        /// events).
        /// </summary>
        private class ReentrantSink : ILogSink
        {
            public int HandleCount;

            public void Handle(LogEvent evt)
            {
                this.HandleCount++;
                // Recursive call — guarded by Logger.dispatching.
                Logger.Info("nested-from-sink");
            }

            public void Flush() { }

            public void Detach() { }
        }

        // -----------------------------------------------------------------
        // LogJsonBuilder direct tests
        //
        // LogJsonBuilder is internal; the Sunlight.Framework assembly grants
        // InternalsVisibleTo this test assembly so the JSON wire format can
        // be locked down with behavioral assertions. Field names ('ts',
        // 'level', 'msg', 'cat', 'props', 'dropped') are contractual with
        // the ingestion server and must not be changed by minification.
        // -----------------------------------------------------------------

        [Test]
        public static void TestLogJsonBuilderEventBasicShape(Assert assert)
        {
            var evt = new LogEvent(
                "2026-04-17T00:00:00.000Z",
                LogLevel.Info,
                string.Empty,
                "hello",
                null,
                null);

            string json = LogJsonBuilder.BuildEvent(evt);

            assert.IsTrue(json.IndexOf("\"ts\":\"2026-04-17T00:00:00.000Z\"") >= 0, "Has ts");
            assert.IsTrue(json.IndexOf("\"level\":\"INFO\"") >= 0, "Level serialized as INFO");
            assert.IsTrue(json.IndexOf("\"msg\":\"hello\"") >= 0, "Has msg");
            assert.IsTrue(json.IndexOf("\"cat\":") < 0, "Empty category is omitted");
            assert.IsTrue(json.IndexOf("\"props\":") < 0, "No props block when null");
        }

        [Test]
        public static void TestLogJsonBuilderLevelToString(Assert assert)
        {
            var trace = LogJsonBuilder.BuildEvent(
                new LogEvent("ts", LogLevel.Trace, string.Empty, "m", null, null));
            var debug = LogJsonBuilder.BuildEvent(
                new LogEvent("ts", LogLevel.Debug, string.Empty, "m", null, null));
            var warn = LogJsonBuilder.BuildEvent(
                new LogEvent("ts", LogLevel.Warn, string.Empty, "m", null, null));
            var error = LogJsonBuilder.BuildEvent(
                new LogEvent("ts", LogLevel.Error, string.Empty, "m", null, null));

            assert.IsTrue(trace.IndexOf("\"level\":\"TRACE\"") >= 0, "Trace → TRACE");
            assert.IsTrue(debug.IndexOf("\"level\":\"DEBUG\"") >= 0, "Debug → DEBUG");
            assert.IsTrue(warn.IndexOf("\"level\":\"WARN\"") >= 0, "Warn → WARN");
            assert.IsTrue(error.IndexOf("\"level\":\"ERROR\"") >= 0, "Error → ERROR");
        }

        [Test]
        public static void TestLogJsonBuilderPropertiesFlattening(Assert assert)
        {
            var evt = new LogEvent(
                "ts",
                LogLevel.Info,
                "c1",
                "m",
                new string[] { "k1", "v1", "k2", "v2" },
                null);

            string json = LogJsonBuilder.BuildEvent(evt);

            // Flat [k,v,k,v] becomes a nested object under "props".
            assert.IsTrue(json.IndexOf("\"props\":{\"k1\":\"v1\",\"k2\":\"v2\"}") >= 0,
                "Flat properties array pairs into an object");
            assert.IsTrue(json.IndexOf("\"cat\":\"c1\"") >= 0, "Category present");
        }

        [Test]
        public static void TestLogJsonBuilderOddPropertiesDropsTrailing(Assert assert)
        {
            // An odd-length properties array is a caller bug; trailing key is
            // dropped silently rather than emitting a malformed key:value pair.
            var evt = new LogEvent(
                "ts",
                LogLevel.Info,
                string.Empty,
                "m",
                new string[] { "k1", "v1", "orphan" },
                null);

            string json = LogJsonBuilder.BuildEvent(evt);

            assert.IsTrue(json.IndexOf("\"k1\":\"v1\"") >= 0, "Even pair kept");
            assert.IsTrue(json.IndexOf("\"orphan\"") < 0, "Trailing key dropped");
        }

        [Test]
        public static void TestLogJsonBuilderEnvelopeIncludesDropped(Assert assert)
        {
            var events = new List<LogEvent>();
            events.Add(new LogEvent("ts", LogLevel.Info, string.Empty, "a", null, null));
            events.Add(new LogEvent("ts", LogLevel.Info, string.Empty, "b", null, null));

            string json = LogJsonBuilder.BuildEnvelope(events, 7);

            assert.IsTrue(json.IndexOf("\"events\":[") >= 0, "Has events array");
            assert.IsTrue(json.IndexOf("\"dropped\":7") >= 0, "Dropped count in envelope");
            assert.IsTrue(json.IndexOf("\"msg\":\"a\"") >= 0, "First event present");
            assert.IsTrue(json.IndexOf("\"msg\":\"b\"") >= 0, "Second event present");
        }

        [Test]
        public static void TestLogJsonBuilderNullKeySkipped(Assert assert)
        {
            // A null slot in the properties array (e.g. from a caller that
            // forgot to initialize one entry) must NOT emit "null":"value" —
            // that would produce an unintentional string-literal key. The
            // pair is silently skipped instead.
            var evt = new LogEvent(
                "ts",
                LogLevel.Info,
                string.Empty,
                "m",
                new string[] { "k1", "v1", null, "orphan-val", "k2", "v2" },
                null);

            string json = LogJsonBuilder.BuildEvent(evt);

            assert.IsTrue(json.IndexOf("\"k1\":\"v1\"") >= 0, "First pair kept");
            assert.IsTrue(json.IndexOf("\"k2\":\"v2\"") >= 0, "Third pair kept");
            assert.IsTrue(json.IndexOf("\"null\"") < 0, "Null key is not serialized as the literal \"null\"");
            assert.IsTrue(json.IndexOf("orphan-val") < 0, "Value paired with null key is dropped");
        }

        [Test]
        public static void TestHttpLogSinkDetachFlushesQueuedEvents(Assert assert)
        {
            // Detach should ship any residual queued events via the beacon
            // path before going quiet — otherwise the last events before a
            // sink removal or page unload would be lost silently.
            ResetLogger();
            var timer = new ControllableTimer();
            var payloads = new List<string>();
            var sink = new HttpLogSink(
                "/ClientLogs.ashx", 100, 5000, 500, timer,
                (endpoint, payload) => payloads.Add(payload));
            Logger.AddSink(sink);

            Logger.Info("queued-1");
            Logger.Info("queued-2");
            assert.Equal(0, payloads.Count, "No flush yet — below batchSize");

            Logger.RemoveSink(sink);

            assert.Equal(1, payloads.Count, "Detach drained the queue through the transport");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"queued-1\"") >= 0, "Residual event 'queued-1' included");
            assert.IsTrue(payloads[0].IndexOf("\"msg\":\"queued-2\"") >= 0, "Residual event 'queued-2' included");
        }

        [Test]
        public static void TestNamedLoggerPassesPropertiesThrough(Assert assert)
        {
            // NamedLogger.Info(msg, props) must hand the properties array
            // straight to DispatchInternal without wrapping/copying. The
            // test locks this in so a future refactor cannot silently drop
            // the properties argument on the category path.
            ResetLogger();
            var fake = new FakeSink();
            Logger.AddSink(fake);

            var props = new string[] { "userId", "42", "op", "save" };
            var log = Logger.ForCategory("named-props");
            log.Info("named-with-props", props);

            assert.Equal(1, fake.Events.Count, "One event dispatched through NamedLogger");
            assert.StrictEqual(props, fake.Events[0].Properties, "Properties array forwarded unchanged");
            assert.Equal("named-props", fake.Events[0].Category, "Category preserved");
        }

        [Test]
        public static void TestForCategoryNullAndEmptyShareCacheSlot(Assert assert)
        {
            // ForCategory(null) normalizes to the empty string and then looks
            // up the same cache bucket as ForCategory(""). Without this
            // guarantee, a caller that drifts between null and "" would get
            // two distinct NamedLogger instances silently.
            ResetLogger();
            var nullLog = Logger.ForCategory(null);
            var emptyLog = Logger.ForCategory(string.Empty);

            assert.StrictEqual(emptyLog, nullLog,
                "Null and empty string resolve to the same cached NamedLogger");
        }

        [Test]
        public static void TestLogJsonBuilderCallContextCorrelation(Assert assert)
        {
            var ctx = CallContext.StartRoot();
            var evt = new LogEvent(
                "ts",
                LogLevel.Info,
                string.Empty,
                "m",
                null,
                ctx);

            string json = LogJsonBuilder.BuildEvent(evt);

            assert.IsTrue(json.IndexOf("\"actionId\":") >= 0, "actionId present");
            assert.IsTrue(json.IndexOf("\"traceId\":") >= 0, "traceId present");
            assert.IsTrue(json.IndexOf("\"spanId\":") >= 0, "spanId present");
            assert.IsTrue(json.IndexOf("\"depth\":0") >= 0, "Root depth is 0");
            // Root context has no ParentSpanId, so the key must be absent.
            assert.IsTrue(json.IndexOf("\"parentSpanId\":") < 0,
                "parentSpanId omitted for root context");
        }
    }
}
