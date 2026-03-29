# ADR 0019: Extract IBindingStrategy from SkinInstance

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: Framework boundaries, public extension points

## Context

`SkinInstance` is the runtime object that manages a bound template — it holds the DOM root, element references, binder state, and child element lifecycle. Before the Razor binding graph (ADR-0018), all binding logic lived directly inside `SkinInstance`: binder iteration, `LiveBinder` creation, `PropertyChanged` subscription, deactivation cleanup.

Adding graph-based binding alongside the existing binder-based binding required a way for `SkinInstance` to delegate binding management without conditional logic (`if (isGraphMode) ... else ...`) scattered throughout its methods.

Relevant paths:

- strategy interface: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/IBindingStrategy.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/IBindingStrategy.cs)
- graph strategy: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphBindingStrategy.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphBindingStrategy.cs)
- legacy strategy: [Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/LegacyBinderStrategy.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/LegacyBinderStrategy.cs)
- SkinInstance: [Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs)

## Decision Drivers

- Keep `SkinInstance` focused on template lifecycle (bind, activate, deactivate, dispose) without binding-engine specifics
- Enable graph-based and binder-based binding to coexist without runtime mode checks
- Preserve identical behavior for existing XWML templates (zero regression risk)
- Allow future binding strategies without modifying `SkinInstance`

## Options Considered

### Option 1: Conditional branching inside SkinInstance

Add `if (isGraphMode)` checks in `Activate()`, `Deactivate()`, `Dispose()`, `UpdateBinderSource()`.

Pros:

- Minimal code change — add conditions around existing code
- No new classes

Cons:

- Scatters binding-engine concerns throughout SkinInstance
- Every new binding approach adds more conditions
- Harder to test each path in isolation

### Option 2: Strategy pattern via IBindingStrategy (chosen)

Extract binding management into an `IBindingStrategy` interface. `SkinInstance` holds one strategy instance selected at construction time. All binding operations delegate through the interface.

Pros:

- `SkinInstance` delegates cleanly: `bindingStrategy.PushInitialValues()`, `bindingStrategy.Deactivate()`
- Each strategy is self-contained and independently testable
- New binding approaches implement the interface without touching `SkinInstance`
- Constructor overloads select the strategy — no runtime type checks needed

Cons:

- Additional interface and two implementation classes
- One level of indirection on the binding hot path (negligible in JS runtime)

## Decision

`SkinInstance` delegates all binding management to an `IBindingStrategy`:

```
IBindingStrategy
├── PushInitialValues(dataContext, templateParent, elementsOfInterest)
├── WireSubscriptions(dataContext, templateParent)
├── OnDataContextChanged(newDataContext)
├── OnTemplateParentChanged(newTemplateParent)
├── Deactivate()
└── Dispose()
```

**Strategy selection** happens in the constructor via overloading:

- `SkinInstance(..., NativeArray<SkinBinderInfo> binders, ...)` → `LegacyBinderStrategy`
- `SkinInstance(..., GraphDescriptor graphDescriptor, ...)` → `GraphBindingStrategy`

The `LegacyBinderStrategy` contains the exact binder loop code extracted from the original `SkinInstance` — same iteration order, same `LiveBinder` creation logic, same `QueuedActivation`/`QueuedDeactivation` behavior. No behavioral changes.

**What stays in SkinInstance:**

- Template lifecycle: `Bind()`, `Activate()`, `Deactivate()`, `Dispose()`
- Child UIElement cascade: DataContext propagation, child activate/deactivate
- Part ID mapping: `GetChildById()`
- DOM management: root element, child nodes transfer

These are orthogonal to the binding strategy and remain in `SkinInstance`.

## Consequences

Positive:

- XWML templates behave identically — `LegacyBinderStrategy` is a mechanical extraction with no logic changes
- `SkinInstance` dropped from ~450 lines to ~250 lines of binding-independent lifecycle code
- Graph-based and binder-based bindings are completely isolated from each other
- Future binding approaches (e.g., a hypothetical virtual-DOM strategy) only need to implement `IBindingStrategy`

Negative:

- One extra virtual dispatch per binding operation (insignificant in JS)
- `LegacyBinderStrategy` duplicates the `hasDataContextBinding` array tracking that `SkinInstance` previously owned — bridged via a public property for `SkinInstance`'s child element DataContext propagation
- `QueuedDeactivation` is a no-op for graph mode but still scheduled via `TaskScheduler` — minor overhead

## References

- Related: ADR-0017 (Razor templates), ADR-0018 (binding graph)
- Pattern: Strategy (GoF) — runtime selection of algorithm family
