using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.CLR.AST;

namespace NScript.CLR.Decompiler.Blocks
{
    internal class DoWhileBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        internal DoWhileBlock(
            Block loop,
            Block condition)
            : base(new Block[]{loop, condition})
        {
            this.Loop = loop;
            this.Condition = condition;

            /*
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i].Successors.Count == 1
                    && this.Children[i].Children.Count == 1
                    && this.Children[i] is InstructionBlockCollection)
                {
                    if (this.Children[i].Successors[0] == this.Successors[0])
                    {
                        var breakStatement = new SpecialBranch((InstructionBlockCollection)this.Children[i], true);
                    }
                    else if (this.Children[i].Successors[0] == this.Condition)
                    {
                        var continueStatement = new SpecialBranch((InstructionBlockCollection)this.Children[i], false);
                    }
                }
            }
             */
        }

        protected DoWhileBlock(Block[] blocks)
            : base(blocks) { }

        public static DoWhileBlock Create(
            Block parentBlock,
            int index)
        {
            Block currentBlock = parentBlock.Children[index];

            int minPredecessorIndex = int.MaxValue;
            int maxPredecessorIndex = int.MinValue;
            for (int i = 0; i < currentBlock.Predecessors.Count; i++)
            {
                int predecessorIndex = parentBlock.Children.IndexOf(currentBlock.Predecessors[i]);

                minPredecessorIndex = Math.Min(minPredecessorIndex, predecessorIndex);
                maxPredecessorIndex = Math.Max(maxPredecessorIndex, predecessorIndex);
            }

            if ((minPredecessorIndex > index && currentBlock.BlockStartIndex > 0) ||
                maxPredecessorIndex < index)
            {
                // This block either belongs to while/for loop or is just a part of conditional statement.
                //
                return null;
            }

            if (!parentBlock.IsInNonBranchingRangeBlock(index, maxPredecessorIndex))
            {
                return null;
            }

            if (!ConditionalStatement.IsConditional(parentBlock.Children[maxPredecessorIndex]))
            {
                return null;
            }

            var loopBlocks = new List<Block>();

            for (int loopBlockIndex = index; loopBlockIndex < maxPredecessorIndex; loopBlockIndex++)
            {
                loopBlocks.Add(parentBlock.Children[loopBlockIndex]);
            }

            // Let's keep conditionalBlock around else the position will change to index+1.
            //
            var conditionalBlock = parentBlock.Children[maxPredecessorIndex];
            var loopBlock = new Block(loopBlocks.ToArray());

            return new DoWhileBlock(
                loopBlock,
                conditionalBlock);
       }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block Condition
        { get; protected set; }

        public Block Loop
        { get; protected set; }
        #endregion

        #region public functions
        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.Statement statement = Statement.ToStatement(this.Loop.ToAstNode(variableResolver));
            AST.ScopeBlock scopeBlock = statement as AST.ScopeBlock;

            if (scopeBlock == null)
            {
                scopeBlock = new ScopeBlock(
                    this.Context.ClrContext,
                    statement.Location);
                scopeBlock.AddStatement(statement);
            }

            // We have to not this condition, because it's only when we match the condition (which is false for our
            // case), we loop.
            AST.Expression conditionExpression = (Expression) this.Condition.ToAstNode(variableResolver);
            conditionExpression = new UnaryExpression(
                this.Context.ClrContext,
                conditionExpression.Location,
                conditionExpression,
                UnaryOperator.LogicalNot);

            return new AST.DoWhileLoop(
                this.Context.ClrContext,
                this.ComputeLocation(),
                conditionExpression,
                scopeBlock);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        protected override void WriteHeadToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine("{0} Do-While-Loop Begin", linePrefix);
            base.WriteHeadToStream(writer, linePrefix);
        }

        protected override void WriteBodyToStream(System.IO.TextWriter writer, string linePrefix)
        {
            string linePrefix2 = linePrefix + " .  ";
            writer.WriteLine("{0} Loop:", linePrefix);
            this.Loop.Write(writer, linePrefix2);
            writer.WriteLine("{0} Condition", linePrefix);
            this.Condition.Write(writer, linePrefix2);
        }

        protected override void WriteTailToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine(
                "{0} Do-While-Loop End Id:{1}",
                linePrefix,
                this.Id);
        }
        #endregion
    }
}
