//-----------------------------------------------------------------------
// <copyright file="LogSinkFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Bootstrap helper: looks at two well-known <c>window</c> globals
    /// set by the host page (<c>__nscriptLogEndpoint</c> and
    /// <c>__nscriptLogWsEndpoint</c>) and picks the appropriate sink
    /// shape:
    /// <list type="bullet">
    /// <item>Both set → <see cref="FailoverLogSink"/> (WS primary, HTTP fallback)</item>
    /// <item>WS only → <see cref="WebSocketLogSink"/> with built-in <see cref="WindowWebSocketTransport"/></item>
    /// <item>URL only → <see cref="HttpLogSink"/></item>
    /// <item>Neither → <see cref="ConsoleSink"/></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Bundle bootstrap recipe: call <see cref="Logger.ClearSinks"/>
    /// first, then <see cref="CreateFromBootstrap"/>, then
    /// <see cref="Logger.AddSink"/>. Doing this before any code that
    /// might log keeps the lazy <see cref="ConsoleSink"/> install path
    /// from racing in.
    /// </remarks>
    public static class LogSinkFactory
    {
        /// <summary> Default batch/queue parameters — matches HttpLogSink usage in shipping bundles. </summary>
        private const int DefaultBatchSize = 32;
        private const int DefaultFlushIntervalMs = 2000;
        private const int DefaultMaxQueueSize = 500;
        private const int DefaultAckTimeoutMs = 5000;
        private const int DefaultMaxRetry = 3;

        /// <summary>
        /// Build the appropriate sink for the current page's globals
        /// configuration. Returns a single <see cref="ILogSink"/> the
        /// caller can hand to <see cref="Logger.AddSink"/>.
        /// </summary>
        public static ILogSink CreateFromBootstrap(IWindowTimer timer)
        {
            if (timer == null) { throw new ArgumentNullException("timer"); }

            string httpEndpoint = LogSinkFactory.GetHttpEndpoint();
            string wsEndpoint = LogSinkFactory.GetWsEndpoint();
            bool hasHttp = !string.IsNullOrEmpty(httpEndpoint);
            bool hasWs = !string.IsNullOrEmpty(wsEndpoint);

            if (hasWs && hasHttp)
            {
                var transport = new WindowWebSocketTransport(wsEndpoint);
                var ws = new WebSocketLogSink(
                    transport.IsConnected,
                    transport.Send,
                    DefaultBatchSize,
                    DefaultFlushIntervalMs,
                    DefaultMaxQueueSize,
                    DefaultAckTimeoutMs,
                    DefaultMaxRetry,
                    timer);
                transport.OnAck(ws.HandleAck);
                transport.OnDisconnect(ws.NotifyDisconnected);

                var http = new HttpLogSink(
                    httpEndpoint,
                    DefaultBatchSize,
                    DefaultFlushIntervalMs,
                    DefaultMaxQueueSize,
                    timer);

                return new FailoverLogSink(ws, http, transport.IsConnected);
            }

            if (hasWs)
            {
                var transport = new WindowWebSocketTransport(wsEndpoint);
                var ws = new WebSocketLogSink(
                    transport.IsConnected,
                    transport.Send,
                    DefaultBatchSize,
                    DefaultFlushIntervalMs,
                    DefaultMaxQueueSize,
                    DefaultAckTimeoutMs,
                    DefaultMaxRetry,
                    timer);
                transport.OnAck(ws.HandleAck);
                transport.OnDisconnect(ws.NotifyDisconnected);
                return ws;
            }

            if (hasHttp)
            {
                return new HttpLogSink(
                    httpEndpoint,
                    DefaultBatchSize,
                    DefaultFlushIntervalMs,
                    DefaultMaxQueueSize,
                    timer);
            }

            return new ConsoleSink();
        }

        /// <summary>
        /// Read the host page's <c>window.__nscriptLogEndpoint</c>
        /// global (HTTP ingest URL), or null if unset. Wrapped in
        /// <c>typeof</c> so an unset global doesn't throw a
        /// ReferenceError at script-eval time.
        /// </summary>
        [Script("return (typeof window !== 'undefined' && window.__nscriptLogEndpoint) ? window.__nscriptLogEndpoint : null;")]
        private static extern string GetHttpEndpoint();

        /// <summary>
        /// Read the host page's <c>window.__nscriptLogWsEndpoint</c>
        /// global (WebSocket ingest URL), or null if unset.
        /// </summary>
        [Script("return (typeof window !== 'undefined' && window.__nscriptLogWsEndpoint) ? window.__nscriptLogWsEndpoint : null;")]
        private static extern string GetWsEndpoint();
    }
}
