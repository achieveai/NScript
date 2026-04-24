// -----------------------------------------------------------------------
// <copyright file="TestStartup.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.Test
{
    using System;
    using System.Threading;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Program/Startup hybrid used as <c>TEntryPoint</c> for
    /// <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/>
    /// in the SourceMap.Server end-to-end test suite. The type must be
    /// <c>public</c> so the factory can locate it via reflection.
    /// </summary>
    /// <remarks>
    /// Each test swaps its own <see cref="SourceMapFileHandlerOptions"/> onto
    /// <see cref="CurrentOptions"/> in <c>TestInitialize</c>, then constructs a
    /// fresh <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/>.
    /// <c>Main</c> pulls the latched options at request time, so each factory
    /// instance sees the options its test configured.
    /// </remarks>
    public class TestStartup
    {
        private static SourceMapFileHandlerOptions currentOptions;

        /// <summary>
        /// Options used by the endpoint on the next pipeline build. Swapped
        /// atomically so race conditions between parallel test runs cannot
        /// leak configuration across factories.
        /// </summary>
        public static SourceMapFileHandlerOptions CurrentOptions
        {
            get => Volatile.Read(ref currentOptions);
            set => Volatile.Write(ref currentOptions, value);
        }

        /// <summary>
        /// Route prefix used when registering the handler. Must match what the
        /// tests issue HTTP requests against.
        /// </summary>
        public const string PathPrefix = "/sourcemap";

        /// <summary>
        /// Entry point invoked by <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/>.
        /// Kept minimal: routing + the one endpoint under test, nothing else.
        /// </summary>
        /// <param name="args"> Process args (unused). </param>
        public static void Main(string[] args)
        {
            // WebApplicationFactory probes the entry assembly for its content
            // root; set it explicitly so `WebApplication.CreateBuilder` does
            // not try to locate wwwroot relative to a non-existent directory
            // built from the assembly's simple name.
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory,
            });
            var app = builder.Build();
            var options = CurrentOptions ?? new SourceMapFileHandlerOptions();
            app.MapSourceMapFiles(PathPrefix, options);
            app.Run();
        }
    }

    /// <summary>
    /// Thin wrapper around <see cref="WebApplicationFactory{TEntryPoint}"/>
    /// that pins the content root to <see cref="AppContext.BaseDirectory"/>.
    /// </summary>
    /// <remarks>
    /// The default content-root discovery logic of the factory prefers the
    /// solution-relative path derived from the entry-point type's simple
    /// assembly name, which does not exist in our layout. Hosting the app in
    /// the test bin directory keeps fixtures resolvable via
    /// <c>AppContext.BaseDirectory</c> and matches the path used by the
    /// production <c>SourceMapFileHandler</c> when deployed.
    /// </remarks>
    public sealed class SourceMapServerFactory : WebApplicationFactory<TestStartup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            // Returning null forces the factory to build the host via the
            // Main-invocation path (HostFactoryResolver), where our
            // WebApplicationOptions.ContentRootPath takes effect.
            return null;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(AppContext.BaseDirectory);
        }
    }
}
