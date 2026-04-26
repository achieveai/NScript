# Source maps & browser debugging

> **Audience:** *App authors* debugging compiled NScript apps in browser DevTools; *contributors* working on the source-map dev server.

## TL;DR

NScript emits standard JavaScript source maps alongside the generated `.js` (V3 format), allowing browser DevTools to set breakpoints in original `.cs` files and step through C# source rather than compiled JS. The maps include a `sourcesLong` extension carrying full filesystem paths used by the **`SourceMap.Server`** ASP.NET Core middleware (`SourceMapFileHandler`), which serves the original `.cs` files back to DevTools when requested. Configure with `<SourceMapRoot>` in the project file and register the handler with `MapSourceMapFiles(prefix, options)` in your test/dev host. The path-resolution layer enforces an `AllowedSourceRoots` allow-list to defend against arbitrary-file-read via tampered maps.

## Reference — emission knobs

| MSBuild property | Purpose |
|---|---|
| `<SourceMapRoot>` | Base path / URL prefix written into the emitted `.map` `sourceRoot` field. Often `file:///source/MyApp/` for local dev or `/sourcemap/MyApp/` when the source-map dev server is in use. |
| `<JsOutputPath>` | Directory where `.js` and the matching `.map` land. The emitter writes both files plus `//# sourceMappingURL=` line in the `.js`. |

## Reference — source-map dev server (`SourceMap.Server`)

| Type | File | Purpose |
|---|---|---|
| `SourceMapFileHandler` | `Sources/Compiler/SourceMap.Server/SourceMapFileHandler.cs` | ASP.NET Core endpoint extension `MapSourceMapFiles(prefix, options)` |
| `SourceMapFileHandlerOptions` | `Sources/Compiler/SourceMap.Server/SourceMapFileHandlerOptions.cs` | `MapsDirectory`, `AllowedSourceRoots`, `MaxMapFileSizeBytes`, `MaxSourceFileSizeBytes` |
| `SourceMapSources` | `Sources/Compiler/SourceMap.Server/SourceMapSources.cs` | Helpers for resolving `sourcesLong` entries into local file paths |

The handler accepts requests of the form `/{prefix}/{mapName}/{*sourceName}`:

1. Loads `{MapsDirectory}/{mapName}.map`.
2. Looks up `sourceName` in the map's `sources` array.
3. Resolves the matching `sourcesLong` entry into an absolute path.
4. Validates the path is under one of `AllowedSourceRoots`.
5. Streams the file back if every check passes; otherwise returns 404.

A 404 (not 403) is the deliberate response for every failure mode — file missing, path outside allow-list, oversized map. This keeps the handler from leaking whether a path *exists*.

## Reference — request flow

```mermaid
sequenceDiagram
    participant DT as Browser DevTools
    participant Host as ASP.NET Core host
    participant Handler as SourceMapFileHandler
    participant FS as File system

    DT->>Host: GET /js/MyApp.js
    Host-->>DT: ...; //# sourceMappingURL=MyApp.map
    DT->>Host: GET /js/MyApp.map
    Host-->>DT: { version:3, sources:[...], sourcesLong:[...] }
    DT->>Host: GET /sourcemap/MyApp/MyService.cs
    Host->>Handler: HandleAsync(mapName=MyApp, source=MyService.cs)
    Handler->>FS: Read MapsDirectory/MyApp.map
    Handler->>Handler: Resolve sourcesLong[i] → absolute path
    Handler->>Handler: Validate against AllowedSourceRoots
    Handler->>FS: Stream original .cs file
    Handler-->>DT: 200 OK with C# source content
    DT->>DT: Show .cs in Sources panel; breakpoints work
```

## Quick start — enable source maps

### 1. Project file

```xml
<PropertyGroup>
  <GenerateJs>True</GenerateJs>
  <SourceMapRoot>/sourcemap/MyApp/</SourceMapRoot>
</PropertyGroup>
```

After build you'll see:

```
wwwroot/js/
  MyApp.js
  MyApp.map
```

### 2. Host setup (test web app or dev server)

```csharp
using OwaSourceMapper.Server;  // namespace differs from project/assembly name SourceMap.Server

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Serve the JS itself
app.UseStaticFiles();

// Serve original sources back to DevTools
app.MapSourceMapFiles("/sourcemap", new SourceMapFileHandlerOptions
{
    MapsDirectory = Path.Combine(app.Environment.ContentRootPath, "wwwroot/js"),
    AllowedSourceRoots = new[]
    {
        Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "../../"))
    },
});

app.Run();
```

### 3. Open DevTools

- Chromium / Edge: open Sources panel; you should see your `.cs` files under the original folder structure.
- Firefox: same, but enable "Show Original Sources" in DevTools settings if not visible.

Set a breakpoint in a `.cs` file. When the JS hits the corresponding location, DevTools pauses at the C# line, with locals named per the C# variable names (where preserved by the compiler).

