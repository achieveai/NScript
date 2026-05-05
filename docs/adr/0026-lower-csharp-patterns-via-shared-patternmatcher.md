# ADR 0026: Lower C# patterns via a shared `PatternMatcher`

- Status: Accepted
- Date: 2026-05-05
- Deciders: NScript compiler team
- Technical Area: Compiler / Stage 2 converter

## Context

Stage 2 of the NScript pipeline (`Cs2Jsc`) translates the deserialised
`CLR.AST.Pattern` tree into JavaScript boolean expressions. Two call
sites need this lowering:

1. `IsPatternConverter` — for `x is <pattern>` expressions.
2. `SwitchExpressionConverter` — for each switch arm of `switch { ... }`.

Historically each call site contained its own ad-hoc dispatch, with
support limited to `ConstantPattern`, `DeclarationPattern`, and
`DiscardPattern`. WI #47 (Phase C) adds `RelationalPattern`,
`BinaryPattern`, and `NegatedPattern` (C# 9 logical / relational
combinators), with `RecursivePattern`, `ListPattern`, `SlicePattern`,
and `ITuplePattern` planned next.

If the new pattern shapes were threaded into both call sites
independently, the lowering rules would diverge — `is` and `switch`
would interpret `not (x and y)` differently, or one would honour a
constant pattern's strict-equality semantics while the other used
loose equality.

## Decision Drivers

- A pattern's JS lowering must be observably identical regardless of
  whether it appears under `is` or in a switch arm.
- New pattern shapes will keep arriving (records-as-patterns,
  list/slice, property patterns) and each one must land in exactly
  one place, not two.
- The lowering needs the scrutinee's static type to choose between
  the type-equality fast path and the `Type__AsType` runtime check
  for declaration patterns; both call sites already have this
  information available.

## Options Considered

### Option 1: Duplicate the new dispatch in each converter

Pros:

- No new module to reason about.

Cons:

- Two sources of truth that drift over time; one regression hides in
  one path while the other still works, masking the bug.
- Each pattern shape lands twice; reviewers must check both copies.

### Option 2: Centralise lowering in a shared `PatternMatcher`

Introduce `PatternMatcher.LowerToCondition(converter, pattern,
scrutinee, scrutineeStaticType)` — a single recursive function that
both converters call.

Pros:

- One canonical lowering for every pattern shape.
- Recursive combinators (`and`, `or`, `not`) compose naturally — the
  recursive call is on the same function.
- New pattern shapes land in exactly one switch arm.
- The scrutinee static type is plumbed through once at the call site
  so the declaration-pattern fast path keeps working from both
  entry points.

Cons:

- One more file in `ExpressionsConverter/`.

## Decision

Selected **Option 2**. `IsPatternConverter` and
`SwitchExpressionConverter` both delegate to
`PatternMatcher.LowerToCondition`. The matcher recognises the C# 8 and
C# 9 pattern shapes today and throws a clear `NotImplementedException`
(pointing at `docs/language/csharp9-13-status.md`) for shapes that are
not yet supported, so a missing case fails loudly at compile time
rather than silently dropping the pattern.

`SwitchExpressionConverter` keeps a small inline fast path for
`BoundConstantPattern` and `BoundDeclarationPattern` to preserve the
existing test fixtures' generated JS verbatim — the matcher's
behaviour for those shapes is identical, but reusing the existing JST
constructors avoids cosmetic diffs in the converter golden files.

## Consequences

Positive:

- Pattern lowering rules cannot diverge between `is` and `switch`.
- The Roslyn-side gotcha — `BoundRelationalPattern` inheriting from
  `BoundConstantPattern`, requiring a specific dispatch order — is
  documented at exactly one switch in `BoundAstToAstBase`; downstream
  the matcher just sees a flat `RelationalPattern`.
- New pattern shapes (recursive / list / slice / property) only need
  to be added to:
  1. `BoundAstToAstBase` (Stage 1 visitor),
  2. ProtoBuf serialisation (next free tag in the 223–235 reserved
     block),
  3. `BondToAst` (deserialisation),
  4. `PatternMatcher.LowerToCondition` (one new switch arm).

Negative:

- Adding a thin function-call indirection on the `is`-pattern hot
  path. The matcher is a `static` dispatch with no allocation
  overhead, so this is a non-issue in practice.

## References

- [Issue #47](https://github.com/achieveai/NScript/issues/47) — C# 9–13
  language gaps umbrella.
- [`docs/language/csharp9-13-status.md`](../language/csharp9-13-status.md)
  — per-feature status and pipeline seams.
- [ADR 0006](./0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
  — the two-stage pipeline this lowering lives on.
