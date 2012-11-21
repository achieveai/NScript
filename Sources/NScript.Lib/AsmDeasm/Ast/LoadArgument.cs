using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class LoadArgument : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadArgument(
            ExecutionContext context,
            InstructionBlock instructionBlock,
            params UnitExpression[] expressions)
            : base(context, instructionBlock, expressions)
        {
            this.ArgumentIndex = (int)instructionBlock.Instruction.OpCodeArgument;

            if (instructionBlock.Instruction.CodeOp.Code == Ops.IlOpCode.LoadArgumentAddress)
            {
                this.IsReference = true;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public int ArgumentIndex
        { get; private set; }

        public bool IsReference
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write("{0}{1}", prefix, this.Context.MethodSignature.GetArgumentId(this.ArgumentIndex));
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
