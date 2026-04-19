//-----------------------------------------------------------------------
// <copyright file="ConsoleSink.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Default <see cref="ILogSink"/> — writes each event as a single-line
    /// JSON string to the browser console, routed by severity to
    /// <c>console.error</c> / <c>console.warn</c> / <c>console.log</c>.
    /// </summary>
    /// <remarks>
    /// Preserves the pre-WI-11 on-console behavior: existing developers
    /// tailing DevTools Console see the same JSON shape they always saw,
    /// plus the new <c>cat</c> and <c>props</c> fields when set.
    /// JSON construction lives in <see cref="LogJsonBuilder"/> (pure C#) so
    /// that key names survive NScript minification without ceremony.
    /// </remarks>
    public class ConsoleSink : ILogSink
    {
        public void Handle(LogEvent evt)
        {
            string payload = LogJsonBuilder.BuildEvent(evt);
            ConsoleSink.EmitToConsole((int)evt.Level, payload);
        }

        public void Flush()
        {
        }

        public void Detach()
        {
        }

        /// <summary>
        /// Route a pre-serialized JSON payload to the appropriate console
        /// method. Trace/Debug/Info → <c>console.log</c>, Warn →
        /// <c>console.warn</c>, Error → <c>console.error</c>. Kept small and
        /// flat so the NScript JS parser handles it predictably.
        /// </summary>
        [Script(@"
            if (level >= 3) console.error(payload);
            else if (level >= 2) console.warn(payload);
            else console.log(payload);
        ")]
        private static extern void EmitToConsole(int level, string payload);
    }
}
