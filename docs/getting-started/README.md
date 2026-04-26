# Getting started with NScript

> **Audience:** *App authors* — first time picking up NScript.

## TL;DR

NScript compiles your C# project into a single JavaScript file. You write `.cs` files, run `dotnet build`, and a `.js` artifact lands in your web app's script directory. This page walks you from a fresh checkout to a "Hello, World" page in the browser, then lists the day-1 surprises that catch new contributors.

## Prerequisites

- **.NET SDK 10.0.100** — pinned by `global.json` at the repo root (`rollForward: latestMinor`). Newer 10.0.x SDKs are accepted; older majors are not the supported configuration.
- **A web server to host the generated JS** — `npx serve`, IIS Express, `python -m http.server`, anything that serves static files.
- **A modern browser** — Chrome / Edge / Firefox. NScript's runtime targets evergreen browsers; no IE11 support.

NScript itself ships as a custom MSBuild SDK (`Mcqdb.NScript.Sdk`) plus a set of NuGet runtime libraries. You do not call the compiler directly — `dotnet build` does it for you.

## Quick start: Hello, World

The repo includes Visual Studio project templates under `NScriptToolSet/Templates/ProjectTemplate/`. The minimum NScript program is:

```csharp
namespace HelloWorld
{
    using System.Web.Html;
    using System.Runtime.CompilerServices;  // for [EntryPoint]

    public class Program
    {
        [EntryPoint]
        public static void Main()
        {
            var container = Document.GetElementById("app");
            container.InnerHTML = "<h1>Hello, World from C#!</h1>";
        }
    }
}
```

The `[EntryPoint]` attribute on the `static Main` method is how NScript identifies the application root (the attribute targets methods — see `Sources/Framework/mscorlib/Runtime/CompilerServices/EntryPointAttribute.cs`). Without it, demand-driven dead-code elimination (see [ADR 0022](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)) will strip your program.

### Project file

```xml
<Project>
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <NoStdLib>True</NoStdLib>
    <GenerateJs>True</GenerateJs>
    <JsOutputPath>..\WebApplication\Scripts</JsOutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="$(NScriptToolSetPath)\lib\mscorlib.dll" />
    <Reference Include="$(NScriptToolSetPath)\lib\System.Core.dll" />
    <Reference Include="$(NScriptToolSetPath)\lib\System.Web.dll" />
    <Reference Include="$(NScriptToolSetPath)\lib\System.Web.Html.dll" />
    <Reference Include="$(NScriptToolSetPath)\lib\Sunlight.Framework.dll" />
    <Reference Include="$(NScriptToolSetPath)\lib\Sunlight.Framework.UI.dll" />
  </ItemGroup>
</Project>
```

`<NoStdLib>True</NoStdLib>` is critical: NScript ships its own `mscorlib.dll` whose surface is documented in [framework/core.md](../framework/core.md). The default .NET corlib has types and methods NScript cannot translate. (Projects using the `Mcqdb.NScript.Sdk` MSBuild SDK get the equivalent `<NoStandardLib>true</NoStandardLib>` set automatically — see [build/msbuild-sdk.md](../build/msbuild-sdk.md).)

### Host page

```html
<!DOCTYPE html>
<html>
<head><meta charset="utf-8"><title>Hello</title></head>
<body>
    <div id="app"></div>
    <script src="Scripts/HelloWorld.js"></script>
</body>
</html>
```

### Build

```bash
dotnet build HelloWorld.csproj -c Debug
```

The build runs `csc` to produce a DLL with the embedded NScript bound AST resource (`$$BstInfo$$`), then runs `nscript.exe` over that DLL to emit the `.js` file. See [compiler/pipeline.md](../compiler/pipeline.md) for the full flow.

## Day-1 pitfalls

These are the surprises that bite every new NScript contributor at least once:

### 1. `[EntryPoint]` is required — silently

Without `[EntryPoint]` the emitted JavaScript runs nothing. There is no startup error: the file is simply (almost) empty because demand-driven DCE pruned everything. If you `console.log` shows nothing on page load, this is the first thing to check.

### 2. `<NoStdLib>True</NoStdLib>` is not optional

If you forget it, the C# compiler will resolve `string`, `int`, etc. to the *default* .NET BCL surface and your code will compile fine — but the NScript stage will then fail to find type definitions in its embedded AST resources, often with cryptic resolution errors. Always set `<NoStdLib>True</NoStdLib>` in NScript projects.

### 3. C# field names get minified

Anything compiled by NScript has its private fields and (with `-uglify`) sometimes property names rewritten to short tokens (`_a`, `_b`, …). This breaks anonymous-object property bags as JSON keys, broken JSON.stringify of `[JsonType]` objects with the wrong attribute set, and any code that grabs a property by string name. See [interop/json-and-imported-types.md](../interop/json-and-imported-types.md) for the typed escape hatches and `framework-logging.md` for the `string[]` flat-array pattern.

### 4. Framework tests need a Debug compiler build

`Test/Framework/*` projects are compiled by the NScript compiler from `NScriptToolSet/bin/Debug/`. If you only built Release, the framework tests will use a stale or missing compiler. Run:

```bash
dotnet build NScript_Full.sln -c Debug
```

before working on framework-side tests. See [testing/README.md](../testing/README.md).

### 5. `dynamic`, `yield return`, reflection, P/Invoke — none of these compile

NScript's supported C# subset is documented in [language/limitations.md](../language/limitations.md). The compiler error for these is usually clear, but the common surprise is that `IEnumerable<T>` LINQ chains work fine while `yield return` in your own iterator method does not.

### 6. `extern` methods need a `[Script]` body

Methods declared `extern` are NScript's primary inline-JS escape hatch. Without `[Script("...")]` they emit nothing and crash at runtime. See [ADR 0009](../adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md) and the [interop attributes reference](../interop/attributes.md).

### 7. `Logger.Trace` / `Logger.Debug` are stripped in Release

`[Conditional("DEBUG")]` is applied by the *caller's* compilation. If your project's Release config does not define `DEBUG` (the default), every call to `Trace` / `Debug` — including argument evaluation — is removed. This is the documented behavior, but the first time you hit "where did my logs go?" it's usually this. See [Structured client logging](../framework-logging.md).

## Where to next

| You want to… | Read |
|---|---|
| Understand the supported BCL surface | [framework/core.md](../framework/core.md) |
| Manipulate the DOM | [framework/web.md](../framework/web.md) |
| Build a data-bound view | [framework/sunlight-core.md](../framework/sunlight-core.md) + [templates/razor.md](../templates/razor.md) |
| Call into native JavaScript | [interop/attributes.md](../interop/attributes.md) |
| Debug your generated JavaScript | [debugging/source-maps.md](../debugging/source-maps.md) |
| Contribute to the compiler | [compiler/pipeline.md](../compiler/pipeline.md) |

## Cross-links

- [ADR 0006 — Two-stage compilation pipeline](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
- [ADR 0022 — Demand-driven conversion and dead-code elimination](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)
- [Build & MSBuild SDK](../build/msbuild-sdk.md)
