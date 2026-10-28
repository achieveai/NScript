# ADR 0004: Define When and How to Refresh the Checked-in Roslyn Drop

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Dependency maintenance and release policy

## Context

ADR 0002 establishes that NScript depends on a minimal Roslyn fork on the `features/physhi-updated` branch. ADR 0003 establishes that the `roslyn` submodule is the source of truth, while normal NScript builds consume checked-in binaries from `Dependencies/Roslyn`.

That still leaves an operational policy gap: when maintainers should refresh the checked-in Roslyn binary drop, what validation is required before accepting that refresh, and whether binary refreshes must happen for every branch change or can be batched.

This policy matters because Roslyn updates affect the core compiler dependency used by [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj) and downstream compiler entry points such as [Sources/Compiler/JsCsc/JsCsc.csproj](../../Sources/Compiler/JsCsc/JsCsc.csproj) and [Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj](../../Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj). The corresponding compiler-facing test surface also depends on that integration, including [Test/Compiler/NScript.Csc.Lib.Test/NScript.Csc.Lib.Test.csproj](../../Test/Compiler/NScript.Csc.Lib.Test/NScript.Csc.Lib.Test.csproj), [Test/Compiler/NScript.CLR.Test/NScript.CLR.Test.csproj](../../Test/Compiler/NScript.CLR.Test/NScript.CLR.Test.csproj), and [Test/Compiler/NScript.Converter.Test/NScript.Converter.Test.csproj](../../Test/Compiler/NScript.Converter.Test/NScript.Converter.Test.csproj).

## Decision Drivers

- avoid unnecessary churn in checked-in binaries
- keep Roslyn refreshes reproducible and reviewable
- require enough validation to catch integration regressions early
- allow maintainers to move the fork forward deliberately instead of mechanically

## Options Considered

### Option 1: Refresh the checked-in Roslyn drop on every submodule update

Every change to the fork branch results in a binary refresh commit.

Pros:

- source and binaries stay tightly synchronized
- less chance of a stale binary drop after a fork change

Cons:

- creates unnecessary binary churn
- increases maintenance overhead for branch changes that are not yet needed by NScript
- can make review history noisy

### Option 2: Refresh the checked-in Roslyn drop only when there is a concrete NScript need, with a required validation bar

Maintainers update the binary drop deliberately when the forked source changes in a way that matters to NScript and validate the integration before committing.

Pros:

- reduces unnecessary binary updates
- keeps refreshes tied to explicit reasons and validation
- preserves a manageable maintenance workflow

Cons:

- the checked-in drop may temporarily lag behind the latest fork branch head
- requires maintainers to exercise judgment about when an update is warranted

### Option 3: Allow arbitrary binary refreshes with ad hoc validation

Let maintainers update the drop whenever convenient without a defined validation policy.

Pros:

- lowest process overhead
- maximum flexibility for local workflows

Cons:

- weakens reviewability and reproducibility
- makes it harder to know whether a given Roslyn update was sufficiently validated
- increases regression risk for compiler integration

## Decision

NScript will refresh the checked-in Roslyn binary drop deliberately, not mechanically for every fork branch change.

Maintainers should refresh `Dependencies/Roslyn` when at least one of the following is true:

- the `features/physhi-updated` branch gained or changed an NScript-required hook or internal-access behavior
- the fork includes a bug fix, compatibility fix, or security fix that affects NScript builds or compiler behavior
- NScript intentionally wants newer Roslyn behavior or compatibility support
- the currently checked-in Roslyn drop no longer builds or tests cleanly with the maintained NScript codebase

Maintainers do not need to refresh the checked-in binaries for every intermediate branch update if there is no concrete NScript reason to consume that change yet.

## Batching Policy

Roslyn refreshes may be batched.

That means maintainers may advance the submodule across multiple `features/physhi-updated` commits and publish one binary refresh for the resulting chosen commit instead of one refresh per intermediate commit.

However, each committed binary refresh must still satisfy these rules:

- the `roslyn` submodule pointer in the repo must identify the exact source commit used to produce the checked-in binaries
- the checked-in binaries must be reproducible from that exact submodule commit
- the commit message or pull request should state the reason for the refresh and the validation performed

## Validation Policy

Each Roslyn binary refresh must pass a minimum NScript validation bar before it is accepted.

The preferred validation is:

1. Build [NScript_Full.sln](../../NScript_Full.sln) in Debug.
2. Build the Roslyn-dependent compiler entry points that exercise the integration surface, at minimum [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj), [Sources/Compiler/JsCsc/JsCsc.csproj](../../Sources/Compiler/JsCsc/JsCsc.csproj), and [Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj](../../Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj).
3. Run the compiler-facing tests most directly tied to the Roslyn integration, at minimum [Test/Compiler/NScript.Csc.Lib.Test/NScript.Csc.Lib.Test.csproj](../../Test/Compiler/NScript.Csc.Lib.Test/NScript.Csc.Lib.Test.csproj), [Test/Compiler/NScript.CLR.Test/NScript.CLR.Test.csproj](../../Test/Compiler/NScript.CLR.Test/NScript.CLR.Test.csproj), and [Test/Compiler/NScript.Converter.Test/NScript.Converter.Test.csproj](../../Test/Compiler/NScript.Converter.Test/NScript.Converter.Test.csproj).

