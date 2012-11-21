using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.Ast;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class ConditionalValue : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        private ConditionalValue(
            Block condition,
            Block loadConst1,
            Block branch,
            Block loadConst2)
            : base(new Block[] { condition, loadConst1, branch, loadConst2 })
        {
            this.Conditional = condition;

            if ((int)loadConst1.GetInstructionAt(0).OpCodeArgument == 0)
            {
                this.LoadConst0 = loadConst1;
                this.LoadConst1 = loadConst2;
            }
            else
            {
                this.LoadConst0 = loadConst2;
                this.LoadConst1 = loadConst1;
            }
        }

        internal static ConditionalValue Create(
            Block parentBlock,
            int blockIndex)
        {
            // We need atleast 5 elements, 4 for this class itself, and 1 consumer
            //
            if (blockIndex >= parentBlock.Children.Count - 5)
            {
                return null;
            }

            int tailSize = 0;
            if (ConditionalStatement.IsConditionalTail(
                parentBlock.Children,
                blockIndex + 1,
                ref tailSize) &&
                tailSize == 3 &&
                parentBlock.Children[blockIndex].Successors.Count == 2)
            {
                var conditional = parentBlock.Children[blockIndex];
                var loadConst1 = parentBlock.Children[blockIndex + 1];
                var branch = parentBlock.Children[blockIndex + 2];
                var loadConst2 = parentBlock.Children[blockIndex + 3];

                // We need both loadConst in conditional
                //
                if (!conditional.Successors.Contains(loadConst1) ||
                    !conditional.Successors.Contains(loadConst2))
                {
                    return null;
                }

                return new ConditionalValue(
                    conditional,
                    loadConst1,
                    branch,
                    loadConst2);
            }

            return null;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block Conditional
        { get; private set; }

        public Block LoadConst1
        { get; private set; }

        public Block LoadConst0
        { get; private set; }
        #endregion

        #region public functions
        public override IList<Ast.IAstNode> ToExpressions(
            ExecutionContext executionContext,
            Stack<Ast.Expression> expressionStack)
        {
            var tmp = this.Conditional.ToExpressions(executionContext, expressionStack);

            if (ConditionalStatement.GetTrueBranch(this.Conditional) == this.LoadConst1)
            {
                return tmp;
            }
            else
            {
                var exp = expressionStack.Pop();
                exp = BinaryExpression.NegateExpression(exp);
                expressionStack.Push(exp);
                return new Expression[] { exp };
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
