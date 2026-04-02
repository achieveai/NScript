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

                // Check if this is a chained subscription
                if (!object.IsNullOrUndefined(entry.PathSegments) && entry.PathSegments.Length > 1)
                {
                    WireChainSubscription(i, entry, source);
                    continue;
                }

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

        /// <summary>
        /// Creates a chain callback that marks the node dirty and flushes.
        /// Each callback is created in its own method scope to avoid JS closure-in-loop bugs.
        /// </summary>
        private static Action<INotifyPropertyChanged, string> CreateChainCallback(
            GraphState state, GraphDescriptor descriptor, int nodeIdx)
        {
            return delegate(INotifyPropertyChanged sender, string propName)
            {
                state.Dirty[nodeIdx] = true;
                GraphEngine.Flush(descriptor, state);
            };
        }

        /// <summary>
        /// Wires a chained property path subscription (e.g., Customer.Address.City).
        /// Subscribes to the root property's PropertyChanged on the source object.
        /// When any segment changes, the node is marked dirty and flushed.
        /// </summary>
        private void WireChainSubscription(int subIdx, SubscriptionEntry entry, object rootSource)
        {
            NativeArray<string> pathSegments = entry.PathSegments;
            int pathLen = pathSegments.Length;

            // Ensure chain arrays are big enough
            if (this.state.ChainLiveObjects.Length <= subIdx)
            {
                NativeArray<NativeArray> newLive = new NativeArray<NativeArray>(subIdx + 1);
                NativeArray<NativeArray> newListeners = new NativeArray<NativeArray>(subIdx + 1);
                for (int k = 0; k < this.state.ChainLiveObjects.Length; k++)
                {
                    newLive[k] = this.state.ChainLiveObjects[k];
                    newListeners[k] = this.state.ChainListeners[k];
                }
                this.state.ChainLiveObjects = newLive;
                this.state.ChainListeners = newListeners;
            }

            NativeArray liveObjects = new NativeArray(pathLen);
            NativeArray chainCallbacks = new NativeArray(pathLen);
            this.state.ChainLiveObjects[subIdx] = liveObjects;
            this.state.ChainListeners[subIdx] = chainCallbacks;

            // Subscribe to root property on the source object
            liveObjects[0] = rootSource;
            INotifyPropertyChanged obs = rootSource as INotifyPropertyChanged;
            if (!object.IsNullOrUndefined(obs))
            {
                Action<INotifyPropertyChanged, string> chainCallback =
                    GraphBindingStrategy.CreateChainCallback(this.state, this.descriptor, entry.NodeIdx);
                obs.AddPropertyChangedListener(pathSegments[0], chainCallback);
                chainCallbacks[0] = chainCallback;
            }
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

            // Clean up chain subscriptions
            if (!object.IsNullOrUndefined(this.state.ChainLiveObjects))
            {
                for (int i = 0; i < this.state.ChainLiveObjects.Length; i++)
                {
                    NativeArray liveObjects = this.state.ChainLiveObjects[i];
                    NativeArray chainCbs = this.state.ChainListeners[i];
                    if (object.IsNullOrUndefined(liveObjects) || object.IsNullOrUndefined(chainCbs)) continue;

                    NativeArray subs = this.descriptor.Subscriptions;
                    SubscriptionEntry entry = (SubscriptionEntry)subs[i];
                    NativeArray<string> pathSegments = entry.PathSegments;
                    if (object.IsNullOrUndefined(pathSegments)) continue;

                    for (int seg = 0; seg < liveObjects.Length; seg++)
                    {
                        object obj = liveObjects[seg];
                        if (object.IsNullOrUndefined(obj)) continue;
                        INotifyPropertyChanged obs = obj as INotifyPropertyChanged;
                        if (object.IsNullOrUndefined(obs)) continue;
                        if (seg < chainCbs.Length && !object.IsNullOrUndefined(chainCbs[seg]))
                        {
                            obs.RemovePropertyChangedListener(
                                pathSegments[seg],
                                (Action<INotifyPropertyChanged, string>)chainCbs[seg]);
                        }
                    }
                    this.state.ChainLiveObjects[i] = null;
                    this.state.ChainListeners[i] = null;
                }
            }
        }
    }
}
