// -----------------------------------------------------------------------
// <copyright file="LogAckDto.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Wire-shape DTO returned by both HTTP (<c>LogController</c>) and
    /// WebSocket (<c>WebSocketLogProtocol</c>) entry points. Lists the ids
    /// the server confirmed via <see cref="System.IO.TextWriter"/>-equivalent
    /// MEL emission, so the client-side <c>WebSocketLogSink</c> can remove
    /// them from its in-flight retransmit map.
    /// </summary>
    /// <remarks>
    /// An empty array is a legitimate response (e.g. an envelope of all
    /// duplicate ids that were de-duped, or all ids that hit a per-event
    /// exception inside the service). Clients must not treat it as an error.
    /// </remarks>
    public sealed class LogAckDto
    {
        /// <summary> Ids the server successfully forwarded to MEL. Empty array is valid. </summary>
        [JsonPropertyName("ackIds")]
        public string[] AckIds { get; set; } = System.Array.Empty<string>();
    }
}
