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
            int listenerIdx = 0;

            for (int i = 0; i < subCount; i++)
            {
                SubscriptionEntry entry = (SubscriptionEntry)subscriptions[i];
                object source = this.state.Sources[entry.SourceSlot];
                if (object.IsNullOrUndefined(source)) continue;

                INotifyPropertyChanged observable = source as INotifyPropertyChanged;
                if (object.IsNullOrUndefined(observable)) continue;

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
