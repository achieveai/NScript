# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## What is NScript

NScript is a **C# to JavaScript transpiler** — a compiler + runtime framework that translates C# source code into JavaScript for client-side web applications. It is NOT a simple source-to-source transpiler; it uses a **multi-stage pipeline** through Roslyn bound trees.

## Build Commands

```bash
# Build the full solution
dotnet build NScript_Full.sln -c Release

# Build in Debug (required before running framework tests — they use Debug compiler)
dotnet build NScript_Full.sln -c Debug

# Run all tests
dotnet test NScript_Full.sln -c Release

# Run a single test project
dotnet test Test/Compiler/CssParser.Test/CssParser.Test.csproj

# Run a specific test by name
dotnet test Test/Compiler/NScriptTest/NScriptTest.csproj --filter "FullyQualifiedName~TestMethodName"
```

**NuGet package generation**: Set `<GenerateNScriptPackages>true</GenerateNScriptPackages>` in root `Directory.Build.props`, then build Release. Packages output to `NScriptToolSet/`.

## Compiler Pipeline (Critical to Understand)

The compilation is a **two-stage process with an assembly as the handoff artifact**:

**Stage 1 — Roslyn compilation** (`NScript.Csc.Lib`):
- Subclasses Roslyn's `CSharpCompiler` using a **minimal Roslyn fork** (`roslyn/` submodule, branch `features/physhi-updated`)
- Hooks into `CSharpCompilation.OnBoundExpressionGenerated` to capture bound method bodies
- Serializes bound AST via `SymbolSerializer` and embeds it as `$$BstInfo$$` resource in the output DLL
- The emitted DLL is the durable handoff artifact between stages

**Stage 2 — JavaScript emission** (`Cs2Jsc`):
- Loads compiled DLL via `ClrContext` (Mono.Cecil-based, NOT System.Reflection)
- Extracts `$$BstInfo$$` and `$$ResInfo$$` resources
- Reconstructs AST via `BondToAst`, converts to JST (JavaScript AST), emits JavaScript

## Roslyn Fork

The `roslyn/` submodule is a **narrow, surgical fork** — not a broad compiler fork. Changes are limited to:
1. `OnBoundExpressionGenerated` callback in `CSharpCompilation` / `MethodCompiler`
2. `OnBeforeCompilation` hook in `CommonCompiler`
3. `InternalsVisibleTo` for `NScript.csc.lib`
4. Disabled assembly signing for private builds

Forked binaries are checked into `Dependencies/Roslyn/`. See `roslyn/AGENTS.md` for fork maintenance details. See `docs/adr/0002-*` through `docs/adr/0005-*` for architectural decisions.

## Project Layout

| Directory | Purpose | Target Framework |
|-----------|---------|-----------------|
| `Sources/Compiler/` | Compiler pipeline (18 projects) | `net6.0` |
| `Sources/Framework/` | Runtime libraries compiled to JS | `netstandard2.1` |
| `Test/Compiler/` | Compiler unit/integration tests | `net6.0` |
| `Test/Framework/` | Framework behavioral tests (compiled to JS) | `netstandard2.1` |
| `Dependencies/Roslyn/` | Checked-in forked Roslyn binaries | — |
| `NScriptToolSet/` | Build output + NuGet packages | — |
| `docs/adr/` | 24 Architecture Decision Records | — |

### Framework projects compile with a custom CSC

The `Sources/Framework/Directory.Build.props` sets `NoStdLib=true`, `CscToolPath` pointing to the NScript compiler in `NScriptToolSet/bin/`, and `LangVersion=8.0`. Framework projects target `netstandard2.1` but are compiled by NScript's custom compiler to produce DLLs with embedded AST resources.

### Framework tests require Debug build first

`Test/Framework/Directory.build.props` points `CscToolPath` to `NScriptToolSet/bin/Debug/` — framework tests always use the Debug-built compiler.

