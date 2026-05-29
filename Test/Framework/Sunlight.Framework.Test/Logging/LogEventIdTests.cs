//-----------------------------------------------------------------------
// <copyright file="LogEventIdTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Logging
{
    using System.Collections.Generic;
    using SunlightUnit;
    using Sunlight.Framework;

    /// <summary>
    /// Tests for the per-emit <see cref="LogEvent.Id"/> contract introduced for
    /// the WebSocket ACK / server-side de-dup path. The id MUST be:
    /// (a) populated for every dispatched event, (b) per-emit unique (two
    /// emits from the same call site produce distinct ids), and (c) serialized
    /// by <see cref="LogJsonBuilder"/> in both single-event and envelope shapes.
    /// </summary>
    [TestFixture]
    public class LogEventIdTests
    {
        /// <summary>
        /// In-memory capture sink. Mirrors the FakeSink pattern from
        /// LoggerTests so the assembly stays self-contained.
        /// </summary>
        private class CaptureSink : ILogSink
        {
            public readonly List<LogEvent> Events = new List<LogEvent>();

            public void Handle(LogEvent evt) { this.Events.Add(evt); }

            public void Flush() { }

            public void Detach() { }
        }

        private static void ResetLogger()
        {
            Logger.ClearSinks();
            Logger.MinLevel = LogLevel.Trace;
        }

        [Test]
        public static void TestLoggerInfoEmitsNonEmptyId(Assert assert)
        {
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            Logger.Info("info-with-id");

            assert.Equal(1, sink.Events.Count, "One event dispatched");
            assert.IsTrue(sink.Events[0].Id != null && sink.Events[0].Id.Length > 0,
                "Info event carries a non-empty id");
        }

        [Test]
        public static void TestLoggerWarnEmitsNonEmptyId(Assert assert)
        {
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            Logger.Warn("warn-with-id");

            assert.Equal(1, sink.Events.Count, "One event dispatched");
            assert.IsTrue(sink.Events[0].Id != null && sink.Events[0].Id.Length > 0,
                "Warn event carries a non-empty id");
        }

        [Test]
        public static void TestLoggerErrorEmitsNonEmptyId(Assert assert)
        {
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            Logger.Error("error-with-id");

            assert.Equal(1, sink.Events.Count, "One event dispatched");
            assert.IsTrue(sink.Events[0].Id != null && sink.Events[0].Id.Length > 0,
                "Error event carries a non-empty id");
        }

        [Test]
        public static void TestLoggerDebugEmitsNonEmptyId(Assert assert)
        {
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            Logger.Debug("debug-with-id");

            assert.Equal(1, sink.Events.Count, "One event dispatched");
            assert.IsTrue(sink.Events[0].Id != null && sink.Events[0].Id.Length > 0,
                "Debug event carries a non-empty id");
        }

        [Test]
        public static void TestLoggerTraceEmitsNonEmptyId(Assert assert)
        {
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            Logger.Trace("trace-with-id");

            assert.Equal(1, sink.Events.Count, "One event dispatched");
            assert.IsTrue(sink.Events[0].Id != null && sink.Events[0].Id.Length > 0,
                "Trace event carries a non-empty id");
        }

        [Test]
        public static void TestSameCallSiteTwoEmitsDistinctIds(Assert assert)
        {
            // Per-emit semantics: the same call site emitting twice MUST
            // produce two distinct ids. This is what makes per-event WS ACK
            // targeting safe — a source-stable id would collide across
            // concurrent emits.
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            Logger.Info("same-call-site");
            Logger.Info("same-call-site");

            assert.Equal(2, sink.Events.Count, "Two events dispatched");
            assert.IsTrue(sink.Events[0].Id != sink.Events[1].Id,
                "Two emits from the same call site produce distinct ids");
        }

        [Test]
        public static void TestNamedLoggerInfoEmitsId(Assert assert)
        {
            // The category path through NamedLogger flows through the same
            // DispatchInternal — the id must be present there too.
            ResetLogger();
            var sink = new CaptureSink();
            Logger.AddSink(sink);

            var log = Logger.ForCategory("named-id-cat");
            log.Info("named-info");

            assert.Equal(1, sink.Events.Count, "One event dispatched via NamedLogger");
            assert.IsTrue(sink.Events[0].Id != null && sink.Events[0].Id.Length > 0,
                "NamedLogger.Info event carries a non-empty id");
        }

        [Test]
        public static void TestBuildEventEmitsIdAsFirstField(Assert assert)
        {
            // The id MUST be the first field in every event object so the
            // server-side WebSocket ACK path can streaming-parse just the id
            // from the front without building the full object.
            var evt = new LogEvent(
                "abcdef0123456789",
                "2026-04-17T00:00:00.000Z",
                LogLevel.Info,
                string.Empty,
                "m",
                null,
                null);

            string json = LogJsonBuilder.BuildEvent(evt);

            assert.IsTrue(json.IndexOf("{\"id\":\"abcdef0123456789\"") == 0,
                "id is the first field in the single-event JSON shape");
            assert.IsTrue(json.IndexOf("\"id\":") >= 0, "id field is present");
            // Confirm exactly one "id":, no duplicates inside the event.
            int first = json.IndexOf("\"id\":");
            int next = json.IndexOf("\"id\":", first + 1);
            assert.Equal(-1, next, "Only one id field emitted per event");
        }

        [Test]
        public static void TestBuildEnvelopeEmitsIdPerEvent(Assert assert)
        {
            // Envelope shape: {"events":[{...},{...}],"dropped":N}. Each
            // event object inside the array must carry its own id field.
            var events = new List<LogEvent>();
            events.Add(new LogEvent("id-aaaa00000001", "ts", LogLevel.Info, string.Empty, "a", null, null));
            events.Add(new LogEvent("id-bbbb00000002", "ts", LogLevel.Info, string.Empty, "b", null, null));

            string json = LogJsonBuilder.BuildEnvelope(events, 0);

            assert.IsTrue(json.IndexOf("\"id\":\"id-aaaa00000001\"") >= 0,
                "First envelope event includes its id");
            assert.IsTrue(json.IndexOf("\"id\":\"id-bbbb00000002\"") >= 0,
                "Second envelope event includes its id");
        }
    }
}
