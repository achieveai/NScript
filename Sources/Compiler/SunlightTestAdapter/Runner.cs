namespace SunlightTestAdapter;

using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

public class TestRunner
{
    private const string SyntheticHost = "https://nscript.test";
    private const string TestJsPath = "/test-bundle.js";
    private const string QunitJsPath = "/qunit.js";
    private const string QunitCssPath = "/qunit.css";
    private const string IndexPath = "/index.html";
    private const string SourceMapPrefix = "/sourcemap/";

    private readonly string _jsFilePath;
    private Dictionary<string, string>? _sourceLongMap;

    public TestRunner(string jsFilePath)
    {
        _jsFilePath = jsFilePath;
    }

    // Builds sources[i] -> sourcesLong[i] lookup from the companion .map file.
    private Dictionary<string, string> GetSourceLongMap()
    {
        if (_sourceLongMap != null) return _sourceLongMap;
        var mapPath = Path.ChangeExtension(_jsFilePath, ".map");
        if (!File.Exists(mapPath)) return _sourceLongMap = new();
        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(mapPath));
            var root = doc.RootElement;
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (root.TryGetProperty("sources", out var srcProp) &&
                root.TryGetProperty("sourcesLong", out var longProp))
            {
                var sources = srcProp.EnumerateArray().ToArray();
                var longs = longProp.EnumerateArray().ToArray();
                for (int i = 0; i < Math.Min(sources.Length, longs.Length); i++)
                {
                    var shortName = sources[i].GetString();
                    var longPath = longs[i].GetString();
                    if (shortName != null && longPath != null)
                        map[shortName] = longPath;
                }
            }
            return _sourceLongMap = map;
        }
        catch { return _sourceLongMap = new(); }
    }

    // Rewrites sourceRoot in the map JSON so DevTools fetches sources from our
    // synthetic host instead of the baked-in SrcMapper.ashx / localhost URL.
    private static string PatchSourceRoot(string mapJson) =>
        Regex.Replace(
            mapJson,
            @"""sourceRoot""\s*:\s*""[^""]*""",
            $@"""sourceRoot"":""{SyntheticHost}{SourceMapPrefix}""");

    public async Task<RootObject[]> RunTests(IMessageLogger logger)
    {
        IPlaywright? playwright = null;
        IBrowser? browser = null;

        try
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--disable-gpu" },
            });

            var page = await browser.NewPageAsync();
            await WireRoutes(page);

            page.Console += (_, msg) => logger.SendMessage(
                msg.Type == "error" ? TestMessageLevel.Error : TestMessageLevel.Informational,
                $"[browser:{msg.Type}] {msg.Text}");
            page.PageError += (_, err) => logger.SendMessage(
                TestMessageLevel.Error,
                $"[browser:pageerror] {err}");

            var response = await page.GotoAsync(
                SyntheticHost + IndexPath,
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
                return Array.Empty<RootObject>();
            }

            return JsonSerializer.Deserialize<RootObject[]>(jsonString) ?? Array.Empty<RootObject>();
        }
        catch (Exception e)
        {
            logger.SendMessage(TestMessageLevel.Error, e.ToString());
            return Array.Empty<RootObject>();
        }
        finally
        {
            if (browser != null)
            {
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

        // Serve the companion .map file for the test bundle. Rewrite sourceRoot to
        // point at our synthetic host so DevTools requests source files from us
        // rather than the baked-in SrcMapper.ashx URL.
        await page.RouteAsync(SyntheticHost + "/**/*.map", async route =>
        {
            var mapPath = Path.ChangeExtension(_jsFilePath, ".map");
            if (!File.Exists(mapPath))
            {
                await route.FulfillAsync(new RouteFulfillOptions { Status = 404 });
                return;
            }
            var mapJson = await File.ReadAllTextAsync(mapPath);
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json; charset=utf-8",
                Body = PatchSourceRoot(mapJson),
            });
        });

        // Serve individual source files via the sourcesLong[] lookup table.
        // DevTools requests https://nscript.test/sourcemap/{sources[i]} after
        // reading the patched sourceRoot from the .map file above.
        await page.RouteAsync(SyntheticHost + "/sourcemap/**", async route =>
        {
            var sourceName = Uri.UnescapeDataString(
                route.Request.Url[(SyntheticHost + SourceMapPrefix).Length..]);
            var sourceMap = GetSourceLongMap();
            if (!sourceMap.TryGetValue(sourceName, out var filePath) || !File.Exists(filePath))
            {
                await route.FulfillAsync(new RouteFulfillOptions { Status = 404 });
                return;
            }
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "text/plain; charset=utf-8",
                BodyBytes = await File.ReadAllBytesAsync(filePath),
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
