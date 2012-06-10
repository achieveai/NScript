using System;
using System.Collections.Generic;
using Cs2JsC.Lib.AsmDeasm.Ops;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    internal class ForLoopBuilder
    {
        public static void Process(
            Block block)
        {
            for (int i = 0; i < block.Children.Count; i++)
            {
                ForLoopBuilder.Process(block.Children[i]);

                var whileBlock = block.Children[i] as WhileBlock;

                if (whileBlock != null &&
                    ForLoopBuilder.Create(whileBlock) == null)
                {
                    // Create forLoop with help from pdb lines.
                    //
                    ForLoopBuilder.CreateUsingPdbInfo(whileBlock);
                }
            }
        }

        private static void CreateUsingPdbInfo(WhileBlock whileBlock)
        {
            if (ForLoopBuilder.CompareCodeLocation(
                whileBlock.Condition.GetInstructionAt(0),
                whileBlock.Loop.GetInstructionAt(0)) >= 0)
            {
                return;
            }

            // Now here again we know that our condition was placed before our
            // loop in the code. In this case, we should have For loop to go through
            //

            int incrementBlockStartIndex = - 1;

            for (int i = whileBlock.Loop.Children.Count - 1; i >= 1; i--)
            {
                if (ForLoopBuilder.CompareCodeLocation(
                    whileBlock.Loop.GetInstructionAt(0),
                    whileBlock.Loop.Children[i].GetInstructionAt(0)) > 0)
                {
                    incrementBlockStartIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (incrementBlockStartIndex <= 0)
            {
                return;
            }

            ForLoopBuilder.CreateForBlock(
                whileBlock,
                incrementBlockStartIndex);
            return;
        }

        private static ForBlock Create(
            WhileBlock whileBlock)
        {
            var breakBlock = whileBlock.Condition.Successors[0];

            var whileContinueBlock = whileBlock.Condition;

            List<Block> branches = new List<Block>();

            ForLoopBuilder.GetInternalBranches(
                branches,
                whileBlock,
                whileBlock.Loop);

            for (int i = branches.Count - 1; i >= 0; i--)
            {
                if (branches[i] == breakBlock)
                {
                    branches.RemoveAt(i);
                }

                if (branches[i] == whileContinueBlock)
                {
                    // There is no way we can jump here using branch in for loop.
                    //
                    return null;
                }
            }

            int lastIndex = -1;

            for (int i = 0; i < branches.Count; i++)
            {
                lastIndex = Math.Max(
                    lastIndex,
                    whileBlock.Loop.Children.IndexOf(branches[i]));
            }

            if (lastIndex <= 0)
            { return null; }

            // Last step to check if this fits the bill for ForLoop.
            // All the blocks from lastIndex point till the end should
            // be basic stacked blocks.
            //
            for (int i = lastIndex + 1; i < whileBlock.Loop.Children.Count; i++)
            {
                if (!(whileBlock.Loop.Children[i] is StackedBlock) ||
                    whileBlock.Loop.Children[i].Predecessors.Count > 1)
                {
                    return null;
                }
            }

            return ForLoopBuilder.CreateForBlock(whileBlock, lastIndex);
        }

        private static ForBlock CreateForBlock(
            WhileBlock whileBlock,
            int incrementalBlocksIndex)
        {
            int whileBlockIndex = whileBlock.Parent.Children.IndexOf(whileBlock);

            var incommingBlock = whileBlock.FallinBlock;
            var conditionBlock = whileBlock.Condition;
            var loopBlock = whileBlock.Loop;
            var parentBlock = whileBlock.Parent;

            whileBlock.Dissolve();
            loopBlock.Dissolve();

            // now let's fix incrementalBlockIndex to take into account dissolved whileBlock
            //
            var loopBlockChildren = parentBlock.CreateChildrenArray<Block>(
                whileBlockIndex + 1,
                incrementalBlocksIndex + whileBlockIndex + 1);

            loopBlock = loopBlockChildren.Length > 1
                    ? new Block(loopBlockChildren)
                    : loopBlockChildren[0];

            int conditionIndex = parentBlock.Children.IndexOf(conditionBlock);

            var incrementalBlockChildren = parentBlock.CreateChildrenArray<Block>(
                whileBlockIndex + 2,
                conditionIndex);

            var incrementalBlock = incrementalBlockChildren.Length > 1
                    ? new Block(incrementalBlockChildren)
                    : incrementalBlockChildren[0];

            return new ForBlock(
                incommingBlock,
                loopBlock,
                incrementalBlock,
                conditionBlock);
        }

        public static void GetInternalBranches(
            List<Block> branches,
            WhileBlock topBlock,
            Block block)
        {
            // While block acts as boundaries for break and continue statement
            //
            if (block is DoWhileBlock)
            {
                return;
            }

            // Same is true with switch block
            //
            if (block is SwitchBlock)
            {
                return;
            }

            if (block is JumpBlock)
            {
                return;
            }

            // Can't find any breaks or continues in basic block
            //
            if (block is BasicBlock)
            {
                BasicBlock basicBlock = block as BasicBlock;

                var lastInstruction = basicBlock.GetInstructionAt(
                    basicBlock.BlockEndIndex - basicBlock.BlockStartIndex);

                // Only conditional branch (normally if(...) break/continue; or normal
                // branches can be considered. condtionalBranch are stackBlocks and normal
                // branches are InstructionBlock
                //
                if ((basicBlock is StackedBlock && lastInstruction.CodeOp.Flow == Ops.FlowType.ConditionalBranch) ||
                    (basicBlock is InstructionBlock && lastInstruction.CodeOp.Flow == Ops.FlowType.Branch))
                {
                    // Ignore if this block belongs to ifElse blocks flow.
                    //
                    if (!JumpBlockBuilder.CheckIfElseContinuation(basicBlock))
                    {
                        var instructionBlock = basicBlock as InstructionBlock;
                        Block successor;

                        if (instructionBlock == null)
                        {
                            instructionBlock = (InstructionBlock)basicBlock.Children[block.Children.Count - 1];
                            successor = instructionBlock.Successors[1];
                        }
                        else
                        {
                            successor = instructionBlock.Successors[0];
                        }

                        // Check if successor falls in one of the point of interests.
                        //
                        if (topBlock.Loop.Children.Contains(successor) ||
                            topBlock.Condition == successor ||
                            topBlock.Condition.Successors[0] == successor)
                        {
                            branches.Add(successor);
                        }
                    }
                }

                return;
            }

            for (int i = 0; i < block.Children.Count; i++)
            {
                ForLoopBuilder.GetInternalBranches(
                    branches,
                    topBlock,
                    block.Children[i]);
            }
        }

        public static int CompareCodeLocation(
            Instruction block1,
            Instruction block2)
        {
            if (block1.SourceCode == null)
            {
                if (block2.SourceCode != null)
                {
                    return 1;
                }

                return 0;
            }

            if (block2.SourceCode == null)
            {
                return -1;
            }

            int returnValue = block1.SourceCode.StartPosition.Key.CompareTo(
                block2.SourceCode.StartPosition.Key);

            if (returnValue == 0)
            {
                returnValue = block1.SourceCode.StartPosition.Value.CompareTo(
                    block2.SourceCode.StartPosition.Value);
            }

            return returnValue;
        }
    }
}
