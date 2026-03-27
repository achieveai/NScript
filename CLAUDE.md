# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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

Forked binaries are checked into `Dependencies/Roslyn/`. See `roslyn/CLAUDE.md` for fork maintenance details. See `docs/adr/0002-*` through `docs/adr/0005-*` for architectural decisions.

## Project Layout

| Directory | Purpose | Target Framework |
|-----------|---------|-----------------|
| `Sources/Compiler/` | Compiler pipeline (18 projects) | `net6.0` |
| `Sources/Framework/` | Runtime libraries compiled to JS | `netstandard2.1` |
| `Test/Compiler/` | Compiler unit/integration tests | `net6.0` |
| `Test/Framework/` | Framework behavioral tests (compiled to JS) | `netstandard2.1` |
| `Dependencies/Roslyn/` | Checked-in forked Roslyn binaries | — |
| `NScriptToolSet/` | Build output + NuGet packages | — |
| `docs/adr/` | 16 Architecture Decision Records | — |

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

Architecture Decision Records in `docs/adr/` document all major design decisions. Key ones:
- **ADR-0002–0005**: Roslyn fork strategy, consumption, refresh process, integration contract
- **ADR-0006**: Two-stage compiler pipeline (Bound C# → JavaScript)
- **ADR-0007–0008**: JavaScript runtime type model and class hierarchy mapping
- **ADR-0010**: Attributed CLR facades for native JS types
- **ADR-0014**: Observable framework as reactive binding contract
- **ADR-0016**: XWML template language with strict CSS/binding diagnostics

## C# Feature Support

Supported: classes, interfaces, generics, LINQ, lambdas, async/await, null-coalescing, pattern matching, indices/ranges, C# 8 features.

NOT supported: `dynamic`, `yield break`/`yield return`, reflection, P/Invoke. See `csharp8-todos.md` for detailed C# 8 status.

## Prerequisites

- .NET 8.0 SDK (version 8.0.416, see `global.json`)
- The solution uses a custom MSBuild SDK: `Mcqdb.NScript.Sdk` v1.0.4-beta1
