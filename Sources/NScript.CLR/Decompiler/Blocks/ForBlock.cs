using System;

namespace NScript.CLR.Decompiler.Blocks
{
    using NScript.CLR.AST;

    class ForBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ForBlock(
            Block fallinBlock,
            Block loopBlock,
            Block incrementBlock,
            Block conditionBlock)
            : base(ForBlock.MakeArray(fallinBlock, loopBlock, incrementBlock, conditionBlock))
        {
            this.FallinBlock = fallinBlock;
            this.LoopBlock = loopBlock;
            this.IncrementBlock = incrementBlock;
            this.ConditionBlock = conditionBlock;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block ConditionBlock { get; set; }

        public Block IncrementBlock { get; set; }

        public Block LoopBlock { get; set; }

        public Block FallinBlock { get; set; }
        #endregion

        #region public functions
        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            Statement fallinBlock = null;
            Statement incrementBlock = null;
            Statement loopBlock = null;

            if (this.FallinBlock != null &&
                !(this.FallinBlock is InstructionBlock))
            {
                fallinBlock = Statement.ToStatement(this.FallinBlock.ToAstNode(variableResolver));
            }

            if (this.IncrementBlock != null && this.IncrementBlock.Children.Count > 0)
            {
                incrementBlock = Statement.ToStatement(this.IncrementBlock.ToAstNode(variableResolver));
            }

            if (this.LoopBlock != null && this.LoopBlock.Children.Count > 0)
            {
                loopBlock = Statement.ToStatement(this.LoopBlock.ToAstNode(variableResolver));
            }

            Expression conditionExpression = null;
            if (this.ConditionBlock.Successors.Count == 2)
            {
                // We have to not this condition, because it's only when we match the condition (which is false for our
                // case), we loop.
                conditionExpression = (AST.Expression)this.ConditionBlock.ToAstNode(variableResolver);
                conditionExpression = new AST.UnaryExpression(
                    this.Context.ClrContext,
                    conditionExpression.Location,
                    conditionExpression,
                    AST.UnaryOperator.LogicalNot);
            }

            ScopeBlock scopeLoopBlock = loopBlock as ScopeBlock;
            if (scopeLoopBlock == null)
            {
                scopeLoopBlock = new ScopeBlock(
                    loopBlock.Context,
                    loopBlock.Location);
                scopeLoopBlock.AddStatement(loopBlock);
            }

            return new AST.ForLoop(
                this.Context.ClrContext,
                null,
                conditionExpression,
                fallinBlock,
                incrementBlock,
                scopeLoopBlock);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static Block[] MakeArray(
            Block fallinBlock,
            Block loopBlock,
            Block incrementBlock,
            Block conditionBlock)
        {
            if (loopBlock == null && incrementBlock == null)
            {
                throw new InvalidOperationException("can't have for with no increment block and loopBlock");
            }

            if (loopBlock == null)
            {
                return new Block[] { fallinBlock, incrementBlock, conditionBlock };
            }
            else if (incrementBlock == null)
            {
                return new Block[] { fallinBlock, loopBlock, conditionBlock };
            }

            return new Block[] { fallinBlock, loopBlock, incrementBlock, conditionBlock };
        }
        #endregion
    }
}
