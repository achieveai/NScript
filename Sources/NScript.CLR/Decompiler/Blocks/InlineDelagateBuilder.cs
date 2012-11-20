//-----------------------------------------------------------------------
// <copyright file="InlineDelegateBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for InlineDelegateBuilder
    /// </summary>
    internal class InlineDelegateBuilder
    {
        /// <summary>
        /// Tracker for delegate starter block.
        /// </summary>
        int functionStartIndex = -1;

        /// <summary>
        /// Inline delegate class.
        /// </summary>
        TypeReference delegateClass = null;

        /// <summary>
        /// Mapping from fields in inline delegate class to arguments.
        /// </summary>
        List<Tuple<FieldReference, ParameterDefinition>> argumentMappings =
            new List<Tuple<FieldReference, ParameterDefinition>>();

        /// <summary>
        /// Mapping from fields in inline delegate class to locals.
        /// </summary>
        List<Tuple<FieldReference, string, Block>> localMappings =
            new List<Tuple<FieldReference,string, Block>>();

        /// <summary>
        /// If any delegate on delegateClass was found or not.
        /// </summary>
        bool inlineLocalSharingDelegateFound = false;

        /// <summary>
        /// Prevents a default instance of the <see cref="InlineDelegateBuilder"/> class from being created.
        /// </summary>
        /// <param name="block">The block.</param>
        private InlineDelegateBuilder(RootBlock block)
        {
            this.InitializeFieldMappings(
                block);

            this.Process((Block)block);

            if (this.inlineLocalSharingDelegateFound)
            {
                block.Context.InlineDelegateInfo = new InlineDelegateInfo(
                    this.functionStartIndex,
                    this.delegateClass,
                    this.argumentMappings,
                    this.localMappings);
            }
        }

        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void Process(RootBlock block)
        {
            new InlineDelegateBuilder(block);
        }

        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        private void Process(Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                if (this.ProcessStoredDelegates(
                    block,
                    childIndex))
                {
                    if (block.Children[childIndex] is InlineDelegateBlock)
                    {
                        continue;
                    }
                }
                else if (this.ProcessDelegate(block, childIndex))
                {
                    continue;
                }

                this.Process(block.Children[childIndex]);
            }
        }

        /// <summary>
        /// Processes the delegate.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="childIndex">Index of the child.</param>
        /// <returns>true if delegate block was creted</returns>
        private bool ProcessDelegate(
            Block parentBlock,
            int childIndex)
        {
            StackedBlock delegateBlock = parentBlock.Children[childIndex] as StackedBlock;

            if (this.IsInlineDelegateCtor(delegateBlock))
            {
                new InlineDelegateBlock(
                    delegateBlock,
                    new BasicBlock[] {
                        delegateBlock
                    });

                return true;
            }

            return false;
        }

        /// <summary>
        /// Processes the stored delegates.
        /// </summary>
        /// <param name="parentBlock">The parent block.</param>
        /// <param name="blockIndex">Index of the block.</param>
        /// <returns>true if delegate block was created.</returns>
        /// <remarks>
        /// in this case we are looking for delegates that are stored on a variable so as to avoid
        /// re-creating delegate every time. the expression looks like following:
        /// if (!load(var))
        /// {
        /// store(var, new delegate(....));
        /// }
        /// load(var)
        /// </remarks>
        private bool ProcessStoredDelegates(
            Block parentBlock,
            int blockIndex)
        {
            if (parentBlock.Children.Count < blockIndex + 2)
            {
                return false;
            }

            IfElseBlock ifBlock = parentBlock.Children[blockIndex] as IfElseBlock;
            if (ifBlock == null)
            {
                return false;
            }

            StackedBlock conditionBlock = ifBlock.Condition as StackedBlock;
            if (conditionBlock == null
                || conditionBlock.DependentCount() != 1)
            {
                return false;
            }

            InstructionBlock ifConditionBlock = conditionBlock.GetDependent(0) as InstructionBlock;
            if (ifConditionBlock == null)
            {
                return false;
            }

            IlInstruction nextLoadBlock = parentBlock.Children[blockIndex + 1].GetInstructionAt(0);

            if (nextLoadBlock == null)
            {
                return false;
            }

            if (nextLoadBlock.OpCode != ifConditionBlock.Instruction.OpCode
                || !object.Equals(
                    nextLoadBlock.OpCodeArgument,
                    ifConditionBlock.Instruction.OpCodeArgument))
            {
                return false;
            }

            StackedBlock delegateStoringBlock = ifBlock.IfBlock as StackedBlock;
            if (delegateStoringBlock == null
                && !(ifBlock.IfBlock is InstructionBlock))
            {
                delegateStoringBlock = ifBlock.IfBlock.Children[0] as StackedBlock;
            }

            if (delegateStoringBlock == null)
            {
                return false;
            }

            if (PostfixBlockBuilder.AreReciprocatingOperations(
                ifConditionBlock,
                delegateStoringBlock) == null)
            {
                return false;
            }

            StackedBlock delegateBlock =
                delegateStoringBlock.GetDependent(delegateStoringBlock.DependentCount() - 1)
                    as StackedBlock;

            if (delegateBlock == null)
            {
                return false;
            }

            if (!this.IsInlineDelegateCtor(delegateBlock))
            {
                return false;
            }

            Block delegateConsumingBlock = parentBlock.Children[blockIndex + 1];
            if (!(delegateConsumingBlock is StackedBlock)
                && !(delegateConsumingBlock is InstructionBlock))
            {
                return false;
            }

            int expansionCount = delegateConsumingBlock.Children.Count;
            delegateConsumingBlock.Dissolve();

            new InlineDelegateBlock(
                delegateBlock,
                new Block[] {
                    parentBlock.Children[blockIndex],
                    parentBlock.Children[blockIndex + 1]
                });

            if (expansionCount > 1)
            {
                new StackedBlock(
                    (InstructionBlock)parentBlock.Children[blockIndex + expansionCount - 1],
                    parentBlock.CreateChildrenArray<Block>(
                        blockIndex,
                        blockIndex + expansionCount - 1));
            }


            return true;
        }

        /// <summary>
        /// Determines whether the specified block is inline delegate ctor.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns>
        /// <c>true</c> if the specified block is inline delegate ctor; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInlineDelegateCtor(
            StackedBlock block)
        {
            if (block == null)
            {
                return false;
            }

            if (block.RootBlock.Instruction.OpCode != OpCode.Newobj
                || block.DependentCount() != 2)
            {
                return false;
            }

            InstructionBlock loadFunctionBlock = block.GetDependent(1) as InstructionBlock;
            if (loadFunctionBlock == null
                || loadFunctionBlock.Instruction.OpCode != OpCode.LoadFunction)
            {
                return false;
            }

            MethodReference methodReference = (MethodReference)
                loadFunctionBlock.Instruction.OpCodeArgument;

            // This method is inline delegate if it is compiler generated.
            if (methodReference.Resolve().CustomAttributes.SelectAttribute(
                    block.Context.ClrContext.KnownReferences.CompilerGeneratedAttribute) != null)
            {
                return true;
            }

            // This is true in case of inline delegates that shares local variables.
            if (this.delegateClass != null
                && methodReference.DeclaringType.IsSameDefinition(this.delegateClass))
            {
                this.inlineLocalSharingDelegateFound = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes the field mappings.
        /// </summary>
        /// <param name="initBlock">The init block.</param>
        private void InitializeFieldMappings(RootBlock rootBlock)
        {
            VariableDefinition tempVarIndex;
            int lastIndex = InlinePropertyInitializerBuilder.GetSettersLastIndex(
                    rootBlock,
                    0,
                    out tempVarIndex);

            if (lastIndex < 0)
            {
                return;
            }

            HashSet<ParameterDefinition> argumentsUsed = new HashSet<ParameterDefinition>();
            HashSet<string> fieldsUsed = new HashSet<string>();
            List<Tuple<FieldReference, Block>> localInitValues = new List<Tuple<FieldReference,Block>>();
            List<Tuple<FieldReference, ParameterDefinition>> argMappings =
                new List<Tuple<FieldReference, ParameterDefinition>>();

            StackedBlock ctorStoreBlock = rootBlock.Children[0] as StackedBlock;
            MethodReference constructorReference = (MethodReference)
                ctorStoreBlock.GetInstructionAt(ctorStoreBlock.InstructionCount - 2).OpCodeArgument;

            TypeDefinition typeDefinition = constructorReference.DeclaringType.Resolve();

            if (typeDefinition.CustomAttributes.SelectAttribute(
                ctorStoreBlock.Context.ClrContext.KnownReferences.CompilerGeneratedAttribute) == null)
            {
                // This does not seem like initializer for inline delegate class.
                return;
            }

            for (int setterIndex = 1; setterIndex < lastIndex; setterIndex++)
            {
                StackedBlock setter = rootBlock.Children[setterIndex] as StackedBlock;

                if (setter == null
                    || setter.RootBlock.Instruction.OpCode != OpCode.StoreField)
                {
                    // This does not seem to be like DelegateClass initializer.
                    return;
                }

                FieldReference fieldReference = (FieldReference)
                    setter.RootBlock.Instruction.OpCodeArgument;
                fieldsUsed.Add(fieldReference.Name);

                // Let's check the value that's been used. This will be used to determine if this
                // is using argument as field or not, else this will be used to determine init value
                // of local variable.
                InstructionBlock argSetterValue = setter.GetDependent(1) as InstructionBlock;

                if (argSetterValue != null
                    && argSetterValue.Instruction.OpCode == OpCode.LoadArgument)
                {
                    if (!argumentsUsed.Contains((ParameterDefinition)argSetterValue.Instruction.OpCodeArgument))
                    {
                        argMappings.Add(
                            Tuple.Create(
                                fieldReference,
                                (ParameterDefinition)argSetterValue.Instruction.OpCodeArgument));

                        continue;
                    }
                }

                localInitValues.Add(
                    Tuple.Create(
                        fieldReference,
                        (Block)setter.GetDependent(1)));
            }

            this.argumentMappings = argMappings;

            foreach (var field in typeDefinition.Fields)
            {
                if (!fieldsUsed.Contains(field.Name))
                {
                    this.localMappings.Add(
                        Tuple.Create(
                            (FieldReference)field,
                            field.Name,
                            (Block)null));
                }
            }

            foreach (var tuple in localInitValues)
            {
                this.localMappings.Add(
                    Tuple.Create(
                        tuple.Item1,
                        tuple.Item1.Name,
                        tuple.Item2));
            }

            this.functionStartIndex = lastIndex;
            this.delegateClass = constructorReference.DeclaringType;
        }
    }
}
