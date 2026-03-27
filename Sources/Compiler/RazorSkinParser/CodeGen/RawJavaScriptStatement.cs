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
}
