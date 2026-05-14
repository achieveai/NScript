namespace SunlightTestAdapter;

public class Settings
{
    public string? JsFilePath { get; set; }

    // Optional. When set, Discoverer/Executor pivot to this DLL instead of the
    // testhost-provided source (typically the wrapper assembly that brought in
    // Microsoft.NET.Test.Sdk). Path is relative to the runsettings file or absolute.
    public string? TestSourceAssembly { get; set; }
}
