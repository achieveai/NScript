//-----------------------------------------------------------------------
// <copyright file="InlineAssignmentBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for InlineAssignmentBlock
    /// </summary>
    internal class InlineAssignmentBlock : BasicBlock
    {
        /// <summary>
        /// Backing field for value block
        /// </summary>
        private readonly BasicBlock valueBlock;

        /// <summary>
        /// Backing field for Setter block
        /// </summary>
        private readonly StackedBlock setterBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineAssignmentBlock"/> class.
        /// </summary>
        /// <param name="setterBlock">The setter block.</param>
        /// <param name="valueBlock">The value block.</param>
        /// <param name="childBlock">The child block.</param>
        public InlineAssignmentBlock(
            StackedBlock setterBlock,
            BasicBlock valueBlock,
            BasicBlock childBlock)
            : base(new BasicBlock[] { childBlock })
        {
            this.valueBlock = valueBlock;
            this.setterBlock = setterBlock;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineAssignmentBlock"/> class.
        /// </summary>
        /// <param name="setterBlock">The setter block.</param>
        /// <param name="valueBlock">The value block.</param>
        /// <param name="childBlocks">The child blocks.</param>
        public InlineAssignmentBlock(
            StackedBlock setterBlock,
            BasicBlock valueBlock,
            BasicBlock[] childBlocks)
            : base(childBlocks)
        {
            this.valueBlock = valueBlock;
            this.setterBlock = setterBlock;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public BasicBlock Value
        {
            get { return this.valueBlock; }
        }

        /// <summary>
        /// Gets the setter.
        /// </summary>
        /// <value>The setter.</value>
        public StackedBlock Setter
        {
            get { return this.setterBlock; }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            List<AST.Expression> arguments = new List<Expression>();

            for (int dependentIndex = 0; dependentIndex < this.Setter.DependentCount() - 1; dependentIndex++)
            {
                arguments.Add((Expression)this.Setter.GetDependent(dependentIndex).ToAstNode(variableResolver));
            }

            AST.Expression lhs;

            switch (OpCodeHelper.GetElementAccessType(this.Setter.RootBlock.Instruction))
            {
                case ElementAccessType.SetStaticProperty:
                    {
                        MethodReference methodReference =
                            (MethodReference)this.Setter.RootBlock.Instruction.OpCodeArgument;

                        lhs = new AST.PropertyReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            new InternalPropertyReference(
                                null,
                                methodReference));
                    }
                    break;
                case ElementAccessType.SetProperty:
                    {
                        MethodReference methodReference =
                            (MethodReference)this.Setter.RootBlock.Instruction.OpCodeArgument;

                        AST.Expression objExp = arguments[0];
                        arguments.RemoveAt(0);

                        lhs = new AST.PropertyReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            new InternalPropertyReference(
                                null,
                                methodReference),
                            objExp,
                            arguments);
                    }
                    break;
                case ElementAccessType.SetStaticField:
                    lhs = new AST.FieldReferenceExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (FieldReference)this.Setter.RootBlock.Instruction.OpCodeArgument);
                    break;
                case ElementAccessType.SetField:
                    lhs = new AST.FieldReferenceExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (FieldReference)this.Setter.RootBlock.Instruction.OpCodeArgument,
                        arguments[0]);
                    break;
                case ElementAccessType.SetArray:
                    lhs = new AST.ArrayElementExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        arguments[0],
                        arguments[1]);
                    break;
                case ElementAccessType.SetReferenceArgument:
                    lhs = new AST.VariableAddressReference(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)this.Setter.RootBlock.Instruction.OpCodeArgument));
                    break;
                case ElementAccessType.SetArgument:
                    lhs = new AST.VariableReference(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)this.Setter.RootBlock.Instruction.OpCodeArgument));
                    break;
                case ElementAccessType.SetLocal:
                    lhs = new AST.VariableReference(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        variableResolver.ResolveLocal(
                            (VariableDefinition)this.Setter.RootBlock.Instruction.OpCodeArgument));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new AST.BinaryExpression(
                this.Context.ClrContext,
                this.ComputeLocation(),
                lhs,
                (AST.Expression)this.Value.ToAstNode(variableResolver),
                BinaryOperator.Assignment);
        }
    }
}