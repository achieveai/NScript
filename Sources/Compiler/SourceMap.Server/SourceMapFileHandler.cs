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
    using System.Linq;
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

            // Pre-resolve static configuration once at registration. MapsDirectory / AllowedSourceRoots
            // are derived from startup options — recomputing them per request wastes syscalls
            // (Path.GetFullPath, Directory.Exists) and GC allocations under concurrent DevTools load.
            // Resolution is deferred (not thrown) so misconfiguration still surfaces as a 404 rather
            // than crashing the host at startup; null/empty values are reported on first request.
            string resolvedMapsDir = options.ResolveMapsDirectory();
            IReadOnlyList<string> resolvedAllowedRoots = resolvedMapsDir != null
                ? options.ResolveAllowedRoots(resolvedMapsDir)
                : null;

            return endpoints.MapGet(
                pattern,
                (HttpContext ctx, string mapName, string sourceName) =>
                    HandleAsync(ctx, mapName, sourceName, options, resolvedMapsDir, resolvedAllowedRoots));
        }

        /// <summary>
        /// Core request handler. Exposed internally so the test project can drive it without
        /// standing up a full HTTP pipeline. Tests may pass <c>null</c> for the pre-resolved
        /// directory / allow-list to force per-request resolution (matches the behaviour of
        /// registration-time misconfiguration).
        /// </summary>
        internal static Task HandleAsync(
            HttpContext ctx,
            string mapName,
            string sourceName,
            SourceMapFileHandlerOptions options)
        {
            return HandleAsync(ctx, mapName, sourceName, options, preResolvedMapsDir: null, preResolvedAllowedRoots: null);
        }

        private static async Task HandleAsync(
            HttpContext ctx,
            string mapName,
            string sourceName,
            SourceMapFileHandlerOptions options,
            string preResolvedMapsDir,
            IReadOnlyList<string> preResolvedAllowedRoots)
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

            string mapsDir = preResolvedMapsDir ?? options.ResolveMapsDirectory();
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

            // Parse the map up-front. Both the local-file branch and the repo-redirect fallback
            // need its sources/sourceRoot data, so a single parse keeps them aligned and avoids
            // repeated disk I/O.
            SourceMapSources sources = null;
            if (TryGetFileSize(mapPath, out long mapSize) && mapSize <= options.MaxMapFileSizeBytes)
            {
                sources = await SourceMapSources.TryParseAsync(mapPath, options.MaxMapFileSizeBytes, ctx.RequestAborted);
            }

            if (!options.ServeFromSourcesLong)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }

            if (sources == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string longPath = sources.ResolveLongPath(sourceName);
            if (longPath == null)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }

            string resolvedLongPath;
            try
            {
                resolvedLongPath = Path.GetFullPath(longPath);
            }
            catch (ArgumentException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }
            catch (PathTooLongException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }
            catch (NotSupportedException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }

            // Allow-list check: the resolved path MUST live under one of the configured source
            // roots. Without this, a map that points at /etc/passwd via sourcesLong would be
            // served as-is — the primary defence against tampered or attacker-controlled maps.
            var allowedRoots = preResolvedAllowedRoots ?? options.ResolveAllowedRoots(mapsDir);
            if (!IsContainedInAny(resolvedLongPath, allowedRoots))
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }

            FileInfo info;
            try
            {
                info = new FileInfo(resolvedLongPath);
            }
            catch (ArgumentException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }
            catch (PathTooLongException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }
            catch (NotSupportedException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }

            if (!info.Exists)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
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
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                await NotFoundOrRedirectAsync(ctx, sources, sourceName, options);
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

        /// <summary>
        /// When the caller has decided the local lookup must fail, optionally consult the
        /// repo-URL redirect branch before returning <c>404</c>. The redirect only fires when
        /// <paramref name="options"/>.<see cref="SourceMapFileHandlerOptions.RepoUrlRedirectOnMiss"/>
        /// is true AND the parsed map's <c>sourceRoot</c> is an http(s) URL AND the requested
        /// short name exists in the map's <c>sources</c> array.
        /// </summary>
        private static Task NotFoundOrRedirectAsync(
            HttpContext ctx,
            SourceMapSources sources,
            string sourceName,
            SourceMapFileHandlerOptions options)
        {
            if (TryBuildRepoRedirect(sources, sourceName, options, out string redirectUrl))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Found;
                ctx.Response.Headers["Location"] = redirectUrl;
                return Task.CompletedTask;
            }

            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Builds a redirect URL to the repo-hosted source when redirect is opted-in and the
        /// parsed map carries an http(s) <c>sourceRoot</c>. Returns false (and leaves
        /// <paramref name="redirectUrl"/> null) when the redirect should NOT fire — the caller
        /// then falls through to a plain 404.
        /// </summary>
        internal static bool TryBuildRepoRedirect(
            SourceMapSources sources,
            string sourceName,
            SourceMapFileHandlerOptions options,
            out string redirectUrl)
        {
            redirectUrl = null;

            if (options == null || !options.RepoUrlRedirectOnMiss)
            {
                return false;
            }

            if (sources == null || string.IsNullOrEmpty(sourceName))
            {
                return false;
            }

            // The short name MUST exist in the parsed map; without this check, a 302 would be
            // emitted for any path the attacker dangles after the prefix. ContainsName is the
            // contract: "the compiler emitted a `sources[]` entry exactly equal to this string".
            if (!sources.ShortNames.Contains(sourceName))
            {
                return false;
            }

            string root = sources.SourceRoot;
            if (string.IsNullOrEmpty(root))
            {
                return false;
            }

            // Refuse to redirect to anything other than http(s) — the legacy `{file}.ashx` root,
            // a relative path, or a hostile `javascript:`/`data:` URI would all be unsafe.
            if (!root.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !root.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // sourceRoot is documented as either being a directory prefix (trailing `/`) or
            // the immediate parent. Append literally — the compiler is responsible for shaping
            // a working URL ahead of time.
            redirectUrl = root + sourceName;
            return true;
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
