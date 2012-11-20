using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using System.IO;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class StoreLocal : AssignementExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public StoreLocal(
            ExecutionContext context,
            InstructionBlock block,
            bool isStatement,
            params Expression[] expressions)
            : base(context, block, expressions[0], isStatement, expressions)
        {
            this.LocalVariableIndex = (int)block.Instruction.OpCodeArgument;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public int LocalVariableIndex
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
                this.Context.Variables[this.LocalVariableIndex]);

            this.Value.Write(
                textWriter,
                prefix,
                indentation);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
