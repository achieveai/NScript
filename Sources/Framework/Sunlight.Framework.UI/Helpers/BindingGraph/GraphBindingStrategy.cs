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

        public void Dispose()
        {
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
                if (this.descriptor.NodeTypes[i] == GraphNodeType.Gate)
                {
                    object gateElem = this.state.GateElements[i];
                    if (!object.IsNullOrUndefined(gateElem))
                    {
                        ((System.Web.Html.Element)gateElem).Remove();
                        this.state.GateElements[i] = null;
                    }
                }

                this.state.Values[i] = null;
                this.state.Dirty[i] = false;
            }

            this.state.ElemRefs = null;
            this.state.Sources[0] = null;
            this.state.Sources[1] = null;
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
                // Synchronous flush: mark node dirty and evaluate immediately.
                // This ensures DOM is up-to-date before the next line of application
                // code executes. The reentrancy guard in Flush() (try-finally)
                // prevents cascading flushes if a DOM write triggers further changes.
                state.Dirty[nodeIdx] = true;
                GraphEngine.Flush(descriptor, state);
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
