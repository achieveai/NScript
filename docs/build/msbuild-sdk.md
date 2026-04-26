# MSBuild SDK & build integration

> **Audience:** *App authors* setting up an NScript project; *Contributors* working on the SDK or build pipeline.

## TL;DR

NScript ships an MSBuild SDK (`NScript.Sdk`, packaged as `Mcqdb.NScript.Sdk`) that wires the custom compiler (`csc.cmd`) and the JS-emit step (`nscript.cmd`) into `dotnet build`. A project consumes the SDK by importing `Sdk.props` and `Sdk.targets` (or, in newer projects, via `<Project Sdk="Mcqdb.NScript.Sdk/1.0.4-beta1">`). Setting `<GenerateJs>True</GenerateJs>` triggers the `ScriptGenerate` target after `Build`, which runs `cs2jsc` over the compiled DLL and emits `<AssemblyName>.js` to `<JsOutputPath>`. Release configuration auto-enables `Minify`, `Uglify`, and `JsOptimize`.

## Reference — SDK files

| File | Purpose |
|---|---|
| `Sources/Compiler/NScript.Sdk/Sdk/Sdk.props` | Imported early; sets defaults (`OutputType=Library`, `TargetFramework=netstandard2.1`, `NoStandardLib=true`, `JsOutputPath=./`) |
| `Sources/Compiler/NScript.Sdk/Sdk/Sdk.targets` | Imported late; defines `AfterCompile` → `ScriptGenerate` target chain |
| `Sources/Compiler/NScript.Sdk/Sdk/csc.cmd` | Wrapper that invokes `JsCsc.exe` with the right toolset path |
| `Sources/Compiler/NScript.Sdk/Sdk/nscript.cmd` | Wrapper that invokes `cs2jsc.exe` |

## Reference — MSBuild properties

| Property | Default | Purpose |
|---|---|---|
| `<GenerateJs>` | `false` | When `True`, run the `ScriptGenerate` target after compile to emit JS |
| `<JsOutputPath>` | `./` | Output directory for emitted `.js` (and `.map`) |
| `<NoStandardLib>` | `true` | Don't reference the .NET BCL — NScript uses its own `mscorlib` clone |
| `<TargetFramework>` | `netstandard2.1` | The custom CSC targets `netstandard2.1` even though NScript output is JS |
| `<CscToolPath>` | (set by SDK) | Path to NScript's compiler binaries |
| `<CscToolExe>` | `csc.cmd` (or `csc.exe`) | The compiler entry point |
| `<NScriptExe>` | `nscript.cmd` | The JS emitter entry point |
| `<Minify>` | `true` in Release, else `false` | Short-name identifier substitution |
| `<Uglify>` | `true` in Release, else `false` | Drop comments, whitespace, optional braces |
| `<JsOptimize>` | `true` in Release, else `false` | Devirtualisation, accessor inlining, function dedup ([ADRs 0023, 0024](../adr/0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md)) |
| `<SourceMapRoot>` | (unset) | Base path for source-map generation; pass `-sourceMapRoot` to `cs2jsc`. See [debugging/source-maps.md](../debugging/source-maps.md). |
| `<PluginConfig>` | (unset) | Path to a `PluginConfig.xml` registering converter plugins ([compiler/plugins.md](../compiler/plugins.md)) |

## Reference — `ScriptGenerate` target chain

```mermaid
flowchart LR
    A[dotnet build] --> B[CoreCompile<br/>(custom csc.cmd → JsCsc)]
    B --> C[AfterCompile<br/>(SDK-provided)]
    C -->|GenerateJs == True| D[ScriptGenerate]
    D --> E[Invoke nscript.cmd<br/>→ cs2jsc.exe]
    E --> F[(JsOutputPath/<br/>AssemblyName.js)]
```

`ScriptGenerate` has incremental-build inputs/outputs:
- Inputs: the compiled DLL + `@(ReferencePath)`
- Output: `$(JsOutputPath)\$(AssemblyName).js`

If neither has changed since the last build, `ScriptGenerate` is skipped.

## Quick start — minimal NScript project

```xml
<Project>
  <Import Project="$(NScriptSdkDir)\Sdk\Sdk.props" />

  <PropertyGroup>
    <RootNamespace>MyApp</RootNamespace>
    <AssemblyName>MyApp</AssemblyName>
    <GenerateJs>True</GenerateJs>
    <JsOutputPath>./wwwroot/js</JsOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="$(NScriptSdkDir)\..\Framework\mscorlib\NScript.MsCorlib.csproj" />
    <ProjectReference Include="$(NScriptSdkDir)\..\Framework\System.Web\System.Web.csproj" />
    <ProjectReference Include="$(NScriptSdkDir)\..\Framework\System.Web.Html\System.Web.Html.csproj" />
  </ItemGroup>

  <Import Project="$(NScriptSdkDir)\Sdk\Sdk.targets" />
</Project>
```

```bash
dotnet build -c Release
# Output: ./wwwroot/js/MyApp.js (minified, uglified, optimised)
```

## Examples

### Real-world setup — TodoApp

The `Test/Framework/TodoApp/TodoApp.csproj` is the canonical "complete app" example. Highlights:

