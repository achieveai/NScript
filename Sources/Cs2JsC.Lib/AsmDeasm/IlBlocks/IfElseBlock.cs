using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    class IfElseBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        internal IfElseBlock(
            Block condition,
            Block ifBlock,
            Block elseBlock)
            : base(new Block[]{condition, ifBlock, elseBlock})
        {
            this.Condition = condition;
            this.IfBlock = ifBlock;
            this.ElseBlock = elseBlock;
        }

        internal IfElseBlock(
            Block condition,
            Block ifBlock)
            : base(new Block[]{condition, ifBlock})
        {
            this.Condition = condition;
            this.IfBlock = ifBlock;
        }

        public static Block Create(
            Block parentBlock,
            int blockIndex)
        {
            // If statement will provide two next locations.
            // 1. If block
            // 2. Else block, or Continuation point if there is no else block.
            //
            // To check if second block is continuation block, we need to check if
            // it is If block's successor. Now there is yet another set of problems.
            // 
            // We can exit these blocks by
            // 1. natural flow, This is most common.
            // 2. Return statement, next common.
            // 3. break and continue statement.
            //
            // Natural flow is pretty simple to follow, it will lead to continuation block.
            // Return will have no successors.
            // Break will have block next to parent loop block.
            // Continue will either be start of conditional block (in while case) or some n blocks before
            // end which are increment blocks in "for" block.
            //

            if (!ConditionalStatement.IsConditional(parentBlock.Children[blockIndex]) ||
                parentBlock.Children[blockIndex].StackBefore > 0 ||
                parentBlock.Children[blockIndex].StackAfter > 0)
            {
                return null;
            }

            int[] indexes = new int[] {
                parentBlock.Children.IndexOf(parentBlock.Children[blockIndex].Successors[0]),
                parentBlock.Children.IndexOf(parentBlock.Children[blockIndex].Successors[1])};

            int ifBlockIndex =indexes[0];
            int secondBlockIndex = indexes[1];

            Block firstIfBlock = parentBlock.Children[blockIndex].Successors[0];
            Block continuationBlock = parentBlock.Children[blockIndex].Successors[1];

            if (secondBlockIndex < ifBlockIndex &&
                secondBlockIndex != -1)
            {
                int tmp = ifBlockIndex;
                ifBlockIndex = secondBlockIndex;
                secondBlockIndex = tmp;

                var tmpBlock = firstIfBlock;
                firstIfBlock = continuationBlock;
                continuationBlock = tmpBlock;
            }

            if (ifBlockIndex < blockIndex)
            {
                return null;
            }

            List<Block> rangeSuccessors = IfElseBlock.GetIfElseBlocksSuccessors(
                parentBlock,
                blockIndex,
                ifBlockIndex,
                secondBlockIndex);

            // The secondBlockIndex is in the list of successors for ifBlock.
            // This means that there is no else block.
            //
            if (rangeSuccessors.Count == 0 ||
                rangeSuccessors[0] == continuationBlock)
            {
                // TODO: We need to check for break and continue statements, but we don't really care
                // about them right now.
                //

                secondBlockIndex = secondBlockIndex == -1 ? parentBlock.Children.Count : secondBlockIndex;

                List<Block> ifBlockMembers = new List<Block>();
                for (int i = ifBlockIndex; i < secondBlockIndex; i++)
                {
                    ifBlockMembers.Add(parentBlock.Children[i]);
                }

                if (ifBlockMembers.Count == 0)
                {
                    throw new NotSupportedException();
                }

                Block ifBlock = new Block(ifBlockMembers.ToArray());

                return new IfElseBlock(
                    parentBlock.Children[blockIndex],
                    ifBlock);
            }
            else
            {
                int continousBlockIndex = parentBlock.Children.IndexOf(rangeSuccessors[0]);

                if (continousBlockIndex < secondBlockIndex && continousBlockIndex != -1)
                {
                    return null;
                }

                rangeSuccessors = IfElseBlock.GetIfElseBlocksSuccessors(
                    parentBlock,
                    blockIndex,
                    secondBlockIndex,
                    continousBlockIndex);

                if (rangeSuccessors.Count != 0 && parentBlock.Children.IndexOf(rangeSuccessors[0]) != continousBlockIndex)
                {
                    return null;
                }

                int ifBlockEnd = secondBlockIndex == -1 ? parentBlock.Children.Count : secondBlockIndex;

                List<Block> ifBlockMembers = new List<Block>();
                for (int i = ifBlockIndex; i < ifBlockEnd; i++)
                {
                    ifBlockMembers.Add(parentBlock.Children[i]);
                }

                if (ifBlockMembers.Count == 0)
                {
                    throw new NotSupportedException();
                }

                List<Block> elseBlockMembers = new List<Block>();
                int elseBlockEnd = continousBlockIndex == -1 ? parentBlock.Children.Count : continousBlockIndex;

                for (int i = secondBlockIndex; i < elseBlockEnd; i++)
                {
                    elseBlockMembers.Add(parentBlock.Children[i]);
                }

                Block ifBlock = new Block(ifBlockMembers.ToArray());
                Block elseBlock = new Block(elseBlockMembers.ToArray());

                return new IfElseBlock(
                    parentBlock.Children[blockIndex],
                    ifBlock,
                    elseBlock);
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block Condition
        { get; private set; }

        public Block IfBlock
        { get; private set; }

        public Block ElseBlock
        { get; private set; }
        #endregion

        #region public functions
        public override IList<Ast.IAstNode> ToExpressions(ExecutionContext executionContext, Stack<Ast.Expression> expressionStack)
        {
            this.Condition.ToExpressions(executionContext, expressionStack);
            Ast.Expression conditionExperssion = Ast.BinaryExpression.NegateExpression(expressionStack.Pop());
            var ifExperssions = this.IfBlock.ToExpressions(executionContext, new Stack<Ast.Expression>());
            IList<Ast.IAstNode> elseExpressions = null;

            if (this.ElseBlock != null)
            {
                elseExpressions = this.ElseBlock.ToExpressions(executionContext, new Stack<Ast.Expression>());
            }

            return new Ast.IAstNode[]{
                new Ast.IfElseExpression(
                    conditionExperssion,
                    ifExperssions,
                    elseExpressions)};
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions\
        private static bool IsBreakStatement(
            Block currentBlock,
            Block block)
        {
            Block parent = currentBlock.Parent;

            while (parent != null)
            {
                if (parent is DoWhileBlock)
                {
                    return parent.Successors[0] == block;
                }
                parent = parent.Parent;
            }

            return false;
        }

        private static bool IsContinueStatement(
            Block currentBlock,
            Block block)
        {
            Block parent = currentBlock.Parent;

            while (parent != null)
            {
                if (parent is DoWhileBlock)
                {
                    var whileBlock = (DoWhileBlock)parent;
                    return whileBlock.Condition == block;
                }

                parent = parent.Parent;
            }

            return false;
        }

        private static List<Block> GetIfElseBlocksSuccessors(
            Block parentBlock,
            int ifBlockIndex,
            int startIndex,
            int endIndex)
        {
            var kvPair = parentBlock.GetRangePredecessorAndSuccessor(
                startIndex,
                (endIndex == -1 ? parentBlock.Children.Count : endIndex) - 1);

            if (!kvPair.HasValue)
            {
                return null;
            }

            var psPair = kvPair.Value;

            // IfBlock should only have one and only one predecessor and that predecessor is ConditionalStatement
            //
            if (psPair.Key.Count != 1 &&
                psPair.Key[0] != parentBlock.Children[ifBlockIndex])
            {
                return null;
            }

            for (int i = psPair.Value.Count - 1; i >= 0; i--)
            {
                if (IfElseBlock.IsBreakStatement(
                        parentBlock.Children[ifBlockIndex],
                        psPair.Value[i]) ||
                    IfElseBlock.IsContinueStatement(
                        parentBlock.Children[ifBlockIndex],
                        psPair.Value[i]))
                {
                    psPair.Value.RemoveAt(i);
                }
            }

            if (psPair.Value.Count > 1)
            {
                return null;
            }

            return psPair.Value;
        }

        protected override void WriteHeadToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine("{0} If-Block Begin", linePrefix);
            base.WriteHeadToStream(writer, linePrefix);
        }

        protected override void WriteBodyToStream(System.IO.TextWriter writer, string linePrefix)
        {
            string linePrefix2 = linePrefix + " .  ";
            writer.WriteLine("{0} Condition", linePrefix);
            this.Condition.Write(writer, linePrefix2);
            writer.WriteLine("{0} IfBlock", linePrefix);
            this.IfBlock.Write(writer, linePrefix2);

            if (this.ElseBlock != null)
            {
                writer.WriteLine("{0} ElseBlock", linePrefix);
                this.ElseBlock.Write(writer, linePrefix2);
            }
        }

        protected override void WriteTailToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine(
                "{0} If-Block End Id:{1}",
                linePrefix,
                this.Id);
        }
        #endregion
    }
}
