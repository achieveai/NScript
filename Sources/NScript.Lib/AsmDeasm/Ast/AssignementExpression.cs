using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    abstract class AssignementExpression : SimpleExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public AssignementExpression(
            ExecutionContext context,
            InstructionBlock block,
            Expression assignementValue,
            bool isStatement,
            params Expression[] expressions)
            : base(context, block, expressions)
        {
            this.Value = assignementValue;
            this.IsStatement = isStatement;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Expression Value
        { get; private set; }

        public override AstType NodeType
        { get { return AstType.Assignment; } }
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
