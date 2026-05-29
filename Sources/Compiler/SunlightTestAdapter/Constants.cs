namespace SunlightTestAdapter;

public static class Constants
{
    public const string ExecutorUri = "executor://nscript.qunit.testadapter";

    public const string SettingsName = "NSTest";

    public const string JsFilePathStr = "JsFilePath";

    public const string TestSourceAssemblyStr = "TestSourceAssembly";

    /// <summary>
    /// Optional file path the adapter writes structured JSONL log events
    /// to. When set, the executor stands up an in-process Kestrel host
    /// that serves the test bundle's static assets AND ingests
    /// browser-side <c>WebSocketLogSink</c> / <c>HttpLogSink</c> traffic
    /// at the same origin. When unset, today's Playwright-route static
    /// serving path stays in effect — zero behavioral change for
    /// existing consumers.
    /// </summary>
    public const string LogEndpointStr = "LogEndpoint";
}
