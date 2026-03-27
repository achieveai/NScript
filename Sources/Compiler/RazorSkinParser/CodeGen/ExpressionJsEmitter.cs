using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NScript.RazorSkin.CodeGen
{
    public static class ExpressionJsEmitter
    {
        /// <summary>
        /// Convert a C# property access like "Model.Name" to a JS getter call like "src.get_name()".
        /// </summary>
        public static string ToJsGetter(
            string csharpExpression,
            string dataContextParam = "dc",
            string templateParentParam = "tp",
            ISet<string> knownFunctionNames = null)
        {
            // Replace "Model." with DataContext param and "Control." with TemplateParent param
            var expr = csharpExpression
                .Replace("Model.", dataContextParam + ".")
                .Replace("Control.", templateParentParam + ".");

            // Convert property accesses to getter calls: .PropertyName -> .get_propertyName()
            // Match ANY uppercase-initial identifier after "." unconditionally.
            // Skip known function names from @functions blocks (M2) — they should remain
            // as bare function calls, not be transformed to property getters.
            expr = Regex.Replace(expr, @"\.([A-Z])(\w*)",
                match =>
                {
                    var fullName = match.Groups[1].Value + match.Groups[2].Value;
                    if (knownFunctionNames != null && knownFunctionNames.Contains(fullName))
                        return "." + fullName;
                    return $".get_{match.Groups[1].Value.ToLower()}{match.Groups[2].Value}()";
                });

            // Also handle bare function calls (not preceded by ".") that match known names.
            // These appear at the start of an expression or after operators: FormatPrice(dc.get_total())
            // They should not be treated as getters. This is already correct since the regex
            // only matches identifiers preceded by ".", so bare calls are not affected.

            return expr;
        }

        /// <summary>
        /// Convert a C# property name to NScript JS getter function name.
        /// </summary>
        public static string PropertyToGetterName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return propertyName;
            return "get_" + char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }

        /// <summary>
        /// Convert a C# property name to NScript JS setter function name.
        /// </summary>
        public static string PropertyToSetterName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return propertyName;
            return "set_" + char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }
    }
}
