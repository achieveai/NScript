// -----------------------------------------------------------------------
// <copyright file="WebSocketLogHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Hosting
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Sunlight.Logging.Server.Protocol;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// Convenience helper: when the consumer is OK with us owning the
    /// <see cref="WebSocket"/> receive loop, they call
    /// <see cref="RunAsync"/> from their endpoint handler and we drive
    /// reassembly + ACK send via <see cref="WebSocketLogProtocol"/>.
    /// </summary>
    /// <remarks>
    /// BYOWS consumers may skip this and call
    /// <see cref="WebSocketLogProtocol.HandleFrameAsync"/> directly from
    /// their own receive loop, retaining control over fragmentation and
    /// keep-alive policies.
    /// </remarks>
    public static class WebSocketLogHandler
    {
        // Initial buffer for a single ReceiveAsync slice. Most envelopes
        // are well under 64KB; the loop grows the assembly buffer as
        // needed via MemoryStream.
        private const int ReceiveBufferSize = 16 * 1024;

        /// <summary>
        /// Drive the WebSocket receive loop until the client closes or
        /// the cancellation token fires. Each fully-reassembled text
        /// frame is fed through <see cref="WebSocketLogProtocol.HandleFrameAsync"/>
        /// and the resulting ACK bytes are written back over the same
        /// socket.
        /// </summary>
        /// <param name="socket">The opened server-side WebSocket.</param>
        /// <param name="service">The ingestion service.</param>
        /// <param name="ct">Cancellation token tied to the request lifetime.</param>
        public static async Task RunAsync(
            WebSocket socket,
            ILogIngestionService service,
            CancellationToken ct)
        {
            if (socket == null) { throw new ArgumentNullException(nameof(socket)); }
            if (service == null) { throw new ArgumentNullException(nameof(service)); }

            // Single rented buffer for the per-ReceiveAsync slice.
            var rented = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
            try
            {
                while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
                {
                    // Reassemble one logical frame: keep reading slices
                    // until EndOfMessage. We avoid allocating a MemoryStream
                    // until we hit a fragmentation case, since the common
                    // case is a single sub-buffer read.
                    var first = await socket.ReceiveAsync(rented.AsMemory(0, ReceiveBufferSize), ct)
                        .ConfigureAwait(false);

                    if (first.MessageType == WebSocketMessageType.Close)
                    {
                        // Acknowledge close per protocol.
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "client closed",
                            ct).ConfigureAwait(false);
                        return;
                    }

                    ReadOnlyMemory<byte> frame;
                    if (first.EndOfMessage)
                    {
                        // Fast path: one slice was the whole message.
                        frame = rented.AsMemory(0, first.Count);
                    }
                    else
                    {
                        using var assembly = new MemoryStream();
                        assembly.Write(rented, 0, first.Count);
                        while (true)
                        {
                            var more = await socket.ReceiveAsync(rented.AsMemory(0, ReceiveBufferSize), ct)
                                .ConfigureAwait(false);
                            if (more.MessageType == WebSocketMessageType.Close)
                            {
                                await socket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "client closed mid-frame",
                                    ct).ConfigureAwait(false);
                                return;
                            }
                            assembly.Write(rented, 0, more.Count);
                            if (more.EndOfMessage) { break; }
                        }
                        frame = assembly.ToArray();
                    }

                    var ackBytes = await WebSocketLogProtocol
                        .HandleFrameAsync(service, frame, ct)
                        .ConfigureAwait(false);

                    // Send the ACK back as a text frame so the client's
                    // onmessage handler sees a JSON string (matches
                    // WindowWebSocketTransport's parser).
                    await socket.SendAsync(
                        ackBytes,
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        cancellationToken: ct).ConfigureAwait(false);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }
}
