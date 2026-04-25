# Compiler pipeline

> **Audience:** *Contributors* working on the NScript compiler — anyone touching `Sources/Compiler/`.

## TL;DR

NScript is a **two-stage compiler**, not a source-to-source transpiler. Stage 1 (`NScript.Csc.Lib`) subclasses Roslyn's `CSharpCompiler` and uses a [minimal Roslyn fork](../adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md) to capture bound method bodies, serialise them into NScript's AST format, and embed them as a `$$BstInfo$$` resource in the emitted DLL. Stage 2 (`Cs2Jsc`) loads the DLL via `ClrContext` (Mono.Cecil), extracts the AST resource, reconstructs an NScript CLR AST via `BondToAst`, lowers to JST (JavaScript AST), and emits JS via `cs2jsc`. The DLL is the **durable handoff artifact**. Pipeline canonicalised in [ADR 0006](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md).

## Reference — pipeline diagram

```mermaid
flowchart TB
    subgraph Stage1[Stage 1 — Roslyn compilation \(NScript.Csc.Lib\)]
      A[".cs source files"] --> B["Roslyn CSharpCompilation<br/>\(forked: OnBoundExpressionGenerated\)"]
      B --> C["BoundExpression capture"]
      C --> D["SymbolSerializer<br/>(NScript AST → bytes)"]
      D --> E["Embed as $$BstInfo$$<br/>resource in DLL"]
      E --> F[("Compiled .dll<br/>with embedded AST")]
    end

    subgraph Stage2[Stage 2 — JS emission \(Cs2Jsc\)]
      F --> G["ClrContext<br/>\(Mono.Cecil load\)"]
      G --> H["Extract $$BstInfo$$<br/>and $$ResInfo$$"]
      H --> I["BondToAst<br/>(AST reconstruction)"]
      I --> J["NScript.Converter<br/>(CLR AST → JST)"]
      J --> K["JST optimiser<br/>(devirtualise, dedup, DCE)"]
      K --> L["cs2jsc emit"]
      L --> M[("Output .js + .map")]
    end
```

## Reference — project map

| Project | Role |
|---|---|
| `NScript.Csc.Lib` | Roslyn subclass; capture bound bodies; embed `$$BstInfo$$` resource |
| `JsCsc` / `JsCsc.Lib` | The `csc.exe`-equivalent CLI that drives Stage 1 |
| `NScript.CLR` | Mono.Cecil-based assembly loader (`ClrContext`) — loads compiled DLLs into the converter |
| `NScript.Converter` | The core compiler: bound AST → JST. Hosts `Builder`, `RuntimeScopeManager`, `MethodConverter`, `TypeScopeManager`. |
| `NScript.Converter.Plugins` | Built-in plugins: `XwmlTemplatingPlugin`, `TestGenerator`. |
| `NScript.JST` | JST (JavaScript AST) — canonical IR with visitor-pattern traversal. |
| `NScript.JSParser` | Parser used by `[Script]` body resolution and Razor expression parsing. |
| `Cs2Jsc` | The CLI orchestrator: loads DLLs, runs the converter, emits JS files. |
| `XwmlParser` | XML-based template parser (XWML frontend). |
| `RazorSkinParser` | `.skin.cshtml` template parser (Razor frontend, [ADR 0017](../adr/0017-add-razor-skin-templates-as-a-second-template-frontend.md)). |
| `NScript.Template.Compiler` | Shared template-compiler infrastructure used by both frontends. |
| `CssParser` | Strict CSS class parser used by template diagnostics ([ADR 0016](../adr/0016-standardize-xwml-as-the-canonical-template-language-with-strict-css-and-binding-diagnostics.md)). |
| `Autoprefixer` | Vendor-prefix injection over parsed CSS. |
| `SourceMap` / `SourceMap.Server` | Source-map generation and the WI-15 source-map dev server. See [debugging/source-maps.md](../debugging/source-maps.md). |
| `NScript.Lib` / `NScript.Utils` | Shared utilities and reference-data lookup. |
| `NScript.Sdk` | MSBuild SDK that wires `JsCsc` into `dotnet build`. See [build/msbuild-sdk.md](../build/msbuild-sdk.md). |

