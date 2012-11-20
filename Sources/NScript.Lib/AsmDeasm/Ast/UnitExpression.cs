using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    abstract class UnitExpression : SimpleExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        protected UnitExpression(
            ExecutionContext context,
            InstructionBlock instructionBlock,
            Expression[] expressions)
            : base(context, instructionBlock, expressions)
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        { get { return AstType.UnitWork; } }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