## Key Architectural Patterns

### Plugin Architecture
Converter pipeline is extensible via `IConverterPlugin` (`IMethodConverterPlugin`, `ITypeConverterPlugin`, `IRuntimeConverterPlugin`). Plugins are registered in `PluginConfig.xml` files (XML, not reflection-based discovery). Built-in plugins: `XwmlTemplatingPlugin` (template processing), `TestGenerator` (test emission).

### Observable Framework (Platform Contract)
The `Sunlight.Framework.Observables` framework is NOT optional — it is wired into template code generation (`XwmlParser`), property change notification, and collection tracking. Key base class: `ObservableObject`. Uses string-based property names (optimized for JS generation). `AutoFireAttribute` on properties auto-emits change notifications.

### XWML Templates
Custom XML-based template language parsed by `XwmlParser`. Templates use `{PropertyName}` binding syntax with OneTime/OneWay/TwoWay modes. Template `.html` files live alongside `.cs` view classes.

### JavaScript AST (JST)
`NScript.JST` defines the complete JavaScript syntax tree as the canonical intermediate representation. Uses visitor pattern for traversal and code generation.

### Entry Point Convention
Application entry detected via `[EntryPoint]` attribute on a class containing a public static `Main()` method.

## ADR Index

Architecture Decision Records in `docs/adr/` document all major design decisions (24 total, all Accepted).

### Engineering Process
| ADR | File | Decision |
|-----|------|----------|
| 0001 | `0001-adopt-architecture-decision-records.md` | Adopt ADRs in `docs/adr/` with sequential numbering and standardized template |

### Roslyn Fork & Compiler Integration
| ADR | File | Decision |
|-----|------|----------|
| 0002 | `0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md` | Minimal Roslyn fork on `features/physhi-updated` with surgical extension points for bound-body access |
| 0003 | `0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md` | Submodule is source of truth; normal builds consume checked-in binaries from `Dependencies/Roslyn` |
| 0004 | `0004-define-when-and-how-to-refresh-the-checked-in-roslyn-drop.md` | Refresh binary drop deliberately on concrete need; validate via full build + integration tests |
| 0005 | `0005-constrain-nscript-to-a-small-roslyn-integration-contract.md` | Roslyn internals isolated to `NScript.Csc.Lib`; downstream stages consume serialized AST resources only |

