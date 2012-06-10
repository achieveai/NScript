using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    public interface IAstNode
    {
        AstType NodeType
        { get; }

        void Write(
            TextWriter textWriter,
            string prefix = "",
            string indentation = "");
    }
}
