namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    class JumpBlockBuilder
    {
        public static void ProcessBreaks(Block block)
        {
            JumpBlockBuilder.Process(block, true);
        }

        public static void ProcessContinues(Block block)
        {
            JumpBlockBuilder.Process(block, false);
        }

        private static void Process(
            Block block,
            bool processBreaks)
        {
            if (block is JumpBlock)
            {
                return;
            }

            for (int i = 0; i < block.Children.Count; i++)
            {
                if (!(block is BasicBlock))
                {
                    JumpBlockBuilder.Process(block.Children[i], processBreaks);
                }
            }

            BasicBlock basicBlock = block as BasicBlock;

            if (basicBlock == null)
            {
                return;
            }

            var lastInstruction = basicBlock.GetInstructionAt(
                basicBlock.BlockEndIndex - basicBlock.BlockStartIndex);

            if ((basicBlock is StackedBlock && lastInstruction.CodeOp.Flow == Ops.FlowType.ConditionalBranch) ||
                (basicBlock is InstructionBlock && lastInstruction.CodeOp.Flow == Ops.FlowType.Branch))
            {
                JumpBlockBuilder.Create(basicBlock, processBreaks);
            }
        }

        private static void Create(
            BasicBlock instructionBlock,
            bool processBreaks)
        {
            if (JumpBlockBuilder.CheckIfElseContinuation(instructionBlock))
            {
                return;
            }

            var parentBlock = instructionBlock.Parent;

            while(parentBlock != null)
            {
                bool checkForBreak = false;
                if (parentBlock is ForBlock)
                {
                    ForBlock forBlock = parentBlock as ForBlock;
                    if (!processBreaks &&
                        instructionBlock.Successors[instructionBlock.Successors.Count-1] == forBlock.IncrementBlock)
                    {
                        new JumpBlock(instructionBlock, JumpType.Continue);
                        return;
                    }

                    checkForBreak = true;
                }
                else if (parentBlock is DoWhileBlock)
                {
                    DoWhileBlock whileBlock = parentBlock as DoWhileBlock;
                    if (!processBreaks &&
                        instructionBlock.Successors[instructionBlock.Successors.Count - 1] == whileBlock.Condition)
                    {
                        new JumpBlock(instructionBlock, JumpType.Continue);
                        return;
                    }

                    checkForBreak = true;
                }
                else if (parentBlock is SwitchBlock)
                {
                    checkForBreak = true;
                }

                if (checkForBreak && processBreaks)
                {
                    // For break, the successor of instruction should be next instruction to this
                    // parent.
                    //
                    if (parentBlock.BlockEndIndex == instructionBlock.Successors[instructionBlock.Successors.Count-1].BlockStartIndex -1)
                    {
                        new JumpBlock(instructionBlock, JumpType.Break);
                        return;
                    }
                }

                if (!processBreaks &&
                    parentBlock != instructionBlock.Parent &&
                    parentBlock.Children.Contains(instructionBlock.Successors[instructionBlock.Successors.Count - 1]))
                {
                    new JumpBlock(instructionBlock, JumpType.Goto);
                    return;
                }

                parentBlock = parentBlock.Parent;
            }

            return;
        }

        internal static bool CheckIfElseContinuation(BasicBlock block)
        {
            var parentIfBlock = JumpBlockBuilder.GetParentIfBlock(block);

            if (parentIfBlock == null)
            {
                return false;
            }

            if (block == parentIfBlock.Children[0])
            {
                return true;
            }

            // this break will always be last break in the block segment.
            //
            if (block.Parent.Children.IndexOf(block) < block.Parent.Children.Count-1)
            {
                return false;
            }

            // Now let's loop through all the continuous parent if blocks and check if block points to 
            // next block to one of the if blocks.
            //
            while (parentIfBlock != null)
            {
                if (block.Successors[block.Successors.Count-1].BlockStartIndex == parentIfBlock.BlockEndIndex + 1)
                {
                    return true;
                }

                if (parentIfBlock.Parent.Children.IndexOf(parentIfBlock) == parentIfBlock.Parent.Children.Count - 1)
                {
                    // In this case the if block is the last block in flow. Now if parent is also ifBlock let's
                    // keep looping.
                    //
                    parentIfBlock = JumpBlockBuilder.GetParentIfBlock(parentIfBlock);
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private static IfElseBlock GetParentIfBlock(
            Block block)
        {
            IfElseBlock returnValu = block.Parent as IfElseBlock;

            if (returnValu != null)
            {
                return returnValu;
            }

            if (block.Parent != null)
            {
                return block.Parent.Parent as IfElseBlock;
            }

            return null;
        }
    }
}
