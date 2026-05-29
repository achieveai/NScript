// -----------------------------------------------------------------------
// <copyright file="EndpointRouteBuilderExtensions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Extensions
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Sunlight.Logging.Server.Hosting;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// Endpoint mapping helpers for the Sunlight log ingestion stack.
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Default WebSocket sub-path under the host's root. The HTTP
        /// controller lives at <c>/_log</c> (set by
        /// <see cref="Hosting.LogController"/>'s route attribute); this
        /// extension only owns the WS endpoint.
        /// </summary>
        public const string DefaultWebSocketPath = "/_log/ws";

        /// <summary>
        /// Map controllers + a WebSocket endpoint that forwards each
        /// frame through <see cref="WebSocketLogHandler.RunAsync"/>.
        /// Consumers that want their own WS endpoint should skip this and
        /// wire <c>WebSocketLogProtocol.HandleFrameAsync</c> directly
        /// from their own handler.
        /// </summary>
        /// <param name="endpoints">Endpoint route builder.</param>
        /// <param name="webSocketPath">
        /// Path the WebSocket endpoint listens on. Defaults to
        /// <see cref="DefaultWebSocketPath"/>.
        /// </param>
        public static IEndpointRouteBuilder MapSunlightLogIngestion(
            this IEndpointRouteBuilder endpoints,
            string? webSocketPath = null)
        {
            if (endpoints == null) { throw new ArgumentNullException(nameof(endpoints)); }

            // The LogController is wired by the consumer's AddControllers
            // pipeline (we used AddApplicationPart in AddSunlightLogIngestion).
            // We need to make sure MapControllers is called — if the
            // consumer hasn't yet, doing it here keeps the API one-stop.
            endpoints.MapControllers();

            var wsPath = string.IsNullOrEmpty(webSocketPath) ? DefaultWebSocketPath : webSocketPath;
            endpoints.Map(wsPath, async (HttpContext ctx) =>
            {
                if (!ctx.WebSockets.IsWebSocketRequest)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var service = ctx.RequestServices.GetRequiredService<ILogIngestionService>();
                using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
                await WebSocketLogHandler.RunAsync(socket, service, ctx.RequestAborted);
            });

            return endpoints;
        }
    }
}
