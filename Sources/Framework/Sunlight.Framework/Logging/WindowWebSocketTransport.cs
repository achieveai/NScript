//-----------------------------------------------------------------------
// <copyright file="WindowWebSocketTransport.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;

    /// <summary>
    /// Optional convenience helper for consumers that don't already have
    /// a <see cref="WebSocket"/> open. Wraps the browser
    /// <c>window.WebSocket</c> and packages the
    /// <c>(Func&lt;bool&gt; isConnected, Action&lt;string&gt; send)</c>
    /// pair that <see cref="WebSocketLogSink"/>'s ctor expects, plus an
    /// onmessage hook that parses the server's
    /// <c>{"ackIds":[...]}</c> frames into the sink's
    /// <see cref="WebSocketLogSink.HandleAck"/> callback.
    /// </summary>
    /// <remarks>
    /// Consumers with an existing application WebSocket should skip this
    /// helper and pass their own callbacks to
    /// <see cref="WebSocketLogSink"/> directly — that's the entire point
    /// of the BYOWS design.
    /// </remarks>
    public class WindowWebSocketTransport
    {
        private readonly WebSocket socket;
        private Action<string[]> ackCallback;
        private Action disconnectCallback;

        public WindowWebSocketTransport(string url)
        {
            if (url == null) { throw new ArgumentNullException("url"); }
            this.socket = new WebSocket(url);
            this.socket.OnMessage += this.OnSocketMessage;
            this.socket.OnClose += this.OnSocketClose;
            this.socket.OnError += this.OnSocketError;
        }

        /// <summary>
        /// Connection-state probe for <see cref="WebSocketLogSink"/>'s
        /// <c>isConnected</c> callback. Returns true only when the
        /// underlying <see cref="WebSocket.ReadyState"/> is Open.
        /// </summary>
        public bool IsConnected()
        {
            return this.socket.ReadyState == WebSocketReadyState.Open;
        }

        /// <summary>
        /// Send-payload callback for <see cref="WebSocketLogSink"/>'s
        /// <c>sendPayload</c> parameter. Fire-and-forget: drops the send
        /// if the socket has rotated to a non-Open state since the sink
        /// last checked, so a mid-tick race cannot crash the timer.
        /// </summary>
        public void Send(string payload)
        {
            if (this.socket.ReadyState != WebSocketReadyState.Open) { return; }
            try { this.socket.Send(payload); }
            catch { /* swallow — sink retransmits on next tick */ }
        }

        /// <summary>
        /// Register the callback invoked with the ackIds list whenever
        /// the server replies with a <c>{"ackIds":[...]}</c> frame.
        /// Typically wired to <see cref="WebSocketLogSink.HandleAck"/>.
        /// </summary>
        public void OnAck(Action<string[]> callback)
        {
            this.ackCallback = callback;
        }

        /// <summary>
        /// Register the callback fired when the underlying socket closes
        /// or errors out. Typically wired to
        /// <see cref="WebSocketLogSink.NotifyDisconnected"/>.
        /// </summary>
        public void OnDisconnect(Action callback)
        {
            this.disconnectCallback = callback;
        }

        private void OnSocketMessage(WebSocket sock, MessageEvent evt)
        {
            // evt.Data is whatever the server sent. For our protocol it's
            // a JSON string of shape {"ackIds":["a","b"]}.
            if (evt == null || evt.Data == null) { return; }
            var ids = WindowWebSocketTransport.ParseAckIds(evt.Data);
            if (ids != null && this.ackCallback != null)
            {
                this.ackCallback(ids);
            }
        }

        private void OnSocketClose(WebSocket sock, WebSocketCloseEvent evt)
        {
            if (this.disconnectCallback != null) { this.disconnectCallback(); }
        }

        private void OnSocketError(WebSocket sock, ErrorEvent evt)
        {
            // Error events are commonly followed by a close, but we
            // surface a disconnect on error too so the sink doesn't sit
            // on un-acked events if the close never fires.
            if (this.disconnectCallback != null) { this.disconnectCallback(); }
        }

        /// <summary>
        /// Parse <c>{"ackIds":[...]}</c> via native <c>JSON.parse</c> +
        /// a small JS shim. Returns the ids array, or null if the frame
        /// shape is wrong.
        /// </summary>
        [Script(@"
            try {
                var parsed = (typeof data === 'string') ? @:JSON.parse(data) : data;
                if (parsed && parsed.ackIds && parsed.ackIds.length !== undefined) {
                    return parsed.ackIds;
                }
                return null;
            } catch (e) { return null; }
        ")]
        private static extern string[] ParseAckIds(object data);
    }
}
