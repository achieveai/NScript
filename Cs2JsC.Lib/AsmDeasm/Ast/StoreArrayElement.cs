using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.ILParser;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class StoreArrayElement : AssignementExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public StoreArrayElement(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, instruction, expressions[2], false, expressions)
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.Assignment; }
        }
        #endregion

        #region public functions
        public override void Write(System.IO.TextWriter textWriter, string prefix = "", string indentation = "")
        {
            textWriter.Write(prefix);
            Expression.WriteWrapped(this.DependentExpressions[0], textWriter);
            textWriter.Write('[');
            this.DependentExpressions[1].Write(textWriter);
            textWriter.Write("] = ");
            this.DependentExpressions[2].Write(textWriter);

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
