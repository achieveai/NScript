namespace NScript.JST
{
    using NScript.Utils;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EmptyStatement : Statement
    {
        public EmptyStatement(
            Location location,
            IdentifierScope scope)
            : base(location, scope)
        {
        }

        public override void Write(JSWriter writer)
        {
            writer.Write(Symbols.SemiColon);
        }
    }
}
