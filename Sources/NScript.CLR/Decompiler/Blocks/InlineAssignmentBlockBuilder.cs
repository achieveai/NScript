//-----------------------------------------------------------------------
// <copyright file="InlineAssignmentBlockBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Mono.Cecil.Cil;

namespace NScript.CLR.Decompiler.Blocks
{
    /// <summary>
    /// Definition for InlineAssignmentBlockBuilder
    /// </summary>
    internal static class InlineAssignmentBlockBuilder
    {
        /// <summary>
        /// Processes the non locals.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void Process(Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                InlineAssignmentBlockBuilder.Process(block.Children[childIndex]);
            }

            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                Block assignmentBlock = InlineAssignmentBlockBuilder.ProcessInlineAssignmentType1(
                    block, childIndex);

                if (assignmentBlock == null)
                {
                    assignmentBlock = InlineAssignmentBlockBuilder.ProcessInlineEqualsType2(block, childIndex);
                }

                if (assignmentBlock != null
                    && assignmentBlock.Parent is SerialBlock
                    && assignmentBlock.Parent.Children.Count == 1)
                {
                    assignmentBlock.Parent.Dissolve();
                }
            }
        }

        /// <summary>
        /// Merges the temp locals.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <param name="midValue">The mid value.</param>
        /// <returns></returns>
        public static SerialBlock MergeTempLocals(
            Block parentBlock,
            int blockIndex,
            InstructionBlock midValue)
        {
            int dissolvedChildrenCount = -1;
            InstructionBlock loadTempVariableInstructionBlock;
            StackedBlock postfixConsumer = parentBlock.Children[blockIndex + 1] as StackedBlock;
            SerialBlock returnValue = null;

            if (postfixConsumer != null)
            {
                dissolvedChildrenCount = postfixConsumer.Children.Count;
                loadTempVariableInstructionBlock = postfixConsumer.GetDependent(0) as InstructionBlock;
            }
            else
            {
                loadTempVariableInstructionBlock = parentBlock.Children[blockIndex + 1] as InstructionBlock;
            }

            if (loadTempVariableInstructionBlock != null
                && loadTempVariableInstructionBlock.Instruction.OpCode == OpCode.LoadLocal
                && loadTempVariableInstructionBlock.Instruction.OpCodeArgument.Equals(
                    midValue.Instruction.OpCodeArgument))
            {
                // Dissolve the stacked block so that we can move the postfix into the stacked
                // block.
                if (dissolvedChildrenCount > 0)
                {
                    parentBlock.Children[blockIndex + 1].Dissolve();
                }

                returnValue = new SerialBlock(
                    new BasicBlock[]
                        {
                            parentBlock.Children[blockIndex] as BasicBlock,
                            loadTempVariableInstructionBlock
                        });

                // now that we have created serial block that will be placeholder for postfix block,
                // recreate stacked block and make this serial block as it's child. in effect
                // we are reducing the number of children of parent by 1.
                if (dissolvedChildrenCount > 0)
                {
                    BasicBlock[] dependentBlocks = new BasicBlock[dissolvedChildrenCount - 1];
                    for (int childIndex = 0; childIndex < dependentBlocks.Length; childIndex++)
                    {
                        dependentBlocks[childIndex] = (BasicBlock)parentBlock.Children[blockIndex + childIndex];
                    }

                    new StackedBlock(
                        (InstructionBlock)parentBlock.Children[blockIndex + dissolvedChildrenCount - 1],
                        dependentBlocks);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Processes the inline equals type2.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <returns></returns>
        private static Block ProcessInlineEqualsType2(
            Block parentBlock,
            int blockIndex)
        {
            // Pattern for this one is
            // 1.     Value              ----}
            // 2.   Dup                      }  InlineAssignment for local variable.
            // 3.     DupCopy                }
            // 4.   StoreLocal(tmp)      ----}
            // 5. StoreReal
            // 6.   LoadLocal(tmp)
            // 7. UseType2
            //
            // In this case we are expecting to get StoreReal at blockIndex.
            StackedBlock storeReal = parentBlock.Children[blockIndex] as StackedBlock;
            if (storeReal == null)
            {
                return null;
            }

            if (PostfixBlockBuilder.GetStoreOperationDependencies(storeReal.RootBlock) < 0)
            {
                // If the setter is not one of the lhs for assignment, we don't need to
                // move any further.
                return null;
            }

            InlineAssignmentBlock storeRealArg = storeReal.GetDependent(storeReal.DependentCount() - 1) as InlineAssignmentBlock;
            if (storeRealArg == null)
            {
                return null;
            }

            StackedBlock storeTmp = storeRealArg.Setter;
            if (!(storeTmp.RootBlock.Instruction.OpCodeArgument is VariableDefinition))
            {
                return null;
            }

            return InlinePropertyInitializerBuilder.CreateAndMerge<BasicBlock>(
                parentBlock,
                blockIndex,
                blockIndex + 1,
                (VariableDefinition)storeTmp.RootBlock.Instruction.OpCodeArgument,
                (a) => new InlineAssignmentBlock(
                    storeReal,
                    storeRealArg.Value,
                    a));
        }

        private static Block ProcessInlineAssignmentType1(
            Block parentBlock,
            int blockIndex)
        {
            // Pattern for this one is
            // 1.     Value
            // 2.   Dup              -------}
            // 3.     DupCopy               }  serialBlock for inline assignment.
            // 4.   StoreLocal       -------}
            // 7. UseType1
            //
            // In this case we are expecting to get the serialBlock above at blockIndex.

            SerialBlock serialBlock = parentBlock.Children[blockIndex] as SerialBlock;

            if (serialBlock == null
                || serialBlock.Children.Count != 2
                || serialBlock.Children[1].GetInstructionAt(0).OpCode != OpCode.DupCopy)
            {
                return null;
            }

            StackedBlock storeLocalBlock = serialBlock.Children[1] as StackedBlock;
            if (storeLocalBlock == null
                || storeLocalBlock.Children.Count != 2
                || !(storeLocalBlock.GetDependent(0) is InstructionBlock))
            {
                return null;
            }

            var accessType = OpCodeHelper.GetElementAccessType(storeLocalBlock.RootBlock.Instruction);

            switch (accessType)
            {
                case ElementAccessType.SetStaticProperty:
                case ElementAccessType.SetStaticField:
                case ElementAccessType.SetArgument:
                case ElementAccessType.SetLocal:
                    break;
                default:
                    return null;
            }

            return new InlineAssignmentBlock(
                storeLocalBlock,
                (BasicBlock)((StackedBlock)serialBlock.Children[0]).GetDependent(0),
                serialBlock);
        }
    }
}