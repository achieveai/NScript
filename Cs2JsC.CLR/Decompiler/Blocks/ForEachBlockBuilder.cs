//-----------------------------------------------------------------------
// <copyright file="ForEachBlockBuilder.cs" company="">
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
    /// Definition for ForEachBlockBuilder
    /// </summary>
    internal static class ForEachBlockBuilder
    {
        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void Process(
            Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                if (block.Children[childIndex].StackBefore > 0
                    || block.Children[childIndex].StackAfter > 0)
                {
                    return;
                }

                ForEachBlockBuilder.Process(block.Children[childIndex]);

                ForEachBlockBuilder.Create(
                    block,
                    childIndex);
            }
        }

        /// <summary>
        /// Creates the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="childIndex">Index of the child.</param>
        private static void Create(Block block, int childIndex)
        {
            if (childIndex >= block.Children.Count - 1)
            {
                return;
            }

            // Check current block for GetEnumerableCall.
            StackedBlock assignmentBlock = block.Children[childIndex] as StackedBlock;
            if (assignmentBlock == null
                || assignmentBlock.RootBlock.Instruction.OpCode != OpCode.StoreLocal)
            {
                return;
            }

            StackedBlock getEnumeratorCall = assignmentBlock.GetDependent(0) as StackedBlock;
            if (getEnumeratorCall == null
                || getEnumeratorCall.RootBlock.Instruction.OpCode != OpCode.CallVirtual)
            {
                return;
            }

            MethodReference getEnumeratorMethod = (MethodReference)getEnumeratorCall.RootBlock.Instruction.OpCodeArgument;
            if (getEnumeratorMethod.DeclaringType.Name != "IEnumerable"
                && getEnumeratorMethod.DeclaringType.Namespace == "System.Collections")
            {
                return;
            }

            // Check try-finally block.
            ExceptionHandlerBlock exceptionHandlerBlock = block.Children[childIndex + 1] as ExceptionHandlerBlock;
            if (exceptionHandlerBlock == null)
            {
                return;
            }

            if (exceptionHandlerBlock.IsTryCatch)
            {
                return;
            }

            // Let's check range for tempVariable used for assignment.
            Tuple<IlInstruction, IlInstruction> range =
                block.Context.GetRangeContaining(
                    (VariableDefinition)assignmentBlock.RootBlock.Instruction.OpCodeArgument,
                    assignmentBlock.RootBlock.Instruction);

            if (range.Item1 != assignmentBlock.RootBlock.Instruction
                || range.Item1.Index > exceptionHandlerBlock.BlockEndIndex)
            {
                return;
            }

            // Let's check if we have correct whileBlock or not.
            WhileBlock whileBlock = exceptionHandlerBlock.TryBlock.Children[0] as WhileBlock;
            if (whileBlock == null)
            {
                return;
            }

            StackedBlock conditionBlock = whileBlock.Condition as StackedBlock;
            if (conditionBlock == null)
            {
                return;
            }

            if (conditionBlock.RootBlock.Instruction.OpCode != OpCode.BranchTrue)
            {
                return;
            }

            conditionBlock = conditionBlock.GetDependent(0) as StackedBlock;
            if (conditionBlock == null)
            {
                return;
            }

            if (conditionBlock.RootBlock.Instruction.OpCode != OpCode.CallVirtual)
            {
                return;
            }

            MethodReference methodReference = conditionBlock.RootBlock.Instruction.OpCodeArgument as MethodReference;
            if (methodReference.Name != "MoveNext"
                || (methodReference.DeclaringType.FullName != "System.Collections.IEnumerator"))
            {
                return;
            }

            StackedBlock itemStore = whileBlock.Loop.Children[0] as StackedBlock;
            if (itemStore == null
                || itemStore.RootBlock.Instruction.OpCode != OpCode.StoreLocal)
            {
                return;
            }

            StackedBlock castType = itemStore.GetDependent(0) as StackedBlock;
            if (castType == null)
            {
                return;
            }

            StackedBlock getCurrentCall;
            if (castType.RootBlock.Instruction.OpCode == OpCode.Castclass)
            {
                getCurrentCall = castType.GetDependent(0) as StackedBlock;
            }
            else
            {
                getCurrentCall = castType;
                castType = null;
            }

            if (getCurrentCall == null
                || getCurrentCall.RootBlock.Instruction.OpCode != OpCode.CallVirtual
                || getCurrentCall.InstructionCount != 2
                || getCurrentCall.GetInstructionAt(0).OpCode != OpCode.LoadLocal
                || !getCurrentCall.GetInstructionAt(0).OpCodeArgument.Equals(
                        assignmentBlock.RootBlock.Instruction.OpCodeArgument))
            {
                return;
            }

            methodReference = (MethodReference)getCurrentCall.RootBlock.Instruction.OpCodeArgument;
            if (methodReference.DeclaringType.FullName != "System.Collections.IEnumerator"
                || methodReference.Name != "get_Current")
            {
                return;
            }

            // Let's check if we have correct finallyBlock or not.
            Block finallyBlock = exceptionHandlerBlock.HandlerBlock;
            if (finallyBlock.Children.Count != 3)
            {
                return;
            }

            StackedBlock disposableStore = finallyBlock.Children[0] as StackedBlock;
            if (disposableStore == null
                || disposableStore.RootBlock.Instruction.OpCode != OpCode.StoreLocal)
            {
                return;
            }

            StackedBlock asDisposable = disposableStore.GetDependent(0) as StackedBlock;
            if (asDisposable == null
                || asDisposable.RootBlock.Instruction.OpCode != OpCode.Isinst
                || ((TypeReference)asDisposable.RootBlock.Instruction.OpCodeArgument).FullName != 
                    "System.IDisposable")
            {
                return;
            }

            IfElseBlock ifElseBlock = finallyBlock.Children[1] as IfElseBlock;
            if (ifElseBlock == null)
            {
                return;
            }

            StackedBlock disposeCall = ifElseBlock.IfBlock is StackedBlock
                ? (StackedBlock)ifElseBlock.IfBlock
                : ifElseBlock.IfBlock.Children[0] as StackedBlock;

            if (disposeCall == null
                || disposeCall.RootBlock.Instruction.OpCode != OpCode.CallVirtual)
            {
                return;
            }

            methodReference = (MethodReference)disposeCall.RootBlock.Instruction.OpCodeArgument;
            if (methodReference.Name != "Dispose"
                || methodReference.DeclaringType.FullName != "System.IDisposable")
            {
                return;
            }

            new ForEachBlock(
                getEnumeratorCall.GetDependent(0),
                (VariableDefinition)itemStore.RootBlock.Instruction.OpCodeArgument,
                castType != null
                    ? (TypeReference)castType.RootBlock.Instruction.OpCodeArgument
                    : null,
                new Block[]{block.Children[childIndex], block.Children[childIndex + 1]});
        }
    }
}
