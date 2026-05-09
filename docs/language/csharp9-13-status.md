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
~~Phase D (records / `with` / `init`)~~ ✅ landed, Phase E (collection
expressions) — partial; spread-into-array landed under Phase F1,
~~Phase F3 (required members metadata)~~ ✅ landed,
~~Phase F2 (primary constructors on classes)~~ ✅ landed,
Phase F4 (interface / `List<T>` collection-expression targets),
Phase G (docs sweep) — see issue #47 for the full plan.

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
   and `SwitchExpressionConverter.cs` both delegate pattern lowering to the
   shared `PatternMatcher.LowerToCondition`. Pattern shapes that are not yet
   supported surface a clear `NotImplementedException` pointing back at this
   document.

If a feature lowers entirely into nodes already handled by all three seams it
is *transparent*; otherwise it is a planned phase.

## C# 9 (2020)

| Feature | Status | Notes |
|---|---|---|
| Records (class) | ✅ Supported (compile-time); ⚠️ value-equality runtime caveat | `IsRecord` flows through `TypeSpecSer` (and `GenericInstanceTypeSer`). Synthesised `Equals`, `GetHashCode`, `<Clone>$`, `Deconstruct`, and the protected copy-ctor ride existing bound-tree paths. Validated in `Lang9RecordTests.cs::ConstructPositionalRecord` / `DeconstructPositional`. **Caveat:** Roslyn lowers the synthesised `Equals` against `EqualityComparer<T>.Default`, but NScript's `mscorlib` facade currently returns `null` from `EqualityComparer<T>.Default` (see `Sources/Framework/mscorlib/Collections/Generic/EqualityComparer.cs`). Until that gap is closed, `record.Equals(other)` and `==` between records will surface a runtime NRE on the first member compare; reference-equality and `Deconstruct` are unaffected. Tracked as a follow-up to issue #47. |
| `init` accessors | ✅ Supported (metadata) | `PropertySpecSer.IsInitOnly` (and `IsRequired`) is persisted from the Roslyn property symbol. JS codegen treats init-only as an ordinary setter — sufficient for object-initializer and `with` flows. Validated in `Lang9RecordTests.cs::InitOnlySetters`. Runtime enforcement of `init`/`required` is deferred to a follow-up. |
| `with` expressions | ✅ Supported | New `WithExpressionSer` (ProtoBuf tag 226) carries receiver + clone-method + initializers; Stage 1 `VisitWithExpression` and Stage 2 `ParseWithExpression` lower it onto the existing `InlinePropertyInitilizationExpression` shape. Validated in `Lang9RecordTests.cs::WithSingleMutation` / `WithMultiMutation` / `WithOnDerivedRecord`. |
| Top-level statements | ❌ Out of scope | Conflicts with the `[EntryPoint]` model; explicit non-goal. |
| Target-typed `new()` | ❌ Needs implementation | Roslyn surfaces `ConversionKind.ObjectCreation` to `VisitConversion`, which throws on the `default` arm. Empirically reproduced on `BaseClass b = new();`. **Phase B (small) or Phase C.** |
| Pattern matching — relational `<`, `<=` | ✅ Supported | Visited by `VisitRelationalPattern`; serialised under ProtoBuf tag 223. Lowered to a JS binary expression via `PatternMatcher.LowerToCondition`. Works in both `is` patterns and `switch` arm dispatch (matched before `BoundConstantPattern` because it inherits from it). |
| Pattern matching — logical `and`/`or`/`not` | ✅ Supported | `BoundBinaryPattern` (tag 224) lowers to `&&` / `\|\|`; `BoundNegatedPattern` (tag 225) lowers to `!`. Implemented recursively over `PatternMatcher`. `when` clauses on these new shapes raise a clear `NotImplementedException` rather than silently dropping the guard. |
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
| `record struct` | ⚠️ Partial | The `IsRecord` metadata flag flows through the same path as record classes. Construction, member access, and `Deconstruct` are transparent. **Caveat:** Roslyn does not synthesise a `<Clone>$` method for record structs (the struct copy happens at IL level), so a `with` expression on a record struct currently raises a clear `NotImplementedException` from `BondToAst.ParseWithExpression` rather than emitting nonsense JS. Closing this gap requires routing struct `with` through NScript's struct codegen (which today does not preserve value-copy semantics on assignment). Tracked as a follow-up to issue #47. The compile-only fixture `Lang9RecordTests.cs::RecordStructWith` exists to flag any future regression in the diagnostic. |
| Mixed deconstruction declaration & assignment | ✅ Supported | Validated in `Lang10Features.cs::MixedDeconstructionAssignment`. |
| Constant interpolated strings | ❌ Pre-existing gap | `VisitInterpolatedString` throws — affects *all* `$"…"` strings, not specific to C# 10. (`limitations.md` claims interpolation is supported; that line is **stale** and will be corrected in Phase G.) |
| Extended property patterns (`{ A.B: ... }`) | ❌ Needs implementation | Lowers to recursive pattern. **Phase C.** |
| Lambda natural type (via `var`) | ✅ Supported | Validated in `Lang10Features.cs::NaturalLambdaType`. Roslyn synthesises the delegate type at bind time. |
| Lambda explicit return type | ✅ Supported | Validated in `Lang10Features.cs::LambdaExplicitReturnType` via `int (int a) => a + 1;`. Roslyn carries the explicit return-type symbol on the anonymous function shape. |
| Lambda attributes | ⚠️ Untested | Likely transparent (attributes flow into method symbol metadata) but not exercised. |
| Caller argument expression | ⚠️ Untested | Not validated by a fixture: the `[CallerArgumentExpression]` attribute is not declared in NScript's `mscorlib`, so a fixture would fail at bind time independently of the bound-tree shape. Wire-through is expected to be transparent (the default value is folded to a literal at the call site), but cannot be claimed until the BCL gap is closed. **Phase G.** |
| Parameterless struct constructors | ⚠️ Untested | No fixture; expected to work. The Phase D `record struct` work exercised the surrounding struct codegen path without surfacing issues, but a dedicated fixture for explicit parameterless ctors on plain structs is still pending. |
| `ParenthesizedPattern` | ✅ Supported | Validated in `Lang10Features.cs::ParenthesizedConstantPattern`. Lowers to the inner pattern. |

