# Reactive Binding Graph Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the independent-binder architecture (LiveBinder, ConditionalBinder, CollectionBinder) with a compile-time reactive graph that models data flow from sources through property nodes to DOM targets, with microtask batching and flip-flop elimination.

**Architecture:** Compile-time DAG with static descriptor (shared, immutable) + dynamic state (per-instance, mutable). Changes are collected via dirty-marking and flushed in a single microtask batch. SkinInstance delegates to an internal binding strategy (graph vs legacy) selected by what the factory provides. XWML templates continue using the existing binder path unchanged.

**Tech Stack:** C# (compiled to JS via NScript), NScript Framework (`Sources/Framework/`), NScript Compiler (`Sources/Compiler/RazorSkinParser/`), MSTest, QUnit browser tests

**Spec:** `docs/superpowers/specs/2026-03-28-reactive-binding-graph-design.md`

---

## Scope Decomposition

This plan covers six phases, each producing testable artifacts:

| Phase | Scope | Deliverable |
|-------|-------|-------------|
| 1 | Graph Engine Core (runtime) | `BindingGraph/` classes that evaluate a static descriptor with dynamic state |
| 2 | SkinInstance Integration | Strategy pattern in SkinInstance, backward-compatible with XWML |
| 3 | Compiler — Graph Topology Builder | IR → graph node arrays |
| 4 | Compiler — Graph Descriptor Emission | Graph topology → JS (JST) |
| 5 | Advanced Runtime — Gates, Collections, Events | @if/@else, @foreach, event bindings |
| 6 | End-to-End Browser Tests | Full pipeline verification |

---

## File Structure

### New Framework Files: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/`

| File | Responsibility |
|------|---------------|
| `GraphNodeType.cs` | Node type enum constants: SOURCE, PROPERTY, COMPUTED, DOM_TARGET, EVENT_BINDING, GATE, COLLECTION_MGR, TYPE_GUARD |
| `GraphDescriptor.cs` | Static graph shape — node types, getters, consumers, gate indices, default values, target infos, subscriptions, events, collections. Shared across instances. |
| `GraphState.cs` | Per-instance mutable state — values, dirty flags, gate-open flags, source refs, element refs, listeners, child graph states |
| `GraphEngine.cs` | Core evaluation: `PushInitialValues()` (sync walk), `Flush()` (dirty-node walk with value caching + propagation), `MarkDirty()`, gate closure/reopening, null propagation |
| `GraphFlushCoordinator.cs` | Global singleton: collects dirty graphs, schedules microtask via TaskScheduler, flushes in depth order |
| `IBindingStrategy.cs` | Strategy interface: `PushInitialValues`, `WireSubscriptions`, `OnDataContextChanged`, `OnTemplateParentChanged`, `Deactivate`, `Dispose` |
| `GraphBindingStrategy.cs` | Graph-based strategy implementation wrapping GraphEngine + GraphState |
| `LegacyBinderStrategy.cs` | Wraps existing SkinBinderInfo[]/LiveBinder[] logic extracted from SkinInstance |

### New Compiler Files: `Sources/Compiler/RazorSkinParser/CodeGen/`

| File | Responsibility |
|------|---------------|
| `GraphTopologyBuilder.cs` | Walks IR tree, assigns node indices, builds adjacency list, subscription map, gate/collection/event metadata |
| `GraphDescriptorEmitter.cs` | Emits JS object literal for static graph descriptor (text-based, for snapshot tests) |
| `GraphJSTEmitter.cs` | Emits JST nodes for static graph descriptor (for runtime-correct minified JS) |

### Modified Files

| File | Change |
|------|--------|
| `Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs` | Add second constructor accepting `GraphDescriptor`, delegate lifecycle to `IBindingStrategy` |
| `Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj` | Include new `BindingGraph/*.cs` files (auto-included by glob) |
| `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs` | Switch from SkinBinderInfo emission to graph descriptor emission |
| `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs` | Pass graph descriptor to SkinInstance factory |

### New Test Files

| File | Purpose |
|------|---------|
| `Test/Compiler/RazorSkinParser.Test/GraphTopologyBuilderTests.cs` | Unit tests for IR → graph topology conversion |
| `Test/Compiler/RazorSkinParser.Test/GraphDescriptorEmitterTests.cs` | Snapshot tests for graph descriptor JS output |
| `Test/Compiler/RazorSkinParser.Test/Templates/GraphTextBinding.skin.cshtml` | Graph-mode template for snapshot tests |
| `Test/Compiler/RazorSkinParser.Test/ExpectedOutput/GraphTextBinding.js` | Expected graph descriptor JS |

---

## Phase 1: Graph Engine Core (Runtime)

### Task 1: GraphNodeType Constants

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphNodeType.cs`

- [ ] **Step 1: Create the node type class**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    /// <summary>
    /// Node type constants for the reactive binding graph.
    /// Values are used as array indices in the static descriptor's nodeTypes array.
    /// </summary>
    public static class GraphNodeType
    {
        public const int Source = 0;
        public const int Property = 1;
        public const int Computed = 2;
        public const int DomTarget = 3;
        public const int EventBinding = 4;
        public const int Gate = 5;
        public const int CollectionManager = 6;
        public const int TypeGuard = 7;
    }

    /// <summary>
    /// Subscription mode for the graph.
    /// </summary>
    public static class GraphSubscribeMode
    {
        public const int PerProperty = 0;
        public const int AllProperties = 1;
    }

    /// <summary>
    /// Source slot indices.
    /// </summary>
    public static class GraphSourceSlot
    {
        public const int DataContext = 0;
        public const int TemplateParent = 1;
    }
}
```

- [ ] **Step 2: Verify the file compiles**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphNodeType.cs
git commit -m "feat(binding-graph): add GraphNodeType constants"
```

---

### Task 2: GraphDescriptor — Static Graph Shape

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphDescriptor.cs`

This is the shared, immutable structure created at compile time and reused across all instances of a template.

- [ ] **Step 1: Create the descriptor class**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;

    /// <summary>
    /// Immutable static graph shape shared across all instances of a template.
    /// Created by compiled JS factory code. All arrays have length == nodeCount.
    /// </summary>
    public class GraphDescriptor
    {
        /// <summary>Number of nodes in the graph.</summary>
        public int NodeCount;

        /// <summary>Node type per index (GraphNodeType constants).</summary>
        public NativeArray<int> NodeTypes;

        /// <summary>
        /// Getter function per node. For Property/Computed/Gate nodes: (source) => value.
        /// For Source/DomTarget/EventBinding: null.
        /// </summary>
        public NativeArray<Func<object, object>> Getters;

        /// <summary>
        /// Adjacency list: Consumers[i] is an int[] of node indices that depend on node i.
        /// </summary>
        public NativeArray<NativeArray<int>> Consumers;

        /// <summary>
        /// Gate index for each node. -1 if ungated. If >= 0, the node is only active
        /// when gateOpen[gateIndices[i]] is true.
        /// </summary>
        public NativeArray<int> GateIndices;

        /// <summary>
        /// Default value per node, applied when gate closes or source is null.
        /// Typically "" for text, null for objects.
        /// </summary>
        public NativeArray DefaultValues;

        /// <summary>
        /// Target info per DomTarget/Gate/CollectionManager/EventBinding node.
        /// Null for non-target nodes. Contents vary by node type:
        /// - DomTarget: { elemIdx: int, setter: Action }
        /// - Gate: { markerIdx: int, trueTmpl: Element, falseTmpl: Element, elemCount: int }
        /// - CollectionManager: { markerIdx: int, itemGraph: GraphDescriptor }
        /// - EventBinding: { elemIdx: int, eventName: string }
        /// </summary>
        public NativeArray TargetInfos;

        /// <summary>
        /// Subscription map: array of { propertyName: string, nodeIdx: int, sourceSlot: int }.
        /// Used during Phase 2 (deferred subscription wiring) to register PropertyChanged listeners.
        /// </summary>
        public NativeArray Subscriptions;

        /// <summary>
        /// Source type for type-checking at DataContext boundary.
        /// </summary>
        public Type SourceType;

        /// <summary>
        /// Subscription mode: GraphSubscribeMode.PerProperty or GraphSubscribeMode.AllProperties.
        /// </summary>
        public int SubscribeMode;

        /// <summary>
        /// Source slot that this graph's root Source node reads from.
        /// 0 = DataContext, 1 = TemplateParent.
        /// </summary>
        public int RootSourceSlot;
    }

    /// <summary>
    /// Target info for DomTarget nodes.
    /// </summary>
    public class DomTargetInfo
    {
        public int ElemIdx;
        public Action<object, object> Setter;
    }

    /// <summary>
    /// Subscription entry: maps a property name to its Property node index.
    /// </summary>
    public class SubscriptionEntry
    {
        public string PropertyName;
        public int NodeIdx;
        public int SourceSlot;
    }

    /// <summary>
    /// Target info for Gate nodes (@if/@else).
    /// </summary>
    public class GateTargetInfo
    {
        public int MarkerIdx;
        public object TrueTemplate;
        public object FalseTemplate;
        public int TrueElemCount;
        public int FalseElemCount;
    }

    /// <summary>
    /// Target info for CollectionManager nodes (@foreach).
    /// </summary>
    public class CollectionTargetInfo
    {
        public int MarkerIdx;
        public GraphDescriptor ItemGraph;
        public object ItemTemplate;
    }

    /// <summary>
    /// Target info for EventBinding nodes.
    /// </summary>
    public class EventTargetInfo
    {
        public int ElemIdx;
        public string EventName;
    }
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphDescriptor.cs
git commit -m "feat(binding-graph): add GraphDescriptor static shape classes"
```

---

### Task 3: GraphState — Per-Instance Dynamic State

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphState.cs`

