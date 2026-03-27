# ADR 0005: Constrain NScript to a Small Roslyn Integration Contract

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Compiler integration boundary

## Context

ADR 0002 establishes that NScript uses a minimal Roslyn fork on `features/physhi-updated`. ADR 0003 and ADR 0004 establish how that fork is consumed and refreshed. The remaining architectural question is what NScript is actually allowed to depend on inside that fork.

Today the Roslyn-specific integration is concentrated in [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj), not spread across the rest of the compiler stack. Within that project, NScript depends on a narrow set of Roslyn-specific hooks, internal symbol APIs, and bound-tree node types to serialize Roslyn's post-binding method bodies into NScript's own serialized representation.

Examples in the current code include:

- the `OnBeforeCompilation` hook consumed by [Sources/Compiler/NScript.Csc.Lib/Csc.cs](../../Sources/Compiler/NScript.Csc.Lib/Csc.cs)
- the `OnBoundExpressionGenerated` callback consumed by [Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs](../../Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs)
- internal symbol bridging through `ISymbolInternal` in [Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs](../../Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs)
- Roslyn internal symbol types in [Sources/Compiler/NScript.Csc.Lib/SymbolSerializer.cs](../../Sources/Compiler/NScript.Csc.Lib/SymbolSerializer.cs)
- bound-tree traversal in [Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs](../../Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs)

The rest of the NScript pipeline consumes the serialized result from DLL resources and Mono.Cecil metadata rather than talking to Roslyn internals directly.

## Decision Drivers

- keep the Roslyn fork boundary small and reviewable
- prevent Roslyn internals from leaking across the broader NScript codebase
- preserve the current capture point NScript needs without deepening the fork unnecessarily
- make future Roslyn upgrades feasible by constraining the supported contract

## Options Considered

### Option 1: Explicitly constrain Roslyn internals usage to a small contract centered on `NScript.Csc.Lib`

Treat the Roslyn fork as exposing only a narrow supported integration surface and keep direct internal usage isolated to `NScript.Csc.Lib`.

Pros:

- keeps the dependency boundary understandable
- limits the cost of future Roslyn updates
- avoids coupling downstream converter and JavaScript-generation stages to Roslyn internals

Cons:

- requires discipline when adding new compiler features
- some future capabilities may need an explicit contract expansion

### Option 2: Allow Roslyn internals to be consumed anywhere in the compiler stack as needed

Let downstream NScript projects reference Roslyn internals directly whenever convenient.

Pros:

- lowest short-term friction for adding features
- no need to funnel every Roslyn interaction through one boundary project

Cons:

- spreads Roslyn coupling across the codebase
- makes upgrades and reasoning about the fork significantly harder
- weakens the architectural value of the minimal-fork strategy

### Option 3: Remove all Roslyn internal usage and rely only on public APIs immediately

Redesign the integration to avoid internal Roslyn dependencies entirely.

Pros:

- cleanest theoretical dependency boundary
- avoids friend-assembly and internal API coupling

Cons:

- does not match the current technical needs recorded in ADR 0002
- would require redesigning the current bound-body capture approach

## Decision

NScript will treat the Roslyn fork as exposing a small supported integration contract, and direct use of Roslyn internals will remain isolated to `NScript.Csc.Lib`.

The accepted Roslyn integration contract for NScript is:

- `CommonCompiler.OnBeforeCompilation(Compilation compilation)` as the compiler injection point before emit
- `CSharpCompilation.OnBoundExpressionGenerated` as the callback that exposes bound method bodies and initializers
- `InternalsVisibleTo` access for `NScript.csc.lib`
- internal symbol bridging needed to map Roslyn internal symbols back to public symbol identities, including the current `ISymbolInternal` usage
- Roslyn bound-tree and symbol APIs required to translate bound method bodies into NScript's serialized AST representation inside `NScript.Csc.Lib`

The architectural boundary is also explicit:

- `NScript.Csc.Lib` is the only project that should directly depend on Roslyn internals and fork-specific callbacks
- downstream stages such as `NScript.CLR`, `NScript.Converter`, and `NScript.Lib` should consume serialized AST resources and Mono.Cecil metadata, not Roslyn internals
- if a new feature requires Roslyn internal types or hooks outside this boundary, that change requires a new ADR

This means the supported contract is not "all Roslyn internals that happen to work." It is only the small set required to capture bound bodies and serialize them into NScript-owned data structures.

## Consequences

Positive:

- the Roslyn integration remains concentrated in one boundary project
- downstream pipeline stages stay decoupled from Roslyn internals
- future contract expansions become visible architectural decisions instead of ad hoc code changes

Negative:

- some feature work may require extra boundary design inside `NScript.Csc.Lib`
- Roslyn upgrades still need to validate the exact internal contract recorded here

## References

- [docs/adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md](0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md)
- [docs/adr/0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md](0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md)
- [docs/adr/0004-define-when-and-how-to-refresh-the-checked-in-roslyn-drop.md](0004-define-when-and-how-to-refresh-the-checked-in-roslyn-drop.md)
- [Sources/Compiler/NScript.Csc.Lib/Csc.cs](../../Sources/Compiler/NScript.Csc.Lib/Csc.cs)
- [Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs](../../Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs)
- [Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs](../../Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs)
- [Sources/Compiler/NScript.Csc.Lib/SymbolSerializer.cs](../../Sources/Compiler/NScript.Csc.Lib/SymbolSerializer.cs)
