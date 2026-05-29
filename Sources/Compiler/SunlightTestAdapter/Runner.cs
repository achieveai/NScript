namespace SunlightTestAdapter;

using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

public class TestRunner
{
    private const string SyntheticHost = "https://nscript.test";
    private const string TestJsPath = "/test-bundle.js";
    private const string QunitJsPath = "/qunit.js";
    private const string QunitCssPath = "/qunit.css";
    private const string IndexPath = "/index.html";

    private readonly string _jsFilePath;
    private readonly string? _kestrelBaseUrl;
    private readonly ILogger _logger;

    /// <summary>
    /// Backwards-compatible constructor: when no <c>kestrelBaseUrl</c>
    /// is given, the runner stays on today's Playwright-route static
    /// serving path. Used when <c>Settings.LogEndpoint</c> is unset.
    /// </summary>
    public TestRunner(string jsFilePath)
        : this(jsFilePath, kestrelBaseUrl: null, logger: NullLogger.Instance)
    {
    }

    /// <summary>
    /// When <paramref name="kestrelBaseUrl"/> is non-null the runner
    /// navigates the browser to <c>{kestrelBaseUrl}/index.html</c>
    /// directly and skips the Playwright route interception — the
    /// Kestrel host serves the bundle + ingest endpoints on a single
    /// same-origin port.
    /// </summary>
    /// <param name="logger">
    /// MEL logger the runner emits dogfood lifecycle events to
    /// (browser launch, run start/end, per-test start/end, browser
    /// close). When the caller didn't configure a real factory
    /// <see cref="NullLogger.Instance"/> is the no-op default.
    /// </param>
    public TestRunner(string jsFilePath, string? kestrelBaseUrl, ILogger logger)
    {
        _jsFilePath = jsFilePath;
        _kestrelBaseUrl = kestrelBaseUrl;
        _logger = logger ?? NullLogger.Instance;
    }

    public async Task<RootObject[]> RunTests(IMessageLogger logger)
    {
        IPlaywright? playwright = null;
        IBrowser? browser = null;

        try
        {
            _logger.LogInformation("Sunlight test run starting; bundle={JsFilePath}", _jsFilePath);
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--disable-gpu" },
            });
            _logger.LogDebug("Browser launched; version={BrowserVersion}", browser.Version);

            var page = await browser.NewPageAsync();

            // Same-origin Kestrel path (D8) vs Playwright-route legacy path.
            // The legacy path is kept verbatim for runs with LogEndpoint
            // unset so existing consumers see zero behavioral change.
            string navigationUrl;
            if (_kestrelBaseUrl != null)
            {
                navigationUrl = _kestrelBaseUrl + IndexPath;
            }
            else
            {
                await WireRoutes(page);
                navigationUrl = SyntheticHost + IndexPath;
            }

            page.Console += (_, msg) => logger.SendMessage(
                msg.Type == "error" ? TestMessageLevel.Error : TestMessageLevel.Informational,
                $"[browser:{msg.Type}] {msg.Text}");
            page.PageError += (_, err) => logger.SendMessage(
                TestMessageLevel.Error,
                $"[browser:pageerror] {err}");

            var response = await page.GotoAsync(
                navigationUrl,
                new PageGotoOptions { WaitUntil = WaitUntilState.Load });

            if (response == null || !response.Ok)
            {
                logger.SendMessage(
                    TestMessageLevel.Error,
                    $"Initial navigation failed (status={response?.Status ?? -1}).");
                return Array.Empty<RootObject>();
            }

