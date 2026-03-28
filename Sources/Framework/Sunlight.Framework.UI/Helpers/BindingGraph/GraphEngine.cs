namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;

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
                int gateIdx = desc.GateIndices[i];
                if (gateIdx >= 0 && !state.GateOpen[gateIdx])
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
                    // Evaluate gate condition (like a Property node), then set gateOpen[i].
                    object parentVal = GraphEngine.FindParentValue(desc, state, i);
                    Func<object, object> getter = desc.Getters[i];
                    object condVal = null;
                    if (!object.IsNullOrUndefined(parentVal) && !object.IsNullOrUndefined(getter))
                    {
                        condVal = getter(parentVal);
                    }

                    state.Values[i] = condVal;

                    bool gateIsOpen = !object.IsNullOrUndefined(condVal) && (bool)condVal;
                    bool wasOpen = state.GateOpen[i];
                    state.GateOpen[i] = gateIsOpen;

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
                    // Get method reference from parent (wiring handled separately).
                    object methodRef = GraphEngine.FindParentValue(desc, state, i);
                    state.Values[i] = methodRef;
                }
                // CollectionManager: handled by a separate subsystem; value is set externally.
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
            state.FlushScheduled = false;
            int n = desc.NodeCount;

            for (int i = 0; i < n; i++)
            {
                // Skip clean nodes.
                if (!state.Dirty[i])
                {
                    continue;
                }

                // Skip gated nodes whose gate is closed.
                int gateIdx = desc.GateIndices[i];
                if (gateIdx >= 0 && !state.GateOpen[gateIdx])
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
                else if (nodeType == GraphNodeType.DomTarget)
                {
                    newVal = GraphEngine.FindParentValue(desc, state, i);
                }
                else if (nodeType == GraphNodeType.EventBinding)
                {
                    newVal = GraphEngine.FindParentValue(desc, state, i);
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
                    bool gateIsOpen = !object.IsNullOrUndefined(newVal) && (bool)newVal;
                    bool wasOpen = state.GateOpen[i];
                    state.GateOpen[i] = gateIsOpen;

                    if (!gateIsOpen && wasOpen)
                    {
                        GraphEngine.ApplyGateClosure(desc, state, i);
                    }
                    else if (gateIsOpen && !wasOpen)
                    {
                        GraphEngine.MarkGatedConsumersDirty(desc, state, i);
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
        /// Scans the consumers adjacency list to find which node lists nodeIdx as a consumer.
        /// Since nodes are in topological order, the parent is always at a lower index.
        /// O(nodes * edges) — acceptable for small graphs (&lt;50 nodes).
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        /// <param name="nodeIdx">The index of the node whose parent value to find.</param>
        /// <returns>The cached value of the parent node, or null if no parent found.</returns>
        public static object FindParentValue(GraphDescriptor desc, GraphState state, int nodeIdx)
        {
            // Parent must be at a lower index (topological order).
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
        /// Applies gate closure: sets defaults on all nodes gated by this gate.
        /// Called when a gate node transitions from open to closed.
        /// </summary>
        /// <param name="desc">The static graph descriptor.</param>
        /// <param name="state">The per-instance graph state.</param>
        /// <param name="gateIdx">The index of the gate node that closed.</param>
        public static void ApplyGateClosure(GraphDescriptor desc, GraphState state, int gateIdx)
        {
            int n = desc.NodeCount;

            for (int i = gateIdx + 1; i < n; i++)
            {
                if (desc.GateIndices[i] == gateIdx)
                {
                    state.Values[i] = desc.DefaultValues[i];
                    state.Dirty[i] = false;

                    // If DomTarget: apply default to DOM.
                    if (desc.NodeTypes[i] == GraphNodeType.DomTarget)
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
    }
}
