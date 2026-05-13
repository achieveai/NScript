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

Phase status (all under issue #47):

- ~~Phase B (lift the LangVersion pin)~~ ✅ landed.
- ~~Phase C (relational / logical / negated pattern family)~~ ✅ landed (PR #56). Extended-property patterns (`{ A.B: ... }`) remain deferred — see C# 10 table below.
- ~~Phase D (records / `with` / `init`)~~ ✅ landed.
- ~~Phase E (collection expressions — `T[]` literal-only)~~ ✅ landed.
- ~~Phase F1 (collection-expression spread sources for `T[]`)~~ ✅ merged (PR #59).
- ~~Phase F2 (primary constructors on plain classes)~~ ✅ merged (PR #62).
- ~~Phase F3 (`required` members metadata)~~ ✅ merged (PR #60).
- ~~Phase F4 (`List<T>` targets + List/array spread bridges)~~ ✅ merged (PR #64).
- ~~Phase F5 (BCL interface targets + `IEnumerable<T>` spread sources)~~ ✅ merged (PR #65).
- ~~Phase F6 (C# 8 indices/ranges back-fill + sub-spread closure; `[CollectionBuilder]` reclassified Non-Goal)~~ ✅ merged (PR #66).
- ~~Phase G (docs sweep + cheap ⚠️ Untested promotions)~~ ✅ merged (PR #67).
- **WI-63 (V8 runtime/execution coverage for the unblocked Phase C/F2/F-runtime waves) — this PR.**

Parallel non-gating work outside #47 itself:

- Sub-issue [#63](https://github.com/achieveai/NScript/issues/63) — V8 / runtime execution coverage for the compile-only fixtures landed under Phases D/E/F.
- Follow-up issues for residual gaps: `BoundAstToAstBase` partial-class extraction; `EqualityComparer<T>.Default` runtime fix (record value-equality); `[CallerArgumentExpression]` BCL facade; generic-attribute `SymbolSerializer` audit. Filed against #47 as needed when demand surfaces.

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
| Pattern matching — relational `<`, `<=` | ✅ Supported | Visited by `VisitRelationalPattern`; serialised under ProtoBuf tag 223. Lowered to a JS binary expression via `PatternMatcher.LowerToCondition`. Works in both `is` patterns and `switch` arm dispatch (matched before `BoundConstantPattern` because it inherits from it). Runtime-gated by `Lang9PatternExecutionTests` (V8 execution snapshot — covers `is` and switch-arm dispatch). |
| Pattern matching — logical `and`/`or`/`not` | ✅ Supported | `BoundBinaryPattern` (tag 224) lowers to `&&` / `\|\|`; `BoundNegatedPattern` (tag 225) lowers to `!`. Implemented recursively over `PatternMatcher`. `when` clauses on these new shapes raise a clear `NotImplementedException` rather than silently dropping the guard. Runtime-gated by `Lang9PatternExecutionTests` (V8 execution snapshot). |
| Pattern matching — type pattern (without var) | ⚠️ Partial | Declaration patterns in `is` form work today (`d is Derived`); bare type patterns route through the same path. Re-validate during Phase C. Runtime-gated by `Lang9PatternExecutionTests` (V8 execution snapshot — `is`-form against a base/derived hierarchy). **Caveat:** declaration patterns inside `switch` arms (`o switch { string s => ..., int i => ..., }`) trigger `InvalidOperationException` from `BoundAstToAstBase.GetLocalVariable` because the switch-arm scope is not pushed onto `scopeBlockStack` before the arm's bound local is visited. Deferred to a follow-up under #47 — closing it is a Stage-1 visitor change, not a fixture concern. |
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
| Global usings | ✅ Supported | Validated by `Lang10GlobalUsingTests.cs::UsesGloballyImportedText` together with the project-level `GlobalUsings.cs` directive (`global using System.Text;`). The bound tree sees the fully-qualified `System.Text.StringBuilder` reference after symbol resolution; `global using` is purely a symbol-resolution concern, no new Stage 1 shape reaches `BoundAstToAstBase`. |
| `record struct` | ⚠️ Partial | The `IsRecord` metadata flag flows through the same path as record classes. Construction, member access, and `Deconstruct` are transparent. **Caveat:** Roslyn does not synthesise a `<Clone>$` method for record structs (the struct copy happens at IL level), so a `with` expression on a record struct currently raises a clear `NotImplementedException` from `BondToAst.ParseWithExpression` rather than emitting nonsense JS. Closing this gap requires routing struct `with` through NScript's struct codegen (which today does not preserve value-copy semantics on assignment). Tracked as a follow-up to issue #47. The compile-only fixture `Lang9RecordTests.cs::RecordStructWith` exists to flag any future regression in the diagnostic. |
| Mixed deconstruction declaration & assignment | ✅ Supported | Validated in `Lang10Features.cs::MixedDeconstructionAssignment`. |
| Constant interpolated strings | ❌ Pre-existing gap | `VisitInterpolatedString` throws — affects only the `const` context (`const string s = $"...";`); ordinary `$"..."` is lowered to `string.Concat` / `string.Format` by Roslyn before reaching Stage 1. Surfaced as two rows in `limitations.md` (non-constant ✅ Supported / constant ⚠️ Bug). |
| Extended property patterns (`{ A.B: ... }`) | ❌ Needs implementation | Lowers to recursive pattern. **Phase C.** |
| Lambda natural type (via `var`) | ✅ Supported | Validated in `Lang10Features.cs::NaturalLambdaType`. Roslyn synthesises the delegate type at bind time. |
| Lambda explicit return type | ✅ Supported | Validated in `Lang10Features.cs::LambdaExplicitReturnType` via `int (int a) => a + 1;`. Roslyn carries the explicit return-type symbol on the anonymous function shape. |
| Lambda attributes | ✅ Supported | Validated in `Lang10Features.cs::LambdaWithAttribute`. Exercises both attribute-on-lambda (`[LambdaMarker("inc")] (int x) => x + 1`) and attribute-on-lambda-parameter (`([LambdaMarker("p")] int x) => x * 2`) shapes. The attribute lives on the synthesised lambda method symbol's metadata; the bound tree shape is the same `BoundLambda` as an unattributed lambda. |
| Caller argument expression | ⚠️ BCL-blocked | `[CallerArgumentExpression]` is not declared in NScript's `mscorlib`, so any fixture would fail at bind time independently of the bound-tree shape. Wire-through is expected to be transparent (the default value is folded to a literal at the call site) but cannot be claimed until the BCL gap is closed. Deferred to a follow-up under #47 if demand surfaces. |
| Parameterless struct constructors | ✅ Supported | Validated in `Lang10Features.cs::ParameterlessStructConstructor` via `public struct S { public S() { X = 7; } }`. Roslyn lowers an explicit parameterless struct ctor to an ordinary `BoundConstructor` shape; the F2 record-struct work already exercised the surrounding struct codegen path. |
| `ParenthesizedPattern` | ⚠️ Compile-only (runtime gap on boxed scrutinee) | Validated in `Lang10Features.cs::ParenthesizedConstantPattern`. Lowers to the inner pattern at bind time. **Runtime gap surfaced by WI-63:** `object o = 5; o is (5)` evaluates to `false` through V8 — the boxed scrutinee comparison against an `int` constant fails. The compile-only fixture continues to gate the bind-time shape; no V8 snapshot is emitted from `Lang9PatternExecutionTests` for the parenthesized form until the lowering is corrected. Deferred to a follow-up under #47. |

## C# 11 (2022)

| Feature | Status | Notes |
|---|---|---|
| Raw string literals (`"""…"""`) | ✅ Supported | Validated in `Lang11Features.cs::RawStrings`. Roslyn folds to ordinary `string` constants at parse time. |
| `nameof(parameter)` in attributes referencing enclosing method parameters | ✅ Supported | Validated in `Lang11Features.cs::NameOfParameterInAttribute` via `[Obsolete("..." + nameof(value))]` on a method whose parameter is `value`. Roslyn resolves the parameter symbol at attribute-bind time and folds to a constant string. |
| File-local types (`file class`) | ✅ Supported | Validated in `Lang11Features.cs::UseFileLocalHelper` + `FileLocalHelper`. Bound tree sees an ordinary class with a mangled metadata name. |
| Required members (`required`) | ✅ Supported (metadata) | `IsRequired` is persisted on `PropertySpecSer` / `FieldSpecSer` (shipped under the records slice). The BCL attribute facades — `System.Runtime.CompilerServices.RequiredMemberAttribute`, `System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute`, `System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute` — live in NScript's `mscorlib` so Roslyn binds the C# 11 syntax. Validated in `Lang11Features.cs::Lang11RequiredTests` (init-only properties, fields, derived members, `[SetsRequiredMembers]`). NScript follows the NRT / `init` precedent here: `required` is **compile-time strict, runtime permissive** — Roslyn enforces it at every consumer call site (CS9035 if a required member is missing from an initializer), but no runtime check is emitted. |
| Generic attributes | ⚠️ BCL-blocked | A generic attribute (`class FooAttribute<T> : Attribute`) introduces a generic type symbol in attribute position; the symbol-side metadata path through `SymbolSerializer` has not been audited and may need bespoke work to round-trip the type-argument. Deferred to a follow-up under #47 if demand surfaces. |
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
| Collection expressions (`[1, 2, 3]`) | ✅ Supported (Phase F6) | Phase E supports `T[]` targets with literal-only inputs (emitted as `InlineArrayInitialization`). Phase F1 added `T[]`-to-`T[]` spreads via `ArrayWithSpreadsInitialization` (`[].concat(...)` through `ArrayG<T>`) plus the discriminated `CollectionExpressionElementSer` proto hierarchy (tags 228 / 229 / 230). **Phase F4** extends coverage to `List<T>` direct targets — both literal-only and with spread sources whose static type is `List<T>` or `T[]` — by reusing the existing `NewCollectionInitializerExpression` (proto tag 212) lowering: a synthesised `new List<T>()` followed by `Add(...)`/`AddRange(src)` calls. Phase F4 also enables `List<T>` spread sources into `T[]` targets via a synthesised `List<T>.ToArray()` bridge so the F1 array-source converter handles both shapes. **Phase F5** covers the five list-shaped BCL interface targets (`IEnumerable<T>`, `IList<T>`, `IReadOnlyList<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`) — all collapse to the same Phase F4 `List<T>` lowering. Phase F5 also adds `IEnumerable<T>` spread sources for both `T[]` and `List<T>` targets via the existing `AddRange(IEnumerable<T>)` overload (List target) or a synthesised `new List<T>(); AddRange(src); ToArray()` bridge (array target). **Phase F6** completes the F-track: indices/ranges back-fill (see C# 8 row in `limitations.md`) lights up element-position sub-spreads like `[..src[1..3]]` transparently because `arr[1..3]` lowers to a `RuntimeHelpers.GetSubArray<T>(...)` call which then flows through F1's array-source converter. Validated in `Lang12CollectionExpressionTests.cs`. **Non-Goals** (will not be supported): `[CollectionBuilder]`-attributed user types depend on `Span<T>`/`ReadOnlySpan<T>` (the builder method's `static T CreateCollection(ReadOnlySpan<T> values)` signature uses a ref struct that has no JS runtime semantics); `Span<T>` / `ReadOnlySpan<T>` targets and `params Span<T>`. See `docs/language/limitations.md`. |
| Spread `..` element | ✅ Supported (Phase F6) | `T[]`-to-`T[]` spreads ship in Phase F1 (`SpreadElementSer` proto tag 230, `ArrayWithSpreadsConverter` lowers to `[].concat(...)` through `ArrayG<T>`). Phase F4 extends spread coverage to `List<T>` sources (into both `T[]` and `List<T>` targets) and `T[]` sources into `List<T>` targets, lowered through the existing `NewCollectionInitializerExpression` path with `Add`/`AddRange` calls and a synthesised `ToArray()` bridge for the `List`→array shape. **Phase F5** adds `IEnumerable<T>` spread sources into both `T[]` and `List<T>` targets via the existing `AddRange(IEnumerable<T>)` overload (List target) or a synthesised `new List<T>(); AddRange(src); ToArray()` bridge (array target). **Phase F6** unblocks index/range sub-spreads (`[..src[1..3]]`) transparently: the C# 8 `arr[1..3]` lowers to a `RuntimeHelpers.GetSubArray<T>(...)` call whose static return type is `T[]`, and the existing F1 array-source converter handles the result. Source-level fixtures live in `Lang12CollectionExpressionTests.cs`. |
| Primary constructors on classes | ⚠️ Compile-only (runtime gap on captured-param read from non-ctor method bodies) | Validated in `Lang12Features.cs::Lang12PrimaryCtorTests` via `PrimaryCtorOnClass`, `PrimaryCtorWithBaseCall`, and `PrimaryCtorMultipleParams`. Roslyn synthesises private backing fields for captured parameters; the constructor body's reference is rewritten at bind time, but reads from a sibling method body (e.g. `Counter.Reset()`'s `_count = initial`) surface a `BoundParameterReference` to the primary-ctor parameter that the Stage 2 deserializer (`BondToAst.ParseArgumentVariable`) cannot resolve — `InvalidOperationException` at convert time. Discovered by WI-63 attempting end-to-end V8 execution of the existing F2 fixtures. The compile-time path (records → `BoundFieldAccess` on the synthesised property) continues to be exercised via `Lang9RecordTests.cs`. No V8 snapshot is emitted under #63 for the F2 fixtures until the visitor lowers captured-parameter reads from non-ctor method bodies. Deferred to a follow-up under #47. |
| `using` alias for any type | ✅ Supported | Validated in `Lang12Features.cs::AliasAnyType` using **tuple-syntax** (`using Pair = (int X, int Y);`) and **array-syntax** (`using Numbers = int[];`) aliases — the C# 12 grammar additions. Aliases are resolved at symbol resolution; bound tree sees the underlying type. Closed generic aliases (legal since C# 1.0) are not exercised here. |
| Default lambda parameter values | ✅ Supported | Validated by `Lang12LambdaDefaultTests.cs::LambdaDefaultParameter` via `Func<int, int> f = (int x = 5) => x + 1;`. The default value is recorded on the lambda's parameter symbol and folded at the call site by Roslyn; the lambda body sees an ordinary `BoundParameter` access. Explicit-argument call site only (`f(7)`) — `Func<int,int>` requires one argument, so the parameterless `f()` form does not bind. |
| Inline arrays | ❌ Out of scope | Issue #47 Non-Goals. |
| `ref readonly` parameters | ❌ Out of scope | Issue #47 Non-Goals. |
| `Experimental` attribute | ✅ Supported | Pure attribute metadata; transparent. |
| Interceptors | ❌ Out of scope | Source-generator-based; depends on generator hosting. |

## C# 13 (2024)

| Feature | Status | Notes |
|---|---|---|
| Escape sequence `\e` | ✅ Supported | Validated in `Lang13Features.cs::EscapeSequenceEsc`. Lex-time fold to `0x1B`. Runtime-gated by `Lang13ExecutionTests` (V8 execution snapshot — asserts the emitted code-unit value matches `0x1B`). |
| Method group natural type | ⚠️ Compile-only for C# 13 overload-pruning (runtime gap) | C# 10's single-overload natural type validated in `Lang10Features.cs::MethodGroupNaturalType`. C# 13's overload-pruning extension (constraint-failing generic candidates pruned during natural-type inference) validated in `Lang13Features.cs::MethodGroupOverloadPruning`. Both compile transparently — Roslyn finalises the delegate symbol at bind time. **Runtime gap surfaced by WI-63:** end-to-end V8 execution of `MethodGroupOverloadPruning` raises `ConverterLocationException: Can't access generic type (!!0) if they are ignored` — the constraint-pruned generic candidate is dead-code-eliminated yet a downstream type reference still resolves to its `!!0` type parameter. The C# 10 single-overload form remains runtime-clean; only the C# 13 pruning fixture is excluded from `Lang13ExecutionTests`. Deferred to a follow-up under #47. |
| `params` collections | ⚠️ Partial (runtime-clean for `List<T>` / `IEnumerable<T>`; compile-only for `IList<T>` / `ICollection<T>` / `IReadOnlyList<T>` / `IReadOnlyCollection<T>`) | `params T[]` continues to work via the existing path. **Phase F4** lights up `params List<T>` (rides the same `NewCollectionInitializerExpression` lowering as direct `List<T>` collection-expression targets); validated in `Lang13Features.cs::ParamsCollections`. `params IEnumerable<T>` is also runtime-clean (the foreach lowering against the underlying `List<T>` facade works). **Phase F5 BCL-interface targets (`IList<T>`/`ICollection<T>`/`IReadOnlyList<T>`/`IReadOnlyCollection<T>`) bind cleanly but have a runtime gap on member access:** `xs.Count` against an interface-typed receiver dispatches through the interface-suffixed getter slot (e.g. `V_get_Count_<suffix>`) at runtime, which the synthesised `List<T>` instance does not expose — V8 raises `TypeError`. Compile-only coverage retained in `Lang13Features.cs::ParamsCollectionsInterfaceCount_RuntimeGap`. `params Span<T>` / `params ReadOnlySpan<T>` are Non-Goals. Runtime-gated by `Lang13ExecutionTests` for the `params List<T>` and `params IEnumerable<T>` shapes (V8 execution snapshot — exercises the synthesised collection construction and `foreach` consumption at the call site). |
| `System.Threading.Lock` | ❌ Out of scope | Issue #47 Non-Goals — depends on the new BCL primitive that does not exist in NScript's `mscorlib`. |
| `field` keyword in property accessors (preview) | ❌ Out of scope | Reserved by C# 14 to mean "synthesised backing field"; pinning `<LangVersion>13</LangVersion>` deliberately avoids the rebinding. |
| Implicit index access on initializers | ⚠️ Untested (Stage 1 gap surfaced) | Compile attempt under `<LangVersion>13</LangVersion>` of `new Buffer { Items = { [^1] = 0 } }` trips `SymbolSerializer.Serialize(TypeSymbol)` with `NotSupportedException` (`type.Kind` is neither `NamedType` nor `Array/Pointer/TypeParameter`) — the nested-initializer composition with `^N` produces a Stage 1 bound shape the serializer does not yet model. The standalone `arr[^1] = 0` form is already covered by `Lang8IndexRangeTests.cs`. Fixture deferred to sub-issue #73; a future Stage 1 audit can promote this row once the unsupported `TypeSymbol` shape is identified. |
| `partial` properties | ✅ Supported | Validated by `Lang13PartialPropertyTests.cs::RoundTripPartialProperty` via one *declaring* partial property and one *implementing* partial property across two `partial class` declarations in the same file. Roslyn merges the two halves into a single property symbol at bind time, so the bound tree sees an ordinary property — no new Stage 1 shape reaches `BoundAstToAstBase`. |

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
- [`limitations.md`](./limitations.md) — consumer-facing summary of the supported surface area.
- [`csharp8-todos.md`](../../csharp8-todos.md) — open C# 8 items; a few rows here reference its open work.
- [ADR 0006](../adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md) — the two-stage pipeline this matrix lives on top of.
- [ADR 0002](../adr/0002-maintain-a-minimal-roslyn-fork-for-nscript-hooks.md) — the Roslyn fork that drives bound-tree shapes.
