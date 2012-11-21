using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.CLR.Decompiler.Blocks
{
    internal class SpecialBranch : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public SpecialBranch(
            InstructionBlockCollection block,
            bool isBreak)
            : base(new Block[]{block})
        {
            this.IsBreak = isBreak;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public bool IsBreak
        { get; private set; }
        #endregion

        #region public functions
        /*
        public override IList<Ast.IAstNode> ToExpressions(ExecutionContext executionContext, Stack<Ast.Expression> expressionStack)
        {
            return new Ast.IAstNode[]{
                new Ast.SpecialBranchExpression(
                    executionContext,
                    this.IsBreak)
            };
        }
         */
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        protected override void AddChild(Block block)
        {
            if (this.Children.Count > 0)
            {
                throw new InvalidOperationException();
            }
            base.AddChild(block);
        }
        #endregion
    }
}
