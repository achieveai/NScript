// -----------------------------------------------------------------------
// <copyright file="IngestPipelineEndToEndTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace SunlightTestAdapter.Integration.Test;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sunlight.Logging.Server.Models;

/// <summary>
/// Stub Main so the SDK accepts this as a runnable assembly. The test
/// runner doesn't invoke it — the entry-point exists purely to satisfy
/// the build pipeline (Microsoft.NET.Test.Sdk + FrameworkReference
/// Microsoft.AspNetCore.App together require one).
/// </summary>
public static class StubEntryPoint
{
    public static void Main(string[] args) { }
}

/// <summary>
/// End-to-end proof that the ingest pipeline + Serilog file sink work
/// together. Boots an <see cref="IngestHost"/> against a temp JSONL
/// path, then drives BOTH halves of the v2 + addendum scope:
/// (a) the browser-side pipeline by POSTing a <see cref="LogEnvelopeDto"/>
///     to the controller (simulates the
///     <c>HttpLogSink</c> emit path), AND
/// (b) the adapter-side dogfood pipeline by creating an
///     <see cref="ILogger"/> on
///     <see cref="IngestHost.LoggerFactory"/> with category
///     <c>SunlightTestAdapter.TestRunner</c> and emitting through it
///     (simulates the runner's lifecycle logs).
/// Then asserts that BOTH families of events appear in the same
/// <c>tests.jsonl</c> file — proving the end-to-end pipeline without
/// needing a real browser. (The dogfood paths in step 15 already
/// exercise this code path during a normal run; this test is the
/// hermetic sanity check that the wiring stays correct.)
/// </summary>
[TestClass]
public class IngestPipelineEndToEndTests
{
    private string _logPath = null!;
    private IngestHost _host = null!;
    private HttpClient _http = null!;

    [TestInitialize]
    public async Task Init()
    {
        var tempDir = Path.Combine(
            Path.GetTempPath(),
            "SunlightIngestE2E_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        _logPath = Path.Combine(tempDir, "tests.jsonl");

        // The bundle file doesn't need to exist for this test — we
        // never hit /test-bundle.js. Provide a path that maps to a
        // realistic file so the route handler is happy if it does run.
        var bundlePath = Path.Combine(tempDir, "test-bundle.js");
        await File.WriteAllTextAsync(bundlePath, "/* dummy */");

        var settings = new Settings { LogEndpoint = _logPath };
        _host = await IngestHost.CreateAsync(settings, bundlePath, messageLogger: null);
        _http = new HttpClient { BaseAddress = new Uri(_host.HttpBaseUrl) };
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        _http?.Dispose();
        if (_host != null)
        {
            await _host.DisposeAsync();
        }
    }

    [TestMethod]
    public async Task BothBrowserAndAdapterEventsLandInSameJsonl()
    {
        // Browser side: POST an envelope mimicking what HttpLogSink
        // would emit. The controller drives MEL on category
        // "Sunlight.Browser.Fixture", which the Serilog sink writes
        // to _logPath.
        var browserEnv = new LogEnvelopeDto
        {
            Events = new List<LogEventDto>
            {
                new LogEventDto
                {
                    Id = "browser-evt-1",
                    Ts = "2026-05-29T00:00:00.000Z",
                    Level = "INFO",
                    Msg = "hello from browser",
                    Cat = "Fixture",
                    TraceId = "trace-aaa",
                    SpanId = "span-bbb",
                },
            },
        };
        var resp = await _http.PostAsJsonAsync("/_log", browserEnv);
        Assert.IsTrue(resp.IsSuccessStatusCode, "POST /_log returned success");

        // Adapter dogfood side: emit through the same ILoggerFactory
        // the Runner/Executor use during a real run.
        var runnerLogger = _host.LoggerFactory.CreateLogger("SunlightTestAdapter.TestRunner");
        var executorLogger = _host.LoggerFactory.CreateLogger("SunlightTestAdapter.Executor");
        runnerLogger.LogInformation("Sunlight test run starting; bundle={JsFilePath}", "fixture-bundle.js");
        executorLogger.LogInformation(
            "Sunlight test run end; source={Source} passed={Passed} failed={Failed} total={Total}",
            "fixture.dll", 2, 0, 2);

        // Give the Serilog file sink its 250ms flushToDiskInterval
        // window to write — plus a margin.
        await Task.Delay(700);

        // Force a flush by tearing down the host. The Serilog file
        // sink's Dispose flushes the buffered writes synchronously.
        await _host.DisposeAsync();
        _host = null!;  // suppress double-dispose in Cleanup

        var lines = await File.ReadAllLinesAsync(_logPath);
        Assert.IsTrue(lines.Length >= 3, $"Expected at least 3 events, got {lines.Length}");

        var sourceContexts = new List<string>();
        var messages = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            using var doc = JsonDocument.Parse(line);
            if (doc.RootElement.TryGetProperty("SourceContext", out var sc))
            {
                sourceContexts.Add(sc.GetString() ?? string.Empty);
            }
            if (doc.RootElement.TryGetProperty("@mt", out var mt))
            {
                messages.Add(mt.GetString() ?? string.Empty);
            }
        }

        // Browser-side category that the LogIngestionService produces.
        Assert.IsTrue(
            sourceContexts.Contains("Sunlight.Browser.Fixture"),
            $"Expected Sunlight.Browser.Fixture in source contexts but saw: [{string.Join(", ", sourceContexts)}]");
        // Adapter-side categories (the addendum dogfood path).
        Assert.IsTrue(
            sourceContexts.Contains("SunlightTestAdapter.TestRunner"),
            "Expected SunlightTestAdapter.TestRunner in source contexts");
        Assert.IsTrue(
            sourceContexts.Contains("SunlightTestAdapter.Executor"),
            "Expected SunlightTestAdapter.Executor in source contexts");

        Assert.IsTrue(
            messages.Exists(m => m == "{Message}"),
            "Browser event uses the canonical {Message} template");
        Assert.IsTrue(
            messages.Exists(m => m.Contains("Sunlight test run end")),
            "Adapter run-end message present");
    }

    [TestMethod]
    public async Task DroppedCountFromBrowserEnvelopeSurfacesAsOverflowWarning()
    {
        var env = new LogEnvelopeDto
        {
            Events = new List<LogEventDto>(),
            Dropped = 5,
        };
        var resp = await _http.PostAsJsonAsync("/_log", env);
        Assert.IsTrue(resp.IsSuccessStatusCode);

        await Task.Delay(700);
        await _host.DisposeAsync();
        _host = null!;

        var lines = await File.ReadAllLinesAsync(_logPath);
        bool foundOverflow = false;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            using var doc = JsonDocument.Parse(line);
            if (doc.RootElement.TryGetProperty("SourceContext", out var sc)
                && sc.GetString() == "Sunlight.Browser.Overflow")
            {
                foundOverflow = true;
                break;
            }
        }
        Assert.IsTrue(foundOverflow, "Browser-side dropped count surfaced as Overflow warning");
    }
}
