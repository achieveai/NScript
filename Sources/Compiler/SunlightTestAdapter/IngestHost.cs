namespace SunlightTestAdapter;

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Serilog.Extensions.Logging;
using Sunlight.Logging.Server.Extensions;

/// <summary>
/// In-process Kestrel host that boots one WebApplication serving BOTH
/// the test-bundle static assets (replacing the old Playwright
/// page.RouteAsync interception) AND the Sunlight log ingestion
/// controller + WebSocket endpoint, all on the same port.
/// </summary>
/// <remarks>
/// Gated by <see cref="Settings.LogEndpoint"/> being non-null. When unset,
/// the executor keeps today's Playwright-route static-serving path so
/// existing consumers see zero behavioral change.
/// </remarks>
public sealed class IngestHost : IAsyncDisposable
{
    // Embedded resource file names matching the Resources/ subfolder
    // already shipped with the adapter. index.html is built inline via
    // BuildIndexHtml() because it carries the per-host endpoint globals
    // that vary per run.
    private const string QunitJsResource = "qunit.js";
    private const string QunitCssResource = "qunit.css";

    private readonly WebApplication _app;
    private readonly Serilog.ILogger? _serilogRootForDispose;

    public string HttpBaseUrl { get; }
    public string WsBaseUrl { get; }
    public ILoggerFactory LoggerFactory { get; }

    private IngestHost(
        WebApplication app,
        string httpBaseUrl,
        string wsBaseUrl,
        ILoggerFactory loggerFactory,
        Serilog.ILogger? serilogRoot)
    {
        _app = app;
        HttpBaseUrl = httpBaseUrl;
        WsBaseUrl = wsBaseUrl;
        LoggerFactory = loggerFactory;
        _serilogRootForDispose = serilogRoot;
    }

