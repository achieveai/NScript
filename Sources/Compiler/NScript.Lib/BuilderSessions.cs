namespace NScript.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
        private static readonly Dictionary<string, (string optionsKey, Builder builder)> Sessions =
            new Dictionary<string, (string optionsKey, Builder builder)>();

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
                if (!Sessions.TryGetValue(outputKey, out var entry) || entry.optionsKey != optionsKey)
                {
                    entry.builder?.Dispose();
                    entry = (optionsKey, createBuilder());
                    Sessions[outputKey] = entry;
                }

                return entry.builder.Execute(plugins);
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
    }
}
