# Reactive Binding Graph for NScript Skin Templates

## Overview

Replace the current independent-binder architecture (LiveBinder, MultiDependencyBinder, ConditionalBinder, CollectionBinder) with a **compile-time reactive graph** that models data flow from sources (DataContext, TemplateParent) through property accesses, computed expressions, and conditional gates to DOM targets and event bindings.

The graph topology is determined at compile time by the Razor template compiler and emitted as a static descriptor. At runtime, a generic `BindingGraph` engine interprets the descriptor with per-instance mutable state. Changes are collected via dirty-marking and flushed in a single microtask batch, with depth-based priority ordering across nested skins.

### Goals

- **One subscription per property** — 5 bindings reading `Model.Price` produce 1 listener, not 5
- **Microtask batching** — changing Price, Quantity, Tax in one frame produces 1 DOM update for a computed Total, not 3
- **Flip-flop elimination** — `IsActive` toggling `true -> false -> true` in one frame produces no DOM change
- **No runtime exceptions** — type mismatches produce null/default values, never thrown exceptions
- **Backward compatible** — XWML templates continue using the existing binder path unchanged
- **Memory efficient** — static graph descriptor shared across all instances; only runtime state is per-instance

### Non-Goals

- Runtime graph construction (graph shape is always compile-time)
- Deep equality comparison (reference equality only)
- Async/parallel flush (single-threaded JS runtime)
- XWML migration (XWML stays on legacy `SkinBinderInfo[]` path)

## 1. Core Model — The Reactive Binding Graph

### What It Is

A directed acyclic graph where:
- **Roots** are data sources (DataContext, TemplateParent)
- **Interior nodes** are property accesses, computed expressions, type guards, and gates
- **Leaves** are DOM targets (text content, attributes, styles, classes) and event bindings

### Node Types

| Node Type | Role | Inputs | Output |
|---|---|---|---|
| **Source** | Entry point (DataContext/TemplateParent) | External object | Type-checked reference or null |
| **Property** | Read a property from parent node | 1 parent node | Property value or null |
| **Computed** | Evaluate expression from multiple inputs | N parent nodes | Computed value or null |
| **TypeGuard** | Cast check at explicit cast boundaries | 1 parent node | Typed reference or null |
| **Gate** | @if condition — enables/disables subtree | 1 condition node | Boolean (controls propagation) |
| **CollectionManager** | @foreach — spawns child graphs per item | 1 collection node | Creates/destroys child graph instances |
| **DOMTarget** | Writes value to DOM element | 1 value node | Side effect (DOM update) |
| **EventBinding** | Resolves method ref, wires addEventListener | 1 method node | Side effect (event listener) |

### Key Property: One Subscription Per Source Property

Today: 5 LiveBinders watching `Model.Price` = 5 listeners on the same property.

New: One Property node for `Model.Price` in the graph, fan-out edges to 5 consumers. One listener, one callback, marks one node dirty, propagation fans out during flush.

### Value Caching

Every node caches its last output value in the dynamic state. On re-evaluation during flush:
- If new value === cached value (reference equality): **stop propagation**. Consumers stay clean.
- If different: update cache, mark consumers dirty, continue flush.

This is the flip-flop optimization — `IsActive` going `true -> false -> true` within one synchronous frame resolves to "net change: none" when the flush compares against the cached value.

## 2. Static/Dynamic Split

### Static Graph (Shared — one per template)

Created at compile time, stored alongside the template's DOM cache (like today's `tmplStore`). Immutable. Shared across all instances of this template.

Contains everything that describes the graph's **shape and behavior**:
- Node types
- Getter functions
- Consumer pointers (edges / adjacency list)
- Gate indices (which gate controls each node, -1 if ungated)
- Default values (used when gate closes or source is null)
- Target info (element index, setter function)
- Subscription map (property names, source-to-node mapping)
- Collection descriptors (item graph reference)
- Event descriptors (element index, event name)
- Subscription mode flag (per-property vs per-object)
- Source type name (for type checking)
- Node count

