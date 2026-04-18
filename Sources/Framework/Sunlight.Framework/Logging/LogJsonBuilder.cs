//-----------------------------------------------------------------------
// <copyright file="LogJsonBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Builds the JSON envelope for <see cref="LogEvent"/>s in pure C# so that
    /// field names are stable string literals (not minified). Defers only string
    /// escaping to the browser's native <c>JSON.stringify</c>, which handles
    /// quoting, control characters, and non-ASCII safely.
    /// </summary>
    /// <remarks>
    /// The JSON is assembled through plain C# string concatenation rather than
    /// inside a <c>[Script]</c> block because the NScript JavaScript parser does
    /// not support <c>for (var i = 0; ...)</c> loops inside <c>[Script]</c>
    /// blocks — assignment-inside-var-init produces an AST shape the parser
    /// mishandles. Doing the iteration in ordinary C# sidesteps that limitation
    /// while still producing identical output.
    /// </remarks>
    internal static class LogJsonBuilder
    {
        /// <summary> Serialize a single <see cref="LogEvent"/> as a JSON object string. </summary>
        public static string BuildEvent(LogEvent evt)
        {
            var sb = new StringBuilder();
            LogJsonBuilder.AppendEvent(sb, evt);
            return sb.ToString();
        }

        /// <summary>
        /// Serialize a batch of events as the HTTP envelope:
        /// <c>{ "events": [...], "dropped": N }</c>. <c>dropped</c> is always
        /// included so the ingestion handler can surface overflow.
        /// </summary>
        public static string BuildEnvelope(List<LogEvent> events, int dropped)
        {
            var sb = new StringBuilder();
            sb.Append("{\"events\":[");
            for (int i = 0; i < events.Count; i++)
            {
                if (i > 0) { sb.Append(","); }
                LogJsonBuilder.AppendEvent(sb, events[i]);
            }
            sb.Append("],\"dropped\":");
            sb.Append(dropped.ToString());
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Append a single event object to <paramref name="sb"/>. Split out so
        /// both the single-event and batch paths share one serialization code
        /// path and stay in sync.
        /// </summary>
        private static void AppendEvent(StringBuilder sb, LogEvent evt)
        {
            sb.Append("{\"ts\":");
            sb.Append(LogJsonBuilder.JsonEscape(evt.TimestampIso));
            sb.Append(",\"level\":");
            sb.Append(LogJsonBuilder.JsonEscape(LogJsonBuilder.LevelToString(evt.Level)));
            sb.Append(",\"msg\":");
            sb.Append(LogJsonBuilder.JsonEscape(evt.Message));

            if (!string.IsNullOrEmpty(evt.Category))
            {
                sb.Append(",\"cat\":");
                sb.Append(LogJsonBuilder.JsonEscape(evt.Category));
            }

            // Flat [k,v,k,v,...] → {k:v,k:v,...}. Odd trailing element is
            // dropped: a key without a value is almost always a caller bug.
            var props = evt.Properties;
            if (props != null && props.Length >= 2)
            {
                sb.Append(",\"props\":{");
                bool first = true;
                int n = props.Length;
                int i = 0;
                while (i + 1 < n)
                {
                    if (!first) { sb.Append(","); }
                    first = false;
                    sb.Append(LogJsonBuilder.JsonEscape(props[i]));
                    sb.Append(":");
                    sb.Append(LogJsonBuilder.JsonEscape(props[i + 1]));
                    i = i + 2;
                }
                sb.Append("}");
            }

            // Correlation fields are owned by CallContext itself so that
            // adding/removing a field is a one-place change — this sink and
            // any future ones (e.g. WebSocket) stay automatically in sync.
            // Wrapped in a lambda (not passed as a method group) because
            // JsonEscape is a [Script]-bodied extern and NScript's C#-to-JS
            // converter is not guaranteed to synthesize a delegate for such
            // methods safely.
            var ctx = evt.Context;
            if (ctx != null)
            {
                ctx.AppendCorrelationJson(sb, s => LogJsonBuilder.JsonEscape(s));
            }

            sb.Append("}");
        }

        private static string LevelToString(LogLevel level)
        {
            if (level == LogLevel.Trace) { return "TRACE"; }
            if (level == LogLevel.Debug) { return "DEBUG"; }
            if (level == LogLevel.Info) { return "INFO"; }
            if (level == LogLevel.Warn) { return "WARN"; }
            return "ERROR";
        }

        /// <summary>
        /// Wrap <paramref name="s"/> as a JSON-safe quoted string via the
        /// browser's native <c>JSON.stringify</c> — handles escape sequences,
        /// embedded quotes, control characters, and non-ASCII code points.
        /// Returns the literal <c>"null"</c> (without quotes) for null input,
        /// matching <c>JSON.stringify(null)</c> semantics.
        /// </summary>
        [Script(@"return @:JSON.stringify(s);")]
        private static extern string JsonEscape(string s);
    }
}
