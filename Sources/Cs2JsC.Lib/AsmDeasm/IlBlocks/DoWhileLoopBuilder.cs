using System;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    internal class DoWhileLoopBuilder
    {
        public static void Process(
            Block block)
        {
            for (int i = 0; i < block.Children.Count; i++)
            {
                if (IfElseBlockBuilder.IsFlowRootBlock(block) ||
                    DoWhileLoopBuilder.Create(block, i) == null)
                {
                    IfElseBlockBuilder.Process(block.Children[i]);
                }
            }
        }

        /// <summary>
        /// All the loops will have conditional statement that makes a jump
        /// backwards. We just need to check for such conditional statements.
        /// 
        /// Do-while loop's body will have 2 or more predecessors, one is the condition
        /// and the other one would be incoming flow.
        /// 
        /// for While loop and for loop, there will be only 1 predecessor for main loop.
        /// </summary>
        /// <param name="parentBlock"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static DoWhileBlock Create(
            Block parentBlock,
            int index)
        {
            if (index < 1)
            {
                return null;
            }

            Block conditionalBlock = parentBlock.Children[index];

            if (!ConditionalStatementBuilder.IsConditionalChild(conditionalBlock))
            {
                return null;
            }

            Block nextBlock = parentBlock.Children[index + 1];
            Block branchedBlock = conditionalBlock.Successors[0] != nextBlock
                    ? conditionalBlock.Successors[0]
                    : conditionalBlock.Successors[1];

            if (branchedBlock.BlockStartIndex > conditionalBlock.BlockStartIndex)
            {
                return null;
            }

            int indexOfBranch = parentBlock.Children.IndexOf(branchedBlock);

            indexOfBranch = Math.Max(0, indexOfBranch);

            // We know that condition block will have predecessor that is a block before
            // indexOfBranch in while and for loop cases.
            //
            if (!parentBlock.GetRangePredecessorAndSuccessor(
                indexOfBranch,
                index - 1).HasValue)
            {
                throw new NotSupportedException("Gotos are not yet supported.");
            }

            var blockChildren = parentBlock.CreateChildrenArray<Block>(
                indexOfBranch,
                index);

            if (branchedBlock.Predecessors.Count > 1)
            {
                return new DoWhileBlock(
                    blockChildren.Length > 1
                        ? new Block(blockChildren)
                        : blockChildren[0],
                    conditionalBlock);
            }

            if (indexOfBranch == 0)
            {
                throw new InvalidOperationException("While loop will always have branch instruction before it");
            }

            // We know that condition block will have predecessor that is a block before
            // indexOfBranch in while and for loop cases.
            //
            if (!parentBlock.GetRangePredecessorAndSuccessor(
                indexOfBranch - 1,
                index).HasValue)
            {
                throw new NotSupportedException("Gotos are not yet supported.");
            }

            return new WhileBlock(
                parentBlock.Children[indexOfBranch - 1],
                blockChildren.Length > 1
                    ? new Block(blockChildren)
                    : blockChildren[0],
                conditionalBlock);
        }
    }
}