## Reference — handoff resources

| Resource | Producer | Consumer | Contents |
|---|---|---|---|
| `$$BstInfo$$` | `NScript.Csc.Lib::SerializationHelper` | `NScript.Converter::ConverterContext` | Serialised NScript CLR AST for every method body in the assembly |
| `$$ResInfo$$` | `NScript.Csc.Lib::Csc` | `NScript.Converter::ConverterContext` | Serialised resource bundle (strings, embedded files via `[Resources]`) |

The DLL is the *only* coupling between Stage 1 and Stage 2 — there is no shared in-memory state, no Roslyn handle, no source-text dependency. This makes incremental builds trivial: Stage 2 only re-runs for changed DLLs.

## Reference — the Roslyn fork

NScript depends on a [narrow surgical fork](../adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md) of the Roslyn C# compiler. The diff against upstream is intentionally small ([ADR 0005](../adr/0005-constrain-nscript-to-a-small-roslyn-integration-contract.md)):

1. `CSharpCompilation.OnBoundExpressionGenerated` — callback fired for each bound method body.
2. `MethodCompiler.OnBoundExpressionGenerated` — propagation through the compiler pipeline.
3. `CommonCompiler.OnBeforeCompilation` — early hook for resource injection.
4. `InternalsVisibleTo("NScript.csc.lib")` — exposes Roslyn internals to the subclass.
5. Disabled assembly signing for private builds.

The forked binaries live in `Dependencies/Roslyn/` and are checked into the repo. The fork source is a git submodule at `roslyn/` on branch `features/physhi-updated`. See [ADR 0003](../adr/0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md) and [ADR 0004](../adr/0004-define-when-and-how-to-refresh-the-checked-in-roslyn-drop.md) for refresh procedures.

## Reference — JST optimisation passes

Once the converter has produced JST, several passes run before emit:

1. **Demand-driven conversion + DCE** ([ADR 0022](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)) — only types/methods reachable from `[EntryPoint]` are converted; unreached identifiers are eliminated.
2. **Devirtualisation + accessor inlining** ([ADR 0023](../adr/0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md)) — non-virtual methods become static functions; trivial getters/setters inline at call sites.
3. **Identifier resolution** ([ADR 0021](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md)) — every JST identifier is replaced with a resolved `IIdentifier` object before emission.
4. **Minification** — short identifier substitution.
5. **Function deduplication** ([ADR 0024](../adr/0024-deduplicate-structurally-identical-functions-after-minification.md)) — post-minification, structurally identical function bodies share a single emitted definition.

## Quick start — build and trace one file through the pipeline

```bash
# Build full solution Debug (required for framework tests, useful for tracing)
dotnet build NScript_Full.sln -c Debug

# Build a single .NScriptproj
dotnet build Test/Framework/TodoApp/TodoApp.csproj -c Debug

# Inspect the embedded resource on a Stage-1 output DLL:
ildasm /text Test/Framework/TodoApp/bin/Debug/netstandard2.1/TodoApp.dll \
  | grep -A2 "BstInfo"

# Force-trace Stage 2 emission (env var honored by Cs2Jsc):
NSCRIPT_TRACE_EMIT=1 \
  NScriptToolSet/bin/Debug/cs2jsc.exe \
  --in Test/Framework/TodoApp/bin/Debug/netstandard2.1/TodoApp.dll \
  --out /tmp/todoapp.js
```

The trace flag dumps the JST tree before and after each optimisation pass. Useful for understanding why a particular call site emits the way it does.

## Examples

### Tracing what reaches the JS output

A common contributor question: "why didn't this method appear in the emitted JS?" Answer: demand-driven conversion ([ADR 0022](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)) walks from `[EntryPoint]` and only emits transitively reachable identifiers. To trace:

1. Verify the entry point: `grep -r "EntryPoint" Test/Framework/TodoApp/`
2. Check whether your method is called from a reachable call site.
3. Look at the converter's reachability log: set `NSCRIPT_TRACE_DCE=1` and re-run.

