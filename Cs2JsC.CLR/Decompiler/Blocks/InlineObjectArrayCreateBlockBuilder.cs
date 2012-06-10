//-----------------------------------------------------------------------
// <copyright file="InlineObjectArrayCreateBlockBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for InlineObjectArrayCreateBlockBuilder
    /// </summary>
    internal static class InlineObjectArrayCreateBlockBuilder
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
                InlineObjectArrayCreateBlockBuilder.Process(block.Children[childIndex]);
            }

            if (block is SerialBlock &&
                (InlineObjectArrayCreateBlockBuilder.CreateInlineObjectArray(
                    block,
                    0) != null
                 || InlineObjectArrayCreateBlockBuilder.CreateStaticNumberArray(
                    block,
                    0) != null))
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
                    if (InlineObjectArrayCreateBlockBuilder.CreateInlineObjectArray(
                        block,
                        childIndex) == null)
                    {
                        InlineObjectArrayCreateBlockBuilder.CreateStaticNumberArray(
                            block,
                            childIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether {CC2D43FA-BBC4-448A-9D0B-7B57ADF2655C}[is inline object array block] [the specified parent block].
        /// 1. We need to check if this block is saving an Array.
        /// 2. Get the size of the array being created.
        /// 3 Check that n (size) blocks are saving to the array elements.
        /// 4. Next block after nth block should be holding LoadArraye instruction.
        /// 5. The temporary variable that is used to store this array should have a range
        ///    between 1 and 4.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <returns>size of the array.</returns>
        private static Block CreateInlineObjectArray(
            Block parentBlock,
            int blockIndex)
        {
            StackedBlock stackedBlock = parentBlock.Children[blockIndex] as StackedBlock;
            VariableDefinition localVariable = null;

            // New Array block is going to be Stacked block where root will be save to local
            // instruction and dependent block will be new array instruction.
            if (stackedBlock == null ||
                stackedBlock.RootBlock.Instruction.OpCode != OpCode.StoreLocal ||
                stackedBlock.DependentCount() > 1)
            {
                return null;
            }

            localVariable = (VariableDefinition)stackedBlock.RootBlock.Instruction.OpCodeArgument;

            // first dependent block has to be StackedBlock with root block with newArray
            // instruction.
            stackedBlock = stackedBlock.GetDependent(0) as StackedBlock;

            if (stackedBlock == null ||
                stackedBlock.RootBlock.Instruction.OpCode != OpCode.Newarr)
            {
                return null;
            }

            InstructionBlock instructionBlock = stackedBlock.GetDependent(0) as InstructionBlock;

            // This is final test to make sure that we are at the start of inline array.
            // in this case, we have to verify that the new array argument has constant
            // value as input.
            if (instructionBlock == null ||
                instructionBlock.Instruction.OpCode != OpCode.LoadConstantInt32)
            {
                return null;
            }

            int elementCount = (int)instructionBlock.Instruction.OpCodeArgument;

            int[] blockToIndexMapping = new int[elementCount];
            int arrayElementIndex = 1, lastSavedIndex = -1;

            for (; arrayElementIndex <= elementCount; arrayElementIndex++)
            {
                // We are looking past our limit.
                if (parentBlock.Children.Count <= arrayElementIndex + blockIndex)
                {
                    return null;
                }

                stackedBlock = parentBlock.Children[blockIndex + arrayElementIndex] as StackedBlock;

                // This instruction should be store instruction.
                // if that is not the case, break (we don't return because this could be statement
                // to load this array, which is also for our interest.
                if (stackedBlock == null)
                {
                    break;
                }

                OpCode opCode = stackedBlock.RootBlock.Instruction.OpCode;
                if (opCode != OpCode.StoreElement
                    && opCode != OpCode.StoreElement_ref
                    && opCode != OpCode.StoreElementInt8
                    && opCode != OpCode.StoreElementInt16
                    && opCode != OpCode.StoreElementInt32
                    && opCode != OpCode.StoreElementInt64
                    && opCode != OpCode.StoreElementIntPtr
                    && opCode != OpCode.StoreElementDouble
                    && opCode != OpCode.StoreElementSingle)
                {
                    break;
                }

                instructionBlock = stackedBlock.GetDependent(0) as InstructionBlock;

                // Make sure that we are storing to correct array in correct variable.
                if (instructionBlock == null ||
                    instructionBlock.Instruction.OpCode != OpCode.LoadLocal ||
                    !localVariable.Equals(instructionBlock.Instruction.OpCodeArgument))
                {
                    return null;
                }

                // make sure that we are loading a const and the value is greater than
                // lastSavedIndex and less than ElementCount.
                instructionBlock = stackedBlock.GetDependent(1) as InstructionBlock;
                if (instructionBlock == null ||
                    instructionBlock.Instruction.OpCode != OpCode.LoadConstantInt32 ||
                    lastSavedIndex >= (int)instructionBlock.Instruction.OpCodeArgument ||
                    elementCount <= (int)instructionBlock.Instruction.OpCodeArgument)
                {
                    return null;
                }

                // Ok till now we know that we have stored 
                lastSavedIndex = (int) instructionBlock.Instruction.OpCodeArgument;
                blockToIndexMapping[lastSavedIndex] = arrayElementIndex;
            }

            // To suceed there are 2 possibilities for the next block.
            // 1. it is StackedBlock and Dependent[0] is load array instruction.
            // 2. it is InstructionBlock and it is LoadArrayInstruction.
            return InlinePropertyInitializerBuilder.CreateAndMerge<BasicBlock>(
                parentBlock,
                blockIndex,
                blockIndex + arrayElementIndex,
                localVariable,
                (a) => new InlineObjectArrayCreateBlock(
                    blockToIndexMapping,
                    a));
        }

        private static Block CreateStaticNumberArray(
            Block parentBlock,
            int blockIndex)
        {
            MethodReference arrayCopyMethod =
                parentBlock.Context.ClrContext
                    .KnownReferences
                    .InitializeArray;

            //          LoadConst: Size of array
            //      NewArray: Type of array
            //  Dup
            //      Dup_Copy
            //      LdToken: static field with data for array
            //  MethodCall: CopyArrayContent
            if (blockIndex >= parentBlock.Children.Count - 1)
            {
                return null;
            }

            StackedBlock dupBlock = parentBlock.Children[blockIndex] as StackedBlock;
            if (dupBlock == null)
            {
                return null;
            }

            StackedBlock arrayBlock = dupBlock.Children[0] as StackedBlock;
            if (arrayBlock == null
                || arrayBlock.RootBlock.Instruction.OpCode != OpCode.Newarr)
            {
                return null;
            }

            InstructionBlock constSizeBlock = arrayBlock.Children[0] as InstructionBlock;
            if (constSizeBlock == null)
            {
                return null;
            }

            StackedBlock arrayCopyBlock = parentBlock.Children[blockIndex + 1] as StackedBlock;
            if (arrayCopyBlock == null
                || arrayCopyBlock.RootBlock.Instruction.OpCode != OpCode.Call
                || !arrayCopyMethod.IsSame((MethodReference)arrayCopyBlock.RootBlock.Instruction.OpCodeArgument))
            {
                return null;
            }

            FieldReference fieldReference = arrayCopyBlock.Children[1].GetInstructionAt(0).OpCodeArgument as FieldReference;
            if (fieldReference == null)
            {
                throw new InvalidProgramException("Unexpected code flow");
            }

            FieldDefinition fieldDef = fieldReference.Resolve();
            byte[] data = new byte[fieldDef.InitialValue.Length];
            fieldDef.InitialValue.CopyTo(data, 0);

            return new InlineNumberArrayCreateBlock(
                (TypeReference)arrayBlock.RootBlock.Instruction.OpCodeArgument,
                (int)constSizeBlock.Instruction.OpCodeArgument,
                data,
                dupBlock,
                arrayCopyBlock);
        }
    }
}
