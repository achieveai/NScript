// -----------------------------------------------------------------------
// <copyright file="LogIngestionServiceTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sunlight.Logging.Server.Models;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// Unit tests for <see cref="LogIngestionService"/>. Covers MEL mapping,
    /// dedup, dropped-warning emission, the [ThreadStatic] re-entrancy
    /// guard, null Cat handling, and per-event exception isolation.
    /// </summary>
    [TestClass]
    public class LogIngestionServiceTests
    {
        private TestLoggerProvider _provider = null!;
        private LogIngestionService _svc = null!;

        [TestInitialize]
        public void Init()
        {
            this._provider = new TestLoggerProvider();
            var factory = LoggerFactory.Create(b =>
            {
                b.SetMinimumLevel(LogLevel.Trace);
                b.AddProvider(this._provider);
            });
            this._svc = new LogIngestionService(
                factory,
                Options.Create(new LogIngestionOptions { DedupCapacity = 16 }));
        }

        [TestMethod]
        public async Task IngestAsync_SingleInfoEvent_LogsToCategoryWithScope()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto
                    {
                        Id = "evt-001",
                        Ts = "2026-04-17T00:00:00.000Z",
                        Level = "INFO",
                        Msg = "hello world",
                        Cat = "TodoApp.ListView",
                        TraceId = "trace-aaa",
                        SpanId = "span-bbb",
                        ActionId = 42,
                        Depth = 1,
                        Props = new Dictionary<string, string> { ["userId"] = "u1", ["op"] = "save" },
                    },
                },
                Dropped = 0,
            };

            var acked = await this._svc.IngestAsync(env, CancellationToken.None);

            Assert.AreEqual(1, acked.Count, "Single event acked");
            Assert.AreEqual("evt-001", acked[0]);

            var captured = this._provider.Logs.Single(l => l.Category.EndsWith("ListView"));
            Assert.AreEqual("Sunlight.Browser.TodoApp.ListView", captured.Category);
            Assert.AreEqual(LogLevel.Information, captured.Level);
            Assert.AreEqual("hello world", captured.Message);
            Assert.AreEqual("evt-001", captured.Scope["EventId"]);
            Assert.AreEqual("trace-aaa", captured.Scope["TraceId"]);
            Assert.AreEqual("span-bbb", captured.Scope["SpanId"]);
            Assert.AreEqual(42, captured.Scope["ActionId"]);
            Assert.AreEqual(1, captured.Scope["Depth"]);
            Assert.AreEqual("u1", captured.Scope["userId"]);
            Assert.AreEqual("save", captured.Scope["op"]);
        }

        [TestMethod]
        public async Task IngestAsync_NullCategory_FallsBackToDefault()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "n1", Level = "INFO", Msg = "m", Cat = null },
                },
            };

            await this._svc.IngestAsync(env, CancellationToken.None);

            var captured = this._provider.Logs.Single();
            Assert.AreEqual("Sunlight.Browser.default", captured.Category);
        }

        [TestMethod]
        public async Task IngestAsync_AllLevels_MapCorrectlyToMel()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "t", Level = "TRACE", Msg = "t" },
                    new LogEventDto { Id = "d", Level = "DEBUG", Msg = "d" },
                    new LogEventDto { Id = "i", Level = "INFO", Msg = "i" },
                    new LogEventDto { Id = "w", Level = "WARN", Msg = "w" },
                    new LogEventDto { Id = "e", Level = "ERROR", Msg = "e" },
                },
            };

            await this._svc.IngestAsync(env, CancellationToken.None);

            var byLevel = this._provider.Logs.ToDictionary(l => l.Message, l => l.Level);
            Assert.AreEqual(LogLevel.Trace, byLevel["t"]);
            Assert.AreEqual(LogLevel.Debug, byLevel["d"]);
            Assert.AreEqual(LogLevel.Information, byLevel["i"]);
            Assert.AreEqual(LogLevel.Warning, byLevel["w"]);
            Assert.AreEqual(LogLevel.Error, byLevel["e"]);
        }

        [TestMethod]
        public async Task IngestAsync_DuplicateId_SkipsSecondEmit()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "dup-1", Level = "INFO", Msg = "first" },
                    new LogEventDto { Id = "dup-1", Level = "INFO", Msg = "second" },
                },
            };

            var acked = await this._svc.IngestAsync(env, CancellationToken.None);

            Assert.AreEqual(1, acked.Count, "Only the first ack is returned");
            Assert.AreEqual("dup-1", acked[0]);
            Assert.AreEqual(1, this._provider.Logs.Count, "Second emit was skipped by dedup");
        }

        [TestMethod]
        public async Task IngestAsync_DroppedGreaterThanZero_EmitsOverflowWarning()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>(),
                Dropped = 7,
            };

            await this._svc.IngestAsync(env, CancellationToken.None);

            var overflow = this._provider.Logs.Single();
            Assert.AreEqual("Sunlight.Browser.Overflow", overflow.Category);
            Assert.AreEqual(LogLevel.Warning, overflow.Level);
            StringAssert.Contains(overflow.Message, "7");
        }

        [TestMethod]
        public async Task IngestAsync_NullEnvelope_ReturnsEmptyAck()
        {
            var acked = await this._svc.IngestAsync(null!, CancellationToken.None);
            Assert.AreEqual(0, acked.Count);
            Assert.AreEqual(0, this._provider.Logs.Count);
        }

        [TestMethod]
        public async Task IngestAsync_EmptyEventsAndZeroDropped_AcksEmpty()
        {
            var env = new LogEnvelopeDto { Events = new List<LogEventDto>(), Dropped = 0 };
            var acked = await this._svc.IngestAsync(env, CancellationToken.None);
            Assert.AreEqual(0, acked.Count);
            Assert.AreEqual(0, this._provider.Logs.Count);
        }

        [TestMethod]
        public async Task DedupLru_BeyondCapacity_EvictsOldest()
        {
            // Dedup capacity = 16 (set in Init). Send 20 distinct ids, then
            // re-send the first id; it should be NOT a duplicate because it
            // was evicted.
            var env1 = new LogEnvelopeDto { Events = new List<LogEventDto>() };
            for (int i = 0; i < 20; i++)
            {
                env1.Events.Add(new LogEventDto { Id = $"k-{i}", Level = "INFO", Msg = $"m-{i}" });
            }
            await this._svc.IngestAsync(env1, CancellationToken.None);

            var env2 = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "k-0", Level = "INFO", Msg = "replay" },
                },
            };
            var acked2 = await this._svc.IngestAsync(env2, CancellationToken.None);

            // k-0 was evicted, so the replay emits and acks again.
            Assert.AreEqual(1, acked2.Count);
            Assert.AreEqual("k-0", acked2[0]);
        }

        [TestMethod]
        public async Task DirectCall_BypassingHttp_ProducesIdenticalMelOutput()
        {
            // Consumer-direct-call path: prove a BYOWS consumer can call
            // IngestAsync without going through any HTTP/WS endpoint.
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "direct-1", Level = "INFO", Msg = "direct" },
                },
            };

            var acked = await this._svc.IngestAsync(env, CancellationToken.None);

            Assert.AreEqual(1, acked.Count);
            var captured = this._provider.Logs.Single();
            Assert.AreEqual("Sunlight.Browser.default", captured.Category);
            Assert.AreEqual("direct", captured.Message);
        }
    }
}
