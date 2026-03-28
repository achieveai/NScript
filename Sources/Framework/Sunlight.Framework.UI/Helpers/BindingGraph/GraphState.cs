namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Per-instance mutable state for a binding graph. Created when SkinInstance
    /// is instantiated. Disposed with the instance.
    /// </summary>
    public class GraphState
    {
        /// <summary>Cached output value per node.</summary>
        public NativeArray Values;

        /// <summary>Dirty flag per node.</summary>
        public NativeArray<bool> Dirty;

        /// <summary>Gate-open flag per node (only meaningful for Gate nodes).</summary>
        public NativeArray<bool> GateOpen;

        /// <summary>Source objects: [0]=DataContext, [1]=TemplateParent.</summary>
        public NativeArray Sources;

        /// <summary>DOM element references resolved from the cloned template.</summary>
        public NativeArray ElemRefs;

        /// <summary>Active PropertyChanged listener handles for cleanup.</summary>
        public NativeArray Listeners;

        /// <summary>Number of active listeners (for cleanup iteration).</summary>
        public int ListenerCount;

        /// <summary>Whether subscriptions have been wired (Phase 2 complete).</summary>
        public bool SubscriptionsActive;

        /// <summary>Whether a flush is scheduled for this graph.</summary>
        public bool FlushScheduled;

        /// <summary>Depth in the skin nesting hierarchy (0 = root).</summary>
        public int Depth;

        /// <summary>Back-reference to the static descriptor.</summary>
        public GraphDescriptor Descriptor;

        /// <summary>Active cloned DOM element per Gate node (null for non-gate nodes).</summary>
        public NativeArray GateElements;

        /// <summary>Active event listener handles per EventBinding node (for cleanup).</summary>
        public NativeArray EventListeners;

        /// <summary>Per-CollectionManager: child graph states per item.</summary>
        public NativeArray<NativeArray<GraphState>> ChildGraphStates;

        /// <summary>Per-CollectionManager: item element arrays per item.</summary>
        public NativeArray<NativeArray> ItemElements;

        /// <summary>Per-CollectionManager: collection change listener reference.</summary>
        public NativeArray CollectionListeners;

        /// <summary>
        /// Creates a new GraphState for the given descriptor.
        /// </summary>
        public GraphState(GraphDescriptor descriptor, NativeArray elemRefs, int depth)
        {
            this.Descriptor = descriptor;
            this.ElemRefs = elemRefs;
            this.Depth = depth;

            int n = descriptor.NodeCount;
            this.Values = new NativeArray(n);
            this.Dirty = new NativeArray<bool>(n);
            this.GateOpen = new NativeArray<bool>(n);
            this.Sources = new NativeArray(2);
            this.Listeners = new NativeArray(0);
            this.ListenerCount = 0;
            this.SubscriptionsActive = false;
            this.FlushScheduled = false;

            this.GateElements = new NativeArray(n);
            this.EventListeners = new NativeArray(n);
            this.ChildGraphStates = new NativeArray<NativeArray<GraphState>>(n);
            this.ItemElements = new NativeArray<NativeArray>(n);
            this.CollectionListeners = new NativeArray(n);

            // All gates start open (true). Gate.Evaluate during initial push
            // will close gates whose condition is false.
            for (int i = 0; i < n; i++)
            {
                this.GateOpen[i] = true;
            }
        }
    }
}