```javascript
var OrderSkin_graph = {
  nodeTypes:     [SOURCE, PROPERTY, DOM_TARGET, PROPERTY, COMPUTED, ...],
  getters:       [null, getter_CustomerName, null, getter_Price, computed_Total, ...],
  consumers:     [[1,3,7,11,13], [2], [], [5], [5], [6], ...],
  gateIndices:   [-1, -1, -1, -1, -1, -1, -1, -1, 8, 8, ...],
  defaultValues: [null, null, "", null, null, "", ...],
  targetInfos:   [null, null, {elem:0, set:SetText}, ...],
  subscriptions: [["CustomerName",1], ["Price",3], ["Quantity",4], ...],
  events:        [{elem:3, evt:"click", nodeIdx:13}],
  collections:   [{managerIdx:12, itemGraph: OrderItem_graph}],
  sourceType:    "OrderViewModel",
  subscribeMode: SUBSCRIBE_PER_PROPERTY,
  nodeCount:     15,
};
```

### Dynamic Graph (Per-instance — one per SkinInstance)

Created when `SkinInstance` is instantiated. Mutable. Disposed with the instance.

Contains everything that represents **current runtime state**:

```javascript
function createGraphState(staticGraph, elemRefs) {
  return {
    values:         new Array(staticGraph.nodeCount),
    dirty:          new Array(staticGraph.nodeCount),
    gateOpen:       new Array(staticGraph.nodeCount),
    sources:        [null, null],  // [dataContext, templateParent]
    elemRefs:       elemRefs,      // DOM element references
    listeners:      [],            // active subscription handles
    itemMarkers:    [],            // @foreach comment node markers
    childGraphs:    [],            // @foreach child graph dynamic states
    flushScheduled: false,
    depth:          0,
  };
}
```

### Why This Split Matters

- **Memory**: 100 instances of `OrderSkin` share one `OrderSkin_graph`. Each only allocates its dynamic state arrays.
- **Cache-friendly**: Static arrays are dense and accessed sequentially during flush (linear scan in topological order). Dynamic arrays have matching indices.
- **GC pressure**: Static graph is long-lived. Dynamic state has short-lived references that can be nulled on deactivate without touching the shared structure.
- **@foreach efficiency**: All child items share the same static item graph descriptor. 1000 items = 1 static graph + 1000 small dynamic state arrays.

## 3. Lifecycle — Two-Phase Activation + Microtask Batching

### Activation (Initial Render)

Two phases for simplicity:

**Phase 1 — Synchronous value push:**
- Graph receives DataContext/TemplateParent source
- Source node type-checks incoming object
- Walks all nodes in topological order (linear scan of static arrays)
- Executes each getter, caches value in dynamic state
- Writes to DOM targets directly
- Wires event bindings
- For gated nodes: if gate is false, apply default values and skip subtree
- For CollectionManager nodes: create child graphs for initial items
- No subscriptions registered yet
- Result: DOM is fully rendered with initial values

