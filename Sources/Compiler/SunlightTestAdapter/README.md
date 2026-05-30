# Mcqdb.NScript.SunlightTestAdapter

A VSTest adapter for NScript / SunlightUnit. Lets `dotnet test` discover and
run NScript-compiled QUnit browser tests — no separate Node runner, no manual
QUnit page wiring.

## What it does

1. **Discovery** — Reads `[TestFixture]` / `[Test]` attribute metadata from a
   NScript-compiled test DLL using `MetadataLoadContext`, so the NScript
   `mscorlib` facade does not need to resolve at runtime.
2. **Execution** — Launches headless Chromium via `Microsoft.Playwright`,
   serves the NScript-emitted JavaScript bundle from a synthetic URL, loads
   QUnit 2.x, and reads `testEnd` / `runEnd` events back to VSTest.

## Install

In a vanilla .NET 8 wrapper project alongside your NScript test project:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <RunSettingsFilePath>$(MSBuildThisFileDirectory)sunlight.runsettings</RunSettingsFilePath>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="Mcqdb.NScript.SunlightTestAdapter" Version="1.1.0" />
  </ItemGroup>
</Project>
```

The NScript-compiled test DLL itself targets `netstandard2.1` with a custom
`mscorlib` facade and cannot reference `Microsoft.NET.Test.Sdk` directly —
hence the thin wrapper above. The wrapper should copy your NScript-compiled
test DLL (and its SunlightUnit / framework siblings) into its own `bin/` so
the adapter can resolve attribute metadata.

## Configure

Point the adapter at your test assembly and the NScript-emitted JS bundle via
a `.runsettings` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <TargetFrameworkVersion>net8.0</TargetFrameworkVersion>
  </RunConfiguration>
  <NSTest>
    <!-- Filename of the NScript-compiled test DLL, resolved relative to the
         wrapper's bin/ at runtime. -->
    <TestSourceAssembly>YourSuite.Test.dll</TestSourceAssembly>
    <!-- Path to the NScript-emitted JS bundle. Relative paths resolve against
         the test DLL directory first, then the current working directory. -->
    <JsFilePath>../../../../path/to/GeneratedScripts/YourSuite.Test.js</JsFilePath>
    <!-- Optional. When set, captures structured client logs as JSONL.
         See "Structured log capture" below. Omit for unchanged behavior. -->
    <LogEndpoint>./logs/browser-tests.jsonl</LogEndpoint>
  </NSTest>
</RunSettings>
```

| Setting | Required | Purpose |
|---------|----------|---------|
| `TestSourceAssembly` | yes | Filename of the NScript-compiled test DLL, resolved relative to the wrapper's `bin/`. |
| `JsFilePath` | yes | Path to the NScript-emitted JS bundle (relative to the test DLL dir, then CWD). |
| `LogEndpoint` | no | When set, the adapter writes structured client + adapter logs to this JSONL file. When unset, behavior is byte-identical to today. |

## Structured log capture

By default the adapter serves the test bundle via Playwright route
interception and forwards browser `console` output to VSTest's logger as
`[browser:log] <text>` — handy for live triage, but not persisted in a
machine-readable shape.

Set `<LogEndpoint>` to capture structured logs instead. When it is present,
the executor boots a single in-process Kestrel host on a loopback port that
serves the test-bundle static assets **and** hosts the
`Sunlight.Logging.Server` ingestion endpoints (`/_log`, `/_log/ws`) at the
same origin. Browser-side `Sunlight.Framework.Logger` events
(`Sunlight.Browser.*`) and adapter lifecycle events (`SunlightTestAdapter.*`)
both flow through one Serilog `CompactJsonFormatter` file sink, so they land
in the configured file — one JSON object per line — ready for `jq`, DuckDB,
or a log shipper.

The browser bundle opts in by clearing the default sink and wiring the
factory at startup:

```csharp
Logger.ClearSinks();
Logger.AddSink(LogSinkFactory.CreateFromBootstrap(new WindowTimer()));
```

`CreateFromBootstrap` reads the `window.__nscriptLogEndpoint` /
`window.__nscriptLogWsEndpoint` globals the host page injects and returns the
matching sink (WebSocket-primary with HTTP failover when both are present).
When `LogEndpoint` is unset, none of this is wired and the legacy
`[browser:log]` forwarding stays in effect.

See [`docs/framework-logging.md`](../../../docs/framework-logging.md) for the
full event schema, sink reference, and server-side ingestion API.

## Run

```bash
dotnet test path/to/YourSuite.Test.Runner.csproj -c Debug
```

The first build of any consumer project invokes `playwright install chromium`
once via this package's `.targets`; subsequent builds short-circuit on a
sentinel file in `bin/`. Opt out by setting `NSTestSkipPlaywrightInstall=true`
in CI environments that provision Chromium externally.

## Caveats

- **One bundle per wrapper.** Each NScript suite must run in its own wrapper
  project / runsettings combo. Two NScript bundles co-loaded in the same page
  collide on shared `Function.prototype` runtime slots after minification.
- **SunlightUnit is not transitive.** You write `[TestFixture]` / `[Test]`
  attributes in your own test project — install `Mcqdb.NScript.SunlightUnit`
  there directly.

## Source

`Sources/Compiler/SunlightTestAdapter/` in the NScript repository.
