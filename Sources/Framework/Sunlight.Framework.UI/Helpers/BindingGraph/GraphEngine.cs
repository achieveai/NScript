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
                // A gate node must never skip itself — it is the controller, not a gated child.
                if (gateIdx >= 0 && gateIdx != i && !state.GateOpen[gateIdx])
                {
                    state.Values[i] = desc.DefaultValues[i];
                    continue;
                }
                if (gateIdx <= -2 && state.GateOpen[-(gateIdx + 2)])
                {
                    state.Values[i] = desc.DefaultValues[i];
                    continue;
                }

                // Evaluate the node value using the shared dispatch method.
                object nodeVal = GraphEngine.EvaluateNodeValue(desc, state, i, nodeType);
                object previousValue = state.Values[i];
                state.Values[i] = nodeVal;

                // Side effects per node type (DOM writes, gate swaps, event wiring, collection rendering).
                if (nodeType == GraphNodeType.Gate)
                {
                    bool gateIsOpen = GraphEngine.IsTruthyValue(nodeVal);
                    bool wasOpen = state.GateOpen[i];
                    state.GateOpen[i] = gateIsOpen;

                    // DOM operations: clone and insert the appropriate template branch.
                    GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(gateInfo))
                    {
                        Element existing = (Element)state.GateElements[i];
                        if (object.IsNullOrUndefined(existing) || gateIsOpen != wasOpen)
                        {
                            if (!object.IsNullOrUndefined(existing))
                            {
                                GraphEngine.DisposeGateSubControls(desc, state, gateInfo, wasOpen);
                                GraphEngine.ClearGateChildElems(state, gateInfo, wasOpen);
                                existing.Remove();
                                state.GateElements[i] = null;
                            }

                            Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];
                            object templateObj = gateIsOpen
                                ? gateInfo.TrueTemplate
                                : gateInfo.FalseTemplate;

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

                                    GraphEngine.ResolveGateChildElems(
                                        state, gateInfo, clone, gateIsOpen);
                                    GraphEngine.CreateSubControls(desc, state, clone,
                                        GraphEngine.GetSubControlDataContext(state));
                                }
                            }
                        }
                    }

                    if (!gateIsOpen && wasOpen)
                    {
                        GraphEngine.ApplyGateClosure(desc, state, i);
                    }
                }
                else if (nodeType == GraphNodeType.DomTarget)
                {
                    DomTargetInfo targetInfo = (DomTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(targetInfo))
                    {
                        object elem = state.ElemRefs[targetInfo.ElemIdx];
                        if (!object.IsNullOrUndefined(targetInfo.Setter) && !object.IsNullOrUndefined(elem))
                        {
                            object val = !object.IsNullOrUndefined(nodeVal)
                                ? nodeVal
                                : desc.DefaultValues[i];
                            targetInfo.Setter(elem, val);
                        }
                    }
                }
                else if (nodeType == GraphNodeType.EventBinding)
                {
                    EventTargetInfo evtInfo = (EventTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(evtInfo) && !object.IsNullOrUndefined(nodeVal))
                    {
                        Element evtElem = GraphEngine.GetEventElement(state, evtInfo.ElemIdx);
                        if (!object.IsNullOrUndefined(evtElem))
                        {
                            Action<Element, ElementEvent> handler = (Action<Element, ElementEvent>)nodeVal;
                            evtElem.Bind(evtInfo.EventName, handler);
                            state.EventListeners[i] = handler;
                        }
                    }
                }
                else if (nodeType == GraphNodeType.CollectionManager)
                {
                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo))
                    {
                        if (nodeVal == previousValue
                            && !object.IsNullOrUndefined(state.ItemElements[i]))
                        {
                            GraphEngine.ReplayPendingCollectionChanges(desc, state, i, colInfo);
                            GraphEngine.RefreshCollectionItems(state, i);
                        }
                        else
                        {
                            state.PendingCollectionChanges[i] = null;
                            if (!object.IsNullOrUndefined(previousValue))
                                GraphEngine.DetachCollectionListener(state, i, previousValue);
                            GraphEngine.ClearCollectionItems(desc, state, i, colInfo);
                            if (!object.IsNullOrUndefined(nodeVal))
                            {
                                IObservableCollection obsCol = GraphEngine.AsObservableCollection(nodeVal);
                                if (!object.IsNullOrUndefined(obsCol))
                                    GraphEngine.RenderCollection(desc, state, i, colInfo, obsCol);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dirty-node flush in topological order. Repeats when a setter dirties a
        /// node that the current pass has already visited.
        /// Implements flip-flop elimination via reference equality comparison.
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        public static void Flush(GraphDescriptor desc, GraphState state)
        {
            if (state.Suspended) return;
            // The outer flush drains changes raised by setters. A re-entrant call
            // only marks nodes dirty; it must not start a nested scan.
            if (state.Flushing) return;
            state.Flushing = true;
            state.FlushScheduled = false;

            try
            {
                int n = desc.NodeCount;
                int passCount = 0;
                while (true)
                {
                for (int i = 0; i < n; i++)
                {
                    // Skip clean nodes.
                    if (!state.Dirty[i])
                    {
                        continue;
                    }

                    // Skip gated nodes whose gate is closed (or inverted gate that is open).
                    // A gate node must never skip itself — it is the controller, not a gated child.
                    int gateIdx = desc.GateIndices[i];
                    if (gateIdx >= 0 && gateIdx != i && !state.GateOpen[gateIdx])
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

                    // Evaluate the new value using the shared dispatch method.
                    object newVal = GraphEngine.EvaluateNodeValue(desc, state, i, nodeType);

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
                                GraphEngine.DisposeGateSubControls(desc, state, gateInfo, wasOpen);
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
                                        GraphEngine.CreateSubControls(desc, state, clone,
                                            GraphEngine.GetSubControlDataContext(state));
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
                            Element evtElem = GraphEngine.GetEventElement(state, evtInfo.ElemIdx);
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
                GraphEngine.ApplySubControlBindings(desc, state);
                if (state.SubControlsActive)
                    GraphEngine.ActivateSubControls(desc, state);

                bool hasDirtyNodes = false;
                for (int i = 0; i < n; i++)
                    if (state.Dirty[i])
                    {
                        hasDirtyNodes = true;
                        break;
                    }
                if (!hasDirtyNodes) break;
                if (++passCount >= 100)
                    throw new Exception("Binding graph did not settle after 100 passes.");
                }
            }
            finally
            {
                state.Flushing = false;
            }
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
        /// Evaluates the new value for a node based on its type.
        /// Shared by PushInitialValues and Flush to eliminate duplicate dispatch logic.
        /// Does NOT perform side effects (DOM writes, gate swaps, event wiring) —
        /// those remain in each caller since they differ between initial push and flush.
        /// </summary>
        public static object EvaluateNodeValue(GraphDescriptor desc, GraphState state, int i, int nodeType)
        {
            if (nodeType == GraphNodeType.Source)
            {
                object sourceVal = state.Sources[desc.RootSourceSlot];
                if (!object.IsNullOrUndefined(sourceVal)
                    && !object.IsNullOrUndefined(desc.SourceType)
                    && !desc.SourceType.IsInstanceOfType(sourceVal))
                {
                    sourceVal = null;
                }
                return sourceVal;
            }
            else if (nodeType == GraphNodeType.Property
                || nodeType == GraphNodeType.Computed
                || nodeType == GraphNodeType.TypeGuard)
            {
                object parentVal = GraphEngine.FindParentValue(desc, state, i);
                Func<object, object, object> getter = desc.Getters[i];
                object templateParent = state.Sources[GraphSourceSlot.TemplateParent];
                bool readsTemplateParent = !object.IsNullOrUndefined(desc.GetterSourceSlots)
                    && desc.GetterSourceSlots[i] == GraphSourceSlot.TemplateParent;
                if ((!object.IsNullOrUndefined(parentVal)
                    || (readsTemplateParent && !object.IsNullOrUndefined(templateParent)))
                    && !object.IsNullOrUndefined(getter))
                {
                    return getter(parentVal, templateParent);
                }
                return null;
            }
            else if (nodeType == GraphNodeType.Gate)
            {
                object parentVal = GraphEngine.FindParentValue(desc, state, i);
                Func<object, object, object> getter = desc.Getters[i];
                if (!object.IsNullOrUndefined(parentVal))
                {
                    if (!object.IsNullOrUndefined(getter))
                        return getter(parentVal, state.Sources[GraphSourceSlot.TemplateParent]);
                    else
                        return parentVal;
                }
                return null;
            }
            else if (nodeType == GraphNodeType.DomTarget)
            {
                return GraphEngine.FindParentValue(desc, state, i);
            }
            else if (nodeType == GraphNodeType.EventBinding)
            {
                object evtParent = GraphEngine.FindParentValue(desc, state, i);
                Func<object, object, object> evtGetter = desc.Getters[i];
                object templateParent = state.Sources[GraphSourceSlot.TemplateParent];
                bool readsTemplateParent = !object.IsNullOrUndefined(desc.GetterSourceSlots)
                    && desc.GetterSourceSlots[i] == GraphSourceSlot.TemplateParent;
                if ((!object.IsNullOrUndefined(evtParent)
                    || (readsTemplateParent && !object.IsNullOrUndefined(templateParent)))
                    && !object.IsNullOrUndefined(evtGetter))
                    return evtGetter(evtParent, templateParent);
                return evtParent;
            }
            else if (nodeType == GraphNodeType.CollectionManager)
            {
                object colParent = GraphEngine.FindParentValue(desc, state, i);
                Func<object, object, object> colGetter = desc.Getters[i];
                if (!object.IsNullOrUndefined(colParent) && !object.IsNullOrUndefined(colGetter))
                    return colGetter(colParent, state.Sources[GraphSourceSlot.TemplateParent]);
                return colParent;
            }

            return null;
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

            // Fallback: scan consumers adjacency list.
            // WARNING: This is O(nodes * edges) — quadratic in the worst case.
            // ParentIndices should always be populated by the compiler.
            // This fallback exists only as a safety net for malformed descriptors.
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
                            Element elem = GraphEngine.GetEventElement(state, evtInfo.ElemIdx);
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
            state.CollectionChangesInProgress[nodeIdx] = true;
            GraphEngine.AttachCollectionListener(desc, state, nodeIdx, colInfo, collection);
            try
            {
                GraphEngine.RenderCollectionItems(desc, state, nodeIdx, colInfo, collection);
            }
            finally
            {
                state.CollectionChangesInProgress[nodeIdx] = false;
            }
            GraphEngine.ReplayPendingCollectionChanges(desc, state, nodeIdx, colInfo);
        }

        /// <summary>
        /// Renders all items in a collection without attaching a collection listener.
        /// Used by RenderCollection (which adds the listener) and by Reset handling
        /// (where the listener is already attached).
        /// </summary>
        public static void RenderCollectionItems(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, IObservableCollection collection)
        {
            Element marker = (Element)state.ElemRefs[colInfo.MarkerIdx];
            if (object.IsNullOrUndefined(marker)) return;

            Node parent = marker.ParentNode;
            if (object.IsNullOrUndefined(parent)) return;

            int count = collection.Count;
            NativeArray items = new NativeArray(count);
            for (int idx = 0; idx < count; idx++)
                items[idx] = collection[idx];
            NativeArray<GraphState> childStates = new NativeArray<GraphState>(count);
            NativeArray itemElems = new NativeArray(count);

            // Parse the template HTML once, then clone per item to avoid
            // repeated innerHTML parsing.
            Element templateElement = null;
            if (!object.IsNullOrUndefined(colInfo.ItemTemplate))
            {
                templateElement = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, marker);
            }
            if (object.IsNullOrUndefined(templateElement)) return;

            for (int idx = 0; idx < count; idx++)
            {
                object item = items[idx];

                Element clone = (Element)templateElement.CloneNode(true);

                parent.InsertBefore(clone, marker);
                itemElems[idx] = clone;

                // Create child graph if item graph descriptor exists.
                if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                {
                    childStates[idx] = GraphEngine.CreateCollectionItemGraph(state, colInfo, clone, item);
                }
            }

            state.ChildGraphStates[nodeIdx] = childStates;
            state.ItemElements[nodeIdx] = itemElems;
        }

        private static GraphState CreateCollectionItemGraph(
            GraphState parentState, CollectionTargetInfo colInfo, Element clone, object item)
        {
            NativeArray childElemRefs = GraphEngine.CollectSpanElements(clone);
            GraphEngine.ResolveEventElements(clone, childElemRefs);
            GraphEngine.ResolveBindElements(clone, childElemRefs);

            GraphState childState = new GraphState(colInfo.ItemGraph, childElemRefs, parentState.Depth + 1);
            NativeArray itemContext = new NativeArray(3);
            itemContext[0] = parentState.Sources[GraphSourceSlot.DataContext];
            itemContext[1] = parentState.Sources[GraphSourceSlot.TemplateParent];
            itemContext[2] = item;
            childState.Sources[GraphSourceSlot.DataContext] = itemContext;
            GraphEngine.CreateSubControls(colInfo.ItemGraph, childState, clone, item);

            // An item setter or Activate() can write to the item. Subscribe
            // before invoking either and drain those notifications afterward.
            childState.Flushing = true;
            try
            {
                GraphEngine.WireChildSubscriptions(colInfo.ItemGraph, childState);
                GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
                GraphEngine.ApplySubControlBindings(colInfo.ItemGraph, childState);
                if (parentState.SubControlsActive)
                    GraphEngine.ActivateSubControls(colInfo.ItemGraph, childState);
            }
            finally
            {
                childState.Flushing = false;
            }
            GraphEngine.Flush(colInfo.ItemGraph, childState);
            return childState;
        }

        private static void RefreshCollectionItems(GraphState state, int nodeIdx)
        {
            NativeArray<GraphState> children = state.ChildGraphStates[nodeIdx];
            if (object.IsNullOrUndefined(children)) return;

            for (int i = 0; i < children.Length; i++)
            {
                GraphState child = children[i];
                if (object.IsNullOrUndefined(child)) continue;

                NativeArray itemContext = (NativeArray)child.Sources[GraphSourceSlot.DataContext];
                bool sourceChanged = itemContext[0] != state.Sources[GraphSourceSlot.DataContext]
                    || itemContext[1] != state.Sources[GraphSourceSlot.TemplateParent];
                if (sourceChanged)
                    GraphEngine.UnwireChildSubscriptions(child.Descriptor, child);
                itemContext[0] = state.Sources[GraphSourceSlot.DataContext];
                itemContext[1] = state.Sources[GraphSourceSlot.TemplateParent];
                child.Suspended = false;
                child.Flushing = true;
                try
                {
                    if (sourceChanged)
                        GraphEngine.WireChildSubscriptions(child.Descriptor, child);
                    GraphEngine.SetDefaultSubControlDataContext(child.Descriptor, child, itemContext[2]);
                    GraphEngine.PushInitialValues(child.Descriptor, child);
                    GraphEngine.ApplySubControlBindings(child.Descriptor, child);
                }
                finally
                {
                    child.Flushing = false;
                }
                GraphEngine.Flush(child.Descriptor, child);
            }
        }

        private static void ReplayPendingCollectionChanges(
            GraphDescriptor desc, GraphState state, int nodeIdx, CollectionTargetInfo colInfo)
        {
            NativeArray<CollectionChangedEventArgs> pending = state.PendingCollectionChanges[nodeIdx];
            state.PendingCollectionChanges[nodeIdx] = null;
            if (object.IsNullOrUndefined(pending)) return;

            int resetIdx = GraphEngine.FindLastCollectionReset(pending);
            if (resetIdx >= 0)
            {
                GraphEngine.OnCollectionChanged(desc, state, nodeIdx, colInfo, pending[resetIdx]);
                return;
            }
            for (int i = 0; i < pending.Length; i++)
                GraphEngine.OnCollectionChanged(desc, state, nodeIdx, colInfo, pending[i]);
        }

        private static int FindLastCollectionReset(NativeArray<CollectionChangedEventArgs> pending)
        {
            for (int i = pending.Length - 1; i >= 0; i--)
                if (pending[i].Action == CollectionChangedAction.Reset)
                    return i;
            return -1;
        }

        /// <summary>
        /// Creates the controls whose host elements currently exist in this graph.
        /// Conditional hosts are created later when their branch enters the DOM.
        /// </summary>
        public static void CreateSubControls(
            GraphDescriptor desc, GraphState state, Element root, object defaultDataContext)
        {
            NativeArray<SubControlInfo> infos = desc.SubControls;
            if (object.IsNullOrUndefined(infos) || object.IsNullOrUndefined(root)) return;

            for (int i = 0; i < infos.Length; i++)
            {
                SubControlInfo info = infos[i];
                if (object.IsNullOrUndefined(info) || object.IsNullOrUndefined(info.TypeFactory)) continue;
                if (!object.IsNullOrUndefined(state.SubControlInstances[i])) continue;

                Element marker = GraphEngine.FindSubControlMarker(root, info.MarkerIdx);
                if (object.IsNullOrUndefined(marker)) continue;

                UIElement control = (UIElement)info.TypeFactory(marker);
                UISkinableElement skinable = control as UISkinableElement;
                if (!object.IsNullOrUndefined(skinable)
                    && !object.IsNullOrUndefined(info.SkinFactory))
                    skinable.Skin = (Skin)info.SkinFactory();

                if (!info.HasDataContextBinding)
                    control.DataContext = defaultDataContext;

                state.SubControlInstances[i] = control;
                state.ElemRefs[info.ElemIdx] = control;
                state.SubControlAppliedValues[i] = null;
                state.SubControlBindingsInitialized[i] = false;
            }
        }

        public static void SetDefaultSubControlDataContext(
            GraphDescriptor desc, GraphState state, object dataContext)
        {
            NativeArray<SubControlInfo> infos = desc.SubControls;
            if (object.IsNullOrUndefined(infos)) return;
            for (int i = 0; i < infos.Length; i++)
            {
                UIElement control = state.SubControlInstances[i];
                if (!object.IsNullOrUndefined(control) && !infos[i].HasDataContextBinding)
                    control.DataContext = dataContext;
            }
        }

        public static void ApplySubControlBindings(GraphDescriptor desc, GraphState state)
        {
            NativeArray<SubControlInfo> infos = desc.SubControls;
            if (object.IsNullOrUndefined(infos)) return;
            for (int i = 0; i < infos.Length; i++)
            {
                UIElement control = state.SubControlInstances[i];
                if (object.IsNullOrUndefined(control)) continue;
                NativeArray<SubControlPropertyInfo> bindings = infos[i].Bindings;
                if (object.IsNullOrUndefined(bindings)) continue;
                NativeArray appliedValues = state.SubControlAppliedValues[i];
                if (object.IsNullOrUndefined(appliedValues))
                {
                    appliedValues = new NativeArray(bindings.Length);
                    state.SubControlAppliedValues[i] = appliedValues;
                }
                bool initialized = state.SubControlBindingsInitialized[i];
                for (int j = 0; j < bindings.Length; j++)
                {
                    SubControlPropertyInfo binding = bindings[j];
                    if (object.IsNullOrUndefined(binding.Setter)) continue;
                    object value = state.Values[binding.NodeIdx];
                    if (initialized && object.Equals(appliedValues[j], value)) continue;
                    binding.Setter(control, value);
                    appliedValues[j] = value;
                }
                state.SubControlBindingsInitialized[i] = true;
            }
        }

        public static void ActivateSubControls(GraphDescriptor desc, GraphState state)
        {
            state.Suspended = false;
            state.SubControlsActive = true;
            NativeArray<UIElement> controls = state.SubControlInstances;
            if (!object.IsNullOrUndefined(controls))
                for (int i = 0; i < controls.Length; i++)
                    if (!object.IsNullOrUndefined(controls[i]) && !controls[i].IsActive)
                    {
                        GraphEngine.WireSubControlPropertyListeners(desc, state, i);
                        controls[i].Activate();
                    }

            if (object.IsNullOrUndefined(state.ChildGraphStates)) return;
            for (int node = 0; node < state.ChildGraphStates.Length; node++)
            {
                NativeArray<GraphState> children = state.ChildGraphStates[node];
                if (object.IsNullOrUndefined(children)) continue;
                for (int i = 0; i < children.Length; i++)
                    if (!object.IsNullOrUndefined(children[i]))
                        GraphEngine.ActivateSubControls(children[i].Descriptor, children[i]);
            }
        }

        public static void DeactivateSubControls(GraphDescriptor desc, GraphState state)
        {
            state.Suspended = true;
            state.SubControlsActive = false;
            GraphEngine.CleanupEventListeners(desc, state);
            NativeArray<UIElement> controls = state.SubControlInstances;
            if (!object.IsNullOrUndefined(controls))
                for (int i = 0; i < controls.Length; i++)
                    if (!object.IsNullOrUndefined(controls[i]))
                    {
                        GraphEngine.UnwireSubControlPropertyListeners(desc, state, i);
                        controls[i].Deactivate();
                    }

            if (object.IsNullOrUndefined(state.ChildGraphStates)) return;
            for (int node = 0; node < state.ChildGraphStates.Length; node++)
            {
                NativeArray<GraphState> children = state.ChildGraphStates[node];
                if (object.IsNullOrUndefined(children)) continue;
                for (int i = 0; i < children.Length; i++)
                    if (!object.IsNullOrUndefined(children[i]))
                        GraphEngine.DeactivateSubControls(children[i].Descriptor, children[i]);
            }
        }

        public static void DisposeSubControls(GraphDescriptor desc, GraphState state)
        {
            state.SubControlsActive = false;
            NativeArray<UIElement> controls = state.SubControlInstances;
            if (object.IsNullOrUndefined(controls)) return;
            NativeArray<SubControlInfo> infos = desc.SubControls;
            for (int i = 0; i < controls.Length; i++)
            {
                UIElement control = controls[i];
                if (object.IsNullOrUndefined(control)) continue;
                GraphEngine.UnwireSubControlPropertyListeners(desc, state, i);
                control.Deactivate();
                control.Dispose();
                state.ElemRefs[infos[i].ElemIdx] = null;
                controls[i] = null;
                state.SubControlAppliedValues[i] = null;
                state.SubControlBindingsInitialized[i] = false;
            }
        }

        private static void WireSubControlPropertyListeners(
            GraphDescriptor desc, GraphState state, int controlIndex)
        {
            SubControlInfo info = desc.SubControls[controlIndex];
            NativeArray<SubControlPropertyInfo> bindings = info.Bindings;
            if (object.IsNullOrUndefined(bindings)) return;

            NativeArray listeners = state.SubControlPropertyListeners[controlIndex];
            if (object.IsNullOrUndefined(listeners))
            {
                listeners = new NativeArray(bindings.Length);
                state.SubControlPropertyListeners[controlIndex] = listeners;
            }

            UIElement control = state.SubControlInstances[controlIndex];
            for (int i = 0; i < bindings.Length; i++)
            {
                SubControlPropertyInfo binding = bindings[i];
                if (object.IsNullOrUndefined(binding.SourceSetter)
                    || object.IsNullOrUndefined(binding.TargetGetter)
                    || !object.IsNullOrUndefined(listeners[i])) continue;

                Action<INotifyPropertyChanged, string> callback =
                    GraphEngine.CreateSubControlPropertyCallback(state, control, binding);
                control.AddPropertyChangedListener(binding.TargetPropertyName, callback);
                listeners[i] = callback;
            }
        }

        private static Action<INotifyPropertyChanged, string> CreateSubControlPropertyCallback(
            GraphState state, UIElement control, SubControlPropertyInfo binding)
        {
            return delegate(INotifyPropertyChanged sender, string propertyName)
            {
                object value = binding.TargetGetter(control);
                object source = state.Sources[GraphSourceSlot.DataContext];
                if (!object.IsNullOrUndefined(source)
                    && !object.Equals(state.Values[binding.NodeIdx], value))
                    binding.SourceSetter(source, value);
            };
        }

        private static void UnwireSubControlPropertyListeners(
            GraphDescriptor desc, GraphState state, int controlIndex)
        {
            NativeArray listeners = state.SubControlPropertyListeners[controlIndex];
            if (object.IsNullOrUndefined(listeners)) return;
            UIElement control = state.SubControlInstances[controlIndex];
            NativeArray<SubControlPropertyInfo> bindings = desc.SubControls[controlIndex].Bindings;
            for (int i = 0; i < listeners.Length; i++)
            {
                Action<INotifyPropertyChanged, string> callback =
                    (Action<INotifyPropertyChanged, string>)listeners[i];
                if (object.IsNullOrUndefined(callback)) continue;
                control.RemovePropertyChangedListener(bindings[i].TargetPropertyName, callback);
                listeners[i] = null;
            }
        }

        private static object GetSubControlDataContext(GraphState state)
        {
            object dataContext = state.Sources[GraphSourceSlot.DataContext];
            if (state.Depth == 0) return dataContext;

            NativeArray itemContext = dataContext as NativeArray;
            return !object.IsNullOrUndefined(itemContext) && itemContext.Length >= 3
                ? itemContext[2]
                : dataContext;
        }

        private static Element GetEventElement(GraphState state, int elemIdx)
        {
            object target = state.ElemRefs[elemIdx];
            UIElement control = target as UIElement;
            if (!object.IsNullOrUndefined(control)) return control.Element;
            return target as Element;
        }

        private static void DisposeGateSubControls(
            GraphDescriptor desc, GraphState state, GateTargetInfo gateInfo, bool wasOpen)
        {
            NativeArray<int> childIndices = wasOpen
                ? gateInfo.TrueChildElemIndices
                : gateInfo.FalseChildElemIndices;
            NativeArray<SubControlInfo> infos = desc.SubControls;
            if (object.IsNullOrUndefined(childIndices) || object.IsNullOrUndefined(infos)) return;

            for (int i = 0; i < infos.Length; i++)
            {
                UIElement control = state.SubControlInstances[i];
                if (object.IsNullOrUndefined(control)) continue;
                for (int j = 0; j < childIndices.Length; j++)
                {
                    if (infos[i].ElemIdx != childIndices[j]) continue;
                    for (int node = 0; node < desc.NodeCount; node++)
                    {
                        if (desc.NodeTypes[node] != GraphNodeType.EventBinding) continue;
                        EventTargetInfo eventInfo = (EventTargetInfo)desc.TargetInfos[node];
                        if (object.IsNullOrUndefined(eventInfo)
                            || eventInfo.ElemIdx != infos[i].ElemIdx) continue;
                        Action<Element, ElementEvent> listener =
                            (Action<Element, ElementEvent>)state.EventListeners[node];
                        if (!object.IsNullOrUndefined(listener))
                        {
                            control.Element.UnBind(eventInfo.EventName, listener);
                            state.EventListeners[node] = null;
                        }
                    }
                    GraphEngine.UnwireSubControlPropertyListeners(desc, state, i);
                    control.Deactivate();
                    control.Dispose();
                    state.SubControlInstances[i] = null;
                    state.ElemRefs[infos[i].ElemIdx] = null;
                    state.SubControlAppliedValues[i] = null;
                    state.SubControlBindingsInitialized[i] = false;
                    break;
                }
            }
        }

        private static Element FindSubControlMarker(Element itemRoot, int index)
        {
            // Check if the itemRoot itself is the marker (happens when the item
            // template consists solely of a sub-control placeholder).
            string selector = "[data-ns-subctl=\"" + index + "\"]";
            if (itemRoot.Matches(selector))
                return itemRoot;
            return itemRoot.QuerySelector(selector);
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
                        GraphEngine.DisposeSubControls(itemDesc, child);
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
            if (state.Suspended || state.CollectionChangesInProgress[nodeIdx])
            {
                NativeArray<CollectionChangedEventArgs> pending = state.PendingCollectionChanges[nodeIdx];
                if (object.IsNullOrUndefined(pending))
                {
                    pending = new NativeArray<CollectionChangedEventArgs>(0);
                    state.PendingCollectionChanges[nodeIdx] = pending;
                }
                pending.Push(args);
                return;
            }

            state.CollectionChangesInProgress[nodeIdx] = true;
            try
            {
                GraphEngine.ApplyCollectionChange(desc, state, nodeIdx, colInfo, args);
                NativeArray<CollectionChangedEventArgs> pending = state.PendingCollectionChanges[nodeIdx];
                state.PendingCollectionChanges[nodeIdx] = null;
                while (!object.IsNullOrUndefined(pending))
                {
                    int resetIdx = GraphEngine.FindLastCollectionReset(pending);
                    if (resetIdx >= 0)
                        GraphEngine.ApplyCollectionChange(desc, state, nodeIdx, colInfo, pending[resetIdx]);
                    else
                        for (int i = 0; i < pending.Length; i++)
                            GraphEngine.ApplyCollectionChange(desc, state, nodeIdx, colInfo, pending[i]);
                    pending = state.PendingCollectionChanges[nodeIdx];
                    state.PendingCollectionChanges[nodeIdx] = null;
                }
            }
            finally
            {
                state.CollectionChangesInProgress[nodeIdx] = false;
            }
        }

        private static void ApplyCollectionChange(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, CollectionChangedEventArgs args)
        {

            Element marker = (Element)state.ElemRefs[colInfo.MarkerIdx];
            if (object.IsNullOrUndefined(marker)) return;

            Node parent = marker.ParentNode;
            if (object.IsNullOrUndefined(parent)) return;

            if (args.Action == CollectionChangedAction.Add)
            {
                GraphEngine.HandleCollectionAdd(state, nodeIdx, colInfo, args, marker, parent);
            }
            else if (args.Action == CollectionChangedAction.Remove)
            {
                GraphEngine.HandleCollectionRemove(state, nodeIdx, colInfo, args);
            }
            else if (args.Action == CollectionChangedAction.Replace)
            {
                GraphEngine.HandleCollectionReplace(state, nodeIdx, colInfo, args, marker, parent);
            }
            else if (args.Action == CollectionChangedAction.Reset)
            {
                // Full reset: clear and re-render items without re-attaching the collection listener
                // (we're already inside the listener callback).
                GraphEngine.ClearCollectionItems(desc, state, nodeIdx, colInfo);
                object collection = state.Values[nodeIdx];
                if (!object.IsNullOrUndefined(collection))
                {
                    IObservableCollection obsCol = GraphEngine.AsObservableCollection(collection);
                    if (!object.IsNullOrUndefined(obsCol))
                        GraphEngine.RenderCollectionItems(desc, state, nodeIdx, colInfo, obsCol);
                }
            }
        }

        /// <summary>
        /// Handles CollectionChangedAction.Add: inserts new items at the specified index.
        /// </summary>
        private static void HandleCollectionAdd(
            GraphState state, int nodeIdx, CollectionTargetInfo colInfo,
            CollectionChangedEventArgs args, Element marker, Node parent)
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

            // Parse template once, then clone per item.
            Element templateElement = null;
            if (!object.IsNullOrUndefined(colInfo.ItemTemplate))
            {
                templateElement = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, marker);
            }
            if (object.IsNullOrUndefined(templateElement)) return;

            // Insert new items.
            for (int j = 0; j < addCount; j++)
            {
                object item = newItems[j];
                Element clone = (Element)templateElement.CloneNode(true);
                parent.InsertBefore(clone, refNode);
                newItemElems[insertIdx + j] = clone;

                if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                {
                    newChildStates[insertIdx + j] = GraphEngine.CreateCollectionItemGraph(
                        state, colInfo, clone, item);
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

        /// <summary>
        /// Handles CollectionChangedAction.Remove: removes items at the specified index.
        /// </summary>
        private static void HandleCollectionRemove(
            GraphState state, int nodeIdx, CollectionTargetInfo colInfo,
            CollectionChangedEventArgs args)
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
                    if (!object.IsNullOrUndefined(childState) && !object.IsNullOrUndefined(colInfo.ItemGraph))
                    {
                        // Dispose child graph state: unwire subscriptions, events,
                        // nested collections, and remove gate DOM elements.
                        GraphEngine.UnwireChildSubscriptions(colInfo.ItemGraph, childState);
                        GraphEngine.CleanupEventListeners(colInfo.ItemGraph, childState);
                        GraphEngine.CleanupCollectionListeners(colInfo.ItemGraph, childState);
                        GraphEngine.DisposeSubControls(colInfo.ItemGraph, childState);
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

        /// <summary>
        /// Handles CollectionChangedAction.Replace: replaces item at the specified index.
        /// </summary>
        private static void HandleCollectionReplace(
            GraphState state, int nodeIdx, CollectionTargetInfo colInfo,
            CollectionChangedEventArgs args, Element marker, Node parent)
        {
            // Reuse the remove/add paths so a replacement gets a new graph and
            // control while unaffected items retain their existing instances.
            GraphEngine.HandleCollectionRemove(state, nodeIdx, colInfo, args);
            GraphEngine.HandleCollectionAdd(state, nodeIdx, colInfo, args, marker, parent);
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
                            Element elem = GraphEngine.GetEventElement(state, evtInfo.ElemIdx);
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
        public static void WireChildSubscriptions(GraphDescriptor desc, GraphState childState)
        {
            if (childState.SubscriptionsActive) return;

            NativeArray subscriptions = desc.Subscriptions;
            if (object.IsNullOrUndefined(subscriptions) || subscriptions.Length == 0) return;

            int subCount = subscriptions.Length;
            NativeArray listeners = new NativeArray(subCount);

            for (int i = 0; i < subCount; i++)
            {
                SubscriptionEntry entry = (SubscriptionEntry)subscriptions[i];
                if (object.IsNullOrUndefined(entry)) continue;

                if (GraphEngine.IsChainedEntry(entry))
                {
                    if (object.IsNullOrUndefined(childState.ChainListeners))
                        childState.ChainListeners =
                            new NativeArray<NativeArray<ChainListenerHandle>>(subCount);
                    GraphEngine.WireChainedSubscription(childState, entry, i);
                    listeners[i] = null;
                    continue;
                }

                object source = GraphEngine.GetCollectionItemSource(childState, entry.SourceSlot);
                INotifyPropertyChanged observable = source as INotifyPropertyChanged;
                if (object.IsNullOrUndefined(observable)) continue;

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

                if (GraphEngine.IsChainedEntry(entry))
                {
                    GraphEngine.UnwireChainedSubscription(childState, i);
                    continue;
                }

                object source = GraphEngine.GetCollectionItemSource(childState, entry.SourceSlot);
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
        /// True when a subscription entry describes a chained property path (e.g. Model.Child.Leaf)
        /// that must listen on every object along the path, not just the root.
        /// </summary>
        public static bool IsChainedEntry(SubscriptionEntry entry)
        {
            return !object.IsNullOrUndefined(entry.PathSegments) && entry.PathSegments.Length > 1;
        }

        /// <summary>
        /// Wires PropertyChanged listeners for every hop of a chained property path. Entry
        /// <c>ChainParentGetters[k]</c> yields the object that owns <c>PathSegments[k]</c>, and a
        /// listener is attached to each for its segment. A change at any level re-targets the
        /// deeper listeners and marks the consuming node dirty. Handles are stored in
        /// <c>state.ChainListeners[entryIndex]</c> for cleanup. Wiring stops at the first null
        /// hop, so a null DataContext or a null mid-path object wires only the live prefix.
        /// </summary>
        public static void WireChainedSubscription(
            GraphState state, SubscriptionEntry entry, int entryIndex)
        {
            NativeArray<string> segments = entry.PathSegments;
            if (object.IsNullOrUndefined(segments)) return;

            NativeArray<ChainListenerHandle> handles = new NativeArray<ChainListenerHandle>(0);
            Action<INotifyPropertyChanged, string> callback =
                GraphEngine.CreateChainCallback(state, entry, entryIndex);

            object dc = state.Sources[GraphSourceSlot.DataContext];
            object tp = state.Sources[GraphSourceSlot.TemplateParent];
            NativeArray<Func<object, object, object>> owners = entry.ChainParentGetters;

            int count = segments.Length;
            object owner = null;
            for (int k = 0; k < count; k++)
            {
                // Owner k is root.PathSegments[0..k-1]. When owner k-1 is null/undefined — a null
                // DataContext (e.g. the skin was handed a foreign view-model type) or a mid-path
                // object that is not set yet — owner k cannot exist and its getter would read a
                // property of null. Stop at the last live hop; a change there re-wires the rest.
                if (k > 0 && object.IsNullOrUndefined(owner)) break;

                owner = null;
                if (!object.IsNullOrUndefined(owners) && k < owners.Length)
                {
                    Func<object, object, object> getOwner = owners[k];
                    if (!object.IsNullOrUndefined(getOwner))
                        owner = getOwner(dc, tp);
                }
                GraphEngine.AttachChainListener(handles, owner, segments[k], callback);
            }

            state.ChainListeners[entryIndex] = handles;
        }

        /// <summary>
        /// Attaches one PropertyChanged listener for a chained-path hop and records a handle so it
        /// can be removed later. No-op when the owner is not observable (e.g. a null mid-path value).
        /// </summary>
        private static void AttachChainListener(
            NativeArray<ChainListenerHandle> handles, object owner, string propertyName,
            Action<INotifyPropertyChanged, string> callback)
        {
            INotifyPropertyChanged observable = owner as INotifyPropertyChanged;
            if (object.IsNullOrUndefined(observable)) return;

            observable.AddPropertyChangedListener(propertyName, callback);

            ChainListenerHandle handle = new ChainListenerHandle();
            handle.Observable = observable;
            handle.PropertyName = propertyName;
            handle.Callback = callback;
            handles.Push(handle);
        }

        /// <summary>
        /// Removes all PropertyChanged listeners previously attached for a chained subscription and
        /// clears its handle list. Safe to call when the chain was never wired.
        /// </summary>
        public static void UnwireChainedSubscription(GraphState state, int entryIndex)
        {
            if (object.IsNullOrUndefined(state.ChainListeners)) return;
            NativeArray<ChainListenerHandle> handles = state.ChainListeners[entryIndex];
            if (object.IsNullOrUndefined(handles)) return;

            for (int k = 0; k < handles.Length; k++)
            {
                ChainListenerHandle handle = handles[k];
                if (object.IsNullOrUndefined(handle) || object.IsNullOrUndefined(handle.Observable))
                    continue;
                handle.Observable.RemovePropertyChangedListener(handle.PropertyName, handle.Callback);
            }

            state.ChainListeners[entryIndex] = null;
        }

        /// <summary>
        /// Creates the PropertyChanged callback for a chained subscription. On any hop change it
        /// re-wires the chain (so a replaced mid-path object moves the deeper listeners onto the new
        /// object), then marks the consuming node dirty and flushes. Built in a separate method so
        /// each entry gets its own closure scope (NScript compiles C# locals as function-scoped vars).
        /// </summary>
        private static Action<INotifyPropertyChanged, string> CreateChainCallback(
            GraphState state, SubscriptionEntry entry, int entryIndex)
        {
            return delegate(INotifyPropertyChanged sender, string propName)
            {
                if (state.Suspended) return;
                GraphEngine.UnwireChainedSubscription(state, entryIndex);
                GraphEngine.WireChainedSubscription(state, entry, entryIndex);
                state.Dirty[entry.NodeIdx] = true;
                GraphEngine.Flush(state.Descriptor, state);
            };
        }

        private static object GetCollectionItemSource(GraphState childState, int sourceSlot)
        {
            // An item graph stores its sources in [parent, control, item].
            NativeArray tuple = childState.Sources[GraphSourceSlot.DataContext] as NativeArray;
            return !object.IsNullOrUndefined(tuple) && sourceSlot < tuple.Length
                ? tuple[sourceSlot]
                : null;
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
        /// Collects compiler-generated marker elements from an item clone for use as
        /// element refs in child graph states. Markers are identified by data-ns-ph
        /// (text placeholders), data-ns-bind (value/attribute bindings), data-ns-evt
        /// (event bindings), or data-ns-subctl (child hosts). Returns each marker in DOM order.
        /// If no markers found, returns a single-element array containing the clone itself.
        /// </summary>
        public static NativeArray CollectSpanElements(Element clone)
        {
            // Collect only compiler-generated markers — elements carrying data-ns-ph,
            // data-ns-bind, data-ns-evt, or data-ns-subctl. User-authored spans
            // must NOT be collected as they are not counted by the compiler's element indexing.
            NativeArray<Element> allMarkers = clone.QuerySelectorAll("[data-ns-ph], [data-ns-bind], [data-ns-evt], [data-ns-subctl]");
            int count = allMarkers.Length;
            NativeArray result = new NativeArray(count > 0 ? count : 1);
            for (int i = 0; i < count; i++)
            {
                result[i] = allMarkers[i];
            }
            if (count == 0)
            {
                result[0] = clone;
            }
            return result;
        }

        public static void ResolveEventElements(Element clone, NativeArray elemRefs)
        {
            NativeArray<Element> evtSpans = clone.QuerySelectorAll("[data-ns-evt]");
            if (object.IsNullOrUndefined(evtSpans) || evtSpans.Length == 0)
                return;

            for (int i = 0; i < evtSpans.Length; i++)
            {
                Element marker = evtSpans[i];
                for (int j = 0; j < elemRefs.Length; j++)
                {
                    if ((object)elemRefs[j] == (object)marker)
                    {
                        // Only replace span markers with their parent; non-span
                        // elements (e.g. <button data-ns-evt/>) bind to themselves.
                        if (marker.TagName == "SPAN")
                            elemRefs[j] = (Element)marker.ParentNode;
                        break;
                    }
                }
            }
        }

        public static void ResolveBindElements(Element clone, NativeArray elemRefs)
        {
            NativeArray<Element> bindSpans = clone.QuerySelectorAll("[data-ns-bind]");
            if (object.IsNullOrUndefined(bindSpans) || bindSpans.Length == 0)
                return;

            for (int i = 0; i < bindSpans.Length; i++)
            {
                Element marker = bindSpans[i];
                for (int j = 0; j < elemRefs.Length; j++)
                {
                    if ((object)elemRefs[j] == (object)marker)
                    {
                        // Only replace span markers with their parent; non-span
                        // elements (e.g. <input data-ns-bind/>) bind to themselves.
                        if (marker.TagName == "SPAN")
                            elemRefs[j] = (Element)marker.ParentNode;
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
            GraphEngine.ResolveEventElements(clone, spans);
            GraphEngine.ResolveBindElements(clone, spans);
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