    /// <summary>
    /// Build and start the host. <paramref name="settings.LogEndpoint"/>
    /// becomes the Serilog JSONL output path.
    /// </summary>
    public static async Task<IngestHost> CreateAsync(
        Settings settings,
        string jsBundlePath,
        IMessageLogger? messageLogger)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrEmpty(settings.LogEndpoint))
        {
            throw new InvalidOperationException(
                "IngestHost.CreateAsync requires Settings.LogEndpoint to be set. " +
                "Callers must gate on LogEndpoint != null and keep today's " +
                "Playwright-route path when null (per v2 D12).");
        }

        var builder = WebApplication.CreateBuilder();

        // Bind to a kernel-assigned port on the loopback. Reading the
        // bound port from IServerAddressesFeature after Start happens
        // below.
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        // Replace MEL providers with a Serilog file sink targeting the
        // configured JSONL path. The same factory is exposed to callers
        // so the adapter (Runner/Executor) can dogfood it and emit
        // SunlightTestAdapter.* events into the same file.
        var serilogRoot = SerilogLoggerConfig.BuildJsonlLogger(settings.LogEndpoint!);
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new SerilogLoggerProvider(serilogRoot, dispose: false));
        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        builder.Services.AddSunlightLogIngestion();

        var app = builder.Build();
        app.UseWebSockets();
        app.MapSunlightLogIngestion();

        // Embedded resources: index.html / qunit.js / qunit.css. We
        // resolve them lazily on first GET so an embedded-resource
        // miss surfaces as a 500 inside the request (with a clear
        // error) rather than a startup crash.
        app.MapGet("/index.html", () => Results.Content(
            BuildIndexHtml(),
            "text/html; charset=utf-8"));
        app.MapGet("/qunit.js", () => Results.Content(
            ReadEmbeddedResource(QunitJsResource),
            "application/javascript; charset=utf-8"));
        app.MapGet("/qunit.css", () => Results.Content(
            ReadEmbeddedResource(QunitCssResource),
            "text/css; charset=utf-8"));

        // The JS test bundle lives on disk (the adapter compiled it).
        // 404 with a clear body so a misconfigured JsFilePath surfaces
        // as a useful diagnostic instead of an empty page.
        app.MapGet("/test-bundle.js", async (HttpContext ctx) =>
        {
            if (!File.Exists(jsBundlePath))
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                await ctx.Response.WriteAsync($"Test bundle not found at: {jsBundlePath}");
                return;
            }
            ctx.Response.ContentType = "application/javascript; charset=utf-8";
            await ctx.Response.SendFileAsync(jsBundlePath);
        });

        await app.StartAsync();

        // Bound port discovery — UseUrls("…:0") gives the kernel a port,
        // we read it back from IServerAddressesFeature.
        var addresses = app.Services
            .GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
            .Features.Get<IServerAddressesFeature>();
        var httpBase = addresses?.Addresses.FirstOrDefault()
            ?? throw new InvalidOperationException("Kestrel did not surface a bound address");
        // Strip any trailing slash so callers can concatenate paths
        // without doubling.
        if (httpBase.EndsWith('/')) httpBase = httpBase[..^1];

        var wsBase = httpBase.StartsWith("https:")
            ? "wss:" + httpBase["https:".Length..]
            : "ws:" + httpBase["http:".Length..];

        var factory = app.Services.GetRequiredService<ILoggerFactory>();
        messageLogger?.SendMessage(
            TestMessageLevel.Informational,
            $"Sunlight ingest host listening at {httpBase} (WS {wsBase}); JSONL → {settings.LogEndpoint}");

        return new IngestHost(app, httpBase, wsBase, factory, serilogRoot);
    }

    public async ValueTask DisposeAsync()
    {
        await _app.StopAsync();
        await _app.DisposeAsync();
        // CompactJsonFormatter is buffered behind the Serilog file sink;
        // disposing the root logger flushes outstanding writes.
        if (_serilogRootForDispose is IDisposable d) d.Dispose();
    }

    private static string ReadEmbeddedResource(string fileName)
    {
        var asm = typeof(IngestHost).Assembly;
        var resourceName = $"SunlightTestAdapter.Resources.{fileName}";
        using var stream = asm.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource not found: {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Build the index.html that boots the test bundle. Mirrors the
    /// Playwright-route variant in <c>TestRunner.BuildHtml</c> but injects
    /// the <c>__nscriptLogEndpoint</c> / <c>__nscriptLogWsEndpoint</c>
    /// globals that <c>LogSinkFactory.CreateFromBootstrap</c> reads, and
    /// uses relative URLs so the browser hits the same origin.
    /// </summary>
    private static string BuildIndexHtml()
    {
        return @"<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<title>NScript Test Suite</title>
<link rel='stylesheet' href='/qunit.css'>
<script>
  // Endpoints LogSinkFactory.CreateFromBootstrap looks for. Same-origin
  // since Kestrel serves both static assets and the ingest endpoints.
  window.__nscriptLogEndpoint = '/_log';
  window.__nscriptLogWsEndpoint = (location.protocol === 'https:' ? 'wss:' : 'ws:') + '//' + location.host + '/_log/ws';
  window.__nscriptResults = [];
  window.__nscriptResultsPromise = new Promise(function(resolve) {
    function safeString(v) {
      if (v === null || v === undefined) return v;
      var t = typeof v;
      if (t === 'string' || t === 'number' || t === 'boolean') return v;
      try { return String(v); } catch (e) { return '<unstringifiable>'; }
    }
    function project(t) {
      var assertions = [];
      if (t && t.assertions && t.assertions.length) {
        for (var i = 0; i < t.assertions.length; i++) {
          var a = t.assertions[i];
          assertions.push({
            passed: !!a.passed,
            message: safeString(a.message),
            expected: safeString(a.expected),
            actual: safeString(a.actual),
          });
        }
      }
      return {
        name: t && t.name,
        suiteName: t && t.suiteName,
        fullName: t && t.fullName,
        status: t && t.status,
        runtime: t && t.runtime,
        assertions: assertions,
      };
    }
    document.addEventListener('DOMContentLoaded', function() {
      QUnit.on('testEnd', function(t) {
        try { window.__nscriptResults.push(project(t)); }
        catch (e) { window.__nscriptResults.push({ name: t && t.name, status: 'failed', assertions: [{ passed: false, message: 'capture error: ' + e }] }); }
      });
      QUnit.on('runEnd', function() { resolve(window.__nscriptResults); });
    });
  });
</script>
<script src='/qunit.js'></script>
</head>
<body>
<div id='qunit'></div>
<div id='qunit-fixture'></div>
<script src='/test-bundle.js'></script>
</body>
</html>";
    }
}
