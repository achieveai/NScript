// -----------------------------------------------------------------------
// <copyright file="SourceMapFileHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server
{
    using System;
    using System.IO;
    using System.Net;
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
            if (mapName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || mapName.Contains("..", StringComparison.Ordinal))
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

            string mapPath = Path.Combine(mapsDir, mapName + ".map");
            var sources = SourceMapSources.TryParse(mapPath);
            if (sources == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string longPath = sources.ResolveLongPath(sourceName);
            if (longPath == null || !options.ServeFromSourcesLong)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!File.Exists(longPath))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            ctx.Response.ContentType = "text/plain; charset=utf-8";
            await using var stream = File.OpenRead(longPath);
            await stream.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
        }
    }
}
