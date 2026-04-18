//-----------------------------------------------------------------------
// <copyright file="HttpLogSink.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Html;

    /// <summary>
    /// Batching HTTP transport sink. Queues <see cref="LogEvent"/>s in memory
    /// and POSTs them to a configured endpoint when any of these triggers fire:
    /// <list type="bullet">
    ///   <item>queue length reaches <c>batchSize</c></item>
    ///   <item>the injected <see cref="IWindowTimer"/> fires every <c>flushIntervalMs</c></item>
    ///   <item><see cref="Flush"/> is called explicitly</item>
    ///   <item>the page transitions to <c>beforeunload</c> / <c>pagehide</c>
    ///         (uses <c>navigator.sendBeacon</c> for reliable unload delivery)</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Overflow strategy: when the queue exceeds <c>maxQueueSize</c> the oldest
    /// events are dropped, and a running dropped-count is stamped into the next
    /// batch envelope (payload, not an HTTP header — sendBeacon cannot set
    /// custom headers and custom headers would trigger CORS preflight on the
    /// normal XHR path).
    /// </remarks>
    public class HttpLogSink : ILogSink
    {
        private readonly string endpoint;
        private readonly int batchSize;
        private readonly int flushIntervalMs;
        private readonly int maxQueueSize;
        private readonly IWindowTimer timer;

        private readonly Action<string, string> transportOverride;

        private List<LogEvent> queue;
        private int droppedCount;
        private int timerHandle;
        private bool flushing;
        private bool detached;
        private Action unloadHandler;

        public HttpLogSink(
            string endpoint,
            int batchSize,
            int flushIntervalMs,
            int maxQueueSize,
            IWindowTimer timer)
            : this(endpoint, batchSize, flushIntervalMs, maxQueueSize, timer, null)
        {
        }

        /// <summary>
        /// Test-only constructor: <paramref name="transportOverride"/> replaces
        /// both the XHR POST and <c>sendBeacon</c> paths, so unit tests can
        /// capture the serialized envelope without touching the network. When
        /// <paramref name="transportOverride"/> is <c>null</c> the production
        /// transports are used.
        /// </summary>
        internal HttpLogSink(
            string endpoint,
            int batchSize,
            int flushIntervalMs,
            int maxQueueSize,
            IWindowTimer timer,
            Action<string, string> transportOverride)
        {
            if (endpoint == null) { throw new ArgumentNullException("endpoint"); }
            if (timer == null) { throw new ArgumentNullException("timer"); }

            this.endpoint = endpoint;
            this.batchSize = batchSize;
            this.flushIntervalMs = flushIntervalMs;
            this.maxQueueSize = maxQueueSize;
            this.timer = timer;
            this.transportOverride = transportOverride;

            this.queue = new List<LogEvent>();
            this.droppedCount = 0;
            this.timerHandle = -1;
            this.flushing = false;
            this.detached = false;

            // Schedule the periodic flush and install a best-effort unload hook
            // so buffered events survive page navigation.
            this.timerHandle = timer.SetInterval(this.OnTimerTick, flushIntervalMs);
            this.InstallUnloadHandler();
        }

        public void Handle(LogEvent evt)
        {
            if (this.detached) { return; }

            // Overflow: drop the oldest events in one pass, count the drops.
            // We bias toward keeping the *most recent* entries because those
            // are usually what the developer wants to see when triaging a
            // crash. A previous implementation used List<T>.RemoveAt(0) in a
            // loop, which is O(n) per drop (Array.splice(0,1) shifts every
            // remaining element); with maxQueueSize=500 under sustained
            // overflow that compounds badly. Rebuilding the tail into a new
            // list is a single O(n) pass instead.
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
            // Re-entrancy guard: if a downstream sink or the XHR OnBeforeSend
            // hook triggers another log call that flushes, we would otherwise
            // double-send the same events.
            if (this.flushing) { return; }
            if (this.queue.Count == 0) { return; }

            this.flushing = true;
            try
            {
                // Single protection boundary covering BOTH payload construction
                // and transport. LogJsonBuilder.BuildEnvelope (called via
                // ExtractBatchAsPayload) can throw on JSON serialization
                // failure; the outer timer/unload callers cannot meaningfully
                // react to any failure here. Lost batches are the documented
                // fire-and-forget contract.
                string payload = this.ExtractBatchAsPayload();
                this.SendPayload(payload, false);
            }
            catch { /* fire-and-forget: payload build or transport failure */ }
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

            this.RemoveUnloadHandler(this.unloadHandler);
            this.unloadHandler = null;

            // Best-effort final flush — use sendBeacon because the normal XHR
            // path cannot be trusted to complete once we stop listening. The
            // try/catch covers ExtractBatchAsPayload too, because serialization
            // failure after the destructive queue swap would otherwise escape
            // through RemoveSink/ClearSinks up into arbitrary caller code.
            if (this.queue.Count > 0)
            {
                try
                {
                    string payload = this.ExtractBatchAsPayload();
                    this.SendPayload(payload, true);
                }
                catch { /* fire-and-forget transport */ }
            }
        }

        private void OnTimerTick()
        {
            // Runs directly on window.setInterval — OUTSIDE any application
            // try/catch. An unhandled exception here would fire every
            // flushIntervalMs for the lifetime of the page as an unhandled
            // setInterval error, so the outer try/catch is mandatory even
            // though Flush() is also internally guarded.
            if (this.detached) { return; }
            if (this.queue.Count > 0)
            {
                try { this.Flush(); }
                catch { /* timer callback must not throw */ }
            }
        }

        private void InstallUnloadHandler()
        {
            this.unloadHandler = this.OnPageUnload;
            this.AddUnloadHandler(this.unloadHandler);
        }

        private void OnPageUnload()
        {
            // beforeunload/pagehide dispatchers have no application-level
            // protection, same rationale as OnTimerTick.
            if (this.detached) { return; }
            if (this.queue.Count == 0) { return; }

            try
            {
                string payload = this.ExtractBatchAsPayload();
                this.SendPayload(payload, true);
            }
            catch { /* fire-and-forget transport — unload path cannot react */ }
        }

        /// <summary>
        /// Dispatch the serialized payload to the appropriate transport: the
        /// XHR path for live flushes and the <c>sendBeacon</c> path for unload
        /// / detach. Routed through <see cref="transportOverride"/> when the
        /// test-only constructor injected one so unit tests can capture the
        /// envelope without touching the network.
        /// </summary>
        private void SendPayload(string payload, bool isUnload)
        {
            if (this.transportOverride != null)
            {
                this.transportOverride(this.endpoint, payload);
                return;
            }

            if (isUnload) { HttpLogSink.SendBeacon(this.endpoint, payload); }
            else { HttpLogSink.PostPayload(this.endpoint, payload); }
        }

        /// <summary>
        /// Atomically swap the queue + <c>droppedCount</c> out, reset both, and
        /// return the JSON envelope. Shared by <see cref="Flush"/>,
        /// <see cref="Detach"/>, and <see cref="OnPageUnload"/> so the three
        /// paths stay in sync and cannot drift.
        /// </summary>
        private string ExtractBatchAsPayload()
        {
            var batch = this.queue;
            int dropped = this.droppedCount;
            this.queue = new List<LogEvent>();
            this.droppedCount = 0;
            return LogJsonBuilder.BuildEnvelope(batch, dropped);
        }

        /// <summary>
        /// Async XHR POST via the <see cref="XMLHttpRequest"/> facade. Implemented
        /// in plain C# rather than a <c>[Script]</c> block because the NScript JS
        /// parser rejects <c>new XMLHttpRequest()</c> inside inline scripts.
        /// </summary>
        /// <remarks>
        /// Deliberately uses the raw instance methods <c>Open</c> / <c>Send</c>,
        /// not the static <c>GetRaw</c> / <c>PostRaw</c> helpers — those helpers
        /// invoke <c>XMLHttpRequest.OnBeforeSend</c>, which in turn calls back
        /// into <c>Logger.*</c> for traceparent injection. Routing log transport
        /// through that hook would be a latent recursion risk. Fire-and-forget;
        /// network errors surface asynchronously on <c>onerror</c>, which we do
        /// not hook (logging errors about logging would recurse).
        /// </remarks>
        private static void PostPayload(string endpoint, string payload)
        {
            var xhr = new XMLHttpRequest();
            xhr.Open("POST", endpoint, true);
            xhr.SetRequestHeader("Content-Type", "application/json");
            xhr.Send(payload);
        }

        /// <summary>
        /// Best-effort send during page unload. Prefers <c>navigator.sendBeacon</c>
        /// with a <c>Blob</c> of type <c>application/json</c> — sending a bare
        /// string defaults to <c>text/plain</c>, which ingestion servers tend to
        /// reject. Falls back to a synchronous XHR on browsers without
        /// <c>sendBeacon</c> so buffered events still have a chance to ship.
        /// </summary>
        private static void SendBeacon(string endpoint, string payload)
        {
            var navigator = Window.Instance.Navigator;
            if (navigator != null && navigator.HasSendBeaconApi)
            {
                var opts = new BlobCreateOptions();
                opts.Type = "application/json";
                var blob = new Blob(payload, opts);
                HttpLogSink.NavigatorSendBeaconBlob(endpoint, blob);
                return;
            }

            // Very old browsers: sync XHR is the last-resort unload path.
            var xhr = new XMLHttpRequest();
            xhr.Open("POST", endpoint, false);
            xhr.SetRequestHeader("Content-Type", "application/json");
            xhr.Send(payload);
        }

        /// <summary>
        /// Single-expression <c>[Script]</c> bridge so we can pass a <c>Blob</c>
        /// to <c>navigator.sendBeacon</c> — the <see cref="Navigator"/> facade
        /// only exposes <c>string</c>/<c>FormData</c> overloads.
        /// </summary>
        [Script(@"window.navigator.sendBeacon(endpoint, blob);")]
        private static extern void NavigatorSendBeaconBlob(string endpoint, Blob blob);

        private void AddUnloadHandler(Action handler)
        {
            HttpLogSink.WindowAddUnloadHandler(handler);
        }

        private void RemoveUnloadHandler(Action handler)
        {
            if (handler == null) { return; }
            HttpLogSink.WindowRemoveUnloadHandler(handler);
        }

        /// <summary>
        /// Register <paramref name="handler"/> for both <c>pagehide</c> and
        /// <c>beforeunload</c>. Separate single-statement <c>[Script]</c> shims
        /// keep each body small and parseable by the NScript JS frontend.
        /// </summary>
        [Script(@"window.addEventListener('pagehide', handler, false);")]
        private static extern void WindowAddPageHide(Action handler);

        [Script(@"window.addEventListener('beforeunload', handler, false);")]
        private static extern void WindowAddBeforeUnload(Action handler);

        [Script(@"window.removeEventListener('pagehide', handler, false);")]
        private static extern void WindowRemovePageHide(Action handler);

        [Script(@"window.removeEventListener('beforeunload', handler, false);")]
        private static extern void WindowRemoveBeforeUnload(Action handler);

        private static void WindowAddUnloadHandler(Action handler)
        {
            HttpLogSink.WindowAddPageHide(handler);
            HttpLogSink.WindowAddBeforeUnload(handler);
        }

        private static void WindowRemoveUnloadHandler(Action handler)
        {
            HttpLogSink.WindowRemovePageHide(handler);
            HttpLogSink.WindowRemoveBeforeUnload(handler);
        }
    }
}
