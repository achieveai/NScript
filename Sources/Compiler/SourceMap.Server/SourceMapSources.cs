// -----------------------------------------------------------------------
// <copyright file="SourceMapSources.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Parsed view of the <c>sources</c> and <c>sourcesLong</c> arrays from an NScript-generated
    /// <c>.map</c> file. Used by the ASP.NET Core source handler to resolve short source names
    /// referenced by browser DevTools back to the absolute file path the compiler recorded.
    /// </summary>
    public sealed class SourceMapSources
    {
        private readonly Dictionary<string, string> shortToLong;
        private readonly string sourceRoot;

        private SourceMapSources(Dictionary<string, string> shortToLong, string sourceRoot)
        {
            this.shortToLong = shortToLong;
            this.sourceRoot = sourceRoot;
        }

        /// <summary>
        /// Gets the short source names recorded in the map's <c>sources</c> array
        /// (e.g. <c>"C$/Sources/.../MyFile.cs"</c>).
        /// </summary>
        public IReadOnlyCollection<string> ShortNames => this.shortToLong.Keys;

        /// <summary>
        /// Gets the <c>sourceRoot</c> string recorded in the map, or null when the field is
        /// absent. The handler's repo-URL redirect branch uses this together with a
        /// resolved short name to redirect DevTools at <c>{sourceRoot}{sources[i]}</c>.
        /// </summary>
        public string SourceRoot => this.sourceRoot;

        /// <summary>
        /// Parses the <c>sources</c>/<c>sourcesLong</c> arrays out of a source map file.
        /// Returns <c>null</c> when the file cannot be read or the expected arrays are missing —
        /// callers should treat that as a 404 (map not servable).
        /// </summary>
        /// <param name="mapFilePath"> Absolute or relative path to the <c>.map</c> file. </param>
        /// <returns> A <see cref="SourceMapSources"/> instance, or <c>null</c> on failure. </returns>
        public static SourceMapSources TryParse(string mapFilePath)
        {
            if (string.IsNullOrEmpty(mapFilePath) || !File.Exists(mapFilePath))
            {
                return null;
            }

            string content;
            try
            {
                content = File.ReadAllText(mapFilePath);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }

            return TryParseContent(content);
        }

        /// <summary>
        /// Async variant of <see cref="TryParse(string)"/> with an explicit byte-size cap. Files
        /// larger than <paramref name="maxBytes"/> are rejected without being fully read — this is
        /// the DoS guard for a request-scoped parse.
        /// </summary>
        /// <param name="mapFilePath"> Absolute or relative path to the <c>.map</c> file. </param>
        /// <param name="maxBytes"> Upper bound on the file's byte length. Files exceeding this are refused. </param>
        /// <param name="cancellationToken"> Cancellation token (e.g. <c>HttpContext.RequestAborted</c>). </param>
        /// <returns> A <see cref="SourceMapSources"/> instance, or <c>null</c> on failure. </returns>
        public static async Task<SourceMapSources> TryParseAsync(
            string mapFilePath,
            long maxBytes,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(mapFilePath))
            {
                return null;
            }

            FileInfo info;
            try
            {
                info = new FileInfo(mapFilePath);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (PathTooLongException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (SecurityException)
            {
                return null;
            }

            if (!info.Exists || info.Length > maxBytes)
            {
                return null;
            }

            string content;
            try
            {
                await using var stream = new FileStream(
                    mapFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    options: FileOptions.Asynchronous | FileOptions.SequentialScan);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (SecurityException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }

            return TryParseContent(content);
        }

        /// <summary>
        /// Parses a source-map JSON payload in-memory. Exposed for tests and for callers that
        /// already have the map content loaded.
        /// </summary>
        /// <param name="mapJson"> The full <c>.map</c> JSON payload. </param>
        /// <returns> A <see cref="SourceMapSources"/> instance, or <c>null</c> when the payload
        /// is not valid JSON or lacks the expected <c>sources</c>/<c>sourcesLong</c> arrays. </returns>
        public static SourceMapSources TryParseContent(string mapJson)
        {
            if (string.IsNullOrWhiteSpace(mapJson))
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(mapJson);
                var root = doc.RootElement;

                if (!root.TryGetProperty("sources", out var sources)
                    || sources.ValueKind != JsonValueKind.Array)
                {
                    return null;
                }

                string parsedSourceRoot = null;
                if (root.TryGetProperty("sourceRoot", out var srEl)
                    && srEl.ValueKind == JsonValueKind.String)
                {
                    parsedSourceRoot = srEl.GetString();
                }

                // "sourcesLong" is optional (non-standard NScript extension). When absent, the
                // short source names ARE the resolvable paths — e.g. when the map was generated
                // with a repo-URL sourceRoot rather than local disk paths.
                JsonElement longs = default;
                bool hasLongs = root.TryGetProperty("sourcesLong", out longs)
                    && longs.ValueKind == JsonValueKind.Array;

                var map = new Dictionary<string, string>(StringComparer.Ordinal);
                for (int i = 0; i < sources.GetArrayLength(); i++)
                {
                    string shortName = sources[i].GetString();
                    if (string.IsNullOrEmpty(shortName))
                    {
                        continue;
                    }

                    string longName = shortName;
                    if (hasLongs && i < longs.GetArrayLength())
                    {
                        var longEl = longs[i];
                        if (longEl.ValueKind == JsonValueKind.String)
                        {
                            longName = longEl.GetString() ?? shortName;
                        }
                    }

                    map[shortName] = longName;
                }

                return map.Count == 0 ? null : new SourceMapSources(map, parsedSourceRoot);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Resolves a short source name (the value browser DevTools requests) to its long/absolute
        /// file path, as recorded in the <c>sourcesLong</c> array. Returns <c>null</c> when no match.
        /// </summary>
        /// <param name="shortName"> The short source name from the map's <c>sources</c> array. </param>
        public string ResolveLongPath(string shortName)
        {
            if (string.IsNullOrEmpty(shortName))
            {
                return null;
            }

            return this.shortToLong.TryGetValue(shortName, out var longName) ? longName : null;
        }
    }
}
