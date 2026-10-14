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

        /// <summary>
        /// The marker text inside a generic type's factory, after its prototype: each
        /// instantiation runs what a patch puts before it, in the factory's closure.
        /// </summary>
        public static string ForFactory(string typeFullName) => "/*@G " + typeFullName + "*/";

        /// <summary>The marker text for the end of a plugin's section, e.g. its templates.</summary>
        public static string ForSection(string sectionName) => "/*@S " + sectionName + "*/";

        /// <summary>
        /// The marker text for a line whose one string literal a patch writes again in place,
        /// the line after the marker, e.g. a bundle's CSS.
        /// </summary>
        public static string ForLiteral(string name) => "/*@L " + name + "*/";

        /// <summary>
        /// Writes <paramref name="value"/> as the one string literal on the line after
        /// <paramref name="marker"/>, in place. The line count stays; the map's columns after
        /// the literal on its line move by its change in length. Null when the marker is not
        /// there once, or its next line has no literal.
        /// </summary>
        public static string ReplaceLiteral(
            string script,
            OwaSourceMapper.SourceMap map,
            string scriptFile,
            string marker,
            string value)
        {
            int at = script.IndexOf(marker, System.StringComparison.Ordinal);
            if (at < 0 || script.IndexOf(marker, at + marker.Length, System.StringComparison.Ordinal) >= 0)
            {
                return null;
            }

            int lineStart = script.IndexOf(System.Environment.NewLine, at, System.StringComparison.Ordinal);
            int lineEnd = lineStart < 0 ? -1 : script.IndexOf(System.Environment.NewLine, lineStart + 1, System.StringComparison.Ordinal);
            if (lineEnd < 0)
            {
                lineEnd = script.Length;
            }

            // The literal runs from the line's first quote to its last: only ";" follows it.
            int open = lineStart < 0 ? -1 : script.IndexOf('"', lineStart, lineEnd - lineStart);
            int close = open < 0 ? -1 : script.LastIndexOf('"', lineEnd - 1, lineEnd - open);
            if (close <= open)
            {
                return null;
            }

            string literal = Utils.ToJSString(value);
            int line = 0;
            for (int i = 0; i < lineStart; i++)
            {
                if (script[i] == '\n')
                {
                    line++;
                }
            }

            // The line after the marker's: the writer's new line ends in '\n'.
            line++;
            int lineFirst = lineStart + System.Environment.NewLine.Length;
            map.ShiftColumns(line, open - lineFirst, literal.Length - (close - open - 1), scriptFile);
            return script.Substring(0, open + 1) + literal + script.Substring(close);
        }

        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .WriteIdentifier(this.Text);
        }
    }
}
