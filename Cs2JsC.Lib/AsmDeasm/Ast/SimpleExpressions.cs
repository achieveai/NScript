using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    abstract class SimpleExpression : Expression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        protected SimpleExpression(
            ExecutionContext context,
            InstructionBlock instructionBlock,
            Expression[] expressions)
            : base(context, expressions)
        {
            this.Block = instructionBlock;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public InstructionBlock Block
        { get; private set; }
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
