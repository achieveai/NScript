using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class LoadFromReference : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadFromReference(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base (context, instruction, expressions)
        {
            this.Address = expressions[0];
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Expression Address
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write("{0}", prefix);
            this.Address.Write(textWriter);
            textWriter.Write(".lr()");
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
