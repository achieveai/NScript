namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using System.Collections;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Stateless evaluation engine for reactive binding graphs.
    /// All state lives in GraphState (per-instance) and GraphDescriptor (shared static).
    /// All methods are static.
    /// </summary>
    public static class GraphEngine
    {
        /// <summary>
        /// Phase 1 synchronous value push during Activate().
        /// Walks all nodes in topological order (index 0..N), evaluates each node,
        /// and writes initial values to DOM targets.
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        public static void PushInitialValues(GraphDescriptor desc, GraphState state)
        {
            int n = desc.NodeCount;

            for (int i = 0; i < n; i++)
            {
                int nodeType = desc.NodeTypes[i];

                // Skip gated nodes whose gate is closed — apply default values instead.
                // Convention: gateIdx >= 0 means "only when gate is open".
                //             gateIdx <= -2 means "only when gate at index -(gateIdx+2) is CLOSED" (inverted gate).
                int gateIdx = desc.GateIndices[i];
                if (gateIdx >= 0 && !state.GateOpen[gateIdx])
                {
                    state.Values[i] = desc.DefaultValues[i];
                    continue;
                }
                if (gateIdx <= -2 && state.GateOpen[-(gateIdx + 2)])
                {
                    state.Values[i] = desc.DefaultValues[i];
                    continue;
                }

                if (nodeType == GraphNodeType.Source)
                {
                    // Read from state.Sources[desc.RootSourceSlot].
                    // Type-check against desc.SourceType at the boundary.
                    object sourceVal = state.Sources[desc.RootSourceSlot];
                    if (!object.IsNullOrUndefined(sourceVal)
                        && !object.IsNullOrUndefined(desc.SourceType)
                        && !desc.SourceType.IsInstanceOfType(sourceVal))
                    {
                        sourceVal = null;
                    }
                    state.Values[i] = sourceVal;
                }
                else if (nodeType == GraphNodeType.Property
                    || nodeType == GraphNodeType.Computed
                    || nodeType == GraphNodeType.TypeGuard)
                {
                    // Call getter(parentValue). If parent value is null, output null.
                    object parentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> getter = desc.Getters[i];
                    if (!object.IsNullOrUndefined(parentVal) && !object.IsNullOrUndefined(getter))
                    {
                        state.Values[i] = getter(parentVal);
                    }
                    else
                    {
                        state.Values[i] = null;
                    }
                }
                else if (nodeType == GraphNodeType.Gate)
                {
                    // Evaluate gate condition, then set gateOpen[i].
                    // If getter is null, use parentVal directly as the condition
                    // (the parent Property node already evaluated it to a boolean).
                    object parentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> getter = desc.Getters[i];
                    object condVal = null;
                    if (!object.IsNullOrUndefined(parentVal))
                    {
                        if (!object.IsNullOrUndefined(getter))
                            condVal = getter(parentVal);
                        else
                            condVal = parentVal;
                    }

                    state.Values[i] = condVal;

                    bool gateIsOpen = GraphEngine.IsTruthyValue(condVal);
                    bool wasOpen = state.GateOpen[i];
                    state.GateOpen[i] = gateIsOpen;

                    // DOM operations: clone and insert the appropriate template branch.
                    GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(gateInfo))
                    {
                        Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];
                        object templateObj = gateIsOpen
                            ? gateInfo.TrueTemplate
                            : gateInfo.FalseTemplate;

                        if (!object.IsNullOrUndefined(templateObj) && !object.IsNullOrUndefined(marker))
                        {
                            // Template is an HTML string — parse it into a DOM element.
                            Element clone = GraphEngine.ParseTemplateHtml((string)templateObj, marker);

                            if (!object.IsNullOrUndefined(clone))
                            {
                                Node parent = marker.ParentNode;
                                if (!object.IsNullOrUndefined(parent))
                                {
                                    parent.InsertBefore(clone, marker);
                                }
                                state.GateElements[i] = clone;

                                // Resolve child elem refs from the rendered template.
                                // Elements inside gate branches don't exist in static HTML;
                                // they're created when the gate renders its template.
                                GraphEngine.ResolveGateChildElems(
                                    state, gateInfo, clone, gateIsOpen);
                            }
                        }
                    }

                    // If gate just closed, apply defaults to all gated children.
                    if (!gateIsOpen && wasOpen)
                    {
                        GraphEngine.ApplyGateClosure(desc, state, i);
                    }
                }
                else if (nodeType == GraphNodeType.DomTarget)
                {
                    // Get input from parent, write to DOM via targetInfo.Setter(element, value).
                    object inputVal = GraphEngine.FindParentValue(desc, state, i);
                    state.Values[i] = inputVal;

                    DomTargetInfo targetInfo = (DomTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(targetInfo))
                    {
                        object elem = state.ElemRefs[targetInfo.ElemIdx];
                        if (!object.IsNullOrUndefined(targetInfo.Setter) && !object.IsNullOrUndefined(elem))
                        {
                            object val = !object.IsNullOrUndefined(inputVal)
                                ? inputVal
                                : desc.DefaultValues[i];
                            targetInfo.Setter(elem, val);
                        }
                    }
                }
                else if (nodeType == GraphNodeType.EventBinding)
                {
                    // Get method reference by applying getter to parent value.
                    object evtParentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> evtGetter = desc.Getters[i];
                    object methodRef = null;
                    if (!object.IsNullOrUndefined(evtParentVal) && !object.IsNullOrUndefined(evtGetter))
                        methodRef = evtGetter(evtParentVal);
                    state.Values[i] = methodRef;

                    EventTargetInfo evtInfo = (EventTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(evtInfo) && !object.IsNullOrUndefined(methodRef))
                    {
                        Element evtElem = (Element)state.ElemRefs[evtInfo.ElemIdx];
                        if (!object.IsNullOrUndefined(evtElem))
                        {
                            Action<Element, ElementEvent> handler = (Action<Element, ElementEvent>)methodRef;
                            evtElem.Bind(evtInfo.EventName, handler);
                            state.EventListeners[i] = handler;
                        }
                    }
                }
                else if (nodeType == GraphNodeType.CollectionManager)
                {
                    // Get collection by applying getter to the parent value (same as Property nodes).
                    object parentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> getter = desc.Getters[i];
                    object collection = null;
                    if (!object.IsNullOrUndefined(parentVal) && !object.IsNullOrUndefined(getter))
                        collection = getter(parentVal);
                    state.Values[i] = collection;

                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo) && !object.IsNullOrUndefined(collection))
                    {
                        IObservableCollection obsCol = GraphEngine.AsObservableCollection(collection);
                        if (!object.IsNullOrUndefined(obsCol))
                        {
                            GraphEngine.RenderCollection(desc, state, i, colInfo, obsCol);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dirty-node flush on microtask boundary.
        /// Linear scan in topological order; evaluates dirty nodes, propagates changes.
        /// Implements flip-flop elimination via reference equality comparison.
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        public static void Flush(GraphDescriptor desc, GraphState state)
        {
            // Reentrancy guard: if a property setter triggers another PropertyChanged
            // during flush, skip the re-entrant call. The outer flush will pick up
            // newly dirtied nodes as it continues its forward scan.
            if (state.Flushing) return;
            state.Flushing = true;
            state.FlushScheduled = false;
            int n = desc.NodeCount;

            for (int i = 0; i < n; i++)
            {
                // Skip clean nodes.
                if (!state.Dirty[i])
                {
                    continue;
                }

                // Skip gated nodes whose gate is closed (or inverted gate that is open).
                int gateIdx = desc.GateIndices[i];
                if (gateIdx >= 0 && !state.GateOpen[gateIdx])
                {
                    state.Dirty[i] = false;
                    continue;
                }
                if (gateIdx <= -2 && state.GateOpen[-(gateIdx + 2)])
                {
                    state.Dirty[i] = false;
                    continue;
                }

                int nodeType = desc.NodeTypes[i];
                object newVal = null;

                if (nodeType == GraphNodeType.Source)
                {
                    newVal = state.Sources[desc.RootSourceSlot];
                    if (!object.IsNullOrUndefined(newVal)
                        && !object.IsNullOrUndefined(desc.SourceType)
                        && !desc.SourceType.IsInstanceOfType(newVal))
                    {
                        newVal = null;
                    }
                }
                else if (nodeType == GraphNodeType.Property
                    || nodeType == GraphNodeType.Computed
                    || nodeType == GraphNodeType.TypeGuard)
                {
                    object parentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> getter = desc.Getters[i];
                    if (!object.IsNullOrUndefined(parentVal) && !object.IsNullOrUndefined(getter))
                    {
                        newVal = getter(parentVal);
                    }
                    else
                    {
                        newVal = null;
                    }
                }
                else if (nodeType == GraphNodeType.Gate)
                {
                    // Gate: use parentVal directly (getter is null — parent Property already evaluated condition)
                    object parentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> getter = desc.Getters[i];
                    if (!object.IsNullOrUndefined(parentVal))
                    {
                        if (!object.IsNullOrUndefined(getter))
                            newVal = getter(parentVal);
                        else
                            newVal = parentVal;
                    }
                    else
                    {
                        newVal = null;
                    }
                }
                else if (nodeType == GraphNodeType.DomTarget)
                {
                    newVal = GraphEngine.FindParentValue(desc, state, i);
                }
                else if (nodeType == GraphNodeType.EventBinding)
                {
                    object evtFlushParent = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> evtFlushGetter = desc.Getters[i];
                    if (!object.IsNullOrUndefined(evtFlushParent) && !object.IsNullOrUndefined(evtFlushGetter))
                        newVal = evtFlushGetter(evtFlushParent);
                    else
                        newVal = evtFlushParent;
                }
                else if (nodeType == GraphNodeType.CollectionManager)
                {
                    object colParentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> colGetter = desc.Getters[i];
                    if (!object.IsNullOrUndefined(colParentVal) && !object.IsNullOrUndefined(colGetter))
                        newVal = colGetter(colParentVal);
                    else
                        newVal = colParentVal;
                }

                // Flip-flop elimination: compare new value to cached value.
                // If unchanged (reference equality), clear dirty flag and DON'T dirty consumers.
                object oldVal = state.Values[i];
                state.Dirty[i] = false;

                if (newVal == oldVal)
                {
                    // No change — skip propagation.
                    continue;
                }

                // Value changed — update cache.
                state.Values[i] = newVal;

                // Handle gate open/close side effects.
                if (nodeType == GraphNodeType.Gate)
                {
                    bool gateIsOpen = GraphEngine.IsTruthyValue(newVal);
                    bool wasOpen = state.GateOpen[i];
                    state.GateOpen[i] = gateIsOpen;

                    // DOM swap: remove old branch, insert new branch.
                    if (gateIsOpen != wasOpen)
                    {
                        GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[i];
                        if (!object.IsNullOrUndefined(gateInfo))
                        {
                            // Clear child elem refs from the OLD branch before removing.
                            GraphEngine.ClearGateChildElems(state, gateInfo, wasOpen);

                            // Remove current branch element.
                            Element oldElem = (Element)state.GateElements[i];
                            if (!object.IsNullOrUndefined(oldElem))
                            {
                                oldElem.Remove();
                                state.GateElements[i] = null;
                            }

                            // Insert new branch template.
                            object templateObj = gateIsOpen
                                ? gateInfo.TrueTemplate
                                : gateInfo.FalseTemplate;
                            Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];

                            if (!object.IsNullOrUndefined(templateObj) && !object.IsNullOrUndefined(marker))
                            {
                                Element clone = GraphEngine.ParseTemplateHtml((string)templateObj, marker);
                                if (!object.IsNullOrUndefined(clone))
                                {
                                    Node parent = marker.ParentNode;
                                    if (!object.IsNullOrUndefined(parent))
                                    {
                                        parent.InsertBefore(clone, marker);
                                    }
                                    state.GateElements[i] = clone;

                                    // Resolve child elem refs from the new branch.
                                    GraphEngine.ResolveGateChildElems(
                                        state, gateInfo, clone, gateIsOpen);
                                }
                            }
                        }
                    }

                    int invertedRef = -(i + 2);

                    if (!gateIsOpen && wasOpen)
                    {
                        // Gate closed: reset true-branch, activate false-branch
                        GraphEngine.ApplyGateClosure(desc, state, i);
                        GraphEngine.MarkGatedConsumersDirty(desc, state, invertedRef);
                    }
                    else if (gateIsOpen && !wasOpen)
                    {
                        // Gate opened: activate true-branch, reset false-branch
                        GraphEngine.MarkGatedConsumersDirty(desc, state, i);
                        GraphEngine.ApplyGateClosure(desc, state, invertedRef);
                    }
                }

                // Apply DOM write for DomTarget nodes.
                if (nodeType == GraphNodeType.DomTarget)
                {
                    DomTargetInfo targetInfo = (DomTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(targetInfo))
                    {
                        object elem = state.ElemRefs[targetInfo.ElemIdx];
                        if (!object.IsNullOrUndefined(targetInfo.Setter) && !object.IsNullOrUndefined(elem))
                        {
                            object val = !object.IsNullOrUndefined(newVal)
                                ? newVal
                                : desc.DefaultValues[i];
                            targetInfo.Setter(elem, val);
                        }
                    }
                }

                // EventBinding: re-wire listener when method reference changes.
                if (nodeType == GraphNodeType.EventBinding)
                {
                    EventTargetInfo evtInfo = (EventTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(evtInfo))
                    {
                        Element evtElem = (Element)state.ElemRefs[evtInfo.ElemIdx];
                        if (!object.IsNullOrUndefined(evtElem))
                        {
                            // Remove old listener.
                            Action<Element, ElementEvent> oldHandler =
                                (Action<Element, ElementEvent>)state.EventListeners[i];
                            if (!object.IsNullOrUndefined(oldHandler))
                            {
                                evtElem.UnBind(evtInfo.EventName, oldHandler);
                            }

                            // Wire new listener.
                            if (!object.IsNullOrUndefined(newVal))
                            {
                                Action<Element, ElementEvent> newHandler =
                                    (Action<Element, ElementEvent>)newVal;
                                evtElem.Bind(evtInfo.EventName, newHandler);
                                state.EventListeners[i] = newHandler;
                            }
                            else
                            {
                                state.EventListeners[i] = null;
                            }
                        }
                    }
                }

                // CollectionManager: re-render when collection reference changes.
                if (nodeType == GraphNodeType.CollectionManager)
                {
                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo))
                    {
                        // Clear old collection items.
                        GraphEngine.ClearCollectionItems(desc, state, i, colInfo);

                        // Detach old collection listener.
                        if (!object.IsNullOrUndefined(oldVal))
                        {
                            GraphEngine.DetachCollectionListener(state, i, oldVal);
                        }

                        // Render new collection.
                        if (!object.IsNullOrUndefined(newVal))
                        {
                            IObservableCollection newObsCol = GraphEngine.AsObservableCollection(newVal);
                            if (!object.IsNullOrUndefined(newObsCol))
                                GraphEngine.RenderCollection(desc, state, i, colInfo, newObsCol);
                        }
                    }
                }

                // Mark all consumers dirty.
                NativeArray<int> consumers = desc.Consumers[i];
                if (!object.IsNullOrUndefined(consumers))
                {
                    for (int c = 0; c < consumers.Length; c++)
                    {
                        state.Dirty[consumers[c]] = true;
                    }
                }
            }

            state.Flushing = false;
        }

        /// <summary>
        /// Marks a node dirty. Called from a PropertyChanged listener.
        /// If no flush is scheduled, schedules one via GraphFlushCoordinator.
        /// </summary>
        /// <param name="state">The per-instance graph state.</param>
        /// <param name="nodeIdx">The index of the node to mark dirty.</param>
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
        /// Finds the parent node value that feeds into the node at nodeIdx.
        /// Uses pre-computed ParentIndices for O(1) lookup when available,
        /// falls back to scanning the consumers adjacency list.
        /// For nodes with multiple parents (e.g. Computed), returns the first parent's value.
        /// Use FindParentValues for multi-parent access.
        /// </summary>
        public static object FindParentValue(GraphDescriptor desc, GraphState state, int nodeIdx)
        {
            // Fast path: use pre-computed parent indices.
            if (!object.IsNullOrUndefined(desc.ParentIndices))
            {
                NativeArray<int> parents = desc.ParentIndices[nodeIdx];
                if (!object.IsNullOrUndefined(parents) && parents.Length > 0)
                {
                    return state.Values[parents[0]];
                }
                return null;
            }

            // Fallback: scan consumers adjacency list. O(nodes * edges).
            for (int p = 0; p < nodeIdx; p++)
            {
                NativeArray<int> consumers = desc.Consumers[p];
                if (object.IsNullOrUndefined(consumers))
                {
                    continue;
                }

                for (int c = 0; c < consumers.Length; c++)
                {
                    if (consumers[c] == nodeIdx)
                    {
                        return state.Values[p];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all parent values for a node with multiple inputs (e.g. Computed nodes).
        /// Uses pre-computed ParentIndices for O(1) lookup.
        /// Returns null if ParentIndices not available or node has no parents.
        /// </summary>
        public static NativeArray FindParentValues(GraphDescriptor desc, GraphState state, int nodeIdx)
        {
            if (object.IsNullOrUndefined(desc.ParentIndices))
            {
                return null;
            }

            NativeArray<int> parents = desc.ParentIndices[nodeIdx];
            if (object.IsNullOrUndefined(parents) || parents.Length == 0)
            {
                return null;
            }

            NativeArray values = new NativeArray(parents.Length);
            for (int i = 0; i < parents.Length; i++)
            {
                values[i] = state.Values[parents[i]];
            }
            return values;
        }

        /// <summary>
        /// Applies gate closure: sets defaults on all nodes gated by this gate.
        /// Called when a gate node transitions from open to closed.
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        /// <param name="gateIdx">The index of the gate node that closed.</param>
        public static void ApplyGateClosure(GraphDescriptor desc, GraphState state, int gateIdx)
        {
            int n = desc.NodeCount;
            // Start from max(gateIdx+1, 0) to handle inverted gate indices (negative values).
            int start = gateIdx >= 0 ? gateIdx + 1 : 0;

            for (int i = start; i < n; i++)
            {
                if (desc.GateIndices[i] != gateIdx)
                {
                    continue;
                }

                state.Values[i] = desc.DefaultValues[i];
                state.Dirty[i] = false;

                int nodeType = desc.NodeTypes[i];

                // DomTarget: apply default to DOM.
                if (nodeType == GraphNodeType.DomTarget)
                {
                    DomTargetInfo targetInfo = (DomTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(targetInfo))
                    {
                        object elem = state.ElemRefs[targetInfo.ElemIdx];
                        if (!object.IsNullOrUndefined(targetInfo.Setter) && !object.IsNullOrUndefined(elem))
                        {
                            targetInfo.Setter(elem, desc.DefaultValues[i]);
                        }
                    }
                }
                // EventBinding: unbind listener when gate closes.
                else if (nodeType == GraphNodeType.EventBinding)
                {
                    Action<Element, ElementEvent> handler =
                        (Action<Element, ElementEvent>)state.EventListeners[i];
                    if (!object.IsNullOrUndefined(handler))
                    {
                        EventTargetInfo evtInfo = (EventTargetInfo)desc.TargetInfos[i];
                        if (!object.IsNullOrUndefined(evtInfo))
                        {
                            Element elem = (Element)state.ElemRefs[evtInfo.ElemIdx];
                            if (!object.IsNullOrUndefined(elem))
                            {
                                elem.UnBind(evtInfo.EventName, handler);
                            }
                        }
                        state.EventListeners[i] = null;
                    }
                }
                // CollectionManager: detach listener and clear items when gate closes.
                else if (nodeType == GraphNodeType.CollectionManager)
                {
                    object collection = state.Values[i];
                    if (!object.IsNullOrUndefined(collection))
                    {
                        GraphEngine.DetachCollectionListener(state, i, collection);
                    }

                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo))
                    {
                        GraphEngine.ClearCollectionItems(desc, state, i, colInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Marks all nodes gated by this gate as dirty.
        /// Called when a gate node transitions from closed to open (gate reopens).
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        /// <param name="gateIdx">The index of the gate node that reopened.</param>
        public static void MarkGatedConsumersDirty(GraphDescriptor desc, GraphState state, int gateIdx)
        {
            int n = desc.NodeCount;

            for (int i = 0; i < n; i++)
            {
                if (desc.GateIndices[i] == gateIdx)
                {
                    state.Dirty[i] = true;
                }
            }
        }

        /// <summary>
        /// Renders all items in a collection for a CollectionManager node.
        /// Creates child graph state + DOM elements per item.
        /// </summary>
        public static void RenderCollection(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, IObservableCollection collection)
        {
            Element marker = (Element)state.ElemRefs[colInfo.MarkerIdx];
            if (object.IsNullOrUndefined(marker)) return;

            Node parent = marker.ParentNode;
            if (object.IsNullOrUndefined(parent)) return;

            int count = collection.Count;
            NativeArray<GraphState> childStates = new NativeArray<GraphState>(count);
            NativeArray itemElems = new NativeArray(count);

            for (int idx = 0; idx < count; idx++)
            {
                object item = collection[idx];

                // Item template is an HTML string — parse to DOM element.
                Element clone = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, marker);
                if (object.IsNullOrUndefined(clone)) continue;

                parent.InsertBefore(clone, marker);
                itemElems[idx] = clone;

                // Create child graph if item graph descriptor exists.
                if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                {
                    // Build element refs for the item's bindings.
                    // Find all <span> elements inside the clone — these are the binding markers.
                    NativeArray childElemRefs = GraphEngine.CollectSpanElements(clone);

                    // Resolve event target elements from data-evt-idx markers in the item HTML.
                    GraphEngine.ResolveEventElements(clone, childElemRefs);

                    GraphState childState = new GraphState(colInfo.ItemGraph, childElemRefs, state.Depth + 1);
                    childState.Sources[GraphSourceSlot.DataContext] = item;
                    GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
                    GraphEngine.WireChildSubscriptions(colInfo.ItemGraph, childState, item);
                    childStates[idx] = childState;
                }
            }

            state.ChildGraphStates[nodeIdx] = childStates;
            state.ItemElements[nodeIdx] = itemElems;

            // Attach collection change listener.
            GraphEngine.AttachCollectionListener(desc, state, nodeIdx, colInfo, collection);
        }

        /// <summary>
        /// Clears all rendered items for a CollectionManager node.
        /// </summary>
        public static void ClearCollectionItems(GraphDescriptor desc, GraphState state, int nodeIdx, CollectionTargetInfo colInfo)
        {
            NativeArray itemElems = state.ItemElements[nodeIdx];
            NativeArray<GraphState> childStatesForCleanup = state.ChildGraphStates[nodeIdx];
            if (!object.IsNullOrUndefined(itemElems))
            {
                for (int idx = 0; idx < itemElems.Length; idx++)
                {
                    // Remove gate elements from child graph before removing item element.
                    if (!object.IsNullOrUndefined(childStatesForCleanup))
                    {
                        GraphState childState = childStatesForCleanup[idx];
                        if (!object.IsNullOrUndefined(childState))
                            GraphEngine.RemoveChildGateElements(childState);
                    }

                    Element elem = (Element)itemElems[idx];
                    if (!object.IsNullOrUndefined(elem))
                    {
                        elem.Remove();
                    }
                }
            }

            // Dispose child graph states — clean up listeners recursively.
            NativeArray<GraphState> childStates = state.ChildGraphStates[nodeIdx];
            if (!object.IsNullOrUndefined(childStates))
            {
                GraphDescriptor itemDesc = colInfo.ItemGraph;

                for (int idx = 0; idx < childStates.Length; idx++)
                {
                    GraphState child = childStates[idx];
                    if (object.IsNullOrUndefined(child))
                    {
                        continue;
                    }

                    // Clean up ALL listeners on child states: property, event, and collection.
                    if (!object.IsNullOrUndefined(itemDesc))
                    {
                        GraphEngine.UnwireChildSubscriptions(itemDesc, child);
                        GraphEngine.CleanupEventListeners(itemDesc, child);
                        GraphEngine.CleanupCollectionListeners(itemDesc, child);
                    }

                    for (int v = 0; v < child.Values.Length; v++)
                    {
                        child.Values[v] = null;
                    }
                }
            }

            state.ChildGraphStates[nodeIdx] = null;
            state.ItemElements[nodeIdx] = null;
        }

        /// <summary>
        /// Attaches a CollectionChanged listener for incremental updates.
        /// </summary>
        public static void AttachCollectionListener(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, IObservableCollection collection)
        {
            INotifyCollectionChanged notifier = collection as INotifyCollectionChanged;
            if (object.IsNullOrUndefined(notifier)) return;

            int capturedNodeIdx = nodeIdx;
            GraphDescriptor capturedDesc = desc;
            GraphState capturedState = state;
            CollectionTargetInfo capturedColInfo = colInfo;

            Action<INotifyCollectionChanged, CollectionChangedEventArgs> handler =
                delegate(INotifyCollectionChanged sender, CollectionChangedEventArgs args)
                {
                    GraphEngine.OnCollectionChanged(
                        capturedDesc, capturedState, capturedNodeIdx, capturedColInfo, args);
                };

            notifier.CollectionChanged += handler;
            state.CollectionListeners[nodeIdx] = handler;
        }

        /// <summary>
        /// Detaches a CollectionChanged listener.
        /// </summary>
        public static void DetachCollectionListener(GraphState state, int nodeIdx, object oldCollection)
        {
            object handler = state.CollectionListeners[nodeIdx];
            if (object.IsNullOrUndefined(handler)) return;

            INotifyCollectionChanged notifier = oldCollection as INotifyCollectionChanged;
            if (!object.IsNullOrUndefined(notifier))
            {
                notifier.CollectionChanged -=
                    (Action<INotifyCollectionChanged, CollectionChangedEventArgs>)handler;
            }

            state.CollectionListeners[nodeIdx] = null;
        }

        /// <summary>
        /// Handles incremental collection changes (add, remove, replace, reset).
        /// </summary>
        public static void OnCollectionChanged(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, CollectionChangedEventArgs args)
        {
            Element marker = (Element)state.ElemRefs[colInfo.MarkerIdx];
            if (object.IsNullOrUndefined(marker)) return;

            Node parent = marker.ParentNode;
            if (object.IsNullOrUndefined(parent)) return;

            if (args.Action == CollectionChangedAction.Add)
            {
                int insertIdx = args.ChangeIndex;
                IList newItems = args.NewItems;
                NativeArray<GraphState> childStates = state.ChildGraphStates[nodeIdx];
                NativeArray itemElems = state.ItemElements[nodeIdx];

                int oldCount = object.IsNullOrUndefined(itemElems) ? 0 : itemElems.Length;
                int addCount = newItems.Count;
                int newCount = oldCount + addCount;

                // Build new arrays with items inserted at position.
                NativeArray<GraphState> newChildStates = new NativeArray<GraphState>(newCount);
                NativeArray newItemElems = new NativeArray(newCount);

                // Copy items before insertion point.
                for (int j = 0; j < insertIdx && j < oldCount; j++)
                {
                    newChildStates[j] = !object.IsNullOrUndefined(childStates) ? childStates[j] : null;
                    newItemElems[j] = !object.IsNullOrUndefined(itemElems) ? itemElems[j] : null;
                }

                // Find the reference node for InsertBefore.
                Node refNode = marker;
                if (insertIdx < oldCount && !object.IsNullOrUndefined(itemElems))
                {
                    refNode = (Node)itemElems[insertIdx];
                }

                // Insert new items.
                for (int j = 0; j < addCount; j++)
                {
                    object item = newItems[j];
                    Element clone = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, marker);
                    if (object.IsNullOrUndefined(clone)) return;
                    parent.InsertBefore(clone, refNode);
                    newItemElems[insertIdx + j] = clone;

                    if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                    {
                        NativeArray childElemRefs = GraphEngine.CollectSpanElements(clone);
                        GraphEngine.ResolveEventElements(clone, childElemRefs);

                        GraphState childState = new GraphState(
                            colInfo.ItemGraph, childElemRefs, state.Depth + 1);
                        childState.Sources[GraphSourceSlot.DataContext] = item;
                        GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
                        GraphEngine.WireChildSubscriptions(colInfo.ItemGraph, childState, item);
                        newChildStates[insertIdx + j] = childState;
                    }
                }

                // Copy items after insertion point.
                for (int j = insertIdx; j < oldCount; j++)
                {
                    newChildStates[j + addCount] = !object.IsNullOrUndefined(childStates) ? childStates[j] : null;
                    newItemElems[j + addCount] = !object.IsNullOrUndefined(itemElems) ? itemElems[j] : null;
                }

                state.ChildGraphStates[nodeIdx] = newChildStates;
                state.ItemElements[nodeIdx] = newItemElems;
            }
            else if (args.Action == CollectionChangedAction.Remove)
            {
                int removeIdx = args.ChangeIndex;
                int removeCount = args.OldItems.Count;
                NativeArray<GraphState> childStates = state.ChildGraphStates[nodeIdx];
                NativeArray itemElems = state.ItemElements[nodeIdx];

                if (object.IsNullOrUndefined(itemElems)) return;

                int oldCount = itemElems.Length;
                int newCount = oldCount - removeCount;

                // Remove DOM elements (backwards to avoid index shifting).
                // Also remove any gate elements from child graph states —
                // gates render their content as siblings of the item element,
                // so removing just the item element leaves gate content behind.
                for (int j = removeIdx + removeCount - 1; j >= removeIdx; j--)
                {
                    if (!object.IsNullOrUndefined(childStates))
                    {
                        GraphState childState = childStates[j];
                        if (!object.IsNullOrUndefined(childState))
                        {
                            GraphEngine.RemoveChildGateElements(childState);
                        }
                    }

                    Element elem = (Element)itemElems[j];
                    if (!object.IsNullOrUndefined(elem))
                    {
                        elem.Remove();
                    }
                }

                // Rebuild arrays without removed items.
                NativeArray<GraphState> newChildStates = new NativeArray<GraphState>(newCount);
                NativeArray newItemElems = new NativeArray(newCount);

                for (int j = 0; j < removeIdx; j++)
                {
                    newChildStates[j] = !object.IsNullOrUndefined(childStates) ? childStates[j] : null;
                    newItemElems[j] = itemElems[j];
                }
                for (int j = removeIdx + removeCount; j < oldCount; j++)
                {
                    newChildStates[j - removeCount] = !object.IsNullOrUndefined(childStates) ? childStates[j] : null;
                    newItemElems[j - removeCount] = itemElems[j];
                }

                state.ChildGraphStates[nodeIdx] = newChildStates;
                state.ItemElements[nodeIdx] = newItemElems;
            }
            else if (args.Action == CollectionChangedAction.Replace)
            {
                int replaceIdx = args.ChangeIndex;
                NativeArray<GraphState> childStates = state.ChildGraphStates[nodeIdx];

                if (!object.IsNullOrUndefined(childStates) && replaceIdx < childStates.Length)
                {
                    GraphState oldChild = childStates[replaceIdx];
                    if (!object.IsNullOrUndefined(oldChild) && !object.IsNullOrUndefined(colInfo.ItemGraph))
                    {
                        // Unwire old child subscriptions to avoid leaks.
                        GraphEngine.UnwireChildSubscriptions(colInfo.ItemGraph, oldChild);
                        GraphEngine.CleanupEventListeners(colInfo.ItemGraph, oldChild);
                    }

                    // Update the child graph's DataContext to the new item and re-push.
                    object newItem = args.NewItems[0];
                    if (!object.IsNullOrUndefined(oldChild) && !object.IsNullOrUndefined(colInfo.ItemGraph))
                    {
                        oldChild.Sources[GraphSourceSlot.DataContext] = newItem;
                        oldChild.SubscriptionsActive = false;
                        GraphEngine.PushInitialValues(colInfo.ItemGraph, oldChild);
                        GraphEngine.WireChildSubscriptions(colInfo.ItemGraph, oldChild, newItem);
                    }
                }
            }
            else if (args.Action == CollectionChangedAction.Reset)
            {
                // Full reset: clear and re-render.
                GraphEngine.ClearCollectionItems(desc, state, nodeIdx, colInfo);
                object collection = state.Values[nodeIdx];
                if (!object.IsNullOrUndefined(collection))
                {
                    IObservableCollection obsCol = GraphEngine.AsObservableCollection(collection);
                    if (object.IsNullOrUndefined(obsCol)) return;
                    Element resetMarker = (Element)state.ElemRefs[colInfo.MarkerIdx];
                    if (!object.IsNullOrUndefined(resetMarker))
                    {
                        Node resetParent = resetMarker.ParentNode;
                        if (!object.IsNullOrUndefined(resetParent))
                        {
                            int count = obsCol.Count;
                            NativeArray<GraphState> newStates = new NativeArray<GraphState>(count);
                            NativeArray newElems = new NativeArray(count);

                            for (int idx = 0; idx < count; idx++)
                            {
                                object item = obsCol[idx];
                                Element clone = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, resetMarker);
                                if (object.IsNullOrUndefined(clone)) continue;
                                resetParent.InsertBefore(clone, resetMarker);
                                newElems[idx] = clone;

                                if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                                {
                                    NativeArray childElemRefs = GraphEngine.CollectSpanElements(clone);
                                    GraphEngine.ResolveEventElements(clone, childElemRefs);

                                    GraphState childState = new GraphState(
                                        colInfo.ItemGraph, childElemRefs, state.Depth + 1);
                                    childState.Sources[GraphSourceSlot.DataContext] = item;
                                    GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
                                    GraphEngine.WireChildSubscriptions(colInfo.ItemGraph, childState, item);
                                    newStates[idx] = childState;
                                }
                            }

                            state.ChildGraphStates[nodeIdx] = newStates;
                            state.ItemElements[nodeIdx] = newElems;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cleans up all event listeners for disposal.
        /// </summary>
        public static void CleanupEventListeners(GraphDescriptor desc, GraphState state)
        {
            int n = desc.NodeCount;
            for (int i = 0; i < n; i++)
            {
                if (desc.NodeTypes[i] == GraphNodeType.EventBinding)
                {
                    Action<Element, ElementEvent> handler =
                        (Action<Element, ElementEvent>)state.EventListeners[i];
                    if (!object.IsNullOrUndefined(handler))
                    {
                        EventTargetInfo evtInfo = (EventTargetInfo)desc.TargetInfos[i];
                        if (!object.IsNullOrUndefined(evtInfo))
                        {
                            Element elem = (Element)state.ElemRefs[evtInfo.ElemIdx];
                            if (!object.IsNullOrUndefined(elem))
                            {
                                elem.UnBind(evtInfo.EventName, handler);
                            }
                        }
                        state.EventListeners[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Cleans up all collection listeners for disposal.
        /// </summary>
        public static void CleanupCollectionListeners(GraphDescriptor desc, GraphState state)
        {
            int n = desc.NodeCount;
            for (int i = 0; i < n; i++)
            {
                if (desc.NodeTypes[i] == GraphNodeType.CollectionManager)
                {
                    object collection = state.Values[i];
                    if (!object.IsNullOrUndefined(collection))
                    {
                        GraphEngine.DetachCollectionListener(state, i, collection);
                    }

                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo))
                    {
                        GraphEngine.ClearCollectionItems(desc, state, i, colInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a value is truthy. Works with both raw JS booleans and boxed NScript booleans.
        /// Uses JavaScript-level truthiness (!!val) to avoid type unbox issues.
        /// </summary>
        [System.Runtime.CompilerServices.Script(@"return !!val;")]
        private static extern bool IsTruthyValue(object val);

        /// <summary>
        /// Wires property change subscriptions for a child graph state (e.g., a foreach item).
        /// Similar to GraphBindingStrategy.WireSubscriptions but operates on a standalone state.
        /// </summary>
        public static void WireChildSubscriptions(GraphDescriptor desc, GraphState childState, object dataContext)
        {
            if (childState.SubscriptionsActive) return;

            NativeArray subscriptions = desc.Subscriptions;
            if (object.IsNullOrUndefined(subscriptions) || subscriptions.Length == 0) return;

            INotifyPropertyChanged observable = dataContext as INotifyPropertyChanged;
            if (object.IsNullOrUndefined(observable)) return;

            int subCount = subscriptions.Length;
            NativeArray listeners = new NativeArray(subCount);

            for (int i = 0; i < subCount; i++)
            {
                SubscriptionEntry entry = (SubscriptionEntry)subscriptions[i];
                if (object.IsNullOrUndefined(entry)) continue;

                Action<INotifyPropertyChanged, string> callback =
                    GraphBindingStrategy.CreatePropertyCallback(childState, desc, entry.NodeIdx);

                observable.AddPropertyChangedListener(entry.PropertyName, callback);
                listeners[i] = callback;
            }

            childState.Listeners = listeners;
            childState.ListenerCount = subCount;
            childState.SubscriptionsActive = true;
        }

        /// <summary>
        /// Unwires property change subscriptions for a child graph state.
        /// Counterpart to WireChildSubscriptions — must be called when disposing
        /// child graphs (e.g., on collection item removal) to avoid memory leaks.
        /// </summary>
        public static void UnwireChildSubscriptions(GraphDescriptor desc, GraphState childState)
        {
            if (!childState.SubscriptionsActive) return;

            NativeArray subscriptions = desc.Subscriptions;
            if (object.IsNullOrUndefined(subscriptions)) return;

            int subCount = subscriptions.Length;
            for (int i = 0; i < subCount; i++)
            {
                SubscriptionEntry entry = (SubscriptionEntry)subscriptions[i];
                if (object.IsNullOrUndefined(entry)) continue;

                object source = childState.Sources[entry.SourceSlot];
                if (object.IsNullOrUndefined(source)) continue;

                INotifyPropertyChanged observable = source as INotifyPropertyChanged;
                if (object.IsNullOrUndefined(observable)) continue;

                if (i < childState.ListenerCount && !object.IsNullOrUndefined(childState.Listeners[i]))
                {
                    observable.RemovePropertyChangedListener(
                        entry.PropertyName,
                        (Action<INotifyPropertyChanged, string>)childState.Listeners[i]);
                }
            }

            childState.SubscriptionsActive = false;
        }

        /// <summary>
        /// Checks if an object implements IObservableCollection.
        /// Uses the standard as-cast which goes through NScript's type system.
        /// </summary>
        private static IObservableCollection AsObservableCollection(object obj)
        {
            return obj as IObservableCollection;
        }

        /// <summary>
        /// Collects span elements from an item clone for use as element refs in child graph states.
        /// Returns a NativeArray with each span as an entry. If no spans found, returns
        /// a single-element array containing the clone itself.
        /// </summary>
        public static NativeArray CollectSpanElements(Element clone)
        {
            NativeArray<Element> spans = clone.GetElementsByTagName("span");
            int spanCount = spans.Length;
            NativeArray result = new NativeArray(spanCount > 0 ? spanCount : 1);
            for (int i = 0; i < spanCount; i++)
            {
                result[i] = spans[i];
            }
            if (spanCount == 0)
            {
                result[0] = clone;
            }
            return result;
        }

        /// <summary>
        /// Replaces event marker spans (with data-ns-evt attribute) in elemRefs
        /// with their parent element. Event marker spans are inserted by the compiler
        /// into item template HTML to occupy the correct ElemIdx positions.
        /// The actual event target is the parent of the marker span.
        /// </summary>
        public static void ResolveEventElements(Element clone, NativeArray elemRefs)
        {
            NativeArray<Element> evtSpans = clone.QuerySelectorAll("[data-ns-evt]");
            if (object.IsNullOrUndefined(evtSpans) || evtSpans.Length == 0)
                return;

            for (int i = 0; i < evtSpans.Length; i++)
            {
                Element span = evtSpans[i];
                // Find this span in elemRefs and replace with its parent element
                for (int j = 0; j < elemRefs.Length; j++)
                {
                    if ((object)elemRefs[j] == (object)span)
                    {
                        elemRefs[j] = (Element)span.ParentNode;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses an HTML template string into a DOM Element by creating a temporary
        /// container, setting its innerHTML, and returning the first child element.
        /// Uses the marker element's ownerDocument to create the container.
        /// </summary>
        public static Element ParseTemplateHtml(string html, Element referenceElement)
        {
            if (object.IsNullOrUndefined(html) || html == "")
                return null;

            Document doc = referenceElement.OwnerDocument;
            Element container = doc.CreateElement("div");
            container.InnerHTML = html;

            if (container.ChildNodes.Length > 0)
                return (Element)container.ChildNodes[0];

            return null;
        }

        /// <summary>
        /// Removes all gate elements from a child graph state.
        /// Called when removing a collection item whose child graph contains gates.
        /// Gate elements are rendered as siblings of the item element, so they must
        /// be explicitly removed when the item is removed.
        /// </summary>
        public static void RemoveChildGateElements(GraphState childState)
        {
            if (object.IsNullOrUndefined(childState.GateElements))
                return;

            for (int i = 0; i < childState.GateElements.Length; i++)
            {
                Element gateElem = (Element)childState.GateElements[i];
                if (!object.IsNullOrUndefined(gateElem))
                {
                    gateElem.Remove();
                    childState.GateElements[i] = null;
                }
            }
        }

        /// <summary>
        /// After a gate renders a branch template, resolves child elem refs from the
        /// rendered DOM and updates state.ElemRefs. Elements inside gate branches don't
        /// exist in the static HTML — they're created dynamically when the gate renders.
        /// Uses CollectSpanElements to find marker spans in the rendered template.
        /// </summary>
        public static void ResolveGateChildElems(
            GraphState state, GateTargetInfo gateInfo, Element clone, bool gateIsOpen)
        {
            NativeArray<int> childIndices = gateIsOpen
                ? gateInfo.TrueChildElemIndices
                : gateInfo.FalseChildElemIndices;

            if (object.IsNullOrUndefined(childIndices) || childIndices.Length == 0)
                return;

            NativeArray spans = GraphEngine.CollectSpanElements(clone);
            int spanCount = spans.Length;

            for (int k = 0; k < childIndices.Length; k++)
            {
                if (k < spanCount)
                {
                    state.ElemRefs[childIndices[k]] = spans[k];
                }
            }
        }

        /// <summary>
        /// Clears child elem refs when a gate switches branches or closes.
        /// The old branch's elements are being removed from DOM, so their refs become stale.
        /// </summary>
        public static void ClearGateChildElems(
            GraphState state, GateTargetInfo gateInfo, bool wasOpen)
        {
            NativeArray<int> childIndices = wasOpen
                ? gateInfo.TrueChildElemIndices
                : gateInfo.FalseChildElemIndices;

            if (object.IsNullOrUndefined(childIndices) || childIndices.Length == 0)
                return;

            for (int k = 0; k < childIndices.Length; k++)
            {
                state.ElemRefs[childIndices[k]] = null;
            }
        }
    }
}
