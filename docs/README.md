# NScript Documentation

NScript is a **C# to JavaScript transpiler** plus a runtime framework for client-side web applications. Source C# is compiled by a custom Roslyn-based front end into a DLL with an embedded bound AST, then a second stage (`cs2jsc`) emits the JavaScript. This documentation covers both sides of that contract: how to *write* an NScript application and how the toolchain *processes* it.

## Audience routing

If you are **writing an NScript application**, start with:

1. [Getting started](getting-started/README.md) — prerequisites, first app, day-1 pitfalls
2. [Framework: Core BCL surface](framework/core.md) — what works in `mscorlib` / `System.Core`
3. [Framework: Web & DOM](framework/web.md) — `System.Web` and `System.Web.Html`
4. [Framework: Sunlight Core](framework/sunlight-core.md) — `ObservableObject`, binders, `IoC`, `Logger`, `TaskScheduler`, `CallContext`
5. [Framework: Sunlight UI](framework/sunlight-ui.md) — `UIElement`, `ListView`, `SkinFactory`, binding-strategy attributes
6. [Templates: Razor `.skin.cshtml`](templates/razor.md) — modern template frontend
7. [Templates: XWML (legacy)](templates/xwml.md) — original XML-based templates
8. [Interop: attributes reference](interop/attributes.md) — every script-attribute you might apply
9. [Interop: JsonType & ImportedType patterns](interop/json-and-imported-types.md) — typed JS interop
10. [Interop: dynamic types](interop/dynamic.md) — `System.Dynamic` support level
11. [Language: limitations & unsupported C# features](language/limitations.md) — what NScript will *not* compile
12. [Debugging: source maps](debugging/source-maps.md) — debugging generated JS in the browser

If you are **contributing to the NScript toolchain itself**, start with:

1. [Compiler pipeline overview](compiler/pipeline.md) — two-stage architecture, JST, DCE, devirtualization, dedup
2. [Compiler plugins](compiler/plugins.md) — writing an `IConverterPlugin`
3. [Build & MSBuild SDK](build/msbuild-sdk.md) — `Mcqdb.NScript.Sdk`, `Directory.Build.props`, package generation
4. [Testing](testing/README.md) — compiler unit tests vs framework behavioral tests; SunlightUnit

## Status — DoD coverage for #42

| § | Page | Topic |
|---|------|-------|
| 1 | [framework/core.md](framework/core.md) | Framework Core (BCL surface) |
| 2 | [framework/web.md](framework/web.md) | Framework Web & DOM |
| 3 | [framework/sunlight-core.md](framework/sunlight-core.md) | Sunlight.Framework (core) |
| 4 | [framework/sunlight-ui.md](framework/sunlight-ui.md) | Sunlight.Framework.UI (controls) |
| 5 | [templates/razor.md](templates/razor.md) | Razor skin templates + CSS |
| 6 | [interop/attributes.md](interop/attributes.md) | Interop attributes reference |
| 7 | [interop/json-and-imported-types.md](interop/json-and-imported-types.md) | JsonType / ImportedType patterns |
| 8 | [interop/dynamic.md](interop/dynamic.md) | Dynamic types (`System.Dynamic`) |
| 9 | [templates/xwml.md](templates/xwml.md) | XWML templates (legacy) |
| 10 | [compiler/pipeline.md](compiler/pipeline.md) | Compiler pipeline overview |
| 11 | [compiler/plugins.md](compiler/plugins.md) | Plugin architecture |
| 12 | [build/msbuild-sdk.md](build/msbuild-sdk.md) | Build & MSBuild SDK |
| 13 | [getting-started/README.md](getting-started/README.md) | Getting started / Hello World |
| 14 | [language/limitations.md](language/limitations.md) | Limitations & unsupported features |
| 15 | [testing/README.md](testing/README.md) | Testing |
| 16 | [debugging/source-maps.md](debugging/source-maps.md) | Debugging & source maps |

## Other documentation in this repo

- [Architecture Decision Records](adr/) — 25 accepted ADRs covering Roslyn fork, pipeline shape, runtime type model, template system, optimization passes
- [Structured client logging](framework-logging.md) — `Logger`, `ILogSink`, JSON schema, server-side ingestion (Serilog / NLog)
- [Plans](plans/) — feature plans and execution notes

## Authoring conventions

When extending these docs:

- Use [`_template.md`](_template.md) as the page skeleton.
- Cross-link to ADRs in `adr/`; do not re-derive recorded decisions.
- Every code example must compile under NScript's supported C# subset (no `dynamic`, no `yield return`, no reflection, no P/Invoke). See [`language/limitations.md`](language/limitations.md).
- One H1 per page; H2/H3 below. Filenames are kebab-case.
- Diagrams: Mermaid first, images only when Mermaid can't express the shape.
- For logging / property-bag samples, always use `string[]` flat arrays — NScript renames C# field names during minification.
