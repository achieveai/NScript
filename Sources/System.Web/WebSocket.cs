// file:	WebSocket.cs
//
// summary:	Implements the web socket class
namespace System.Web
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Message event.
    /// </summary>
    [ImportedType, IgnoreNamespace, ScriptName("MessageEvent")]
    public class MessageEvent
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public extern string Type
        { get; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public extern object Data
        { get; }
    }

    /// <summary>
    /// Web socket close event.
    /// </summary>
    [ImportedType, IgnoreNamespace, ScriptName("CloseEvent")]
    public class WebSocketCloseEvent
    {
        /// <summary>
        /// Gets a value indicating whether the was clean.
        /// </summary>
        /// <value>
        /// true if was clean, false if not.
        /// </value>
        public extern bool WasClean
        { get; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public extern ushort Code
        { get; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public extern string Reason
        { get; }
    }

    /// <summary>
    /// Values that represent WebSocketReadyState.
    /// </summary>
    public enum WebSocketReadyState
    {
        /// <summary>
        /// .
        /// </summary>
        Connecting,
        /// <summary>
        /// .
        /// </summary>
        Open,
        /// <summary>
        /// .
        /// </summary>
        Closing,
        /// <summary>
        /// .
        /// </summary>
        Closed
    }

    /// <summary>
    /// Web socket.
    /// </summary>
    [IgnoreNamespace, ScriptName("WebSocket")]
    public class WebSocket
    {
        /// <summary>
        /// Web socket.
        /// </summary>
        /// <param name="url"> URL of the document. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern WebSocket(string url);

        /// <summary>
        /// Gets the state of the ready.
        /// </summary>
        /// <value>
        /// The ready state.
        /// </value>
        public extern WebSocketReadyState ReadyState
        { get; }

        /// <summary>
        /// Gets the buffered amount.
        /// </summary>
        /// <value>
        /// The buffered amount.
        /// </value>
        public extern long BufferedAmount
        { get; }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>
        /// The extensions.
        /// </value>
        public extern string Extensions
        { get; }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public extern string Protocol
        { get; }

        /// <summary>
        /// Event queue for all listeners interested in OnOpen events.
        /// </summary>
        public extern event Action<WebSocket, MessageEvent> OnOpen;

        /// <summary>
        /// Event queue for all listeners interested in OnError events.
        /// </summary>
        public extern event Action<WebSocket, object> OnError;

        /// <summary>
        /// Event queue for all listeners interested in OnClose events.
        /// </summary>
        public extern event Action<WebSocket, object> OnClose;

        /// <summary>
        /// Query if this object is available.
        /// </summary>
        /// <returns>
        /// true if available, false if not.
        /// </returns>
        [Script("return typeof WebSocket == \"undefined\";")]
        public extern static bool IsAvailable();

        /// <summary>
        /// Closes this object.
        /// </summary>
        public extern void Close();

        /// <summary>
        /// Closes this object.
        /// </summary>
        /// <param name="code"> The code. </param>
        public extern void Close(ushort code);

        /// <summary>
        /// Closes this object.
        /// </summary>
        /// <param name="code">   The code. </param>
        /// <param name="reason"> The reason. </param>
        public extern void Close(ushort code, string reason);

        /// <summary>
        /// Send this message.
        /// </summary>
        /// <param name="message"> The message. </param>
        public extern void Send(string message);
    }
}
