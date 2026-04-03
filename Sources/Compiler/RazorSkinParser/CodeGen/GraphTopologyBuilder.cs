using System.Collections.Generic;
using System.Linq;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    // --- Output types ---

    public static class GraphNodeTypeConstants
    {
        public const int Source = 0;
        public const int Property = 1;
        public const int Computed = 2;
        public const int DomTarget = 3;
        public const int EventBinding = 4;
        public const int Gate = 5;
        public const int CollectionManager = 6;
        public const int TypeGuard = 7;
    }

    public class SubscriptionInfo
    {
        public string PropertyName { get; set; }
        public int NodeIdx { get; set; }
        public int SourceSlot { get; set; }
        /// <summary>
        /// For chained paths (e.g., "Customer.Address.City"), the individual path segments.
        /// Null for single-property subscriptions.
        /// </summary>
        public string[] PathSegments { get; set; }
    }

    public class DomTargetTopology
    {
        public int NodeIdx { get; set; }
        public int ElemIdx { get; set; }
        public ExpressionTarget Target { get; set; }
        public string AttributeName { get; set; }
        public string AttributePrefix { get; set; }
    }

    public class EventTopology
    {
        public int NodeIdx { get; set; }
        public int ElemIdx { get; set; }
        public string EventName { get; set; }
        public string HandlerExpression { get; set; }
    }

    public class GateTopology
    {
        public int NodeIdx { get; set; }
        public int MarkerIdx { get; set; }
        public ConditionalNode IrNode { get; set; }
        /// <summary>
        /// Elem indices allocated inside the true branch (for runtime ElemRef resolution).
        /// These elements exist only in the gate's trueTemplate DOM, not the static HTML.
        /// </summary>
        public int[] TrueChildElemIndices { get; set; }
        /// <summary>
        /// Elem indices allocated inside the false branch (for runtime ElemRef resolution).
        /// </summary>
        public int[] FalseChildElemIndices { get; set; }
    }

    public class CollectionTopology
    {
        public int NodeIdx { get; set; }
        public int MarkerIdx { get; set; }
        public LoopNode IrNode { get; set; }
        public GraphTopology ItemTopology { get; set; }
    }

    public class GraphTopology
    {
        public int NodeCount { get; set; }
        public int[] NodeTypes { get; set; }
        public string[] GetterExpressions { get; set; }
        public List<int>[] Consumers { get; set; }
        public int[] GateIndices { get; set; }
        public object[] DefaultValues { get; set; }
        public List<SubscriptionInfo> Subscriptions { get; set; } = new List<SubscriptionInfo>();
        public List<DomTargetTopology> DomTargets { get; set; } = new List<DomTargetTopology>();
        public List<EventTopology> Events { get; set; } = new List<EventTopology>();
        public List<GateTopology> Gates { get; set; } = new List<GateTopology>();
        public List<CollectionTopology> Collections { get; set; } = new List<CollectionTopology>();
        public string ModelTypeName { get; set; }
        public int RootSourceSlot { get; set; }
        public int TotalElemSlots { get; set; }
        /// <summary>
        /// For item graphs, the variable prefix to strip from expressions (e.g., "item.").
        /// </summary>
        public string ItemVariablePrefix { get; set; }

        /// <summary>
        /// Parent indices per node (inverse of Consumers). ParentIndices[j] lists
        /// nodes that feed into node j. Computed at build time for O(1) runtime lookup.
        /// </summary>
        public List<int>[] ParentIndices { get; set; }

        /// <summary>
        /// Returns the set of all elem indices that are inside gate branches
        /// (not present in static HTML, resolved at runtime when gates render).
        /// </summary>
        public HashSet<int> GetGatedElemIndices()
        {
            var result = new HashSet<int>();
            foreach (var gate in Gates)
            {
                if (gate.TrueChildElemIndices != null)
                    foreach (var idx in gate.TrueChildElemIndices)
                        result.Add(idx);
                if (gate.FalseChildElemIndices != null)
                    foreach (var idx in gate.FalseChildElemIndices)
                        result.Add(idx);
            }
            return result;
        }
    }

    // --- Builder ---

    public static class GraphTopologyBuilder
    {
        public static GraphTopology Build(SkinTemplateNode template)
        {
            var ctx = new BuildContext();

            // Node 0 is always the Source node (DataContext root)
            ctx.AddNode(GraphNodeTypeConstants.Source, null, null);

            // Walk all children
            WalkChildren(template.Children, ctx, gateIndex: -1);

            var topo = ctx.ToTopology(template.ModelTypeName);
            topo.ItemVariablePrefix = template.ItemVariablePrefix;
            return topo;
        }

        private static void WalkChildren(List<IRNode> children, BuildContext ctx, int gateIndex)
        {
            foreach (var child in children)
            {
                switch (child)
                {
                    case ExpressionBindingNode binding:
                        ProcessBinding(binding, ctx, gateIndex);
                        break;
                    case EventNode evt:
                        ProcessEvent(evt, ctx, gateIndex);
                        break;
                    case ConditionalNode cond:
                        ProcessConditional(cond, ctx, gateIndex);
                        break;
                    case LoopNode loop:
                        ProcessLoop(loop, ctx, gateIndex);
                        break;
                    case HtmlNode _:
                        // Static HTML — no graph nodes needed
                        break;
                    case SubControlNode sub:
                        // Sub-controls could be expanded later
                        break;
                    default:
                        // Walk generic children
                        if (child.Children.Count > 0)
                            WalkChildren(child.Children, ctx, gateIndex);
                        break;
                }
            }
        }

        private static void ProcessBinding(ExpressionBindingNode binding, BuildContext ctx, int gateIndex)
        {
            var deps = binding.Classification.Dependencies;
            var isOneWay = binding.Classification.Mode == BindingMode.OneWay;

            if (deps.Count == 0)
            {
                // No dependencies — create a property node from the expression directly
                int propIdx = ctx.GetOrCreatePropertyNode(
                    binding.Classification.CSharpExpression, 0);
                if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);

                int domIdx = ctx.AddDomTarget(binding, propIdx, gateIndex);
                return;
            }

            if (deps.Count == 1)
            {
                var dep = deps[0];
                bool isChained = dep.PropertyChain != null && dep.PropertyChain.Contains(".");

                if (isChained)
                {
                    // Chained path: Property node for root + Computed node for full expression
                    int propIdx = ctx.GetOrCreatePropertyNode(dep.PropertyName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);

                    if (isOneWay)
                    {
                        var segments = dep.PropertyChain.Split('.');
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            dep.SourceKind == BindingSourceKind.TemplateParent ? 1 : 0,
                            segments);
                    }

                    // Computed node evaluates the full chain expression
                    int computedIdx = ctx.AddNode(GraphNodeTypeConstants.Computed,
                        binding.Classification.CSharpExpression, null);
                    if (gateIndex != -1) ctx.SetGateIndex(computedIdx, gateIndex);
                    ctx.AddEdge(0, computedIdx);
                    ctx.AddEdge(propIdx, computedIdx);

                    int domIdx = ctx.AddDomTarget(binding, computedIdx, gateIndex);
                }
                else
                {
                    // Check if the expression is more complex than a simple property access.
                    // Ternary expressions, comparisons, etc. need a Computed node to preserve
                    // the full expression logic. A Property node only returns the property value.
                    bool isComplexExpression = IsComplexExpression(
                        binding.Classification.CSharpExpression);

                    int propIdx = ctx.GetOrCreatePropertyNode(dep.PropertyName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);

                    if (isOneWay)
                    {
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            dep.SourceKind == BindingSourceKind.TemplateParent ? 1 : 0);
                    }

                    if (isComplexExpression)
                    {
                        // Complex expression with single dep: Property node for subscription +
                        // Computed node for full expression evaluation (like multi-dep case)
                        int computedIdx = ctx.AddNode(GraphNodeTypeConstants.Computed,
                            binding.Classification.CSharpExpression, null);
                        if (gateIndex != -1) ctx.SetGateIndex(computedIdx, gateIndex);
                        ctx.AddEdge(0, computedIdx);
                        ctx.AddEdge(propIdx, computedIdx);
                        ctx.AddDomTarget(binding, computedIdx, gateIndex);
                    }
                    else
                    {
                        // Simple single property — existing behavior
                        ctx.AddDomTarget(binding, propIdx, gateIndex);
                    }
                }
            }
            else
            {
                // Multiple dependencies — Property nodes + Computed node
                var propIndices = new List<int>();
                foreach (var dep in deps)
                {
                    int propIdx = ctx.GetOrCreatePropertyNode(dep.PropertyName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);
                    propIndices.Add(propIdx);

                    if (isOneWay)
                    {
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            dep.SourceKind == BindingSourceKind.TemplateParent ? 1 : 0);
                    }
                }

                // Create Computed node
                int computedIdx = ctx.AddNode(GraphNodeTypeConstants.Computed,
                    binding.Classification.CSharpExpression, null);
                if (gateIndex != -1) ctx.SetGateIndex(computedIdx, gateIndex);

                // Wire Source(0) -> Computed FIRST so FindParentValue returns the source.
                // The Computed getter evaluates the full expression against the DataContext,
                // not against a single property value.
                ctx.AddEdge(0, computedIdx);

                // Also wire property nodes -> computed for dirty propagation.
                foreach (int propIdx in propIndices)
                {
                    ctx.AddEdge(propIdx, computedIdx);
                }

                // Create DomTarget consuming from computed
                int domIdx = ctx.AddDomTarget(binding, computedIdx, gateIndex);
            }
        }

        private static void ProcessEvent(EventNode evt, BuildContext ctx, int gateIndex)
        {
            // Store the handler expression as the getter expression so the emitter can
            // build a proper getter function to extract the method reference from the source.
            int evtIdx = ctx.AddNode(GraphNodeTypeConstants.EventBinding, evt.HandlerExpression, null);
            if (gateIndex != -1) ctx.SetGateIndex(evtIdx, gateIndex);

            ctx.AddEdge(0, evtIdx); // Source -> EventBinding

            ctx.Topology.Events.Add(new EventTopology
            {
                NodeIdx = evtIdx,
                ElemIdx = ctx.NextElemIdx(),
                EventName = evt.DomEventName,
                HandlerExpression = evt.HandlerExpression
            });
        }

        private static void ProcessConditional(ConditionalNode cond, BuildContext ctx, int gateIndex)
        {
            // Create property node for the condition's dependencies
            var deps = cond.Condition.Dependencies;
            int conditionSourceIdx;

            if (deps.Count == 1)
            {
                var condExpr = cond.Condition.CSharpExpression ?? "";
                var propName = deps[0].PropertyName;

                // Classify the condition expression to determine how to feed the gate:
                // - "!Model.X" → negated property (gate checks !field)
                // - "Model.X != null" → direct property (gate checks truthiness = non-null)
                // - "Model.X == null" → negated property (gate checks !truthiness = null)
                // - "!Model.X != null" → unsupported degenerate case; falls through to direct property (non-negated).
                bool isNegated = condExpr.TrimStart().StartsWith("!");
                bool isNotNull = condExpr.Contains("!= null") || condExpr.Contains("!=null");
                bool isNull = !isNotNull && (condExpr.Contains("== null") || condExpr.Contains("==null"));

                // "X != null" is equivalent to truthiness check on X — no special handling needed
                // "X == null" is equivalent to !X (negated truthiness)
                if (isNull)
                    isNegated = true;

                if (isNegated && !isNotNull)
                {
                    // Simple negation — create a dedicated Property node with negated getter.
                    // Use "!" prefix convention: the emitter will build "return !dc.field;"
                    int propIdx = ctx.GetOrCreatePropertyNode(propName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);
                    if (cond.Condition.Mode == BindingMode.OneWay)
                    {
                        ctx.AddSubscription(propName, propIdx,
                            deps[0].SourceKind == BindingSourceKind.TemplateParent ? 1 : 0);
                    }

                    // Create a new non-shared Property node with "!" + propName as getter
                    conditionSourceIdx = ctx.AddNode(GraphNodeTypeConstants.Property,
                        "!" + propName, null);
                    if (gateIndex != -1) ctx.SetGateIndex(conditionSourceIdx, gateIndex);
                    ctx.AddEdge(propIdx, conditionSourceIdx);
                }
                else
                {
                    conditionSourceIdx = ctx.GetOrCreatePropertyNode(propName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(conditionSourceIdx, gateIndex);
                    if (cond.Condition.Mode == BindingMode.OneWay)
                    {
                        ctx.AddSubscription(propName, conditionSourceIdx,
                            deps[0].SourceKind == BindingSourceKind.TemplateParent ? 1 : 0);
                    }
                }
            }
            else
            {
                // Use source directly for 0-dep conditions
                conditionSourceIdx = 0;
            }

            // Create Gate node.
            // A gate's gateIndex is always itself — the runtime uses this to identify gate nodes.
            // For nested gates, child nodes reference the gate node's index via gateIndex parameter
            // passed to WalkChildren, not the gate node itself.
            int gateIdx = ctx.AddNode(GraphNodeTypeConstants.Gate,
                cond.Condition.CSharpExpression, false);
            ctx.SetGateIndex(gateIdx, gateIdx); // Gate's gateIndex is always itself

            ctx.AddEdge(conditionSourceIdx, gateIdx);

            int markerIdx = ctx.NextElemIdx();

            // Track elem indices allocated inside each branch.
            // These elements exist only in the gate's template DOM, not the static HTML.
            // The runtime uses these indices to resolve ElemRefs after rendering.
            int trueElemStart = ctx.ElemCounter;
            WalkChildren(cond.TrueBranch, ctx, gateIdx);
            int trueElemEnd = ctx.ElemCounter;

            int falseElemStart = ctx.ElemCounter;
            if (cond.FalseBranch != null && cond.FalseBranch.Count > 0)
            {
                // Convention: -(gateIdx + 2) encodes "inverted gate at gateIdx".
                WalkChildren(cond.FalseBranch, ctx, -(gateIdx + 2));
            }
            int falseElemEnd = ctx.ElemCounter;

            // Build child elem index arrays
            int[] trueChildElems = trueElemEnd > trueElemStart
                ? Enumerable.Range(trueElemStart, trueElemEnd - trueElemStart).ToArray()
                : null;
            int[] falseChildElems = falseElemEnd > falseElemStart
                ? Enumerable.Range(falseElemStart, falseElemEnd - falseElemStart).ToArray()
                : null;

            ctx.Topology.Gates.Add(new GateTopology
            {
                NodeIdx = gateIdx,
                MarkerIdx = markerIdx,
                IrNode = cond,
                TrueChildElemIndices = trueChildElems,
                FalseChildElemIndices = falseChildElems
            });
        }

        private static void ProcessLoop(LoopNode loop, BuildContext ctx, int gateIndex)
        {
            int collIdx = ctx.AddNode(GraphNodeTypeConstants.CollectionManager,
                loop.CollectionExpression, null);
            if (gateIndex != -1) ctx.SetGateIndex(collIdx, gateIndex);

            ctx.AddEdge(0, collIdx);

            // Subscribe to PropertyChanged for the collection property so that
            // collection reference changes (e.g., setting DetailSubTasks to a new
            // ObservableCollection) trigger a Flush that detaches the old listener
            // and re-renders with the new collection.
            string collExpr = loop.CollectionExpression ?? "";
            string propName = collExpr;
            if (propName.StartsWith("Model."))
                propName = propName.Substring("Model.".Length);
            if (!string.IsNullOrEmpty(propName))
            {
                ctx.AddSubscription(propName, collIdx, 0);
            }

            // Build item topology recursively if there's an item template.
            // Note: ModelTypeName is set to null — the item type isn't known at compile
            // time from the loop variable name alone. The GraphEngine skips the type check
            // when sourceType is null, which is correct since the collection getter already
            // returns properly-typed items.
            GraphTopology itemTopology = null;
            if (loop.ItemTemplate != null && loop.ItemTemplate.Count > 0)
            {
                var itemTemplate = new SkinTemplateNode
                {
                    TemplateName = "ItemTemplate",
                    ModelTypeName = null,
                    ItemVariablePrefix = loop.ItemVariableName + ".",
                    Children = loop.ItemTemplate
                };
                itemTopology = Build(itemTemplate);
            }

            ctx.Topology.Collections.Add(new CollectionTopology
            {
                NodeIdx = collIdx,
                MarkerIdx = ctx.NextElemIdx(),
                IrNode = loop,
                ItemTopology = itemTopology
            });
        }

        /// <summary>
        /// Detects whether a C# expression is more complex than a simple property access.
        /// Ternary operators, comparisons, logical operators, arithmetic, and string
        /// concatenation all indicate that a Computed node is needed to preserve the logic.
        /// </summary>
        private static bool IsComplexExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return false;

            // Check for common operators that indicate complex expressions.
            // We check for operators that wouldn't appear in a simple "Prefix.PropertyName" path.
            return expression.Contains("?")    // ternary
                || expression.Contains("+")    // concatenation/arithmetic
                || expression.Contains("-")    // subtraction
                || expression.Contains("*")    // multiplication
                || expression.Contains("/")    // division
                || expression.Contains("==")   // equality
                || expression.Contains("!=")   // inequality
                || expression.Contains("&&")   // logical AND
                || expression.Contains("||")   // logical OR
                || expression.Contains(">")    // comparison
                || expression.Contains("<")    // comparison
                || expression.Contains("!");   // negation (standalone, not part of !=)
        }

        // --- Internal build context ---

        private class BuildContext
        {
            private readonly List<int> _nodeTypes = new List<int>();
            private readonly List<string> _getterExpressions = new List<string>();
            private readonly List<List<int>> _consumers = new List<List<int>>();
            private readonly List<int> _gateIndices = new List<int>();
            private readonly List<object> _defaultValues = new List<object>();

            // Property deduplication: key = propertyName, value = node index
            private readonly Dictionary<string, int> _propertyNodeMap = new Dictionary<string, int>();

            // Subscription deduplication: key = propertyName
            private readonly HashSet<string> _subscribedProperties = new HashSet<string>();

            private int _elemCounter;

            public int ElemCounter => _elemCounter;

            public GraphTopology Topology { get; } = new GraphTopology();

            public int AddNode(int nodeType, string getterExpression, object defaultValue)
            {
                int idx = _nodeTypes.Count;
                _nodeTypes.Add(nodeType);
                _getterExpressions.Add(getterExpression);
                _consumers.Add(new List<int>());
                _gateIndices.Add(-1);
                _defaultValues.Add(defaultValue);
                return idx;
            }

            public int GetOrCreatePropertyNode(string propertyName, int sourceNodeIdx)
            {
                if (_propertyNodeMap.TryGetValue(propertyName, out int existing))
                    return existing;

                int idx = AddNode(GraphNodeTypeConstants.Property, propertyName, null);
                _propertyNodeMap[propertyName] = idx;

                // Wire Source -> Property
                AddEdge(sourceNodeIdx, idx);

                return idx;
            }

            public void AddEdge(int from, int to)
            {
                if (!_consumers[from].Contains(to))
                    _consumers[from].Add(to);
            }

            public void SetGateIndex(int nodeIdx, int gateIdx)
            {
                int existing = _gateIndices[nodeIdx];
                if (existing != -1 && existing != gateIdx)
                {
                    // Node is shared across multiple gate branches (e.g., a property used
                    // in both true and false branches). Make it ungated so it always evaluates.
                    _gateIndices[nodeIdx] = -1;
                    return;
                }
                _gateIndices[nodeIdx] = gateIdx;
            }

            public void AddSubscription(string propertyName, int nodeIdx, int sourceSlot, string[] pathSegments = null)
            {
                // For chains, deduplicate by full chain key; for simple, by property name
                var dedupeKey = pathSegments != null ? string.Join(".", pathSegments) : propertyName;
                if (_subscribedProperties.Contains(dedupeKey))
                    return;

                _subscribedProperties.Add(dedupeKey);
                Topology.Subscriptions.Add(new SubscriptionInfo
                {
                    PropertyName = propertyName,
                    NodeIdx = nodeIdx,
                    SourceSlot = sourceSlot,
                    PathSegments = pathSegments
                });
            }

            public int AddDomTarget(ExpressionBindingNode binding, int producerIdx, int gateIndex)
            {
                string defaultVal = GetDefaultForTarget(binding.Target);
                int domIdx = AddNode(GraphNodeTypeConstants.DomTarget,
                    binding.Classification.CSharpExpression, defaultVal);

                if (gateIndex != -1) SetGateIndex(domIdx, gateIndex);

                AddEdge(producerIdx, domIdx);

                Topology.DomTargets.Add(new DomTargetTopology
                {
                    NodeIdx = domIdx,
                    ElemIdx = NextElemIdx(),
                    Target = binding.Target,
                    AttributeName = binding.AttributeName,
                    AttributePrefix = binding.AttributePrefix
                });

                return domIdx;
            }

            public int NextElemIdx()
            {
                return _elemCounter++;
            }

            public GraphTopology ToTopology(string modelTypeName)
            {
                int n = _nodeTypes.Count;
                Topology.NodeCount = n;
                Topology.NodeTypes = _nodeTypes.ToArray();
                Topology.GetterExpressions = _getterExpressions.ToArray();
                Topology.Consumers = _consumers.ToArray();
                Topology.GateIndices = _gateIndices.ToArray();
                Topology.DefaultValues = _defaultValues.ToArray();
                Topology.ModelTypeName = modelTypeName;
                Topology.RootSourceSlot = 0;

                // Build ParentIndices by inverting the Consumers adjacency list.
                var parentIndices = new List<int>[n];
                for (int i = 0; i < n; i++)
                    parentIndices[i] = new List<int>();

                for (int from = 0; from < n; from++)
                {
                    foreach (int to in _consumers[from])
                    {
                        parentIndices[to].Add(from);
                    }
                }

                Topology.ParentIndices = parentIndices;
                Topology.TotalElemSlots = _elemCounter;
                return Topology;
            }

            private static string GetDefaultForTarget(ExpressionTarget target)
            {
                switch (target)
                {
                    case ExpressionTarget.TextContent:
                    case ExpressionTarget.Attribute:
                    case ExpressionTarget.CssClass:
                    case ExpressionTarget.Style:
                        return "";
                    default:
                        return "";
                }
            }
        }
    }
}
