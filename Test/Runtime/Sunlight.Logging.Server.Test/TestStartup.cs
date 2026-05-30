// -----------------------------------------------------------------------
// <copyright file="TestStartup.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Test
{
    using System;
    using System.Threading;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Sunlight.Logging.Server.Extensions;

    /// <summary>
    /// Program/Startup hybrid used as <c>TEntryPoint</c> for
    /// <see cref="WebApplicationFactory{TEntryPoint}"/>. Wires the
    /// Sunlight log ingestion stack plus a per-test
    /// <see cref="TestLoggerProvider"/> latched into
    /// <see cref="CurrentProvider"/> so assertions can inspect what
    /// landed in MEL.
    /// </summary>
    public class TestStartup
    {
        private static TestLoggerProvider? currentProvider;

        public static TestLoggerProvider? CurrentProvider
        {
            get => Volatile.Read(ref currentProvider);
            set => Volatile.Write(ref currentProvider, value);
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory,
            });

            builder.Logging.ClearProviders();
            var provider = CurrentProvider;
            if (provider != null)
            {
                builder.Logging.AddProvider(provider);
                builder.Logging.SetMinimumLevel(LogLevel.Trace);
            }

            builder.Services.AddSunlightLogIngestion();

            var app = builder.Build();
            app.UseWebSockets();
            app.MapSunlightLogIngestion();
            app.Run();
        }
    }

    /// <summary>
    /// Pins the content root so the factory's default discovery doesn't
    /// drift into a missing path.
    /// </summary>
    public sealed class SunlightLoggingServerFactory : WebApplicationFactory<TestStartup>
    {
        protected override IHostBuilder? CreateHostBuilder() => null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(AppContext.BaseDirectory);
        }
    }
}