### Compiler Pipeline & Architecture
| ADR | File | Decision |
|-----|------|----------|
| 0006 | `0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md` | Two-stage pipeline: Roslyn compile → DLL with embedded AST → `cs2jsc` → JavaScript |
| 0013 | `0013-define-nscript-as-a-multi-frontend-translation-architecture.md` | Multiple frontends (C#/Roslyn, inline JS, XWML, Razor) converge on shared JST backend |

### JavaScript Runtime & Type System
| ADR | File | Decision |
|-----|------|----------|
| 0007 | `0007-define-the-javascript-runtime-type-model.md` | JS function objects as type objects with .NET-like metadata (FullName, TypeId, BaseType, interfaces) |
| 0008 | `0008-define-how-class-and-interface-hierarchies-map-to-javascript.md` | Classes use prototype chains; interfaces use explicit runtime metadata maps via `baseInterfaces` |
| 0010 | `0010-model-native-javascript-types-through-attributed-clr-facades.md` | Native JS types modeled via attributed CLR facades (`ImportedType`, `Extended`, `JsonType`) |
| 0011 | `0011-treat-arrays-as-a-special-wrapped-runtime-substrate.md` | Arrays wrapped via `ArrayG<T>`/`NativeArray<T>` backed by native JS arrays |

### JavaScript Interop
| ADR | File | Decision |
|-----|------|----------|
| 0009 | `0009-prefer-inline-script-attribute-for-javascript-dependencies.md` | JS dependency bodies inline on `ScriptAttribute` annotations, not in separate files |
| 0012 | `0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md` | Inline script bodies parsed into JST, resolved against scope; unresolved names fail compilation |

### Template System
| ADR | File | Decision |
|-----|------|----------|
| 0016 | `0016-standardize-xwml-as-the-canonical-template-language-with-strict-css-and-binding-diagnostics.md` | XWML is canonical template language with strict CSS class matching and typed binding syntax |
| 0017 | `0017-add-razor-skin-templates-as-a-second-template-frontend.md` | Razor `.skin.cshtml` added as second frontend with full C# expressions; both coexist |
| 0018 | `0018-replace-independent-binders-with-compile-time-reactive-binding-graph.md` | Razor uses compile-time DAG binding model with deduplication and comment markers |
| 0019 | `0019-extract-ibindingstrategy-from-skininstance.md` | `SkinInstance` delegates binding to `IBindingStrategy` (`LegacyBinderStrategy`/`GraphBindingStrategy`) |
| 0020 | `0020-auto-detect-binding-mode-from-roslyn-semantic-analysis.md` | Auto-classify binding modes via Roslyn semantic analysis; no manual mode annotations |

### Framework & Reactive System
| ADR | File | Decision |
|-----|------|----------|
| 0014 | `0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md` | `Sunlight.Framework.Observables` is the canonical reactive binding contract |
| 0015 | `0015-defer-layout-sensitive-dom-reads-and-batch-them.md` | Layout-sensitive DOM reads batched through deferred `requestAnimationFrame` callbacks |

### Optimization & Code Generation
| ADR | File | Decision |
|-----|------|----------|
| 0021 | `0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md` | All generated JS symbols must be resolved `IIdentifier` objects, never raw strings |
| 0022 | `0022-demand-driven-conversion-and-dead-code-elimination.md` | Demand-driven conversion from entry points; identifier-level dead code elimination |
| 0023 | `0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md` | Non-virtual methods devirtualized to static functions; trivial accessors inlined |
| 0024 | `0024-deduplicate-structurally-identical-functions-after-minification.md` | Post-minification deduplication of structurally identical function bodies |

## C# Feature Support

Supported: classes, interfaces, generics, LINQ, lambdas, async/await, null-coalescing, pattern matching, indices/ranges, C# 8 features.

NOT supported: `dynamic`, `yield break`/`yield return`, reflection, P/Invoke. See `csharp8-todos.md` for detailed C# 8 status.

## JST Code Generation Rules (CRITICAL)

When generating JavaScript AST (JST) nodes — especially in codegen plugins like `RazorSkinParser` or `XwmlParser`:

1. **ALL identifiers must be resolved through the scope system.** Never use raw string names for variables, fields, methods, or types. The compiler renames everything during minification. Use `RuntimeScopeManager.Resolve()`, `ResolveStatic()`, `ResolveType()`, or `ResolveFactory()`.

2. **Object literal field keys:** Use `InlineObjectInitializer.AddInitializer(IIdentifier, Expression)` with a resolved field identifier, NOT the `(string, Expression)` overload. String keys produce unminified names that don't match runtime field access.

3. **Type constructors (parameterless):** Use `ResolveType(typeDef)[0]` + `new` syntax. `ResolveFactory()` only works for constructors WITH parameters — parameterless constructors don't get factory functions in NScript.

4. **`[JsonType]` attribute:** Adds `importedExtension` wrapper on field access. Don't use for types created as object literals in codegen — the wrapper won't match. Use typed instances (`new Type()`) instead.

5. **Raw body function expressions:** If unavoidable (complex computed expressions), use `enforceSuggestion=true` on the `IdentifierScope` so parameter names match the raw body text. Prefer fully resolved JST expressions where possible.

## Prerequisites

- .NET 8.0 SDK (version 8.0.416, see `global.json`)
- The solution uses a custom MSBuild SDK: `Mcqdb.NScript.Sdk` v1.0.4-beta1
