//-----------------------------------------------------------------------
// <copyright file="PostfixBlockBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for PostfixBlockBuilder
    /// </summary>
    internal static class PostfixBlockBuilder
    {
        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        public static void Process(Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                PostfixBlockBuilder.Process(block.Children[childIndex]);
            }

            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                if (PostfixBlockBuilder.CreatePostfixBlock(
                    block,
                    childIndex) != null)
                {
                    if (block is SerialBlock
                        && block.Children.Count == 1)
                    {
                        block.Dissolve();
                    }
                }
            }
        }

        /// <summary>
        /// Creates the postfix block.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <returns></returns>
        public static Block CreatePostfixBlock(
            Block parentBlock,
            int blockIndex)
        {
            Block returnValue = PostfixBlockBuilder.CreateLocalPostfixBlock(
                parentBlock,
                blockIndex);

            if (returnValue == null)
            {
                returnValue = PostfixBlockBuilder.CreateFieldPostfixBlock(
                    parentBlock,
                    blockIndex);
            }

            return returnValue;
        }

        /// <summary>
        /// Creates the field postfix block.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <returns></returns>
        private static Block CreateFieldPostfixBlock(
            Block parentBlock,
            int blockIndex)
        {
            StackedBlock setterBlock = parentBlock.Children[blockIndex] as StackedBlock;

            if (setterBlock == null
                || blockIndex == parentBlock.Children.Count - 1)
            {
                // We need atleast two blocks to work with.
                return null;
            }

            int dependencies = PostfixBlockBuilder.GetStoreOperationDependencies(setterBlock.RootBlock);
            if (dependencies < 0)
            {
                return null;
            }

            // for Postfix with dependencies > 0 operation, we have following pattern
            // 0.       Getter                    getterBlock    ----}
            // 1.     Dup                       opArg1               }  InlineAssignmentBlock.
            // 1'       DupCopy                                      }
            // 2.     StoreLocal (tempVar)                       ----}
            // 3.     LoadConst (+-1)           opArg2
            // 3.   add/sub                     opBlock
            // 4. Setter.                       setterBlock
            // From the above sequence, we can see that getter is at
            // setter.dependents[dependents.Length-1].dependents[0].dependents[0].dependents[0].
            //                           add/sub         store         dup
            StackedBlock opBlock = setterBlock.GetDependent(setterBlock.DependentCount() - 1) as StackedBlock;
            if (opBlock == null)
            {
                return null;
            }

            // Let's check if second last instruction is really add/sub and with 1 or -1 as argument, if not return null.
            OpCode nextOpCode = opBlock.RootBlock.Instruction.OpCode;
            bool isIncrement;

            if (nextOpCode == OpCode.Sub
                || nextOpCode == OpCode.Sub_ovf
                || nextOpCode == OpCode.Sub_ovf_un)
            {
                isIncrement = false;
            }
            else if (nextOpCode == OpCode.Add
                || nextOpCode == OpCode.Add_ovf
                || nextOpCode == OpCode.Add_ovf_un)
            {
                isIncrement = true;
            }
            else
            {
                return null;
            }

            InstructionBlock instructionBlock = opBlock.GetDependent(1) as InstructionBlock;
            if (instructionBlock == null ||
                instructionBlock.Instruction.OpCode != OpCode.LoadConstantInt32)
            {
                return null;
            }

            if ((int)instructionBlock.Instruction.OpCodeArgument == -1)
            {
                isIncrement = !isIncrement;
            }
            else if ((int)instructionBlock.Instruction.OpCodeArgument != 1)
            {
                return null;
            }

            InlineAssignmentBlock opArg1SerialBlock = opBlock.GetDependent(0) as InlineAssignmentBlock;
            if (opArg1SerialBlock == null)
            {
                return null;
            }

            // Only if we have dependencies > 0, we have to check for store instruction
            // else there is no need to have this instruction in there.
            InstructionBlock midValue = (opArg1SerialBlock.Setter).RootBlock;
            nextOpCode = midValue.Instruction.OpCode;

            if (nextOpCode != OpCode.StoreLocal)
            {
                return null;
            }

            List<Block> argumentBlocks =
                PostfixBlockBuilder.AreReciprocatingOperations(
                    opArg1SerialBlock.Value,
                    setterBlock);

            if (argumentBlocks == null)
            {
                return null;
            }

            BasicBlock childBlock = setterBlock;

            childBlock = InlineAssignmentBlockBuilder.MergeTempLocals(
                parentBlock,
                blockIndex,
                midValue);

            if (childBlock == null)
            {
                childBlock = setterBlock;
            }
            else
            {
                midValue = null;
            }

            // Now that we have all the list of arguments, we know that we are dealing with
            // postfix instruction.
            return new PostfixBlock(
                opArg1SerialBlock.Value,
                setterBlock,
                argumentBlocks,
                midValue,
                isIncrement,
                childBlock);
        }

        private static Block CreateLocalPostfixBlock(
            Block parentBlock,
            int blockIndex)
        {
            if (blockIndex >= parentBlock.Children.Count - 1)
            {
                return null;
            }

            // for Postfix with dependencies == 0 (i.e. locals and statics) operations, we have following pattern.
            // where both setter and dup are siblings in serial block.
            // 0. Getter                getterBlock
            // 1. Dup                   getterDupBlock
            // 1'     DupCopy
            // 2.     LoadConst (+-1)
            // 2.   add/sub             opBlock
            // 3. setter                setterBlock

            StackedBlock getterDupBlock = parentBlock.Children[blockIndex] as StackedBlock;
            if (getterDupBlock == null
                || getterDupBlock.RootBlock.Instruction.OpCode != OpCode.Dup)
            {
                return null;
            }

            StackedBlock setterBlock = parentBlock.Children[blockIndex + 1] as StackedBlock;
            if (setterBlock == null
                || blockIndex == parentBlock.Children.Count - 1
                || PostfixBlockBuilder.GetStoreOperationDependencies(setterBlock.RootBlock) != 0)
            {
                // We need atleast two blocks to work with.
                return null;
            }

            StackedBlock opBlock = setterBlock.GetDependent(setterBlock.DependentCount() - 1) as StackedBlock;
            if (opBlock == null)
            {
                return null;
            }

            // Let's check if second last instruction is really add/sub and with 1 or -1 as argument, if not return null.
            OpCode opCode = opBlock.RootBlock.Instruction.OpCode;
            bool isIncrement;

            if (opCode == OpCode.Sub
                || opCode == OpCode.Sub_ovf
                || opCode == OpCode.Sub_ovf_un)
            {
                isIncrement = false;
            }
            else if (opCode == OpCode.Add
                || opCode == OpCode.Add_ovf
                || opCode == OpCode.Add_ovf_un)
            {
                isIncrement = true;
            }
            else
            {
                return null;
            }

            // Since we have post increment or decrement, we can have only 1 or -1 as arguments
            InstructionBlock opArg2Block = opBlock.GetDependent(1) as InstructionBlock;

            if (opArg2Block == null ||
                opArg2Block.Instruction.OpCode != OpCode.LoadConstantInt32)
            {
                return null;
            }

            if ((int)opArg2Block.Instruction.OpCodeArgument == -1)
            {
                isIncrement = !isIncrement;
            }
            else if ((int)opArg2Block.Instruction.OpCodeArgument != 1)
            {
                return null;
            }

            InstructionBlock opArg1Block = opBlock.GetDependent(0) as InstructionBlock;
            if (opArg1Block == null
                || opArg1Block.Instruction.OpCode != OpCode.DupCopy)
            {
                return null;
            }

            Block getterBlock = getterDupBlock.GetDependent(0);

            List<Block> argumentBlocks =
                PostfixBlockBuilder.AreReciprocatingOperations(
                    getterBlock,
                    setterBlock);

            if (argumentBlocks == null)
            {
                return null;
            }

            SerialBlock childBlock = new SerialBlock(
                new BasicBlock[]
                {
                    getterDupBlock,
                    setterBlock
                });

            // Now that we have all the list of arguments, we know that we are dealing with
            // postfix instruction.
            return new PostfixBlock(
                getterBlock,
                setterBlock,
                argumentBlocks,
                null,
                isIncrement,
                childBlock);
        }

        /// <summary>
        /// Gets the store operation dependencies.
        /// </summary>
        /// <param name="instructionBlock">The instruction block.</param>
        /// <returns></returns>
        public static int GetStoreOperationDependencies(
            InstructionBlock instructionBlock)
        {
            switch (OpCodeHelper.GetElementAccessType(instructionBlock.Instruction))
            {
                case ElementAccessType.SetProperty:
                case ElementAccessType.SetStaticProperty:
                    {
                        MethodReference methodReference = (MethodReference)instructionBlock.Instruction.OpCodeArgument;
                        return methodReference.Parameters.Count;
                    }
                case ElementAccessType.SetArray:
                    return 2;
                case ElementAccessType.SetReferenceArgument:
                case ElementAccessType.SetField:
                    return 1;
                case ElementAccessType.SetLocal:
                case ElementAccessType.SetArgument:
                case ElementAccessType.SetStaticField:
                    return 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Ares the reciprocating operations.
        /// </summary>
        /// <param name="readBlock">The read block.</param>
        /// <param name="writeBlock">The write block.</param>
        /// <returns>List of arguments for the operation.</returns>
        public static List<Block> AreReciprocatingOperations(
            Block readBlock,
            StackedBlock writeBlock)
        {
            StackedBlock readStackedBlock = readBlock as StackedBlock;
            InstructionBlock readInstructionBlock =
                readStackedBlock != null
                    ? readStackedBlock.RootBlock
                    : readBlock as InstructionBlock;

            if (readInstructionBlock == null)
            {
                return null;
            }

            ElementAccessType writeAccess =
                OpCodeHelper.GetElementAccessType(
                    writeBlock.RootBlock.Instruction);
            ElementAccessType readAccess =
                OpCodeHelper.GetElementAccessType(
                    readInstructionBlock.Instruction);

            int dependencies = -1;

            switch (writeAccess)
            {
                case ElementAccessType.None:
                case ElementAccessType.GetStaticProperty:
                case ElementAccessType.GetProperty:
                case ElementAccessType.GetStaticField:
                case ElementAccessType.GetField:
                case ElementAccessType.GetArray:
                case ElementAccessType.GetReferenceArgument:
                case ElementAccessType.GetArgument:
                case ElementAccessType.GetLocal:
                    return null;
                case ElementAccessType.SetStaticProperty:
                    if (readAccess != ElementAccessType.GetStaticProperty)
                    {
                        return null;
                    }

                    if (!((MethodReference)readInstructionBlock.Instruction.OpCodeArgument)
                            .Resolve()
                            .GetPropertyDefinition()
                            .IsSameDefinition(
                                ((MethodReference)writeBlock.RootBlock.Instruction.OpCodeArgument)
                                    .Resolve()
                                    .GetPropertyDefinition()))
                    {
                        return null;
                    }

                    return new List<Block>();
                case ElementAccessType.SetProperty:
                    if (readAccess != ElementAccessType.GetProperty)
                    {
                        return null;
                    }

                    if (((MethodReference)readInstructionBlock.Instruction.OpCodeArgument)
                            .Resolve()
                            .GetPropertyDefinition()
                            .IsSameDefinition(
                                ((MethodReference)writeBlock.RootBlock.Instruction.OpCodeArgument)
                                    .Resolve()
                                    .GetPropertyDefinition()))
                    {
                        dependencies = ((MethodReference)readInstructionBlock.Instruction.OpCodeArgument).Parameters.Count + 1;
                        break;
                    }

                    return null;
                case ElementAccessType.SetStaticField:
                    if (readAccess != ElementAccessType.GetStaticField)
                    {
                        return null;
                    }

                    if (!((FieldReference)readInstructionBlock.Instruction.OpCodeArgument).IsSame(
                            (FieldReference)writeBlock.RootBlock.Instruction.OpCodeArgument))
                    {
                        return null;
                    }

                    return new List<Block>();
                case ElementAccessType.SetField:
                    if (readAccess != ElementAccessType.GetField)
                    {
                        return null;
                    }

                    if (!((FieldReference)readInstructionBlock.Instruction.OpCodeArgument).IsSame(
                            (FieldReference)writeBlock.RootBlock.Instruction.OpCodeArgument))
                    {
                        return null;
                    }

                    dependencies = 1;
                    break;
                case ElementAccessType.SetArray:
                    if (readAccess != ElementAccessType.GetArray)
                    {
                        return null;
                    }

                    dependencies = 2;
                    break;
                case ElementAccessType.SetReferenceArgument:
                    if (readAccess != ElementAccessType.GetReferenceArgument)
                    {
                        return null;
                    }

                    dependencies = 1;
                    break;
                case ElementAccessType.SetArgument:
                    if (readAccess != ElementAccessType.GetArgument)
                    {
                        return null;
                    }

                    if (!readInstructionBlock.Instruction.OpCodeArgument.Equals(
                            writeBlock.RootBlock.Instruction.OpCodeArgument))
                    {
                        return null;
                    }

                    return new List<Block>();
                case ElementAccessType.SetLocal:
                    if (readAccess != ElementAccessType.GetLocal)
                    {
                        return null;
                    }

                    if (!readInstructionBlock.Instruction.OpCodeArgument.Equals(
                            writeBlock.RootBlock.Instruction.OpCodeArgument))
                    {
                        return null;
                    }

                    return new List<Block>();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (readStackedBlock.DependentCount() != dependencies ||
                writeBlock.DependentCount() != dependencies + 1)
            {
                return null;
            }

            List<Block> argumentBlocks = new List<Block>();
            for (int i = 0; i < dependencies; i++)
            {
                Block argument = PostfixBlockBuilder.AreSame(
                    readStackedBlock.GetDependent(i),
                    writeBlock.GetDependent(i));
                if (argument == null)
                {
                    return null;
                }

                argumentBlocks.Add(argument);
            }

            return argumentBlocks;
        }

        /// <summary>
        /// Ares the same.
        /// </summary>
        /// <param name="getterArgument">The getter argument.</param>
        /// <param name="setterArgument">The setter argument.</param>
        /// <returns>return true if both of these arguments resolve to same value.</returns>
        private static Block AreSame(
            Block getterArgument,
            Block setterArgument)
        {
            // Ther are two cases where.
            // 1. setterArgument is InstructionBlock, in which case it is load local.
            //    In this case getter may resolve to load of that local, or Dup + store to that local.
            //    Or both reader and writer could be loadConstInt
            // 2. setterArgument is StackedBlock, in which case it should resolve to getterArgument's parent
            //    which is getter itself and getterArgument should be first child of it's parent.

            InstructionBlock setterInstructionBlock = setterArgument as InstructionBlock;
            InstructionBlock getterInstructionBlock = getterArgument as InstructionBlock;
            StackedBlock getterStackedBlock = getterArgument as StackedBlock;
            StackedBlock setterStackedBlock = setterArgument as StackedBlock;

            if (setterInstructionBlock != null)
            {
                OpCode setterOpCode = setterInstructionBlock.Instruction.OpCode;

                // this is case 1.
                if (setterOpCode != OpCode.LoadLocal
                    && setterOpCode != OpCode.LoadArgument)
                {
                    if (setterOpCode == OpCode.LoadConstantInt32
                        && getterInstructionBlock != null
                        && getterInstructionBlock.Instruction.OpCode == OpCode.LoadConstantInt32
                        && (int)getterInstructionBlock.Instruction.OpCodeArgument
                            == (int)setterInstructionBlock.Instruction.OpCodeArgument)
                    {
                        return getterArgument;
                    }

                    return null;
                }

                OpCode getterOpCode;

                if (getterInstructionBlock != null)
                {
                    getterOpCode = getterInstructionBlock.Instruction.OpCode;

                    if (setterOpCode != getterOpCode)
                    {
                        return null;
                    }

                    if (setterOpCode == OpCode.LoadArgument
                        && ((ParameterReference)setterInstructionBlock.Instruction.OpCodeArgument).Index
                            == ((ParameterReference)getterInstructionBlock.Instruction.OpCodeArgument).Index)
                    {
                        return getterArgument;
                    }

                    if (setterOpCode == OpCode.LoadLocal
                        && (int)setterInstructionBlock.Instruction.OpCodeArgument ==
                            (int)getterInstructionBlock.Instruction.OpCodeArgument)
                    {
                        return getterArgument;
                    }

                    return null;
                }

                // In this case getterArgument should be Dup + store.
                if (getterStackedBlock == null
                    || getterStackedBlock.DependentCount() != 1)
                {
                    return null;
                }

                StackedBlock dupStackBlock = getterStackedBlock.GetDependent(0) as StackedBlock;
                if (dupStackBlock == null
                    || dupStackBlock.RootBlock.Instruction.OpCode != OpCode.Dup)
                {
                    return null;
                }

                IlInstruction getterInstruction = getterStackedBlock.RootBlock.Instruction;
                getterOpCode = getterInstruction.OpCode;

                if ((setterOpCode == OpCode.LoadLocal && getterOpCode != OpCode.StoreLocal)
                    || (setterOpCode == OpCode.LoadArgument && getterOpCode != OpCode.StoreArgument))
                {
                    return null;
                }

                if ((int)setterInstructionBlock.Instruction.OpCodeArgument != (int)getterInstruction.OpCodeArgument)
                {
                    return null;
                }

                return dupStackBlock.GetDependent(0);
            }

            if (getterInstructionBlock == null
                || getterInstructionBlock.Instruction.OpCode != OpCode.DupCopy
                || getterInstructionBlock.Instruction.OpCodeArgument != setterStackedBlock.RootBlock.Instruction)
            {
                // This is second case which is much simpler to verify.
                return null;
            }

            return setterStackedBlock;
        }
    }
}