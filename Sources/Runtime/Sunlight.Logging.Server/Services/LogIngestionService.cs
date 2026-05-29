// -----------------------------------------------------------------------
// <copyright file="LogIngestionService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Sunlight.Logging.Server.Models;

    /// <summary>
    /// Concrete <see cref="ILogIngestionService"/>. Maps each browser event
    /// to an <see cref="ILogger"/> emission with structured scope state,
    /// dedups by id, and isolates per-event failures so a single poison
    /// event cannot fail the whole envelope.
    /// </summary>
    public sealed class LogIngestionService : ILogIngestionService
    {
        /// <summary>
        /// Re-entrancy guard. If an <see cref="ILoggerProvider"/> registered
        /// downstream itself ends up calling back into this service on the
        /// same thread (e.g. via a misconfigured custom sink that emits
        /// browser-shaped events), we short-circuit instead of recursing
        /// infinitely. Mirrors the client-side <c>Logger.dispatching</c>
        /// flag.
        /// </summary>
        [ThreadStatic]
        private static bool _ingesting;

        private readonly ILoggerFactory _loggerFactory;
        private readonly LogIngestionOptions _options;

        // Category-specific ILogger cache. The factory itself caches but
        // we avoid the hash lookup per-event by pinning here.
        private readonly ConcurrentDictionary<string, ILogger> _loggerCache = new ConcurrentDictionary<string, ILogger>();

        // Dedup LRU. Maps id → tick the id was inserted. We bound by
        // _options.DedupCapacity (default 1024) and evict oldest on insert.
        // Lock object guards _dedupOrder + _dedupSet; expected to be
        // contended only mildly because envelopes are batched.
        private readonly LinkedList<string> _dedupOrder = new LinkedList<string>();
        private readonly Dictionary<string, LinkedListNode<string>> _dedupSet = new Dictionary<string, LinkedListNode<string>>();
        private readonly object _dedupLock = new object();

        // Overflow logger pre-resolved once. Used for the single Warning
        // emitted when an envelope's Dropped > 0.
        private readonly ILogger _overflowLogger;

        // Self-logger for service-level diagnostics (re-entrancy hits,
        // unexpected envelope shapes).
        private readonly ILogger<LogIngestionService> _selfLogger;

        public LogIngestionService(
            ILoggerFactory loggerFactory,
            IOptions<LogIngestionOptions> options)
        {
            this._loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value
                ?? new LogIngestionOptions();
            this._overflowLogger = loggerFactory.CreateLogger("Sunlight.Browser.Overflow");
            this._selfLogger = loggerFactory.CreateLogger<LogIngestionService>();
        }

        /// <inheritdoc />
        public ValueTask<IReadOnlyList<string>> IngestAsync(LogEnvelopeDto envelope, CancellationToken ct)
        {
            // Re-entrancy short circuit. Returning an empty ack list is the
            // safest behaviour: the caller will treat the events as not-acked
            // and retransmit later (when we're presumably out of the nested
            // call), which keeps us idempotent.
            if (_ingesting)
            {
                this._selfLogger.LogWarning(
                    "Re-entrant LogIngestionService.IngestAsync on the same thread skipped to avoid infinite recursion");
                return new ValueTask<IReadOnlyList<string>>((IReadOnlyList<string>)Array.Empty<string>());
            }

            if (envelope == null)
            {
                return new ValueTask<IReadOnlyList<string>>((IReadOnlyList<string>)Array.Empty<string>());
            }

            _ingesting = true;
            try
            {
                var acked = new List<string>(envelope.Events?.Count ?? 0);
                if (envelope.Events != null)
                {
                    for (int i = 0; i < envelope.Events.Count; i++)
                    {
                        var evt = envelope.Events[i];
                        if (evt == null) { continue; }

                        // Per-event try/catch so a malformed event cannot
                        // poison the entire batch — the client would
                        // otherwise retransmit forever.
                        try
                        {
                            if (this.TryIngestOne(evt))
                            {
                                if (!string.IsNullOrEmpty(evt.Id))
                                {
                                    acked.Add(evt.Id!);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this._selfLogger.LogError(
                                ex,
                                "Failed to ingest one Sunlight browser event id={EventId} cat={Category}",
                                evt.Id,
                                evt.Cat);
                        }
                    }
                }

                if (envelope.Dropped > 0)
                {
                    // Surface client-side overflow as a single Warning so
                    // queries can find queue saturation without scanning
                    // every event. Category is independent so it never
                    // collides with a real browser category.
                    this._overflowLogger.LogWarning(
                        "Sunlight browser sink dropped {DroppedCount} events due to queue overflow",
                        envelope.Dropped);
                }

                return new ValueTask<IReadOnlyList<string>>((IReadOnlyList<string>)acked);
            }
            finally
            {
                _ingesting = false;
            }
        }

        /// <summary>
        /// Map one event to a single <see cref="ILogger.Log"/> call.
        /// Returns true if the event was actually emitted, false if it was
        /// skipped by the dedup LRU. Per-event exceptions bubble up to the
        /// per-event try/catch in <see cref="IngestAsync"/>.
        /// </summary>
        private bool TryIngestOne(LogEventDto evt)
        {
            if (!string.IsNullOrEmpty(evt.Id) && this.IsDuplicate(evt.Id!))
            {
                // Idempotent retry: do NOT ack the duplicate. Returning
                // false means the id stays out of the ack list, which is
                // what the client expects (the original ack already shipped).
                return false;
            }

            var category = "Sunlight.Browser." + (string.IsNullOrEmpty(evt.Cat) ? "default" : evt.Cat);
            var logger = this._loggerCache.GetOrAdd(category, key => this._loggerFactory.CreateLogger(key));

            var level = MapLevel(evt.Level);
            if (!logger.IsEnabled(level))
            {
                // Level filter: still ACK so the client stops retransmitting
                // — we successfully "delivered" the event by deciding to
                // discard it per configuration.
                return true;
            }

            using (logger.BeginScope(BuildScope(evt)))
            {
                // Single canonical structured property: {Message}. Keeps
                // the destructuring simple downstream (Serilog/etc).
                logger.Log(level, "{Message}", evt.Msg ?? string.Empty);
            }
            return true;
        }

        /// <summary>
        /// Build the MEL scope dictionary from an event's correlation +
        /// props. Returns a Dictionary instead of an anonymous object so
        /// the keys are not minified and stay queryable by name in
        /// Serilog/etc.
        /// </summary>
        private static IReadOnlyDictionary<string, object?> BuildScope(LogEventDto evt)
        {
            var scope = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(evt.Id)) { scope["EventId"] = evt.Id; }
            if (!string.IsNullOrEmpty(evt.Ts)) { scope["Timestamp"] = evt.Ts; }
            if (!string.IsNullOrEmpty(evt.TraceId)) { scope["TraceId"] = evt.TraceId; }
            if (!string.IsNullOrEmpty(evt.SpanId)) { scope["SpanId"] = evt.SpanId; }
            if (!string.IsNullOrEmpty(evt.ParentSpanId)) { scope["ParentSpanId"] = evt.ParentSpanId; }
            if (evt.ActionId.HasValue) { scope["ActionId"] = evt.ActionId.Value; }
            if (evt.Depth.HasValue) { scope["Depth"] = evt.Depth.Value; }

            if (evt.Props != null)
            {
                foreach (var pair in evt.Props)
                {
                    if (string.IsNullOrEmpty(pair.Key)) { continue; }
                    // Don't shadow correlation fields by accident — props
                    // win unless the key collides with a reserved name.
                    if (!scope.ContainsKey(pair.Key))
                    {
                        scope[pair.Key] = pair.Value;
                    }
                }
            }
            return scope;
        }

        private static LogLevel MapLevel(string? level)
        {
            if (string.IsNullOrEmpty(level)) { return LogLevel.Information; }
            // Client emits all-caps strings via LogJsonBuilder.LevelToString.
            switch (level)
            {
                case "TRACE": return LogLevel.Trace;
                case "DEBUG": return LogLevel.Debug;
                case "INFO": return LogLevel.Information;
                case "WARN": return LogLevel.Warning;
                case "ERROR": return LogLevel.Error;
                default: return LogLevel.Information;
            }
        }

        /// <summary>
        /// True if <paramref name="id"/> was seen recently. Side effect:
        /// inserts the id and evicts the oldest entry if capacity is hit.
        /// </summary>
        private bool IsDuplicate(string id)
        {
            lock (this._dedupLock)
            {
                if (this._dedupSet.TryGetValue(id, out var existing))
                {
                    // Move to front (LRU touch) so a hot id stays in the cache.
                    this._dedupOrder.Remove(existing);
                    this._dedupOrder.AddFirst(existing);
                    return true;
                }

                var node = this._dedupOrder.AddFirst(id);
                this._dedupSet[id] = node;

                while (this._dedupOrder.Count > this._options.DedupCapacity)
                {
                    var oldest = this._dedupOrder.Last;
                    if (oldest == null) { break; }
                    this._dedupOrder.RemoveLast();
                    this._dedupSet.Remove(oldest.Value);
                }
                return false;
            }
        }
    }
}
