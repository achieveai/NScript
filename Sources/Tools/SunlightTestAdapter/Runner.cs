namespace SunlightTestAdapter;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
using PuppeteerSharp;

public class TestRunner
{
    private readonly Settings _settings;
    private readonly string _jsFilePath;

    public TestRunner(Settings settings, string jsFilePath)
    {
        _settings = settings;
        _jsFilePath = jsFilePath;
    }

    public async Task<RootObject[]> RunTests(IMessageLogger logger)
    {
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions {
                Headless = true,
                ExecutablePath = _settings.ChromeExePath
            });

        await using var page = await browser.NewPageAsync();
        var tempHtmlPage = await CreateTempHtmlPage(_jsFilePath);
        /* logger.SendMessage(
            TestMessageLevel.Error,
            $"filename is {tempHtmlPage}"); */
        await page.GoToAsync(tempHtmlPage);

        // TODO: Check how to pass attachments to the test runner
        await page.ScreenshotAsync("screenshot.png");

        var res = await page.EvaluateExpressionAsync("resultsPromise");
        return JsonConvert.DeserializeObject<RootObject[]>(res.ToString());
    }

    private static async Task<string> CreateTempHtmlPage(string scriptPath)
    {
        var tempFileName = Path.GetTempFileName() + ".html";
        var streamWriter = File.CreateText(tempFileName);
        await streamWriter.WriteAsync(_html);
        await streamWriter.WriteLineAsync($"<script src='{scriptPath}'></script>");
        await streamWriter.FlushAsync();
        streamWriter.Close();

        return tempFileName;
    }

    private const string _html =
    @"
<!DOCTYPE html>
<meta charset='utf-8'>
<title>Test Suite</title>
<link rel='stylesheet' href='https://code.jquery.com/qunit/qunit-2.19.1.css'>
<body>
    <div id='qunit'></div>
    <div id='qunit-fixture'></div>
    <script src='https://code.jquery.com/qunit/qunit-2.19.1.js'></script>
    <script>
        const results = [];
        const resultsPromise = new Promise((res, _) =>
        {
            QUnit.on('runEnd', () => res(results));
        });

        QUnit.on('testEnd', testEnd => results.push(testEnd));
    </script>
    <!--Inject js script here-->
    ";
}