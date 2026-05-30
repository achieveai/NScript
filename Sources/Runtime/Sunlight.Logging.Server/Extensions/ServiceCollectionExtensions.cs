// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Extensions
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sunlight.Logging.Server.Hosting;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// DI registration extensions for the Sunlight log ingestion stack.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register <see cref="ILogIngestionService"/> as a singleton and
        /// add the <see cref="LogController"/> ApplicationPart so the
        /// HTTP route is discovered by the host's controllers.
        /// </summary>
        /// <param name="services">DI container.</param>
        /// <param name="configure">Optional options configuration.</param>
        /// <returns>The same <paramref name="services"/> for chaining.</returns>
        /// <remarks>
        /// Calling <c>AddControllers</c> is idempotent — safe to call from
        /// consumer code that already has its own controllers. The
        /// ApplicationPart is what makes <see cref="LogController"/>
        /// discoverable when this assembly is referenced as a NuGet
        /// package and the consumer's <c>AddControllers</c> scan would
        /// otherwise miss it.
        /// </remarks>
        public static IServiceCollection AddSunlightLogIngestion(
            this IServiceCollection services,
            Action<LogIngestionOptions>? configure = null)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }

            services.AddOptions();
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.AddSingleton<ILogIngestionService, LogIngestionService>();

            // ApplicationPart wires the controller in even when the host
            // didn't reference our assembly's controllers via assembly
            // scanning.
            services
                .AddControllers()
                .AddApplicationPart(typeof(LogController).GetTypeInfo().Assembly);

            return services;
        }
    }
}
