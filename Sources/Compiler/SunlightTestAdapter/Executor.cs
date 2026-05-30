namespace SunlightTestAdapter;

using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

[ExtensionUri(Constants.ExecutorUri)]
public class Executor : ITestExecutor
{
    public void Cancel()
    {
    }

    public void RunTests(
        IEnumerable<TestCase>? tests,
        IRunContext? runContext,
        IFrameworkHandle? frameworkHandle)
    {
        if (tests == null || frameworkHandle == null)
        {
            return;
        }

        RunTestsImpl(tests, runContext, frameworkHandle)
            .GetAwaiter()
            .GetResult();
    }

    public void RunTests(
        IEnumerable<string>? sources,
        IRunContext? runContext,
        IFrameworkHandle? frameworkHandle)
    {
        if (sources == null || frameworkHandle == null)
        {
            return;
        }

        var rv = new List<TestCase>();

        var dict = new Dictionary<string, TestProperty>
        {
            ["Name"] = TestCaseProperties.DisplayName,
            ["FullyQualifiedName"] = TestCaseProperties.FullyQualifiedName,
        };

        var filter = runContext?.GetTestCaseFilter(
            dict.Keys,
            propName => dict[propName]);

        foreach (var source in sources)
        {
            var effectiveSource = Discoverer.ResolveTestSourceAssembly(source, runContext) ?? source;
            var testCasesFromSource = Discoverer.GetTests(effectiveSource);

            rv.AddRange(
                testCasesFromSource.Where(tc =>
                    filter?.MatchTestCase(
                        tc,
                        propName => dict.TryGetValue(propName, out var prop)
                            ? tc.GetPropertyValue(prop)
                            : null) ?? true));
        }

        RunTests(rv, runContext, frameworkHandle);
    }

    private async Task RunTestsImpl(
        IEnumerable<TestCase> tests,
        IRunContext? runContext,
        IFrameworkHandle frameworkHandle)
    {
        var settings = ExtractSettings(runContext);

        foreach (var group in tests.GroupBy(t => t.Source))
        {
            await RunSourceGroup(group.Key, group.ToList(), settings, frameworkHandle);
        }
    }

    private static async Task RunSourceGroup(
        string source,
        IReadOnlyCollection<TestCase> tests,
        Settings? settings,
        IFrameworkHandle frameworkHandle)
    {
        var jsPath = ResolveJsPath(source, settings);

        if (jsPath == null)
        {
            frameworkHandle.SendMessage(
                TestMessageLevel.Error,
                $"Could not locate JS bundle for source: {source}. " +
                $"Tried sibling .js and JsFilePath setting ('{settings?.JsFilePath ?? "<null>"}').");

            foreach (var test in tests)
            {
                frameworkHandle.RecordResult(new TestResult(test)
                {
                    DisplayName = test.DisplayName,
                    Outcome = TestOutcome.NotFound,
                });
            }
            return;
        }

        // Per v2 D12: only boot Kestrel when LogEndpoint is configured.
        // When unset, the runner stays on today's Playwright-route
        // static-serving path with a NullLogger — zero behavioral
        // change for existing consumers.
        IngestHost? host = null;
        try
        {
            string? baseUrl = null;
            ILogger runnerLogger = NullLogger.Instance;
            ILogger executorLogger = NullLogger.Instance;
            if (!string.IsNullOrEmpty(settings?.LogEndpoint))
            {
                host = await IngestHost.CreateAsync(settings, jsPath, frameworkHandle);
                baseUrl = host.HttpBaseUrl;
                runnerLogger = host.LoggerFactory.CreateLogger("SunlightTestAdapter.TestRunner");
                executorLogger = host.LoggerFactory.CreateLogger("SunlightTestAdapter.Executor");
            }

            executorLogger.LogInformation(
                "Sunlight test run start; source={Source} bundle={JsPath} testCount={TestCount}",
                source,
                jsPath,
                tests.Count);

            var tr = new TestRunner(jsPath, baseUrl, runnerLogger);
            var testResults = await tr.RunTests(frameworkHandle);

            int passed = 0;
            int failed = 0;
            foreach (var test in tests)
            {
                var className = ExtractClassName(test.FullyQualifiedName);
                var result = testResults.FirstOrDefault(r =>
                    r.Name == test.DisplayName &&
                    (string.IsNullOrEmpty(className) || r.SuiteName == className));

                // Fall back to display-name-only match if SuiteName matching missed
                // (e.g. emitter doesn't fully populate suiteName for top-level tests).
                result ??= testResults.FirstOrDefault(r => r.Name == test.DisplayName);

                var outcome = result == null
                    ? TestOutcome.NotFound
                    : result.Status == "passed"
                        ? TestOutcome.Passed
                        : TestOutcome.Failed;

                executorLogger.LogDebug(
                    "Test result; name={TestName} outcome={Outcome} runtimeMs={RuntimeMs}",
                    test.DisplayName,
                    outcome,
                    result?.Runtime ?? 0);

                if (outcome == TestOutcome.Passed) passed++;
                else if (outcome == TestOutcome.Failed) failed++;

                frameworkHandle.RecordResult(new TestResult(test)
                {
                    DisplayName = test.DisplayName,
                    Outcome = outcome,
                    ErrorMessage = result != null && result.Status != "passed"
                        ? FormatFailures(result)
                        : null,
                });
            }

            executorLogger.LogInformation(
                "Sunlight test run end; source={Source} passed={Passed} failed={Failed} total={Total}",
                source,
                passed,
                failed,
                tests.Count);
        }
        finally
        {
            if (host != null)
            {
                await host.DisposeAsync();
            }
        }
    }

