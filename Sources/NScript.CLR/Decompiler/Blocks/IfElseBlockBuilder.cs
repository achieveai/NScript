namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class IfElseBlockBuilder
    {
        public static void Process(
            Block block)
        {
            if (block is SwitchBlock)
            {
                foreach (var caseBlock in ((SwitchBlock)block).CaseBlocks)
                {
                    IfElseBlockBuilder.Process(caseBlock.Value);
                }
            }
            else if (block is DoWhileBlock)
            {
                IfElseBlockBuilder.Process(((DoWhileBlock) block).Condition);
                IfElseBlockBuilder.Process(((DoWhileBlock) block).Loop);
            }
            else if (block is ForBlock)
            {
                IfElseBlockBuilder.Process(((ForBlock) block).ConditionBlock);
                IfElseBlockBuilder.Process(((ForBlock) block).LoopBlock);
            }
            else if (block is IfElseBlock)
            {
                IfElseBlockBuilder.Process(((IfElseBlock) block).Condition);

                for (int i = 0; i < block.Children.Count; i++)
                {
                    IfElseBlockBuilder.Process(block.Children[i]);
                }
            }
            else
            {
                for (int i = 0; i < block.Children.Count; i++)
                {
                    IfElseBlockBuilder.Create(block, i);
                    IfElseBlockBuilder.Process(block.Children[i]);
                }
            }
        }

        /// <summary>
        /// Res the process.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void ReProcess(
            Block block)
        {
            IfElseBlockBuilder.DissolveAllIfElse(block);
            IfElseBlockBuilder.Process(block);
        }

        /// <summary>
        /// Dissolves all if else.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void DissolveAllIfElse(
            Block block)
        {
            if (block.StackAfter > 0)
            {
                return;
            }

            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                if (block.Children[childIndex].StackAfter != 0)
                {
                    continue;
                }

                IfElseBlockBuilder.DissolveAllIfElse(block.Children[childIndex]);
            }

            IfElseBlock ifElseBlock = block as IfElseBlock;
            if (ifElseBlock != null)
            {
                if (ifElseBlock.ElseBlock != null
                    && ifElseBlock.ElseBlock.GetType() == typeof(Block))
                {
                    ifElseBlock.ElseBlock.Dissolve();
                }
                if (ifElseBlock.IfBlock.GetType() == typeof(Block))
                {
                    ifElseBlock.IfBlock.Dissolve();
                }

                ifElseBlock.Dissolve();
            }
        }

        /// <summary>
        /// If statement will provide two next locations.
        /// 1. If block
        /// 2. Else block, or Continuation point if there is no else block.
        ///
        /// To check if second block is continuation block, we need to check if
        /// it is If block's successor. Now there is yet another set of problems.
        /// 
        /// We can exit these blocks by
        /// 1. natural flow, This is most common.
        /// 2. Return statement, next common.
        /// 3. break and continue statement.
        ///
        /// Natural flow is pretty simple to follow, it will lead to continuation block.
        /// Return will have no successors.
        /// Break will have block next to parent loop block.
        /// Continue will either be start of conditional block (in while case) or some n blocks before
        /// end which are increment blocks in "for" block.
        ///
        /// </summary>
        /// <param name="parentBlock"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static IfElseBlock Create(
            Block parentBlock,
            int index)
        {
            // condition of the if block will have at least 1 blocks behind it.
            // also stack after the condition will be 0
            //
            if (index >= parentBlock.Children.Count - 1 ||
                !ConditionalStatementBuilder.IsConditionalChild(parentBlock.Children[index]))
            {
                return null;
            }

            Block conditionalBlock = parentBlock.Children[index];
            Block nextBlock = parentBlock.Children[index + 1];
            Block branchedBlock = conditionalBlock.Successors[0] != nextBlock
                    ? conditionalBlock.Successors[0]
                    : conditionalBlock.Successors[1];

            // Whatever happens, but in if block we can't branch backwards, that only happens
            // in loops.
            //
            if (branchedBlock.BlockStartIndex < conditionalBlock.BlockStartIndex)
            {
                return null;
            }

            int indexOfBranch = parentBlock.Children.IndexOf(branchedBlock);

            if (indexOfBranch == -1)
            {
                // This is the case when if statement is at the end of parent block.
                // In this case branchedBlock should be next to one of the parent block and remaining
                // parent blocks should be last blocks.
                //
                var lastParentBlock = parentBlock;
                var tmpParentBlock = parentBlock.Parent;

                while(tmpParentBlock != null)
                {
                    if (lastParentBlock != tmpParentBlock.Children[tmpParentBlock.Children.Count-1])
                    {
                        int indexOfParent = tmpParentBlock.Children.IndexOf(lastParentBlock);

                        if (tmpParentBlock.Children[indexOfParent + 1] != branchedBlock)
                        {
                            return null;
                        }

                        break;
                    }

                    lastParentBlock = tmpParentBlock;
                    tmpParentBlock = lastParentBlock.Parent;
                }

                var blockChildren = parentBlock.CreateChildrenArray<Block>(
                    index + 1,
                    parentBlock.Children.Count);

                return new IfElseBlock(
                    conditionalBlock,
                    blockChildren.Length > 1
                        ? new Block(blockChildren)
                        : blockChildren[0]);
            }

            // Now that we have both nextBlock and branchedBlock in current parent,
            // let's check all the succors of all the blocks from nextBlock till branchBlock.
            //
            var ifBlockNeighbors = parentBlock.GetRangePredecessorAndSuccessor(
                index + 1,
                indexOfBranch - 1,
                (blk) => !(blk is JumpBlock));

            if (!ifBlockNeighbors.HasValue)
            {
                throw new NotSupportedException("Gotos are not yet supported");
            }

            var successors = ifBlockNeighbors.Value.Value;

            // Now we will look at the first parent index that is contained in successors.
            // If there is no sibling index or only sibling index is nextBlockIndex, then
            // what we have is ifBlock
            //
            int minSiblingIndex = parentBlock.Children.Count;

            for (int i = 0; i < successors.Count; i++)
            {
                int siblingIndex = parentBlock.Children.IndexOf(successors[i]);

                if (siblingIndex > index + 1)
                {
                    minSiblingIndex = Math.Min(minSiblingIndex, siblingIndex);
                }
            }

            // return cond ? x : y, ends up resolving into
            // if (cond) return x; return y;
            // For this case (and this case only do we check if if successors.Count == 0
            // is our next statement is also return statement or not. If so create proper
            // if statement.
            if (minSiblingIndex == indexOfBranch
                || (successors.Count == 0
                    && (!(parentBlock.Children[indexOfBranch] is StackedBlock)
                        || indexOfBranch - index != 2
                        || ((StackedBlock)parentBlock.Children[indexOfBranch]).RootBlock.Instruction.OpCode != OpCode.Return)))
            {
                // There is only one way this is possible and that is if we
                // have only if statement and no else block.
                //
                var blockChildren = parentBlock.CreateChildrenArray<Block>(
                    index + 1,
                    indexOfBranch);

                return new IfElseBlock(
                    conditionalBlock,
                    blockChildren.Length > 1
                        ? new Block(blockChildren)
                        : blockChildren[0]);
            }

            // This is IfElse block. We will need to create 2 set of blocks.
            //
            var ifBlockChildren = parentBlock.CreateChildrenArray<Block>(
                index + 1,
                indexOfBranch);

            var elseBlockChildren = parentBlock.CreateChildrenArray<Block>(
                indexOfBranch,
                minSiblingIndex);

            return new IfElseBlock(
                conditionalBlock,
                ifBlockChildren.Length > 1
                    ? new Block(ifBlockChildren)
                    : ifBlockChildren[0],
                elseBlockChildren.Length > 1
                    ? new Block(elseBlockChildren)
                    : elseBlockChildren[0]);
        }
    }
}
