# ADR 0002: Maintain a Minimal Roslyn Fork for NScript Hooks

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Compiler architecture and external dependencies

## Context

NScript's compiler integration needs access to Roslyn compilation stages that are not available through the standard public APIs alone. In particular, NScript needs to observe Roslyn's bound method bodies during compilation and run custom logic before emit.

The repository currently depends on Roslyn binaries through checked-in references under `Dependencies/Roslyn`, while also carrying a `roslyn` git submodule that points to a custom fork. The maintenance note in [roslyn/CLAUDE.md](../../roslyn/CLAUDE.md) documents that this fork exists specifically to add a narrow set of hooks for NScript, not to create a broad long-lived compiler divergence.

The intended fork branch for this integration is `features/physhi-updated`, which is the focused Roslyn branch described in the submodule maintenance notes.

The current forked Roslyn branch adds:

- `CSharpCompilation.OnBoundExpressionGenerated` to expose bound method bodies after binding
- `CommonCompiler.OnBeforeCompilation(Compilation compilation)` as a pre-compilation extension point
- `InternalsVisibleTo` for `NScript.csc.lib`
- private-build adjustments so the custom Roslyn binaries can be built without the standard signing pipeline

These changes are architectural because they define how NScript integrates with the C# front end, how future Roslyn upgrades will be performed, and how much divergence from upstream Roslyn is acceptable.

## Decision Drivers

- NScript must observe Roslyn bound bodies before later compilation stages continue
- NScript must keep Roslyn divergence as small as possible
- upgrade and maintenance cost must remain manageable
- custom compiler behavior must remain explicit and reproducible in source control

## Options Considered

### Option 1: Maintain a minimal Roslyn fork with surgical extension points

Keep a dedicated Roslyn fork for NScript and limit changes to narrowly scoped hooks and build adjustments required for integration.

Pros:

- preserves NScript's required access to bound-tree and pre-emit stages
- keeps custom behavior explicit and auditable
- minimizes architectural churn in NScript itself
- keeps the integration model stable for current compiler code

Cons:

- requires maintaining a fork and periodically reconciling with upstream Roslyn
- private binaries and internal access increase upgrade coordination effort

### Option 2: Depend only on stock upstream Roslyn public APIs

Remove fork-specific hooks and redesign NScript to operate only through public Roslyn surfaces.

Pros:

- simplest external dependency story
- easier to consume standard Roslyn updates
- avoids maintaining a custom compiler fork

Cons:

- may not expose the compilation stages NScript depends on today
- would likely require a significant redesign of the current compiler integration
- risks losing bound-tree visibility that motivated the Roslyn integration work

### Option 3: Carry a broader custom Roslyn fork with NScript-specific behavior embedded directly

Move more NScript behavior into Roslyn itself and deepen the fork.

Pros:

- maximum control over the compilation pipeline
- fewer external coordination points between NScript and Roslyn internals

Cons:

- highest long-term maintenance burden
- makes upstream upgrades substantially harder
- increases the chance that NScript becomes tied to a permanently divergent compiler implementation

## Decision

NScript will maintain a minimal Roslyn fork whose purpose is only to expose the narrow extension points needed by `NScript.csc.lib`.

The canonical branch for these NScript-specific Roslyn changes is `features/physhi-updated` unless a future ADR records a replacement branch or upstreaming strategy.

The fork is an integration dependency, not a separate product direction. Future Roslyn modifications should follow these rules:

- prefer adding small extension points over rewriting Roslyn behavior
- keep NScript-specific logic out of Roslyn whenever it can live in `NScript.csc.lib`
- limit customizations to callback exposure, controlled internal access, and necessary private-build adjustments
- treat upstream Roslyn as the architectural baseline and keep divergence reviewable against it

The accepted hooks described in [roslyn/CLAUDE.md](../../roslyn/CLAUDE.md) are part of this dependency strategy:

- bound-body callback through `CSharpCompilation.OnBoundExpressionGenerated`
- pre-compilation hook through `CommonCompiler.OnBeforeCompilation`
- `InternalsVisibleTo` for `NScript.csc.lib`

Any future Roslyn fork changes beyond this narrow scope should trigger a new ADR.

## Consequences

Positive:

- NScript retains access to the compilation phases it needs
- the fork remains intentionally small and easier to maintain
- future contributors have a clear rule for what is and is not acceptable inside the Roslyn fork

Negative:

- Roslyn upgrades require verifying that the custom hooks still apply cleanly
- NScript remains dependent on non-public Roslyn integration points
- build and dependency documentation must stay aligned with the actual custom binaries in use

## References

- [docs/adr/0001-adopt-architecture-decision-records.md](0001-adopt-architecture-decision-records.md)
- [roslyn/CLAUDE.md](../../roslyn/CLAUDE.md)
- [roslyn/PHYSHI-UPDATED.md](../../roslyn/PHYSHI-UPDATED.md)
- [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj)