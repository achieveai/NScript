// -----------------------------------------------------------------------
// <copyright file="ILogIngestionService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Sunlight.Logging.Server.Models;

    /// <summary>
    /// Public surface that consumer apps call directly from their HTTP or
    /// WebSocket handler. Hides the dedup, MEL mapping, and re-entrancy
    /// guard. Returns the ids successfully emitted to
    /// <c>Microsoft.Extensions.Logging</c> so the caller can ACK them back
    /// to the client-side <c>WebSocketLogSink</c>.
    /// </summary>
    /// <remarks>
    /// BYOWS consumers should prefer calling this directly over going
    /// through the convenience <c>MapSunlightLogIngestion</c> endpoint —
    /// it keeps the consumer in control of their WebSocket receive loop.
    /// </remarks>
    public interface ILogIngestionService
    {
        /// <summary>
        /// Ingest one envelope. Each event is mapped to an
        /// <c>ILogger.Log</c> call on category
        /// <c>"Sunlight.Browser." + (Cat ?? "default")</c>. Correlation
        /// fields and props become MEL scope state.
        /// </summary>
        /// <param name="envelope">Deserialized client envelope. Required.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// The ids of events that were successfully forwarded to MEL.
        /// Events skipped by the dedup LRU or that hit a per-event exception
        /// are omitted. An empty list is valid.
        /// </returns>
        ValueTask<IReadOnlyList<string>> IngestAsync(LogEnvelopeDto envelope, CancellationToken ct);
    }
}
