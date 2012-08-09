using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class StoreToReference : UnaryExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public StoreToReference(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, instruction, expressions)
        {
            this.Address = expressions[0];
            this.Value = expressions[1];

            if (instruction.StackAfter == 0)
            {
                this.IsStatement = true;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Expression Address
        { get; private set; }

        public Expression Value
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write("{0}(", prefix);
            this.Address.Write(textWriter);
            textWriter.Write(").sr(");
            this.Value.Write(textWriter);
            textWriter.Write(")");
            if (this.IsStatement)
            {
                textWriter.Write(";");
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
