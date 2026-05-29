//-----------------------------------------------------------------------
// <copyright file="WebSocketLogSink.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Batching WebSocket transport sink. Symmetric to
    /// <see cref="HttpLogSink"/> in batchSize/flushIntervalMs/maxQueueSize
    /// semantics; differs by tracking per-event in-flight state keyed by
    /// <see cref="LogEvent.Id"/> so the server can ACK individual events
    /// and the sink can retransmit unacked ones until <c>maxRetry</c>.
    /// </summary>
    /// <remarks>
    /// Callback-based / BYOWS: the consumer owns the WebSocket and supplies
    /// <c>isConnected</c> + <c>sendPayload</c> closures. The consumer's
    /// onmessage handler decodes ACK frames and calls
    /// <see cref="HandleAck"/>; the consumer signals disconnection via
    /// <see cref="NotifyDisconnected"/> so this sink can surface pending
    /// + in-flight events for a higher-level failover sink to drain.
    /// </remarks>
    public class WebSocketLogSink : ILogSink
    {
        /// <summary>
        /// Per-event retransmit bookkeeping for the in-flight map.
        /// </summary>
        public class InFlightEntry
        {
            public LogEvent Event;
            public int SentAtMs;
            public int RetryCount;

            public InFlightEntry(LogEvent evt, int sentAtMs)
            {
                this.Event = evt;
                this.SentAtMs = sentAtMs;
                this.RetryCount = 0;
            }
        }

        private readonly Func<bool> isConnected;
        private readonly Action<string> sendPayload;
        private readonly int batchSize;
        private readonly int flushIntervalMs;
        private readonly int maxQueueSize;
        private readonly int ackTimeoutMs;
        private readonly int maxRetry;
        private readonly IWindowTimer timer;
        private readonly Func<int> nowMsOverride;

        private List<LogEvent> queue;
        private StringDictionary<InFlightEntry> inFlight;
        private int droppedCount;
        private int timerHandle;
        private bool flushing;
        private bool detached;

        /// <summary>
        /// Fires when an event has been retransmitted <c>maxRetry</c>
        /// times without ACK. A <see cref="FailoverLogSink"/> subscribes
        /// to route the give-up event onto the HTTP path.
        /// </summary>
        public event Action<LogEvent> RetryExhausted;

        /// <summary>
        /// Fires when <see cref="NotifyDisconnected"/> surfaces residual
        /// pending + in-flight events. Subscribers (typically a
        /// <see cref="FailoverLogSink"/>) drain those into a fallback
        /// transport.
        /// </summary>
        public event Action<List<LogEvent>> DisconnectedWithPending;

        public WebSocketLogSink(
            Func<bool> isConnected,
            Action<string> sendPayload,
            int batchSize,
            int flushIntervalMs,
            int maxQueueSize,
            int ackTimeoutMs,
            int maxRetry,
            IWindowTimer timer)
            : this(isConnected, sendPayload, batchSize, flushIntervalMs, maxQueueSize, ackTimeoutMs, maxRetry, timer, null)
        {
        }

        /// <summary>
        /// Test-only constructor: <paramref name="nowMsOverride"/> replaces
        /// the <c>Date.now()</c> clock so unit tests can deterministically
        /// drive retransmit timing. When null the production clock is used.
        /// </summary>
        internal WebSocketLogSink(
            Func<bool> isConnected,
            Action<string> sendPayload,
            int batchSize,
            int flushIntervalMs,
            int maxQueueSize,
            int ackTimeoutMs,
            int maxRetry,
            IWindowTimer timer,
            Func<int> nowMsOverride)
        {
            if (isConnected == null) { throw new ArgumentNullException("isConnected"); }
            if (sendPayload == null) { throw new ArgumentNullException("sendPayload"); }
            if (timer == null) { throw new ArgumentNullException("timer"); }
            // Same ctor guards as HttpLogSink — fail fast on pathological
            // values that would otherwise produce confusing runtime
            // behaviour.
            if (batchSize < 1) { throw new ArgumentException("batchSize must be >= 1"); }
            if (flushIntervalMs < 1) { throw new ArgumentException("flushIntervalMs must be >= 1"); }
            if (maxQueueSize < 1) { throw new ArgumentException("maxQueueSize must be >= 1"); }
            if (ackTimeoutMs < 1) { throw new ArgumentException("ackTimeoutMs must be >= 1"); }
            if (maxRetry < 0) { throw new ArgumentException("maxRetry must be >= 0"); }

            this.isConnected = isConnected;
            this.sendPayload = sendPayload;
            this.batchSize = batchSize;
            this.flushIntervalMs = flushIntervalMs;
            this.maxQueueSize = maxQueueSize;
            this.ackTimeoutMs = ackTimeoutMs;
            this.maxRetry = maxRetry;
            this.timer = timer;
            this.nowMsOverride = nowMsOverride;

            this.queue = new List<LogEvent>();
            this.inFlight = new StringDictionary<InFlightEntry>();
            this.droppedCount = 0;
            this.timerHandle = -1;
            this.flushing = false;
            this.detached = false;

            this.timerHandle = timer.SetInterval(this.OnTimerTick, flushIntervalMs);
        }

        public void Handle(LogEvent evt)
        {
            if (this.detached) { return; }

            // Overflow: drop oldest, count drops. Same rationale as
            // HttpLogSink — recent events are more diagnostically useful.
            if (this.queue.Count >= this.maxQueueSize)
            {
                int excess = this.queue.Count - this.maxQueueSize + 1;
                this.droppedCount += excess;
                var trimmed = new List<LogEvent>();
                for (int i = excess; i < this.queue.Count; i++)
                {
                    trimmed.Add(this.queue[i]);
                }
                this.queue = trimmed;
            }

            this.queue.Add(evt);

            if (this.queue.Count >= this.batchSize)
            {
                this.Flush();
            }
        }

        public void Flush()
        {
            // Re-entrancy guard — same rationale as HttpLogSink. A
            // downstream sink that triggers another log call during
            // Flush would otherwise double-send.
            if (this.flushing) { return; }
            if (this.queue.Count == 0) { return; }
            // No point flushing while disconnected — events stay queued
            // for the next reconnect tick. The DisconnectedWithPending
            // event drains the queue via the failover sink if needed.
            if (!this.isConnected()) { return; }

            this.flushing = true;
            try
            {
                var batch = this.queue;
                int dropped = this.droppedCount;
                string payload = LogJsonBuilder.BuildEnvelope(batch, dropped);
                this.queue = new List<LogEvent>();
                this.droppedCount = 0;

                int now = this.GetNowMs();
                for (int i = 0; i < batch.Count; i++)
                {
                    var evt = batch[i];
                    if (evt.Id != null && evt.Id.Length > 0)
                    {
                        this.inFlight[evt.Id] = new InFlightEntry(evt, now);
                    }
                }
                this.sendPayload(payload);
            }
            catch { /* fire-and-forget transport */ }
            finally
            {
                this.flushing = false;
            }
        }

        public void Detach()
        {
            if (this.detached) { return; }
            this.detached = true;

            if (this.timerHandle >= 0)
            {
                this.timer.ClearInterval(this.timerHandle);
                this.timerHandle = -1;
            }

            // Final-flush attempt. If the WS is up we'll ship the batch;
            // if not, the events stay in queue and will surface to a
            // failover sink via the next disconnect drain.
            if (this.queue.Count > 0 && this.isConnected())
            {
                try { this.Flush(); }
                catch { /* fire-and-forget */ }
            }
        }

        /// <summary>
        /// Consumer calls this when their onmessage handler decodes an
        /// ACK frame. Each acked id is removed from the in-flight map so
        /// it won't be retransmitted on the next timer tick.
        /// </summary>
        public void HandleAck(string[] ackIds)
        {
            if (ackIds == null) { return; }
            for (int i = 0; i < ackIds.Length; i++)
            {
                string id = ackIds[i];
                if (id != null && id.Length > 0)
                {
                    this.inFlight.Remove(id);
                }
            }
        }

        /// <summary>
        /// Consumer calls this when their WebSocket transitions away
        /// from OPEN. Surfaces all queued + in-flight events so a
        /// <see cref="FailoverLogSink"/> can drain them to HTTP.
        /// </summary>
        public void NotifyDisconnected()
        {
            var combined = new List<LogEvent>();
            for (int i = 0; i < this.queue.Count; i++)
            {
                combined.Add(this.queue[i]);
            }
            foreach (var pair in this.inFlight)
            {
                combined.Add(pair.Value.Event);
            }

            // Reset local state so a future reconnect starts clean.
            this.queue = new List<LogEvent>();
            this.inFlight = new StringDictionary<InFlightEntry>();

            if (this.DisconnectedWithPending != null && combined.Count > 0)
            {
                this.DisconnectedWithPending(combined);
            }
        }

        private void OnTimerTick()
        {
            if (this.detached) { return; }

            // Tick has two jobs: drive a periodic flush of the queue, and
            // re-send any inFlight whose ACK has timed out (incrementing
            // retryCount). Both guarded so the timer callback never
            // escapes a throw — same rationale as HttpLogSink.
            if (this.queue.Count > 0)
            {
                try { this.Flush(); }
                catch { /* timer callback must not throw */ }
            }

            try
            {
                this.RetransmitTimedOutInFlight();
            }
            catch { /* timer callback must not throw */ }
        }

        /// <summary>
        /// Walk the in-flight map; for any entry whose ACK has been
        /// outstanding past <c>ackTimeoutMs</c>, either re-send (if
        /// retry budget remains) or drop + raise
        /// <see cref="RetryExhausted"/>.
        /// </summary>
        private void RetransmitTimedOutInFlight()
        {
            if (this.inFlight.Count == 0) { return; }
            if (!this.isConnected()) { return; }

            int now = this.GetNowMs();

            // Two-pass to avoid mutating the dictionary while iterating
            // (NScript Dictionary enumeration is not modification-safe).
            var resendIds = new List<string>();
            var giveUpIds = new List<string>();
            foreach (var pair in this.inFlight)
            {
                var entry = pair.Value;
                if (now - entry.SentAtMs < this.ackTimeoutMs) { continue; }
                if (entry.RetryCount >= this.maxRetry)
                {
                    giveUpIds.Add(pair.Key);
                }
                else
                {
                    resendIds.Add(pair.Key);
                }
            }

            for (int i = 0; i < giveUpIds.Count; i++)
            {
                var entry = this.inFlight[giveUpIds[i]];
                this.inFlight.Remove(giveUpIds[i]);
                this.droppedCount++;
                if (this.RetryExhausted != null)
                {
                    this.RetryExhausted(entry.Event);
                }
            }

            for (int i = 0; i < resendIds.Count; i++)
            {
                var entry = this.inFlight[resendIds[i]];
                var single = new List<LogEvent>();
                single.Add(entry.Event);
                try
                {
                    string payload = LogJsonBuilder.BuildEnvelope(single, 0);
                    this.sendPayload(payload);
                    entry.SentAtMs = now;
                    entry.RetryCount++;
                }
                catch { /* fire-and-forget per-retry */ }
            }
        }

        /// <summary>
        /// Test-visible accessor — count of unacked events the sink is
        /// currently tracking. Internal to keep the public API tight.
        /// </summary>
        internal int InFlightCount { get { return this.inFlight.Count; } }

        /// <summary>
        /// Test-visible accessor — number of events queued but not yet
        /// flushed.
        /// </summary>
        internal int QueueCount { get { return this.queue.Count; } }

        /// <summary>
        /// Test-visible accessor — count of events dropped this session.
        /// </summary>
        internal int DroppedCount { get { return this.droppedCount; } }

        /// <summary>
        /// Wall-clock millisecond counter. Defers to <c>Date.now()</c>
        /// via a <c>[Script]</c> bridge — matches the timer's basis.
        /// Routed through <see cref="nowMsOverride"/> when the test-only
        /// constructor injected one so unit tests can drive deterministic
        /// timeout behaviour without sleeping.
        /// </summary>
        private int GetNowMs()
        {
            if (this.nowMsOverride != null) { return this.nowMsOverride(); }
            return WebSocketLogSink.NowMs();
        }

        [Script("return @:Date.now();")]
        private static extern int NowMs();
    }
}
