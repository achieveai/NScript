using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.ILParser;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class LoadArrayElement : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadArrayElement(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            :base(context, instruction, expressions)
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public override void Write(System.IO.TextWriter textWriter, string prefix = "", string indentation = "")
        {
            textWriter.Write(prefix);
            Expression.WriteWrapped(this.DependentExpressions[0], textWriter);
            textWriter.Write('[');
            this.DependentExpressions[1].Write(textWriter);
            textWriter.Write(']');
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