## Examples

### Per-test-host wiring

For framework tests served by `Test/Framework/TestWebApplication`, the source-map server can be added so QUnit failures show in the original `.cs`:

```csharp
endpoints.MapSourceMapFiles("/sourcemap", new SourceMapFileHandlerOptions
{
    MapsDirectory = Path.Combine(env.ContentRootPath, "GeneratedScripts"),
    AllowedSourceRoots = new[] { Path.GetFullPath(Path.Combine(env.ContentRootPath, "../..")) },
    MaxMapFileSizeBytes = 32L * 1024 * 1024,  // larger maps for big test bundles
});
```

### Disabling source maps in Release

Source-map emission is on by default whenever `<GenerateJs>True</GenerateJs>` is set. To suppress for production builds:

```xml
<PropertyGroup>
  <SourceMapRoot></SourceMapRoot>  <!-- empty disables -->
  <Uglify>true</Uglify>
</PropertyGroup>
```

The `.map` file isn't emitted, and the `//# sourceMappingURL=` line is omitted.

## Known gotchas

### `SourceMapRoot` must match the URL the browser uses

If your site serves `MyApp.js` from `/static/js/` but `SourceMapRoot` is `file:///source/MyApp/`, DevTools will look for `.cs` files on the local filesystem (which works only for the developer who built the binary). For shared dev/staging servers, set `SourceMapRoot` to the `/sourcemap/...` path served by the dev server.

### `AllowedSourceRoots` must be set explicitly for non-trivial deployments

The default allow-list is `MapsDirectory`'s parent hierarchy — fine for "build and run on the same machine" but wrong as soon as your CI builds elsewhere and ships maps + JS without the source tree. Set `AllowedSourceRoots` to the actual source root *on the hosting machine*, or accept that source-map browsing won't work in that environment.

### 404 means "any failure", not "missing file"

`SourceMapFileHandler` returns 404 for: file not found, map not found, path outside allow-list, oversized map, malformed map, mismatched source name. This is deliberate — it prevents DevTools from probing for which files exist. Diagnose via server logs (the handler logs the actual reason at Information level), not by HTTP status.

### Map file size cap defaults to 16 MB

Big template-heavy apps can produce maps larger than 16 MB. Bump `MaxMapFileSizeBytes` if you see 404s on a known-good map and the log shows "map exceeds size limit."

### `sourcesLong` is an NScript-specific extension

Standard source maps use `sources` (relative names) and `sourceRoot` (URL prefix). NScript additionally writes `sourcesLong` with absolute paths so the dev server can locate files without needing `sourceRoot` to be a real filesystem path. Other tooling that consumes the map ignores `sourcesLong`.

### Breakpoints in heavily inlined methods may not stop

Devirtualisation + accessor inlining ([ADR 0023](../adr/0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md)) and function dedup ([ADR 0024](../adr/0024-deduplicate-structurally-identical-functions-after-minification.md)) can collapse multiple call sites into a single emitted function. DevTools may not be able to map back to one specific original line. Disable optimisations (`<JsOptimize>false</JsOptimize>`) when you need to debug into trivial accessors.

### Browser caching of `.map` files

DevTools aggressively caches source maps. After rebuilding, hard-refresh DevTools (close and reopen) to pick up new mappings — a regular page refresh isn't enough.

### The legacy `SrcMapper.ashx` still ships

`Sources/Compiler/SourceMap/SrcMapper.ashx` is the older WebForms handler. The ASP.NET Core `SourceMapFileHandler` replaces it for new hosts. Don't mix; pick one per deployment.

## Diagnostics

| Symptom | Cause |
|---|---|
| DevTools shows JS, not C#, in Sources panel | `<SourceMapRoot>` not set, or `.map` file 404s — check Network tab |
| `.map` loads but `.cs` files all 404 | `AllowedSourceRoots` missing/wrong, or sources not on the hosting machine |
| Breakpoint placed in `.cs` doesn't trigger | Optimisations inlined the code; disable `<JsOptimize>` for the debug build |
| `sourceMappingURL` comment present but DevTools doesn't load it | Browser source-map setting disabled, or map file exceeds DevTools' own size limits |
| Source-map server returns 200 with empty body | `MaxSourceFileSizeBytes` exceeded — increase the cap |
| Wrong line shown when breakpoint hits | Stale `.map` cached by DevTools — close/reopen to refresh |

## Cross-links

- [ADR 0006 — Compiler pipeline](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
- [ADR 0023 — Devirtualisation](../adr/0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md)
- [ADR 0024 — Function dedup](../adr/0024-deduplicate-structurally-identical-functions-after-minification.md)
- [Compiler pipeline](../compiler/pipeline.md) — where source maps are produced
- [MSBuild SDK](../build/msbuild-sdk.md) — `<SourceMapRoot>` configuration
- [Testing](../testing/README.md) — wiring source maps into framework-test hosts
