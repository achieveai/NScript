namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// JumpType enum for JumpBlock.
    /// </summary>
    internal enum JumpType
    {
        /// <summary>
        /// Break statement.
        /// </summary>
        Break,

        /// <summary>
        /// Continue statement.
        /// </summary>
        Continue,

        /// <summary>
        /// Goto.
        /// This is not used for time being.
        /// </summary>
        Goto
    }

    /// <summary>
    /// JumpBlock is used to mark branches that signify break, continue or goto statements in C#
    /// </summary>
    class JumpBlock : Block
    {
        /// <summary>
        /// Backing field for jumptype.
        /// </summary>
        private readonly JumpType jumpType;

        /// <summary>
        /// Initializes a new instance of the <see cref="JumpBlock"/> class.
        /// </summary>
        /// <param name="branchBlock">The branch block.</param>
        /// <param name="jumpType">Type of the jump.</param>
        internal JumpBlock(
            BasicBlock branchBlock,
            JumpType jumpType)
            :base(new Block[]{branchBlock})
        {
            var instruction = branchBlock.GetInstructionAt(
                branchBlock.BlockEndIndex - branchBlock.BlockStartIndex);

            if (instruction.FlowType != FlowType.Branch
                && instruction.FlowType != FlowType.ConditionalBranch
                && instruction.FlowType != FlowType.Leave)
            {
                throw new ArgumentException("branchBlock");
            }
            this.jumpType = jumpType;
        }

        /// <summary>
        /// Gets the type of the jump.
        /// </summary>
        /// <value>The type of the jump.</value>
        public JumpType JumpType
        {
            get
            {
                return this.jumpType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is conditional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is conditional; otherwise, <c>false</c>.
        /// </value>
        public bool IsConditional
        {
            get
            {
                return this.Children[0] is StackedBlock;
            }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.Statement returnValue;

            if (this.JumpType == JumpType.Break)
            {
                returnValue = new AST.BreakStatement(
                    this.Context.ClrContext,
                    this.ComputeLocation());
            }
            else if (this.JumpType == JumpType.Continue)
            {
                returnValue = new AST.ContinueStatement(
                    this.Context.ClrContext,
                    this.ComputeLocation());
            }
            else
            {
                if (this.IsHandlerEndLeave())
                {
                    return null;
                }

                Block returnBlock = this.GetReturnBlock();
                if (returnBlock != null)
                {
                    return (AST.Statement)returnBlock.ToAstNode(variableResolver);
                }
                else if (this.Successors[0].GetInstructionAt(0).Index == this.GetInstructionAt(0).Index + 1)
                {
                    // this is equivalent to NoOp, let's ignore this.
                    return null;
                }
                else
                {
                    throw new NotSupportedException("Goto is not supported");
                }
            }

            if (this.Children[0] is StackedBlock)
            {
                List<AST.Statement> ifBlock = new List<AST.Statement>();
                ifBlock.Add(returnValue);

                AST.ScopeBlock trueBlock = new AST.ScopeBlock(returnValue.Context, returnValue.Location);
                trueBlock.AddStatement(returnValue);

                // This means that we have if (...) break/continue;
                returnValue = new AST.IfBlockStatement(
                    this.Context.ClrContext,
                    returnValue.Location,
                    new AST.UnaryExpression(
                        this.Context.ClrContext,
                        returnValue.Location,
                        (AST.Expression)this.Children[0].ToAstNode(variableResolver),
                        AST.UnaryOperator.LogicalNot),
                    trueBlock,
                    null);
            }

            return returnValue;
        }

        /// <summary>
        /// Determines whether is handler end leave.
        /// </summary>
        /// <returns>
        /// <c>true</c> if is handler end leave; otherwise, <c>false</c>.
        /// </returns>
        public bool IsHandlerEndLeave()
        {
            if (this.JumpType != Blocks.JumpType.Goto)
            {
                return false;
            }

            IlInstruction instruction = this.GetInstructionAt(0);
            if (instruction.OpCode == OpCode.Leave)
            {
                foreach (var handler in this.Context.MethodDefinition.Body.ExceptionHandlers)
                {
                    // To match with EndBlock, we need to move to next Instruction.
                    IlInstruction nextInstruction = instruction.Next;
                    if (nextInstruction != null
                        &&( nextInstruction.Label == handler.TryEnd.Offset.ToString()
                            || nextInstruction.Label == handler.HandlerEnd.Offset.ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the return block.
        /// </summary>
        /// <returns></returns>
        public Block GetReturnBlock()
        {
            if (this.jumpType != Blocks.JumpType.Goto)
            {
                return null;
            }

            // Let's check if this is leave statement with leave outside of the block.
            Block gotoSuccessor = this.Successors[this.Successors.Count - 1];

            while (gotoSuccessor is InstructionBlock
                && gotoSuccessor.GetInstructionAt(0).OpCode == OpCode.Nop)
            {
                gotoSuccessor = gotoSuccessor.Successors[0];
            }

            while (gotoSuccessor.StackAfter != 0)
            {
                gotoSuccessor = gotoSuccessor.Parent;
            }

            if ((gotoSuccessor is StackedBlock
                    && ((StackedBlock)gotoSuccessor).RootBlock.Instruction.OpCode == OpCode.Return)
                || (gotoSuccessor is InstructionBlock
                    && ((InstructionBlock)gotoSuccessor).Instruction.OpCode == OpCode.Return))
            {
                return gotoSuccessor;
            }

            return null;
        }
    }
}
