//-----------------------------------------------------------------------
// <copyright file="CompilerLog.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Utils
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Serilog;
    using Serilog.Formatting.Compact;

    /// <summary>
    /// Shared opt-in structured JSONL logging facility for the NScript compiler
    /// pipeline. When <see cref="Initialize(string, string, string)"/> has not been
    /// called, <see cref="ForComponent(string)"/> returns a silent no-op logger and
    /// no file I/O occurs — preserving the default zero-overhead path.
    ///
    /// Both compiler stages (Stage 1 <c>csc</c>, Stage 2 <c>cs2jsc</c>) may write to
    /// the same log file in a single build. The file sink is opened in shared-append
    /// mode so cross-process writes coexist. A <c>RunId</c> property enriches every
    /// event for cross-stage correlation.
    /// </summary>
    public static class CompilerLog
    {
        /// <summary>
        /// Environment variable fallback for the log file path. Useful when csc is
        /// invoked via MSBuild response files where adding custom tokens is awkward.
        /// </summary>
        public const string LogPathEnvVar = "NSCRIPT_LOG_PATH";

        /// <summary>
        /// Environment variable fallback for the run id.
        /// </summary>
        public const string RunIdEnvVar = "NSCRIPT_LOG_RUNID";

        private static readonly object initLock = new object();

        /// <summary>
        /// Shared no-op logger returned when structured logging is disabled.
        /// A `LoggerConfiguration` with no sinks silently discards every event —
        /// no allocations aside from the logger itself, which is singleton.
        /// </summary>
        private static readonly ILogger silentLogger =
            new LoggerConfiguration().CreateLogger();

        private static volatile bool isEnabled;

        private static ILogger rootLogger;

        private static string resolvedLogPath;

        private static string resolvedRunId;

        private static string resolvedStage;

        /// <summary>
        /// Gets a value indicating whether structured logging is enabled.
        /// </summary>
        public static bool IsEnabled => isEnabled;

        /// <summary>
        /// Gets the resolved log file path (null when not initialized).
        /// </summary>
        public static string LogPath => resolvedLogPath;

        /// <summary>
        /// Gets the resolved run id (null when not initialized).
        /// </summary>
        public static string RunId => resolvedRunId;

        /// <summary>
        /// Gets the resolved stage name (null when not initialized).
        /// </summary>
        public static string Stage => resolvedStage;

        /// <summary>
        /// Initialize the shared logger. Idempotent — repeated calls are a no-op.
        /// Safe to call from any stage entry point.
        /// </summary>
        /// <param name="path">     Log file path. When null or whitespace, logging stays disabled. </param>
        /// <param name="stage">    Stage label (e.g. "csc", "cs2jsc"). Defaults to "unknown". </param>
        /// <param name="runId">    Optional cross-process run correlation id. When null, env var or a fresh GUID is used. </param>
        public static void Initialize(string path, string stage, string runId = null)
        {
            if (isEnabled)
            {
                return;
            }

            var resolvedPath = ResolveLogPath(path);
            if (string.IsNullOrWhiteSpace(resolvedPath))
            {
                return;
            }

            lock (initLock)
            {
                if (isEnabled)
                {
                    return;
                }

                try
                {
                    EnsureDirectoryExists(resolvedPath);

                    var effectiveStage = string.IsNullOrWhiteSpace(stage) ? "unknown" : stage;
                    var effectiveRunId = ResolveRunId(runId);

                    var configuration = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.WithProperty("RunId", effectiveRunId)
                        .Enrich.WithProperty("Stage", effectiveStage)
                        .Enrich.WithProperty("Pid", Process.GetCurrentProcess().Id)
                        .Enrich.WithProperty("MachineName", Environment.MachineName)
                        .WriteTo.File(
                            formatter: new CompactJsonFormatter(),
                            path: resolvedPath,
                            shared: true,
                            rollOnFileSizeLimit: false,
                            flushToDiskInterval: TimeSpan.FromSeconds(1));

                    rootLogger = configuration.CreateLogger();
                    Log.Logger = rootLogger;

                    resolvedLogPath = resolvedPath;
                    resolvedRunId = effectiveRunId;
                    resolvedStage = effectiveStage;
                    isEnabled = true;
                }
                catch (Exception ex)
                {
                    // Logger bootstrap must never break the compiler. Surface a one-line
                    // stderr warning and continue with logging disabled.
                    Console.Error.WriteLine($"CompilerLog: failed to initialize ({ex.Message}); logging disabled.");
                    rootLogger = null;
                    isEnabled = false;
                }
            }
        }

        /// <summary>
        /// Returns a logger enriched with the given <c>Component</c> name. When the
        /// shared logger has not been initialized, returns a silent no-op logger.
        /// </summary>
        public static ILogger ForComponent(string component)
        {
            if (!isEnabled || rootLogger == null)
            {
                return silentLogger;
            }

            return rootLogger.ForContext("Component", component ?? "unknown");
        }

        /// <summary>
        /// Flushes and closes the shared logger. Safe to call when not initialized.
        /// </summary>
        public static void Shutdown()
        {
            if (!isEnabled)
            {
                return;
            }

            lock (initLock)
            {
                if (!isEnabled)
                {
                    return;
                }

                try
                {
                    Log.CloseAndFlush();
                }
                catch
                {
                    // Swallow on shutdown — nothing we can do about a logger that is already disposed.
                }
                finally
                {
                    rootLogger = null;
                    resolvedLogPath = null;
                    resolvedRunId = null;
                    resolvedStage = null;
                    isEnabled = false;
                }
            }
        }

        /// <summary>
        /// Resolve a run id from (in priority order) explicit arg, <see cref="RunIdEnvVar"/>, or a fresh GUID.
        /// </summary>
        public static string ResolveRunId(string explicitId)
        {
            if (!string.IsNullOrWhiteSpace(explicitId))
            {
                return explicitId.Trim();
            }

            var fromEnv = Environment.GetEnvironmentVariable(RunIdEnvVar);
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                return fromEnv.Trim();
            }

            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Resolve a log path from (in priority order) explicit arg or <see cref="LogPathEnvVar"/>.
        /// Returns null (logging disabled) when neither is provided.
        /// </summary>
        public static string ResolveLogPath(string explicitPath)
        {
            if (!string.IsNullOrWhiteSpace(explicitPath))
            {
                return explicitPath.Trim();
            }

            var fromEnv = Environment.GetEnvironmentVariable(LogPathEnvVar);
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                return fromEnv.Trim();
            }

            return null;
        }

        private static void EnsureDirectoryExists(string path)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
