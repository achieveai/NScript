//-----------------------------------------------------------------------
// <copyright file="OpAssignmentBlockBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System.Collections.Generic;

    /// <summary>
    /// Definition for OpAssignmentBlockBuilder
    /// </summary>
    internal static class OpAssignmentBlockBuilder
    {
        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void Process(Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                OpAssignmentBlockBuilder.Process(block.Children[childIndex]);
            }

            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                Block opAssignment = OpAssignmentBlockBuilder.CreateOpAssignmentBlock(block, childIndex);
                if (opAssignment == null)
                {
                    OpAssignmentBlockBuilder.CreateInlineOpAssignmentBlock(block, childIndex);
                }
            }
        }

        /// <summary>
        /// Creates the op assignment block.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="index">The index.</param>
        private static Block CreateOpAssignmentBlock(
            Block parentBlock,
            int index)
        {
            StackedBlock childBlock = parentBlock.Children[index] as StackedBlock;

            if (childBlock == null
                || PostfixBlockBuilder.GetStoreOperationDependencies(childBlock.RootBlock) < 0)
            {
                return null;
            }

            StackedBlock opBlock = childBlock.GetDependent(childBlock.DependentCount() - 1) as StackedBlock;
            if (opBlock == null)
            {
                return null;
            }

            if (!OpAssignmentBlock.GetOperator(opBlock.RootBlock.Instruction.OpCode).HasValue)
            {
                return null;
            }

            List<Block> arguments =
                PostfixBlockBuilder.AreReciprocatingOperations(
                    opBlock.Children[0],
                    childBlock);

            if (arguments == null)
            {
                return null;
            }

            return new OpAssignmentBlock(
                childBlock,
                opBlock,
                childBlock);
        }

        /// <summary>
        /// Creates the inline op assignment block.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="index">The index.</param>
        /// <returns>OpAssignmentBlock if constructed</returns>
        private static Block CreateInlineOpAssignmentBlock(
            Block parentBlock,
            int index)
        {
            InlineAssignmentBlock childBlock = parentBlock.Children[index] as InlineAssignmentBlock;
            if (childBlock == null)
            {
                return null;
            }

            StackedBlock opBlock = childBlock.Value as StackedBlock;
            if (opBlock == null)
            {
                return null;
            }

            if (!OpAssignmentBlock.GetOperator(opBlock.RootBlock.Instruction.OpCode).HasValue)
            {
                return null;
            }

            List<Block> arguments =
                PostfixBlockBuilder.AreReciprocatingOperations(
                    opBlock.Children[0],
                    childBlock.Setter);

            if (arguments == null)
            {
                return null;
            }

            return new OpAssignmentBlock(
                childBlock.Setter,
                opBlock,
                childBlock);
        }
    }
}
