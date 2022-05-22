namespace SunlightTestAdapter;

using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

[ExtensionUri(Constants.ExecutorUri)]
public class Executor : ITestExecutor
{
    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
    {
        RunTestsImpl(tests, runContext, frameworkHandle).Wait();
    }

    public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
    {
        var tests = new List<TestCase>();
        foreach (var source in sources)
        {
            tests.AddRange(Discoverer.GetTests(source));
        }

        RunTests(tests, runContext, frameworkHandle);
    }

    private async Task RunTestsImpl(
        IEnumerable<TestCase> tests,
        IRunContext runContext,
        IFrameworkHandle frameworkHandle)
    {
        var settings = ExtractSettings(runContext);

        var jsFileName = Directory.GetFiles(runContext.TestRunDirectory + "\\..\\", "*.js")[0];

        if (settings == null)
        {
            frameworkHandle.SendMessage(
                TestMessageLevel.Error,
                "Failed to parse settings"
            );

            return;
        }

        var tr = new TestRunner(settings, jsFileName);
        var testResults = await tr.RunTests(frameworkHandle);

        foreach (var test in tests)
        {
            // Check in testResults for the outcome
            var result = testResults
                .Where(testResult => testResult.Name == test.DisplayName)
                .FirstOrDefault();

            frameworkHandle.RecordResult(new TestResult(test)
            {
                Outcome = result == null ? TestOutcome.NotFound
                : (result.Status == "passed"
                    ? TestOutcome.Passed
                    : TestOutcome.Failed),
                ErrorMessage = result?.Status != "passed"
                    ? result?.Errors[0].Message ?? null
                    : null,
            });
        }
    }

    private Settings? ExtractSettings(IRunContext runContext)
    {
        var provider = runContext.RunSettings.GetSettings(
            Constants.SettingsName) as SettingsProvider;

        var xml = new XmlDocument();
        xml.LoadXml(runContext.RunSettings.SettingsXml);

        return provider?.Settings;
    }
}

public class Settings
{
    public string? ChromeExePath
    { get; private set; }

    public static Settings Extract(XmlNode node)
    {
        var settings = new Settings();

        if (node.Name == Constants.SettingsName)
        {
            var chromeExePath = node.SelectSingleNode(nameof(ChromeExePath))?.FirstChild;
            if (chromeExePath != null && chromeExePath.NodeType == XmlNodeType.Text)
            {
                settings.ChromeExePath = chromeExePath.Value;
            }
        }

        return settings;
    }
}

[SettingsName(Constants.SettingsName)]
public class SettingsProvider : ISettingsProvider
{
    public Settings? Settings
    {get; private set;}

    public void Load(XmlReader reader)
    {
        var xml = new XmlDocument();
        reader.Read();

        Settings = Settings.Extract(xml.ReadNode(reader));
    }
}