If the repository already has known unrelated failures that prevent the full preferred validation from succeeding, maintainers may use scoped validation instead, but they must:

- validate the directly affected Roslyn-dependent projects and tests
- document the unrelated pre-existing failures separately from the Roslyn refresh
- avoid treating unrelated red builds as evidence that the Roslyn refresh is acceptable

## Consequences

Positive:

- Roslyn binary refreshes stay tied to real NScript needs
- maintainers can batch updates to reduce binary churn
- validation expectations are explicit and reviewable

Negative:

- the checked-in binary drop may intentionally lag behind the latest fork commit
- maintainers must document refresh reasons and validation results consistently
- scoped validation requires judgment when the full repo is not green

## Refresh Log

### 2026-10-28: Roslyn 5.9.0 (C# 14)

- Reason: newer Roslyn fixes and the upstream compiler-output cache hooks for faster builds (#112, unblocks #113).
- Language: the C# 14 feature set is the same as in the previous fork (`b2297ec5` already had `CSharp14`). This refresh adds only newer preview-only features.
- Source: upstream commit `35d9211b841e`, the commit named in the 5.9.0 NuGet nuspec. Upstream has no tag for it.
- Process: merged (not rebased) into `features/physhi-5.9.0`, then fast-forwarded `features/physhi-updated` to the same commit `21bcede73fc`. The canonical branch from ADR 0002 is unchanged.
- Conflicts: `global.json` took upstream. `CommonCompiler.cs` kept both the NScript hooks (`OnBeforeCompilation`, `GetManifestResources`) and upstream's `CheckCache` / `OnCompilationSucceeded`.
- Build fixes, all fork-local:
  - `git config core.longpaths true` before merging on Windows.
  - `roslyn/NuGet.Config` clears `packageSourceMapping`, so a user-level mapping cannot hide the dnceng feeds.
  - The full `Restore.cmd` fails on a Razor test path over 260 characters. Build `src/Compilers/CSharp/Portable/Microsoft.CodeAnalysis.CSharp.csproj` in Release directly.
  - Run `Dependencies/Roslyn/copyRoslyn.bat` with the real `cmd.exe`. In PowerShell, `cmd` can resolve to msys and silently do nothing.
- Output: both DLLs are unsigned, version 42.42.42.42, with `InternalsVisibleTo` for `NScript.csc.lib`. A second Release build was byte-identical to the checked-in drop.
- NScript code changes: none.
- Validation:
  - `NScript_Full.sln` Debug build, including `--no-incremental`: green.
  - Compiler tests, one project at a time: CssParser 25, CLR 127, Converter 413, Csc.Lib 70 (+2 pre-existing skips), JSParser 54, Utils 163, RazorSkinParser 281, SourceMap.Server 72, SourceMap 96, SunlightTestAdapter.Integration 2, XwmlParser 44. All passed.
  - Not run: Cs2JsCTest and SourceMap.Server.E2E.Test. Neither is in the solution.
  - Browser suites: QUnit 358/358, e2e-todo 55/55, e2e-sheet 11/11, `bench-spreadsheet.mjs --check` OK (DOM write counts match; absolute-ms warnings only).
- Scope: `LangVersion` stays 13 in the Framework props. C# 14 features need converter tests before they are enabled.
  - Follow-up in the same PR: #115 and #116 fixed the two crashes, and #117 raised `LangVersion` to 14 with browser and converter tests. The generated framework and demo-app JS was byte-identical before and after.
- C# 14 probe (LangVersion 14, run in Chromium): `field` keyword, null-conditional assignment, `nameof` on unbound generics, `ref`/`out` on simple lambda parameters, and partial constructors/events work. Extension members and user-defined compound assignment operators crash `nscript.exe`. First-class span does not apply (no `Span<T>` in NScript's mscorlib).
- Next refresh: when .NET 11 ships C# 15 as GA.

## References

- [docs/adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md](0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md)
- [docs/adr/0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md](0003-define-how-nscript-consumes-and-updates-forked-roslyn-binaries.md)
- [NScript_Full.sln](../../NScript_Full.sln)
- [Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj](../../Sources/Compiler/NScript.Csc.Lib/NScript.csc.lib.csproj)
- [Sources/Compiler/JsCsc/JsCsc.csproj](../../Sources/Compiler/JsCsc/JsCsc.csproj)
- [Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj](../../Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj)
- [Test/Compiler/NScript.Csc.Lib.Test/NScript.Csc.Lib.Test.csproj](../../Test/Compiler/NScript.Csc.Lib.Test/NScript.Csc.Lib.Test.csproj)
- [Test/Compiler/NScript.CLR.Test/NScript.CLR.Test.csproj](../../Test/Compiler/NScript.CLR.Test/NScript.CLR.Test.csproj)
- [Test/Compiler/NScript.Converter.Test/NScript.Converter.Test.csproj](../../Test/Compiler/NScript.Converter.Test/NScript.Converter.Test.csproj)