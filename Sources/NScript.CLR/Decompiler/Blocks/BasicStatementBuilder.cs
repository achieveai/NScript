using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.CLR.Decompiler.Blocks
{
    internal class BasicStatementBuilder
    {
        public static void Process(RootBlock rootBlock)
        {
            for (int i = 0; i < rootBlock.Children.Count; i++)
            {
                BasicStatementBuilder.ConvertToStatementBlock(
                    (BasicBlock) rootBlock.Children[i]);
            }
        }

        public static void ConvertToStatementBlock(
            BasicBlock basicBlock)
        {
            if (basicBlock is InstructionBlock)
            { return; }

            // In Stage1 there are only 2 type of BasicBlocks
            // 1. InstructionBlock
            // 2. BasicBlock itself
            // Anything else should be an error.
            //
            if (basicBlock.GetType() != typeof(BasicBlock))
            {
                throw new InvalidOperationException();
            }

            for (int iChild = 0; iChild < basicBlock.Children.Count; iChild++)
            {
                BasicStatementBuilder.ConvertToStatementBlock(
                    ((BasicBlock)basicBlock.Children[iChild]));
            }

            int firstDependent = basicBlock.Children.Count;
            do
            {
                firstDependent = BasicBlockBuilder.GetFirstDependentIndex(
                    basicBlock,
                    firstDependent - 1);
            }
            // Here we make sure that we go through the full dependent chain before finalizing the
            // firstDependent. One example with checking predecessorAndSuccessor becomes necessory
            // is for return foo(i) ? bar(i) : 10; kind of condition where return block is dependent
            // on if block.
            while (firstDependent > 0
                && !basicBlock.GetRangePredecessorAndSuccessor(
                    firstDependent,
                    basicBlock.Children.Count - 1).HasValue);

            if (firstDependent == 0)
            {
                // Since our last block depends on all the blocks.
                //
                BasicStatementBuilder.TransformToStacked(basicBlock);
                return;
            }

            BasicBlock[] childBlocks = new BasicBlock[basicBlock.Children.Count];

            for (int iChild = 0; iChild < childBlocks.Length; iChild++)
            {
                childBlocks[iChild] = (BasicBlock)basicBlock.Children[iChild];
            }


            // Here we also need to identify the block that is the result of this serial block.
            // This will then be used in determining the return value of closure function.
            //
            new SerialBlock(childBlocks);

            basicBlock.Dissolve();

            return;
        }

        private static void TransformToStacked(BasicBlock basicBlock)
        {
            BasicBlock[] dependents = new BasicBlock[basicBlock.Children.Count - 1];

            for (int iDependent = 0; iDependent < dependents.Length; iDependent++)
            {
                dependents[iDependent] = (BasicBlock)basicBlock.Children[iDependent];
            }

            InstructionBlock consumerInstruction = (InstructionBlock)basicBlock.Children[basicBlock.Children.Count - 1];

            basicBlock.Dissolve();

            new StackedBlock(
                consumerInstruction,
                dependents);
            return;
        }
    }
}
