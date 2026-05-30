//-----------------------------------------------------------------------
// <copyright file="FailoverLogSink.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Composite sink that prefers a <see cref="WebSocketLogSink"/> when
    /// its <c>isConnected</c> probe returns true, and falls back to a
    /// <see cref="HttpLogSink"/> otherwise. Drains pending + in-flight
    /// events from the WS sink to HTTP on disconnect, and reroutes any
    /// per-event give-ups (RetryExhausted) from WS to HTTP.
    /// </summary>
    /// <remarks>
    /// Logger.AddSink wraps the entire failover unit as one sink —
    /// downstream consumers see a single <see cref="ILogSink"/> contract
    /// and don't have to reason about which transport actually shipped
    /// any given event.
    /// </remarks>
    public class FailoverLogSink : ILogSink
    {
        private readonly WebSocketLogSink wsSink;
        private readonly HttpLogSink httpSink;
        private readonly Func<bool> isWsConnected;
        private readonly Action<List<LogEvent>> disconnectedHandler;
        private readonly Action<LogEvent> retryExhaustedHandler;
        private bool detached;

        public FailoverLogSink(
            WebSocketLogSink wsSink,
            HttpLogSink httpSink,
            Func<bool> isWsConnected)
        {
            if (wsSink == null) { throw new ArgumentNullException("wsSink"); }
            if (httpSink == null) { throw new ArgumentNullException("httpSink"); }
            if (isWsConnected == null) { throw new ArgumentNullException("isWsConnected"); }

            this.wsSink = wsSink;
            this.httpSink = httpSink;
            this.isWsConnected = isWsConnected;
            this.detached = false;

            // Both event subscriptions keep the WS sink's give-up paths
            // routed onto HTTP so no event is dropped just because WS
            // had a transient bad time. Stored delegates so Detach
            // can unsubscribe — lambdas (not method groups) because
            // NScript's C#-to-JS converter may not synthesize a stable
            // delegate for an instance method group on a class, same
            // precaution as LogJsonBuilder's ctx.AppendCorrelationJson
            // (sb, s => ...) call.
            this.disconnectedHandler = pending => this.OnWsDisconnectedWithPending(pending);
            this.retryExhaustedHandler = evt => this.OnWsRetryExhausted(evt);
            this.wsSink.DisconnectedWithPending += this.disconnectedHandler;
            this.wsSink.RetryExhausted += this.retryExhaustedHandler;
        }

        public void Handle(LogEvent evt)
        {
            if (this.detached) { return; }
            if (this.isWsConnected())
            {
                this.wsSink.Handle(evt);
            }
            else
            {
                this.httpSink.Handle(evt);
            }
        }

        public void Flush()
        {
            // Forward to both — each is idempotent w.r.t. empty queues.
            this.wsSink.Flush();
            this.httpSink.Flush();
        }

        public void Detach()
        {
            if (this.detached) { return; }
            this.detached = true;
            this.wsSink.DisconnectedWithPending -= this.disconnectedHandler;
            this.wsSink.RetryExhausted -= this.retryExhaustedHandler;
            this.wsSink.Detach();
            this.httpSink.Detach();
        }

        /// <summary>
        /// Drain the WS sink's residual queue + in-flight events into
        /// the HTTP path. Called when the consumer signals a WS
        /// disconnect via <see cref="WebSocketLogSink.NotifyDisconnected"/>.
        /// </summary>
        private void OnWsDisconnectedWithPending(List<LogEvent> pending)
        {
            if (pending == null) { return; }
            for (int i = 0; i < pending.Count; i++)
            {
                this.httpSink.Handle(pending[i]);
            }
        }

        /// <summary>
        /// Reroute a single event that exhausted WS retries onto HTTP.
        /// Keeps the failover semantics symmetric: the event is "in
        /// flight" on SOME transport at all times.
        /// </summary>
        private void OnWsRetryExhausted(LogEvent evt)
        {
            if (evt == null) { return; }
            this.httpSink.Handle(evt);
        }
    }
}