            var jsonString = await page.EvaluateAsync<string>(
                @"async () => {
                    const timeout = new Promise((_, reject) =>
                        setTimeout(() => reject(new Error('Timed out waiting for QUnit runEnd after 120s')), 120000));
                    const results = await Promise.race([window.__nscriptResultsPromise, timeout]);
                    return JSON.stringify(results);
                  }");

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                _logger.LogWarning("Run completed with empty result JSON");
                return Array.Empty<RootObject>();
            }

            var results = JsonSerializer.Deserialize<RootObject[]>(jsonString) ?? Array.Empty<RootObject>();
            int passed = results.Count(r => r.Status == "passed");
            int failed = results.Length - passed;
            _logger.LogInformation(
                "Sunlight test run completed; passed={Passed} failed={Failed} total={Total}",
                passed,
                failed,
                results.Length);
            return results;
        }
        catch (Exception e)
        {
            logger.SendMessage(TestMessageLevel.Error, e.ToString());
            _logger.LogError(e, "Sunlight test run threw");
            return Array.Empty<RootObject>();
        }
        finally
        {
            if (browser != null)
            {
                _logger.LogDebug("Closing browser");
                await browser.CloseAsync();
            }
            playwright?.Dispose();
        }
    }

    private async Task WireRoutes(IPage page)
    {
        var qunitJs = ReadEmbeddedResource("qunit.js");
        var qunitCss = ReadEmbeddedResource("qunit.css");

        await page.RouteAsync(SyntheticHost + IndexPath, async route =>
        {
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "text/html; charset=utf-8",
                Body = BuildHtml(),
            });
        });

        await page.RouteAsync(SyntheticHost + QunitJsPath, async route =>
        {
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/javascript; charset=utf-8",
                Body = qunitJs,
            });
        });

        await page.RouteAsync(SyntheticHost + QunitCssPath, async route =>
        {
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "text/css; charset=utf-8",
                Body = qunitCss,
            });
        });

        await page.RouteAsync(SyntheticHost + TestJsPath, async route =>
        {
            if (!File.Exists(_jsFilePath))
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 404,
                    Body = $"Test bundle not found: {_jsFilePath}",
                });
                return;
            }

            var bytes = await File.ReadAllBytesAsync(_jsFilePath);
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/javascript; charset=utf-8",
                BodyBytes = bytes,
            });
        });
    }

    private static string BuildHtml()
    {
        return $@"<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<title>NScript Test Suite</title>
<link rel='stylesheet' href='{QunitCssPath}'>
<script>
  // Capture every testEnd, resolve a promise when runEnd fires.
  // We extract only the fields the .NET adapter cares about (name, suiteName,
  // status, runtime, assertions) — blindly JSON.stringify-ing the whole
  // testEnd payload throws TypeError(""Converting circular structure to JSON"")
  // when assertion values contain DOM nodes wired into NScript binder graphs
  // (HTMLButtonElement -> EventBinder -> target -> HTMLButtonElement).
  window.__nscriptResults = [];
  window.__nscriptResultsPromise = new Promise(function(resolve) {{
    function safeString(v) {{
      if (v === null || v === undefined) return v;
      var t = typeof v;
      if (t === 'string' || t === 'number' || t === 'boolean') return v;
      try {{ return String(v); }} catch (e) {{ return '<unstringifiable>'; }}
    }}
    function project(t) {{
      var assertions = [];
      if (t && t.assertions && t.assertions.length) {{
        for (var i = 0; i < t.assertions.length; i++) {{
          var a = t.assertions[i];
          assertions.push({{
            passed: !!a.passed,
            message: safeString(a.message),
            expected: safeString(a.expected),
            actual: safeString(a.actual),
          }});
        }}
      }}
      return {{
        name: t && t.name,
        suiteName: t && t.suiteName,
        fullName: t && t.fullName,
        status: t && t.status,
        runtime: t && t.runtime,
        assertions: assertions,
      }};
    }}
    document.addEventListener('DOMContentLoaded', function() {{
      QUnit.on('testEnd', function(t) {{
        try {{ window.__nscriptResults.push(project(t)); }}
        catch (e) {{ window.__nscriptResults.push({{ name: t && t.name, status: 'failed', assertions: [{{ passed: false, message: 'capture error: ' + e }}] }}); }}
      }});
      QUnit.on('runEnd', function() {{ resolve(window.__nscriptResults); }});
    }});
  }});
</script>
<script src='{QunitJsPath}'></script>
</head>
<body>
<div id='qunit'></div>
<div id='qunit-fixture'></div>
<script src='{TestJsPath}'></script>
</body>
</html>";
    }

    private static string ReadEmbeddedResource(string fileName)
    {
        var asm = typeof(TestRunner).Assembly;
        var resourceName = $"{typeof(TestRunner).Namespace}.Resources.{fileName}";

        using var stream = asm.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource not found: {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

/// <summary>
/// Trivial no-op logger used as the default when no
/// <see cref="ILoggerFactory"/> is configured (i.e. the legacy
/// LogEndpoint-unset code path). Keeps <see cref="TestRunner"/> from
/// having to null-check every emit.
/// </summary>
internal sealed class NullLogger : ILogger
{
    public static readonly NullLogger Instance = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    { }
}
