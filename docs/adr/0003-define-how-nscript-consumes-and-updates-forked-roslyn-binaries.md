# ADR 0003: Define How NScript Consumes and Updates Forked Roslyn Binaries

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Dependency management and build workflow

## Context

ADR 0002 records that NScript intentionally depends on a minimal Roslyn fork on the `features/physhi-updated` branch. The remaining architectural question is how that fork is consumed in day-to-day NScript development and how maintainers should refresh the compiler binaries safely.

The repository contains two Roslyn-related assets:

- the `roslyn` submodule, whose remote points to the custom fork
- the checked-in binaries under `Dependencies/Roslyn`, which are referenced directly by NScript compiler projects

For example, [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj) references `Microsoft.CodeAnalysis.dll` and `Microsoft.CodeAnalysis.CSharp.dll` from `Dependencies/Roslyn` via `HintPath`. The copy script at [Dependencies/Roslyn/copyRoslyn.bat](../../Dependencies/Roslyn/copyRoslyn.bat) shows that these checked-in binaries are refreshed from Roslyn build outputs under `artifacts/bin/.../netstandard2.0`.

This means NScript does not build Roslyn as part of normal solution builds. Instead, routine NScript builds consume a prebuilt Roslyn drop that is periodically refreshed from the forked submodule branch.

## Decision Drivers

- keep routine NScript builds fast and predictable
- keep the Roslyn fork source of truth explicit
- make Roslyn updates reproducible for maintainers
- ensure copied Roslyn binaries remain compatible with NScript compiler projects

## Options Considered

### Option 1: Treat the Roslyn submodule as the source of truth and check in refreshed binaries for normal NScript builds

Use the forked `roslyn` submodule as the canonical source and commit the built Roslyn binaries under `Dependencies/Roslyn` for normal NScript development.

Pros:

- keeps normal NScript builds fast because Roslyn does not need to be rebuilt every time
- preserves a clear audited source for the customized Roslyn binaries
- gives maintainers an explicit update workflow from source to checked-in artifacts

Cons:

- requires discipline to keep the checked-in binaries synchronized with the submodule branch
- binary updates must be reviewed carefully because source and artifacts live in different locations

### Option 2: Build Roslyn from the submodule as part of regular NScript builds

Make normal NScript builds depend on building the Roslyn submodule first.

Pros:

- always guarantees binaries are generated from local source
- reduces the risk of stale checked-in Roslyn artifacts

Cons:

- substantially slows the normal NScript inner loop
- complicates setup and build expectations for contributors
- increases failure surface for builds unrelated to Roslyn changes

### Option 3: Stop checking in Roslyn binaries and fetch them from an external package or artifact source

Use published artifacts rather than checked-in binaries.

Pros:

- keeps the repository smaller
- can centralize binary distribution outside the main repo

Cons:

- adds external infrastructure and availability dependencies
- makes the relationship between forked source and consumed binaries less direct inside the repository
- complicates private/custom fork distribution

## Decision

The Roslyn submodule is the source of truth for NScript's customized Roslyn dependency.

Normal NScript builds will continue to depend on checked-in Roslyn binaries under `Dependencies/Roslyn` so that routine builds remain fast and do not require rebuilding Roslyn.

When the `features/physhi-updated` branch is updated, maintainers must refresh the checked-in Roslyn binaries from the submodule's prebuilt outputs and commit the updated binary drop together with the corresponding submodule pointer change when applicable.

The supported update workflow is:

1. Update the `roslyn` submodule to the desired commit on `features/physhi-updated`.
2. Build the Roslyn fork.
3. Refresh `Dependencies/Roslyn` from the Roslyn build outputs using the process captured by [Dependencies/Roslyn/copyRoslyn.bat](../../Dependencies/Roslyn/copyRoslyn.bat).
4. Validate that NScript builds successfully and that relevant tests pass.
5. Commit the resulting source and binary updates together.

## Compatibility Constraints

The copied Roslyn binaries must remain compatible with the NScript compiler host and integration model.

In particular:

- NScript compiler projects target `$(CompilerNetFramework)`, currently `net6.0`, as defined in [Directory.Build.props](../../Directory.Build.props).
- The Roslyn binaries copied into `Dependencies/Roslyn` are currently taken from Roslyn `netstandard2.0` outputs, which keeps them consumable by the `net6.0` NScript compiler projects.
- The copied binaries must continue to expose the NScript-specific internals access expected by `NScript.csc.lib`, including the `InternalsVisibleTo` relationship recorded in ADR 0002.
- The copied binaries must remain API-compatible with the NScript compiler code that references Roslyn internals and extension points.
- If a future Roslyn update requires changing the target framework of the copied artifacts, the compiler hosting model, or the friend-assembly arrangement, that change requires a new ADR.

## Consequences

Positive:

- contributors get fast normal builds without rebuilding Roslyn
- maintainers have an explicit workflow for keeping source and binaries aligned
- the source of truth for Roslyn customization remains unambiguous

Negative:

- stale binaries are possible if maintainers update the submodule without refreshing `Dependencies/Roslyn`
- Roslyn updates require coordinated source, artifact, and validation work
- repository history will continue to include checked-in binary changes

## References

- [docs/adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md](0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md)
- [Directory.Build.props](../../Directory.Build.props)
- [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj)
- [Dependencies/Roslyn/copyRoslyn.bat](../../Dependencies/Roslyn/copyRoslyn.bat)
- [roslyn/CLAUDE.md](../../roslyn/CLAUDE.md)
- [roslyn/PHYSHI-UPDATED.md](../../roslyn/PHYSHI-UPDATED.md)
- [.gitmodules](../../.gitmodules)