// -----------------------------------------------------------------------
// <copyright file="FixtureMapEmitter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Builds a compiler-emitted <c>.map</c> + companion <c>.js</c> pair for the
    /// SourceMap.Server end-to-end test suite. The map is produced by the real
    /// <see cref="OwaSourceMapper.SourceMap"/> class (the same type the NScript
    /// compiler uses) so the tests exercise the V3 JSON shape the handler will
    /// encounter in production.
    /// </summary>
    /// <remarks>
    /// Kept deliberately free of any MSTest dependency so it can be reused from
    /// the <c>SourceMap.Server.TestHost</c> console entry point that backs the
    /// Playwright browser test. The helper only needs:
    /// <list type="bullet">
    ///   <item>A set of fixture source files on disk.</item>
    ///   <item>A target output directory for the <c>.map</c> + <c>.js</c>.</item>
    ///   <item>An optional <c>sourceRoot</c> to embed in the map (defaults to a
    ///     relative URL so the fixture is portable between <c>WebApplicationFactory</c>
    ///     and a real Kestrel host).</item>
    /// </list>
    /// Designed to stay forward-compatible with WI-19 (repo-linked source maps):
    /// callers can pass any <c>sourceRoot</c> — including a GitHub or Azure DevOps
    /// raw-file URL — and the emitter will thread it through without change.
    /// </remarks>
    public static class FixtureMapEmitter
    {
        /// <summary>
        /// Default map name used by the E2E suite. Matches the URL segment in
        /// <c>/sourcemap/{mapName}/{shortName}</c>.
        /// </summary>
        public const string DefaultMapName = "app";

        /// <summary>
        /// Emits a <c>{mapName}.map</c> plus a small companion <c>{mapName}.js</c>
        /// into <paramref name="outputDir"/>. The <c>.js</c> contains a single
        /// function body and a <c>//# sourceMappingURL=</c> comment pointing at
        /// the emitted map so a browser loading the JS will fetch the map.
        /// </summary>
        /// <param name="mapName">
        /// URL-safe map name (used for both file stems and the <c>sources[]</c>
        /// short-name layout). Must satisfy the handler's <c>mapName</c> regex.
        /// </param>
        /// <param name="outputDir">
        /// Directory into which <c>{mapName}.map</c> and <c>{mapName}.js</c> are
        /// written. Created if it does not exist.
        /// </param>
        /// <param name="fixtureFiles">
        /// Absolute paths to the fixture source files that should be recorded in
        /// the map's <c>sources</c> / <c>sourcesLong</c> arrays. Each file gets
        /// one (0, i*8) → (0, 0) mapping anchor — enough for DevTools to resolve a
        /// <c>Debugger.setBreakpointByUrl</c> against the source URL.
        /// </param>
        /// <param name="sourceRoot">
        /// Optional value for the map's <c>sourceRoot</c>. When null or empty, the
        /// handler contract dictates the legacy <c>{file}.ashx</c> fallback; when
        /// set to a URL (e.g. <c>"/sourcemap/app"</c> or
        /// <c>"https://raw.githubusercontent.com/owner/repo/sha/"</c>), the emitted
        /// map's <c>sourceRoot</c> is wired to that value.
        /// </param>
        /// <param name="emitLegacyAshxHandler">
        /// Whether to keep the legacy <c>SrcMapper.ashx</c> sidecar alongside the
        /// <c>.map</c>. Defaults to <c>false</c> for the E2E suite since the new
        /// handler replaces the legacy WebForms one.
        /// </param>
        /// <returns>
        /// The emitted artifacts — full paths to the <c>.map</c> and <c>.js</c>,
        /// plus the short-name → long-path dictionary the map records.
        /// </returns>
        public static EmittedMap Emit(
            string mapName,
            string outputDir,
            IReadOnlyList<string> fixtureFiles,
            string sourceRoot = null,
            bool emitLegacyAshxHandler = false)
        {
            if (string.IsNullOrEmpty(mapName))
            {
                throw new ArgumentException("mapName must be non-empty", nameof(mapName));
            }

            if (string.IsNullOrEmpty(outputDir))
            {
                throw new ArgumentException("outputDir must be non-empty", nameof(outputDir));
            }

            if (fixtureFiles == null || fixtureFiles.Count == 0)
            {
                throw new ArgumentException("At least one fixture file is required", nameof(fixtureFiles));
            }

            Directory.CreateDirectory(outputDir);

            var map = new OwaSourceMapper.SourceMap
            {
                File = mapName + ".js",
                EmitLegacyAshxHandler = emitLegacyAshxHandler,
            };

            if (!string.IsNullOrEmpty(sourceRoot))
            {
                map.SourceRoot = sourceRoot;
            }

            // Each fixture gets one deterministic anchor mapping at (1, 1) in the
            // generated JS pointing to (1, 1) in the source. The URL-safe short
            // name is computed the same way SourceMap.ToString() does (":" → "$",
            // "\" → "/") so tests can predict what appears in the sources array.
            var shortLongPairs = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < fixtureFiles.Count; i++)
            {
                string absolute = Path.GetFullPath(fixtureFiles[i]);
                map.AddMapping(
                    sLine: 0,
                    sCol: i * 8,
                    tLine: 0,
                    tCol: 0,
                    file: absolute);

                string shortName = ToShortName(absolute);
                shortLongPairs[shortName] = absolute;
            }

            map.Write(outputDir);

            string mapPath = Path.Combine(outputDir, mapName + ".map");
            string jsPath = Path.Combine(outputDir, mapName + ".js");

            // Compute the sourceMappingURL. The handler resolves maps by map name
            // alone; the companion .js only needs a URL that returns the right
            // .map bytes when fetched by the browser. When sourceRoot is a path,
            // we anchor the mapping URL relative to the .js so a host serving the
            // .js from "/app.js" resolves the map from "/app.map" at the root.
            string mappingUrl = mapName + ".map";
            File.WriteAllText(
                jsPath,
                BuildFixtureJs(mapName, fixtureFiles.Count, mappingUrl));

            return new EmittedMap(mapPath, jsPath, shortLongPairs);
        }

        /// <summary>
        /// Mirrors <see cref="OwaSourceMapper.SourceMap.ToString"/>'s short-name
        /// computation so tests can produce the exact URL segment the handler
        /// will match against the map's <c>sources</c> array.
        /// </summary>
        public static string ToShortName(string absolutePath)
        {
            return Path.GetFullPath(absolutePath).Replace(":", "$").Replace("\\", "/");
        }

        private static string BuildFixtureJs(string mapName, int fixtureCount, string mappingUrl)
        {
            // The body is intentionally tiny. It exists only so Chromium has
            // something to parse and a sourceMappingURL to follow.
            var sb = new System.Text.StringBuilder();
            sb.Append("// SourceMap.Server E2E fixture — generated by FixtureMapEmitter\n");
            sb.Append("(function fixtureEntry(){\n");
            sb.Append("  var count = ").Append(fixtureCount).Append(";\n");
            sb.Append("  return count;\n");
            sb.Append("})();\n");
            sb.Append("//# sourceMappingURL=").Append(mappingUrl).Append('\n');
            return sb.ToString();
        }

        /// <summary>
        /// Result of a fixture emission.
        /// </summary>
        public sealed class EmittedMap
        {
            internal EmittedMap(string mapPath, string jsPath, IReadOnlyDictionary<string, string> shortToLong)
            {
                this.MapPath = mapPath;
                this.JsPath = jsPath;
                this.ShortToLong = shortToLong;
            }

            /// <summary> Absolute path to the emitted <c>.map</c> file. </summary>
            public string MapPath { get; }

            /// <summary> Absolute path to the emitted companion <c>.js</c> file. </summary>
            public string JsPath { get; }

            /// <summary>
            /// URL-safe short name → absolute long path, mirroring the map's
            /// <c>sources</c> / <c>sourcesLong</c> arrays.
            /// </summary>
            public IReadOnlyDictionary<string, string> ShortToLong { get; }
        }
    }
}