**Phase 2 — Deferred subscription wiring:**
- Scheduled via `TaskScheduler.EnqueueLowPriTask` (same as today's QueuedActivation)
- Walks Source nodes and subscription map
- Registers one `PropertyChanged` listener per unique (source, propertyName) pair (SUBSCRIBE_PER_PROPERTY mode) or one listener per source object (SUBSCRIBE_ALL mode)
- After this, the graph is "live" — ready for reactive updates

### Reactive Updates (After Activation)

Three stages per update cycle:

**Stage 1: COLLECT** (synchronous, instant)
- `PropertyChanged` fires on source object
- Graph marks the corresponding Property node dirty in dynamic state
- If no flush is scheduled: `GraphFlushCoordinator.scheduleDirty(graph)` → schedules microtask
- Subsequent `PropertyChanged` calls in same frame: mark more nodes dirty, microtask already pending

**Stage 2: FLUSH** (runs on microtask boundary via TaskScheduler)
- `GraphFlushCoordinator.flushAll()` processes all dirty graphs in depth order
- For each dirty graph: linear scan of nodes array
  - Skip clean nodes
  - Skip gated nodes whose gate is closed
  - Execute getter with current input values
  - Compare output to cached value (reference equality)
  - If unchanged: clear dirty flag, stop (consumers stay clean)
  - If changed: update cached value, mark consumers dirty

**Stage 3: APPLY** (part of flush walk)
- When a DOMTarget node is reached and its input changed: write to DOM
- When an EventBinding node's method reference changed: unwire old listener, wire new
- When a Gate node flips: insert/remove DOM range (see Section 5)
- When a CollectionManager's collection changed: create/destroy child graphs (see Section 6)

### Microtask Scheduling

```
First PropertyChanged in a frame:
  -> mark node dirty
  -> coordinator.scheduleDirty(graph)
  -> TaskScheduler.EnqueueHighPriTask(coordinator.flushAll)
  -> coordinator.flushScheduled = true

Subsequent PropertyChanged in same frame:
  -> mark node dirty
  -> coordinator.scheduleDirty(graph)
  -> coordinator.flushScheduled already true, skip scheduling

flushAll runs:
  -> sort pending graphs by depth
  -> flush each graph
  -> coordinator.flushScheduled = false
```

### DataContext Change

1. Source node receives new object
2. Type-check against expected type (null if wrong type)
3. If Phase 2 already ran (subscriptions active): unsubscribe from old source, subscribe to new
4. Mark all consumers of the Source node dirty
5. Schedule flush

### Deactivation / Dispose

- **Deactivate**: unsubscribe all listeners, keep graph structure and cached values (can reactivate)
- **Dispose**: unsubscribe, set all values to default, release DOM references, destroy child graphs

## 4. Type Safety — Guards and Graceful Degradation

### Source Boundary Type Checks

Every Source node has a compiled type expectation (from the `sourceType` field in the static graph). On receiving a new object:

```javascript
function sourceTypeCheck(obj, expectedType) {
  return (obj != null && expectedType.isInstanceOfType(obj))
    ? obj
    : null;
}
```

If the check fails, the Source node outputs `null`. Null propagates naturally:
- Property nodes: null input -> null output (getter not called)
- Computed nodes: any null input -> null output (expression not evaluated)
- Gate nodes: null condition -> false (gate closed, subtree inactive)
- DOM targets: null value -> default value (clear text, remove attribute, etc.)
- Event bindings: null method -> unwire listener (no handler active)

**No exceptions thrown. Ever.** A wrong-type DataContext produces an empty but stable UI.

### Explicit Cast Type Guards

Where the C# expression has an explicit cast, the compiler emits a TypeGuard node:

```
Source(DataContext) -> Property(SelectedItem) -> TypeGuard(CustomerVM) -> Property(Name) -> DOMTarget
```

TypeGuard nodes are only emitted where the Razor expression has an explicit cast. Normal property access trusts the source type validated at the boundary.

### Gate Closure — Null Propagation and Memory Release

When a Gate node evaluates to `false`:
1. Set gate's output to `false`, set `gateOpen[gateIdx]` to `false`
2. For each consumer recursively (depth-first through `consumers` pointers):
   - Set node value to `defaultValue` (from static graph)
   - Release cached object references (in dynamic state)
   - If DOMTarget: apply `defaultValue` to DOM
   - If EventBinding: unwire listener
   - If CollectionManager: dispose all child graphs, clear DOM
3. Mark all affected nodes as clean (they have their default values)

When gate reopens:
1. Set `gateOpen[gateIdx]` to `true`
2. Mark all gated nodes as dirty
3. Normal flush re-evaluates them from source, pulling fresh values

This ensures gated subtrees don't hold references to objects that are no longer relevant.

## 5. DOM Markers and Range Operations

### Marker Strategy

All dynamic DOM regions use comment nodes as markers. The factory emits them as part of the template HTML:

```javascript
domStore[100].innerHTML =
  '<h1></h1>' +
  '<span class="total"></span>' +
  '<!--r:if:0-->' +
  '<!--r:fe:0-->' +
  '<button></button>';
```

Comment markers are lightweight (no layout impact), invisible to the user, and survive `cloneNode(true)`.

### @if Range Operations

The Gate node tracks (in dynamic state):
- `markerElement`: reference to the comment node
- `elementCount`: how many DOM nodes the branch inserts (known at compile time, stored in static graph)
- `branchTemplates`: true-branch and false-branch DOM fragments (pre-compiled in static graph)

**Gate opens (condition becomes true):**
```
fragment = trueBranch.cloneNode(true)
Insert all child nodes of fragment after markerElement
```

**Gate closes (condition becomes false):**
```
Remove elementCount nodes following markerElement
```

**@if/@else toggle (switch branches):**
```
Remove current branch elements (N nodes after marker)
Clone other branch template
Insert after marker
```

No wrapper `<span>` element. The template author's DOM structure is preserved exactly.

### @foreach Item Tracking

The CollectionManager tracks (in dynamic state):
- `collectionMarker`: reference to the collection's comment node
- `itemMarkers[]`: array of comment nodes, one per item
- `itemElementCounts[]`: how many DOM nodes each item injected
- `childGraphStates[]`: corresponding dynamic graph states for each item

Each item's DOM region is delimited by a comment marker:

```html
<!--r:fe:0-->              <!-- collection marker -->
<!--r:item:0-->            <!-- item 0 -->
<li>Item A</li>
<span>detail A</span>
<!--r:item:1-->            <!-- item 1 -->
<li>Item B</li>
<span>detail B</span>
```

**Add item at index N:**
1. Create comment marker `<!--r:item:N-->`
2. Clone item template from static item graph
3. Create child graph dynamic state, push initial values into cloned DOM
4. Find insertion point: after previous item's last element (or after collection marker if index 0)
5. Insert comment marker + all item root elements
6. Register child graph with flush coordinator at parent depth + 1

**Remove item at index N:**
1. Find `<!--r:item:N-->` marker
2. Remove marker + `itemElementCounts[N]` elements after it
3. Dispose child graph dynamic state (unsubscribe, release references)
4. Splice out from tracking arrays

**Reset:**
1. Remove all elements between collection marker and the end of the collection region
2. Dispose all child graph states
3. Recreate from new collection contents

## 6. @foreach — Child Graph Lifecycle

### Child Graph Structure

Each item gets:
- Its own dynamic graph state using the compiled **static item graph descriptor** (shared across all items)
- Its own DOM fragment (cloned from item template in the static graph)
- Depth = parent depth + 1
- DataContext = the collection item

### Collection Change Handling

During the parent graph's flush, when the CollectionManager node's collection value has changed:

| Collection Event | Action |
|---|---|
| **Add(item, index)** | Create child graph state + DOM fragment, insert at position |
| **Remove(index)** | Deactivate + dispose child graph state, remove DOM elements |
| **Replace(index, item)** | Update existing child graph's Source node with new item, mark dirty |
| **Reset** | Dispose all child graph states, recreate from new collection |

### Item Isolation

Items are fully independent. Changing `item[5].Name`:
1. Marks dirty in item[5]'s graph state
2. item[5]'s graph joins flush coordinator at its depth
3. Only item[5]'s DOM updates
4. Items 0-4 and 6+ are untouched

## 7. Flush Priority — Depth-Based Ordering

### The Problem

A property change on a parent DataContext could dirty multiple graphs at different nesting depths. Child graphs must see updated sources from their parents.

### Depth-Based Priority

Each graph state knows its depth:
- Root skin (top-level UISkinableElement) = depth 0
- Child skin inside a sub-control = parent depth + 1
- @foreach item skins = collection manager's graph depth + 1

### Global Flush Coordinator

Single instance, manages all pending dirty graphs:

```
GraphFlushCoordinator:
  pendingGraphs: organized by depth (array of depth buckets)

  scheduleDirty(graphState):
    add graphState to pendingGraphs at graphState.depth
    if not flushScheduled:
      TaskScheduler.EnqueueHighPriTask(flushAll)
      flushScheduled = true

  flushAll():
    for depth 0 to maxDepth:
      for each graph at this depth:
        graph.flush()
    pendingGraphs.clear()
    flushScheduled = false
```

### Why This Is Safe

Parent graph at depth N flushes before children at depth N+1. If parent's flush changes a child's DataContext, the child graph gets its Source node marked dirty during the parent's apply stage — and it's still in the queue at depth N+1 waiting to flush.

## 8. Integration with SkinInstance

### Internal Strategy Selection

`SkinInstance` gains an internal binding strategy selected based on what the factory provides:

- Factory provides graph descriptor -> `GraphBindingStrategy`
- Factory provides `SkinBinderInfo[]` array -> `LegacyBinderStrategy` (today's code, unchanged)

### Strategy Interface

```
IBindingStrategy:
  PushInitialValues(dataContext, templateParent, elementsOfInterest)
  WireSubscriptions()
  OnDataContextChanged(newDataContext)
  OnTemplateParentChanged(newTemplateParent)
  Deactivate()
  Dispose()
```

### SkinInstance Delegation

Existing public API unchanged. Internal methods delegate:

- `Activate()` -> `strategy.PushInitialValues()` + schedule `strategy.WireSubscriptions()`
- `UpdateDataContext()` -> `strategy.OnDataContextChanged()`
- `Deactivate()` -> `strategy.Deactivate()`
- `Dispose()` -> `strategy.Dispose()`

Child UIElement cascade (DataContext propagation, child Activate/Deactivate) stays in SkinInstance — it's orthogonal to the binding strategy.

### Backward Compatibility

XWML templates produce `SkinBinderInfo[]` arrays -> `LegacyBinderStrategy` -> exact same behavior as today. Zero regression risk.

## 9. Per-Object Subscription Option

### Motivation

For templates with few bindings (3-5 properties), registering one listener per property may be more overhead than a single "any property changed" listener that marks all children dirty and relies on value comparison to filter unchanged nodes.

### Compile-Time Flag

The static graph descriptor includes a `subscribeMode` field:

- **SUBSCRIBE_PER_PROPERTY** (default): One listener per (source, propertyName) pair. Property change marks only the specific Property node dirty. Best for templates with many bindings.
- **SUBSCRIBE_ALL**: One listener per source object for all property changes. Property change marks ALL direct Property node children dirty. Value comparison at flush filters unchanged. Best for templates with few bindings.

The compiler selects the mode based on binding count heuristics. The runtime engine's subscription wiring in Phase 2 checks this flag and registers accordingly.

This is a future optimization — the initial implementation uses SUBSCRIBE_PER_PROPERTY exclusively. The flag is reserved in the descriptor to avoid breaking changes when the optimization is added.

## 10. Compiled Output — Full Example

Given this template:

```razor
@model OrderViewModel

<h1>@Model.CustomerName</h1>
<span class="total">@(Model.Price * Model.Quantity)</span>

@if (Model.HasDiscount)
{
    <div class="discount">@Model.DiscountLabel</div>
}

<ul>
@foreach (var item in Model.Items)
{
    <li>@item.Name</li>
}
</ul>

<button onclick="@Model.OnSubmit">Submit</button>
```

### Generated JavaScript

```javascript
// === STATIC GRAPH (shared, immutable) ===

var OrderSkin_graph = {
  nodeTypes:     [SOURCE, PROPERTY, DOM_TARGET, PROPERTY, PROPERTY,
                  COMPUTED, DOM_TARGET, PROPERTY, GATE,
                  PROPERTY, DOM_TARGET, PROPERTY, COLLECTION_MGR,
                  PROPERTY, EVENT_BINDING],
  getters:       [null, getter_CustomerName, null, getter_Price, getter_Quantity,
                  computed_PriceTimesQty, null, getter_HasDiscount, null,
                  getter_DiscountLabel, null, getter_Items, null,
                  getter_OnSubmit, null],
  consumers:     [[1,3,4,7,9,11,13], [2], [], [5], [5],
                  [6], [], [8], [],
                  [10], [], [12], [],
                  [14], []],
  gateIndices:   [-1,-1,-1,-1,-1, -1,-1,-1,-1, 8,8,-1,-1, -1,-1],
  defaultValues: [null,null,"",null,null, null,"",null,false,
                  null,"",null,null, null,null],
  targetInfos:   [null,null,{elem:0,set:SetText},null,null,
                  null,{elem:1,set:SetText},null,{marker:0,trueTmpl:T,falseTmpl:null,count:1},
                  null,{elem:2,set:SetText},null,{marker:1,itemGraph:OrderItem_graph},
                  null,{elem:3,evt:"click"}],
  subscriptions: [["CustomerName",1],["Price",3],["Quantity",4],
                  ["HasDiscount",7],["DiscountLabel",9],
                  ["Items",11],["OnSubmit",13]],
  sourceType:    OrderViewModel,
  subscribeMode: SUBSCRIBE_PER_PROPERTY,
  nodeCount:     15,
};

// Item graph (shared across all @foreach items)
var OrderItem_graph = {
  nodeTypes:     [SOURCE, PROPERTY, DOM_TARGET],
  getters:       [null, getter_ItemName, null],
  consumers:     [[1], [2], []],
  gateIndices:   [-1, -1, -1],
  defaultValues: [null, null, ""],
  targetInfos:   [null, null, {elem:0, set:SetText}],
  subscriptions: [["Name", 1]],
  sourceType:    ItemViewModel,
  subscribeMode: SUBSCRIBE_PER_PROPERTY,
  nodeCount:     3,
};

// Getter functions (minified via NScript JST)
function getter_CustomerName(s) { return s.get_customerName_a(); }
function getter_Price(s) { return s.get_price_b(); }
function getter_Quantity(s) { return s.get_quantity_c(); }
function computed_PriceTimesQty(s) { return s.get_price_b() * s.get_quantity_c(); }
function getter_HasDiscount(s) { return s.get_hasDiscount_d(); }
function getter_DiscountLabel(s) { return s.get_discountLabel_e(); }
function getter_Items(s) { return s.get_items_f(); }
function getter_OnSubmit(s) { return s.get_onSubmit_g(); }
function getter_ItemName(s) { return s.get_name_h(); }

// Factory
function OrderSkin_factory(skinFactory, doc) {
  var domStore, htmlRoot;
  if (!(domStore = DocStorageGetter(doc))[100]) {
    domStore[100] = doc.createElement("div");
    domStore[100].innerHTML =
      '<h1></h1>' +
      '<span class="total"></span>' +
      '<!--r:if:0-->' +
      '<ul><!--r:fe:0--></ul>' +
      '<button>Submit</button>';
  }
  htmlRoot = domStore[100].cloneNode(true);
  var elemRefs = [
    /* 0 */ GetElementFromPath(htmlRoot, [0]),
    /* 1 */ GetElementFromPath(htmlRoot, [1]),
    /* 2 */ null, // placeholder, resolved by gate
    /* 3 */ GetElementFromPath(htmlRoot, [4]),
  ];
  return SkinInstance_factory(
    skinFactory, htmlRoot, [], elemRefs,
    OrderSkin_graph,   // graph descriptor
    null, 0, 0);
}
```

### Comparison: Today vs Graph

| Aspect | Today (SkinBinderInfo[]) | New (Graph) |
|---|---|---|
| Subscriptions for 7 properties | 7+ listeners (duplicates possible) | 7 listeners (deduplicated at compile time) |
| Price + Quantity both change | 2 separate DOM updates for Total | 1 DOM update (microtask batched) |
| IsActive flips true/false/true | 2 DOM swaps | 0 DOM changes (flip-flop eliminated) |
| @if toggle | ConditionalBinder wraps in `<span>`, clones | Comment marker + range insert/remove, no wrapper |
| @foreach item bindings | Clone DOM without bindings | Full reactive child graph per item |
| Event wiring | Separate lifecycle | Part of graph, unified lifecycle |
| DataContext swap | Each LiveBinder re-subscribes independently | One Source node update, cascading flush |
| Memory (100 instances) | 100 x SkinBinderInfo[] (shared via tmplStore) | 1 shared static graph + 100 small dynamic states |
| Wrong-type DataContext | try/catch swallows TypeError | Type check at source, null propagation, no exceptions |

## 11. Summary

The reactive binding graph replaces the current independent-binder model with a unified, compile-time-optimized data flow graph. The key innovations are:

1. **Graph topology at compile time** — the compiler knows all property accesses, dependencies, and structural blocks. No runtime graph construction.
2. **Static/dynamic split** — graph shape is shared, only values and dirty flags are per-instance.
3. **Microtask batching with flip-flop elimination** — changes collect, flush once, skip no-ops.
4. **Comment markers for clean DOM manipulation** — no wrapper elements for @if/@foreach.
5. **Depth-based flush coordinator** — parent skins flush before children, ensuring correct data flow.
6. **Type safety by design** — null propagation and default values replace try/catch exception swallowing.
7. **Backward compatible** — SkinInstance delegates to strategy, XWML path unchanged.
