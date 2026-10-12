namespace NScript.JST
{
    /// <summary>
    /// A comment on its own line that marks the end of a section of a dev bundle, e.g.
    /// <c>/*@T Namespace.Type*/</c>. Patch emit places a changed piece just before its
    /// section's marker. Visitors see an <see cref="EmptyStatement"/>, so transforms drop it.
    /// </summary>
    public sealed class MarkerStatement : EmptyStatement
    {
        public MarkerStatement(IdentifierScope scope, string text)
            : base(null, scope)
        {
            if (!text.StartsWith("/*@", System.StringComparison.Ordinal)
                || text.IndexOf("*/", System.StringComparison.Ordinal) != text.Length - 2)
            {
                throw new System.ArgumentException("A marker is one '/*@...*/' comment.", nameof(text));
            }

            this.Text = text;
        }

        /// <summary>The marker as written, with its comment delimiters.</summary>
        public string Text { get; }

        /// <summary>The marker text for the end of a type's section.</summary>
        public static string ForType(string typeFullName) => "/*@T " + typeFullName + "*/";

        /// <summary>The marker text for the end of a plugin's section, e.g. its templates.</summary>
        public static string ForSection(string sectionName) => "/*@S " + sectionName + "*/";

        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .WriteIdentifier(this.Text);
        }
    }
}
