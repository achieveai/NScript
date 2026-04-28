# C# 9 – 13 feature support status (NScript)

> **Audience:** App authors and contributors deciding whether a given C# 9–13
> feature compiles cleanly through the NScript toolchain.
>
> **Companion:** [`limitations.md`](./limitations.md) is the
> consumer-facing summary; this file is the validated, per-feature breakdown
> with empirical evidence and follow-up references.

## TL;DR

NScript was historically pinned at C# 8 syntax (`<LangVersion>8.0</LangVersion>`
in both `Sources/Framework/Directory.Build.props` and
`Test/Framework/Directory.build.props`). Issue #47 lifts that pin to
**`<LangVersion>13</LangVersion>`** and lands per-version fixture suites that
empirically confirm what is *transparent* (Roslyn lowers to bound nodes the
existing Stage-1 visitor already handles) versus what still needs implementation
work in the compiler pipeline.

The remaining pipeline gaps are sequenced into Phase C (pattern family),
Phase D (records / `with` / `init`), Phase E (collection expressions), Phase F
(required members, primary constructors), Phase G (docs sweep) — see issue #47
for the full plan.

## How a feature lands in the "Supported" column

A feature is moved to **Supported** only when ALL of the following hold:

1. There is a fixture in `Test/Framework/RealScript/LangNFeatures.cs` (where
   *N* matches the C# version that introduced the syntax).
2. The fixture compiles through the NScript csc *without* raising
   `NotImplementedException` from `BoundAstToAstBase` or any downstream visitor.
3. The whole `RealScript.csproj` (and `RealScript.Debug.csproj`) build cleanly
   with `<LangVersion>13</LangVersion>` so the bound tree is processed alongside
   real consumer code.

A feature is **Needs implementation** if it parses but fails any of the
above; **Out of scope** if it depends on a CLR primitive NScript explicitly
does not target (e.g. `Span<T>`, function pointers, `ref` lifetimes — see the
issue's Non-Goals list).

## Pipeline seams

Every new C# language feature must thread through three seams:

1. **Stage 1 visitor** — `Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs`.
   ~95 unimplemented bound-node overrides today. Pattern coverage stops at
   constant / declaration / discard.
2. **Serialisation contract** —
   `Sources/Compiler/JsCsc.Lib/Serialization/ProtoBufSerialization.cs`.
   Only constant / declaration / discard pattern proto types are declared.
   ProtoBuf tag numbers are append-only — a tag block (200–250) is reserved
   for the C# 9–13 work.
3. **Stage 2 converter** —
   `Sources/Compiler/NScript.Converter/ExpressionsConverter/IsPatternConverter.cs`
   (and friends). Throws on any pattern shape it does not recognise.

If a feature lowers entirely into nodes already handled by all three seams it
is *transparent*; otherwise it is a planned phase.

## C# 9 (2020)

| Feature | Status | Notes |
|---|---|---|
| Records (class) | ❌ Needs implementation | `BoundWithExpression` not visited; `IsRecord` not on serialised symbol envelope. **Phase D.** |
| `init` accessors | ❌ Needs implementation | `IsInitOnly` not serialised; treat as ordinary set at runtime — metadata-only support planned. **Phase D.** |
| `with` expressions | ❌ Needs implementation | `BoundWithExpression` is unvisited. **Phase D.** |
| Top-level statements | ❌ Out of scope | Conflicts with the `[EntryPoint]` model; explicit non-goal. |
| Target-typed `new()` | ❌ Needs implementation | Roslyn surfaces `ConversionKind.ObjectCreation` to `VisitConversion`, which throws on the `default` arm. Empirically reproduced on `BaseClass b = new();`. **Phase B (small) or Phase C.** |
| Pattern matching — relational `<`, `<=` | ❌ Needs implementation | `BoundRelationalPattern` unvisited. **Phase C.** |
| Pattern matching — logical `and`/`or`/`not` | ❌ Needs implementation | `BoundBinaryPattern`, `BoundNegatedPattern` unvisited. **Phase C.** |
| Pattern matching — type pattern (without var) | ⚠️ Partial | Declaration patterns work today; bare type patterns route through the same path. Re-validate during Phase C. |
| Discard parameters in lambdas | ✅ Supported | Validated in `Lang9Features.cs::LambdaDiscardParameters`. Roslyn binds `(_, _)` as ordinary lambda parameters. |
| Static anonymous functions / lambdas | ✅ Supported | Validated in `Lang9Features.cs::StaticLambdas`. The `static` keyword forbids captures but emits an ordinary lambda bound node. |
| Function pointers (`delegate*`) | ❌ Out of scope | Issue #47 Non-Goals — depends on calling-convention metadata absent from JS. |
| `nint` / `nuint` | ❌ Out of scope | Issue #47 Non-Goals — no native pointer width on JS. |
| Covariant return types | ❌ Out of scope | Empirically reproduced: Roslyn rejects with `CS8830` because the `netstandard2.1` target lacks `RuntimeFeature.CovariantReturnsOfClasses`. |
| Source generators | ❌ Out of scope | NScript drives compilation via `NScript.Csc.Lib` directly; generator hosting is not wired in. |
| Module initializers | ❌ Needs implementation | Not yet exercised; treated as out-of-scope unless user demand surfaces. |
| Extended `partial` methods | ✅ Supported | Already worked under C# 8; no new bound-tree shape. |
| `static` local function modifier | ✅ Supported | Validated in `Lang9Features.cs::LocalFunctionWithStaticModifier`. Pre-existing C# 8 path. |

## C# 10 (2021)

| Feature | Status | Notes |
|---|---|---|
| File-scoped namespaces | ✅ Supported | Validated by `Lang10Features.cs`, `Lang11Features.cs`, `Lang12Features.cs`, `Lang13Features.cs` all using the syntax. Lowers to the same bound tree as a braced namespace. |
| Global usings | ⚠️ Untested | No fixture in this PR (would require a project-level edit). Should be transparent — purely affects symbol resolution. Re-validate during Phase G. |
| `record struct` | ❌ Needs implementation | Same family as C# 9 records. **Phase D.** |
| Mixed deconstruction declaration & assignment | ✅ Supported | Validated in `Lang10Features.cs::MixedDeconstructionAssignment`. |
| Constant interpolated strings | ❌ Pre-existing gap | `VisitInterpolatedString` throws — affects *all* `$"…"` strings, not specific to C# 10. (`limitations.md` claims interpolation is supported; that line is **stale** and will be corrected in Phase G.) |
| Extended property patterns (`{ A.B: ... }`) | ❌ Needs implementation | Lowers to recursive pattern. **Phase C.** |
| Lambda natural type (via `var`) | ✅ Supported | Validated in `Lang10Features.cs::NaturalLambdaType`. Roslyn synthesises the delegate type at bind time. |
| Lambda explicit return type | ✅ Supported | Validated in `Lang10Features.cs::LambdaExplicitReturnType` via `int (int a) => a + 1;`. Roslyn carries the explicit return-type symbol on the anonymous function shape. |
| Lambda attributes | ⚠️ Untested | Likely transparent (attributes flow into method symbol metadata) but not exercised. |
| Caller argument expression | ⚠️ Untested | Not validated by a fixture: the `[CallerArgumentExpression]` attribute is not declared in NScript's `mscorlib`, so a fixture would fail at bind time independently of the bound-tree shape. Wire-through is expected to be transparent (the default value is folded to a literal at the call site), but cannot be claimed until the BCL gap is closed. **Phase G.** |
| Parameterless struct constructors | ⚠️ Untested | No fixture; expected to work but should be re-validated under Phase D (struct codegen sweep). |
| `ParenthesizedPattern` | ✅ Supported | Validated in `Lang10Features.cs::ParenthesizedConstantPattern`. Lowers to the inner pattern. |

## C# 11 (2022)

| Feature | Status | Notes |
|---|---|---|
| Raw string literals (`"""…"""`) | ✅ Supported | Validated in `Lang11Features.cs::RawStrings`. Roslyn folds to ordinary `string` constants at parse time. |
| `nameof(parameter)` in attributes referencing enclosing method parameters | ✅ Supported | Validated in `Lang11Features.cs::NameOfParameterInAttribute` via `[Obsolete("..." + nameof(value))]` on a method whose parameter is `value`. Roslyn resolves the parameter symbol at attribute-bind time and folds to a constant string. |
| File-local types (`file class`) | ✅ Supported | Validated in `Lang11Features.cs::UseFileLocalHelper` + `FileLocalHelper`. Bound tree sees an ordinary class with a mangled metadata name. |
| Required members (`required`) | ❌ Needs implementation | `IsRequired` not serialised. NScript will follow the NRT precedent — metadata only, no runtime enforcement. **Phase F.** |
| Generic attributes | ⚠️ Untested | Symbol-side metadata path not audited; likely needs `SymbolSerializer` work. **Phase F.** |
| Auto-default of struct fields | ✅ Supported | Validated in `Lang11Features.cs::StructAutoDefault`. The auto-default prologue is part of Roslyn's lowering — bound tree shape unchanged. |
| List patterns (`[1, 2, ..]`) | ❌ Needs implementation | `BoundListPattern`, `BoundSlicePattern` unvisited. **Phase C.** |
| Newlines in interpolation holes | ❌ Pre-existing gap | Subsumed by the broader interpolated-string gap. |
| UTF-8 string literals (`"…"u8`) | ❌ Out of scope | Issue #47 Non-Goals — depends on `ReadOnlySpan<byte>`. |
| Extended `nameof` for type parameters | ✅ Supported | Validated in `Lang11Features.cs::NameOfTypeParameter<T>` via `nameof(T)`. Roslyn folds the type-parameter name to a constant string at bind time. |
| `ref` fields, `ref struct` improvements | ❌ Out of scope | Issue #47 Non-Goals. |
| Static abstract interface members | ❌ Out of scope | Issue #47 Non-Goals. |

## C# 12 (2023)

| Feature | Status | Notes |
|---|---|---|
| Collection expressions (`[1, 2, 3]`) | ❌ Needs implementation | `BoundCollectionExpression` unvisited. **Phase E.** |
| Spread `..` element | ❌ Needs implementation | Subsumed by collection expressions. **Phase E.** |
| Primary constructors on classes | ❌ Needs implementation | Parameter-capture surfaces `BoundPrimaryConstructorParameterAccess` for unused-but-referenced parameters. **Phase F.** |
| `using` alias for any type | ✅ Supported | Validated in `Lang12Features.cs::AliasAnyType` using **tuple-syntax** (`using Pair = (int X, int Y);`) and **array-syntax** (`using Numbers = int[];`) aliases — the C# 12 grammar additions. Aliases are resolved at symbol resolution; bound tree sees the underlying type. Closed generic aliases (legal since C# 1.0) are not exercised here. |
| Default lambda parameter values | ⚠️ Untested | May surface a new lambda metadata shape. Conservative — re-validate before claiming support. |
| Inline arrays | ❌ Out of scope | Issue #47 Non-Goals. |
| `ref readonly` parameters | ❌ Out of scope | Issue #47 Non-Goals. |
| `Experimental` attribute | ✅ Supported | Pure attribute metadata; transparent. |
| Interceptors | ❌ Out of scope | Source-generator-based; depends on generator hosting. |

## C# 13 (2024)

| Feature | Status | Notes |
|---|---|---|
| Escape sequence `\e` | ✅ Supported | Validated in `Lang13Features.cs::EscapeSequenceEsc`. Lex-time fold to `0x1B`. |
| Method group natural type | ✅ Supported | C# 10's single-overload natural type validated in `Lang10Features.cs::MethodGroupNaturalType`. C# 13's overload-pruning extension (constraint-failing generic candidates pruned during natural-type inference) validated in `Lang13Features.cs::MethodGroupOverloadPruning`. Both compile transparently — Roslyn finalises the delegate symbol at bind time. |
| `params` collections | ❌ Needs implementation | When the parameter type is `IEnumerable<T>` / `Span<T>`, Roslyn lowers via the collection-expression pipeline. **Phase E** for `T[]`-equivalent shapes; `Span<T>` shapes are out of scope. |
| `System.Threading.Lock` | ❌ Out of scope | Issue #47 Non-Goals — depends on the new BCL primitive that does not exist in NScript's `mscorlib`. |
| `field` keyword in property accessors (preview) | ❌ Out of scope | Reserved by C# 14 to mean "synthesised backing field"; pinning `<LangVersion>13</LangVersion>` deliberately avoids the rebinding. |
| Implicit index access on initializers | ⚠️ Untested | Indices/ranges are listed as a partial item in `csharp8-todos.md`; revisit during Phase G. |
| `partial` properties | ⚠️ Untested | New bound-tree shape; revisit if user demand surfaces. |

## Empirical evidence — failure modes encountered while writing fixtures

These were observed while iterating on Phase A; they are recorded here so future
contributors do not re-discover them and so the matrix above is grounded.

| Fixture under test | Failure | Root cause |
|---|---|---|
| `BaseClass b = new();` | `NotImplementedException: ObjectCreation not supported.` | `VisitConversion` falls into `default` arm for `ConversionKind.ObjectCreation` (line 643). |
| `private const string Salutation = $"{Greeting}, world";` | `NotImplementedException` from `VisitInterpolatedString` (line 937). | Pre-existing C# 6 gap — interpolated strings have never been visited; Roslyn does not lower them eagerly when the operand is a const. |
| `public override DerivedClass Make() => …` | `CS8830`: target runtime doesn't support covariant return types. | `netstandard2.1` doesn't surface `RuntimeFeature.CovariantReturnsOfClasses`. Out of scope. |

## Cross-links

- Issue [#47](https://github.com/achieveai/NScript/issues/47) — umbrella issue and phase plan.
- [`limitations.md`](./limitations.md) — consumer-facing summary (will be refreshed in Phase G).
- [`csharp8-todos.md`](../../csharp8-todos.md) — open C# 8 items; a few rows here reference its open work.
- [ADR 0006](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md) — the two-stage pipeline this matrix lives on top of.
- [ADR 0002](../adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md) — the Roslyn fork that drives bound-tree shapes.
