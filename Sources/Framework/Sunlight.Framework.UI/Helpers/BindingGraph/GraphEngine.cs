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
                // A skipped nested gate gets no value (not its default), so Flush renders
                // whichever branch it selects once its own branch opens (see ResetNestedGate).
                if ((gateIdx >= 0 && gateIdx != i && !state.GateOpen[gateIdx])
                    || (gateIdx <= -2 && state.GateOpen[-(gateIdx + 2)]))
                {
                    state.Values[i] = nodeType == GraphNodeType.Gate ? null : desc.DefaultValues[i];
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
                                GraphEngine.TearDownGateBranch(desc, state, gateInfo, i, wasOpen);
                            }

                            Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];
                            object templateObj = gateIsOpen
                                ? gateInfo.TrueTemplate
                                : gateInfo.FalseTemplate;

                            if (!object.IsNullOrUndefined(templateObj) && !object.IsNullOrUndefined(marker))
                            {
                                Element clone = GraphEngine.InsertGateBranch(state, i, (string)templateObj, marker);
                                if (!object.IsNullOrUndefined(clone))
                                {
                                    GraphEngine.ResolveGateChildElems(desc, state, gateInfo, i, gateIsOpen);
                                    GraphEngine.CreateGateBranchSubControls(desc, state, i);
                                    // Only after both scans above: they read the branch by marker
                                    // position and data-ns-subctl index and must not see caller content.
                                    GraphEngine.PlaceGateBranchProjections(state, i);
                                }
                            }
                        }
                    }

                    if (!gateIsOpen && wasOpen)
                    {
                        GraphEngine.ApplyGateClosure(desc, state, i);
                    }
                    else if (gateIsOpen && !wasOpen)
                    {
                        // Re-activation found the gate opened while inactive: reset the
                        // @else side (its nested gates included), as Flush does.
                        GraphEngine.ApplyGateClosure(desc, state, -(i + 2));
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

                            // A row whose Dispose throws still lets the new collection render;
                            // the first error then propagates.
                            Exception first = null;
                            try
                            {
                                GraphEngine.ClearCollectionItems(desc, state, i, colInfo);
                            }
                            catch (Exception ex)
                            {
                                first = ex;
                            }

                            try
                            {
                                if (!object.IsNullOrUndefined(nodeVal))
                                    GraphEngine.RenderCollectionSource(desc, state, i, colInfo, nodeVal);
                            }
                            catch (Exception ex)
                            {
                                if (first == null) first = ex;
                            }

                            if (first != null) throw first;
                        }
                    }
                }
            }

            GraphEngine.WireTwoWayValues(desc, state);
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

                        // A nested gate reset by ResetNestedGate has no value and no branch:
                        // render its current side even though GateOpen did not change.
                        bool firstRender = gateIsOpen == wasOpen
                            && object.IsNullOrUndefined(oldVal)
                            && object.IsNullOrUndefined(state.GateElements[i]);

                        // DOM swap: remove old branch, insert new branch.
                        if (gateIsOpen != wasOpen || firstRender)
                        {
                            GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[i];
                            if (!object.IsNullOrUndefined(gateInfo))
                            {
                                GraphEngine.TearDownGateBranch(desc, state, gateInfo, i, wasOpen);

                                // Insert new branch template.
                                object templateObj = gateIsOpen
                                    ? gateInfo.TrueTemplate
                                    : gateInfo.FalseTemplate;
                                Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];

                                if (!object.IsNullOrUndefined(templateObj) && !object.IsNullOrUndefined(marker))
                                {
                                    Element clone = GraphEngine.InsertGateBranch(state, i, (string)templateObj, marker);
                                    if (!object.IsNullOrUndefined(clone))
                                    {
                                        // Resolve child elem refs from the new branch.
                                        GraphEngine.ResolveGateChildElems(desc, state, gateInfo, i, gateIsOpen);
                                        GraphEngine.CreateGateBranchSubControls(desc, state, i);
                                        // Only after both scans above: they read the branch by marker
                                        // position and data-ns-subctl index and must not see caller content.
                                        GraphEngine.PlaceGateBranchProjections(state, i);
                                    }
                                }
                            }
                        }

                        int invertedRef = -(i + 2);

                        if (!gateIsOpen && wasOpen)
                        {
                            // Gate closed: reset true-branch, mark the false-branch dirty.
                            // If resetting throws, the @else nodes are still marked dirty,
                            // but the throw leaves this Flush pass: their text writes, events
                            // and sub-control activation wait for the next Flush.
                            try
                            {
                                GraphEngine.ApplyGateClosure(desc, state, i);
                            }
                            finally
                            {
                                GraphEngine.MarkGatedConsumersDirty(desc, state, invertedRef);
                            }
                        }
                        else if (gateIsOpen && !wasOpen)
                        {
                            // Gate opened: activate true-branch, reset false-branch
                            GraphEngine.MarkGatedConsumersDirty(desc, state, i);
                            GraphEngine.ApplyGateClosure(desc, state, invertedRef);
                        }
                        else if (firstRender && !gateIsOpen)
                        {
                            // The false side was evaluated while its branch was absent, so its
                            // cached values would skip the writes to the new branch: reset them.
                            // If resetting throws, they are still marked dirty, but the throw
                            // leaves this Flush pass, so their writes wait for the next Flush.
                            try
                            {
                                GraphEngine.ApplyGateClosure(desc, state, invertedRef);
                            }
                            finally
                            {
                                GraphEngine.MarkGatedConsumersDirty(desc, state, invertedRef);
                            }
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
                            // Detach the old collection's listener first. The node already
                            // holds the new collection, so a teardown that disposes the host
                            // would otherwise detach from the wrong collection and leave the
                            // old listener live.
                            Exception first = null;
                            try
                            {
                                if (!object.IsNullOrUndefined(oldVal))
                                    GraphEngine.DetachCollectionListener(state, i, oldVal);
                            }
                            catch (Exception ex)
                            {
                                first = ex;
                            }

                            // Clear old collection items. A row whose Dispose throws still
                            // lets the swap finish, so the old collection stops driving rows;
                            // the first error then propagates.
                            try
                            {
                                GraphEngine.ClearCollectionItems(desc, state, i, colInfo);
                            }
                            catch (Exception ex)
                            {
                                if (first == null) first = ex;
                            }

                            // A teardown that disposed the host ends the swap: a listener on
                            // the new collection would outlive the host's cleanup.
                            try
                            {
                                if (!state.Disposed && !object.IsNullOrUndefined(newVal))
                                    GraphEngine.RenderCollectionSource(desc, state, i, colInfo, newVal);
                            }
                            catch (Exception ex)
                            {
                                if (first == null) first = ex;
                            }

                            if (first != null) throw first;
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

            // A flush already running on this graph drains new dirty nodes in
            // its own settle loop; scheduling again would only add a no-op pass.
            if (state.Flushing) return;

            if (!state.FlushScheduled)
            {
                state.FlushScheduled = true;
                GraphFlushCoordinator.ScheduleDirty(state);
            }
        }

        /// <summary>
        /// Entry point for every PropertyChanged notification that targets a
        /// node. Batched mode defers to the coordinator; otherwise the graph
        /// flushes synchronously so the DOM is current before the setter returns.
        /// </summary>
        public static void NotifyDirty(GraphState state, int nodeIdx)
        {
            if (GraphFlushCoordinator.BatchingEnabled)
            {
                GraphEngine.MarkDirty(state, nodeIdx);
                return;
            }

            state.Dirty[nodeIdx] = true;
            GraphEngine.Flush(state.Descriptor, state);
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

            // Two-way value= elements leave with the branch, nested @if branches included:
            // whoever still holds one must not write the model. Reopening wires the new ones.
            GraphEngine.UnwireGatedTwoWayValues(desc, state, gateIdx);

            // A node that throws (a row whose Dispose throws) still lets every other gated
            // node reset; the first error then propagates.
            Exception first = null;
            for (int i = start; i < n; i++)
            {
                if (desc.GateIndices[i] != gateIdx)
                {
                    continue;
                }

                object closedValue = state.Values[i];
                state.Values[i] = desc.DefaultValues[i];
                state.Dirty[i] = false;

                try
                {
                    GraphEngine.CloseGatedNode(desc, state, i, closedValue);
                }
                catch (Exception ex)
                {
                    if (first == null) first = ex;
                }
            }

            if (first != null) throw first;
        }

        /// <summary>
        /// Undoes one gated node's side effects after its value was reset: writes the default
        /// to a DOM target, unbinds an event, detaches and clears a collection, or resets a
        /// nested gate.
        /// </summary>
        private static void CloseGatedNode(GraphDescriptor desc, GraphState state, int i, object closedValue)
        {
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
                // Values[i] was reset by the caller; the listener hangs off the collection it held.
                if (!object.IsNullOrUndefined(closedValue))
                {
                    GraphEngine.DetachCollectionListener(state, i, closedValue);
                }

                CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                if (!object.IsNullOrUndefined(colInfo))
                {
                    GraphEngine.ClearCollectionItems(desc, state, i, colInfo);
                }
            }
            else if (nodeType == GraphNodeType.Gate)
            {
                GraphEngine.ResetNestedGate(desc, state, i);
            }
        }

        /// <summary>
        /// Returns a nested @if to its never-rendered state when the branch holding it
        /// closes: removes its branch (which can sit outside the closing branch's own
        /// nodes), resets both of its sides recursively, and clears its value so the
        /// next evaluation renders a branch again, the @else side included.
        /// </summary>
        private static void ResetNestedGate(GraphDescriptor desc, GraphState state, int gateIdx)
        {
            bool wasOpen = state.GateOpen[gateIdx];
            GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[gateIdx];
            if (!object.IsNullOrUndefined(gateInfo))
            {
                GraphEngine.TearDownGateBranch(desc, state, gateInfo, gateIdx, wasOpen);
            }

            state.GateOpen[gateIdx] = false;
            // Both sides reset even when one throws; the first error then propagates.
            Exception first = null;
            try
            {
                GraphEngine.ApplyGateClosure(desc, state, gateIdx);
            }
            catch (Exception ex)
            {
                first = ex;
            }

            try
            {
                GraphEngine.ApplyGateClosure(desc, state, -(gateIdx + 2));
            }
            catch (Exception ex)
            {
                if (first == null) first = ex;
            }

            state.Values[gateIdx] = null;
            if (first != null) throw first;
        }

        /// <summary>
        /// Takes down the rendered branch of a gate: disposes its sub-controls, unbinds its
        /// events (before removal: an event marker after a void root resolves to the gate's
        /// parent, which outlives the branch), clears its elem refs and removes it.
        /// </summary>
        private static void TearDownGateBranch(
            GraphDescriptor desc, GraphState state, GateTargetInfo gateInfo, int gateIdx, bool wasOpen)
        {
            GraphEngine.DisposeGateSubControls(desc, state, gateInfo, wasOpen);
            GraphEngine.UnbindGatedEvents(desc, state, wasOpen ? gateIdx : -(gateIdx + 2));
            GraphEngine.ClearGateChildElems(state, gateInfo, wasOpen);
            GraphEngine.RemoveGateBranch(state, gateIdx);
        }

        /// <summary>
        /// Unwires the two-way listeners of the DomTarget nodes on one side of a gate
        /// (<paramref name="gateRef"/>: gate index, or -(gate + 2) for @else), and of both
        /// sides of every @if nested there: a nested node is gated by its own @if, not by
        /// the one that closed.
        /// </summary>
        private static void UnwireGatedTwoWayValues(GraphDescriptor desc, GraphState state, int gateRef)
        {
            if (object.IsNullOrUndefined(state.TwoWayListeners)) return;
            for (int i = 0; i < desc.NodeCount; i++)
            {
                // A gate node can carry its own index: it is the controller, not a nested @if.
                if (desc.GateIndices[i] != gateRef || i == gateRef) continue;
                if (desc.NodeTypes[i] == GraphNodeType.DomTarget)
                {
                    GraphEngine.UnwireTwoWayValue(state, i);
                }
                else if (desc.NodeTypes[i] == GraphNodeType.Gate)
                {
                    GraphEngine.UnwireGatedTwoWayValues(desc, state, i);
                    GraphEngine.UnwireGatedTwoWayValues(desc, state, -(i + 2));
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
        /// Renders a @foreach source. An observable collection renders and then tracks its
        /// changes. An array or other IList renders once (OneTime); a new instance assigned to
        /// the source property renders again. Anything else is logged, never silently empty.
        /// </summary>
        public static void RenderCollectionSource(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, object source)
        {
            IObservableCollection obsCol = GraphEngine.AsObservableCollection(source);
            if (!object.IsNullOrUndefined(obsCol))
            {
                GraphEngine.RenderCollection(desc, state, nodeIdx, colInfo, obsCol);
                return;
            }

            IList list = source as IList;
            if (!object.IsNullOrUndefined(list) && !(source is string))
            {
                int count = list.Count;
                NativeArray items = new NativeArray(count);
                for (int idx = 0; idx < count; idx++)
                    items[idx] = list[idx];
                GraphEngine.RenderItemArray(desc, state, nodeIdx, colInfo, items);
                return;
            }

            Logger.Error("GraphEngine: @foreach source is neither an observable collection nor an array/IList; "
                + "no rows rendered. Use ObservableCollection<T>, List<T> or T[].");
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
            Exception first = null;
            try
            {
                GraphEngine.RenderCollectionItems(desc, state, nodeIdx, colInfo, collection);
            }
            catch (Exception ex)
            {
                first = ex;
            }
            finally
            {
                state.CollectionChangesInProgress[nodeIdx] = false;
            }

            // A failed row must not drop the changes queued while the rows built (a row's
            // setter removing its item): replay them before the error goes on, so the next
            // change applies in order. The render error wins over a replay error.
            try
            {
                GraphEngine.ReplayPendingCollectionChanges(desc, state, nodeIdx, colInfo);
            }
            catch (Exception ex)
            {
                if (first == null) first = ex;
            }

            if (first != null) throw first;
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
            int count = collection.Count;
            NativeArray items = new NativeArray(count);
            for (int idx = 0; idx < count; idx++)
                items[idx] = collection[idx];
            GraphEngine.RenderItemArray(desc, state, nodeIdx, colInfo, items);
        }

        private static void RenderItemArray(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, NativeArray items)
        {
            Element marker = (Element)state.ElemRefs[colInfo.MarkerIdx];
            if (object.IsNullOrUndefined(marker)) return;

            Node parent = marker.ParentNode;
            if (object.IsNullOrUndefined(parent)) return;

            int count = items.Length;
            NativeArray<GraphState> childStates = new NativeArray<GraphState>(count);
            NativeArray itemElems = new NativeArray(count);

            // Parse the template HTML once, then clone per item to avoid
            // repeated innerHTML parsing.
            NativeArray<Element> template = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, marker);
            if (object.IsNullOrUndefined(template)) return;

            for (int idx = 0; idx < count; idx++)
            {
                itemElems[idx] = GraphEngine.InsertRow(template, parent, marker);
            }

            // Commit before building row graphs, as HandleCollectionAdd does: when a later row
            // throws, the rows built before it are tracked, so a clear, a Remove or Dispose
            // retires them. The exception reaches the caller.
            state.ChildGraphStates[nodeIdx] = childStates;
            state.ItemElements[nodeIdx] = itemElems;

            // Every row builds even when an earlier one throws, so only the failed row is left
            // unbound; the first error then reaches the caller.
            Exception first = null;
            if (!object.IsNullOrUndefined(colInfo.ItemGraph))
            {
                for (int idx = 0; idx < count; idx++)
                {
                    // A row that disposed the owner (a setter tearing the view down) ends the
                    // render: a later row would wire listeners that no cleanup reaches.
                    if (state.Disposed) break;
                    try
                    {
                        GraphEngine.CreateCollectionItemGraph(
                            state, colInfo, (NativeArray<Element>)itemElems[idx], items[idx], childStates, idx);
                    }
                    catch (Exception ex)
                    {
                        if (first == null) first = ex;
                    }
                }
            }

            if (!state.Disposed)
                GraphEngine.ReapplySelectTargets(desc, state, parent);
            if (first != null) throw first;
        }

        /// <summary>
        /// A &lt;select&gt;'s value only sticks once a matching &lt;option&gt; exists, and the
        /// options of <c>&lt;select value="@X"&gt;@foreach(...){&lt;option&gt;}</c> render after
        /// (or change after) the value was set. Re-apply the select's own DOM targets.
        /// </summary>
        private static void ReapplySelectTargets(GraphDescriptor desc, GraphState state, Node parent)
        {
            if (!GraphEngine.IsSelectElement(parent)) return;

            for (int j = 0; j < desc.NodeCount; j++)
            {
                if (desc.NodeTypes[j] != GraphNodeType.DomTarget) continue;
                DomTargetInfo info = (DomTargetInfo)desc.TargetInfos[j];
                if (object.IsNullOrUndefined(info) || object.IsNullOrUndefined(info.Setter)) continue;
                object elem = state.ElemRefs[info.ElemIdx];
                if (elem != (object)parent) continue;

                object val = state.Values[j];
                info.Setter(elem, !object.IsNullOrUndefined(val) ? val : desc.DefaultValues[j]);
            }
        }

        [System.Runtime.CompilerServices.Script(@"return n.nodeName == 'SELECT';")]
        private static extern bool IsSelectElement(Node n);

        /// <summary>
        /// Builds a row's graph and stores it in <paramref name="rows"/>[<paramref name="rowIdx"/>]
        /// before wiring anything, so a build that throws partway still leaves the row's state
        /// tracked: removing or clearing the row retires the subscriptions, listeners and
        /// controls it attached, and removes any @if branch it rendered (RemoveChildGateElements).
        /// </summary>
        private static void CreateCollectionItemGraph(
            GraphState parentState, CollectionTargetInfo colInfo, NativeArray<Element> rowNodes, object item,
            NativeArray<GraphState> rows, int rowIdx)
        {
            NativeArray childElemRefs = GraphEngine.AssignMarkersToSlots(
                colInfo.ItemGraph, GraphEngine.CollectGateSpanElements(rowNodes));

            GraphState childState = new GraphState(colInfo.ItemGraph, childElemRefs, parentState.Depth + 1);
            rows[rowIdx] = childState;
            NativeArray itemContext = new NativeArray(3);
            itemContext[0] = parentState.Sources[GraphSourceSlot.DataContext];
            itemContext[1] = parentState.Sources[GraphSourceSlot.TemplateParent];
            itemContext[2] = item;
            childState.Sources[GraphSourceSlot.DataContext] = itemContext;

            // A row that throws while it builds (a sub-control refusing its content, a
            // setter) is retired before the error goes on, so nothing it wired stays live.
            // The build error wins over one from the retire.
            try
            {
                for (int k = 0; k < rowNodes.Length; k++)
                    GraphEngine.CreateSubControls(colInfo.ItemGraph, childState, rowNodes[k], item);

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
            }
            catch (Exception ex)
            {
                Exception first = ex;
                try { GraphEngine.RetireChildState(colInfo.ItemGraph, childState); }
                catch (Exception) { }
                throw first;
            }
        }

        private static void RefreshCollectionItems(GraphState state, int nodeIdx)
        {
            NativeArray<GraphState> children = state.ChildGraphStates[nodeIdx];
            if (object.IsNullOrUndefined(children)) return;

            for (int i = 0; i < children.Length; i++)
            {
                // A retired row (one whose build threw) stays retired: rewiring it would leave
                // listeners that a later Remove or Dispose no longer reaches.
                GraphState child = children[i];
                if (object.IsNullOrUndefined(child) || child.Disposed) continue;

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
            if (object.IsNullOrUndefined(state.PendingCollectionChanges[nodeIdx])) return;
            if (state.Disposed)
            {
                state.PendingCollectionChanges[nodeIdx] = null;
                return;
            }

            // Replay under the in-progress guard, so a change made while one replays (a row's
            // setter removing an item) queues behind the saved changes instead of running
            // ahead of them.
            state.CollectionChangesInProgress[nodeIdx] = true;
            Exception first = null;
            try
            {
                first = GraphEngine.DrainPendingCollectionChanges(desc, state, nodeIdx, colInfo, null);
            }
            finally
            {
                state.CollectionChangesInProgress[nodeIdx] = false;
            }

            if (first != null) throw first;
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
            // A branch's top-level whitespace is a text node: it holds no host.
            if (object.IsNullOrUndefined(infos) || object.IsNullOrUndefined(root)
                || object.IsNullOrUndefined(root.TagName)) return;

            for (int i = 0; i < infos.Length; i++)
            {
                SubControlInfo info = infos[i];
                if (object.IsNullOrUndefined(info) || object.IsNullOrUndefined(info.TypeFactory)) continue;
                if (!object.IsNullOrUndefined(state.SubControlInstances[i])) continue;

                Element marker = GraphEngine.FindSubControlMarker(root, info.MarkerIdx);
                if (object.IsNullOrUndefined(marker)) continue;

                UIElement control = (UIElement)info.TypeFactory(marker);
                // Nested skins flush after this graph (see GraphFlushCoordinator).
                // Must be set before the skin is assigned: SkinInstance.Bind reads it.
                control.BindingDepth = state.Depth + 1;
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

        /// <summary>
        /// Creates the controls of a rendered gate branch. A branch can have several
        /// top-level nodes, and a host can sit in any of them, not only the first.
        /// </summary>
        private static void CreateGateBranchSubControls(GraphDescriptor desc, GraphState state, int gateNodeIdx)
        {
            object dataContext = GraphEngine.GetSubControlDataContext(state);
            NativeArray<Element> nodes = (NativeArray<Element>)state.GateExtraElements[gateNodeIdx];
            if (object.IsNullOrUndefined(nodes))
            {
                GraphEngine.CreateSubControls(desc, state, (Element)state.GateElements[gateNodeIdx], dataContext);
                return;
            }

            for (int k = 0; k < nodes.Length; k++)
                GraphEngine.CreateSubControls(desc, state, nodes[k], dataContext);
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
                    if (!object.IsNullOrUndefined(children[i]) && !children[i].Disposed)
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
                    if (!object.IsNullOrUndefined(children[i]) && !children[i].Disposed)
                        GraphEngine.DeactivateSubControls(children[i].Descriptor, children[i]);
            }
        }

        public static void DisposeSubControls(GraphDescriptor desc, GraphState state)
        {
            state.SubControlsActive = false;
            NativeArray<UIElement> controls = state.SubControlInstances;
            if (object.IsNullOrUndefined(controls)) return;
            NativeArray<SubControlInfo> infos = desc.SubControls;

            // A control that throws while it tears down must not keep the later ones alive:
            // tear every one down, then rethrow the first error.
            Exception first = null;
            for (int i = 0; i < controls.Length; i++)
            {
                UIElement control = controls[i];
                if (object.IsNullOrUndefined(control)) continue;
                // Each step runs even when an earlier one throws: a control whose
                // Deactivate throws is still disposed.
                try
                {
                    try { GraphEngine.UnwireSubControlPropertyListeners(desc, state, i); }
                    catch (Exception ex) { if (first == null) first = ex; }
                    try { control.Deactivate(); }
                    catch (Exception ex) { if (first == null) first = ex; }
                    try { control.Dispose(); }
                    catch (Exception ex) { if (first == null) first = ex; }
                }
                finally
                {
                    // A control's Dispose may have disposed the host, which drops ElemRefs.
                    if (!object.IsNullOrUndefined(state.ElemRefs))
                        state.ElemRefs[infos[i].ElemIdx] = null;
                    controls[i] = null;
                    state.SubControlAppliedValues[i] = null;
                    state.SubControlBindingsInitialized[i] = false;
                }
            }

            if (first != null) throw first;
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
        /// Permanently retires a collection item's graph: marks it Disposed first,
        /// so a flush the coordinator already queued for it (batched mode) is
        /// rejected by FlushOne instead of writing to detached DOM, re-creating
        /// gated sub-controls, or re-attaching nested collection listeners. Then
        /// unwires its subscriptions, events, nested collections (recursively
        /// retiring their items) and sub-controls. A second call does nothing: a row that
        /// threw while it built is retired at once and again when it is removed.
        /// </summary>
        public static void RetireChildState(GraphDescriptor itemDesc, GraphState child)
        {
            if (child.Disposed) return;
            child.Disposed = true;

            // Disposed blocks a second retire, so finish every step even when one throws
            // (a sub-control's or a nested row's Dispose), then rethrow the first error.
            Exception first = null;
            try { GraphEngine.UnwireChildSubscriptions(itemDesc, child); }
            catch (Exception ex) { first = ex; }
            try { GraphEngine.CleanupEventListeners(itemDesc, child); }
            catch (Exception ex) { if (first == null) first = ex; }
            try { GraphEngine.CleanupCollectionListeners(itemDesc, child); }
            catch (Exception ex) { if (first == null) first = ex; }
            try { GraphEngine.DisposeSubControls(itemDesc, child); }
            catch (Exception ex) { if (first == null) first = ex; }
            if (first != null) throw first;
        }

        /// <summary>
        /// Clears all rendered items for a CollectionManager node.
        /// </summary>
        public static void ClearCollectionItems(GraphDescriptor desc, GraphState state, int nodeIdx, CollectionTargetInfo colInfo)
        {
            NativeArray itemElems = state.ItemElements[nodeIdx];
            NativeArray<GraphState> childStates = state.ChildGraphStates[nodeIdx];

            // Commit first, as HandleCollectionRemove does: a row whose sub-control Dispose
            // throws must not leave stale rows behind for the next render.
            state.ChildGraphStates[nodeIdx] = null;
            state.ItemElements[nodeIdx] = null;

            int count = !object.IsNullOrUndefined(itemElems) ? itemElems.Length
                : !object.IsNullOrUndefined(childStates) ? childStates.Length : 0;
            try
            {
                // Retires every row (listeners, events, nested collections, controls) and
                // removes its nodes and gate content, even when one row throws.
                GraphEngine.TearDownRemovedRows(colInfo.ItemGraph, childStates, itemElems, 0, count - 1);
            }
            finally
            {
                if (!object.IsNullOrUndefined(childStates))
                {
                    for (int idx = 0; idx < childStates.Length; idx++)
                    {
                        GraphState child = childStates[idx];
                        if (object.IsNullOrUndefined(child)) continue;
                        for (int v = 0; v < child.Values.Length; v++)
                        {
                            child.Values[v] = null;
                        }
                    }
                }
            }
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
            Exception first = null;
            try
            {
                first = GraphEngine.TryApplyCollectionChange(desc, state, nodeIdx, colInfo, args, null);

                // Changes queued while this one applied still apply, in order, even after one
                // throws, so a failed row never lets a later change run ahead of them.
                first = GraphEngine.DrainPendingCollectionChanges(desc, state, nodeIdx, colInfo, first);
            }
            finally
            {
                state.CollectionChangesInProgress[nodeIdx] = false;
            }

            if (first != null) throw first;
        }

        /// <summary>
        /// Applies the queued changes in the order they arrived; a change queued while they
        /// apply goes after them. Keeps going after one throws and stops once the owner is
        /// disposed. Returns the first error so far.
        /// </summary>
        private static Exception DrainPendingCollectionChanges(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, Exception first)
        {
            NativeArray<CollectionChangedEventArgs> pending = state.PendingCollectionChanges[nodeIdx];
            state.PendingCollectionChanges[nodeIdx] = null;
            while (!object.IsNullOrUndefined(pending) && !state.Disposed)
            {
                int resetIdx = GraphEngine.FindLastCollectionReset(pending);
                if (resetIdx >= 0)
                    first = GraphEngine.TryApplyCollectionChange(
                        desc, state, nodeIdx, colInfo, pending[resetIdx], first);
                else
                    for (int i = 0; i < pending.Length && !state.Disposed; i++)
                        first = GraphEngine.TryApplyCollectionChange(
                            desc, state, nodeIdx, colInfo, pending[i], first);
                pending = state.PendingCollectionChanges[nodeIdx];
                state.PendingCollectionChanges[nodeIdx] = null;
            }

            return first;
        }

        /// <summary>
        /// Applies one change and returns the first error so far: <paramref name="first"/>,
        /// or this change's error when there was none before.
        /// </summary>
        private static Exception TryApplyCollectionChange(
            GraphDescriptor desc, GraphState state, int nodeIdx,
            CollectionTargetInfo colInfo, CollectionChangedEventArgs args, Exception first)
        {
            try
            {
                GraphEngine.ApplyCollectionChange(desc, state, nodeIdx, colInfo, args);
            }
            catch (Exception ex)
            {
                if (first == null) first = ex;
            }

            return first;
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
                // (we're already inside the listener callback). A row whose Dispose throws
                // still lets the new contents render; the first error then propagates.
                Exception first = null;
                try
                {
                    GraphEngine.ClearCollectionItems(desc, state, nodeIdx, colInfo);
                }
                catch (Exception ex)
                {
                    first = ex;
                }

                try
                {
                    object collection = state.Values[nodeIdx];
                    if (!object.IsNullOrUndefined(collection))
                    {
                        IObservableCollection obsCol = GraphEngine.AsObservableCollection(collection);
                        if (!object.IsNullOrUndefined(obsCol))
                            GraphEngine.RenderCollectionItems(desc, state, nodeIdx, colInfo, obsCol);
                    }
                }
                catch (Exception ex)
                {
                    if (first == null) first = ex;
                }

                if (first != null) throw first;
            }

            GraphEngine.ReapplySelectTargets(desc, state, parent);
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
            // Where the next row starts; rows are inserted before it.
            Node refNode = marker;
            if (insertIdx < oldCount && !object.IsNullOrUndefined(itemElems))
            {
                refNode = GraphEngine.RowStart(
                    colInfo.ItemGraph,
                    !object.IsNullOrUndefined(childStates) ? childStates[insertIdx] : null,
                    (NativeArray<Element>)itemElems[insertIdx]);
            }

            // Parse template once, then clone per item.
            NativeArray<Element> template = GraphEngine.ParseTemplateHtml((string)colInfo.ItemTemplate, marker);
            if (object.IsNullOrUndefined(template)) return;

            // Insert new items.
            for (int j = 0; j < addCount; j++)
            {
                newItemElems[insertIdx + j] = GraphEngine.InsertRow(template, parent, refNode);
            }

            // Copy items after insertion point.
            for (int j = insertIdx; j < oldCount; j++)
            {
                newChildStates[j + addCount] = !object.IsNullOrUndefined(childStates) ? childStates[j] : null;
                newItemElems[j + addCount] = !object.IsNullOrUndefined(itemElems) ? itemElems[j] : null;
            }

            // Commit before building row graphs. A row graph that throws still leaves one
            // tracked row per item, so the next Remove finds its row instead of computing a
            // negative array length. The exception still reaches the caller.
            state.ChildGraphStates[nodeIdx] = newChildStates;
            state.ItemElements[nodeIdx] = newItemElems;

            if (object.IsNullOrUndefined(colInfo.ItemGraph)) return;

            // As in RenderItemArray: build every added row, then rethrow the first error.
            Exception first = null;
            for (int j = 0; j < addCount; j++)
            {
                if (state.Disposed) break;
                try
                {
                    GraphEngine.CreateCollectionItemGraph(
                        state, colInfo, (NativeArray<Element>)newItemElems[insertIdx + j], newItems[j],
                        newChildStates, insertIdx + j);
                }
                catch (Exception ex)
                {
                    if (first == null) first = ex;
                }
            }

            if (first != null) throw first;
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

            // Commit the bookkeeping before tearing rows down: a row whose
            // sub-control Dispose throws must not leave rows out of step.
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

            GraphEngine.TearDownRemovedRows(
                colInfo.ItemGraph, childStates, itemElems, removeIdx, removeIdx + removeCount - 1);
        }

        /// <summary>
        /// Tears down rows [from, to] in order. A row that throws still lets the
        /// remaining rows leave the DOM; the first error then propagates.
        /// </summary>
        private static void TearDownRemovedRows(
            GraphDescriptor itemDesc, NativeArray<GraphState> childStates, NativeArray itemElems, int from, int to)
        {
            Exception first = null;
            for (int j = from; j <= to; j++)
            {
                try
                {
                    GraphState childState = !object.IsNullOrUndefined(childStates) ? childStates[j] : null;
                    NativeArray<Element> rowNodes = !object.IsNullOrUndefined(itemElems)
                        ? (NativeArray<Element>)itemElems[j]
                        : null;
                    GraphEngine.TearDownRow(itemDesc, childState, rowNodes);
                }
                catch (Exception ex)
                {
                    if (first == null) first = ex;
                }
            }

            if (first != null) throw first;
        }

        /// <summary>
        /// Retires one row's graph, then removes its gate content and every top-level
        /// node of the row even when retiring throws (the first error then propagates).
        /// Gates render as siblings of the row's nodes, so removing just those nodes
        /// would leave gate content behind.
        /// </summary>
        private static void TearDownRow(GraphDescriptor itemDesc, GraphState childState, NativeArray<Element> rowNodes)
        {
            Exception first = null;
            try
            {
                if (!object.IsNullOrUndefined(childState) && !object.IsNullOrUndefined(itemDesc))
                    GraphEngine.RetireChildState(itemDesc, childState);
            }
            catch (Exception ex)
            {
                first = ex;
            }

            try
            {
                if (!object.IsNullOrUndefined(childState))
                    GraphEngine.RemoveChildGateElements(childState);
            }
            catch (Exception ex)
            {
                if (first == null) first = ex;
            }

            if (!object.IsNullOrUndefined(rowNodes))
            {
                for (int k = 0; k < rowNodes.Length; k++)
                    rowNodes[k].Remove();
            }

            if (first != null) throw first;
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
            // The new row is added even when retiring the old one throws; the first
            // error then propagates.
            Exception first = null;
            try
            {
                GraphEngine.HandleCollectionRemove(state, nodeIdx, colInfo, args);
            }
            catch (Exception ex)
            {
                first = ex;
            }

            try
            {
                GraphEngine.HandleCollectionAdd(state, nodeIdx, colInfo, args, marker, parent);
            }
            catch (Exception ex)
            {
                if (first == null) first = ex;
            }

            if (first != null) throw first;
        }

        /// <summary>
        /// Cleans up all event listeners for disposal.
        /// </summary>
        public static void CleanupEventListeners(GraphDescriptor desc, GraphState state)
        {
            GraphEngine.UnwireTwoWayValues(state);
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
        /// Binds <c>input</c> and <c>change</c> on every resolved two-way <c>value=</c>
        /// element that is not bound yet; a replaced element (a gate re-rendered its branch)
        /// is unbound and its successor bound. Each event writes the element's value back to
        /// the source unless it already equals the bound value.
        /// </summary>
        public static void WireTwoWayValues(GraphDescriptor desc, GraphState state)
        {
            int n = desc.NodeCount;
            for (int i = 0; i < n; i++)
            {
                if (desc.NodeTypes[i] != GraphNodeType.DomTarget) continue;
                DomTargetInfo info = (DomTargetInfo)desc.TargetInfos[i];
                if (object.IsNullOrUndefined(info) || object.IsNullOrUndefined(info.SourceSetter)) continue;

                Element elem = state.ElemRefs[info.ElemIdx] as Element;
                if (object.IsNullOrUndefined(elem)) continue;
                if (object.IsNullOrUndefined(state.TwoWayListeners))
                {
                    state.TwoWayListeners = new NativeArray(n);
                    state.TwoWayElements = new NativeArray(n);
                }

                if (state.TwoWayElements[i] == elem) continue;
                GraphEngine.UnwireTwoWayValue(state, i);

                Action<Element, ElementEvent> handler = GraphEngine.CreateTwoWayHandler(state, i, info, elem);
                elem.Bind("input", handler);
                elem.Bind("change", handler);
                state.TwoWayListeners[i] = handler;
                state.TwoWayElements[i] = elem;
            }
        }

        private static Action<Element, ElementEvent> CreateTwoWayHandler(
            GraphState state, int nodeIdx, DomTargetInfo info, Element elem)
        {
            return delegate(Element sender, ElementEvent evt)
            {
                string value = GraphEngine.GetElementValue(elem);
                object source = state.Sources[GraphSourceSlot.DataContext];
                if (!object.IsNullOrUndefined(source) && !object.Equals(state.Values[nodeIdx], value))
                    info.SourceSetter(source, value);
            };
        }

        [System.Runtime.CompilerServices.Script(@"return e.value;")]
        private static extern string GetElementValue(Element e);

        private static void UnwireTwoWayValue(GraphState state, int nodeIdx)
        {
            Element bound = state.TwoWayElements[nodeIdx] as Element;
            Action<Element, ElementEvent> handler =
                (Action<Element, ElementEvent>)state.TwoWayListeners[nodeIdx];
            if (!object.IsNullOrUndefined(bound) && !object.IsNullOrUndefined(handler))
            {
                bound.UnBind("input", handler);
                bound.UnBind("change", handler);
            }

            state.TwoWayListeners[nodeIdx] = null;
            state.TwoWayElements[nodeIdx] = null;
        }

        private static void UnwireTwoWayValues(GraphState state)
        {
            if (object.IsNullOrUndefined(state.TwoWayListeners)) return;
            for (int i = 0; i < state.TwoWayListeners.Length; i++)
            {
                GraphEngine.UnwireTwoWayValue(state, i);
            }
        }

        /// <summary>
        /// Cleans up all collection listeners for disposal.
        /// </summary>
        public static void CleanupCollectionListeners(GraphDescriptor desc, GraphState state)
        {
            // A list whose teardown throws (a row's control Dispose) must not keep the later
            // lists alive: clean every one, then rethrow the first error.
            Exception first = null;
            int n = desc.NodeCount;
            for (int i = 0; i < n; i++)
            {
                if (desc.NodeTypes[i] == GraphNodeType.CollectionManager)
                {
                    object collection = state.Values[i];
                    if (!object.IsNullOrUndefined(collection))
                    {
                        try { GraphEngine.DetachCollectionListener(state, i, collection); }
                        catch (Exception ex) { if (first == null) first = ex; }
                    }

                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo))
                    {
                        try { GraphEngine.ClearCollectionItems(desc, state, i, colInfo); }
                        catch (Exception ex) { if (first == null) first = ex; }
                    }
                }
            }

            if (first != null) throw first;
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

            // Record before attaching: a getter that throws partway (a chained path's owner)
            // leaves the listeners attached so far where UnwireChildSubscriptions finds them.
            childState.Listeners = listeners;
            childState.ListenerCount = subCount;
            childState.SubscriptionsActive = true;

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
        /// that must listen on every object along the path, not just the root. A one-segment
        /// entry that carries owner getters is chained too: the compiler emits it for Model.X in
        /// a loop inside a loop, whose source slot cannot reach the model.
        /// </summary>
        public static bool IsChainedEntry(SubscriptionEntry entry)
        {
            return !object.IsNullOrUndefined(entry.PathSegments)
                && (entry.PathSegments.Length > 1 || !object.IsNullOrUndefined(entry.ChainParentGetters));
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

            // Stored first: an owner getter that throws partway still leaves the hops attached
            // so far where UnwireChainedSubscription finds them.
            NativeArray<ChainListenerHandle> handles = new NativeArray<ChainListenerHandle>(0);
            state.ChainListeners[entryIndex] = handles;
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

                // A leaf change moves no owner (owners are the path before the leaf), so the
                // hops stay wired: re-wiring here would unhook and re-hook every row's
                // listeners on each change of a shared model property.
                if (!GraphEngine.IsChainLeafChange(state, entry, entryIndex, sender, propName))
                {
                    GraphEngine.UnwireChainedSubscription(state, entryIndex);
                    GraphEngine.WireChainedSubscription(state, entry, entryIndex);
                }

                GraphEngine.NotifyDirty(state, entry.NodeIdx);
            };
        }

        /// <summary>
        /// True when a chained subscription fired for its leaf hop only: the chain is fully
        /// wired, the sender and property are the last hop's, and no earlier hop listens to
        /// the same object and property (then the change could also move an owner).
        /// </summary>
        private static bool IsChainLeafChange(
            GraphState state, SubscriptionEntry entry, int entryIndex,
            INotifyPropertyChanged sender, string propName)
        {
            if (string.IsNullOrEmpty(propName) || object.IsNullOrUndefined(state.ChainListeners)) return false;
            NativeArray<ChainListenerHandle> handles = state.ChainListeners[entryIndex];
            if (object.IsNullOrUndefined(handles) || handles.Length != entry.PathSegments.Length) return false;

            int last = handles.Length - 1;
            if (handles[last].Observable != sender || handles[last].PropertyName != propName) return false;
            for (int k = 0; k < last; k++)
                if (handles[k].Observable == sender && handles[k].PropertyName == propName) return false;
            return true;
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
        /// Collects compiler-generated markers from an item clone, the clone itself first, in
        /// DOM order (which is the compiler's element-index order). Markers are data-ns-ph
        /// (text placeholders and gate/collection anchors), data-ns-subctl (child hosts) and
        /// data-ns-m="k" (an element owning k attribute bindings/events, listed k times).
        /// </summary>
        public static NativeArray CollectSpanElements(Element clone)
        {
            NativeArray result = new NativeArray(0);
            GraphEngine.CollectMarkers(clone, result);
            return result;
        }

        private const string MarkerSelector = "[data-ns-ph], [data-ns-m], [data-ns-subctl]";

        /// <summary>Pushes the node (if it is a marker) and then its descendant markers.</summary>
        private static void CollectMarkers(Element node, NativeArray result)
        {
            if (object.IsNullOrUndefined(node) || object.IsNullOrUndefined(node.TagName)) return;
            GraphEngine.PushMarker(node, result);
            NativeArray<Element> inner = node.QuerySelectorAll(GraphEngine.MarkerSelector);
            for (int i = 0; i < inner.Length; i++) GraphEngine.PushMarker(inner[i], result);
        }

        private static void PushMarker(Element element, NativeArray result)
        {
            string ownerCount = element.GetAttribute("data-ns-m");
            if (!object.IsNullOrUndefined(ownerCount))
            {
                int count = int.Parse(ownerCount);
                for (int k = 0; k < count; k++) result.Push(element);
            }
            else if (element.HasAttribute("data-ns-ph") || element.HasAttribute("data-ns-subctl"))
            {
                result.Push(element);
            }
        }

        /// <summary>
        /// Builds an item graph's element refs: markers fill the element slots in order,
        /// skipping slots inside @if branches (absent from the row DOM until their gate
        /// renders them; ResolveGateChildElems fills them then).
        /// </summary>
        private static NativeArray AssignMarkersToSlots(GraphDescriptor desc, NativeArray markers)
        {
            NativeArray<bool> gated = GraphEngine.GatedElemSlots(desc, null);
            NativeArray refs = new NativeArray(0);
            int slot = 0;
            for (int m = 0; m < markers.Length; m++)
            {
                while (slot < gated.Length && gated[slot])
                {
                    refs[slot] = null;
                    slot++;
                }

                refs[slot] = markers[m];
                slot++;
            }

            return refs;
        }

        /// <summary>
        /// Element slots rendered by gates: every gate's branch slots, or with
        /// <paramref name="within"/> only those of gates whose anchor is one of those slots
        /// (the gates nested in a branch). Indexed by slot; unset entries are false.
        /// </summary>
        private static NativeArray<bool> GatedElemSlots(GraphDescriptor desc, NativeArray<int> within)
        {
            NativeArray<bool> gated = new NativeArray<bool>(0);
            for (int i = 0; i < desc.NodeCount; i++)
            {
                if (desc.NodeTypes[i] != GraphNodeType.Gate) continue;
                GateTargetInfo gate = (GateTargetInfo)desc.TargetInfos[i];
                if (object.IsNullOrUndefined(gate)) continue;
                if (!object.IsNullOrUndefined(within) && !GraphEngine.ContainsSlot(within, gate.MarkerIdx)) continue;
                GraphEngine.MarkSlots(gated, gate.TrueChildElemIndices);
                GraphEngine.MarkSlots(gated, gate.FalseChildElemIndices);
            }

            return gated;
        }

        private static void MarkSlots(NativeArray<bool> gated, NativeArray<int> slots)
        {
            if (object.IsNullOrUndefined(slots)) return;
            for (int k = 0; k < slots.Length; k++)
            {
                int slot = slots[k];
                while (gated.Length <= slot) gated.Push(false);
                gated[slot] = true;
            }
        }

        private static bool ContainsSlot(NativeArray<int> slots, int slot)
        {
            for (int k = 0; k < slots.Length; k++)
            {
                if (slots[k] == slot) return true;
            }

            return false;
        }

        /// <summary>
        /// Collects compiler-generated markers across every top-level node of a
        /// gate branch or a @foreach row, each node first and then its descendants,
        /// in document order.
        /// </summary>
        public static NativeArray CollectGateSpanElements(NativeArray<Element> nodes)
        {
            NativeArray result = new NativeArray(0);
            for (int k = 0; k < nodes.Length; k++)
            {
                GraphEngine.CollectMarkers(nodes[k], result);
            }

            return result;
        }

        /// <summary>
        /// Parses template HTML in a &lt;template&gt; element so it keeps the shape it has in
        /// its real parent: a &lt;tr&gt;, &lt;td&gt; or &lt;option&gt; row survives, where a
        /// &lt;div&gt; container would drop the tags. Returns the parsed top-level nodes.
        /// </summary>
        [System.Runtime.CompilerServices.Script(@"var t = doc.createElement('template'); t.innerHTML = html; return t.content.childNodes;")]
        private static extern NativeArray<Node> ParseFragmentNodes(Document doc, string html);

        /// <summary>
        /// Parses a gate branch template and inserts every top-level node
        /// before the marker. The first node is the branch root (GateElements);
        /// any further nodes (e.g. the event marker span that trails a void root
        /// element) are kept in GateExtraElements so they are removed with it.
        /// </summary>
        public static Element InsertGateBranch(GraphState state, int gateNodeIdx, string html, Element marker)
        {
            if (object.IsNullOrUndefined(html) || html == "") return null;

            // Copy first: inserting a node elsewhere removes it from the live list.
            NativeArray<Node> children = GraphEngine.ParseFragmentNodes(marker.OwnerDocument, html);
            NativeArray<Element> nodes = new NativeArray<Element>(0);
            for (int k = 0; k < children.Length; k++) nodes.Push((Element)children[k]);
            if (nodes.Length == 0) return null;

            Node parent = marker.ParentNode;
            if (!object.IsNullOrUndefined(parent))
            {
                for (int k = 0; k < nodes.Length; k++) parent.InsertBefore(nodes[k], marker);
            }

            state.GateElements[gateNodeIdx] = nodes[0];
            state.GateExtraElements[gateNodeIdx] = nodes.Length > 1 ? nodes : null;
            return nodes[0];
        }

        /// <summary>Removes a gate branch's root and any extra top-level nodes.</summary>
        public static void RemoveGateBranch(GraphState state, int gateNodeIdx)
        {
            // The branch may hold this skin's outlet: take the caller content out first.
            GraphEngine.ParkGateBranchProjections(state, gateNodeIdx);

            Element root = (Element)state.GateElements[gateNodeIdx];
            if (!object.IsNullOrUndefined(root)) root.Remove();
            state.GateElements[gateNodeIdx] = null;

            if (object.IsNullOrUndefined(state.GateExtraElements)) return;
            NativeArray<Element> extras = (NativeArray<Element>)state.GateExtraElements[gateNodeIdx];
            if (!object.IsNullOrUndefined(extras))
            {
                for (int k = 1; k < extras.Length; k++)
                {
                    if (!object.IsNullOrUndefined(extras[k])) extras[k].Remove();
                }
            }
            state.GateExtraElements[gateNodeIdx] = null;
        }

        /// <summary>
        /// Moves each parked slot's caller content into its outlet when the outlet is
        /// <paramref name="scope"/> or inside it: between the two anchors. A slot already placed
        /// is left alone (one outlet per slot on any render path).
        /// </summary>
        public static void PlaceProjections(GraphState state, Element scope)
        {
            NativeArray<SlotProjection> slots = state.Projections;
            if (object.IsNullOrUndefined(slots) || object.IsNullOrUndefined(scope)
                || object.IsNullOrUndefined(scope.TagName)) return;

            // Find every outlet before moving anything: one slot's content can hold a control
            // with its own outlets, which would then sit ahead of ours in document order.
            NativeArray<Element> starts = new NativeArray<Element>(slots.Length);
            for (int k = 0; k < slots.Length; k++)
            {
                SlotProjection slot = slots[k];
                if (!object.IsNullOrUndefined(slot.Start)) continue;

                string selector = GraphEngine.SlotAnchorSelector(slot.Name);
                Element start = scope.Matches(selector) ? scope : scope.QuerySelector(selector);
                if (object.IsNullOrUndefined(start)) continue;

                Element end = start.NextElementSibling;
                if (object.IsNullOrUndefined(end) || !end.Matches(selector))
                    throw new Exception("The outlet for slot '" + slot.Name
                        + "' lost its closing anchor; the skin HTML and its outlet are out of step.");
                starts[k] = start;
            }

            for (int k = 0; k < slots.Length; k++)
            {
                Element start = starts[k];
                if (object.IsNullOrUndefined(start)) continue;

                SlotProjection slot = slots[k];
                Element end = start.NextElementSibling;
                Node parent = start.ParentNode;
                while (!object.IsNullOrUndefined(slot.Holder.FirstChild))
                    parent.InsertBefore(slot.Holder.FirstChild, end);
                slot.Start = start;
                slot.End = end;
            }
        }

        /// <summary>Moves each placed slot's caller content back into its holder.</summary>
        public static void ParkProjections(GraphState state)
        {
            NativeArray<SlotProjection> slots = state.Projections;
            if (object.IsNullOrUndefined(slots)) return;
            for (int k = 0; k < slots.Length; k++)
                GraphEngine.ParkProjection(slots[k]);
        }

        private static void ParkProjection(SlotProjection slot)
        {
            if (object.IsNullOrUndefined(slot.Start)) return;
            Node node = slot.Start.NextSibling;
            while (!object.IsNullOrUndefined(node) && node != (Node)slot.End)
            {
                Node next = node.NextSibling;
                slot.Holder.AppendChild(node);
                node = next;
            }

            slot.Start = null;
            slot.End = null;
        }

        /// <summary>Selects the anchors of the outlet for one slot ("" is @RenderBody).</summary>
        public static string SlotAnchorSelector(string name)
        {
            return "template[data-ns-slot=\"" + name + "\"]";
        }

        private static void PlaceGateBranchProjections(GraphState state, int gateNodeIdx)
        {
            if (object.IsNullOrUndefined(state.Projections)) return;
            NativeArray<Element> extras = (NativeArray<Element>)state.GateExtraElements[gateNodeIdx];
            if (object.IsNullOrUndefined(extras))
            {
                GraphEngine.PlaceProjections(state, (Element)state.GateElements[gateNodeIdx]);
                return;
            }

            for (int k = 0; k < extras.Length; k++)
                GraphEngine.PlaceProjections(state, extras[k]);
        }

        private static void ParkGateBranchProjections(GraphState state, int gateNodeIdx)
        {
            NativeArray<SlotProjection> slots = state.Projections;
            if (object.IsNullOrUndefined(slots)) return;

            Element root = (Element)state.GateElements[gateNodeIdx];
            NativeArray<Element> extras = object.IsNullOrUndefined(state.GateExtraElements)
                ? null
                : (NativeArray<Element>)state.GateExtraElements[gateNodeIdx];
            for (int k = 0; k < slots.Length; k++)
            {
                SlotProjection slot = slots[k];
                if (object.IsNullOrUndefined(slot.Start)) continue;
                bool inBranch = GraphEngine.HoldsNode(root, slot.Start);
                if (!object.IsNullOrUndefined(extras))
                    for (int e = 0; e < extras.Length && !inBranch; e++)
                        inBranch = GraphEngine.HoldsNode(extras[e], slot.Start);
                if (inBranch) GraphEngine.ParkProjection(slot);
            }
        }

        private static bool HoldsNode(Element container, Element node)
        {
            return !object.IsNullOrUndefined(container)
                && !object.IsNullOrUndefined(container.TagName)
                && (container == node || container.Contains(node));
        }

        /// <summary>
        /// Unbinds the listeners of every EventBinding node controlled by the
        /// given gate reference (gate index for the true branch, -(gate + 2)
        /// for the false branch). Must run before the branch's elem refs are
        /// cleared, otherwise the target element can no longer be found.
        /// </summary>
        public static void UnbindGatedEvents(GraphDescriptor desc, GraphState state, int gateRef)
        {
            if (object.IsNullOrUndefined(state.EventListeners)) return;
            int n = desc.NodeCount;
            for (int j = 0; j < n; j++)
            {
                if (desc.GateIndices[j] != gateRef || desc.NodeTypes[j] != GraphNodeType.EventBinding) continue;
                Action<Element, ElementEvent> handler = (Action<Element, ElementEvent>)state.EventListeners[j];
                if (object.IsNullOrUndefined(handler)) continue;
                EventTargetInfo evtInfo = (EventTargetInfo)desc.TargetInfos[j];
                if (!object.IsNullOrUndefined(evtInfo))
                {
                    Element elem = GraphEngine.GetEventElement(state, evtInfo.ElemIdx);
                    if (!object.IsNullOrUndefined(elem)) elem.UnBind(evtInfo.EventName, handler);
                }
                state.EventListeners[j] = null;
            }
        }

        /// <summary>
        /// Parses a row template into every top-level node (elements and text, in order),
        /// copied out of the live list. A row is all of them, not just the first: a
        /// <c>&lt;td/&gt;&lt;td/&gt;</c> row or a void root's trailing event marker has
        /// several. Null for an empty template.
        /// </summary>
        public static NativeArray<Element> ParseTemplateHtml(string html, Element referenceElement)
        {
            if (object.IsNullOrUndefined(html) || html == "")
                return null;

            NativeArray<Node> children = GraphEngine.ParseFragmentNodes(referenceElement.OwnerDocument, html);
            NativeArray<Element> nodes = new NativeArray<Element>(0);
            for (int k = 0; k < children.Length; k++) nodes.Push((Element)children[k]);
            return nodes.Length > 0 ? nodes : null;
        }

        /// <summary>
        /// The node a row starts at. A gate renders its branch just before its marker, so
        /// when the row's first node is an open gate's marker the row starts at that
        /// branch's first node (and again if that node is itself an open gate's marker).
        /// </summary>
        private static Node RowStart(GraphDescriptor itemDesc, GraphState row, NativeArray<Element> rowNodes)
        {
            Node start = rowNodes[0];
            if (object.IsNullOrUndefined(itemDesc) || object.IsNullOrUndefined(row)
                || object.IsNullOrUndefined(row.GateElements))
                return start;

            bool moved = true;
            while (moved)
            {
                moved = false;
                for (int g = 0; g < itemDesc.NodeCount; g++)
                {
                    if (itemDesc.NodeTypes[g] != GraphNodeType.Gate) continue;
                    GateTargetInfo gate = (GateTargetInfo)itemDesc.TargetInfos[g];
                    Node branch = (Node)row.GateElements[g];
                    if (object.IsNullOrUndefined(gate) || object.IsNullOrUndefined(branch)) continue;
                    if ((object)row.ElemRefs[gate.MarkerIdx] != (object)start) continue;
                    start = branch;
                    moved = true;
                    break;
                }
            }

            return start;
        }

        /// <summary>
        /// Deep-clones every template node and inserts the copies, in order, before
        /// <paramref name="refNode"/>. Returns the new row's top-level nodes.
        /// </summary>
        private static NativeArray<Element> InsertRow(NativeArray<Element> template, Node parent, Node refNode)
        {
            NativeArray<Element> row = new NativeArray<Element>(template.Length);
            for (int k = 0; k < template.Length; k++)
            {
                row[k] = template[k].CloneNode(true);
                parent.InsertBefore(row[k], refNode);
            }

            return row;
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
                if (!object.IsNullOrUndefined(childState.GateElements[i]))
                    GraphEngine.RemoveGateBranch(childState, i);
            }
        }

        /// <summary>
        /// After a gate renders a branch template, resolves child elem refs from the
        /// rendered DOM and updates state.ElemRefs. Elements inside gate branches don't
        /// exist in the static HTML — they're created dynamically when the gate renders.
        /// Uses CollectSpanElements to find marker spans in the rendered template.
        /// </summary>
        public static void ResolveGateChildElems(
            GraphDescriptor desc, GraphState state, GateTargetInfo gateInfo, int gateNodeIdx, bool gateIsOpen)
        {
            NativeArray<int> childIndices = gateIsOpen
                ? gateInfo.TrueChildElemIndices
                : gateInfo.FalseChildElemIndices;

            if (object.IsNullOrUndefined(childIndices) || childIndices.Length == 0)
                return;

            NativeArray<Element> nodes = (NativeArray<Element>)state.GateExtraElements[gateNodeIdx];
            if (object.IsNullOrUndefined(nodes))
            {
                nodes = new NativeArray<Element>(0);
                nodes.Push((Element)state.GateElements[gateNodeIdx]);
            }
            NativeArray markers = GraphEngine.CollectGateSpanElements(nodes);

            // Slots of gates nested in this branch are not in the branch DOM yet.
            NativeArray<bool> nested = GraphEngine.GatedElemSlots(desc, childIndices);
            int m = 0;
            for (int k = 0; k < childIndices.Length; k++)
            {
                int slot = childIndices[k];
                if (slot < nested.Length && nested[slot]) continue;
                if (m >= markers.Length)
                    throw new Exception("Gate branch has fewer element markers (" + markers.Length
                        + ") than element slots; the template HTML and its graph are out of step.");
                state.ElemRefs[slot] = markers[m];
                m++;
            }

            if (m != markers.Length)
                throw new Exception("Gate branch has " + markers.Length + " element markers but "
                    + m + " element slots; the template HTML and its graph are out of step.");

            GraphEngine.WireTwoWayValues(desc, state);
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
