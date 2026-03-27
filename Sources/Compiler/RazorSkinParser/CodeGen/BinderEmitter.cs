using System.Collections.Generic;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class BinderEmitter
    {
        // Binder type flags matching XwmlParser BinderType enum.
        // Low nibble = source type, high nibble = binder kind.
        // DataContext = 0x1, TemplateParent = 0x3, PropertyBinder = 0x10
        private const int DATACONTEXT = 0x1;       // BinderType.DataContext
        private const int TEMPLATEPARENT = 0x3;     // BinderType.TemplateParent
        private const int PROPERTYBINDER = 0x10;    // BinderType.PropertyBinder
        private const int CSSBINDER = 0x50;         // BinderType.CssBinder
        private const int STYLEBINDER = 0x60;       // BinderType.StyleBinder
        private const int ATTRIBUTEBINDER = 0x70;   // BinderType.AttributeBinder

        // Combined flags used in generated JS:
        // ONEWAY_DATACONTEXT  = PropertyBinder | DataContext = 0x11 = 17
        // ONETIME_DATACONTEXT = DataContext = 0x01 = 1
        // ONEWAY_TEMPLATEPARENT = PropertyBinder | TemplateParent = 0x13 = 19
        // ONETIME_TEMPLATEPARENT = TemplateParent = 0x03 = 3
        private const int ONEWAY_DATACONTEXT = PROPERTYBINDER | DATACONTEXT;   // 17
        private const int ONETIME_DATACONTEXT = DATACONTEXT;                    // 1
        private const int ONEWAY_TEMPLATEPARENT = PROPERTYBINDER | TEMPLATEPARENT; // 19
        private const int ONETIME_TEMPLATEPARENT = TEMPLATEPARENT;              // 3

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

            // Compute binder type flags from source kind and binding mode
            int flags;
            bool isOneWay = binding.Classification.Mode == BindingMode.OneWay;
            if (binding.Classification.SourceKind == BindingSourceKind.TemplateParent)
                flags = isOneWay ? ONEWAY_TEMPLATEPARENT : ONETIME_TEMPLATEPARENT;
            else
                flags = isOneWay ? ONEWAY_DATACONTEXT : ONETIME_DATACONTEXT;
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
