//-----------------------------------------------------------------------
// <copyright file="OpAssignmentBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for OpAssignmentBlock
    /// </summary>
    internal class OpAssignmentBlock : BasicBlock
    {
        /// <summary>
        /// Backing field for Setter
        /// </summary>
        private StackedBlock setter;

        /// <summary>
        /// Backing field for OpBlock
        /// </summary>
        private StackedBlock opBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpAssignmentBlock"/> class.
        /// </summary>
        /// <param name="childBlock">The child block.</param>
        public OpAssignmentBlock(
            StackedBlock setter,
            StackedBlock opBlock,
            BasicBlock childBlock)
            : base(new BasicBlock[] { childBlock })
        {
            this.setter = setter;
            this.opBlock = opBlock;
        }

        /// <summary>
        /// Gets the setter.
        /// </summary>
        public StackedBlock Setter
        {
            get { return this.setter; }
        }

        /// <summary>
        /// Gets the op block.
        /// </summary>
        public StackedBlock OpBlock
        { get { return this.opBlock; } }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <returns>BinaryOperator if correct operator exists else null</returns>
        public static BinaryOperator? GetOperator(OpCode opCode)
        {
            switch (opCode)
            {
                case OpCode.Add:
                case OpCode.Add_ovf:
                case OpCode.Add_ovf_un:
                    return BinaryOperator.PlusAssignment;
                case OpCode.Sub:
                case OpCode.Sub_ovf:
                case OpCode.Sub_ovf_un:
                    return BinaryOperator.MinusAssignment;
                case OpCode.Mul:
                case OpCode.Mul_ovf:
                case OpCode.Mul_ovf_un:
                    return BinaryOperator.MulAssignment;
                case OpCode.Div:
                case OpCode.Div_un:
                    return BinaryOperator.DivAssignment;
                case OpCode.Rem:
                case OpCode.Rem_un:
                    return BinaryOperator.ModAssignment;
                case OpCode.And:
                    return BinaryOperator.BitwiseAndAssignment;
                case OpCode.Or:
                    return BinaryOperator.BitwiseOrAssignment;
                case OpCode.Xor:
                    return BinaryOperator.BitwiseXorAssignment;
                case OpCode.Shl:
                    return BinaryOperator.LeftShiftAssignment;
                case OpCode.Shr:
                    return BinaryOperator.RightShiftAssignment;
                case OpCode.Shr_un:
                    return BinaryOperator.UnsignedRightShiftAssignment;
            }

            return null;
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            List<Block> arguments = PostfixBlockBuilder.AreReciprocatingOperations(
                this.OpBlock.Children[0],
                this.Setter);
            Location location = this.ComputeLocation();
            AST.BinaryOperator effectiveOperator = OpAssignmentBlock.GetOperator(this.OpBlock.RootBlock.Instruction.OpCode).Value;

            if (effectiveOperator == BinaryOperator.PlusAssignment
                || effectiveOperator == BinaryOperator.MinusAssignment)
            {
                InstructionBlock instructionBlock = opBlock.GetDependent(1) as InstructionBlock;
                if (instructionBlock != null
                    && instructionBlock.Instruction.OpCode == OpCode.LoadConstantInt32)
                {
                    int constValue = (int)instructionBlock.Instruction.OpCodeArgument;
                    if (constValue == 1 || constValue == -1)
                    {
                        return new UnaryExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            PostfixBlock.GetLhs(
                                location,
                                arguments,
                                this.Setter,
                                variableResolver),
                            (constValue == -1) == (effectiveOperator == BinaryOperator.PlusAssignment)
                                ? UnaryOperator.PreDecrement
                                : UnaryOperator.PreIncrement);
                    }
                }
            }

            return new BinaryExpression(
                this.Context.ClrContext,
                this.ComputeLocation(),
                PostfixBlock.GetLhs(
                    location,
                    arguments,
                    this.Setter,
                    variableResolver),
                (AST.Expression)opBlock.GetDependent(1).ToAstNode(variableResolver),
                effectiveOperator);
        }
    }
}
