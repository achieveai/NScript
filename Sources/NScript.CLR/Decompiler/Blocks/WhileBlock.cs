using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.CLR.Decompiler.Blocks
{
    class WhileBlock : DoWhileBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public WhileBlock(
            Block fallinBlock,
            Block loopBlock,
            Block conditionalBlock)
            : base(new Block[]{fallinBlock, loopBlock, conditionalBlock})
        {
            this.FallinBlock = fallinBlock;
            this.Loop = loopBlock;
            this.Condition = conditionalBlock;
        }

        public WhileBlock(
            Block loopBlock,
            Block conditionalBlock)
            : base(new Block[]{loopBlock, conditionalBlock})
        {
            this.Loop = loopBlock;
            this.Condition = conditionalBlock;
        }

        public static WhileBlock CreateBlock(
            Block rootBlock,
            int blockId)
        {
            if (blockId > rootBlock.Children.Count - 3)
            {
                return null;
            }

            if (rootBlock.Children[blockId].Successors.Count != 1 ||
                rootBlock.Children[blockId].Successors[0] == rootBlock.Children[blockId + 1])
            {
                return null;
            }

            bool isLoop = false;
            int maxLoopLookback = -1;

            if (rootBlock.Children[blockId + 1].Predecessors.Count > 0)
            {
                isLoop = true;
                foreach (var predecessor in rootBlock.Children[blockId + 1].Predecessors)
                {
                    int index = rootBlock.Children.IndexOf(predecessor);
                    maxLoopLookback = Math.Max(index, maxLoopLookback);
                    if (index < blockId)
                    {
                        isLoop = false;
                    }
                }
            }

            if (!isLoop ||
                !ConditionalStatement.IsConditional(rootBlock.Children[maxLoopLookback]))
            {
                return null;
            }

            var conditionalBlockList = new List<Block>();

            int startConditionalBlock = rootBlock.Children.IndexOf(rootBlock.Children[blockId].Successors[0]);

            if (startConditionalBlock > maxLoopLookback)
            {
                return null;
            }

            if (!ConditionalStatement.IsConditional(rootBlock.Children[maxLoopLookback]))
            {
                return null;
            }

            for (int i = startConditionalBlock; i <= maxLoopLookback; i++)
            {
                conditionalBlockList.Add(rootBlock.Children[i]);
            }

            var conditionalBlock = new Block(conditionalBlockList.ToArray());

            conditionalBlockList.Clear();
            for (int i = blockId + 1; i < startConditionalBlock; i++)
            {
                conditionalBlockList.Add(rootBlock.Children[i]);
            }

            var loopBlock = new Block(conditionalBlockList.ToArray());

            return new WhileBlock(
                rootBlock.Children[blockId],
                loopBlock,
                conditionalBlock);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block FallinBlock
        { get; private set; }
        #endregion

        #region public functions
        /*
        public override IList<Ast.IAstNode> ToExpressions(ExecutionContext executionContext, Stack<Ast.Expression> expressionStack)
        {
            List<Ast.IAstNode> returnValue = new List<Ast.IAstNode>();

            returnValue.AddRange(
                this.FallinBlock.ToExpressions(
                    executionContext, expressionStack));

            this.Condition.ToExpressions(executionContext, expressionStack);

            returnValue.Add(
                new Ast.WhileLoop(
                    executionContext,
                    expressionStack.Pop(),
                    false,
                    this.Loop.ToExpressions(executionContext, new Stack<Ast.Expression>())));

            return returnValue;
        }*/

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.ScopeBlock scopeBlock = new AST.ScopeBlock(
                this.Context.ClrContext,
                null);

            variableResolver.PushScope(scopeBlock);
            if (this.Loop is BasicBlock)
            {
                AST.Statement statement = AST.Statement.ToStatement(this.Loop.ToAstNode(variableResolver));
                scopeBlock.AddStatement(statement);
            }
            else
            {
                foreach (var child in this.Loop.Children)
                {
                    AST.Statement statement = AST.Statement.ToStatement(child.ToAstNode(variableResolver));
                    if (statement != null)
                    {
                        scopeBlock.AddStatement(statement);
                    }
                }
            }
            variableResolver.PopScope();

            return new AST.WhileLoop(
                this.Context.ClrContext,
                this.ComputeLocation(),
                WhileBlock.ConditionToAstNode(this.Condition, variableResolver),
                scopeBlock);
        }

        /// <summary>
        /// Conditions to ast node.
        /// </summary>
        /// <param name="conditionalBlock">The conditional block.</param>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>Expression for provided condition</returns>
        internal static AST.Expression ConditionToAstNode(
            Block conditionalBlock,
            VariableResolver variableResolver)
        {
            AST.Expression conditionExpression;
            if (conditionalBlock.Successors.Count == 2)
            {
                // We have to not this condition, because it's only when we match the condition (which is false for our
                // case), we loop.
                conditionExpression = (AST.Expression)conditionalBlock.ToAstNode(variableResolver);
                conditionExpression = new AST.UnaryExpression(
                    conditionalBlock.Context.ClrContext,
                    conditionExpression.Location,
                    conditionExpression,
                    AST.UnaryOperator.LogicalNot);
            }
            else
            {
                conditionExpression = new AST.BooleanLiteral(
                    conditionalBlock.Context.ClrContext,
                    conditionalBlock.ComputeLocation(),
                    true);
            }

            return conditionExpression;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        protected override void WriteHeadToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine("{0} While-Loop Begin", linePrefix);
            base.WriteHeadToStream(writer, linePrefix);
        }

        protected override void WriteBodyToStream(System.IO.TextWriter writer, string linePrefix)
        {
            string linePrefix2 = linePrefix + " .  ";
            if (this.FallinBlock != null)
            {
                writer.WriteLine("{0} FallingBlock", linePrefix);
                this.FallinBlock.Write(writer, linePrefix2);
            }

            writer.WriteLine("{0} Condition", linePrefix);
            this.Condition.Write(writer, linePrefix2);
            writer.WriteLine("{0} Loop:", linePrefix);
            this.Loop.Write(writer, linePrefix2);
        }

        protected override void WriteTailToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine(
                "{0} While-Loop End Id:{1}",
                linePrefix,
                this.Id);
        }
        #endregion
    }
}
