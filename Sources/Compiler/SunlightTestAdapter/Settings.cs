namespace SunlightTestAdapter;

public class Settings
{
    public string? JsFilePath { get; set; }

    // Optional. When set, Discoverer/Executor pivot to this DLL instead of the
    // testhost-provided source (typically the wrapper assembly that brought in
    // Microsoft.NET.Test.Sdk). Path is relative to the runsettings file or absolute.
    public string? TestSourceAssembly { get; set; }

    /// <summary>
    /// Optional structured-log JSONL output path. When non-null the
    /// executor boots an in-process Kestrel host that serves the
    /// test-bundle static assets, hosts the
    /// <c>Sunlight.Logging.Server</c> controller, and configures
    /// Serilog to write CompactJsonFormatter lines here. When null the
    /// adapter falls back to today's Playwright-route static-serving
    /// path (zero behavioral change for existing consumers).
    /// </summary>
    public string? LogEndpoint { get; set; }
}