    private static string? ResolveJsPath(string source, Settings? settings)
    {
        var siblingJs = Path.Combine(
            Path.GetDirectoryName(source) ?? string.Empty,
            Path.GetFileNameWithoutExtension(source) + ".js");

        if (File.Exists(siblingJs))
        {
            return siblingJs;
        }

        if (settings?.JsFilePath == null)
        {
            return null;
        }

        var configured = settings.JsFilePath;

        if (Path.IsPathRooted(configured) && File.Exists(configured))
        {
            return configured;
        }

        // Relative paths: resolve against the test DLL directory first, then CWD.
        var sourceDir = Path.GetDirectoryName(source) ?? string.Empty;
        var relativeToSource = Path.GetFullPath(Path.Combine(sourceDir, configured));
        if (File.Exists(relativeToSource))
        {
            return relativeToSource;
        }

        return File.Exists(configured) ? Path.GetFullPath(configured) : null;
    }

    private static string ExtractClassName(string fullyQualifiedName)
    {
        if (string.IsNullOrEmpty(fullyQualifiedName))
        {
            return string.Empty;
        }

        var lastDot = fullyQualifiedName.LastIndexOf('.');
        if (lastDot <= 0)
        {
            return string.Empty;
        }

        var withoutMethod = fullyQualifiedName.Substring(0, lastDot);
        var classDot = withoutMethod.LastIndexOf('.');
        return classDot < 0 ? withoutMethod : withoutMethod.Substring(classDot + 1);
    }

    private static string FormatFailures(RootObject result)
    {
        var failed = result.Assertions?.Where(a => !a.Passed) ?? Enumerable.Empty<Assertion>();
        return string.Concat(failed.Select(a =>
            $"Assertion failed({a.Message}). Expected: {a.Expected}, Actual: {a.Actual}\n"));
    }

    private static Settings? ExtractSettings(IRunContext? runContext)
    {
        if (runContext?.RunSettings?.SettingsXml == null)
        {
            return null;
        }

        if (runContext.RunSettings.GetSettings(Constants.SettingsName) is not SettingsProvider provider)
        {
            return null;
        }

        using var reader = XmlReader.Create(new StringReader(runContext.RunSettings.SettingsXml));
        provider.Load(reader);
        return provider.Settings;
    }
}