```xml
<PropertyGroup>
  <GenerateJs>True</GenerateJs>
  <JsOutputPath>../TestWebApplication/GeneratedScripts</JsOutputPath>
  <NoWarn>0824;0169</NoWarn>
  <NScriptExe>$(NScriptToolsetDir)\bin\$(Configuration)\$(CompilerNetFramework)\nscript.exe</NScriptExe>
</PropertyGroup>

<ItemGroup>
  <None Include="PluginConfig.xml" />
  <EmbeddedResource Include="RazorTemplates\AppShell.skin.cshtml" />
  <EmbeddedResource Include="RazorTemplates\AppShell.css" />
</ItemGroup>
```

Notes:
- `EmbeddedResource` is required for Razor `.skin.cshtml` and CSS files — `RazorSkinParser` reads them from the assembly's resources at conversion time.
- `<None Include="PluginConfig.xml" />` ensures the file is part of the project but doesn't get copied — the SDK looks for it at the project root.
- The explicit `<NScriptExe>` override is the in-tree path; downstream consumers using the NuGet package don't need this.

### Enabling source maps

```xml
<PropertyGroup>
  <GenerateJs>True</GenerateJs>
  <SourceMapRoot>file:///source/MyApp</SourceMapRoot>
</PropertyGroup>
```

`cs2jsc` emits a `.map` file alongside the `.js` and writes a `//# sourceMappingURL=` line. See [debugging/source-maps.md](../debugging/source-maps.md) for the source-map dev server.

### Tweaking optimisation independently

```xml
<PropertyGroup>
  <Configuration>Release</Configuration>
  <Minify>true</Minify>
  <Uglify>false</Uglify>     <!-- Keep whitespace + comments for diagnostics -->
  <JsOptimize>true</JsOptimize>
</PropertyGroup>
```

Each flag is independently controllable. A common debugging combo is `Minify=true; Uglify=false; JsOptimize=true` — short names (so it matches production runtime behavior) but readable layout.

## Known gotchas

### `dotnet build` doesn't run `ScriptGenerate` unless `GenerateJs=True`

A library project that's referenced by a JS-emitting app doesn't need `GenerateJs` itself — only the entry assembly does. The DLL with `$$BstInfo$$` is what `cs2jsc` consumes; emitting `.js` per library would produce many disconnected files.

### `<NoStandardLib>true</NoStandardLib>` is not optional

The custom `csc.cmd` requires it. Without it, MSBuild also references the real `System.dll`, which collides with NScript's `System` namespace types.

### `TargetFramework=netstandard2.1` is a contract

NScript's `mscorlib` clone targets `netstandard2.1`. Don't change this in a project file — `csc.cmd` won't recognise newer TFMs and types will fail to resolve.

### Framework tests need `Configuration=Debug` of the *toolset*

`Test/Framework/Directory.build.props` points `CscToolPath` to `NScriptToolSet/bin/Debug/`. If you only ran `dotnet build -c Release`, `Test/Framework/*` projects fail to find the compiler. Always do `dotnet build NScript_Full.sln -c Debug` once before running framework tests.

### `<JsOutputPath>` directories must exist

`Sdk.targets` includes `<MakeDir>` for `$(JsOutputPath)` but only inside `ScriptGenerate`. Some app `.csproj` files add their own `EnsureJsOutputDirectory` target with `BeforeTargets="ScriptGenerate"` for portability — see TodoApp.csproj.

### Wrapper `csc.cmd` / `nscript.cmd` are Windows-shaped

The SDK ships `.cmd` wrappers. On non-Windows hosts you'd need shell equivalents — the project doesn't currently target Linux/macOS. (Tracked separately; not in scope for the docs.)

### Incremental builds depend on input/output timestamps

`ScriptGenerate` uses `Inputs="...DLL;@(ReferencePath)"` and `Outputs="...js"`. If you edit a `.cs` file but the resulting DLL has the same timestamp (rare but possible with deterministic builds), `ScriptGenerate` may be skipped. Force rebuild with `dotnet build /t:Rebuild` if in doubt.

### `<Minify>` defaults differ in Release

In Release configuration, `Minify`, `Uglify`, and `JsOptimize` all default to `true`. If you want unminified Release output for debugging, you must explicitly set them to `false`.

## Diagnostics

| Symptom | Cause |
|---|---|
| `MSB4019: The imported project "Sdk.props" was not found` | `<NScriptSdkDir>` not set or NuGet package missing; check `Mcqdb.NScript.Sdk` reference |
| `error CS0518: Predefined type 'System.Object' is not defined` | `<NoStandardLib>true</NoStandardLib>` missing — BCL conflict |
| Build succeeds but no `.js` output | `<GenerateJs>True</GenerateJs>` missing |
| `BstInfo resource not found` from `cs2jsc` | The DLL was compiled by stock `csc.exe`, not the NScript `csc.cmd` — check `<CscToolPath>` |
| `ScriptGenerate` skipped on every build | Inputs/outputs check determined nothing changed; `dotnet build /t:Rebuild` to force |
| Plugin from `PluginConfig.xml` not loaded | `<PluginConfig>` MSBuild property missing or path wrong |

## Cross-links

- [Getting started](../getting-started/README.md) — first-project walkthrough
- [Compiler pipeline](../compiler/pipeline.md) — what `ScriptGenerate` actually invokes
- [Compiler plugins](../compiler/plugins.md) — `PluginConfig.xml` shape
- [Source maps](../debugging/source-maps.md) — `<SourceMapRoot>` configuration
- [Testing](../testing/README.md) — Debug-vs-Release toolset requirements
- [ADR 0006 — Compiler pipeline](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
