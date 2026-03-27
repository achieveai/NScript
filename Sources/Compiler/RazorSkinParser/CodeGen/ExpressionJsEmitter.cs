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
            string templateParentParam = "tp")
        {
            // Replace "Model." with DataContext param and "Control." with TemplateParent param
            var expr = csharpExpression
                .Replace("Model.", dataContextParam + ".")
                .Replace("Control.", templateParentParam + ".");

            // Convert property accesses to getter calls: .PropertyName -> .get_propertyName()
            // Match ANY uppercase-initial identifier after "." unconditionally.
            expr = Regex.Replace(expr, @"\.([A-Z])(\w*)",
                match => $".get_{match.Groups[1].Value.ToLower()}{match.Groups[2].Value}()");

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
