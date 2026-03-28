namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;

    /// <summary>
    /// Immutable static graph shape shared across all instances of a template.
    /// Created by compiled JS factory code. All arrays have length == nodeCount.
    /// </summary>
    public class GraphDescriptor
    {
        /// <summary>Number of nodes in the graph.</summary>
        public int NodeCount;

        /// <summary>Node type per index (GraphNodeType constants).</summary>
        public NativeArray<int> NodeTypes;

        /// <summary>
        /// Getter function per node. For Property/Computed/Gate nodes: (source) => value.
        /// For Source/DomTarget/EventBinding: null.
        /// </summary>
        public NativeArray<Func<object, object>> Getters;

        /// <summary>
        /// Adjacency list: Consumers[i] is an int[] of node indices that depend on node i.
        /// </summary>
        public NativeArray<NativeArray<int>> Consumers;

        /// <summary>
        /// Gate index for each node. -1 if ungated. If >= 0, the node is only active
        /// when gateOpen[gateIndices[i]] is true.
        /// </summary>
        public NativeArray<int> GateIndices;

        /// <summary>
        /// Default value per node, applied when gate closes or source is null.
        /// Typically "" for text, null for objects.
        /// </summary>
        public NativeArray DefaultValues;

        /// <summary>
        /// Target info per DomTarget/Gate/CollectionManager/EventBinding node.
        /// Null for non-target nodes.
        /// </summary>
        public NativeArray TargetInfos;

        /// <summary>
        /// Subscription map: array of SubscriptionEntry objects.
        /// Used during Phase 2 (deferred subscription wiring) to register PropertyChanged listeners.
        /// </summary>
        public NativeArray Subscriptions;

        /// <summary>
        /// Source type for type-checking at DataContext boundary.
        /// </summary>
        public Type SourceType;

        /// <summary>
        /// Subscription mode: GraphSubscribeMode.PerProperty or GraphSubscribeMode.AllProperties.
        /// </summary>
        public int SubscribeMode;

        /// <summary>
        /// Source slot that this graph's root Source node reads from.
        /// 0 = DataContext, 1 = TemplateParent.
        /// </summary>
        public int RootSourceSlot;

        /// <summary>
        /// Parent node indices per node. ParentIndices[i] is an int[] of node indices
        /// that feed into node i. Source nodes have empty parents. Property/DomTarget nodes
        /// typically have 1 parent. Computed nodes may have multiple parents.
        /// Pre-computed at compile time for O(1) parent lookup at runtime.
        /// </summary>
        public NativeArray<NativeArray<int>> ParentIndices;
    }

    /// <summary>
    /// Target info for DomTarget nodes.
    /// </summary>
    public class DomTargetInfo
    {
        public int ElemIdx;
        public Action<object, object> Setter;
    }

    /// <summary>
    /// Subscription entry: maps a property name to its Property node index.
    /// </summary>
    public class SubscriptionEntry
    {
        public string PropertyName;
        public int NodeIdx;
        public int SourceSlot;
    }

    /// <summary>
    /// Target info for Gate nodes (@if/@else).
    /// </summary>
    public class GateTargetInfo
    {
        public int MarkerIdx;
        public object TrueTemplate;
        public object FalseTemplate;
        public int TrueElemCount;
        public int FalseElemCount;
    }

    /// <summary>
    /// Target info for CollectionManager nodes (@foreach).
    /// </summary>
    public class CollectionTargetInfo
    {
        public int MarkerIdx;
        public GraphDescriptor ItemGraph;
        public object ItemTemplate;
    }

    /// <summary>
    /// Target info for EventBinding nodes.
    /// </summary>
    public class EventTargetInfo
    {
        public int ElemIdx;
        public string EventName;
    }
}
