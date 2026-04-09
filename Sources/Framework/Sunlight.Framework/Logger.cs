//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    /// <summary>
    /// Structured client-side logger. Every entry is auto-enriched with
    /// CallContext IDs (actionId, traceId, spanId) for OTEL correlation.
    /// Output goes to browser console as structured JSON.
    /// </summary>
    public static class Logger
    {
        private static LogLevel minLevel = LogLevel.Debug;

        public static LogLevel MinLevel
        {
            get { return Logger.minLevel; }
            set { Logger.minLevel = value; }
        }

        public static void Debug(string message)
        {
            if (Logger.minLevel <= LogLevel.Debug)
                Logger.Emit(LogLevel.Debug, message);
        }

        public static void Info(string message)
        {
            if (Logger.minLevel <= LogLevel.Info)
                Logger.Emit(LogLevel.Info, message);
        }

        public static void Warn(string message)
        {
            if (Logger.minLevel <= LogLevel.Warn)
                Logger.Emit(LogLevel.Warn, message);
        }

        public static void Error(string message)
        {
            if (Logger.minLevel <= LogLevel.Error)
                Logger.Emit(LogLevel.Error, message);
        }

        /// <summary>
        /// Emits a structured JSON log entry to the browser console, enriched
        /// with CallContext IDs when an action context is active.
        /// </summary>
        /// <remarks>
        /// [Script] is required here because this method uses JS-native APIs that
        /// have no NScript C# facade: Date.now(), JSON.stringify(), console.log/warn/error,
        /// and dynamic property assignment on an object literal (entry.actionId = ...).
        /// None of these are expressible in NScript's C# subset.
        /// </remarks>
        [Script(@"
            var entry = {
                ts: Date.now(),
                level: level === 0 ? 'DEBUG' : level === 1 ? 'INFO' : level === 2 ? 'WARN' : 'ERROR',
                msg: message
            };
            var ctx = @{[Sunlight.Framework]Sunlight.Framework.CallContext::current};
            if (ctx) {
                entry.actionId = ctx.@{[Sunlight.Framework]Sunlight.Framework.CallContext::ActionId};
                entry.traceId = ctx.@{[Sunlight.Framework]Sunlight.Framework.CallContext::TraceId};
                entry.spanId = ctx.@{[Sunlight.Framework]Sunlight.Framework.CallContext::SpanId};
                if (ctx.@{[Sunlight.Framework]Sunlight.Framework.CallContext::ParentSpanId}) {
                    entry.parentSpanId = ctx.@{[Sunlight.Framework]Sunlight.Framework.CallContext::ParentSpanId};
                }
            }
            if (level >= 3) console.error(JSON.stringify(entry));
            else if (level >= 2) console.warn(JSON.stringify(entry));
            else console.log(JSON.stringify(entry));
        ")]
        private static extern void Emit(LogLevel level, string message);
    }
}
