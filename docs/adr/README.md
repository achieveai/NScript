# Architecture Decision Records

This directory stores Architecture Decision Records (ADRs) for NScript.

An ADR captures a single important technical decision, the context behind it, the options considered, and the consequences of the choice.

## Naming

Use the format `NNNN-short-title.md`.

Examples:

- `0001-adopt-architecture-decision-records.md`
- `0002-standardize-compiler-entry-point.md`

Number ADRs sequentially so they remain easy to reference in discussions and pull requests.

## Status

Use one of these statuses near the top of each ADR:

- `Proposed`
- `Accepted`
- `Superseded`
- `Deprecated`

If an ADR replaces an older one, mention the earlier ADR in the new record and update the older record to note that it has been superseded.

## Process

1. Copy [adr-template.md](./adr-template.md) to a new numbered file.
2. Fill in the context, options, decision, and consequences.
3. Open a pull request and review the ADR alongside the code change it affects.
4. Mark the ADR as `Accepted` when the decision is approved.

## Scope

Create an ADR when a decision is likely to matter beyond a single small code change, especially for:

- compiler architecture
- framework boundaries
- build and packaging strategy
- external dependencies
- testing strategy
- public extension points

Avoid ADRs for routine refactors or localized implementation details that do not carry broader architectural impact.

## Index

| ADR | Title | Status |
| --- | ----- | ------ |
| [0001](0001-adopt-architecture-decision-records.md) | Adopt Architecture Decision Records | Accepted |
| [0002](0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md) | Maintain a Minimal Roslyn Fork for NScript Hooks | Accepted |
| [0003](0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md) | Define How NScript Consumes and Updates Forked Roslyn Binaries | Accepted |
| [0004](0004-define-when-and-how-to-refresh-the-checked-in-roslyn-drop.md) | Define When and How to Refresh the Checked-In Roslyn Drop | Accepted |
| [0005](0005-constrain-nscript-to-a-small-roslyn-integration-contract.md) | Constrain NScript to a Small Roslyn Integration Contract | Accepted |
| [0006](0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md) | Standardize the Compiler Pipeline from Bound C# to JavaScript | Accepted |
| [0007](0007-define-the-javascript-runtime-type-model.md) | Define the JavaScript Runtime Type Model | Accepted |
| [0008](0008-define-how-class-and-interface-hierarchies-map-to-javascript.md) | Define How Class and Interface Hierarchies Map to JavaScript | Accepted |
| [0009](0009-prefer-inline-script-attribute-for-javascript-dependencies.md) | Prefer Inline Script Attribute for JavaScript Dependencies | Accepted |
| [0010](0010-model-native-javascript-types-through-attributed-clr-facades.md) | Model Native JavaScript Types Through Attributed CLR Facades | Accepted |
| [0011](0011-treat-arrays-as-a-special-wrapped-runtime-substrate.md) | Treat Arrays as a Special Wrapped Runtime Substrate | Accepted |
| [0012](0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md) | Parse and Resolve Script Blocks Against Types, Members, and Known Globals | Accepted |
| [0013](0013-define-nscript-as-a-multi-frontend-translation-architecture.md) | Define NScript as a Multi-Frontend Translation Architecture | Accepted |
| [0014](0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md) | Standardize the Observable Framework as the Reactive Binding Contract | Accepted |
| [0015](0015-defer-layout-sensitive-dom-reads-and-batch-them.md) | Defer Layout-Sensitive DOM Reads and Batch Them | Accepted |
| [0016](0016-standardize-xwml-as-the-canonical-template-language-with-strict-css-and-binding-diagnostics.md) | Standardize XWML as the Canonical Template Language | Accepted |
| [0017](0017-add-razor-skin-templates-as-a-second-template-frontend.md) | Add Razor Skin Templates as a Second Template Frontend | Accepted |
| [0018](0018-replace-independent-binders-with-compile-time-reactive-binding-graph.md) | Replace Independent Binders with Compile-Time Reactive Binding Graph | Accepted |
| [0019](0019-extract-ibindingstrategy-from-skininstance.md) | Extract IBindingStrategy from SkinInstance | Accepted |
| [0020](0020-auto-detect-binding-mode-from-roslyn-semantic-analysis.md) | Auto-Detect Binding Mode from Roslyn Semantic Analysis | Accepted |
| [0021](0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md) | Require Resolved Identifiers for All Generated JavaScript Symbols | Accepted |
| [0022](0022-demand-driven-conversion-and-dead-code-elimination.md) | Demand-Driven Conversion and Dead Code Elimination | Accepted |
| [0023](0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md) | Devirtualize Non-Virtual Methods and Inline Trivial Accessors | Accepted |
