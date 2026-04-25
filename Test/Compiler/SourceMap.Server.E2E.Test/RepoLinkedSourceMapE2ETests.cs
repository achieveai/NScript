// -----------------------------------------------------------------------
// <copyright file="RepoLinkedSourceMapE2ETests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.E2E.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Playwright;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Browser-driven validation of the WI-19 repo-linked source map flow. Two Kestrel hosts
    /// stand in for the production topology:
    /// <list type="bullet">
    ///   <item><description>The "app" host serves a tiny HTML+JS page along with a
    ///   <c>.map</c> file whose <c>sourceRoot</c> points at the second host.</description></item>
    ///   <item><description>The "repo" host plays the role of <c>raw.githubusercontent.com</c>
    ///   / <c>dev.azure.com/_apis/git/items</c> — it answers GET requests for the original
    ///   source files.</description></item>
    /// </list>
    /// Chromium loads the page, then the test exercises the same end-to-end flow DevTools
    /// would walk through: fetching the <c>.map</c>, reading its <c>sourceRoot</c>, and
    /// fetching the original source from the repo host. Together these confirm a deployed
    /// build with a repo-linked map round-trips through a real browser HTTP stack — the
    /// piece local unit tests can only stub.
    /// </summary>
    /// <remarks>
    /// Gated behind the <c>ENABLE_PLAYWRIGHT_E2E=true</c> environment variable so normal
    /// CI runs (which don't have Playwright browsers installed) don't pay the cost.
    /// To run locally:
    /// <code>
    ///   pwsh -Command "&amp; { Test/Compiler/SourceMap.Server.E2E.Test/bin/Debug/net8.0/playwright.ps1 install chromium }"
    ///   $env:ENABLE_PLAYWRIGHT_E2E = "true"
    ///   dotnet test Test/Compiler/SourceMap.Server.E2E.Test/SourceMap.Server.E2E.Test.csproj
    /// </code>
    /// </remarks>
    [TestClass]
    public class RepoLinkedSourceMapE2ETests
    {
        private const string E2EEnvFlag = "ENABLE_PLAYWRIGHT_E2E";
        private const string SourceFileName = "Program.cs";
        private const string SourceFileBody = "// repo-hosted Program.cs\nclass Foo { void Bar() {} }\n";

        private IHost appHost;
        private IHost repoHost;
        private string appBaseUrl;
        private string repoBaseUrl;
        private IPlaywright playwright;
        private IBrowser browser;

        [TestInitialize]
        public async Task InitAsync()
        {
            if (!IsE2EEnabled())
            {
                return;
            }

            // Bind both hosts to ephemeral ports — we read back the actual URL from server features
            // after start so that nothing in the test hard-codes :5000 / :5001 (which would clash
            // with anyone else running Kestrel locally).
            this.repoHost = BuildRepoHost();
            await this.repoHost.StartAsync();
            this.repoBaseUrl = ExtractFirstAddress(this.repoHost);

            this.appHost = BuildAppHost(this.repoBaseUrl);
            await this.appHost.StartAsync();
            this.appBaseUrl = ExtractFirstAddress(this.appHost);

            this.playwright = await Playwright.CreateAsync();
            this.browser = await this.playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
            });
        }

        [TestCleanup]
        public async Task CleanupAsync()
        {
            if (this.browser != null) await this.browser.CloseAsync();
            this.playwright?.Dispose();
            if (this.appHost != null) { await this.appHost.StopAsync(); this.appHost.Dispose(); }
            if (this.repoHost != null) { await this.repoHost.StopAsync(); this.repoHost.Dispose(); }
        }

        [TestMethod]
        public async Task DevTools_FetchesOriginalSource_FromRepoSourceRoot()
        {
            if (!IsE2EEnabled())
            {
                Assert.Inconclusive($"Set {E2EEnvFlag}=true to run Playwright E2E tests.");
                return;
            }

            var ctx = await this.browser.NewContextAsync();
            var page = await ctx.NewPageAsync();

            await page.GotoAsync(this.appBaseUrl + "/index.html");
            await page.WaitForFunctionAsync("() => typeof window.__appLoaded !== 'undefined'");

            // Replay the fetch sequence DevTools performs when "Resolve original source" is
            // engaged: pull the .map, read its sourceRoot + sources[i], and fetch the original
            // file through the repo host. APIRequestContext shares the browser's network stack
            // (cookie jar, redirects, TLS settings), so this matches what a real DevTools session
            // would experience.
            var mapResp = await ctx.APIRequest.GetAsync(this.appBaseUrl + "/app.js.map");
            Assert.AreEqual((int)HttpStatusCode.OK, mapResp.Status);
            string mapJson = await mapResp.TextAsync();
            StringAssert.Contains(mapJson, this.repoBaseUrl, "Map's sourceRoot must point at the repo host");
            StringAssert.Contains(mapJson, SourceFileName);

            var srcResp = await ctx.APIRequest.GetAsync(this.repoBaseUrl + "/" + SourceFileName);
            Assert.AreEqual((int)HttpStatusCode.OK, srcResp.Status);
            string body = await srcResp.TextAsync();
            StringAssert.Contains(body, "class Foo", "Repo host must serve the original C# source body");
        }

        private static bool IsE2EEnabled()
        {
            return string.Equals(
                Environment.GetEnvironmentVariable(E2EEnvFlag),
                "true",
                StringComparison.OrdinalIgnoreCase);
        }

        private static string ExtractFirstAddress(IHost host)
        {
            var server = host.Services.GetService(typeof(Microsoft.AspNetCore.Hosting.Server.IServer))
                as Microsoft.AspNetCore.Hosting.Server.IServer;
            var feature = server?.Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();
            return feature?.Addresses?.FirstOrDefault()
                ?? throw new InvalidOperationException("Kestrel didn't expose any bound addresses");
        }

        private static IHost BuildRepoHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(web =>
                {
                    web.UseKestrel(opts => opts.ListenLocalhost(0));
                    web.Configure(app =>
                    {
                        app.Run(async ctx =>
                        {
                            // Naive raw-file responder — the path component after the leading slash
                            // is treated as the file name. Only the test fixture's known file is
                            // served; everything else 404s.
                            string requested = ctx.Request.Path.Value?.TrimStart('/') ?? string.Empty;
                            if (requested == SourceFileName)
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/plain; charset=utf-8";
                                await ctx.Response.WriteAsync(SourceFileBody, Encoding.UTF8);
                                return;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        });
                    });
                })
                .Build();
        }

        private static IHost BuildAppHost(string repoBaseUrl)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(web =>
                {
                    web.UseKestrel(opts => opts.ListenLocalhost(0));
                    web.Configure(app =>
                    {
                        app.Run(async ctx =>
                        {
                            string path = ctx.Request.Path.Value ?? string.Empty;
                            if (path == "/" || path == "/index.html")
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/html; charset=utf-8";
                                await ctx.Response.WriteAsync(
                                    "<!DOCTYPE html><html><body>"
                                    + "<script src=\"/app.js\"></script>"
                                    + "</body></html>",
                                    Encoding.UTF8);
                                return;
                            }

                            if (path == "/app.js")
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "application/javascript; charset=utf-8";
                                await ctx.Response.WriteAsync(
                                    "window.__appLoaded = true;\n"
                                    + "//# sourceMappingURL=/app.js.map\n",
                                    Encoding.UTF8);
                                return;
                            }

                            if (path == "/app.js.map")
                            {
                                // Repo-linked map: sourceRoot points at the second host, sources[]
                                // contains the repo-relative short name DevTools will append.
                                string mapJson = "{"
                                    + "\"version\":3,"
                                    + "\"file\":\"app.js\","
                                    + "\"sourceRoot\":\"" + repoBaseUrl.TrimEnd('/') + "/\","
                                    + "\"sources\":[\"" + SourceFileName + "\"],"
                                    + "\"names\":[],"
                                    + "\"mappings\":\"\""
                                    + "}";
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "application/json; charset=utf-8";
                                await ctx.Response.WriteAsync(mapJson, Encoding.UTF8);
                                return;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        });
                    });
                })
                .Build();
        }
    }
}
