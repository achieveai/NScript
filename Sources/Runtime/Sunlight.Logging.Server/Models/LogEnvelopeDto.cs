// -----------------------------------------------------------------------
// <copyright file="LogEnvelopeDto.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Wire-shape DTO for the envelope produced by the browser-side
    /// <c>LogJsonBuilder.BuildEnvelope</c>: <c>{ "events": [...], "dropped": N }</c>.
    /// </summary>
    /// <remarks>
    /// <c>Dropped</c> is always present in the wire shape (the client emits it
    /// unconditionally) so a positive value can surface as a single Warning
    /// on category <c>Sunlight.Browser.Overflow</c> on the server.
    /// </remarks>
    public sealed class LogEnvelopeDto
    {
        /// <summary> Batched events; never null on a well-formed envelope. </summary>
        [JsonPropertyName("events")]
        public List<LogEventDto> Events { get; set; } = new List<LogEventDto>();

        /// <summary> Count of events dropped client-side due to queue overflow. </summary>
        [JsonPropertyName("dropped")]
        public int Dropped { get; set; }
    }
}
