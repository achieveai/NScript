namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using Sunlight.Framework.Observables;

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

            // Wire subscriptions immediately so property changes propagate synchronously.
            // This is called again from SkinInstance.QueuedActivation, but WireSubscriptions
            // is idempotent (returns early if already active).
            WireSubscriptions(dataContext, templateParent);
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
            this.state.Sources[GraphSourceSlot.DataContext] = newDataContext;

            if (this.state.SubscriptionsActive)
            {
                UnsubscribeAll();
                this.state.SubscriptionsActive = false;
                WireSubscriptions(newDataContext, this.state.Sources[GraphSourceSlot.TemplateParent]);
            }

            // Mark source node dirty and flush synchronously
            this.state.Dirty[0] = true;
            GraphEngine.Flush(this.descriptor, this.state);
        }

        public void OnTemplateParentChanged(object newTemplateParent)
        {
            this.state.Sources[GraphSourceSlot.TemplateParent] = newTemplateParent;

            if (this.state.SubscriptionsActive)
            {
                UnsubscribeAll();
                this.state.SubscriptionsActive = false;
                WireSubscriptions(this.state.Sources[GraphSourceSlot.DataContext], newTemplateParent);
            }

            // Mark source node dirty and flush synchronously
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

            // Clean up event listeners and collection listeners.
            GraphEngine.CleanupEventListeners(this.descriptor, this.state);
            GraphEngine.CleanupCollectionListeners(this.descriptor, this.state);

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
        /// Creates a property change callback with its own closure scope.
        /// This avoids the JS closure-in-loop bug where all callbacks would share
        /// the same capturedNodeIdx variable (last loop value) if created inline.
        /// </summary>
        public static Action<INotifyPropertyChanged, string> CreatePropertyCallback(
            GraphState state, GraphDescriptor descriptor, int nodeIdx)
        {
            return delegate(INotifyPropertyChanged sender, string propName)
            {
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
