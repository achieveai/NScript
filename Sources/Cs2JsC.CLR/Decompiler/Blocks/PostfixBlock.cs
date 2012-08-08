//-----------------------------------------------------------------------
// <copyright file="PostfixBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Utils;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for PostfixBlock
    /// </summary>
    internal class PostfixBlock : BasicBlock
    {
        /// <summary>
        /// Getter block.
        /// </summary>
        private readonly Block getterBlock;

        /// <summary>
        /// Backing field for Setter.
        /// </summary>
        private readonly StackedBlock setterBlock;

        /// <summary>
        /// Backing field for arguments.
        /// </summary>
        private readonly List<Block> arguments;

        /// <summary>
        /// Backing field for midValue.
        /// </summary>
        private readonly InstructionBlock midValue;

        /// <summary>
        /// Backing field for increment.
        /// </summary>
        private readonly bool isIncrement;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostfixBlock"/> class.
        /// </summary>
        /// <param name="getterBlock">The getter block.</param>
        /// <param name="setterBlock">The setter block.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="midValue">The mid value.</param>
        /// <param name="isIncrement">if set to <c>true</c> [is increment].</param>
        /// <param name="childBlock">The child block.</param>
        public PostfixBlock(
            Block getterBlock,
            StackedBlock setterBlock,
            List<Block> arguments,
            InstructionBlock midValue,
            bool isIncrement,
            BasicBlock childBlock)
            : base(new BasicBlock[] { childBlock })
        {
            this.getterBlock = getterBlock;
            this.setterBlock = setterBlock;
            this.arguments = arguments;
            this.midValue = midValue;
            this.isIncrement = isIncrement;
        }

        /// <summary>
        /// Gets the getter.
        /// </summary>
        /// <value>The getter.</value>
        public Block Getter
        {
            get { return this.getterBlock; }
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
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public List<Block> Arguments
        {
            get { return this.arguments; }
        }

        /// <summary>
        /// Gets the mid value.
        /// </summary>
        /// <value>The mid value.</value>
        public InstructionBlock MidValue
        {
            get { return midValue; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is increment.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is increment; otherwise, <c>false</c>.
        /// </value>
        public bool IsIncrement
        {
            get { return this.isIncrement; }
        }

        /// <summary>
        /// Gets the LHS.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="setterBlock">The setter block.</param>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>Returns expression for LHS for any of opAssignment things</returns>
        public static AST.Expression GetLhs(
            Location location,
            List<Block> arguments,
            StackedBlock setterBlock,
            VariableResolver variableResolver)
        {
            ElementAccessType setAccessType = OpCodeHelper.GetElementAccessType(setterBlock.RootBlock.Instruction);
            List<Expression> expressions = new List<Expression>();

            for (int argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
            {
                expressions.Add((Expression)arguments[argumentIndex].ToAstNode(variableResolver));
            }

            switch (setAccessType)
            {
                case ElementAccessType.SetStaticProperty:
                    {
                        MethodReference setter = (MethodReference)setterBlock.RootBlock.Instruction.OpCodeArgument;
                        MethodDefinition setterDef = setter.Resolve();
                        PropertyDefinition propertyDefinition = (PropertyDefinition)setterDef.GetPropertyDefinition();

                        return new PropertyReferenceExpression(
                            setterBlock.Context.ClrContext,
                            location,
                            new InternalPropertyReference(null, setter));
                    }
                case ElementAccessType.SetProperty:
                    {
                        MethodReference setter = (MethodReference)setterBlock.RootBlock.Instruction.OpCodeArgument;
                        MethodDefinition setterDef = setter.Resolve();
                        PropertyDefinition propertyDefinition =
                            (PropertyDefinition)setterDef.GetPropertyDefinition();

                        Expression leftExpression = expressions[0];
                        expressions.RemoveAt(0);

                        return new PropertyReferenceExpression(
                            leftExpression.Context,
                            location,
                            new InternalPropertyReference(null, setter),
                            leftExpression,
                            expressions);
                    }
                case ElementAccessType.SetStaticField:
                    return new FieldReferenceExpression(
                        setterBlock.Context.ClrContext,
                        location,
                        (FieldReference)setterBlock.RootBlock.Instruction.OpCodeArgument);
                case ElementAccessType.SetField:
                    {
                        Expression leftExpression;
                        leftExpression = expressions[0];
                        expressions.RemoveAt(0);

                        return new FieldReferenceExpression(
                            leftExpression.Context,
                            location,
                            (FieldReference)setterBlock.RootBlock.Instruction.OpCodeArgument,
                            leftExpression);
                    }
                case ElementAccessType.SetArray:
                    return new ArrayElementExpression(
                        setterBlock.Context.ClrContext,
                        location,
                        expressions[0],
                        expressions[1]);
                case ElementAccessType.SetReferenceArgument:
                    return new VariableAddressReference(
                        setterBlock.Context.ClrContext,
                        location,
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)arguments[0].GetInstructionAt(0).OpCodeArgument));
                case ElementAccessType.SetArgument:
                    return new VariableReference(
                        setterBlock.Context.ClrContext,
                        location,
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)setterBlock.RootBlock.Instruction.OpCodeArgument));
                case ElementAccessType.SetLocal:
                    return new VariableReference(
                        setterBlock.Context.ClrContext,
                        location,
                        variableResolver.ResolveLocal(
                            (VariableDefinition)setterBlock.RootBlock.Instruction.OpCodeArgument));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            StackedBlock getterStackedBlock = this.Getter as StackedBlock;
            Location location = this.ComputeLocation();

            Expression returnValue = new UnaryExpression(
                this.Context.ClrContext,
                location,
                PostfixBlock.GetLhs(
                    location,
                    this.Arguments,
                    this.Setter,
                    variableResolver),
                this.isIncrement ? UnaryOperator.PostIncrement : UnaryOperator.PostDecrement);

            if (this.MidValue != null)
            {
                returnValue = new BinaryExpression(
                    this.Context.ClrContext,
                    location,
                    new VariableReference(
                        this.Context.ClrContext,
                        location,
                        variableResolver.ResolveLocal(
                            (VariableDefinition)this.MidValue.Instruction.OpCodeArgument)),
                    returnValue,
                    BinaryOperator.Assignment);
            }

            return returnValue;
        }
    }
}