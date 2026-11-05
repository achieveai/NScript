namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using Sunlight.Framework.Observables;

    public class GraphBindingStrategy : IBindingStrategy
    {
        private GraphDescriptor descriptor;
        private GraphState state;

        public GraphBindingStrategy(GraphDescriptor descriptor, NativeArray elemRefs, int depth,
            System.Web.Html.Element rootElement)
        {
            this.descriptor = descriptor;
            this.state = new GraphState(descriptor, elemRefs, depth);
            GraphEngine.CreateSubControls(descriptor, this.state, rootElement, null);
        }

        public void PushInitialValues(object dataContext, object templateParent, NativeArray elementsOfInterest)
        {
            this.state.Sources[GraphSourceSlot.DataContext] = dataContext;
            this.state.Sources[GraphSourceSlot.TemplateParent] = templateParent;
            this.state.Suspended = false;

            // Capture changes raised by a child setter or Activate() while the
            // initial graph is still being populated, then reconcile them below.
            this.state.Flushing = true;
            try
            {
                WireSubscriptions(dataContext, templateParent);
                GraphEngine.SetDefaultSubControlDataContext(this.descriptor, this.state, dataContext);
                GraphEngine.PushInitialValues(this.descriptor, this.state);
                GraphEngine.ApplySubControlBindings(this.descriptor, this.state);
                GraphEngine.ActivateSubControls(this.descriptor, this.state);
            }
            finally
            {
                this.state.Flushing = false;
            }

            GraphEngine.Flush(this.descriptor, this.state);
        }

        public void WireSubscriptions(object dataContext, object templateParent)
        {
            if (this.state.SubscriptionsActive) return;

            // Update sources to current values
            this.state.Sources[GraphSourceSlot.DataContext] = dataContext;
            this.state.Sources[GraphSourceSlot.TemplateParent] = templateParent;

            NativeArray subscriptions = this.descriptor.Subscriptions;
            if (object.IsNullOrUndefined(subscriptions)) return;

            int subCount = subscriptions.Length;
            NativeArray listeners = new NativeArray(subCount);

            for (int i = 0; i < subCount; i++)
            {
                SubscriptionEntry entry = (SubscriptionEntry)subscriptions[i];

                if (GraphEngine.IsChainedEntry(entry))
                {
                    if (object.IsNullOrUndefined(this.state.ChainListeners))
                        this.state.ChainListeners =
                            new NativeArray<NativeArray<ChainListenerHandle>>(subCount);
                    GraphEngine.WireChainedSubscription(this.state, entry, i);
                    listeners[i] = null;
                    continue;
                }

                object source = this.state.Sources[entry.SourceSlot];
                if (object.IsNullOrUndefined(source)) continue;

                INotifyPropertyChanged observable = source as INotifyPropertyChanged;
                if (object.IsNullOrUndefined(observable)) continue;

                // IMPORTANT: Create callback via a separate method to avoid JS closure-in-loop bug.
                // NScript compiles C# locals as function-scoped `var` — all loop iterations
                // share the same variable binding. By calling a method, each gets its own scope.
                Action<INotifyPropertyChanged, string> callback =
                    GraphBindingStrategy.CreatePropertyCallback(this.state, this.descriptor, entry.NodeIdx);

                observable.AddPropertyChangedListener(entry.PropertyName, callback);
                listeners[i] = callback;
            }

            this.state.Listeners = listeners;
            this.state.ListenerCount = subCount;
            this.state.SubscriptionsActive = true;
        }

        public void OnDataContextChanged(object newDataContext)
        {
            bool wasSubscribed = this.state.SubscriptionsActive;
            if (wasSubscribed)
            {
                UnsubscribeAll();
                this.state.SubscriptionsActive = false;
            }

            this.state.Sources[GraphSourceSlot.DataContext] = newDataContext;
            if (this.state.Suspended) return;
            GraphEngine.SetDefaultSubControlDataContext(this.descriptor, this.state, newDataContext);

            if (wasSubscribed)
                WireSubscriptions(newDataContext, this.state.Sources[GraphSourceSlot.TemplateParent]);

            // Mark source node dirty and flush synchronously
            this.state.Dirty[0] = true;
            GraphEngine.Flush(this.descriptor, this.state);
        }

        public void OnTemplateParentChanged(object newTemplateParent)
        {
            bool wasSubscribed = this.state.SubscriptionsActive;
            if (wasSubscribed)
            {
                UnsubscribeAll();
                this.state.SubscriptionsActive = false;
            }

            this.state.Sources[GraphSourceSlot.TemplateParent] = newTemplateParent;
            if (this.state.Suspended) return;

            if (wasSubscribed)
                WireSubscriptions(this.state.Sources[GraphSourceSlot.DataContext], newTemplateParent);

            // Mark source node dirty and flush synchronously
            this.state.Dirty[0] = true;
            NativeArray<int> getterSources = this.descriptor.GetterSourceSlots;
            if (!object.IsNullOrUndefined(getterSources))
                for (int i = 0; i < getterSources.Length; i++)
                    if (getterSources[i] == GraphSourceSlot.TemplateParent)
                        this.state.Dirty[i] = true;
            GraphEngine.Flush(this.descriptor, this.state);
        }

        public void Deactivate()
        {
            UnsubscribeAll();
            this.state.SubscriptionsActive = false;
            GraphEngine.DeactivateSubControls(this.descriptor, this.state);
        }

        public void SetDepth(int depth)
        {
            this.state.Depth = depth;

            // The constructor created this graph's sub-controls at the default
            // depth before SkinInstance.Bind could tell us the real one; their
            // own skins are not instantiated until activation, so re-stamping
            // here is enough for them to pick up the corrected depth.
            NativeArray<UIElement> controls = this.state.SubControlInstances;
            if (object.IsNullOrUndefined(controls)) return;
            for (int i = 0; i < controls.Length; i++)
            {
                if (!object.IsNullOrUndefined(controls[i]))
                    controls[i].BindingDepth = depth + 1;
            }
        }

        public void Dispose()
        {
            this.state.Disposed = true;
            UnsubscribeAll();
            this.state.SubscriptionsActive = false;

            // Clean up event listeners and collection listeners.
            GraphEngine.CleanupEventListeners(this.descriptor, this.state);
            GraphEngine.CleanupCollectionListeners(this.descriptor, this.state);
            GraphEngine.DisposeSubControls(this.descriptor, this.state);

            int n = this.descriptor.NodeCount;
            for (int i = 0; i < n; i++)
            {
                // Remove gate elements from DOM.
                if (this.descriptor.NodeTypes[i] == GraphNodeType.Gate
                    && !object.IsNullOrUndefined(this.state.GateElements[i]))
                {
                    GraphEngine.RemoveGateBranch(this.state, i);
                }

                this.state.Values[i] = null;
                this.state.Dirty[i] = false;
            }

            this.state.ElemRefs = null;
            this.state.Sources[0] = null;
            this.state.Sources[1] = null;
        }

        /// <summary>
        /// Takes the caller content of a host marked <c>data-ns-content</c> (a Razor
        /// <c>&lt;X&gt;...&lt;/X&gt;</c>) into detached holders, before the skin's nodes are
        /// appended. Throws first, moving nothing, when the skin has no outlet for it.
        /// The caller's graph keeps its refs and listeners on the moved nodes.
        /// </summary>
        public void TakeContent(
            System.Web.Html.Element host, System.Web.Html.Element skinRoot, string skinId, string controlName)
        {
            if (!host.HasAttribute("data-ns-content") || !host.HasChildNodes()) return;

            // <Slot Name="x"> boundary markers must be the host's own children.
            NativeArray<System.Web.Html.Element> markers = host.QuerySelectorAll(SlotContentStartSelector);
            for (int m = 0; m < markers.Length; m++)
            {
                System.Web.Html.Node owner = markers[m].ParentNode;
                while (owner != (System.Web.Html.Node)host && !owner.HasAttribute("data-ns-content"))
                    owner = owner.ParentNode;
                if (owner == (System.Web.Html.Node)host && markers[m].ParentNode != (System.Web.Html.Node)host)
                    throw new Exception("<Slot Name=\"" + markers[m].GetAttribute("data-ns-slot-for")
                        + "\"> must sit directly inside <" + controlName + ">, not inside another element.");
            }

            // First pass: which slot each child goes to, and whether the skin has its outlet.
            // Nothing moves until all checks pass. Content outside any <Slot> is the default slot.
            NativeArray<System.Web.Html.Node> children = host.ChildNodes;
            NativeArray<System.Web.Html.Node> nodes = new NativeArray<System.Web.Html.Node>(0);
            NativeArray<string> owners = new NativeArray<string>(0);
            NativeArray<SlotProjection> slots = new NativeArray<SlotProjection>(0);
            slots.Push(GraphBindingStrategy.NewSlot(host, ""));
            string current = null;
            bool defaultHasContent = false;
            for (int k = 0; k < children.Length; k++)
            {
                System.Web.Html.Node node = children[k];
                System.Web.Html.Element element = node.NodeType == System.Web.Html.ElementType.Element
                    ? node.As<System.Web.Html.Element>() : null;
                if (element != null && element.Matches(SlotContentStartSelector))
                {
                    string slotName = element.GetAttribute("data-ns-slot-for");
                    if (current != null)
                        throw new Exception("<Slot Name=\"" + slotName + "\"> is inside <Slot Name=\""
                            + current + "\"> in <" + controlName + ">; slots do not nest.");
                    if (GraphBindingStrategy.FindSlot(slots, slotName) != null)
                        throw new Exception("<" + controlName + "> has two <Slot Name=\"" + slotName + "\">.");
                    if (!this.DeclaresOutlet(skinRoot, slotName))
                        throw new Exception("Skin '" + skinId + "' for " + controlName + " has no @RenderSlot(\""
                            + slotName + "\"); remove the <Slot Name=\"" + slotName + "\"> content or add the outlet.");
                    slots.Push(GraphBindingStrategy.NewSlot(host, slotName));
                    current = slotName;
                    nodes.Push(node);
                    owners.Push(slotName);
                    continue;
                }

                if (element != null && element.Matches(SlotContentEndSelector))
                {
                    if (current == null)
                        throw new Exception("<" + controlName + "> has a </Slot> with no <Slot Name=\"...\"> before it.");
                    nodes.Push(node);
                    owners.Push(current);
                    current = null;
                    continue;
                }

                if (current == null && (element != null
                    || (node.NodeType == System.Web.Html.ElementType.Text && node.TextContent.Trim().Length > 0)))
                    defaultHasContent = true;
                nodes.Push(node);
                owners.Push(current == null ? "" : current);
            }

            if (defaultHasContent && !this.DeclaresOutlet(skinRoot, ""))
                throw new Exception("Skin '" + skinId + "' for " + controlName
                    + " has no @RenderBody(); remove the content or add the outlet.");

            // Second pass: move. A named slot's markers stay out of its outlet.
            for (int k = 0; k < nodes.Length; k++)
            {
                SlotProjection slot = GraphBindingStrategy.FindSlot(slots, owners[k]);
                System.Web.Html.Node node = nodes[k];
                if (slot.Name != "" && object.IsNullOrUndefined(slot.ContentStart))
                {
                    slot.ContentStart = node.As<System.Web.Html.Element>();
                    host.RemoveChild(node);
                }
                else if (slot.Name != "" && node.NodeType == System.Web.Html.ElementType.Element
                    && node.As<System.Web.Html.Element>().Matches(SlotContentEndSelector))
                {
                    slot.ContentEnd = node.As<System.Web.Html.Element>();
                    host.RemoveChild(node);
                }
                else
                {
                    slot.Holder.AppendChild(node);
                }
            }

            this.state.Projections = slots;
        }

        private const string SlotContentStartSelector = "template[data-ns-slot-for]";
        private const string SlotContentEndSelector = "template[data-ns-slot-end]";

        private static SlotProjection NewSlot(System.Web.Html.Element host, string name)
        {
            SlotProjection slot = new SlotProjection();
            slot.Name = name;
            slot.Holder = host.OwnerDocument.CreateElement("div");
            return slot;
        }

        private static SlotProjection FindSlot(NativeArray<SlotProjection> slots, string name)
        {
            for (int k = 0; k < slots.Length; k++)
                if (slots[k].Name == name) return slots[k];
            return null;
        }

        /// <summary>Places the caller content into a static outlet, once the skin is in the host.</summary>
        public void PlaceContent(System.Web.Html.Element host)
        {
            GraphEngine.PlaceProjections(this.state, host);
        }

        /// <summary>
        /// Puts the caller content back into the host, after the skin's nodes left it, so the
        /// host is as it was before Bind and a new skin can take the content again.
        /// </summary>
        public void ReturnContent(System.Web.Html.Element host)
        {
            NativeArray<SlotProjection> slots = this.state.Projections;
            if (object.IsNullOrUndefined(slots)) return;
            GraphEngine.ParkProjections(this.state);
            for (int k = 0; k < slots.Length; k++)
            {
                SlotProjection slot = slots[k];
                if (!object.IsNullOrUndefined(slot.ContentStart)) host.AppendChild(slot.ContentStart);
                while (!object.IsNullOrUndefined(slot.Holder.FirstChild))
                    host.AppendChild(slot.Holder.FirstChild);
                if (!object.IsNullOrUndefined(slot.ContentEnd)) host.AppendChild(slot.ContentEnd);
            }

            this.state.Projections = null;
        }

        /// <summary>
        /// An outlet for <paramref name="name"/> exists statically in the skin's root, or in
        /// any @if/@else branch template (nested branches have their own templates).
        /// </summary>
        private bool DeclaresOutlet(System.Web.Html.Element skinRoot, string name)
        {
            if (!object.IsNullOrUndefined(skinRoot.QuerySelector(GraphEngine.SlotAnchorSelector(name))))
                return true;

            string anchor = "data-ns-slot=\"" + name + "\"";
            for (int i = 0; i < this.descriptor.NodeCount; i++)
            {
                if (this.descriptor.NodeTypes[i] != GraphNodeType.Gate) continue;
                GateTargetInfo gate = (GateTargetInfo)this.descriptor.TargetInfos[i];
                if (object.IsNullOrUndefined(gate)) continue;
                string whenTrue = (string)gate.TrueTemplate;
                string whenFalse = (string)gate.FalseTemplate;
                if ((!object.IsNullOrUndefined(whenTrue) && whenTrue.IndexOf(anchor) >= 0)
                    || (!object.IsNullOrUndefined(whenFalse) && whenFalse.IndexOf(anchor) >= 0))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// No-op for graph mode: data context flags are managed internally.
        /// </summary>
        public void OnDataContextUpdated(bool dcUpdated, bool tpUpdated) { }

        /// <summary>
        /// No-op for graph mode: deactivation is handled by Deactivate().
        /// </summary>
        public void OnQueuedDeactivation(bool isActive, bool isDisposed) { }

        /// <summary>
        /// Creates a property change callback with its own closure scope.
        /// This avoids the JS closure-in-loop bug where all callbacks would share
        /// the same capturedNodeIdx variable (last loop value) if created inline.
        /// </summary>
        public static Action<INotifyPropertyChanged, string> CreatePropertyCallback(
            GraphState state, GraphDescriptor descriptor, int nodeIdx)
        {
            return delegate(INotifyPropertyChanged sender, string propName)
            {
                if (state.Suspended) return;
                // Default: synchronous flush so the DOM is up-to-date before the
                // next line of application code executes. With
                // GraphFlushCoordinator.BatchingEnabled the node is only marked
                // dirty and one depth-ordered flush runs on the next boundary.
                GraphEngine.NotifyDirty(state, nodeIdx);
            };
        }

        private void UnsubscribeAll()
        {
            NativeArray subscriptions = this.descriptor.Subscriptions;
            if (object.IsNullOrUndefined(subscriptions)) return;

            int subCount = subscriptions.Length;
            for (int i = 0; i < subCount; i++)
            {
                SubscriptionEntry entry = (SubscriptionEntry)subscriptions[i];

                if (GraphEngine.IsChainedEntry(entry))
                {
                    GraphEngine.UnwireChainedSubscription(this.state, i);
                    continue;
                }

                object source = this.state.Sources[entry.SourceSlot];
                if (object.IsNullOrUndefined(source)) continue;

                INotifyPropertyChanged observable = source as INotifyPropertyChanged;
                if (object.IsNullOrUndefined(observable)) continue;

                if (i < this.state.ListenerCount && !object.IsNullOrUndefined(this.state.Listeners[i]))
                {
                    observable.RemovePropertyChangedListener(
                        entry.PropertyName,
                        (Action<INotifyPropertyChanged, string>)this.state.Listeners[i]);
                }
            }
        }
    }
}
