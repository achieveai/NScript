using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.Ops;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class LoadLocalVariable : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadLocalVariable(
            ExecutionContext context,
            InstructionBlock instruction,
            params UnitExpression[] dependentExpressions)
            : base(context, instruction, dependentExpressions)
        {
            this.VariableIndex = (int)instruction.Instruction.OpCodeArgument;

            if (this.Block.Instruction.CodeOp.Code == IlOpCode.LoadLocalAddress)
            {
                this.IsReference = true;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public int VariableIndex
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
            textWriter.Write(
                "{0}{1}{2}",
                prefix,
                this.IsReference ? "__$" : string.Empty,
                this.Context.Variables[this.VariableIndex].Name);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
