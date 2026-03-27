using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class BinderEmitter
    {
        public static string EmitSkinBinderInfo(
            ExpressionBindingNode binding,
            int objectIndex,
            int binderIndex)
        {
            var sb = new StringBuilder();
            var deps = binding.Classification.Dependencies;
            var expr = binding.Classification.CSharpExpression;

            // Getter function
            var getterJs = ExpressionJsEmitter.ToJsGetter(expr);
            sb.Append("SkinBinderInfo_factory(");
            sb.Append($"[function(src) {{ return {getterJs}; }}]");

            // Property names array for live binding
            sb.Append(", [");
            for (int i = 0; i < deps.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append($"\"{deps[i].PropertyName}\"");
            }
            sb.Append("]");

            // Target setter
            var setter = binding.Target switch
            {
                ExpressionTarget.TextContent => "SetTextContent",
                ExpressionTarget.Attribute => "SetAttribute",
                ExpressionTarget.CssClass => "SetClassName",
                ExpressionTarget.Style => "SetStyle",
                _ => "SetTextContent"
            };
            sb.Append($", {setter}");

            // Binder type flags: 17 = ONEWAY|DATACONTEXT, 1 = ONETIME|DATACONTEXT
            var flags = binding.Classification.Mode == BindingMode.OneWay ? "17" : "1";
            sb.Append($", {flags}");

            // Object index, binder index
            sb.Append($", {objectIndex}, {binderIndex}");

            // Converter (null), default value
            sb.Append(", null, \"\"");
            sb.Append(")");

            return sb.ToString();
        }
    }
}