## C# 11 (2022)

| Feature | Status | Notes |
|---|---|---|
| Raw string literals (`"""…"""`) | ✅ Supported | Validated in `Lang11Features.cs::RawStrings`. Roslyn folds to ordinary `string` constants at parse time. |
| `nameof(parameter)` in attributes referencing enclosing method parameters | ✅ Supported | Validated in `Lang11Features.cs::NameOfParameterInAttribute` via `[Obsolete("..." + nameof(value))]` on a method whose parameter is `value`. Roslyn resolves the parameter symbol at attribute-bind time and folds to a constant string. |
| File-local types (`file class`) | ✅ Supported | Validated in `Lang11Features.cs::UseFileLocalHelper` + `FileLocalHelper`. Bound tree sees an ordinary class with a mangled metadata name. |
| Required members (`required`) | ✅ Supported (metadata) | `IsRequired` is persisted on `PropertySpecSer` / `FieldSpecSer` (shipped under the records slice). The BCL attribute facades — `System.Runtime.CompilerServices.RequiredMemberAttribute`, `System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute`, `System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute` — live in NScript's `mscorlib` so Roslyn binds the C# 11 syntax. Validated in `Lang11Features.cs::Lang11RequiredTests` (init-only properties, fields, derived members, `[SetsRequiredMembers]`). NScript follows the NRT / `init` precedent here: `required` is **compile-time strict, runtime permissive** — Roslyn enforces it at every consumer call site (CS9035 if a required member is missing from an initializer), but no runtime check is emitted. |
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
| Collection expressions (`[1, 2, 3]`) | ⚠️ Partial | Phase E supports `T[]` targets with literal-only inputs (emitted as `InlineArrayInitialization`). Phase F1 added `T[]`-to-`T[]` spreads via `ArrayWithSpreadsInitialization` (`[].concat(...)` through `ArrayG<T>`) plus the discriminated `CollectionExpressionElementSer` proto hierarchy (tags 228 / 229 / 230). **Phase F4** extends coverage to `List<T>` direct targets — both literal-only and with spread sources whose static type is `List<T>` or `T[]` — by reusing the existing `NewCollectionInitializerExpression` (proto tag 212) lowering: a synthesised `new List<T>()` followed by `Add(...)`/`AddRange(src)` calls. Phase F4 also enables `List<T>` spread sources into `T[]` targets via a synthesised `List<T>.ToArray()` bridge so the F1 array-source converter handles both shapes. Validated in `Lang12CollectionExpressionTests.cs`. **Phase F5** (deferred) covers the five BCL interface targets (`IEnumerable<T>`, `IList<T>`, `IReadOnlyList<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`) which require facade work in `mscorlib` so Roslyn's collection-expression binder can satisfy a long tail of well-known-member signatures, plus `[CollectionBuilder]`-attributed user types and `IEnumerable<T>` spread sources into `T[]` (iterator-based emit). `Span<T>` / `ReadOnlySpan<T>` remain Non-Goals. |
| Spread `..` element | ✅ Supported (Phase F4) | `T[]`-to-`T[]` spreads ship in Phase F1 (`SpreadElementSer` proto tag 230, `ArrayWithSpreadsConverter` lowers to `[].concat(...)` through `ArrayG<T>`). Phase F4 extends spread coverage to `List<T>` sources (into both `T[]` and `List<T>` targets) and `T[]` sources into `List<T>` targets, lowered through the existing `NewCollectionInitializerExpression` path with `Add`/`AddRange` calls and a synthesised `ToArray()` bridge for the `List`→array shape. Source-level fixtures live in `Lang12CollectionExpressionTests.cs::ListTargetWithSpreadFromList`, `ListTargetWithSpreadFromArray`, and `ArrayTargetWithSpreadFromList`. `IEnumerable<T>` spread sources into `T[]` are deferred to **Phase F5** with the BCL-interface target work because they need an iterator-based emit path. |
| Primary constructors on classes | ✅ Supported | Validated in `Lang12Features.cs::Lang12PrimaryCtorTests` via `PrimaryCtorOnClass` (captured parameter referenced from method bodies and properties), `PrimaryCtorWithBaseCall` (`class D(int x) : B(x)` base-call argument forwarding), and `PrimaryCtorMultipleParams` (multiple captures with disjoint reference sites — field initializers, property bodies, and base-call argument lists). Roslyn synthesises private backing fields for captured parameters and lowers references at bind time, so the bound tree resolves to existing `BoundFieldAccess` / `BoundParameter` shapes already covered by Stage 1 — no `BoundPrimaryConstructorParameterAccess` visitor required. Records (`record class Foo(int X)`) continue to flow through their own `BoundFieldAccess`-against-synthesised-property path validated in `Lang9RecordTests.cs`. |
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
| `params` collections | ⚠️ Partial | `params T[]` continues to work via the existing path. **Phase F4** lights up `params List<T>` (rides the same `NewCollectionInitializerExpression` lowering as direct `List<T>` collection-expression targets); validated in `Lang13Features.cs::ParamsCollections`. `params IEnumerable<T>`/`params IList<T>`/`params ICollection<T>` share the interface-target collection-expression lowering and remain deferred to **Phase F5** with the BCL-interface facade work. `params Span<T>` / `params ReadOnlySpan<T>` are Non-Goals. |
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
