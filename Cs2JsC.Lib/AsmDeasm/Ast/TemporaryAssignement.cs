using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.ILParser;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;

namespace Cs2JsC.Lib.AsmDeasm.Ast
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
