using Microsoft.Extensions.FileProviders;
using OwaSourceMapper.Server;

// Resolve the TestWebApplication directory relative to this project.
// When run from the project dir (dotnet run) the base is the project source dir;
// when run from the bin dir the base is the output dir, so we walk up from either.
string devServerDir = AppContext.BaseDirectory;
string webAppDir = ResolveWebAppDir(devServerDir);
string mapsDir = Path.Combine(webAppDir, "GeneratedScripts");
string repoRoot = Path.GetFullPath(Path.Combine(webAppDir, "../../.."));

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5005");

var app = builder.Build();

// Serve static files from TestWebApplication/.
var fp = new PhysicalFileProvider(webAppDir);
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = fp,
    DefaultFileNames = ["TodoApp.htm"],
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fp,
    ServeUnknownFileTypes = true,
});

// SourceMap.Server — resolves sourcesLong paths for DevTools source requests.
app.MapSourceMapFiles("/sourcemap", new SourceMapFileHandlerOptions
{
    MapsDirectory = mapsDir,
    AllowedSourceRoots = [repoRoot],
});

Console.WriteLine($"Web root : {webAppDir}");
Console.WriteLine($"Maps dir : {mapsDir}");
Console.WriteLine($"Repo root: {repoRoot}");
Console.WriteLine("Open     : http://localhost:5005/TodoApp.htm");

app.Run();

static string ResolveWebAppDir(string startDir)
{
    // Walk up from startDir looking for the TestWebApplication sibling.
    // Handles both `dotnet run` (project dir) and bin/Debug/net8.0 (bin dir).
    var current = new DirectoryInfo(startDir);
    while (current != null)
    {
        string candidate = Path.Combine(current.FullName, "TestWebApplication");
        if (Directory.Exists(candidate))
            return Path.GetFullPath(candidate);

        // Also check sibling of current dir name == "DevServer"
        if (current.Name == "DevServer")
        {
            string sibling = Path.Combine(current.Parent!.FullName, "TestWebApplication");
            if (Directory.Exists(sibling))
                return Path.GetFullPath(sibling);
        }

        current = current.Parent;
    }

    throw new DirectoryNotFoundException(
        "Could not locate TestWebApplication relative to " + startDir +
        ". Run from the DevServer project directory or a parent.");
}
