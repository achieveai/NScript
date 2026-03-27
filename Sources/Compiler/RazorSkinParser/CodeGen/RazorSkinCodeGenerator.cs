using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class RazorSkinCodeGenerator
    {
        public static string Generate(SkinTemplateNode ir)
        {
            var sb = new StringBuilder();

            // Collect all bindings
            var bindings = CollectBindings(ir.Children);
            var htmlContent = CollectHtml(ir.Children);
            int liveBinderCount = bindings.Count(b => b.Classification.Mode == BindingMode.OneWay);

            // Template store variable
            sb.AppendLine($"var {ir.TemplateName}_var = null;");
            sb.AppendLine();

            // Factory method
            sb.AppendLine($"function {ir.TemplateName}_factory(skinFactory, doc) {{");
            sb.AppendLine("  var domStore, htmlRoot, objStorage;");
            sb.AppendLine($"  if (!(domStore = DocStorageGetter(doc))[0]) {{");
            sb.AppendLine($"    domStore[0] = doc.createElement(\"div\");");
            sb.AppendLine($"    domStore[0].innerHTML = \"{EscapeJs(htmlContent)}\";");

            // Binders array
            if (bindings.Count > 0)
            {
                sb.AppendLine("    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [");
                for (int i = 0; i < bindings.Count; i++)
                {
                    var comma = i < bindings.Count - 1 ? "," : "";
                    sb.AppendLine($"      {BinderEmitter.EmitSkinBinderInfo(bindings[i], i, i)}{comma}");
                }
                sb.AppendLine("    ];");
            }
            else
            {
                sb.AppendLine("    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];");
            }

            sb.AppendLine("  }");
            sb.AppendLine("  htmlRoot = domStore[0].cloneNode(true);");
            sb.AppendLine($"  objStorage = new Array({bindings.Count});");

            // Element path mapping
            for (int i = 0; i < bindings.Count; i++)
            {
                sb.AppendLine($"  objStorage[{i}] = GetElementFromPath(htmlRoot, [{i + 1}]);");
            }

            sb.AppendLine($"  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, {liveBinderCount}, 0);");
            sb.AppendLine("}");
            sb.AppendLine();

            // Skin getter function
            sb.AppendLine($"function {ir.TemplateName}() {{");
            sb.AppendLine($"  if (!{ir.TemplateName}_var)");
            sb.AppendLine($"    {ir.TemplateName}_var = Skin_factory({ir.ControlTypeName}, {ir.ModelTypeName}, {ir.TemplateName}_factory, \"0\");");
            sb.AppendLine($"  return {ir.TemplateName}_var;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static List<ExpressionBindingNode> CollectBindings(List<IRNode> nodes)
        {
            var result = new List<ExpressionBindingNode>();
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                    result.Add(binding);
                result.AddRange(CollectBindings(node.Children));
            }
            return result;
        }

        private static string CollectHtml(List<IRNode> nodes)
        {
            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                    sb.Append(html.HtmlContent);
                // Expression slots get placeholder elements
                else if (node is ExpressionBindingNode)
                    sb.Append("<span></span>");
            }
            return sb.ToString();
        }

        private static string EscapeJs(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }
    }
}
