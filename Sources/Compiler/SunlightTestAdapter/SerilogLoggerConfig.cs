namespace SunlightTestAdapter;

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

/// <summary>
/// Wires a Serilog file sink into the host's <see cref="ILoggerFactory"/>.
/// Output is one JSON object per line (<see cref="CompactJsonFormatter"/>),
/// matching the format used by the in-repo
/// <c>NScript.Utils/CompilerLog.cs</c>. The file is opened shared so
/// concurrent test runs in the same workdir do not collide.
/// </summary>
internal static class SerilogLoggerConfig
{
    /// <summary>
    /// Build a configured Serilog <see cref="ILogger"/> ready for use
    /// with <c>builder.Logging.AddSerilog(logger)</c>.
    /// </summary>
    /// <param name="outputPath">Absolute path of the JSONL output file. Parent dir created if needed.</param>
    public static Serilog.ILogger BuildJsonlLogger(string outputPath)
    {
        var dir = System.IO.Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: outputPath,
                shared: true,
                flushToDiskInterval: System.TimeSpan.FromMilliseconds(250))
            .CreateLogger();
    }
}
