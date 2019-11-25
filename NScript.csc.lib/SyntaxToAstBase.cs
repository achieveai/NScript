namespace NScript.Csc.Lib
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SyntaxToAstBase : SyntaxWalker
    {
        public override void Visit(SyntaxNode node)
        {
            switch (node.Kind())
            {
            }
        }
    }
}
