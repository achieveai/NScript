namespace NScript.JST.Writer
{
    using NScript.Utils;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class EndOfStatement : TokenBase
    {
        public EndOfStatement(int level) : base(TokenType.EndOfStatement, null)
        {
            Level = level;
        }

        public int Level { get; }
    }
}
