using System.Collections.Generic;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class BinderEmitter
    {
        // Binder type flags matching SkinBinderInfo.BinderType enum.
        // Low nibble = source type, high nibble = binder kind.
        // PropertyBinder is ALWAYS set — both OneTime and OneWay use the 8-param
        // SkinBinderInfo constructor. The difference is the propertyNames array:
        //   OneWay: ["PropA", "PropB"] → LiveBinder watches for changes
        //   OneTime: [] → no LiveBinder, initial value still flows via SetPropertyValue
        private const int DATACONTEXT = 0x1;       // BinderType.DataContext
        private const int TEMPLATEPARENT = 0x3;     // BinderType.TemplateParent
        private const int PROPERTYBINDER = 0x10;    // BinderType.PropertyBinder

        public static string EmitSkinBinderInfo(
            ExpressionBindingNode binding,
            int objectIndex,
            int binderIndex,
            ISet<string> knownFunctionNames = null)
        {
            var sb = new StringBuilder();
            var deps = binding.Classification.Dependencies;
            var expr = binding.Classification.CSharpExpression;

            // Getter function — use "dc" for DataContext source and "tp" for TemplateParent source
            var getterJs = ExpressionJsEmitter.ToJsGetter(expr, "dc", "tp", knownFunctionNames);
            var paramName = binding.Classification.SourceKind == BindingSourceKind.TemplateParent ? "tp" : "dc";
            sb.Append("Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(");
            sb.Append($"[function({paramName}) {{ return {getterJs}; }}]");

            // Property names array for live binding
            sb.Append(", [");
            for (int i = 0; i < deps.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append($"\"{deps[i].PropertyName}\"");
            }
            sb.Append("]");

            // Target setter — use fully qualified JS-mangled names matching XWML SkinBinderHelper pattern
            var setter = binding.Target switch
            {
                ExpressionTarget.TextContent => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent",
                ExpressionTarget.Attribute => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute",
                ExpressionTarget.CssClass => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetClassName",
                ExpressionTarget.Style => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetStyle",
                _ => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent"
            };
            sb.Append($", {setter}");

            // Compute binder type flags from source kind.
            // PropertyBinder flag is ALWAYS set (both OneTime and OneWay use PropertyBinder).
            // The difference between OneTime and OneWay is the propertyNames array:
            //   OneWay has ["PropA", "PropB"] → LiveBinder watches for changes
            //   OneTime has [] → no LiveBinder, but initial value still flows via SetPropertyValue
            int flags;
            if (binding.Classification.SourceKind == BindingSourceKind.TemplateParent)
                flags = PROPERTYBINDER | TEMPLATEPARENT;  // 0x10 | 0x03 = 19
            else
                flags = PROPERTYBINDER | DATACONTEXT;     // 0x10 | 0x01 = 17
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
