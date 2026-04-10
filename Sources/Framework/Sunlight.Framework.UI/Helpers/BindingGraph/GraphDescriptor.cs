namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;
    using System.Runtime.CompilerServices;

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

        /// <summary>
        /// LIMIT-006: Sub-control entries. Each entry describes a child control
        /// embedded in the template, with property bindings that wire graph nodes
        /// to the sub-control's properties.
        /// </summary>
        public NativeArray<SubControlInfo> SubControls;
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
        /// <summary>
        /// For chained property paths (e.g., ["Customer", "Address", "City"]).
        /// Null for simple single-property subscriptions.
        /// </summary>
        public NativeArray<string> PathSegments;
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
        /// <summary>
        /// Elem indices inside the true branch template. After the gate renders
        /// the true template, the engine resolves these from the rendered DOM
        /// and updates state.ElemRefs. Null if the true branch has no child elements.
        /// </summary>
        public NativeArray<int> TrueChildElemIndices;
        /// <summary>
        /// Elem indices inside the false branch template.
        /// </summary>
        public NativeArray<int> FalseChildElemIndices;
    }

    /// <summary>
    /// Target info for CollectionManager nodes (@foreach).
    /// </summary>
    public class CollectionTargetInfo
    {
        public int MarkerIdx;
        public GraphDescriptor ItemGraph;
        public object ItemTemplate;
        /// <summary>
        /// Sub-control descriptors for controls inside the foreach item template.
        /// Null when no sub-controls are used.
        /// </summary>
        public NativeArray<SubControlInfo> SubControlInfos;
    }

    /// <summary>
    /// Target info for EventBinding nodes.
    /// </summary>
    public class EventTargetInfo
    {
        public int ElemIdx;
        public string EventName;
    }

    /// <summary>
    /// Describes a sub-control in the template.
    /// Used for both collection item sub-controls (MarkerIdx/TypeFactory/SkinFactory)
    /// and LIMIT-006 top-level property bindings (ElemIdx/Bindings).
    /// </summary>
    public class SubControlInfo
    {
        // Collection item sub-control fields (existing)
        public int MarkerIdx;
        public Func<object, object> TypeFactory;
        public Func<object> SkinFactory;

        // LIMIT-006: Top-level sub-control property binding fields
        public int ElemIdx;
        public NativeArray<SubControlPropertyInfo> Bindings;
    }

    /// <summary>
    /// LIMIT-006: Describes a single property binding on a sub-control.
    /// NodeIdx references the graph node whose value should be assigned
    /// to the sub-control via the Setter function.
    /// </summary>
    public class SubControlPropertyInfo
    {
        public int NodeIdx;
        public Action<object, object> Setter;
    }
}
