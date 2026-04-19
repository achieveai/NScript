//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Static entry point for structured client-side logging. Fans emitted
    /// <see cref="LogEvent"/>s out to every registered <see cref="ILogSink"/>
    /// after <see cref="MinLevel"/> filtering.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Backwards-compatible with the pre-WI-11 API: the static
    /// <see cref="Debug"/> / <see cref="Info"/> / <see cref="Warn"/> /
    /// <see cref="Error"/> methods still accept a single <c>string</c> message.
    /// Pre-existing callers (e.g. <c>TaskScheduler</c>'s exception handlers) do
    /// not need to change.
    /// </para>
    /// <para>
    /// <see cref="Trace"/> and <see cref="Debug"/> are tagged with
    /// <c>[Conditional("DEBUG")]</c>: their calls — including argument
    /// evaluation — are stripped from the bound tree by Roslyn during Stage 1
    /// of the NScript pipeline whenever the caller's compilation is built
    /// without the <c>DEBUG</c> symbol (i.e. the Release config). This yields
    /// zero runtime overhead for verbose logs in production builds.
    /// </para>
    /// </remarks>
    public static class Logger
    {
        private static LogLevel minLevel = LogLevel.Debug;
        private static List<ILogSink> sinks;
        private static StringDictionary<NamedLogger> categoryCache;
        private static bool userConfigured;
        private static bool dispatching;

        public static LogLevel MinLevel
        {
            get { return Logger.minLevel; }
            set { Logger.minLevel = value; }
        }

        /// <summary>
        /// Obtain (and cache) a category-scoped logger. Subsequent calls with
        /// the same category string return the same instance so consumers can
        /// hold a cached reference at static-init time without worrying about
        /// allocations per call site.
        /// </summary>
        /// <remarks>
        /// Return type is the concrete <see cref="NamedLogger"/>, not
        /// <see cref="ILogger"/>: the <c>[Conditional("DEBUG")]</c> on Trace /
        /// Debug only fires through the concrete type. See <see cref="ILogger"/>
        /// for the full rationale.
        /// </remarks>
        public static NamedLogger ForCategory(string category)
        {
            if (category == null) { category = string.Empty; }
            if (Logger.categoryCache == null)
            {
                Logger.categoryCache = new StringDictionary<NamedLogger>();
            }

            NamedLogger existing;
            if (Logger.categoryCache.TryGetValue(category, out existing))
            {
                return existing;
            }

            var created = new NamedLogger(category);
            Logger.categoryCache[category] = created;
            return created;
        }

        public static void AddSink(ILogSink sink)
        {
            if (sink == null) { return; }
            Logger.EnsureSinkList();
            Logger.userConfigured = true;
            Logger.sinks.Add(sink);
        }

        public static void RemoveSink(ILogSink sink)
        {
            if (sink == null || Logger.sinks == null) { return; }
            if (Logger.sinks.Remove(sink))
            {
                try { sink.Detach(); }
                catch { /* sink cleanup must not surface to caller */ }
            }
        }

        /// <summary>
        /// Remove every registered sink and detach its resources. After this
        /// call no sinks are installed — the default lazy <see cref="ConsoleSink"/>
        /// is suppressed until the caller explicitly adds one via
        /// <see cref="AddSink"/>. This makes the silent-mode setup possible.
        /// </summary>
        public static void ClearSinks()
        {
            Logger.userConfigured = true;
            if (Logger.sinks == null) { return; }
            for (int i = 0; i < Logger.sinks.Count; i++)
            {
                try { Logger.sinks[i].Detach(); }
                catch { /* sink cleanup must not surface to caller */ }
            }
            Logger.sinks.Clear();
        }

        /// <summary> Flush every registered sink. No-op for sinks without buffering. </summary>
        public static void Flush()
        {
            if (Logger.sinks == null) { return; }
            for (int i = 0; i < Logger.sinks.Count; i++)
            {
                try { Logger.sinks[i].Flush(); }
                catch { /* flush failure must not surface to caller */ }
            }
        }

        [Conditional("DEBUG")]
        public static void Trace(string message)
        {
            if (Logger.minLevel <= LogLevel.Trace)
            {
                Logger.DispatchInternal(LogLevel.Trace, string.Empty, message, null);
            }
        }

        [Conditional("DEBUG")]
        public static void Trace(string message, string[] properties)
        {
            if (Logger.minLevel <= LogLevel.Trace)
            {
                Logger.DispatchInternal(LogLevel.Trace, string.Empty, message, properties);
            }
        }

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            if (Logger.minLevel <= LogLevel.Debug)
            {
                Logger.DispatchInternal(LogLevel.Debug, string.Empty, message, null);
            }
        }

        [Conditional("DEBUG")]
        public static void Debug(string message, string[] properties)
        {
            if (Logger.minLevel <= LogLevel.Debug)
            {
                Logger.DispatchInternal(LogLevel.Debug, string.Empty, message, properties);
            }
        }

        public static void Info(string message)
        {
            if (Logger.minLevel <= LogLevel.Info)
            {
                Logger.DispatchInternal(LogLevel.Info, string.Empty, message, null);
            }
        }

        public static void Info(string message, string[] properties)
        {
            if (Logger.minLevel <= LogLevel.Info)
            {
                Logger.DispatchInternal(LogLevel.Info, string.Empty, message, properties);
            }
        }

        public static void Warn(string message)
        {
            if (Logger.minLevel <= LogLevel.Warn)
            {
                Logger.DispatchInternal(LogLevel.Warn, string.Empty, message, null);
            }
        }

        public static void Warn(string message, string[] properties)
        {
            if (Logger.minLevel <= LogLevel.Warn)
            {
                Logger.DispatchInternal(LogLevel.Warn, string.Empty, message, properties);
            }
        }

        public static void Error(string message)
        {
            if (Logger.minLevel <= LogLevel.Error)
            {
                Logger.DispatchInternal(LogLevel.Error, string.Empty, message, null);
            }
        }

        public static void Error(string message, string[] properties)
        {
            if (Logger.minLevel <= LogLevel.Error)
            {
                Logger.DispatchInternal(LogLevel.Error, string.Empty, message, properties);
            }
        }

        /// <summary>
        /// Shared dispatch path used by the static facade and <see cref="NamedLogger"/>.
        /// Builds a <see cref="LogEvent"/> and fans out to every sink under a
        /// try/catch so a single faulty sink cannot break the others (or the
        /// caller, which may be an exception handler).
        /// </summary>
        /// <remarks>
        /// Internal (not public) because the static facade already applies the
        /// <see cref="MinLevel"/> check; <see cref="NamedLogger"/> must apply it
        /// too to match that contract. External callers should always go through
        /// the level-specific methods so level filters and <c>[Conditional]</c>
        /// stripping are honored.
        /// </remarks>
        internal static void DispatchInternal(
            LogLevel level,
            string category,
            string message,
            string[] properties)
        {
            if (level < Logger.minLevel) { return; }

            // Re-entrancy guard protects against a misbehaving sink that calls
            // back into Logger.* (which would otherwise re-enter and duplicate
            // events). Dropping the nested call is preferable to deadlocking
            // or unbounded recursion.
            if (Logger.dispatching) { return; }

            Logger.dispatching = true;
            try
            {
                Logger.EnsureSinkList();
                if (!Logger.userConfigured && Logger.sinks.Count == 0)
                {
                    // Lazy default: first log call installs a ConsoleSink so
                    // existing callers get the old on-console behavior without
                    // any wiring. Once the app opts in (AddSink/ClearSinks)
                    // this branch never re-fires.
                    Logger.sinks.Add(new ConsoleSink());
                }

                var evt = new LogEvent(
                    Logger.GetIsoTimestamp(),
                    level,
                    category,
                    message,
                    properties,
                    CallContext.Current);

                for (int i = 0; i < Logger.sinks.Count; i++)
                {
                    try { Logger.sinks[i].Handle(evt); }
                    catch { /* sink faults must not escape — protects exception-path callers */ }
                }
            }
            catch
            {
                // Event construction failed (e.g. GetIsoTimestamp bridge threw,
                // ConsoleSink allocation failed). Swallowing is the only safe
                // action: Logger.Error() is called from inside TaskScheduler's
                // catch blocks, so an escaping exception would bypass its
                // finally cleanup (currentTask / CallContext restore) and
                // produce cascading failures.
            }
            finally
            {
                Logger.dispatching = false;
            }
        }

        private static void EnsureSinkList()
        {
            if (Logger.sinks == null) { Logger.sinks = new List<ILogSink>(); }
        }

        [Script("return new Date().toISOString();")]
        private static extern string GetIsoTimestamp();
    }
}