- [ ] **Step 1: Create the state class**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Per-instance mutable state for a binding graph. Created when SkinInstance
    /// is instantiated. Disposed with the instance.
    /// </summary>
    public class GraphState
    {
        /// <summary>Cached output value per node.</summary>
        public NativeArray Values;

        /// <summary>Dirty flag per node.</summary>
        public NativeArray<bool> Dirty;

        /// <summary>Gate-open flag per node (only meaningful for Gate nodes).</summary>
        public NativeArray<bool> GateOpen;

        /// <summary>Source objects: [0]=DataContext, [1]=TemplateParent.</summary>
        public NativeArray Sources;

        /// <summary>DOM element references resolved from the cloned template.</summary>
        public NativeArray ElemRefs;

        /// <summary>Active PropertyChanged listener handles for cleanup.</summary>
        public NativeArray Listeners;

        /// <summary>Number of active listeners (for cleanup iteration).</summary>
        public int ListenerCount;

        /// <summary>Whether subscriptions have been wired (Phase 2 complete).</summary>
        public bool SubscriptionsActive;

        /// <summary>Whether a flush is scheduled for this graph.</summary>
        public bool FlushScheduled;

        /// <summary>Depth in the skin nesting hierarchy (0 = root).</summary>
        public int Depth;

        /// <summary>Back-reference to the static descriptor.</summary>
        public GraphDescriptor Descriptor;

        /// <summary>
        /// Creates a new GraphState for the given descriptor.
        /// </summary>
        public GraphState(GraphDescriptor descriptor, NativeArray elemRefs, int depth)
        {
            this.Descriptor = descriptor;
            this.ElemRefs = elemRefs;
            this.Depth = depth;

            int n = descriptor.NodeCount;
            this.Values = new NativeArray(n);
            this.Dirty = new NativeArray<bool>(n);
            this.GateOpen = new NativeArray<bool>(n);
            this.Sources = new NativeArray(2);
            this.Listeners = new NativeArray(0);
            this.ListenerCount = 0;
            this.SubscriptionsActive = false;
            this.FlushScheduled = false;

            // All gates start open (true). Gate.Evaluate during initial push
            // will close gates whose condition is false.
            for (int i = 0; i < n; i++)
            {
                this.GateOpen[i] = true;
            }
        }
    }
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphState.cs
git commit -m "feat(binding-graph): add GraphState per-instance dynamic state"
```

---

### Task 4: GraphEngine — Core Evaluation

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs`

This is the heart of the system — evaluates nodes, propagates values, handles dirty marking and flush.

