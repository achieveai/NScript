// -----------------------------------------------------------------------
// <copyright file="WebSocketLogProtocol.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Sunlight.Logging.Server.Models;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// Stateless helper that lets a BYOWS (bring-your-own-WebSocket)
    /// consumer process one inbound frame through
    /// <see cref="ILogIngestionService"/> without coupling to our
    /// <see cref="System.Net.WebSockets.WebSocket"/> handler. Given the
    /// raw frame bytes, returns the ACK bytes to send back over the
    /// caller's own pipe.
    /// </summary>
    /// <remarks>
    /// This is the public BYOWS surface (D7). Pure function — no
    /// <see cref="System.Net.WebSockets.WebSocket"/> type touched here. The
    /// caller owns receive-loop, fragmentation reassembly, and send;
    /// they just hand us a reassembled UTF-8 frame and we hand back the
    /// JSON ACK bytes.
    /// </remarks>
    public static class WebSocketLogProtocol
    {
        /// <summary>
        /// Reused serializer options. Case-insensitive matches a slightly
        /// permissive contract — if a client emits e.g. <c>"Level"</c>
        /// instead of <c>"level"</c> the field still binds. Wire shape
        /// stays <c>JsonPropertyName</c>-pinned so we still ROUND-TRIP in
        /// canonical lowercase.
        /// </summary>
        private static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        /// <summary>
        /// Ingest one envelope worth of bytes and produce the ACK bytes.
        /// </summary>
        /// <param name="service">Ingestion service to call. Required.</param>
        /// <param name="frameBytes">
        /// Reassembled UTF-8 JSON frame bytes. Caller is responsible for
        /// stitching together fragmented WebSocket messages before calling.
        /// </param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// UTF-8 JSON bytes of a <see cref="LogAckDto"/>. Always returns a
        /// well-formed ACK frame, including when the envelope is malformed
        /// (in which case <c>AckIds</c> is empty). Caller can send this
        /// straight back over the WebSocket.
        /// </returns>
        public static async Task<byte[]> HandleFrameAsync(
            ILogIngestionService service,
            ReadOnlyMemory<byte> frameBytes,
            CancellationToken ct)
        {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }

            LogEnvelopeDto? envelope = null;
            try
            {
                // Empty/whitespace frames produce a null envelope which is
                // handled the same way as a deserialization failure.
                if (frameBytes.Length > 0)
                {
                    envelope = JsonSerializer.Deserialize<LogEnvelopeDto>(frameBytes.Span, DeserializeOptions);
                }
            }
            catch (JsonException)
            {
                // Malformed frame: do NOT throw out to the receive loop.
                // The consumer can keep the connection alive and the client
                // gets an empty-ack response, which surfaces as zero
                // progress on its retransmit counter — they'll eventually
                // exhaust maxRetry and drop the bad batch.
                envelope = null;
            }

            IReadOnlyList<string> acked;
            if (envelope == null)
            {
                acked = Array.Empty<string>();
            }
            else
            {
                acked = await service.IngestAsync(envelope, ct).ConfigureAwait(false);
            }

            var ackDto = new LogAckDto
            {
                AckIds = acked.Count == 0 ? Array.Empty<string>() : System.Linq.Enumerable.ToArray(acked),
            };

            var json = JsonSerializer.Serialize(ackDto);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