If a method should be reachable but isn't, the most common cause is that the call site lives inside a `[Conditional("DEBUG")]` method whose caller compiled in Release.

### Adding a new built-in optimisation pass

New passes plug into `NScript.Converter::Builder` after the JST is built. The pattern:

```csharp
public class MyOptimisationPass : IConverterPass
{
    public void Run(ConverterContext ctx)
    {
        foreach (var fn in ctx.AllFunctions)
        {
            // mutate fn.Body via JST visitor
        }
    }
}
```

Register in `Builder.cs` and add tests under `Test/Compiler/NScriptTest/`.

## Known gotchas

### Stage 1 must complete before Stage 2 reads the DLL

If you're invoking the compiler manually, run `JsCsc` first to produce the DLL with `$$BstInfo$$`. Calling `Cs2Jsc` against a DLL produced by stock `csc.exe` produces "missing AST resource" errors.

### `ClrContext` uses Mono.Cecil, not `System.Reflection`

The DLL loader is Mono.Cecil-based. This matters for contributors: don't reach for `Assembly.Load` / `Type.GetType` inside the converter. Use `ClrContext` accessors. Reflection-based lookups will be slower and inconsistent with how the rest of the converter sees types.

### Framework tests require a Debug build first

`Test/Framework/Directory.build.props` points `CscToolPath` to `NScriptToolSet/bin/Debug/`. If you've only built Release, framework tests fail to compile their source files. Always run `dotnet build -c Debug` once before framework tests.

### The Roslyn fork must be refreshed deliberately

Don't update `Dependencies/Roslyn/` casually. The drop is binary; it must be regenerated from the `roslyn/` submodule on `features/physhi-updated` and validated through the full build + integration test suite. See [ADR 0004](../adr/0004-define-when-and-how-to-refresh-the-checked-in-roslyn-drop.md).

### Identifier renames after JST construction break codegen plugins

Codegen plugins ([compiler/plugins.md](plugins.md)) emit JST nodes that reference identifiers. The minifier renames symbols *after* plugin emission. If a plugin emits a raw string identifier (instead of resolving via `RuntimeScopeManager`), the rename desyncs and the plugin's output references the wrong name. [ADR 0021](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md) is non-negotiable.

### `$$BstInfo$$` format is internal

The serialisation format is not stable across NScript versions. Don't try to parse it externally. If you need to inspect AST, use `Cs2Jsc --dump-ast` against the DLL.

## Diagnostics

| Symptom | Cause |
|---|---|
| `BstInfo resource not found in DLL` | DLL was compiled by stock `csc.exe`, not `JsCsc` |
| `Cannot deserialize BstInfo: version mismatch` | DLL compiled with a different NScript version than the one running `Cs2Jsc` |
| Method missing from JS output | Demand-driven DCE eliminated it — check reachability from `[EntryPoint]` |
| `Identifier 'X' not resolved` from a plugin | Plugin emitted a raw string instead of going through `RuntimeScopeManager.Resolve` |
| Roslyn-side compile error mentions `OnBoundExpressionGenerated` | Forked binaries missing or corrupt; rebuild from `roslyn/` submodule |

## Cross-links

- [ADR 0002 — Roslyn fork](../adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md)
- [ADR 0005 — Roslyn integration contract](../adr/0005-constrain-nscript-to-a-small-roslyn-integration-contract.md)
- [ADR 0006 — Compiler pipeline](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
- [ADR 0013 — Multi-frontend architecture](../adr/0013-define-nscript-as-a-multi-frontend-translation-architecture.md)
- [ADR 0021 — Resolved identifiers](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md)
- [ADR 0022 — DCE](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)
- [ADR 0023 — Devirtualisation](../adr/0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md)
- [ADR 0024 — Function dedup](../adr/0024-deduplicate-structurally-identical-functions-after-minification.md)
- [Compiler plugins](plugins.md)
- [MSBuild SDK](../build/msbuild-sdk.md)
- [Source maps](../debugging/source-maps.md)
