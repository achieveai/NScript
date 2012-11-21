using System;
using System.Collections.Generic;
using NScript.Lib.AsmDeasm.Ops;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    internal class BasicBlockBuilder
    {
        public static void Process(
            RootBlock block)
        {
            BasicBlockBuilder.BuildBasicBlocks(
                block);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// To find all the dependencies, here we need to iterate through all the blocks where
        /// the stackAfter - pushes > currentBlock's stackBefore - pops.
        /// </remarks>
        /// <param name="block"></param>
        /// <param name="childIndex"></param>
        /// <returns></returns>
        public static int GetFirstDependentIndex(
            Block block,
            int childIndex)
        {
            int dependentStackSize = block.Children[childIndex].GetInstructionAt(0).GetPops();

            if (dependentStackSize > 0)
            {
                return BasicBlockBuilder.GetFirstCleanBlockAtStackLevel(
                    block,
                    block.Children[childIndex].StackBefore - dependentStackSize,
                    childIndex);
            }

            return childIndex;
        }

        private static void BuildBasicBlocks(
            Block block,
            BasicBlock consumerBlock = null)
        {
            if (block is InstructionBlock ||
                !(block.Children[0] is InstructionBlock))
            {
                // This block has already been processed, let's skip this block.
                //
                return;
            }


            // We use containsConsumerBlock to keep track of the type of blocks that we may
            // have as children of this block.
            //
            // 1. If the last block is consumer block, this means that all last block depends on
            //    all previous blocks as component of it's stack. In this case we only try to group
            //    all but the last block.
            // 2. If the last block is not the consumer block, this means that we are building one
            //    or more of the stack levels for consumerBlock. In this case we start grouping elemnts
            //    such that each stack level has first child as the block that builds that stack level
            //    and second child that builds next set of stack levels.
            //
            bool containsConsumerBlock = block.Children[block.Children.Count - 1] == consumerBlock;

            List<int> boundaries = new List<int>();
            List<bool> isMergedBlock = new List<bool>();

            BasicBlockBuilder.CreateBlockBoundaries(
                block,
                containsConsumerBlock,
                boundaries,
                isMergedBlock);

            // Now let's create blocks that follow block rules.
            //
            BasicBlockBuilder.MergeCorssReferencingBlocks(
                block,
                boundaries,
                isMergedBlock);

            // Now let's go over the boundaries and again merge them according to stack levels
            //
            BasicBlockBuilder.MergeIntoSameLevelBlocks(
                block,
                containsConsumerBlock,
                boundaries,
                isMergedBlock);

            // Now let's merge the blocks again to remove cross block references.
            //
            BasicBlockBuilder.MergeCorssReferencingBlocks(
                block,
                boundaries,
                isMergedBlock);

            if (boundaries.Count > 2 ||
                block is RootBlock)
            {
                for (int i = boundaries.Count - 2; i >= 0; i--)
                {
                    if (boundaries[i + 1] - boundaries[i] <= 1)
                    {
                        continue;
                    }

                    new BasicBlock(block.CreateChildrenArray<BasicBlock>(boundaries[i], boundaries[i + 1]));
                }
            }

            for (int iChild = 0; iChild < block.Children.Count; iChild++)
            {
                if (!(block.Children[iChild] is InstructionBlock))
                {
                    BasicBlockBuilder.BuildBasicBlocks(
                        block.Children[iChild],
                        isMergedBlock[iChild] ?
                            consumerBlock :
                            (BasicBlock)block.Children[iChild].Children[block.Children[iChild].Children.Count - 1]);
                }
            }
        }

        private static void MergeIntoSameLevelBlocks(
            Block block,
            bool containsConsumerBlock,
            IList<int> boundaries,
            IList<bool> isMergedBlock)
        {
            int lastStackBuilderBlock = containsConsumerBlock ? boundaries.Count - 2 : boundaries.Count - 1;
            int lastUnmergableBoundary = lastStackBuilderBlock;

            // We only have to merge into stack levels if there are more than one stack levels in this block.
            // Let's check min and max stack levels in current block.
            //
            int minStackLevel = int.MaxValue;
            int maxStackLevel = int.MinValue;
            for (int iBoundary = 1; iBoundary < boundaries.Count; iBoundary++)
            {
                Instruction instruction = block.Children[boundaries[iBoundary] - 1].GetInstructionAt(0);

                if (instruction.GetPushes() > 0)
                {
                    minStackLevel = Math.Min(minStackLevel, instruction.StackAfter);
                    maxStackLevel = Math.Max(maxStackLevel, instruction.StackAfter);
                }
            }

            if (maxStackLevel == minStackLevel && !containsConsumerBlock)
            {
                return;
            }

            for (int iBoundary = lastStackBuilderBlock - 1; iBoundary >= 1; iBoundary--)
            {
                // Check if this block really puts anything on the stackLevel.
                //
                Instruction instruction = block.Children[boundaries[iBoundary] - 1].GetInstructionAt(0);

                // Mostly just checking last instruction for stackBuilding activity would do the work, but
                // in case of dup in the block chain, this does not work. For this purpose, we take a look at
                // stackBefore of first block in range and stackAfter of last block in range.
                //
                bool isStackBuilder = 
                    block.Children[boundaries[iBoundary - 1]].GetInstructionAt(0).StackBefore <
                    instruction.StackAfter;

                // We only create a stack boundary if
                // 1. We are building a stack.
                // 2. if Following blocks has no place to go into (i.e. they are last block on the stack that are not consumed by following
                //    consumer block, then we keep them in this stack level.
                //
                if (isStackBuilder &&
                    (!containsConsumerBlock ||
                     (lastStackBuilderBlock < lastUnmergableBoundary ||
                      instruction.StackAfter < block.Children[boundaries[lastStackBuilderBlock]].StackBefore) ||
                      (lastStackBuilderBlock == lastUnmergableBoundary &&
                      instruction.StackAfter > block.Children[boundaries[lastStackBuilderBlock] - 1].StackBefore)))
                {
                    // Let's remove all the block boundaries 
                    for (int j = Math.Min(lastStackBuilderBlock, lastUnmergableBoundary - 1); j > iBoundary; j--)
                    {
                        boundaries.RemoveAt(iBoundary + 1);
                        isMergedBlock.RemoveAt(iBoundary + 1);
                        isMergedBlock[iBoundary] = block.Parent != null;
                    }

                    // Now we try to merge same stack level blocks into a single block.
                    //
                    if (iBoundary < boundaries.Count - 3 &&
                        block.Children[boundaries[iBoundary + 2] - 1].StackAfter == instruction.StackAfter)
                    {
                        boundaries.RemoveAt(iBoundary + 1);
                        isMergedBlock.RemoveAt(iBoundary + 1);
                        isMergedBlock[iBoundary] = block.Parent != null;
                    }

                    lastStackBuilderBlock = iBoundary - 1;
                }
            }

            if (lastStackBuilderBlock > 0 && lastStackBuilderBlock > lastUnmergableBoundary)
            {
                // Let's remove all the block boundaries 
                for (int j = Math.Min(lastStackBuilderBlock, lastUnmergableBoundary - 1); j >= 1; j--)
                {
                    boundaries.RemoveAt(1);
                    isMergedBlock.RemoveAt(1);
                    isMergedBlock[0] = block.Parent != null;
                }

                // Now we try to merge same stack level blocks into a single block.
                //
                if (1 < boundaries.Count - 3 &&
                    block.Children[boundaries[1 + 2] - 1].StackAfter == block.Children[boundaries[0] - 1].StackAfter)
                {
                    boundaries.RemoveAt(2);
                    isMergedBlock.RemoveAt(2);
                    isMergedBlock[2] = block.Parent != null;
                }
            }
        }

        /// <summary>
        /// Creates the block boundaries using scopes from the flow graph.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="containsConsumerBlock"></param>
        /// <param name="boundaries"></param>
        /// <param name="isMergedBlock"></param>
        private static void CreateBlockBoundaries(
            Block block,
            bool containsConsumerBlock,
            IList<int> boundaries,
            IList<bool> isMergedBlock)
        {
            boundaries.Add(block.Children.Count);

            int startGroupIndex = containsConsumerBlock ?
                block.Children.Count - 1 : block.Children.Count;

            if (containsConsumerBlock)
            {
                boundaries.Insert(0, startGroupIndex);
                isMergedBlock.Insert(0, false);
            }

            // Get tentative block boundaries.
            //
            for (int iChild = startGroupIndex - 1; iChild >= 0; iChild--)
            {
                iChild = BasicBlockBuilder.GetFirstDependentIndex(
                    block,
                    startGroupIndex - 1);
                startGroupIndex = iChild;

                boundaries.Insert(0, iChild);
                isMergedBlock.Insert(0, false);
            }
        }

        private static void MergeCorssReferencingBlocks(
            Block block,
            IList<int> boundaries,
            IList<bool> isMergedBlock)
        {
            for (int iChildBlock = boundaries.Count - 2; iChildBlock >= 0; iChildBlock--)
            {
                if (boundaries[iChildBlock + 1] - boundaries[iChildBlock] > 1)
                {
                    for (int i = iChildBlock; i >= 0; i--)
                    {
                        if (block.GetRangePredecessorAndSuccessor(
                            boundaries[i],
                            boundaries[i + 1] - 1).HasValue)
                        {
                            break;
                        }

                        boundaries.RemoveAt(i);
                        isMergedBlock.RemoveAt(i);

                        // we have deleted the block boundary, so the previous block is going
                        // to be merged block. children of rootBlock are never marked as merged.
                        //
                        isMergedBlock[i - 1] = block.Parent != null;
                        iChildBlock = i;
                    }
                }
            }
        }

        private static int GetFirstCleanBlockAtStackLevel(
            Block block,
            int stackLevel,
            int currentChildIndex)
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> sitesVisited = new HashSet<int>();

            int returnValue = int.MaxValue;
            bool firstLoop = true;
            queue.Enqueue(currentChildIndex);

            while (queue.Count > 0)
            {
                int blockToAnalyzeIndex = queue.Dequeue();
                var blockToAnalyze = block.Children[blockToAnalyzeIndex];

                int stackUnTouchedTill =
                    blockToAnalyze.StackAfter -
                    blockToAnalyze.GetInstructionAt(
                        blockToAnalyze.BlockEndIndex - blockToAnalyze.BlockStartIndex).GetPushes();

                // Since we can't go beyond first block, if we hit first block assume it is
                // the first block at given stackLevel.
                //
                // In case of Dup, our stackBefore may be one less than stackLevel.
                //
                if ((stackUnTouchedTill <= stackLevel &&
                    !firstLoop &&
                    blockToAnalyze.StackBefore <= stackLevel &&
                    blockToAnalyze.GetInstructionAt(0).GetPops() == 0) ||
                    blockToAnalyzeIndex == 0)
                {
                    returnValue = Math.Min(returnValue, blockToAnalyzeIndex);
                }
                else
                {
                    foreach (var predecessor in blockToAnalyze.Predecessors)
                    {
                        int predecessorIndex = block.Children.IndexOf(predecessor);

                        if (predecessorIndex < 0)
                        {
                            if (predecessor.BlockStartIndex < block.BlockStartIndex ||
                                predecessor.BlockStartIndex > block.BlockEndIndex)
                            {
                                throw new InvalidOperationException();
                            }

                            // Here we are moving out of the block, so lets assume that first block is
                            // start of the stackLevel.
                            //
                            predecessorIndex = 0;
                            queue.Enqueue(predecessorIndex);
                        }

                        if (!sitesVisited.Contains(predecessorIndex))
                        {
                            sitesVisited.Add(predecessorIndex);
                            queue.Enqueue(predecessorIndex);
                        }
                    }
                }

                firstLoop = false;
            }

            return returnValue;
        }

    }
}
