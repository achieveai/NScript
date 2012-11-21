using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.Ast
{
    abstract class BaseBlock : IAstNode
    {
        public abstract AstType NodeType
        { get; }

        public abstract void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "");
    }
}
