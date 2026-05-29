// -----------------------------------------------------------------------
// <copyright file="LogEventDto.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Wire-shape DTO for one event in a <see cref="LogEnvelopeDto"/>. Field
    /// names are pinned via <see cref="JsonPropertyNameAttribute"/> so the
    /// browser-side <c>LogJsonBuilder.AppendEvent</c> output deserializes
    /// directly without ambient camelCase policy.
    /// </summary>
    /// <remarks>
    /// All fields nullable so a malformed/partial envelope still deserializes
    /// and the per-event try/catch in <c>LogIngestionService.IngestAsync</c>
    /// can isolate the poison event instead of failing the whole batch.
    /// </remarks>
    public sealed class LogEventDto
    {
        /// <summary> Per-emit runtime-unique 64-bit hex id. </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary> ISO-8601 timestamp captured at emit time. </summary>
        [JsonPropertyName("ts")]
        public string? Ts { get; set; }

        /// <summary> TRACE | DEBUG | INFO | WARN | ERROR. </summary>
        [JsonPropertyName("level")]
        public string? Level { get; set; }

        /// <summary> Human-readable message text. </summary>
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        /// <summary>
        /// Optional category suffix. Server creates <c>ILogger</c> via
        /// <c>"Sunlight.Browser." + (Cat ?? "default")</c>.
        /// </summary>
        [JsonPropertyName("cat")]
        public string? Cat { get; set; }

        /// <summary>
        /// Structured properties bag. Flattened to MEL scope state alongside
        /// correlation fields.
        /// </summary>
        [JsonPropertyName("props")]
        public Dictionary<string, string>? Props { get; set; }

        /// <summary> Trace correlation id (from CallContext). </summary>
        [JsonPropertyName("traceId")]
        public string? TraceId { get; set; }

        /// <summary> Span correlation id (from CallContext). </summary>
        [JsonPropertyName("spanId")]
        public string? SpanId { get; set; }

        /// <summary> Parent span correlation id (from CallContext); nullable. </summary>
        [JsonPropertyName("parentSpanId")]
        public string? ParentSpanId { get; set; }

        /// <summary> Action id from CallContext; nullable. </summary>
        [JsonPropertyName("actionId")]
        public int? ActionId { get; set; }

        /// <summary> CallContext nesting depth; nullable. </summary>
        [JsonPropertyName("depth")]
        public int? Depth { get; set; }
    }
}
