using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using System.IO;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class ArgumentAssignment : AssignementExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ArgumentAssignment(
            ExecutionContext context,
            InstructionBlock block,
            bool isStatement,
            params Expression[] expressions)
            : base(context, block, expressions[0], isStatement, expressions)
        {
            this.ArgumentIndex = (int)block.Instruction.OpCodeArgument;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public int ArgumentIndex
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write(
                "{0}{1} = ",
                prefix,
                this.Context.MethodSignature.GetArgumentId(this.ArgumentIndex));

            this.Value.Write(textWriter);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
