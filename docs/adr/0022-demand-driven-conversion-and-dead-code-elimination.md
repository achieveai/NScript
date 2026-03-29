# ADR 0022: Demand-Driven Conversion and Dead Code Elimination

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: Compiler pipeline, code generation strategy, output size optimization

## Context

NScript compiles C# assemblies to JavaScript. A typical project references framework libraries (`Sunlight.Framework`, `Sunlight.Framework.UI`, `System.Web.Html`, `mscorlib`) that contain hundreds of types and thousands of methods. Emitting JavaScript for every type and method in every referenced assembly would produce enormous output — most of it unused at runtime.

The compiler needs a strategy for deciding what to convert to JavaScript and what to omit.

Relevant paths:

- entry point discovery: [Sources/Compiler/NScript.Converter/Builder.cs](../../Sources/Compiler/NScript.Converter/Builder.cs) (`GetEntryPoint`, `methodDefinitionsToEmit`)
- demand-driven walk: [Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs) (`Convert`, `WalkUsedDependencies`, `ProcessMembers`, `ProcessTypes`)
- dependency analysis: [Sources/Compiler/NScript.Converter/TypeSystemConverter/DependencyAnalyzer.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/DependencyAnalyzer.cs)
- identifier minification: [Sources/Compiler/NScript.JST/IdentifierMinifiedNamer.cs](../../Sources/Compiler/NScript.JST/IdentifierMinifiedNamer.cs)
- plugin methods-to-emit: `IMethodConverterPlugin.GetMethodsToEmitPass1()` / `GetMethodsToEmitPassN()`

## Decision Drivers

- Minimize JavaScript output size — only emit code that can actually execute
- Avoid manual "include" lists — the compiler should determine reachability automatically
- Handle virtual method dispatch correctly — if a virtual method is called on an interface, all concrete implementations must be emitted
- Support compiler plugins that inject additional entry points (template factories, test methods)
- Enable identifier-level dead code elimination — symbols resolved but never referenced in the final JST should not appear in output

## Options Considered

### Option 1: Emit everything

Convert all types and methods in all referenced assemblies to JavaScript.

Pros:

- Simple — no reachability analysis needed
- No risk of missing a needed method

Cons:

- Massive output size — framework libraries alone would produce megabytes of unused JavaScript
- Slower compilation — converting unused code wastes time
- Larger download and parse time for the browser

### Option 2: Demand-driven conversion from entry points (chosen)

Start from the application's `Main()` method (marked with `[EntryPoint]`) plus any methods declared by plugins (`GetMethodsToEmitPass1/N`). Walk the dependency graph to discover all reachable types and members. Convert only what is reachable. Drop identifiers with zero references.

Pros:

- Output contains only reachable code — dramatic size reduction
- Automatic — no manual include lists
- Virtual dispatch is handled by walking interface overrides to ensure all implementations of called virtuals are included
- Plugins can inject additional roots (template factories, test entry points)

Cons:

- Reachability analysis must be correct — missing a dependency produces a runtime error
- Virtual method expansion is currently brute-force (all implementations of a used virtual, not just those for instantiated types)
- Reflection-like patterns (string-based type lookup) can reference types the walker doesn't see

## Decision

NScript uses demand-driven conversion with three phases: **seed**, **walk**, and **eliminate**.

### Phase 1: Seed

The conversion starts from explicit roots:

1. **Application entry point** — the public static `Main()` method on a class with `[EntryPoint]` attribute, discovered by `Builder.GetEntryPoint()`
2. **Plugin-declared methods** — each `IRuntimeConverterPlugin` can declare additional methods via `GetMethodsToEmitPass1()` (before type processing) and `GetMethodsToEmitPassN()` (after N-th pass, for late-bound references). Template plugins use this to inject skin factory methods and getter functions.

All seed methods are enqueued into `usedMembersToProcess`. Their declaring types are enqueued into `usedTypeReferencesToProcess`.

### Phase 2: Walk (WalkUsedDependencies)

The converter iterates until both queues are empty:

1. **ProcessTypes** — for each type in the queue, resolve it, register its type converter, and enqueue any base types, implemented interfaces, and generic type arguments.
2. **ProcessMembers** — for each member (method or field) in the queue, ensure its declaring type is processed, then convert it to JST. During conversion, any referenced types, methods, or fields are enqueued (the converter calls `RuntimeScopeManager.Resolve()` which triggers `usedMembersToProcess.Enqueue()`).
3. **Virtual dispatch expansion** — after each iteration, scan all processed types for interface overrides. If a virtual method is in the `virtualMethodsUsed` set, all concrete implementations of that method across all known types are enqueued. This ensures that `interface.Method()` calls work correctly when the concrete type is only known at runtime.

The walk terminates when no new types or members are discovered. At this point, `typesDefinitionsUsed` and `membersProcessed` contain exactly the set of reachable code.

### Phase 3: Eliminate

After JST generation and identifier resolution:

1. **Identifier usage tracking** — every `IIdentifier` tracks which scopes reference it via `AddUsage()`. During JST construction, each `IdentifierExpression` calls `AddUsage` on its identifier.
2. **Minification** — `IdentifierMinifiedNamer.MinifyNames()` assigns short names based on usage frequency. Identifiers with zero usages receive no name assignment.
3. **Output** — `JSWriter` emits the final JavaScript. Declarations for identifiers with zero references produce no output — the method, field, or type simply does not appear in the generated script.

This means that even if a type is reachable (Phase 2 included it), individual methods on that type that are never called will not appear in the output. The elimination is at the identifier granularity, not the type granularity.

### Consequences for Compiler Plugins

Plugins that generate JST must use the identifier resolution system (ADR-0021) so that the demand walker and eliminator can see their references:

- Template factory functions are registered via `GetMethodsToEmitPass1/N` to become roots
- Runtime types referenced by templates must be resolved via `RuntimeScopeManager.Resolve()` so they are enqueued for processing
- If a plugin emits a raw string name that bypasses `Resolve()`, the referenced type/method may not be walked and will be missing from the output

## Consequences

Positive:

- JavaScript output contains only reachable code — typically 10-50x smaller than full-assembly emission
- No manual include/exclude lists — the dependency graph is authoritative
- Dead methods and unused types are automatically excluded
- Identifier-level elimination means even partially-used types only emit their called methods
- Plugins participate in the same reachability system as application code

Negative:

- The brute-force virtual dispatch expansion can over-include methods — all implementations of a used virtual are emitted, even if some concrete types are never instantiated
- Reflection-like patterns (`Type.GetType("MyClass")`) can reference types invisible to the walker — these require manual `[KeepType]` attributes or plugin roots
- The walk is iterative (re-runs until stable) which can be slow for deeply connected type graphs
- Plugin authors must understand that their generated code participates in the reachability walk — bypassing `Resolve()` causes silent omissions

## References

- Related: ADR-0006 (compiler pipeline), ADR-0021 (resolved identifiers), ADR-0013 (multi-frontend architecture)
- Entry point convention: `[EntryPoint]` attribute on a class with public static `Main()`
