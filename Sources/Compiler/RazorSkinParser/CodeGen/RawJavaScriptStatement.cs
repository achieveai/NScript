using System.Collections.Generic;
using System.Linq;
using NScript.JST;
using NScript.Utils;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// A JST statement that emits pre-generated JavaScript text verbatim.
    /// Used by the Razor skin code generator to inject compiled template output
    /// into the NScript converter pipeline without constructing a full JST tree.
    /// </summary>
    public class RawJavaScriptStatement : Statement
    {
        private readonly string _jsText;

        public RawJavaScriptStatement(string jsText)
            : base(null, null)
        {
            _jsText = jsText;
        }

        public string JsText => _jsText;

        public override void Serialize(ICustomSerializer serializer)
        {
            serializer.AddValue("raw", _jsText);
        }

        public override void Write(JSWriter writer)
        {
            // Write the raw JS text line by line using the writer's identifier output.
            // WriteIdentifier outputs the string as-is without quotes.
            var lines = _jsText.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.TrimEnd('\r');
                if (!string.IsNullOrEmpty(trimmed))
                {
                    writer.WriteNewLine();
                    writer.WriteIdentifier(trimmed);
                }
            }
        }
    }

    /// <summary>
    /// A JST statement that emits pre-generated JavaScript text with lazy identifier
    /// resolution. Identifier names are resolved during Write() (the final JS output pass)
    /// when all scope names have been finalized, ensuring that mangled placeholder names
    /// are replaced with the correct scope-resolved (potentially minified) names.
    /// </summary>
    public class ResolvedJavaScriptStatement : Statement
    {
        private readonly string _jsText;
        private readonly Dictionary<string, IIdentifier> _identifierMappings;
        private readonly Dictionary<string, IList<IIdentifier>> _typeIdentifierMappings;

        public ResolvedJavaScriptStatement(
            string jsText,
            Dictionary<string, IIdentifier> identifierMappings,
            Dictionary<string, IList<IIdentifier>> typeIdentifierMappings)
            : base(null, null)
        {
            _jsText = jsText;
            _identifierMappings = identifierMappings;
            _typeIdentifierMappings = typeIdentifierMappings;
        }

        public override void Serialize(ICustomSerializer serializer)
        {
            serializer.AddValue("raw", _jsText);
        }

        public override void Write(JSWriter writer)
        {
            // Resolve identifiers lazily at write time when scope names are finalized
            var js = _jsText;

            // Replace factory/method identifiers (longer names first to avoid partial matches)
            foreach (var kvp in _identifierMappings.OrderByDescending(k => k.Key.Length))
            {
                var resolvedName = kvp.Value.GetName();
                js = js.Replace(kvp.Key, resolvedName);
            }

            // Replace type identifiers (compound identifiers get joined with '.')
            foreach (var kvp in _typeIdentifierMappings.OrderByDescending(k => k.Key.Length))
            {
                var resolvedName = GetCompoundName(kvp.Value);
                js = js.Replace(kvp.Key, resolvedName);
            }

            // Write the resolved JS text line by line
            var lines = js.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.TrimEnd('\r');
                if (!string.IsNullOrEmpty(trimmed))
                {
                    writer.WriteNewLine();
                    writer.WriteIdentifier(trimmed);
                }
            }
        }

        private static string GetCompoundName(IList<IIdentifier> identifiers)
        {
            if (identifiers.Count == 1)
                return identifiers[0].GetName();

            var parts = new string[identifiers.Count];
            for (int i = 0; i < identifiers.Count; i++)
                parts[i] = identifiers[i].GetName();

            return string.Join(".", parts);
        }
    }
}
