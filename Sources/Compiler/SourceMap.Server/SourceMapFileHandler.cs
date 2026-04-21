// -----------------------------------------------------------------------
// <copyright file="SourceMapFileHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// ASP.NET Core endpoint extensions that replace the legacy <c>SrcMapper.ashx</c>
    /// WebForms handler. The handler resolves requests of the form
    /// <c>/{prefix}/{mapName}/{sourceName}</c> against an NScript-generated source map
    /// and serves the original source file content back to browser DevTools.
    /// </summary>
    public static class SourceMapFileHandler
    {
        /// <summary>
        /// Whitelist of characters allowed in the <c>{mapName}</c> route segment. This is
        /// deliberately narrow: map names are produced by the NScript build and only ever
        /// contain file-safe characters, so anything outside this set must be a probe.
        /// </summary>
        private static readonly Regex MapNamePattern = new Regex(
            @"^[A-Za-z0-9_-][A-Za-z0-9._-]*$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Registers a minimal-API endpoint that serves the original source files referenced
        /// by an NScript-generated <c>.map</c> file. The route pattern is
        /// <c>{prefix}/{mapName}/{*sourceName}</c>; the handler looks up
        /// <c>{options.MapsDirectory}/{mapName}.map</c>, finds <paramref name="mapName"/>'s
        /// entry for the short <c>sourceName</c>, and streams the mapped file back.
        /// </summary>
        /// <param name="endpoints"> The endpoint route builder. </param>
        /// <param name="pathPrefix"> URL prefix for the handler (e.g. <c>"/sourcemap"</c>). </param>
        /// <param name="options"> Options describing where maps are located. </param>
        /// <returns> The endpoint builder so further conventions can be applied. </returns>
        public static IEndpointConventionBuilder MapSourceMapFiles(
            this IEndpointRouteBuilder endpoints,
            string pathPrefix,
            SourceMapFileHandlerOptions options)
        {
            if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(pathPrefix)) throw new ArgumentException("pathPrefix must be non-empty", nameof(pathPrefix));

            string trimmed = pathPrefix.TrimEnd('/');
            string pattern = trimmed + "/{mapName}/{*sourceName}";

            return endpoints.MapGet(
                pattern,
                (HttpContext ctx, string mapName, string sourceName) =>
                    HandleAsync(ctx, mapName, sourceName, options));
        }

        /// <summary>
        /// Core request handler. Exposed internally so the test project can drive it without
        /// standing up a full HTTP pipeline.
        /// </summary>
        internal static async Task HandleAsync(
            HttpContext ctx,
            string mapName,
            string sourceName,
            SourceMapFileHandlerOptions options)
        {
            if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(sourceName))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            // Guard against directory traversal in the map-name segment. sourceName is compared
            // against the in-map entries (safe: only pre-recorded paths match) so it doesn't need
            // a separate traversal check, but mapName is used to build a filesystem path.
            if (!MapNamePattern.IsMatch(mapName))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            string mapsDir = options.ResolveMapsDirectory();
            if (mapsDir == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            // Belt-and-braces containment check: even with the whitelist above, make sure the
            // resolved map path actually lives under mapsDir. Catches edge cases the regex might
            // miss (symlinks, case-folding collisions on non-Windows).
            string mapPath = Path.GetFullPath(Path.Combine(mapsDir, mapName + ".map"));
            if (!IsContained(mapPath, mapsDir))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            if (!options.ServeFromSourcesLong)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!TryGetFileSize(mapPath, out long mapSize) || mapSize > options.MaxMapFileSizeBytes)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var sources = await SourceMapSources.TryParseAsync(mapPath, options.MaxMapFileSizeBytes, ctx.RequestAborted);
            if (sources == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string longPath = sources.ResolveLongPath(sourceName);
            if (longPath == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string resolvedLongPath;
            try
            {
                resolvedLongPath = Path.GetFullPath(longPath);
            }
            catch (ArgumentException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (PathTooLongException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (NotSupportedException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            // Allow-list check: the resolved path MUST live under one of the configured source
            // roots. Without this, a map that points at /etc/passwd via sourcesLong would be
            // served as-is — the primary defence against tampered or attacker-controlled maps.
            var allowedRoots = options.ResolveAllowedRoots(mapsDir);
            if (!IsContainedInAny(resolvedLongPath, allowedRoots))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            FileInfo info;
            try
            {
                info = new FileInfo(resolvedLongPath);
            }
            catch (ArgumentException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (PathTooLongException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (NotSupportedException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!info.Exists)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            // Reparse points (symlinks / junctions) would let an allow-listed directory "escape"
            // its own tree. Rather than trying to follow the link and re-validate, refuse outright.
            if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (info.Length > options.MaxSourceFileSizeBytes)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            // Open the file BEFORE setting response headers so a failure here doesn't leak a
            // half-written response. TOCTOU between FileInfo.Exists and OpenRead is deliberate —
            // the FileStream.ctor will throw FileNotFoundException if the race loses, and we
            // translate that to 404 below.
            FileStream stream;
            try
            {
                stream = new FileStream(
                    resolvedLongPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    options: FileOptions.Asynchronous | FileOptions.SequentialScan);
            }
            catch (FileNotFoundException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (DirectoryNotFoundException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (UnauthorizedAccessException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            catch (IOException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            try
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/plain; charset=utf-8";
                await stream.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
            }
            catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
            {
                // Client disconnect — nothing to do; response has already started.
            }
            finally
            {
                await stream.DisposeAsync();
            }
        }

        private static bool TryGetFileSize(string path, out long size)
        {
            size = 0;
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists)
                {
                    return false;
                }

                size = info.Length;
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        private static bool IsContained(string candidatePath, string directory)
        {
            string normalizedDir = directory;
            if (!normalizedDir.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                && !normalizedDir.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                normalizedDir += Path.DirectorySeparatorChar;
            }

            StringComparison cmp = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            return candidatePath.StartsWith(normalizedDir, cmp);
        }

        private static bool IsContainedInAny(string candidatePath, IReadOnlyList<string> roots)
        {
            if (roots == null || roots.Count == 0)
            {
                return false;
            }

            foreach (var root in roots)
            {
                if (IsContained(candidatePath, root))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
