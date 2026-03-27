using System.Text.RegularExpressions;

namespace NScript.RazorSkin.CodeGen
{
    public static class ExpressionJsEmitter
    {
        /// <summary>
        /// Convert a C# property access like "Model.Name" to a JS getter call like "src.get_name()".
        /// </summary>
        public static string ToJsGetter(string csharpExpression, string sourceParam = "src")
        {
            // Replace "Model." with source param
            var expr = csharpExpression
                .Replace("Model.", sourceParam + ".")
                .Replace("Control.", sourceParam + ".");

            // Convert property accesses to getter calls: .PropertyName -> .get_propertyName()
            expr = Regex.Replace(expr, @"\.([A-Z])(\w*?)(?=[.\s\)\]\+\-\*\/\,\;]|$)",
                match =>
                {
                    var propName = match.Groups[1].Value.ToLower() + match.Groups[2].Value;
                    return $".get_{propName}()";
                });

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
