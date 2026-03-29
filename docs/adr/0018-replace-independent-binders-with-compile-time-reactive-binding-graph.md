# ADR 0018: Replace Independent Binders with a Compile-Time Reactive Binding Graph

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: Runtime binding architecture, template code generation

## Context

The existing XWML binding system uses independent `LiveBinder` instances — one per binding expression. Each `LiveBinder` independently subscribes to `PropertyChanged`, evaluates its getter, and writes to its DOM target. This works but has structural limitations:

- **Duplicate subscriptions** — 5 bindings reading `Model.Price` register 5 separate listeners on the same property.
- **No batching** — changing `Price`, `Quantity`, and `Tax` in one synchronous frame triggers 3 independent DOM writes for a computed `Total`, instead of 1.
- **No flip-flop elimination** — `IsActive` toggling `true → false → true` in one frame produces 2 DOM operations instead of 0.
- **Wrapper elements** — `ConditionalBinder` wraps conditional content in a `<span>` to track the DOM range, polluting the author's DOM structure.
- **Independent lifecycles** — each binder manages its own subscription/unsubscription, making it easy to leak listeners when binder counts grow.

Relevant paths:

- graph descriptor shape: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphDescriptor.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphDescriptor.cs)
- graph state: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphState.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphState.cs)
- evaluation engine: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs)
- flush coordinator: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphFlushCoordinator.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphFlushCoordinator.cs)
- topology builder (compiler): [Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs](../../Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs)
- JST emitter (compiler): [Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs](../../Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs)

## Decision Drivers

- Deduplicate property subscriptions (one listener per source property, fan-out via graph edges)
- Enable microtask batching of DOM writes across multiple property changes
- Support `@if`/`@foreach` without wrapper DOM elements (use comment markers instead)
- Keep the graph topology static and shared across instances for memory efficiency
- Maintain backward compatibility with XWML's independent-binder approach (ADR-0019)

## Options Considered

### Option 1: Extend the existing LiveBinder with batching

Add a shared coordinator that collects dirty binders and flushes them in a microtask. Keep the per-binding architecture.

Pros:

- Minimal structural change — existing binders continue to work
- Incremental improvement

Cons:

- Does not solve duplicate subscriptions (still N listeners for N bindings on the same property)
- Batching bolted onto an independent-binder architecture creates ordering complexity
- `@if`/`@foreach` still need wrapper elements or a separate mechanism

### Option 2: Compile-time reactive binding graph (chosen)

Model data flow as a directed acyclic graph determined at compile time. The compiler emits a static `GraphDescriptor` (topology + getter functions + target info). At runtime, a generic `GraphEngine` interprets the descriptor with per-instance mutable `GraphState`.

Pros:

- One subscription per property — fan-out via graph edges
- Microtask batching with flip-flop elimination (compare cached values, skip no-ops)
- Comment markers for `@if`/`@foreach` — no wrapper elements
- Static descriptor shared across all instances (memory efficient)
- Unified lifecycle — single activate/deactivate path for all bindings

Cons:

- More complex compiler output (graph descriptor vs. flat binder array)
- All node types must be handled in the evaluation loop (growing switch)
- Debugging is harder — graph state is an array of indices rather than named binder objects

### Option 3: Virtual DOM / reconciliation approach

Generate a render function that produces a virtual DOM tree, diff against current DOM, and patch.

Pros:

- Familiar model (React/Preact pattern)
- Handles arbitrary expression complexity

Cons:

- Requires a virtual DOM library (new dependency, significant runtime cost)
- Overkill for the binding patterns NScript templates use (property watches, not arbitrary re-renders)
- Does not integrate with the existing `SkinInstance` / `SkinBinderInfo` architecture

## Decision

Razor templates use a compile-time reactive binding graph. The graph is a DAG with:

- **Source nodes** — entry points (DataContext, TemplateParent) with type checking at the boundary
- **Property nodes** — read a property from the parent node's value
- **Computed nodes** — evaluate an expression from multiple parent values
- **Gate nodes** — `@if` conditions that enable/disable subtrees
- **CollectionManager nodes** — `@foreach` that spawns child graphs per item
- **DomTarget nodes** — write a value to a DOM element (text, attribute, style, class)
- **EventBinding nodes** — wire DOM event listeners from method references

### Static/Dynamic Split

**Static (`GraphDescriptor`)** — created once per template at compile time, shared across all instances:
- Node types, getter functions, consumer adjacency list
- Gate indices, default values, target infos, subscription map
- Parent indices (pre-computed for O(1) lookup)

**Dynamic (`GraphState`)** — created per `SkinInstance`, disposed with the instance:
- Cached values, dirty flags, gate-open flags
- Source object references, DOM element references
- Active listeners, child graph states (for `@foreach` items)

### Evaluation Model

**Initial activation**: synchronous linear scan in topological order (index 0..N). Each node evaluates its getter, caches the result, writes to DOM targets.

**Reactive updates**: property change marks node dirty, triggers synchronous flush. Flush scans forward from the dirty node, comparing new values against cached values (flip-flop elimination). Changed values propagate to consumers; unchanged values stop propagation.

**Gate operations**: gates use HTML comment markers (`<!--r:if:0-->`) as anchors. When a gate opens/closes, the engine clones the branch template and inserts/removes DOM elements relative to the marker. No wrapper elements.

**Collection operations**: the CollectionManager creates child `GraphState` instances per item, each with its own subscription lifecycle. Add/remove/replace/reset operations manipulate DOM elements and child graph states incrementally.

### Microtask Batching Infrastructure

A `GraphFlushCoordinator` exists for future depth-based batched flushing. Currently, property changes trigger synchronous flushes for simplicity and test predictability. The coordinator infrastructure is in place and can be activated by routing property callbacks through `GraphEngine.MarkDirty` instead of direct `GraphEngine.Flush`.

## Consequences

Positive:

- One subscription per source property regardless of how many bindings read it
- `@if`/`@foreach` produce clean DOM with comment markers (no wrapper `<span>`)
- Static descriptor is shared — 100 instances of a template allocate only per-instance state arrays
- Null propagation replaces try/catch — wrong-type DataContext produces empty UI, not exceptions
- Unified lifecycle — all subscriptions, events, and collection listeners managed through one strategy

Negative:

- `GraphEngine` is a large static class (1200+ lines) that handles all node types in a single evaluation loop — future node types require editing this file
- Graph state uses fixed-size arrays indexed by node position — sparse allocation for nodes that don't need all arrays (e.g., GateElements allocated for non-gate nodes)
- Debugging graph issues requires understanding index-based state rather than named binder objects
- The compiler must emit correct topological ordering — any ordering bug produces silent runtime corruption

## References

- Related: ADR-0017 (Razor templates), ADR-0019 (IBindingStrategy), ADR-0014 (observable framework)
- Design spec: [docs/superpowers/specs/2026-03-28-reactive-binding-graph-design.md](../superpowers/specs/2026-03-28-reactive-binding-graph-design.md)