- [ ] **Step 1: Create the engine class with initial value push**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Core graph evaluation engine. Stateless — all state lives in GraphState.
    /// All methods are static to allow sharing across instances.
    /// </summary>
    public static class GraphEngine
    {
        /// <summary>
        /// Phase 1: Synchronous initial value push. Walks all nodes in
        /// topological order (index 0..N), evaluates getters, writes to DOM.
        /// Called during Activate().
        /// </summary>
        public static void PushInitialValues(GraphDescriptor desc, GraphState state)
        {
            int n = desc.NodeCount;
            var nodeTypes = desc.NodeTypes;
            var getters = desc.Getters;
            var gateIndices = desc.GateIndices;
            var targetInfos = desc.TargetInfos;
            var values = state.Values;
            var gateOpen = state.GateOpen;
            var elemRefs = state.ElemRefs;

            for (int i = 0; i < n; i++)
            {
                // Skip gated nodes whose gate is closed
                int gateIdx = gateIndices[i];
                if (gateIdx >= 0 && !gateOpen[gateIdx])
                {
                    values[i] = desc.DefaultValues[i];
                    continue;
                }

                int nodeType = nodeTypes[i];
                object val = null;

                switch (nodeType)
                {
                    case GraphNodeType.Source:
                        val = state.Sources[desc.RootSourceSlot];
                        // Type check at source boundary
                        if (val != null && desc.SourceType != null
                            && !desc.SourceType.IsInstanceOfType(val))
                        {
                            val = null;
                        }
                        break;

                    case GraphNodeType.Property:
                    case GraphNodeType.Computed:
                    case GraphNodeType.TypeGuard:
                        var getter = getters[i];
                        if (getter != null)
                        {
                            // Find the input value — first parent in consumers
                            // For Property nodes, input = parent Source/Property value
                            // For Computed, getter receives the source object directly
                            var parentVal = FindParentValue(desc, state, i);
                            if (parentVal != null)
                            {
                                val = getter(parentVal);
                            }
                        }
                        break;

                    case GraphNodeType.Gate:
                        var gateGetter = getters[i];
                        if (gateGetter != null)
                        {
                            var parentValGate = FindParentValue(desc, state, i);
                            if (parentValGate != null)
                            {
                                val = gateGetter(parentValGate);
                            }
                        }
                        bool isOpen = val != null && (bool)val;
                        gateOpen[i] = isOpen;
                        if (!isOpen)
                        {
                            ApplyGateClosure(desc, state, i);
                        }
                        break;

                    case GraphNodeType.DomTarget:
                        // DomTarget receives value from its parent node
                        var inputVal = FindParentValue(desc, state, i);
                        val = inputVal;
                        var targetInfo = (DomTargetInfo)targetInfos[i];
                        if (targetInfo != null && elemRefs != null)
                        {
                            var elem = elemRefs[targetInfo.ElemIdx];
                            var displayVal = val != null ? val : desc.DefaultValues[i];
                            targetInfo.Setter(elem, displayVal);
                        }
                        break;

                    case GraphNodeType.EventBinding:
                        var methodRef = FindParentValue(desc, state, i);
                        val = methodRef;
                        // Event wiring is handled separately during activation
                        break;
                }

                values[i] = val;
            }
        }

        /// <summary>
        /// Find the parent node's cached value for a given node.
        /// The parent is the node that has this node in its consumers list.
        /// We store this as a reverse lookup: scan consumers to find who feeds node i.
        /// For efficiency, we rely on topological ordering — the parent value
        /// is already computed and cached in values[].
        ///
        /// Implementation: Property/Computed nodes get their input from the source
        /// object (for simple properties) or from the parent node's cached value.
        /// The getter function itself navigates from the source object.
        /// </summary>
        private static object FindParentValue(GraphDescriptor desc, GraphState state, int nodeIdx)
        {
            // For Property/Computed nodes, the getter takes the source object.
            // Walk up the consumers array to find the Source node ancestor
            // and use its cached value.
            // Optimization: Property node getters take the source directly,
            // so we just pass the Source node's value.
            var consumers = desc.Consumers;
            int n = desc.NodeCount;
            for (int parent = 0; parent < nodeIdx; parent++)
            {
                var parentConsumers = consumers[parent];
                if (parentConsumers != null)
                {
                    for (int c = 0; c < parentConsumers.Length; c++)
                    {
                        if (parentConsumers[c] == nodeIdx)
                        {
                            return state.Values[parent];
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Stage 2: Flush dirty nodes. Called by GraphFlushCoordinator on microtask boundary.
        /// Linear scan in topological order. Skips clean nodes and closed gates.
        /// </summary>
        public static void Flush(GraphDescriptor desc, GraphState state)
        {
            int n = desc.NodeCount;
            var nodeTypes = desc.NodeTypes;
            var getters = desc.Getters;
            var gateIndices = desc.GateIndices;
            var targetInfos = desc.TargetInfos;
            var consumers = desc.Consumers;
            var values = state.Values;
            var dirty = state.Dirty;
            var gateOpen = state.GateOpen;
            var elemRefs = state.ElemRefs;

            for (int i = 0; i < n; i++)
            {
                if (!dirty[i]) continue;

                dirty[i] = false;

                // Skip gated nodes whose gate is closed
                int gateIdx = gateIndices[i];
                if (gateIdx >= 0 && !gateOpen[gateIdx])
                {
                    continue;
                }

                int nodeType = nodeTypes[i];
                object oldVal = values[i];
                object newVal = null;

                switch (nodeType)
                {
                    case GraphNodeType.Source:
                        newVal = state.Sources[desc.RootSourceSlot];
                        if (newVal != null && desc.SourceType != null
                            && !desc.SourceType.IsInstanceOfType(newVal))
                        {
                            newVal = null;
                        }
                        break;

                    case GraphNodeType.Property:
                    case GraphNodeType.Computed:
                    case GraphNodeType.TypeGuard:
                        var getter = getters[i];
                        if (getter != null)
                        {
                            var parentVal = FindParentValue(desc, state, i);
                            if (parentVal != null)
                            {
                                newVal = getter(parentVal);
                            }
                        }
                        break;

                    case GraphNodeType.Gate:
                        var gateGetter = getters[i];
                        if (gateGetter != null)
                        {
                            var parentValGate = FindParentValue(desc, state, i);
                            if (parentValGate != null)
                            {
                                newVal = gateGetter(parentValGate);
                            }
                        }
                        bool wasOpen = gateOpen[i];
                        bool isOpen = newVal != null && (bool)newVal;
                        gateOpen[i] = isOpen;

                        if (wasOpen && !isOpen)
                        {
                            ApplyGateClosure(desc, state, i);
                        }
                        else if (!wasOpen && isOpen)
                        {
                            // Gate reopened — mark all gated consumers dirty
                            MarkGatedConsumersDirty(desc, state, i);
                        }
                        break;

                    case GraphNodeType.DomTarget:
                        newVal = FindParentValue(desc, state, i);
                        var targetInfo = (DomTargetInfo)targetInfos[i];
                        if (targetInfo != null && elemRefs != null)
                        {
                            var elem = elemRefs[targetInfo.ElemIdx];
                            var displayVal = newVal != null ? newVal : desc.DefaultValues[i];
                            targetInfo.Setter(elem, displayVal);
                        }
                        break;

                    case GraphNodeType.EventBinding:
                        newVal = FindParentValue(desc, state, i);
                        // Re-wire event if method reference changed
                        break;
                }

                values[i] = newVal;

                // Flip-flop elimination: if value unchanged, don't dirty consumers
                if (newVal == oldVal) continue;

                // Mark consumers dirty
                var nodeConsumers = consumers[i];
                if (nodeConsumers != null)
                {
                    for (int c = 0; c < nodeConsumers.Length; c++)
                    {
                        dirty[nodeConsumers[c]] = true;
                    }
                }
            }

            state.FlushScheduled = false;
        }

        /// <summary>
        /// Mark a specific property node as dirty and schedule flush.
        /// Called from PropertyChanged listener.
        /// </summary>
        public static void MarkDirty(GraphState state, int nodeIdx)
        {
            state.Dirty[nodeIdx] = true;
            if (!state.FlushScheduled)
            {
                state.FlushScheduled = true;
                GraphFlushCoordinator.ScheduleDirty(state);
            }
        }

        /// <summary>
        /// When a gate closes, set all gated nodes to their default values
        /// and clear dirty flags.
        /// </summary>
        private static void ApplyGateClosure(GraphDescriptor desc, GraphState state, int gateIdx)
        {
            int n = desc.NodeCount;
            var gateIndices = desc.GateIndices;
            var defaultValues = desc.DefaultValues;
            var values = state.Values;
            var dirty = state.Dirty;
            var targetInfos = desc.TargetInfos;
            var elemRefs = state.ElemRefs;
            var nodeTypes = desc.NodeTypes;

            for (int i = gateIdx + 1; i < n; i++)
            {
                if (gateIndices[i] != gateIdx) continue;

                values[i] = defaultValues[i];
                dirty[i] = false;

                // Apply default to DOM targets
                if (nodeTypes[i] == GraphNodeType.DomTarget)
                {
                    var targetInfo = (DomTargetInfo)targetInfos[i];
                    if (targetInfo != null && elemRefs != null)
                    {
                        var elem = elemRefs[targetInfo.ElemIdx];
                        targetInfo.Setter(elem, defaultValues[i]);
                    }
                }
            }
        }

        /// <summary>
        /// When a gate reopens, mark all gated consumers as dirty
        /// so they get re-evaluated in the current flush.
        /// </summary>
        private static void MarkGatedConsumersDirty(GraphDescriptor desc, GraphState state, int gateIdx)
        {
            int n = desc.NodeCount;
            var gateIndices = desc.GateIndices;

            for (int i = gateIdx + 1; i < n; i++)
            {
                if (gateIndices[i] == gateIdx)
                {
                    state.Dirty[i] = true;
                }
            }
        }
    }
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs
git commit -m "feat(binding-graph): add GraphEngine core evaluation"
```

---

### Task 5: GraphFlushCoordinator — Microtask Batching

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphFlushCoordinator.cs`

- [ ] **Step 1: Create the coordinator**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;

    /// <summary>
    /// Global flush coordinator. Collects dirty graphs and flushes them
    /// in depth order on a single microtask boundary.
    /// </summary>
    public static class GraphFlushCoordinator
    {
        /// <summary>
        /// Pending dirty graphs organized by depth.
        /// Index = depth, value = array of GraphState at that depth.
        /// </summary>
        private static NativeArray<NativeArray<GraphState>> pendingByDepth;

        /// <summary>Maximum depth seen so far.</summary>
        private static int maxDepth;

        /// <summary>Whether a flush microtask is already scheduled.</summary>
        private static bool flushScheduled;

        /// <summary>Total number of pending graphs.</summary>
        private static int pendingCount;

        /// <summary>
        /// Initialize the coordinator. Called once at application startup.
        /// </summary>
        static GraphFlushCoordinator()
        {
            pendingByDepth = new NativeArray<NativeArray<GraphState>>(8);
            for (int i = 0; i < 8; i++)
            {
                pendingByDepth[i] = new NativeArray<GraphState>(0);
            }
            maxDepth = 0;
            flushScheduled = false;
            pendingCount = 0;
        }

        /// <summary>
        /// Register a dirty graph for the next flush cycle.
        /// Called from GraphEngine.MarkDirty().
        /// </summary>
        public static void ScheduleDirty(GraphState state)
        {
            int depth = state.Depth;

            // Grow depth buckets if needed
            if (depth >= pendingByDepth.Length)
            {
                var newBuckets = new NativeArray<NativeArray<GraphState>>(depth + 4);
                for (int i = 0; i < pendingByDepth.Length; i++)
                {
                    newBuckets[i] = pendingByDepth[i];
                }
                for (int i = pendingByDepth.Length; i < newBuckets.Length; i++)
                {
                    newBuckets[i] = new NativeArray<GraphState>(0);
                }
                pendingByDepth = newBuckets;
            }

            // Add to depth bucket
            var bucket = pendingByDepth[depth];
            var newBucket = new NativeArray<GraphState>(bucket.Length + 1);
            for (int i = 0; i < bucket.Length; i++)
            {
                newBucket[i] = bucket[i];
            }
            newBucket[bucket.Length] = state;
            pendingByDepth[depth] = newBucket;

            if (depth > maxDepth) maxDepth = depth;
            pendingCount++;

            if (!flushScheduled)
            {
                flushScheduled = true;
                TaskScheduler.Instance.EnqueHighPriTask(FlushAll, "GraphFlushCoordinator.FlushAll");
            }
        }

        /// <summary>
        /// Flush all pending dirty graphs in depth order.
        /// Runs on microtask boundary via TaskScheduler.
        /// </summary>
        private static void FlushAll()
        {
            for (int depth = 0; depth <= maxDepth; depth++)
            {
                var bucket = pendingByDepth[depth];
                for (int i = 0; i < bucket.Length; i++)
                {
                    var state = bucket[i];
                    if (state != null)
                    {
                        GraphEngine.Flush(state.Descriptor, state);
                    }
                }
                // Clear bucket
                pendingByDepth[depth] = new NativeArray<GraphState>(0);
            }

            pendingCount = 0;
            flushScheduled = false;
        }

        /// <summary>
        /// Reset coordinator state. Useful for tests.
        /// </summary>
        public static void Reset()
        {
            for (int i = 0; i < pendingByDepth.Length; i++)
            {
                pendingByDepth[i] = new NativeArray<GraphState>(0);
            }
            maxDepth = 0;
            pendingCount = 0;
            flushScheduled = false;
        }
    }
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphFlushCoordinator.cs
git commit -m "feat(binding-graph): add GraphFlushCoordinator microtask batching"
```

---

## Phase 2: SkinInstance Integration

### Task 6: IBindingStrategy Interface

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/IBindingStrategy.cs`

- [ ] **Step 1: Create the strategy interface**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    /// <summary>
    /// Internal strategy interface for SkinInstance binding management.
    /// Selected based on whether the factory provides a GraphDescriptor or SkinBinderInfo[].
    /// </summary>
    public interface IBindingStrategy
    {
        /// <summary>
        /// Phase 1: Push initial values to all bound DOM targets synchronously.
        /// Called during SkinInstance.Activate().
        /// </summary>
        void PushInitialValues(object dataContext, object templateParent, NativeArray elementsOfInterest);

        /// <summary>
        /// Phase 2: Wire reactive subscriptions (deferred via TaskScheduler).
        /// Called from SkinInstance.QueuedActivation().
        /// </summary>
        void WireSubscriptions(object dataContext, object templateParent);

        /// <summary>
        /// Called when the DataContext changes after activation.
        /// </summary>
        void OnDataContextChanged(object newDataContext);

        /// <summary>
        /// Called when the TemplateParent changes after activation.
        /// </summary>
        void OnTemplateParentChanged(object newTemplateParent);

        /// <summary>
        /// Deactivate: unsubscribe listeners but keep state for potential reactivation.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Full cleanup: unsubscribe, release DOM references, destroy child graphs.
        /// </summary>
        void Dispose();
    }
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/IBindingStrategy.cs
git commit -m "feat(binding-graph): add IBindingStrategy interface"
```

---

### Task 7: GraphBindingStrategy

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphBindingStrategy.cs`

- [ ] **Step 1: Create the graph strategy**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Graph-based binding strategy. Wraps GraphEngine + GraphState
    /// and implements IBindingStrategy for SkinInstance delegation.
    /// </summary>
    public class GraphBindingStrategy : IBindingStrategy
    {
        private GraphDescriptor descriptor;
        private GraphState state;

        public GraphBindingStrategy(GraphDescriptor descriptor, NativeArray elemRefs, int depth)
        {
            this.descriptor = descriptor;
            this.state = new GraphState(descriptor, elemRefs, depth);
        }

        public void PushInitialValues(object dataContext, object templateParent, NativeArray elementsOfInterest)
        {
            this.state.Sources[GraphSourceSlot.DataContext] = dataContext;
            this.state.Sources[GraphSourceSlot.TemplateParent] = templateParent;
            GraphEngine.PushInitialValues(this.descriptor, this.state);
        }

        public void WireSubscriptions(object dataContext, object templateParent)
        {
            if (this.state.SubscriptionsActive) return;

            // Update sources to current values (may have changed since Activate)
            this.state.Sources[GraphSourceSlot.DataContext] = dataContext;
            this.state.Sources[GraphSourceSlot.TemplateParent] = templateParent;

            var subscriptions = this.descriptor.Subscriptions;
            if (subscriptions == null) return;

            int subCount = subscriptions.Length;
            var listeners = new NativeArray(subCount);
            int listenerIdx = 0;

            for (int i = 0; i < subCount; i++)
            {
                var entry = (SubscriptionEntry)subscriptions[i];
                var source = this.state.Sources[entry.SourceSlot];
                if (source == null) continue;

                var observable = source as INotifyPropertyChanged;
                if (observable == null) continue;

                int capturedNodeIdx = entry.NodeIdx;
                GraphState capturedState = this.state;
                Action<INotifyPropertyChanged, string> callback =
                    delegate(INotifyPropertyChanged sender, string propName)
                    {
                        GraphEngine.MarkDirty(capturedState, capturedNodeIdx);
                    };

                observable.AddPropertyChangedListener(entry.PropertyName, callback);
                listeners[listenerIdx] = callback;
                listenerIdx++;
            }

            this.state.Listeners = listeners;
            this.state.ListenerCount = listenerIdx;
            this.state.SubscriptionsActive = true;
        }

        public void OnDataContextChanged(object newDataContext)
        {
            object oldDc = this.state.Sources[GraphSourceSlot.DataContext];
            this.state.Sources[GraphSourceSlot.DataContext] = newDataContext;

            if (this.state.SubscriptionsActive)
            {
                // Unsubscribe from old, subscribe to new
                UnsubscribeAll();
                this.state.SubscriptionsActive = false;
                WireSubscriptions(newDataContext, this.state.Sources[GraphSourceSlot.TemplateParent]);
            }

            // Mark source node dirty and flush
            this.state.Dirty[0] = true;
            GraphEngine.Flush(this.descriptor, this.state);
        }

        public void OnTemplateParentChanged(object newTemplateParent)
        {
            this.state.Sources[GraphSourceSlot.TemplateParent] = newTemplateParent;
            // Mark source node dirty and flush
            this.state.Dirty[0] = true;
            GraphEngine.Flush(this.descriptor, this.state);
        }

        public void Deactivate()
        {
            UnsubscribeAll();
            this.state.SubscriptionsActive = false;
        }

        public void Dispose()
        {
            UnsubscribeAll();
            this.state.SubscriptionsActive = false;

            // Clear all cached values
            int n = this.descriptor.NodeCount;
            for (int i = 0; i < n; i++)
            {
                this.state.Values[i] = null;
                this.state.Dirty[i] = false;
            }

            this.state.ElemRefs = null;
            this.state.Sources[0] = null;
            this.state.Sources[1] = null;
        }

        private void UnsubscribeAll()
        {
            var subscriptions = this.descriptor.Subscriptions;
            if (subscriptions == null) return;

            int subCount = subscriptions.Length;
            for (int i = 0; i < subCount; i++)
            {
                var entry = (SubscriptionEntry)subscriptions[i];
                var source = this.state.Sources[entry.SourceSlot];
                if (source == null) continue;

                var observable = source as INotifyPropertyChanged;
                if (observable == null) continue;

                if (i < this.state.ListenerCount && this.state.Listeners[i] != null)
                {
                    observable.RemovePropertyChangedListener(
                        entry.PropertyName,
                        (Action<INotifyPropertyChanged, string>)this.state.Listeners[i]);
                }
            }
        }
    }
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphBindingStrategy.cs
git commit -m "feat(binding-graph): add GraphBindingStrategy implementation"
```

---

### Task 8: LegacyBinderStrategy — Extract Existing Logic

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/LegacyBinderStrategy.cs`
- Modify: `Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs`

This task extracts the existing binder logic from SkinInstance into a strategy class, then modifies SkinInstance to delegate. This is the most delicate task — it must preserve exact behavior.

- [ ] **Step 1: Create LegacyBinderStrategy**

```csharp
namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using Sunlight.Framework.Binders;

    /// <summary>
    /// Legacy binding strategy that wraps the existing SkinBinderInfo[]/LiveBinder[] logic.
    /// Extracted from SkinInstance to maintain backward compatibility with XWML templates.
    /// </summary>
    public class LegacyBinderStrategy : IBindingStrategy
    {
        private NativeArray<SkinBinderInfo> binders;
        private NativeArray<LiveBinder> liveBinders;
        private NativeArray extraObjects;
        private NativeArray elementsOfInterest;
        private NativeArray<bool> hasDataContextBinding;
        private UISkinableElement skinableParent;

        public NativeArray<bool> HasDataContextBinding
        {
            get { return this.hasDataContextBinding; }
        }

        public LegacyBinderStrategy(
            NativeArray<SkinBinderInfo> binders,
            int liveBinderCount,
            int extraObjectCount,
            int elementCount)
        {
            this.binders = binders;
            this.hasDataContextBinding = new NativeArray<bool>(elementCount);

            if (liveBinderCount > 0)
            {
                this.liveBinders = new NativeArray<LiveBinder>(liveBinderCount);
            }

            if (extraObjectCount > 0)
            {
                this.extraObjects = new NativeArray(extraObjectCount);
            }
        }

        public void SetSkinableParent(UISkinableElement parent)
        {
            this.skinableParent = parent;
        }

        public void PushInitialValues(object dataContext, object templateParent, NativeArray elementsOfInterest)
        {
            this.elementsOfInterest = elementsOfInterest;
            var binders = this.binders;
            var binderLength = binders.Length;
            Action<UIElement, object> dataContextSetter = SkinBinderHelper.SetDataContext;

            for (int iBinder = 0, iLiveBinder = 0; iBinder < binderLength; iBinder++)
            {
                var binder = binders[iBinder];
                object source = null;
                switch (binder.BinderType & BinderType.TargetTypes)
                {
                    case BinderType.DataContext:
                        source = dataContext;
                        break;
                    case BinderType.Static:
                        break;
                    case BinderType.TemplateParent:
                        source = templateParent;
                        break;
                }

                if (binder.Mode == DataBindingMode.TwoWay)
                {
                    LiveBinder liveBinder = this.liveBinders[iLiveBinder];
                    if (object.IsNullOrUndefined(liveBinder))
                    {
                        liveBinder = new LiveBinder(binder, this.extraObjects);
                        liveBinder.Source = source;
                        liveBinder.Target = elementsOfInterest[binder.ObjectIndex];
                        liveBinder.IsActive = true;
                        this.liveBinders[iLiveBinder] = liveBinder;
                    }
                    else
                    {
                        liveBinder.Source = source;
                        liveBinder.IsActive = true;
                    }
                }
                else
                {
                    SkinBinderHelper.SetPropertyValue(
                        binder, source, elementsOfInterest[binder.ObjectIndex], this.extraObjects);

                    if ((object)binder.TargetPropertySetter == (object)dataContextSetter)
                    {
                        this.hasDataContextBinding[binder.ObjectIndex] = true;
                    }
                }

                if (binder.Mode != DataBindingMode.OneTime)
                {
                    ++iLiveBinder;
                }
            }
        }

        public void WireSubscriptions(object dataContext, object templateParent)
        {
            var binders = this.binders;
            var liveBinders = this.liveBinders;
            if (object.IsNullOrUndefined(liveBinders)) return;

            var binderLength = binders.Length;
            var liveBindersLength = liveBinders.Length;
            for (int iBinderInfo = 0, iLivebinder = 0;
                iBinderInfo < binderLength && iLivebinder < liveBindersLength;
                iBinderInfo++)
            {
                var binder = binders[iBinderInfo];
                if (binder.Mode != DataBindingMode.OneTime)
                {
                    LiveBinder liveBinder = liveBinders[iLivebinder];
                    if (object.IsNullOrUndefined(liveBinder))
                    {
                        liveBinders[iLivebinder] = liveBinder = new LiveBinder(binder, this.extraObjects);
                        liveBinder.Target = this.elementsOfInterest[binder.ObjectIndex];
                    }

                    switch (binder.BinderType & BinderType.TargetTypes)
                    {
                        case BinderType.DataContext:
                            liveBinder.Source = dataContext;
                            break;
                        case BinderType.TemplateParent:
                            liveBinder.Source = templateParent;
                            break;
                    }

                    liveBinder.IsActive = true;
                    ++iLivebinder;
                }
            }
        }

        public void OnDataContextChanged(object newDataContext)
        {
            UpdateBinderSource(newDataContext, BinderType.DataContext);
        }

        public void OnTemplateParentChanged(object newTemplateParent)
        {
            UpdateBinderSource(newTemplateParent, BinderType.TemplateParent);
        }

        public void Deactivate()
        {
            var liveBinders = this.liveBinders;
            if (object.IsNullOrUndefined(liveBinders)) return;

            for (int i = 0; i < liveBinders.Length; i++)
            {
                if (object.IsNullOrUndefined(liveBinders[i])) continue;
                liveBinders[i].IsActive = false;
            }
        }

        public void Dispose()
        {
            var liveBinders = this.liveBinders;
            if (object.IsNullOrUndefined(liveBinders)) return;

            for (int i = 0; i < liveBinders.Length; i++)
            {
                var liveBinder = liveBinders[i];
                if (object.IsNullOrUndefined(liveBinder)) continue;

                liveBinder.IsActive = false;
                liveBinder.Source = null;
                liveBinder.Target = null;
                liveBinder.Cleanup();
                this.liveBinders[i] = null;
            }
        }

        public void QueuedDeactivation()
        {
            if (object.IsNullOrUndefined(this.liveBinders)) return;

            for (int i = 0; i < this.liveBinders.Length; i++)
            {
                var liveBinder = this.liveBinders[i];
                if (object.IsNullOrUndefined(liveBinder)) return;

                liveBinder.IsActive = false;
                liveBinder.Cleanup();
            }
        }

        private void UpdateBinderSource(object source, BinderType sourceType)
        {
            var liveBinders = this.liveBinders;
            var binders = this.binders;
            var bindersLength = binders.Length;
            var liveBindersLength = object.IsNullOrUndefined(liveBinders) ? 0 : liveBinders.Length;

            for (int iBinder = 0, iLiveBinder = 0; iBinder < bindersLength; iBinder++)
            {
                var binder = binders[iBinder];
                if (binder.Mode != DataBindingMode.OneTime
                    && iLiveBinder < liveBindersLength
                    && !object.IsNullOrUndefined(liveBinders[iLiveBinder]))
                {
                    if (sourceType == (binder.BinderType & BinderType.TargetTypes))
                    {
                        liveBinders[iLiveBinder].Source = source;
                    }
                    ++iLiveBinder;
                }
                else if (binder.Mode == DataBindingMode.OneTime)
                {
                    if (sourceType == (binder.BinderType & BinderType.TargetTypes))
                    {
                        SkinBinderHelper.SetPropertyValue(
                            binder, source,
                            this.elementsOfInterest[binder.ObjectIndex],
                            this.extraObjects);
                    }
                }
            }
        }
    }
}
```

- [ ] **Step 2: Modify SkinInstance to use strategy pattern**

Modify `Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs`:

Add a new constructor overload that accepts `GraphDescriptor` and create an `IBindingStrategy` field. Refactor `Activate()`, `Deactivate()`, `Dispose()`, `UpdateDataContext()`, `QueuedActivation()`, and `QueuedDeactivation()` to delegate to the strategy.

The existing constructor creates a `LegacyBinderStrategy`. The new constructor creates a `GraphBindingStrategy`.

Key changes:
- Add field: `private IBindingStrategy bindingStrategy;`
- Existing constructor: creates `LegacyBinderStrategy`, assigns to `bindingStrategy`
- New constructor (for graph): creates `GraphBindingStrategy`, assigns to `bindingStrategy`
- `Activate()`: calls `bindingStrategy.PushInitialValues(...)` instead of inline binder loop
- `QueuedActivation()`: calls `bindingStrategy.WireSubscriptions(...)`
- `Deactivate()`: calls `bindingStrategy.Deactivate()`
- `Dispose()`: calls `bindingStrategy.Dispose()`
- `UpdateDataContext()`: calls `bindingStrategy.OnDataContextChanged(...)` when active
- `UpdateBinderSource()`: calls `bindingStrategy.OnDataContextChanged/OnTemplateParentChanged`

**CRITICAL**: The `childElements` loop in `Activate()` (lines 313-323) must remain in SkinInstance — it handles UIElement children and is orthogonal to the binding strategy. The `hasDataContextBinding` check stays in SkinInstance, read from `LegacyBinderStrategy.HasDataContextBinding` or always-false for graph mode (graph sets DataContext via nodes, not child propagation).

- [ ] **Step 3: Verify build and ensure no regressions**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 4: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/LegacyBinderStrategy.cs
git add Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs
git commit -m "feat(binding-graph): extract LegacyBinderStrategy, add strategy pattern to SkinInstance"
```

---

## Phase 3: Compiler — Graph Topology Builder

### Task 9: GraphTopologyBuilder — IR to Graph Nodes

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/GraphTopologyBuilderTests.cs`

This class walks the IR tree and builds the graph's node arrays, adjacency list, and metadata.

- [ ] **Step 1: Write failing tests for simple text binding topology**

```csharp
// Test/Compiler/RazorSkinParser.Test/GraphTopologyBuilderTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;

namespace NScript.RazorSkin.Test
{
    [TestClass]
    public class GraphTopologyBuilderTests
    {
        [TestMethod]
        public void SimpleTextBinding_ProducesSourcePropertyDomTarget()
        {
            // Arrange: IR for @Model.Name as text content
            var ir = new SkinTemplateNode
            {
                TemplateName = "Test",
                ModelTypeName = "TestVM"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>" });
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                Classification = new BindingClassification
                {
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    CSharpExpression = "Model.Name",
                    Dependencies = new List<ObservableDependency>
                    {
                        new ObservableDependency
                        {
                            SourceKind = BindingSourceKind.DataContext,
                            PropertyName = "Name",
                            PropertyChain = "Name"
                        }
                    }
                }
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "</div>" });

            // Act
            var topology = GraphTopologyBuilder.Build(ir);

            // Assert: Source(0) -> Property(1) -> DomTarget(2)
            topology.NodeCount.Should().Be(3);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);

            // Consumers: Source feeds Property, Property feeds DomTarget
            topology.Consumers[0].Should().Contain(1);
            topology.Consumers[1].Should().Contain(2);

            // Subscription map
            topology.Subscriptions.Should().HaveCount(1);
            topology.Subscriptions[0].PropertyName.Should().Be("Name");
            topology.Subscriptions[0].NodeIdx.Should().Be(1);
        }

        [TestMethod]
        public void TwoBindings_ShareSourceNode()
        {
            // Arrange: IR for @Model.Name and @Model.Count
            var ir = new SkinTemplateNode
            {
                TemplateName = "Test",
                ModelTypeName = "TestVM"
            };
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                Classification = new BindingClassification
                {
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    CSharpExpression = "Model.Name",
                    Dependencies = new List<ObservableDependency>
                    {
                        new ObservableDependency { SourceKind = BindingSourceKind.DataContext, PropertyName = "Name", PropertyChain = "Name" }
                    }
                }
            });
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                Classification = new BindingClassification
                {
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    CSharpExpression = "Model.Count",
                    Dependencies = new List<ObservableDependency>
                    {
                        new ObservableDependency { SourceKind = BindingSourceKind.DataContext, PropertyName = "Count", PropertyChain = "Count" }
                    }
                }
            });

            var topology = GraphTopologyBuilder.Build(ir);

            // Should be: Source(0) -> Property_Name(1) -> DomTarget(2),
            //                      -> Property_Count(3) -> DomTarget(4)
            topology.NodeCount.Should().Be(5);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);

            // One Source node, two Property nodes
            topology.Consumers[0].Should().HaveCount(2);
        }

        [TestMethod]
        public void OneTimeBinding_HasEmptySubscription()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "Test",
                ModelTypeName = "PlainVM"
            };
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                Classification = new BindingClassification
                {
                    Mode = BindingMode.OneTime,
                    SourceKind = BindingSourceKind.DataContext,
                    CSharpExpression = "Model.AppVersion",
                    Dependencies = new List<ObservableDependency>()
                }
            });

            var topology = GraphTopologyBuilder.Build(ir);

            // OneTime: still Source -> Property -> DomTarget but no subscriptions
            topology.NodeCount.Should().Be(3);
            topology.Subscriptions.Should().HaveCount(0);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "FullyQualifiedName~GraphTopologyBuilder"`
Expected: Build error — `GraphTopologyBuilder` class doesn't exist yet

- [ ] **Step 3: Implement GraphTopologyBuilder**

```csharp
// Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs
using System.Collections.Generic;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Constants matching the runtime GraphNodeType values.
    /// Duplicated here to avoid compiler depending on runtime framework.
    /// </summary>
    public static class GraphNodeTypeConstants
    {
        public const int Source = 0;
        public const int Property = 1;
        public const int Computed = 2;
        public const int DomTarget = 3;
        public const int EventBinding = 4;
        public const int Gate = 5;
        public const int CollectionManager = 6;
        public const int TypeGuard = 7;
    }

    /// <summary>
    /// Result of building graph topology from IR.
    /// </summary>
    public class GraphTopology
    {
        public int NodeCount;
        public int[] NodeTypes;
        public string[] GetterExpressions;    // C# expressions for getter functions
        public List<int>[] Consumers;         // Adjacency list
        public int[] GateIndices;             // -1 if ungated
        public object[] DefaultValues;
        public List<SubscriptionInfo> Subscriptions;
        public List<DomTargetTopology> DomTargets;
        public List<EventTopology> Events;
        public List<GateTopology> Gates;
        public List<CollectionTopology> Collections;
        public string ModelTypeName;
        public int RootSourceSlot;            // 0 = DataContext, 1 = TemplateParent
    }

    public class SubscriptionInfo
    {
        public string PropertyName;
        public int NodeIdx;
        public int SourceSlot;
    }

    public class DomTargetTopology
    {
        public int NodeIdx;
        public int ElemIdx;
        public ExpressionTarget Target;       // TextContent, Attribute, CssClass, Style
        public string AttributeName;          // Only for Attribute target
    }

    public class EventTopology
    {
        public int NodeIdx;
        public int ElemIdx;
        public string EventName;
        public string HandlerExpression;
    }

    public class GateTopology
    {
        public int NodeIdx;
        public int MarkerIdx;
        public ConditionalNode IrNode;
    }

    public class CollectionTopology
    {
        public int NodeIdx;
        public int MarkerIdx;
        public LoopNode IrNode;
        public GraphTopology ItemTopology;
    }

    /// <summary>
    /// Walks the template IR tree and builds a graph topology:
    /// node arrays, adjacency list, subscription map, and metadata.
    /// </summary>
    public static class GraphTopologyBuilder
    {
        public static GraphTopology Build(SkinTemplateNode ir)
        {
            var topology = new GraphTopology
            {
                ModelTypeName = ir.ModelTypeName,
                RootSourceSlot = 0, // DataContext by default
                Subscriptions = new List<SubscriptionInfo>(),
                DomTargets = new List<DomTargetTopology>(),
                Events = new List<EventTopology>(),
                Gates = new List<GateTopology>(),
                Collections = new List<CollectionTopology>()
            };

            var nodeTypes = new List<int>();
            var getterExprs = new List<string>();
            var consumers = new List<List<int>>();
            var gateIndices = new List<int>();
            var defaults = new List<object>();

            // Node 0: Source (DataContext)
            int sourceIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                GraphNodeTypeConstants.Source, null, -1, null);

            // Track property nodes by name to deduplicate
            var propertyNodes = new Dictionary<string, int>();

            // Walk IR children
            int elemIdx = 0;
            WalkChildren(ir.Children, sourceIdx, -1,
                nodeTypes, getterExprs, consumers, gateIndices, defaults,
                propertyNodes, topology, ref elemIdx);

            // Build final arrays
            int n = nodeTypes.Count;
            topology.NodeCount = n;
            topology.NodeTypes = nodeTypes.ToArray();
            topology.GetterExpressions = getterExprs.ToArray();
            topology.Consumers = new List<int>[n];
            topology.GateIndices = gateIndices.ToArray();
            topology.DefaultValues = defaults.ToArray();

            for (int i = 0; i < n; i++)
            {
                topology.Consumers[i] = consumers[i];
            }

            return topology;
        }

        private static void WalkChildren(
            List<IRNode> children,
            int sourceIdx,
            int gateIdx,
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            Dictionary<string, int> propertyNodes,
            GraphTopology topology,
            ref int elemIdx)
        {
            if (children == null) return;

            foreach (var child in children)
            {
                if (child is ExpressionBindingNode binding)
                {
                    AddBinding(binding, sourceIdx, gateIdx,
                        nodeTypes, getterExprs, consumers, gateIndices, defaults,
                        propertyNodes, topology, ref elemIdx);
                }
                else if (child is ConditionalNode cond)
                {
                    AddConditional(cond, sourceIdx, gateIdx,
                        nodeTypes, getterExprs, consumers, gateIndices, defaults,
                        propertyNodes, topology, ref elemIdx);
                }
                else if (child is LoopNode loop)
                {
                    AddLoop(loop, sourceIdx, gateIdx,
                        nodeTypes, getterExprs, consumers, gateIndices, defaults,
                        propertyNodes, topology, ref elemIdx);
                }
                else if (child is EventNode evt)
                {
                    AddEvent(evt, sourceIdx, gateIdx,
                        nodeTypes, getterExprs, consumers, gateIndices, defaults,
                        propertyNodes, topology, ref elemIdx);
                }
                else if (child is HtmlNode)
                {
                    // Static HTML — no graph node needed
                }
            }
        }

        private static void AddBinding(
            ExpressionBindingNode binding,
            int sourceIdx,
            int gateIdx,
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            Dictionary<string, int> propertyNodes,
            GraphTopology topology,
            ref int elemIdx)
        {
            var deps = binding.Classification.Dependencies;
            var expr = binding.Classification.CSharpExpression;
            bool isComputed = deps != null && deps.Count > 1;

            if (isComputed)
            {
                // Computed node: ensure all dependency Property nodes exist
                var inputNodes = new List<int>();
                foreach (var dep in deps)
                {
                    int propNode = GetOrCreatePropertyNode(
                        dep.PropertyName, dep.PropertyChain, sourceIdx, gateIdx,
                        dep.SourceKind, binding.Classification.Mode,
                        nodeTypes, getterExprs, consumers, gateIndices, defaults,
                        propertyNodes, topology);
                    inputNodes.Add(propNode);
                }

                // Add Computed node
                int computedIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                    GraphNodeTypeConstants.Computed, expr, gateIdx, null);

                // Wire inputs -> computed
                foreach (int input in inputNodes)
                {
                    consumers[input].Add(computedIdx);
                }

                // Add DomTarget
                int targetIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                    GraphNodeTypeConstants.DomTarget, null, gateIdx, GetDefaultForTarget(binding.Target));
                consumers[computedIdx].Add(targetIdx);

                topology.DomTargets.Add(new DomTargetTopology
                {
                    NodeIdx = targetIdx,
                    ElemIdx = elemIdx++,
                    Target = binding.Target
                });
            }
            else
            {
                // Simple property binding or one-dep expression
                string propName = deps != null && deps.Count > 0 ? deps[0].PropertyName : null;
                string propChain = deps != null && deps.Count > 0 ? deps[0].PropertyChain : null;
                var sourceKind = deps != null && deps.Count > 0
                    ? deps[0].SourceKind
                    : binding.Classification.SourceKind;

                int propNode = GetOrCreatePropertyNode(
                    propName ?? ExtractPropertyName(expr), propChain ?? ExtractPropertyName(expr),
                    sourceIdx, gateIdx, sourceKind, binding.Classification.Mode,
                    nodeTypes, getterExprs, consumers, gateIndices, defaults,
                    propertyNodes, topology);

                // Add DomTarget
                int targetIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                    GraphNodeTypeConstants.DomTarget, null, gateIdx, GetDefaultForTarget(binding.Target));
                consumers[propNode].Add(targetIdx);

                topology.DomTargets.Add(new DomTargetTopology
                {
                    NodeIdx = targetIdx,
                    ElemIdx = elemIdx++,
                    Target = binding.Target
                });
            }
        }

        private static int GetOrCreatePropertyNode(
            string propertyName,
            string propertyChain,
            int sourceIdx,
            int gateIdx,
            BindingSourceKind sourceKind,
            BindingMode mode,
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            Dictionary<string, int> propertyNodes,
            GraphTopology topology)
        {
            string key = sourceKind.ToString() + ":" + (propertyChain ?? propertyName);
            if (propertyNodes.ContainsKey(key))
            {
                return propertyNodes[key];
            }

            string getterExpr = "Model." + propertyName;
            int propIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                GraphNodeTypeConstants.Property, getterExpr, gateIdx, null);

            consumers[sourceIdx].Add(propIdx);
            propertyNodes[key] = propIdx;

            // Add subscription for OneWay bindings
            if (mode == BindingMode.OneWay && propertyName != null)
            {
                topology.Subscriptions.Add(new SubscriptionInfo
                {
                    PropertyName = propertyName,
                    NodeIdx = propIdx,
                    SourceSlot = sourceKind == BindingSourceKind.TemplateParent ? 1 : 0
                });
            }

            return propIdx;
        }

        private static void AddConditional(
            ConditionalNode cond,
            int sourceIdx,
            int gateIdx,
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            Dictionary<string, int> propertyNodes,
            GraphTopology topology,
            ref int elemIdx)
        {
            // Create Property node for the condition
            var condDeps = cond.Condition.Dependencies;
            string condProp = condDeps != null && condDeps.Count > 0 ? condDeps[0].PropertyName : null;
            var condSourceKind = condDeps != null && condDeps.Count > 0
                ? condDeps[0].SourceKind : BindingSourceKind.DataContext;

            int condPropNode = GetOrCreatePropertyNode(
                condProp ?? "?", condProp, sourceIdx, gateIdx, condSourceKind, cond.Condition.Mode,
                nodeTypes, getterExprs, consumers, gateIndices, defaults,
                propertyNodes, topology);

            // Create Gate node
            int gateNodeIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                GraphNodeTypeConstants.Gate, cond.Condition.CSharpExpression, gateIdx, false);
            consumers[condPropNode].Add(gateNodeIdx);

            topology.Gates.Add(new GateTopology
            {
                NodeIdx = gateNodeIdx,
                MarkerIdx = topology.Gates.Count,
                IrNode = cond
            });

            // Walk true branch children under the new gate
            if (cond.TrueBranch != null)
            {
                WalkChildren(cond.TrueBranch, sourceIdx, gateNodeIdx,
                    nodeTypes, getterExprs, consumers, gateIndices, defaults,
                    propertyNodes, topology, ref elemIdx);
            }
        }

        private static void AddLoop(
            LoopNode loop,
            int sourceIdx,
            int gateIdx,
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            Dictionary<string, int> propertyNodes,
            GraphTopology topology,
            ref int elemIdx)
        {
            // Create Property node for the collection
            string collProp = ExtractPropertyName(loop.CollectionExpression);
            int collPropNode = GetOrCreatePropertyNode(
                collProp, collProp, sourceIdx, gateIdx,
                loop.CollectionSourceKind,
                loop.IsObservableCollection ? BindingMode.OneWay : BindingMode.OneTime,
                nodeTypes, getterExprs, consumers, gateIndices, defaults,
                propertyNodes, topology);

            // Create CollectionManager node
            int collMgrIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                GraphNodeTypeConstants.CollectionManager, null, gateIdx, null);
            consumers[collPropNode].Add(collMgrIdx);

            // Build child item graph topology
            GraphTopology itemTopology = null;
            if (loop.ItemTemplate != null && loop.ItemTemplate.Count > 0)
            {
                var itemIr = new SkinTemplateNode
                {
                    TemplateName = "ItemTemplate",
                    ModelTypeName = "ItemType"
                };
                itemIr.Children.AddRange(loop.ItemTemplate);
                itemTopology = Build(itemIr);
            }

            topology.Collections.Add(new CollectionTopology
            {
                NodeIdx = collMgrIdx,
                MarkerIdx = topology.Collections.Count,
                IrNode = loop,
                ItemTopology = itemTopology
            });
        }

        private static void AddEvent(
            EventNode evt,
            int sourceIdx,
            int gateIdx,
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            Dictionary<string, int> propertyNodes,
            GraphTopology topology,
            ref int elemIdx)
        {
            // Create Property node for method ref
            int methodNode = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                GraphNodeTypeConstants.Property, evt.HandlerExpression, gateIdx, null);
            consumers[sourceIdx].Add(methodNode);

            // Create EventBinding node
            int eventIdx = AddNode(nodeTypes, getterExprs, consumers, gateIndices, defaults,
                GraphNodeTypeConstants.EventBinding, null, gateIdx, null);
            consumers[methodNode].Add(eventIdx);

            topology.Events.Add(new EventTopology
            {
                NodeIdx = eventIdx,
                ElemIdx = elemIdx++,
                EventName = evt.DomEventName,
                HandlerExpression = evt.HandlerExpression
            });
        }

        private static int AddNode(
            List<int> nodeTypes,
            List<string> getterExprs,
            List<List<int>> consumers,
            List<int> gateIndices,
            List<object> defaults,
            int nodeType,
            string getterExpr,
            int gateIdx,
            object defaultValue)
        {
            int idx = nodeTypes.Count;
            nodeTypes.Add(nodeType);
            getterExprs.Add(getterExpr);
            consumers.Add(new List<int>());
            gateIndices.Add(gateIdx);
            defaults.Add(defaultValue);
            return idx;
        }

        private static string ExtractPropertyName(string expression)
        {
            if (expression == null) return null;
            // "Model.Name" → "Name", "Control.CssClass" → "CssClass"
            int dotIdx = expression.LastIndexOf('.');
            if (dotIdx >= 0 && dotIdx < expression.Length - 1)
            {
                return expression.Substring(dotIdx + 1);
            }
            return expression;
        }

        private static object GetDefaultForTarget(ExpressionTarget target)
        {
            switch (target)
            {
                case ExpressionTarget.TextContent: return "";
                case ExpressionTarget.Attribute: return "";
                case ExpressionTarget.CssClass: return "";
                case ExpressionTarget.Style: return "";
                default: return null;
            }
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "FullyQualifiedName~GraphTopologyBuilder"`
Expected: All 3 tests pass

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs
git add Test/Compiler/RazorSkinParser.Test/GraphTopologyBuilderTests.cs
git commit -m "feat(binding-graph): add GraphTopologyBuilder IR-to-graph conversion"
```

---

## Phase 4: Compiler — Graph Descriptor Emission

### Task 10: GraphDescriptorEmitter — Text-Based JS Output

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorEmitter.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/GraphDescriptorEmitterTests.cs`

This emitter produces human-readable JS for snapshot tests (not JST — that's Task 11).

- [ ] **Step 1: Write failing snapshot test**

Create `Test/Compiler/RazorSkinParser.Test/Templates/GraphTextBinding.skin.cshtml`:
```razor
@model Sunlight.Framework.UI.Test.TestViewModelA

<div data-test="1"><span>@Model.PropStr1</span></div>
```

Create test:
```csharp
[TestClass]
public class GraphDescriptorEmitterTests
{
    [TestMethod]
    public void SimpleTextBinding_EmitsGraphDescriptor()
    {
        var ir = CreateSimpleTextBindingIR();
        var topology = GraphTopologyBuilder.Build(ir);

        var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

        js.Should().Contain("nodeTypes:");
        js.Should().Contain(GraphNodeTypeConstants.Source.ToString());
        js.Should().Contain(GraphNodeTypeConstants.Property.ToString());
        js.Should().Contain(GraphNodeTypeConstants.DomTarget.ToString());
        js.Should().Contain("consumers:");
        js.Should().Contain("subscriptions:");
        js.Should().Contain("\"PropStr1\"");
        js.Should().Contain("nodeCount: 3");
    }

    private SkinTemplateNode CreateSimpleTextBindingIR()
    {
        var ir = new SkinTemplateNode { TemplateName = "TestBinding", ModelTypeName = "TestVM" };
        ir.Children.Add(new ExpressionBindingNode
        {
            Target = ExpressionTarget.TextContent,
            Classification = new BindingClassification
            {
                Mode = BindingMode.OneWay,
                SourceKind = BindingSourceKind.DataContext,
                CSharpExpression = "Model.PropStr1",
                Dependencies = new List<ObservableDependency>
                {
                    new ObservableDependency { SourceKind = BindingSourceKind.DataContext, PropertyName = "PropStr1", PropertyChain = "PropStr1" }
                }
            }
        });
        return ir;
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "FullyQualifiedName~GraphDescriptorEmitter"`
Expected: Fails — class doesn't exist

- [ ] **Step 3: Implement GraphDescriptorEmitter**

```csharp
// Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorEmitter.cs
using System.Collections.Generic;
using System.Text;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Emits text-based JavaScript for the static graph descriptor.
    /// Used for snapshot tests and debugging. The JST-based emitter
    /// (GraphJSTEmitter) is used for production runtime output.
    /// </summary>
    public static class GraphDescriptorEmitter
    {
        public static string EmitDescriptor(
            string templateName,
            GraphTopology topology,
            ISet<string> knownFunctionNames)
        {
            var sb = new StringBuilder();
            int n = topology.NodeCount;

            sb.AppendLine($"var {templateName}_graph = {{");

            // nodeTypes
            sb.Append("  nodeTypes: [");
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(topology.NodeTypes[i]);
            }
            sb.AppendLine("],");

            // getters
            sb.Append("  getters: [");
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(", ");
                var expr = topology.GetterExpressions[i];
                if (expr != null)
                {
                    var jsGetter = ExpressionJsEmitter.ToJsGetter(expr, "dc", "tp", knownFunctionNames);
                    sb.Append($"function(dc) {{ return {jsGetter}; }}");
                }
                else
                {
                    sb.Append("null");
                }
            }
            sb.AppendLine("],");

            // consumers (adjacency list)
            sb.Append("  consumers: [");
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append("[");
                var cons = topology.Consumers[i];
                for (int c = 0; c < cons.Count; c++)
                {
                    if (c > 0) sb.Append(", ");
                    sb.Append(cons[c]);
                }
                sb.Append("]");
            }
            sb.AppendLine("],");

            // gateIndices
            sb.Append("  gateIndices: [");
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(topology.GateIndices[i]);
            }
            sb.AppendLine("],");

            // defaultValues
            sb.Append("  defaultValues: [");
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(", ");
                var dv = topology.DefaultValues[i];
                if (dv is string s) sb.Append($"\"{EscapeJs(s)}\"");
                else if (dv is bool b) sb.Append(b ? "true" : "false");
                else sb.Append("null");
            }
            sb.AppendLine("],");

            // targetInfos
            sb.Append("  targetInfos: [");
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(", ");
                var dt = topology.DomTargets.Find(d => d.NodeIdx == i);
                if (dt != null)
                {
                    string setter = dt.Target switch
                    {
                        TemplateIR.ExpressionTarget.TextContent => "SetTextContent",
                        TemplateIR.ExpressionTarget.Attribute => "SetAttribute",
                        TemplateIR.ExpressionTarget.CssClass => "SetClassName",
                        TemplateIR.ExpressionTarget.Style => "SetStyle",
                        _ => "SetTextContent"
                    };
                    sb.Append($"{{elem: {dt.ElemIdx}, set: {setter}}}");
                }
                else
                {
                    sb.Append("null");
                }
            }
            sb.AppendLine("],");

            // subscriptions
            sb.Append("  subscriptions: [");
            for (int i = 0; i < topology.Subscriptions.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                var sub = topology.Subscriptions[i];
                sb.Append($"[\"{sub.PropertyName}\", {sub.NodeIdx}]");
            }
            sb.AppendLine("],");

            // sourceType
            sb.AppendLine($"  sourceType: \"{topology.ModelTypeName}\",");

            // subscribeMode
            sb.AppendLine("  subscribeMode: 0,");

            // nodeCount
            sb.AppendLine($"  nodeCount: {n}");

            sb.Append("};");

            return sb.ToString();
        }

        private static string EscapeJs(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "FullyQualifiedName~GraphDescriptorEmitter"`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorEmitter.cs
git add Test/Compiler/RazorSkinParser.Test/GraphDescriptorEmitterTests.cs
git commit -m "feat(binding-graph): add GraphDescriptorEmitter text-based JS output"
```

---

### Task 11: GraphJSTEmitter — JST Node Emission

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/GraphJSTEmitter.cs`

This emitter produces JST nodes for production-correct, minified JavaScript output. It follows the same pattern as `RazorSkinJSTGenerator` but emits graph descriptor objects instead of SkinBinderInfo arrays.

- [ ] **Step 1: Create GraphJSTEmitter**

The emitter takes a `GraphTopology`, a `RuntimeScopeManager`, a `ClrContext`, and resolved identifiers. It produces JST `Statement` nodes that:

1. Declare the static graph descriptor variable
2. Build the descriptor object literal with:
   - `nodeTypes` as array literal
   - `getters` as array of function expressions (using resolved identifiers for minified property access)
   - `consumers` as array of array literals
   - `gateIndices`, `defaultValues`, `targetInfos`, `subscriptions` arrays
   - `sourceType`, `subscribeMode`, `nodeCount` literals
3. Return JST statements to be included in the factory output

Key design decision: getter functions in the graph use the same `TryBuildResolvedGetterExpression` pattern from `RazorSkinJSTGenerator` to produce minified property access like `dc.get_propStr1_c()` instead of unminified `dc.get_propStr1()`.

Implementation should follow `RazorSkinJSTGenerator.BuildBinderExpressions()` as a pattern but emit graph descriptor structure instead of SkinBinderInfo factory calls.

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/GraphJSTEmitter.cs
git commit -m "feat(binding-graph): add GraphJSTEmitter JST node emission"
```

---

### Task 12: Integrate Graph Emission into RazorSkinJSTGenerator

**Files:**
- Modify: `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs`
- Modify: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`

- [ ] **Step 1: Add graph mode to RazorSkinJSTGenerator**

Add a `bool useGraphMode` parameter (or detect from IR) to `Generate()`. When graph mode is active:

1. Call `GraphTopologyBuilder.Build(ir)` to get topology
2. Call `GraphJSTEmitter` to produce graph descriptor JST
3. Emit factory function that:
   - Creates DOM template (same as today)
   - Creates element references (same as today)
   - Calls `SkinInstance_graph_factory(skinFactory, htmlRoot, childElements, elemRefs, graphDescriptor, partIdMapping)` instead of the existing `SkinInstance_factory` call
4. Emit getter function (same as today)

The existing SkinBinderInfo emission path remains for backward compatibility (fallback when graph mode is disabled).

- [ ] **Step 2: Add graph factory call to RazorTemplatingPlugin**

In `RazorTemplatingPlugin.ResolveRuntimeIdentifiers()`, resolve the new `SkinInstance_graph_factory` identifier alongside the existing `SkinInstance_factory`.

In `GetPostJavascript()`, when the template uses graph mode, emit the graph descriptor variable declaration before the factory function.

- [ ] **Step 3: Verify existing snapshot tests still pass**

Run: `dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj`
Expected: All existing tests pass (graph mode is opt-in, not default yet)

- [ ] **Step 4: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs
git add Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs
git commit -m "feat(binding-graph): integrate graph emission into JST generator and plugin"
```

---

## Phase 5: Advanced Runtime Features

### Task 13: Gate Node Runtime — @if/@else DOM Range Operations

**Files:**
- Modify: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs`

- [ ] **Step 1: Add DOM range operations to GraphEngine**

Extend `PushInitialValues()` and `Flush()` to handle Gate nodes with DOM manipulation:

**Gate opens (condition becomes true):**
- Clone the true-branch template from `GateTargetInfo.TrueTemplate`
- Insert all child nodes of the fragment after the marker element (`ElemRefs[markerIdx]`)
- Update element references for nodes inside the gate

**Gate closes (condition becomes false):**
- Remove `TrueElemCount` DOM nodes following the marker
- Apply default values to all gated nodes (already implemented in `ApplyGateClosure`)

**Gate toggle (@if/@else):**
- Remove current branch elements
- Clone and insert the other branch template

The marker element is a comment node `<!--r:if:N-->` placed in the DOM by the factory.

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs
git commit -m "feat(binding-graph): add Gate node DOM range operations"
```

---

### Task 14: CollectionManager Runtime — @foreach Child Graphs

**Files:**
- Modify: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs`
- Modify: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphState.cs`

- [ ] **Step 1: Add collection fields to GraphState**

Add to `GraphState`:
```csharp
/// <summary>Per-CollectionManager: comment markers for items.</summary>
public NativeArray<NativeArray> ItemMarkers;

/// <summary>Per-CollectionManager: element counts per item.</summary>
public NativeArray<NativeArray<int>> ItemElementCounts;

/// <summary>Per-CollectionManager: child graph states per item.</summary>
public NativeArray<NativeArray<GraphState>> ChildGraphStates;
```

- [ ] **Step 2: Add CollectionManager handling to GraphEngine**

Handle the `CollectionManager` node type in `PushInitialValues()` and `Flush()`:

**Initial render:** For each item in the collection:
1. Create comment marker `<!--r:item:N-->`
2. Clone item template from static item graph
3. Create child `GraphState`, call `PushInitialValues` with item as DataContext
4. Insert comment marker + DOM elements after collection marker
5. Register child graph with flush coordinator at `parentDepth + 1`

**Collection changes (during flush):** Listen for `INotifyCollectionChanged` events:
- `Add(item, index)` → create child graph + DOM, insert at position
- `Remove(index)` → dispose child graph, remove DOM elements
- `Reset` → dispose all, recreate from new collection

- [ ] **Step 3: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 4: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphState.cs
git commit -m "feat(binding-graph): add CollectionManager child graph lifecycle"
```

---

### Task 15: EventBinding Runtime

**Files:**
- Modify: `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs`

- [ ] **Step 1: Add event wiring to PushInitialValues and Flush**

During `PushInitialValues()`, when an `EventBinding` node is reached:
1. Get the method reference from the parent Property node's cached value
2. Get the target element from `EventTargetInfo.ElemIdx`
3. Call `element.AddEventListener(eventName, handler)`

During `Flush()`, when an `EventBinding` node's method reference changes:
1. Remove old event listener
2. Add new event listener

Store active event listener references in GraphState for cleanup during Deactivate/Dispose.

- [ ] **Step 2: Verify build**

Run: `dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs
git commit -m "feat(binding-graph): add EventBinding wiring in graph engine"
```

---

## Phase 6: End-to-End Browser Tests

### Task 16: Browser Test — Simple Text Binding with Graph

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/GraphSimpleText.skin.cshtml`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplatesClass.cs`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj`

- [ ] **Step 1: Create graph-mode template**

`RazorTemplates/GraphSimpleText.skin.cshtml`:
```razor
@model Sunlight.Framework.UI.Test.TestViewModelA

<div data-test="1"><span>@Model.PropStr1</span></div>
```

This is identical to `RazorSimpleText.skin.cshtml` but will be compiled with graph mode once the compiler switch is active.

- [ ] **Step 2: Register template**

Add to `RazorSkinTemplatesClass.cs`:
```csharp
[Skin("Sunlight.Framework.UI.Test.RazorTemplates.GraphSimpleText.skin.cshtml")]
public static Skin GraphSimpleText
{
    get { return null; }
}
```

Add to `.csproj`:
```xml
<EmbeddedResource Include="RazorTemplates\GraphSimpleText.skin.cshtml" />
```

- [ ] **Step 3: Write browser test**

Add to `RazorSkinTemplateTests.cs`:
```csharp
// ------------------------------------------------------------------
// Graph mode tests
// ------------------------------------------------------------------

[Test]
public static void TestGraphSimpleTextBinding(Assert assert)
{
    var element = Window.Instance.Document.CreateElement("div");
    var control = new UISkinableElement(element);

    var vm = new TestViewModelA();
    vm.PropStr1 = "Hello Graph";
    control.DataContext = vm;
    control.Skin = RazorSkinTemplatesClass.GraphSimpleText;

    assert.NotEqual(null, control.Skin, "Graph skin should be compiled and available");

    control.Activate();

    var span = element.QuerySelector("[data-test] span");
    assert.NotEqual(null, span, "Skin should render a span element");
    assert.Equal("Hello Graph", span.TextContent,
        "Span text should match bound PropStr1 value");
}

[Test]
public static void TestGraphOneWayReactivity(Assert assert)
{
    var element = Window.Instance.Document.CreateElement("div");
    var control = new UISkinableElement(element);

    var vm = new TestViewModelA();
    vm.PropStr1 = "Initial";
    control.DataContext = vm;
    control.Skin = RazorSkinTemplatesClass.GraphSimpleText;
    control.Activate();

    var span = element.QuerySelector("[data-test] span");
    assert.Equal("Initial", span.TextContent, "Initial value should be rendered");

    vm.PropStr1 = "Updated";
    assert.Equal("Updated", span.TextContent,
        "Graph binding should update reactively when property changes");
}

[Test]
public static void TestGraphFlipFlopElimination(Assert assert)
{
    var element = Window.Instance.Document.CreateElement("div");
    var control = new UISkinableElement(element);

    var vm = new TestViewModelA();
    vm.PropStr1 = "Original";
    control.DataContext = vm;
    control.Skin = RazorSkinTemplatesClass.GraphSimpleText;
    control.Activate();

    var span = element.QuerySelector("[data-test] span");
    assert.Equal("Original", span.TextContent, "Initial value");

    // Flip-flop: change and change back before flush
    // Both changes happen synchronously before microtask boundary
    vm.PropStr1 = "Changed";
    vm.PropStr1 = "Original";

    // After microtask flush, DOM should show "Original" (net change: none)
    assert.Equal("Original", span.TextContent,
        "Flip-flop should be eliminated — no net DOM change");
}

[Test]
public static void TestGraphDataContextChange(Assert assert)
{
    var element = Window.Instance.Document.CreateElement("div");
    var control = new UISkinableElement(element);

    var vm1 = new TestViewModelA();
    vm1.PropStr1 = "VM1";
    control.DataContext = vm1;
    control.Skin = RazorSkinTemplatesClass.GraphSimpleText;
    control.Activate();

    var span = element.QuerySelector("[data-test] span");
    assert.Equal("VM1", span.TextContent, "Should show first VM value");

    var vm2 = new TestViewModelA();
    vm2.PropStr1 = "VM2";
    control.DataContext = vm2;

    span = element.QuerySelector("[data-test] span");
    assert.Equal("VM2", span.TextContent,
        "Should show second VM value after DataContext change");

    // Old VM changes should NOT affect control
    vm1.PropStr1 = "VM1 Updated";
    span = element.QuerySelector("[data-test] span");
    assert.Equal("VM2", span.TextContent,
        "Old VM changes should not affect control after DataContext swap");
}
```

- [ ] **Step 4: Build and run**

Run: `dotnet build NScript_Full.sln -c Debug`
Expected: Build succeeds. Tests may fail initially until graph mode compilation is wired through.

- [ ] **Step 5: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/GraphSimpleText.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplatesClass.cs
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git add Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj
git commit -m "feat(binding-graph): add browser tests for graph-mode bindings"
```

---

### Task 17: Browser Test — Multi-Binding and Lifecycle

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/GraphMultiBinding.skin.cshtml`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplatesClass.cs`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj`

- [ ] **Step 1: Create multi-binding template**

`RazorTemplates/GraphMultiBinding.skin.cshtml`:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
  <div class="name"><span>@Model.Name</span></div>
  <div class="count"><span>@Model.Count</span></div>
</div>
```

- [ ] **Step 2: Register template and add to .csproj**

Add `[Skin]` registration and `<EmbeddedResource>` entry.

- [ ] **Step 3: Write browser tests**

Tests to add:
- `TestGraphMultiBinding` — Verify two independent bindings update independently
- `TestGraphActivateRendersInitialValues` — DOM empty before Activate, populated after
- `TestGraphDeactivateStopsUpdates` — After Deactivate, property changes don't update DOM
- `TestGraphBatchedUpdates` — Change Name and Count in one frame, verify single DOM update cycle

- [ ] **Step 4: Build and verify**

Run: `dotnet build NScript_Full.sln -c Debug`

- [ ] **Step 5: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/GraphMultiBinding.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplatesClass.cs
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git add Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj
git commit -m "feat(binding-graph): add multi-binding and lifecycle browser tests"
```

---

## Key Reference Files

| File | Pattern to Reuse |
|------|-----------------|
| `Sources/Framework/Sunlight.Framework.UI/Helpers/SkinInstance.cs` | Existing lifecycle: Activate/Deactivate/Dispose, QueuedActivation via TaskScheduler |
| `Sources/Framework/Sunlight.Framework.UI/Helpers/LiveBinder.cs` | PropertyChanged subscription pattern, Source/Target property setters |
| `Sources/Framework/Sunlight.Framework.UI/Helpers/SkinBinderHelper.cs` | SetTextContent, SetAttribute, SetClassName, SetStyle — reuse these as DomTarget setters |
| `Sources/Framework/Sunlight.Framework.UI/Helpers/ConditionalBinder.cs` | DOM fragment cloning and insertion pattern for @if |
| `Sources/Framework/Sunlight.Framework.UI/Helpers/CollectionBinder.cs` | Collection change handling pattern for @foreach |
| `Sources/Framework/Sunlight.Framework/TaskScheduler.cs` | `EnqueHighPriTask` for flush scheduling, `EnqueueLowPriTask` for deferred subscription wiring |
| `Sources/Framework/Sunlight.Framework/Observables/ObservableObject.cs` | `AddPropertyChangedListener`/`RemovePropertyChangedListener` API |
| `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs` | JST emission patterns: scope management, identifier resolution, factory body construction |
| `Sources/Compiler/RazorSkinParser/CodeGen/BinderEmitter.cs` | SkinBinderInfo JS emission (legacy path, keep working) |
| `Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs` | IR node construction, binding classification |
| `Sources/Compiler/RazorSkinParser/RoslynAnalysisPhase.cs` | Observable property detection, binding mode refinement |
| `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs` | Browser test setup pattern: TaskScheduler, Document.CreateElement, UISkinableElement |
| `Test/Compiler/RazorSkinParser.Test/RazorSkinTestHelper.cs` | Snapshot test helper: CompileTemplate, CheckCode, GenerateExpectedOutput |

---

## Important Notes for Implementers

### NScript Framework Constraints

All code in `Sources/Framework/` is C# that gets compiled to JavaScript by the NScript compiler. This means:

1. **No reflection** — `typeof(T)`, `GetType()`, `Assembly.GetTypes()` etc. are not available
2. **Limited C# features** — No `dynamic`, no `yield return`, no LINQ-to-objects (use manual loops)
3. **`NativeArray<T>`** — Use instead of `List<T>` or arrays where the JS equivalent is `Array`
4. **`object.IsNullOrUndefined(x)`** — Use instead of `x == null` for JS null/undefined checks
5. **String-based property names** — `FirePropertyChanged("Name")` not `nameof(Name)`
6. **No generics in some contexts** — The transpiler has limited generic support; prefer concrete types

### Backward Compatibility

- **XWML templates MUST continue working unchanged.** The `LegacyBinderStrategy` wraps the exact existing code path.
- **Existing browser tests MUST continue passing.** The strategy pattern in SkinInstance is transparent to callers.
- **The `SkinInstance_factory` JS function signature must not change.** Add a new `SkinInstance_graph_factory` function for graph mode.

### Testing Strategy

- **Compiler snapshot tests** verify the generated JS text matches expectations (text-based emitter)
- **Compiler unit tests** verify the graph topology builder produces correct node arrays
- **Browser tests** verify end-to-end: template → compile → load in browser → DOM updates correctly
- **Run existing tests after every change** to catch regressions: `dotnet test NScript_Full.sln -c Release`
