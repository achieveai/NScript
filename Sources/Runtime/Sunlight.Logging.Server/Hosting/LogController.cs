// -----------------------------------------------------------------------
// <copyright file="LogController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Sunlight.Logging.Server.Models;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// HTTP entry point for client-side
    /// <c>HttpLogSink</c> POSTs. Convenience wrapper around
    /// <see cref="ILogIngestionService.IngestAsync"/>. Consumers that
    /// already have their own controller may skip this and call the
    /// service directly.
    /// </summary>
    [ApiController]
    [Route("_log")]
    public sealed class LogController : ControllerBase
    {
        private readonly ILogIngestionService _ingestion;

        public LogController(ILogIngestionService ingestion)
        {
            this._ingestion = ingestion;
        }

        /// <summary>
        /// Ingest one HTTP envelope and return the per-event ACKs. Even
        /// though HTTP doesn't strictly need per-event ACK semantics for
        /// delivery (the response itself is the ACK), we still return
        /// the id list so the wire shape stays symmetric with the
        /// WebSocket path and a future client can de-dup against it.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LogAckDto>> Post(
            [FromBody] LogEnvelopeDto envelope,
            CancellationToken ct)
        {
            // Null body deserializes to null; treat as an empty envelope
            // and ACK nothing rather than 400 — the path is
            // best-effort by design.
            if (envelope == null)
            {
                return this.Ok(new LogAckDto());
            }

            var acked = await this._ingestion.IngestAsync(envelope, ct);
            return this.Ok(new LogAckDto
            {
                AckIds = acked.Count == 0
                    ? System.Array.Empty<string>()
                    : System.Linq.Enumerable.ToArray(acked),
            });
        }
    }
}
