using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.ILParser;
using NScript.Lib.AsmDeasm.IlBlocks;

namespace NScript.Lib.AsmDeasm.Ast
{
    class TemporaryAssignement : StoreLocal
    {
        public TemporaryAssignement(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions
            )
            :base(context, instruction, false, expressions){}
    }
}
