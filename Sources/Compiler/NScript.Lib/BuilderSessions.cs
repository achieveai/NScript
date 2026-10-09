namespace NScript.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using NScript.Converter;

    /// <summary>
    /// Dev-mode build sessions for one process (slice 2, Inc 3): one <see cref="Builder"/> per
    /// output file, reused while the options are the same. In batch the process makes one
    /// build, so this is a cold session; in the build service later requests reuse it.
    /// </summary>
    public static class BuilderSessions
    {
        private static readonly object Gate = new object();

        /// <summary>
        /// Keyed by the lower-cased full output path; each entry keeps the options key it was
        /// made with.
        /// </summary>
        private static readonly Dictionary<string, Entry> Sessions = new Dictionary<string, Entry>();

        // Sessions.Count, kept apart so --status can read it while a build holds Gate.
        private static int count;

        /// <summary>
        /// Milliseconds on a monotonic clock, stamped when a build of a session ends. Tests replace it.
        /// </summary>
        public static Func<long> Clock { get; set; } = () => Environment.TickCount64;

        /// <summary>How many outputs have a session entry. Does not wait for a running build.</summary>
        public static int Count => Volatile.Read(ref count);

        /// <summary>
        /// "kind (reason)" of the last build of <paramref name="jsFileName"/>'s session, e.g.
        /// "warm (resources-refreshed)", or null when the output has no session.
        /// </summary>
        public static string LastBuild(string jsFileName)
        {
            var outputKey = Path.GetFullPath(jsFileName).ToLowerInvariant();
            lock (Gate)
            {
                return Sessions.TryGetValue(outputKey, out var entry) && entry.Builder.LastBuildKind != null
                    ? entry.Builder.LastBuildKind + " (" + entry.Builder.LastBuildReason + ")"
                    : null;
            }
        }

        /// <summary>
        /// Builds through the output file's session, making a new one when there is none or
        /// when the options changed (the old one is released).
        /// </summary>
        public static bool Execute(ParseOptions options, Func<Builder> createBuilder, IConverterPlugin[] plugins)
        {
            var outputKey = Path.GetFullPath(options.JsFileName).ToLowerInvariant();
            var optionsKey = OptionsKey(options);
            lock (Gate)
            {
                if (!Sessions.TryGetValue(outputKey, out var entry) || entry.OptionsKey != optionsKey)
                {
                    entry?.Builder.Dispose();
                    entry = new Entry(optionsKey, createBuilder());
                    Sessions[outputKey] = entry;
                    Volatile.Write(ref count, Sessions.Count);
                }

                try
                {
                    return entry.Builder.Execute(plugins);
                }
                finally
                {
                    // Stamped at the end, so a build longer than the sweep timeout is not
                    // released as soon as it finishes.
                    entry.LastUsedMs = Clock();
                }
            }
        }

        /// <summary>
        /// Releases the sessions of outputs not built for <paramref name="maxIdle"/> (a bundle
        /// watched all day but not edited), so their modules can be collected. The next build
        /// of such an output is cold.
        /// </summary>
        /// <returns>How many sessions were dropped.</returns>
        public static int DropIdle(TimeSpan maxIdle)
        {
            lock (Gate)
            {
                long now = Clock();
                var idle = Sessions
                    .Where(pair => now - pair.Value.LastUsedMs >= maxIdle.TotalMilliseconds)
                    .Select(pair => pair.Key)
                    .ToList();
                foreach (var key in idle)
                {
                    Sessions[key].Builder.Dispose();
                    Sessions.Remove(key);
                }

                Volatile.Write(ref count, Sessions.Count);

                return idle.Count;
            }
        }

        /// <summary>
        /// Every option that reaches the <see cref="Builder"/> or could change how inputs
        /// resolve, with paths made full. The hint paths and the plugin config do not reach the
        /// build today (ParseOptions drops -referenceHintPath; Run makes a fixed plugin list),
        /// they are keyed so that wiring them up cannot leave a stale session.
        /// </summary>
        public static string OptionsKey(ParseOptions options)
            => string.Join(
                "|",
                new[]
                {
                    Path.GetFullPath(options.EntryAssembly),
                    options.JsParts.ToString(),
                    options.Minify.ToString(),
                    options.Uglify.ToString(),
                    options.Optimize.ToString(),
                    options.DevMode.ToString(),
                    options.SourceMapRoot ?? string.Empty,
                    options.RepoRoot ?? string.Empty,
                    options.SecondarySourceRoot ?? string.Empty,
                    options.SecondaryRepoRoot ?? string.Empty,
                    options.PluginConfigFileName == null ? string.Empty : Path.GetFullPath(options.PluginConfigFileName),
                }
                .Concat(options.ReferenceDlls.Select(path => "r:" + Path.GetFullPath(path)))
                .Concat(options.ReferencePath.Select(path => "rh:" + Path.GetFullPath(path)))
                .Concat(options.PluginHintPaths.Select(path => "ph:" + Path.GetFullPath(path))));

        private sealed class Entry
        {
            public Entry(string optionsKey, Builder builder)
            {
                this.OptionsKey = optionsKey;
                this.Builder = builder;
            }

            public string OptionsKey { get; }

            public Builder Builder { get; }

            public long LastUsedMs { get; set; }
        }
    }
}
