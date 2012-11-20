namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NScript.Utils;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    class ForBlockBuilder
    {
        public static void Process(
            Block block)
        {
            for (int i = 0; i < block.Children.Count; i++)
            {
                ForBlockBuilder.Process(block.Children[i]);

                var whileBlock = block.Children[i] as WhileBlock;

                ForBlock forBlock = null;

                if (whileBlock != null &&
                    whileBlock.Loop.Children.Count > 0 &&
                    (forBlock = ForBlockBuilder.Create(block, i)) == null)
                {
                    // Create forLoop with help from pdb lines.
                    //
                    forBlock = ForBlockBuilder.CreateUsingPdbInfo(block, i);
                }

                if (forBlock != null)
                {
                    i = block.Children.IndexOf(forBlock);
                }
            }
        }

        private static ForBlock CreateUsingPdbInfo(
            Block parentBlock,
            int whileBlockIndex)
        {
            WhileBlock whileBlock = (WhileBlock) parentBlock.Children[whileBlockIndex];

            if (ForBlockBuilder.CompareCodeLocation(
                whileBlock.Condition.ComputeLocation(),
                whileBlock.Loop.ComputeLocation()) >= 0)
            {
                return null;
            }

            // Now here again we know that our condition was placed before our
            // loop in the code. In this case, we should have For loop to go through
            //

            int incrementBlockStartIndex = - 1;
            int firstValidChild = -1;

            Location firstBlockLocation = null;
            for (int i = 0; i < whileBlock.Loop.Children.Count && firstBlockLocation == null; i++)
            {
                firstValidChild = i;
                firstBlockLocation = whileBlock.Loop.ComputeLocation();
            }

            for (int i = whileBlock.Loop.Children.Count - 1; i >= firstValidChild; i--)
            {
                if (ForBlockBuilder.CompareCodeLocation(
                    firstBlockLocation,
                    whileBlock.Loop.Children[i].GetInstructionAt(0).Location) > 0)
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
                return null;
            }

            var leadingBlocksInfo = ForBlockBuilder.GetLeadingAssignments(
                parentBlock,
                whileBlockIndex);

            return ForBlockBuilder.CreateForBlock(
                parentBlock,
                leadingBlocksInfo == null ? whileBlockIndex : leadingBlocksInfo.Item1,
                whileBlockIndex,
                whileBlock,
                incrementBlockStartIndex);
        }

        private static ForBlock Create(
            Block parentBlock,
            int whileBlockIndex)
        {
            WhileBlock whileBlock = (WhileBlock)parentBlock.Children[whileBlockIndex];
            var breakBlock = whileBlock.Condition.Successors[0];

            var whileContinueBlock = whileBlock.Condition;

            List<Block> branches = new List<Block>();

            ForBlockBuilder.GetInternalBranches(
                branches,
                whileBlock,
                whileBlock.Loop);

            for (int i = branches.Count - 1; i >= 0; i--)
            {
                if (branches[i] == breakBlock)
                {
                    branches.RemoveAt(i);
                }
                else if (branches[i] == whileContinueBlock)
                {
                    // There is no way we can jump here using branch in for loop.
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

            // Let's try tracking variable usage. If all the statements before while block
            // are variable initialization and that variable's scope starts on this
            // initialization and ends before end of while block.
            Tuple<int, List<VariableDefinition>> leadingInitializationInfo = ForBlockBuilder.GetLeadingAssignments(
                parentBlock,
                whileBlockIndex);

            if (lastIndex > 0)
            {
                // Last step to check if this fits the bill for ForLoop.
                // All the blocks from lastIndex point till the end should
                // be basic stacked blocks.
                //
                bool skip = false;
                for (int i = lastIndex; i < whileBlock.Loop.Children.Count; i++)
                {
                    if (!(whileBlock.Loop.Children[i] is StackedBlock) ||
                        whileBlock.Loop.Children[i].Predecessors.Count > 1 ||
                        whileBlock.Loop.Children[i] is JumpBlock)
                    {
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    return ForBlockBuilder.CreateForBlock(
                        parentBlock,
                        leadingInitializationInfo != null ? leadingInitializationInfo.Item1 : whileBlockIndex,
                        whileBlockIndex,
                        whileBlock,
                        lastIndex);
                }
            }

            if (leadingInitializationInfo != null)
            {
                lastIndex = whileBlock.Loop.Children.Count - 1;

                while (lastIndex > 0)
                {
                    StackedBlock stackedBlock = whileBlock.Loop.Children[lastIndex] as StackedBlock;

                    if (stackedBlock != null)
                    {

                        InstructionBlock instructionBlock = stackedBlock.RootBlock;

                        // for most of the for loops, we have assignments in increment block.
                        // check if this one is assignment.
                        if (instructionBlock.Instruction.OpCode
                            != OpCode.StoreLocal)
                        {
                            break;
                        }

                        VariableDefinition variable = (VariableDefinition) instructionBlock.Instruction.OpCodeArgument;

                        // if the variable with above asignment is not one of the ones initialized
                        // in leading scope, we can break out.
                        if (!leadingInitializationInfo.Item2.Contains(variable))
                        {
                            break;
                        }
                    }
                    else
                    {
                        OpAssignmentBlock opAssignmentBlock = whileBlock.Loop.Children[lastIndex] as OpAssignmentBlock;

                        if (opAssignmentBlock == null)
                        {
                            break;
                        }

                        IlInstruction storeInstruction = opAssignmentBlock.GetInstructionAt(
                            opAssignmentBlock.InstructionCount - 1);

                        if (storeInstruction.OpCode != OpCode.StoreLocal
                            || !leadingInitializationInfo.Item2.Contains(
                                    (VariableDefinition)storeInstruction.OpCodeArgument))
                        {
                            break;
                        }
                    }

                    lastIndex--;
                }

                // Only when we have incremental blocks, is this block going to be forLoop.
                if (whileBlock.Loop.Children.Count > lastIndex + 1)
                {
                    return ForBlockBuilder.CreateForBlock(
                        parentBlock,
                        leadingInitializationInfo.Item1,
                        whileBlockIndex,
                        whileBlock,
                        lastIndex + 1);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the leading assignments.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="whileBlockIndex">Index of the while block.</param>
        /// <returns>Tuple with first leading assignemnt block and assigned variables.</returns>
        private static Tuple<int, List<VariableDefinition>> GetLeadingAssignments(
            Block parentBlock,
            int whileBlockIndex)
        {
            int inspectionIndex = whileBlockIndex - 1;
            List<VariableDefinition> scopeVariables = new List<VariableDefinition>();

            while (inspectionIndex >= 0)
            {
                StackedBlock stackedBlock = parentBlock.Children[inspectionIndex] as StackedBlock;

                if (stackedBlock == null)
                {
                    break;
                }

                InstructionBlock instructionBlock = stackedBlock.RootBlock;

                if (instructionBlock.Instruction.OpCode != OpCode.StoreLocal)
                {
                    break;
                }

                // Now that we know that we are on variable assignment, let's check if the
                // usage of this variable starts at this instruction and ends before
                // end of whileBlock.
                VariableDefinition variableIndex = (VariableDefinition)instructionBlock.Instruction.OpCodeArgument;
                var variableRange = parentBlock.Context.GetRangeContaining(variableIndex, instructionBlock.Instruction);

                if (!variableRange.Item1.Equals(instructionBlock.Instruction))
                {
                    break;
                }

                InstructionBlock endRangeBlock = parentBlock.Context.InstructionToBlock[variableRange.Item2];

                if (endRangeBlock.BlockEndIndex > parentBlock.Children[whileBlockIndex].BlockEndIndex)
                {
                    break;
                }

                scopeVariables.Add(variableIndex);
                inspectionIndex--;
            }

            if (inspectionIndex + 1 < whileBlockIndex)
            {
                return Tuple.Create(inspectionIndex + 1, scopeVariables);
            }

            return null;
        }

        private static ForBlock CreateForBlock(
            Block parentBlock,
            int leadingBlockIndex,
            int whileBlockIndex,
            WhileBlock whileBlock,
            int incrementalBlocksIndex)
        {
            bool hasFallinBlock = whileBlock.FallinBlock != null;
            var incommingBlock = whileBlock.FallinBlock;
            var conditionBlock = whileBlock.Condition;
            var loopBlock = whileBlock.Loop;
            int loopStartBlock = hasFallinBlock
                ? whileBlockIndex + 1
                : whileBlockIndex;

            whileBlock.Dissolve();
            loopBlock.Dissolve();

            // now let's fix incrementalBlockIndex to take into account dissolved whileBlock
            // It's possible that we have no loop block and everything is incrementBlock.
            if (incrementalBlocksIndex > 0)
            {
                var loopBlockChildren = parentBlock.CreateChildrenArray<Block>(
                    loopStartBlock,
                    incrementalBlocksIndex + loopStartBlock);

                loopBlock = new Block(loopBlockChildren);
            }
            else
            {
                loopBlock = null;
            }

            int conditionIndex = parentBlock.Children.IndexOf(conditionBlock);

            Block incrementalBlock = null;

            // Just like above it may also be possible that there is no incrementalBlock
            // for us to have incremental block, there should be atleast one loopBlock.
            if (loopStartBlock + 1 < conditionIndex)
            {
                var incrementalBlockChildren = parentBlock.CreateChildrenArray<Block>(
                    loopStartBlock + 1,
                    conditionIndex);

                incrementalBlock = incrementalBlockChildren.Length > 1
                        ? new Block(incrementalBlockChildren)
                        : incrementalBlockChildren[0];
            }

            if (leadingBlockIndex < whileBlockIndex)
            {
                var leadingBlockChildren = parentBlock.CreateChildrenArray<Block>(
                    leadingBlockIndex,
                    loopStartBlock);

                incommingBlock = leadingBlockChildren.Length > 1
                    ? new Block(leadingBlockChildren)
                    : leadingBlockChildren[0];
            }

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
                JumpBlock jumpBlock = (JumpBlock) block;
                if (jumpBlock.JumpType == JumpType.Continue)
                {
                    branches.AddRange(jumpBlock.Successors);
                }

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
                if ((basicBlock is StackedBlock && lastInstruction.FlowType == FlowType.ConditionalBranch) ||
                    (basicBlock is InstructionBlock && lastInstruction.FlowType == FlowType.Branch))
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
                ForBlockBuilder.GetInternalBranches(
                    branches,
                    topBlock,
                    block.Children[i]);
            }
        }

        public static int CompareCodeLocation(
            Location block1,
            Location block2)
        {
            if (block1 == null)
            {
                if (block2 != null)
                {
                    return 1;
                }

                return 0;
            }

            if (block2 == null)
            {
                return -1;
            }

            int returnValue = block1.StartLine.CompareTo(
                block2.StartLine);

            if (returnValue == 0)
            {
                returnValue = block1.StartColumn.CompareTo(
                    block2.StartColumn);
            }

            return returnValue;
        }
    }
}
