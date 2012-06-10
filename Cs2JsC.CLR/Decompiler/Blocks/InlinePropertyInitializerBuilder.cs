//-----------------------------------------------------------------------
// <copyright file="InlinePropertyInitializerBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for InlinePropertyInitializerBuilder
    /// </summary>
    internal static class InlinePropertyInitializerBuilder
    {
        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        public static void Process(
            Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                InlinePropertyInitializerBuilder.Process(block.Children[childIndex]);
            }

            if (block is SerialBlock
                && InlinePropertyInitializerBuilder.CreateInlineSetters(
                        block,
                        0) != null)
            {
                // There is no point in having single InlineObjectArray in serialBlock
                // Let's dissolve the serialBlock.
                if (block.Children.Count == 1)
                {
                    block.Dissolve();
                    return;
                }
            }
            else
            {
                for (int childIndex = 0; childIndex < block.Children.Count - 2; childIndex++)
                {
                    InlinePropertyInitializerBuilder.CreateInlineSetters(
                        block,
                        childIndex);
                }
            }
        }

        /// <summary>
        /// Creates the and merge.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <param name="subBlockEndIndex">Index of the sub block.</param>
        /// <param name="localVariable">Index of the local variable.</param>
        /// <param name="subBlockCreator">The sub block creator.</param>
        /// <returns></returns>
        public static Block CreateAndMerge<T>(
            Block parentBlock,
            int blockIndex,
            int subBlockEndIndex,
            VariableDefinition localVariable,
            Func<T[], T> subBlockCreator) where T : Block
        {
            // To suceed there are 2 possibilities for the next block.
            // 1. it is StackedBlock it's first instruction is consumer for the inline block.
            // 2. it is InstructionBlock and it is loadVar instruction.

            IlInstruction instruction = parentBlock.Children[subBlockEndIndex].GetInstructionAt(0);
            InstructionBlock instructionBlock = parentBlock.Context.InstructionToBlock[instruction];

            if (instruction.OpCode != OpCode.LoadLocal ||
                localVariable != (VariableDefinition)instruction.OpCodeArgument)
            {
                return null;
            }

            // For inline array initialization, the range of this temporary variable falls within
            // the range that we are using to create InlineObjectArrayInitBlock.
            var variableRange = parentBlock.Context.GetRangeContaining(localVariable, instructionBlock.Instruction);
            if (variableRange.Item1.Index < ((StackedBlock)parentBlock.Children[blockIndex]).BlockStartIndex ||
                variableRange.Item1.Index > ((StackedBlock)parentBlock.Children[blockIndex]).BlockEndIndex ||
                variableRange.Item2 != instructionBlock.Instruction)
            {
                return null;
            }

            // Also make sure that instructionBlock is not consumed by DupBlock
            if (instruction.Next.OpCode == OpCode.Dup)
            {
                return null;
            }

            // There is also a case where when we are using inline delegates, and
            // we are using argument in inline delegate, in that case we create temp
            // class and use it's function as delegate.
            if (blockIndex == 0 && instruction.Next.OpCode == OpCode.LoadFunction)
            {
                return null;
            }

            T inlineObject;

            // Since next block is not the instruction block that we are trying to combine with,
            // we need to move all these blocks next to instruction block in a serial block
            // and then combine all these blocks.
            if (parentBlock.Children[subBlockEndIndex] != instructionBlock)
            {
                SerialBlock serialBlock = new SerialBlock(
                    new BasicBlock[]
                    {
                        instructionBlock
                    });

                // create a temp block to be moved.
                BasicBlock basicBlock = new BasicBlock(
                    parentBlock.CreateChildrenArray<T>(
                        blockIndex,
                        subBlockEndIndex));

                // remove the temp block from parentBlock.
                parentBlock.RemoveChildAt(blockIndex);

                // add it to serial block.
                serialBlock.AddChildAt(basicBlock, 0);

                // dissolve temp block.
                basicBlock.Dissolve();

                // create inlineObject.
                inlineObject = subBlockCreator(
                    serialBlock.CreateChildrenArray<T>(
                        0,
                        serialBlock.Children.Count));

                // now dissolve the tempSerial block as well.
                serialBlock.Dissolve();
            }
            else
            {
                inlineObject = subBlockCreator(
                    parentBlock.CreateChildrenArray<T>(
                        blockIndex,
                        subBlockEndIndex + 1));
            }

            return inlineObject;
        }

        /// <summary>
        /// Gets the last index of the setters.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <param name="localVariable">Index of the local variable.</param>
        /// <returns></returns>
        public static int GetSettersLastIndex(
            Block parentBlock,
            int blockIndex,
            out VariableDefinition localVariable)
        {
            StackedBlock stackedBlock = parentBlock.Children[blockIndex] as StackedBlock;
            localVariable = null;

            // New Array block is going to be Stacked block where root will be save to local
            // instruction and dependent block will be new array instruction.
            if (stackedBlock == null
                || stackedBlock.RootBlock.Instruction.OpCode != OpCode.StoreLocal
                || stackedBlock.DependentCount() > 1)
            {
                return -1;
            }

            localVariable = (VariableDefinition)stackedBlock.RootBlock.Instruction.OpCodeArgument;

            // Let's get second last instruction out and check if it is constructor or not.
            var ctorInstruction = stackedBlock.GetInstructionAt(stackedBlock.InstructionCount - 2);
            if (ctorInstruction.OpCode != OpCode.Newobj
                || !(ctorInstruction.OpCodeArgument is MethodReference))
            {
                return -1;
            }

            // first dependent block has to be StackedBlock with root block with newArray
            // instruction.
            var instruction = stackedBlock.GetDependent(0).GetInstructionAt(
                stackedBlock.GetDependent(0).InstructionCount - 1);

            if (instruction.OpCode != OpCode.Newobj)
            {
                return -1;
            }

            InstructionBlock instructionBlock;

            int propertyElementIndex = blockIndex + 1;
            for (; propertyElementIndex < parentBlock.Children.Count - 1; propertyElementIndex++)
            {
                stackedBlock = parentBlock.Children[propertyElementIndex] as StackedBlock;

                // This instruction should be store field or property setter.
                // in both the cases dependent block count is 2.
                if (stackedBlock == null
                    || stackedBlock.DependentCount() != 2
                    || (stackedBlock.RootBlock.Instruction.OpCode != OpCode.CallVirtual
                       && stackedBlock.RootBlock.Instruction.OpCode != OpCode.StoreField
                       && stackedBlock.RootBlock.Instruction.OpCode != OpCode.Call))
                {
                    break;
                }

                instructionBlock = stackedBlock.GetDependent(0) as InstructionBlock;

                // Make sure that we are storing to correct array in correct variable.
                if (instructionBlock == null
                    || instructionBlock.Instruction.OpCode != OpCode.LoadLocal
                    || !localVariable.Equals(instructionBlock.Instruction.OpCodeArgument))
                {
                    return -1;
                }

                // Check if the opCode is not method call, it is call to propertySetter.
                if (stackedBlock.RootBlock.Instruction.OpCode != OpCode.StoreField)
                {
                    MethodReference methodReference =
                        (MethodReference)stackedBlock.RootBlock.Instruction.OpCodeArgument;

                    MethodDefinition methodDefinition = methodReference.Resolve();

                    // This is not a property.
                    if (methodDefinition.GetPropertyDefinition() == null)
                    {
                        return -1;
                    }
                }
            }

            return propertyElementIndex;
        }

        /// <summary>
        /// Creates InlinePropertySetterBlock
        /// 1. We need to check if this block is saving new object.
        /// 2. Now go through all the following blocks that are stackedBlock and either setter method call or setField.
        /// 3. Once we are done with all the setters, our next block should be either Stacked block with first
        ///    argument as load instruction for this object, or it is InstructionBlock with load instruction of this object.
        /// 4. The temporary variable that is used to store this array should have a range
        ///    between 1 and 4.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <returns>Replacement block if creation suceeded.</returns>
        private static Block CreateInlineSetters(
            Block parentBlock,
            int blockIndex)
        {
            VariableDefinition localVariableIndex;
            int propertyElementIndex = InlinePropertyInitializerBuilder.GetSettersLastIndex(
                parentBlock,
                blockIndex,
                out localVariableIndex);

            if (propertyElementIndex < 0)
            {
                return null;
            }

            return InlinePropertyInitializerBuilder.CreateAndMerge<BasicBlock>(
                parentBlock,
                blockIndex,
                propertyElementIndex,
                localVariableIndex,
                (a) => new InlinePropertyInitializerBlock(a));
        }
    }
}