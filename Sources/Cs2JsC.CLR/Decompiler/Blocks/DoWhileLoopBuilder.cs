using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    internal class DoWhileLoopBuilder
    {
        public static void Process(
            Block block)
        {
            for (int i = 0; i < block.Children.Count; i++)
            {
                Block processingBlock = block.Children[i];
                int preCount = block.Children.Count;

                if (processingBlock is SwitchBlock)
                {
                    SwitchBlock switchBlock = (SwitchBlock)processingBlock;

                    for (int j = 0; j < switchBlock.AllCases.Count; j++)
                    {
                        DoWhileLoopBuilder.Process(switchBlock.AllCases[j]);
                    }
                }
                else if (processingBlock is ExceptionHandlerBlock)
                {
                    ExceptionHandlerBlock exceptionHandlerBlock = (ExceptionHandlerBlock)processingBlock;

                    DoWhileLoopBuilder.Process(exceptionHandlerBlock.TryBlock);
                    DoWhileLoopBuilder.Process(exceptionHandlerBlock.HandlerBlock);
                }
                else
                {
                    if (DoWhileLoopBuilder.Create(block, i, false) != null)
                    {
                        i -= preCount - block.Children.Count;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the infinite loop.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void ProcessInfiniteLoop(
            Block block)
        {
            for (int i = 0; i < block.Children.Count; i++)
            {
                Block processingBlock = block.Children[i];
                int preCount = block.Children.Count;

                if (processingBlock is SwitchBlock)
                {
                    SwitchBlock switchBlock = (SwitchBlock)processingBlock;

                    for (int j = 0; j < switchBlock.AllCases.Count; j++)
                    {
                        DoWhileLoopBuilder.ProcessInfiniteLoop(switchBlock.AllCases[j]);
                    }
                }
                else if (processingBlock is ExceptionHandlerBlock)
                {
                    ExceptionHandlerBlock exceptionHandlerBlock = (ExceptionHandlerBlock)processingBlock;

                    DoWhileLoopBuilder.ProcessInfiniteLoop(exceptionHandlerBlock.TryBlock);
                    DoWhileLoopBuilder.ProcessInfiniteLoop(exceptionHandlerBlock.HandlerBlock);
                }
                else if (processingBlock is DoWhileBlock)
                {
                    DoWhileBlock doWhileBlock = (DoWhileBlock)processingBlock;

                    DoWhileLoopBuilder.ProcessInfiniteLoop(doWhileBlock.Loop);
                }
                else
                {
                    if (DoWhileLoopBuilder.Create(block, i, true) != null)
                    {
                        i -= preCount - block.Children.Count;
                    }
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
            int index,
            bool isInfiniteLoopLookup)
        {
            if (index < 1)
            {
                return null;
            }

            Block conditionalBlock;

            if (!isInfiniteLoopLookup)
            {
                conditionalBlock = parentBlock.Children[index];
                if (!ConditionalStatementBuilder.IsConditionalChild(conditionalBlock))
                {
                    return null;
                }
            }
            else
            {
                conditionalBlock = parentBlock.Children[index] as InstructionBlock;
                if (conditionalBlock == null
                    || conditionalBlock.GetInstructionAt(0).OpCode != OpCode.Branch)
                {
                    return null;
                }
            }

            Block branchedBlock =
                isInfiniteLoopLookup
                || conditionalBlock.LogicalSuccessors[0].BlockEndIndex <= conditionalBlock.LogicalSuccessors[1].BlockEndIndex
                    ? conditionalBlock.LogicalSuccessors[0]
                    : conditionalBlock.LogicalSuccessors[1];

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

            if (branchedBlock.Predecessors.Count > 1 ||
                branchedBlock.Predecessors.Count == 1 && branchedBlock.BlockStartIndex == 0)
            {
                if (conditionalBlock.Successors.Count == 1)
                {
                    return new WhileBlock(
                        blockChildren.Length > 1
                            ? new Block(blockChildren)
                            : blockChildren[0],
                        conditionalBlock);
                }
                else
                {
                    return new DoWhileBlock(
                        blockChildren.Length > 1
                            ? new Block(blockChildren)
                            : blockChildren[0],
                        conditionalBlock);
                }
            }

            if (indexOfBranch == 0)
            {
                throw new InvalidOperationException("While loop will always have branch instruction before it");
            }

            // We know that condition block will have predecessor that is a block before
            // indexOfBranch in while and for loop cases.
            //
            var predecessorsAndSuccessors = parentBlock.GetRangePredecessorAndSuccessor(
                indexOfBranch,
                index);

            if (!predecessorsAndSuccessors.HasValue)
            {
                throw new NotSupportedException("Gotos are not yet supported.");
            }

            if (indexOfBranch > 0
                && predecessorsAndSuccessors.Value.Key.Contains(
                    parentBlock.Children[indexOfBranch - 1])
                && parentBlock.Children[indexOfBranch-1].GetInstructionAt(0).OpCode == OpCode.Branch)
            {
                return new WhileBlock(
                    parentBlock.Children[indexOfBranch - 1],
                    blockChildren.Length > 1
                        ? new Block(blockChildren)
                        : blockChildren[0],
                    conditionalBlock);
            }

            return new WhileBlock(
                blockChildren.Length > 1
                    ? new Block(blockChildren)
                    : blockChildren[0],
                conditionalBlock);
        }
    }
}
