using System.Collections.Generic;
using System.Linq;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;

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
        public Location Location { get; set; }
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

    /// <summary>
    /// LIMIT-006: Tracks a sub-control's property bindings in the graph topology.
    /// Each reactive property binding on a sub-control creates a graph node that,
    /// when evaluated, assigns the new value to the sub-control's property.
    /// </summary>
    public class SubControlTopology
    {
        public int ElemIdx { get; set; }
        public string ElementId { get; set; }
        public Location Location { get; set; }
        public string ControlTypeName { get; set; }
        public string ResolvedTypeName { get; set; }
        public List<SubControlPropertyTopology> PropertyBindings { get; set; } = new List<SubControlPropertyTopology>();
    }

    public class SubControlPropertyTopology
    {
        public int NodeIdx { get; set; }
        public string TargetPropertyName { get; set; }
        public string GetterExpression { get; set; }
        public string TwoWaySourceProperty { get; set; }
        public int TwoWaySourceSlot { get; set; }
    }

    public class GraphTopology
    {
        public int NodeCount { get; set; }
        public int[] NodeTypes { get; set; }
        public string[] GetterExpressions { get; set; }
        public int[] GetterSourceSlots { get; set; }
        public List<int>[] Consumers { get; set; }
        public int[] GateIndices { get; set; }
        public object[] DefaultValues { get; set; }
        public List<SubscriptionInfo> Subscriptions { get; set; } = new List<SubscriptionInfo>();
        public List<DomTargetTopology> DomTargets { get; set; } = new List<DomTargetTopology>();
        public List<EventTopology> Events { get; set; } = new List<EventTopology>();
        public List<GateTopology> Gates { get; set; } = new List<GateTopology>();
        public List<CollectionTopology> Collections { get; set; } = new List<CollectionTopology>();
        public List<SubControlTopology> SubControls { get; set; } = new List<SubControlTopology>();
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
            var ctx = new BuildContext(template.ItemVariablePrefix);

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
                        ProcessSubControl(sub, ctx, gateIndex);
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
                var expression = binding.Classification.CSharpExpression;
                int propIdx;
                if (IsComplexExpression(expression))
                {
                    propIdx = ctx.AddNode(GraphNodeTypeConstants.Computed, expression, null);
                    ctx.AddEdge(0, propIdx);
                }
                else
                {
                    propIdx = ctx.GetOrCreatePropertyNode(expression, 0);
                }
                if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);
                SubscribeUndetectedReads(ctx, expression, deps, propIdx);

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
                    int propIdx = ctx.GetOrCreatePropertyNode(
                        GetGetterPropertyName(dep, binding.Classification.CSharpExpression,
                            ctx.ItemVariablePrefix), 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);

                    if (isOneWay)
                    {
                        var segments = dep.PropertyChain.Split('.');
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            ctx.GetDependencySourceSlot(dep, binding.Classification.CSharpExpression),
                            segments);
                    }

                    // Computed node evaluates the full chain expression
                    int computedIdx = ctx.AddNode(GraphNodeTypeConstants.Computed,
                        binding.Classification.CSharpExpression, null);
                    if (gateIndex != -1) ctx.SetGateIndex(computedIdx, gateIndex);
                    ctx.AddEdge(0, computedIdx);
                    ctx.AddEdge(propIdx, computedIdx);

                    SubscribeUndetectedReads(ctx, binding.Classification.CSharpExpression, deps, computedIdx);
                    int domIdx = ctx.AddDomTarget(binding, computedIdx, gateIndex);
                }
                else
                {
                    // Check if the expression is more complex than a simple property access.
                    // Ternary expressions, comparisons, etc. need a Computed node to preserve
                    // the full expression logic. A Property node only returns the property value.
                    bool isComplexExpression = IsComplexExpression(
                        binding.Classification.CSharpExpression);

                    int propIdx = ctx.GetOrCreatePropertyNode(
                        GetGetterPropertyName(dep, binding.Classification.CSharpExpression,
                            ctx.ItemVariablePrefix), 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);

                    if (isOneWay)
                    {
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            ctx.GetDependencySourceSlot(dep, binding.Classification.CSharpExpression));
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
                        SubscribeUndetectedReads(ctx, binding.Classification.CSharpExpression, deps, computedIdx);
                        ctx.AddDomTarget(binding, computedIdx, gateIndex);
                    }
                    else
                    {
                        // Simple single property — existing behavior
                        SubscribeUndetectedReads(ctx, binding.Classification.CSharpExpression, deps, propIdx);
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
                    int propIdx = ctx.GetOrCreatePropertyNode(
                        GetGetterPropertyName(dep, binding.Classification.CSharpExpression,
                            ctx.ItemVariablePrefix), 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);
                    propIndices.Add(propIdx);

                    if (isOneWay)
                    {
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            ctx.GetDependencySourceSlot(dep, binding.Classification.CSharpExpression));
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
                SubscribeUndetectedReads(ctx, binding.Classification.CSharpExpression, deps, computedIdx);
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
                Location = evt.Location,
                EventName = evt.DomEventName,
                HandlerExpression = evt.HandlerExpression
            });
        }

        private static string GetGetterPropertyName(ObservableDependency dependency,
            string expression = null, string itemVariablePrefix = null)
        {
            if (!string.IsNullOrEmpty(dependency.SourceExpressionPrefix))
                return dependency.SourceExpressionPrefix + dependency.PropertyName;
            if (dependency.SourceKind == BindingSourceKind.TemplateParent)
                return "Control." + dependency.PropertyName;
            if (!string.IsNullOrEmpty(itemVariablePrefix))
            {
                if ((expression ?? string.Empty).Contains("Model." + dependency.PropertyName))
                    return "Model." + dependency.PropertyName;
                if ((expression ?? string.Empty).Contains(itemVariablePrefix + dependency.PropertyName))
                    return itemVariablePrefix + dependency.PropertyName;
            }
            return dependency.PropertyName;
        }

        /// <summary>
        /// Subscribes <paramref name="valueIdx"/> to reads the Roslyn analysis does not report as
        /// dependencies: every <c>Control.P</c> (slot 1), and inside an item graph every parent
        /// <c>Model.P</c> (slot 0). Reads already covered by <paramref name="deps"/> are skipped.
        /// A source that does not raise PropertyChanged is ignored at runtime, so an extra
        /// subscription is harmless while a missing one leaves the binding stale.
        /// </summary>
        private static void SubscribeUndetectedReads(BuildContext ctx, string expression,
            List<ObservableDependency> deps, int valueIdx)
        {
            foreach (var path in BindingExpressionConverter.CollectMemberPaths(expression))
            {
                if (path.Count < 2) continue;
                int slot;
                if (path[0] == "Control") slot = 1;
                else if (path[0] == "Model" && !string.IsNullOrEmpty(ctx.ItemVariablePrefix)) slot = 0;
                else continue;

                var propertyName = path[1];
                if (deps.Any(dep => dep.PropertyName == propertyName
                        && ctx.GetDependencySourceSlot(dep, expression) == slot))
                    continue;
                ctx.AddSubscription(propertyName, valueIdx, slot);
            }
        }

        private static bool TryGetControlProperty(string expression, out string propertyName)
        {
            propertyName = null;
            if (string.IsNullOrEmpty(expression))
                return false;
            var candidate = expression.TrimStart();
            if (candidate.StartsWith("!")) candidate = candidate.Substring(1).TrimStart();
            if (!candidate.StartsWith("Control."))
                return false;
            candidate = candidate.Substring("Control.".Length);
            if (candidate.Length == 0 || !char.IsLetter(candidate[0]))
                return false;
            int length = 1;
            while (length < candidate.Length
                && (char.IsLetterOrDigit(candidate[length]) || candidate[length] == '_'))
                length++;
            propertyName = candidate.Substring(0, length);
            return true;
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
                var getterPropertyName = GetGetterPropertyName(deps[0], condExpr,
                    ctx.ItemVariablePrefix);

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
                    int propIdx = ctx.GetOrCreatePropertyNode(getterPropertyName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);
                    if (cond.Condition.Mode == BindingMode.OneWay)
                    {
                        ctx.AddSubscription(propName, propIdx,
                            ctx.GetDependencySourceSlot(deps[0], condExpr));
                    }

                    // Create a new non-shared Property node with "!" + propName as getter
                    conditionSourceIdx = ctx.AddNode(GraphNodeTypeConstants.Property,
                        "!" + getterPropertyName, null);
                    if (gateIndex != -1) ctx.SetGateIndex(conditionSourceIdx, gateIndex);
                    ctx.AddEdge(propIdx, conditionSourceIdx);
                }
                else
                {
                    conditionSourceIdx = ctx.GetOrCreatePropertyNode(getterPropertyName, 0);
                    if (gateIndex != -1) ctx.SetGateIndex(conditionSourceIdx, gateIndex);
                    if (cond.Condition.Mode == BindingMode.OneWay)
                    {
                        ctx.AddSubscription(propName, conditionSourceIdx,
                            ctx.GetDependencySourceSlot(deps[0], condExpr));
                    }
                }
            }
            else
            {
                var conditionExpression = cond.Condition.CSharpExpression?.Trim();
                if (TryGetControlProperty(conditionExpression, out var controlProperty)
                    && (conditionExpression == "Control." + controlProperty
                        || conditionExpression == "!Control." + controlProperty))
                {
                    conditionSourceIdx = ctx.GetOrCreatePropertyNode(
                        "Control." + controlProperty, 0);
                    ctx.AddSubscription(controlProperty, conditionSourceIdx, 1);
                }
                else
                {
                    // Use source directly for 0-dep conditions
                    conditionSourceIdx = 0;
                }
            }

            // Create Gate node.
            // A gate's gateIndex is always itself — the runtime uses this to identify gate nodes.
            // For nested gates, child nodes reference the gate node's index via gateIndex parameter
            // passed to WalkChildren, not the gate node itself.
            int gateIdx = ctx.AddNode(GraphNodeTypeConstants.Gate,
                cond.Condition.CSharpExpression, false);
            // Gate node's gateIndex: -1 if top-level (always evaluates),
            // or the parent gate's index if nested (parent controls its visibility).
            if (gateIndex != -1) ctx.SetGateIndex(gateIdx, gateIndex);

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
                ctx.AddSubscription(propName, collIdx, ctx.GetSourceSlot(collExpr));
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
        /// LIMIT-006: Process sub-control property bindings.
        /// Each reactive property binding gets a Property node in the graph and a subscription.
        /// OneTime bindings are tracked but don't create subscriptions.
        /// </summary>
        private static void ProcessSubControl(SubControlNode sub, BuildContext ctx, int gateIndex)
        {
            int elemIdx = ctx.NextElemIdx();
            sub.RuntimeMarkerIdx = elemIdx;

            var subTopo = new SubControlTopology
            {
                ElemIdx = elemIdx,
                ElementId = sub.ElementId,
                Location = sub.Location,
                ControlTypeName = sub.TypeName,
                ResolvedTypeName = sub.ResolvedTypeName
            };

            foreach (var propBinding in sub.PropertyBindings)
            {
                var deps = propBinding.Classification.Dependencies;
                var isOneWay = propBinding.Classification.Mode == BindingMode.OneWay
                    || propBinding.Classification.Mode == BindingMode.TwoWay;
                var expression = propBinding.IsLiteral
                    ? "\"" + propBinding.Classification.CSharpExpression.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""
                    : propBinding.Classification.CSharpExpression;
                var twoWaySourceProperty = propBinding.Classification.Mode == BindingMode.TwoWay
                    && expression.StartsWith("Model.") ? expression.Substring(6)
                    : propBinding.Classification.Mode == BindingMode.TwoWay
                        && !string.IsNullOrEmpty(ctx.ItemVariablePrefix)
                        && expression.StartsWith(ctx.ItemVariablePrefix)
                        ? expression.Substring(ctx.ItemVariablePrefix.Length) : null;
                var twoWaySourceSlot = twoWaySourceProperty == null ? 0
                    : ctx.GetSourceSlot(expression);

                if (propBinding.IsDelegate && !propBinding.IsLiteral)
                {
                    int delegateIdx = ctx.AddNode(GraphNodeTypeConstants.EventBinding, expression, null);
                    if (gateIndex != -1) ctx.SetGateIndex(delegateIdx, gateIndex);
                    ctx.AddEdge(0, delegateIdx);
                    subTopo.PropertyBindings.Add(new SubControlPropertyTopology
                    {
                        NodeIdx = delegateIdx,
                        TargetPropertyName = propBinding.PropertyName,
                        GetterExpression = expression,
                        TwoWaySourceProperty = twoWaySourceProperty,
                        TwoWaySourceSlot = twoWaySourceSlot
                    });
                    continue;
                }

                if (deps.Count == 0)
                {
                    int valueIdx;
                    if (!propBinding.IsLiteral && expression.StartsWith("Model.")
                        && !IsComplexExpression(expression))
                    {
                        valueIdx = ctx.GetOrCreatePropertyNode(expression, 0);
                    }
                    else
                    {
                        valueIdx = ctx.AddNode(GraphNodeTypeConstants.Computed, expression, null);
                        ctx.AddEdge(0, valueIdx);
                    }
                    if (gateIndex != -1) ctx.SetGateIndex(valueIdx, gateIndex);
                    if (!propBinding.IsLiteral)
                        SubscribeUndetectedReads(ctx, expression, deps, valueIdx);

                    subTopo.PropertyBindings.Add(new SubControlPropertyTopology
                    {
                        NodeIdx = valueIdx,
                        TargetPropertyName = propBinding.PropertyName,
                        GetterExpression = expression,
                        TwoWaySourceProperty = twoWaySourceProperty,
                        TwoWaySourceSlot = twoWaySourceSlot
                    });
                }
                else if (deps.Count == 1)
                {
                    var dep = deps[0];
                    int propIdx = ctx.GetOrCreatePropertyNode(
                        GetGetterPropertyName(dep, expression, ctx.ItemVariablePrefix), 0);
                    if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);

                    if (isOneWay)
                    {
                        ctx.AddSubscription(dep.PropertyName, propIdx,
                            ctx.GetDependencySourceSlot(dep, expression));
                    }

                    int valueIdx = propIdx;
                    if (IsComplexExpression(expression))
                    {
                        valueIdx = ctx.AddNode(GraphNodeTypeConstants.Computed, expression, null);
                        if (gateIndex != -1) ctx.SetGateIndex(valueIdx, gateIndex);
                        ctx.AddEdge(0, valueIdx);
                        ctx.AddEdge(propIdx, valueIdx);
                    }
                    SubscribeUndetectedReads(ctx, expression, deps, valueIdx);

                    subTopo.PropertyBindings.Add(new SubControlPropertyTopology
                    {
                        NodeIdx = valueIdx,
                        TargetPropertyName = propBinding.PropertyName,
                        GetterExpression = expression,
                        TwoWaySourceProperty = twoWaySourceProperty,
                        TwoWaySourceSlot = twoWaySourceSlot
                    });
                }
                else
                {
                    // Multiple dependencies — create Computed node
                    var propIndices = new List<int>();
                    foreach (var dep in deps)
                    {
                        int propIdx = ctx.GetOrCreatePropertyNode(
                            GetGetterPropertyName(dep, expression, ctx.ItemVariablePrefix), 0);
                        if (gateIndex != -1) ctx.SetGateIndex(propIdx, gateIndex);
                        propIndices.Add(propIdx);

                        if (isOneWay)
                        {
                            ctx.AddSubscription(dep.PropertyName, propIdx,
                                ctx.GetDependencySourceSlot(dep, expression));
                        }
                    }

                    int computedIdx = ctx.AddNode(GraphNodeTypeConstants.Computed,
                        expression, null);
                    if (gateIndex != -1) ctx.SetGateIndex(computedIdx, gateIndex);

                    ctx.AddEdge(0, computedIdx);
                    foreach (int propIdx in propIndices)
                        ctx.AddEdge(propIdx, computedIdx);
                    SubscribeUndetectedReads(ctx, expression, deps, computedIdx);

                    subTopo.PropertyBindings.Add(new SubControlPropertyTopology
                    {
                        NodeIdx = computedIdx,
                        TargetPropertyName = propBinding.PropertyName,
                        GetterExpression = expression,
                        TwoWaySourceProperty = twoWaySourceProperty,
                        TwoWaySourceSlot = twoWaySourceSlot
                    });
                }
            }

            foreach (var evt in sub.EventBindings)
            {
                int evtIdx = ctx.AddNode(GraphNodeTypeConstants.EventBinding,
                    evt.HandlerExpression, null);
                if (gateIndex != -1) ctx.SetGateIndex(evtIdx, gateIndex);
                ctx.AddEdge(0, evtIdx);
                ctx.Topology.Events.Add(new EventTopology
                {
                    NodeIdx = evtIdx,
                    ElemIdx = elemIdx,
                    EventName = evt.DomEventName,
                    HandlerExpression = evt.HandlerExpression
                });
            }

            ctx.Topology.SubControls.Add(subTopo);
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
            private readonly string _itemVariablePrefix;
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

            public BuildContext(string itemVariablePrefix)
            {
                _itemVariablePrefix = itemVariablePrefix;
            }

            public string ItemVariablePrefix => _itemVariablePrefix;

            public int GetSourceSlot(string expression)
            {
                if ((expression ?? string.Empty).StartsWith("Control.")) return 1;
                if (!string.IsNullOrEmpty(_itemVariablePrefix)
                    && (expression ?? string.Empty).StartsWith(_itemVariablePrefix)) return 2;
                return 0;
            }

            public int GetDependencySourceSlot(ObservableDependency dependency,
                string expression)
            {
                if (dependency.SourceExpressionPrefix == "Model.") return 0;
                if (dependency.SourceExpressionPrefix == "Control.") return 1;
                if (!string.IsNullOrEmpty(_itemVariablePrefix)
                    && dependency.SourceExpressionPrefix == _itemVariablePrefix) return 2;
                if (dependency.SourceKind == BindingSourceKind.TemplateParent) return 1;
                if (!string.IsNullOrEmpty(_itemVariablePrefix)
                    && (expression ?? string.Empty).Contains(_itemVariablePrefix
                        + dependency.PropertyName)) return 2;
                return 0;
            }

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
                // For chains, deduplicate by full chain key; for simple, by property name.
                // The source slot is part of the identity: the same property on the same
                // node read from different slots (e.g. Control.Count and parent Model.Count
                // in a foreach) are distinct subscriptions; collapsing them drops one and
                // leaves that binding stale when the dropped source changes.
                var dedupeKey = nodeIdx + ":" + sourceSlot + ":" + (pathSegments != null
                    ? string.Join(".", pathSegments) : propertyName);
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
                Topology.GetterSourceSlots = _getterExpressions.Select(expression =>
                    expression != null && expression.StartsWith("!Control.")
                        ? 1 : GetSourceSlot(expression)).ToArray();
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
