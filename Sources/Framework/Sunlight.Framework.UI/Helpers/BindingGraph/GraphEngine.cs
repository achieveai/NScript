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

                    // DOM operations: clone and insert the appropriate template branch.
                    GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(gateInfo))
                    {
                        Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];
                        Element template = gateIsOpen
                            ? (Element)gateInfo.TrueTemplate
                            : (Element)gateInfo.FalseTemplate;

                        if (!object.IsNullOrUndefined(template) && !object.IsNullOrUndefined(marker))
                        {
                            Element clone = template.CloneNode(true);
                            Node parent = marker.ParentNode;
                            if (!object.IsNullOrUndefined(parent))
                            {
                                parent.InsertBefore(clone, marker);
                            }
                            state.GateElements[i] = clone;
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
                    // Get method reference from parent, wire addEventListener.
                    object methodRef = GraphEngine.FindParentValue(desc, state, i);
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
                    // Get collection from parent value.
                    object collection = GraphEngine.FindParentValue(desc, state, i);
                    state.Values[i] = collection;

                    CollectionTargetInfo colInfo = (CollectionTargetInfo)desc.TargetInfos[i];
                    if (!object.IsNullOrUndefined(colInfo) && !object.IsNullOrUndefined(collection))
                    {
                        GraphEngine.RenderCollection(desc, state, i, colInfo, (IObservableCollection)collection);
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
                else if (nodeType == GraphNodeType.CollectionManager)
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

                    // DOM swap: remove old branch, insert new branch.
                    if (gateIsOpen != wasOpen)
                    {
                        GateTargetInfo gateInfo = (GateTargetInfo)desc.TargetInfos[i];
                        if (!object.IsNullOrUndefined(gateInfo))
                        {
                            // Remove current branch element.
                            Element oldElem = (Element)state.GateElements[i];
                            if (!object.IsNullOrUndefined(oldElem))
                            {
                                oldElem.Remove();
                                state.GateElements[i] = null;
                            }

                            // Insert new branch template.
                            Element template = gateIsOpen
                                ? (Element)gateInfo.TrueTemplate
                                : (Element)gateInfo.FalseTemplate;
                            Element marker = (Element)state.ElemRefs[gateInfo.MarkerIdx];

                            if (!object.IsNullOrUndefined(template) && !object.IsNullOrUndefined(marker))
                            {
                                Element clone = template.CloneNode(true);
                                Node parent = marker.ParentNode;
                                if (!object.IsNullOrUndefined(parent))
                                {
                                    parent.InsertBefore(clone, marker);
                                }
                                state.GateElements[i] = clone;
                            }
                        }
                    }

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
                        GraphEngine.ClearCollectionItems(state, i, colInfo);

                        // Detach old collection listener.
                        if (!object.IsNullOrUndefined(oldVal))
                        {
                            GraphEngine.DetachCollectionListener(state, i, oldVal);
                        }

                        // Render new collection.
                        if (!object.IsNullOrUndefined(newVal))
                        {
                            GraphEngine.RenderCollection(desc, state, i, colInfo, (IObservableCollection)newVal);
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
                Element template = (Element)colInfo.ItemTemplate;
                if (object.IsNullOrUndefined(template)) continue;

                Element clone = template.CloneNode(true);
                parent.InsertBefore(clone, marker);
                itemElems[idx] = clone;

                // Create child graph if item graph descriptor exists.
                if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                {
                    NativeArray childElemRefs = new NativeArray(1);
                    childElemRefs[0] = clone;
                    GraphState childState = new GraphState(colInfo.ItemGraph, childElemRefs, state.Depth + 1);
                    childState.Sources[GraphSourceSlot.DataContext] = item;
                    GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
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
        public static void ClearCollectionItems(GraphState state, int nodeIdx, CollectionTargetInfo colInfo)
        {
            NativeArray itemElems = state.ItemElements[nodeIdx];
            if (!object.IsNullOrUndefined(itemElems))
            {
                for (int idx = 0; idx < itemElems.Length; idx++)
                {
                    Element elem = (Element)itemElems[idx];
                    if (!object.IsNullOrUndefined(elem))
                    {
                        elem.Remove();
                    }
                }
            }

            // Dispose child graph states.
            NativeArray<GraphState> childStates = state.ChildGraphStates[nodeIdx];
            if (!object.IsNullOrUndefined(childStates))
            {
                for (int idx = 0; idx < childStates.Length; idx++)
                {
                    GraphState child = childStates[idx];
                    if (!object.IsNullOrUndefined(child))
                    {
                        // Clear child state values.
                        for (int v = 0; v < child.Values.Length; v++)
                        {
                            child.Values[v] = null;
                        }
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
                    Element template = (Element)colInfo.ItemTemplate;
                    Element clone = template.CloneNode(true);
                    parent.InsertBefore(clone, refNode);
                    newItemElems[insertIdx + j] = clone;

                    if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                    {
                        NativeArray childElemRefs = new NativeArray(1);
                        childElemRefs[0] = clone;
                        GraphState childState = new GraphState(
                            colInfo.ItemGraph, childElemRefs, state.Depth + 1);
                        childState.Sources[GraphSourceSlot.DataContext] = item;
                        GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
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
                for (int j = removeIdx + removeCount - 1; j >= removeIdx; j--)
                {
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
            else if (args.Action == CollectionChangedAction.Reset)
            {
                // Full reset: clear and re-render.
                GraphEngine.ClearCollectionItems(state, nodeIdx, colInfo);
                object collection = state.Values[nodeIdx];
                if (!object.IsNullOrUndefined(collection))
                {
                    IObservableCollection obsCol = (IObservableCollection)collection;
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
                                Element template = (Element)colInfo.ItemTemplate;
                                Element clone = template.CloneNode(true);
                                resetParent.InsertBefore(clone, resetMarker);
                                newElems[idx] = clone;

                                if (!object.IsNullOrUndefined(colInfo.ItemGraph))
                                {
                                    NativeArray childElemRefs = new NativeArray(1);
                                    childElemRefs[0] = clone;
                                    GraphState childState = new GraphState(
                                        colInfo.ItemGraph, childElemRefs, state.Depth + 1);
                                    childState.Sources[GraphSourceSlot.DataContext] = item;
                                    GraphEngine.PushInitialValues(colInfo.ItemGraph, childState);
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
                        GraphEngine.ClearCollectionItems(state, i, colInfo);
                    }
                }
            }
        }
    }
}
