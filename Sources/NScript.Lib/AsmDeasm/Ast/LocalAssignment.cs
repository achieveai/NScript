using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using System.IO;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class LocalAssignment : AssignementExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LocalAssignment(
            ExecutionContext context,
            InstructionBlock instruction,
            bool isStatement,
            params Expression[] expressions)
            : base(context, instruction, expressions[0], isStatement, expressions)
        {
            this.LocalIndex = (int)instruction.Instruction.OpCodeArgument;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public int LocalIndex
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
                this.Context.Variables[this.LocalIndex].Name);

            this.Value.Write(textWriter);
            if (this.IsStatement)
            {
                textWriter.Write(';');
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
