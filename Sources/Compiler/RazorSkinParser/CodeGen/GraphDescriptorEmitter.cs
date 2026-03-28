using System.Collections.Generic;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Emits a JavaScript object literal representing the static graph descriptor
    /// for a compiled Razor skin template. Used for snapshot tests and debugging.
    /// </summary>
    public static class GraphDescriptorEmitter
    {
        /// <summary>
        /// Emit a JavaScript variable declaration for the graph descriptor.
        /// </summary>
        public static string EmitDescriptor(
            string templateName,
            GraphTopology topology,
            ISet<string> knownFunctionNames)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"var {templateName}_graph = {{");

            // nodeTypes: [0, 1, 3, ...]
            sb.Append("  nodeTypes: [");
            for (int i = 0; i < topology.NodeCount; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(topology.NodeTypes[i]);
            }
            sb.AppendLine("],");

            // getters: [null, function(dc) { return ...; }, null, ...]
            sb.Append("  getters: [");
            for (int i = 0; i < topology.NodeCount; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(EmitGetter(topology.NodeTypes[i], topology.GetterExpressions[i], knownFunctionNames));
            }
            sb.AppendLine("],");

            // consumers: [[1], [2], [], ...]
            sb.Append("  consumers: [");
            for (int i = 0; i < topology.NodeCount; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append("[");
                var list = topology.Consumers[i];
                for (int j = 0; j < list.Count; j++)
                {
                    if (j > 0) sb.Append(", ");
                    sb.Append(list[j]);
                }
                sb.Append("]");
            }
            sb.AppendLine("],");

            // gateIndices: [-1, -1, 2, ...]
            sb.Append("  gateIndices: [");
            for (int i = 0; i < topology.NodeCount; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(topology.GateIndices[i]);
            }
            sb.AppendLine("],");

            // defaultValues: [null, null, "", false, ...]
            sb.Append("  defaultValues: [");
            for (int i = 0; i < topology.NodeCount; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(EmitDefaultValue(topology.DefaultValues[i]));
            }
            sb.AppendLine("],");

            // targetInfos: [null, null, {elem: 0, set: SetTextContent}, ...]
            sb.Append("  targetInfos: [");
            // Build a lookup from NodeIdx to DomTargetTopology
            var domTargetMap = new Dictionary<int, DomTargetTopology>();
            foreach (var dt in topology.DomTargets)
                domTargetMap[dt.NodeIdx] = dt;

            for (int i = 0; i < topology.NodeCount; i++)
            {
                if (i > 0) sb.Append(", ");
                if (domTargetMap.TryGetValue(i, out var dt))
                    sb.Append($"{{elem: {dt.ElemIdx}, set: {GetSetterName(dt.Target)}}}");
                else
                    sb.Append("null");
            }
            sb.AppendLine("],");

            // subscriptions: [["Name", 1], ...]
            sb.Append("  subscriptions: [");
            for (int i = 0; i < topology.Subscriptions.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                var sub = topology.Subscriptions[i];
                sb.Append($"[\"{sub.PropertyName}\", {sub.NodeIdx}]");
            }
            sb.AppendLine("],");

            // sourceType
            sb.AppendLine($"  sourceType: \"{topology.ModelTypeName}\",");

            // subscribeMode (0 = PerProperty)
            sb.AppendLine("  subscribeMode: 0,");

            // nodeCount
            sb.AppendLine($"  nodeCount: {topology.NodeCount}");

            sb.Append("};");
            return sb.ToString();
        }

        private static string EmitGetter(int nodeType, string getterExpression, ISet<string> knownFunctionNames)
        {
            switch (nodeType)
            {
                case GraphNodeTypeConstants.Source:
                case GraphNodeTypeConstants.DomTarget:
                case GraphNodeTypeConstants.EventBinding:
                    return "null";

                case GraphNodeTypeConstants.Property:
                {
                    // Property nodes store the raw property name (e.g. "Name"), not a full expression.
                    // Emit as: function(dc) { return dc.get_name(); }
                    if (string.IsNullOrEmpty(getterExpression))
                        return "null";

                    // Check if this is a known function name — preserve it as-is
                    if (knownFunctionNames != null && knownFunctionNames.Contains(getterExpression))
                        return $"function(dc) {{ return dc.{getterExpression}; }}";

                    var getterName = ExpressionJsEmitter.PropertyToGetterName(getterExpression);
                    return $"function(dc) {{ return dc.{getterName}(); }}";
                }

                case GraphNodeTypeConstants.Computed:
                case GraphNodeTypeConstants.Gate:
                case GraphNodeTypeConstants.CollectionManager:
                {
                    // Computed/Gate/Collection store the full C# expression (e.g. "Model.Price * Model.Quantity")
                    if (string.IsNullOrEmpty(getterExpression))
                        return "null";

                    var jsExpr = ExpressionJsEmitter.ToJsGetter(
                        getterExpression,
                        "dc",
                        "tp",
                        knownFunctionNames);
                    return $"function(dc) {{ return {jsExpr}; }}";
                }

                default:
                    return "null";
            }
        }

        private static string EmitDefaultValue(object value)
        {
            if (value == null) return "null";
            if (value is bool b) return b ? "true" : "false";
            if (value is string s) return $"\"{s}\"";
            return value.ToString();
        }

        private static string GetSetterName(ExpressionTarget target)
        {
            switch (target)
            {
                case ExpressionTarget.TextContent: return "SetTextContent";
                case ExpressionTarget.Attribute:   return "SetAttribute";
                case ExpressionTarget.CssClass:    return "SetClassName";
                case ExpressionTarget.Style:       return "SetStyle";
                default:                           return "SetTextContent";
            }
        }
    }
}
