using System;
using System.Collections.Generic;
using Cs2JsC.Lib.AsmDeasm.Ops;
using Cs2JsC.Lib.AsmDeasm.Ast;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    class InstructionBlock : BasicBlock
    {
        #region member variables
        bool nextFlowInstructionDirty = true;
        bool previousFlowInstructionDirty = true;

        List<InstructionBlock> previousFlowInstructions = new List<InstructionBlock>();
        List<InstructionBlock> nextFlowInstructions = new List<InstructionBlock>();
        #endregion

        #region constructors/Factories
        public InstructionBlock(
            BlockContext context,
            int instructionIndex)
            : base(context)
        {
            this.Instruction = context.Instructions[instructionIndex];
        }

        internal InstructionBlock(
            BlockContext blockContext,
            Instruction instruction)
            : base(blockContext)
        {
            this.Instruction = instruction;

            if (instruction.Previous == null &&
                instruction.Next == null)
            {
                if (instruction.Index < this.Context.Instructions.Count &&
                    instruction == this.Context.Instructions[instruction.Index])
                {
                    throw new InvalidOperationException();
                }

                if (instruction.Index > 0)
                {
                    instruction.Previous = this.Context.Instructions[instruction.Index - 1];
                    instruction.Previous.Next = instruction;

                    this.Context.InstructionToBlock[instruction.Previous].NextFlowInstructionsDirty = true;
                }
                if (instruction.Index < this.Context.Instructions.Count - 1)
                {
                    instruction.Next = this.Context.Instructions[instruction.Index];
                    instruction.Next.Previous = instruction;

                    this.Context.InstructionToBlock[instruction.Next].PreviousFlowInstructionDirty = true;
                }

                for (int i = instruction.Index; i < this.Context.Instructions.Count; i++)
                {
                    this.Context.Instructions[i].Index++;
                }

                this.Context.LabelToInstruction.Add(
                    instruction.Label,
                    instruction);

                this.Context.Instructions.Insert(
                    instruction.Index,
                    instruction);

                this.Context.InstructionToBlock[instruction] = this;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Instruction Instruction
        {
            get;
            private set;
        }

        public override int StackBefore
        { get { return Instruction.StackBefore; } }

        public override int StackAfter
        { get { return this.Instruction.StackAfter; } }

        public override int BlockStartIndex
        {
            get
            {
                return this.Instruction.Index;
            }
        }

        public override int BlockEndIndex
        {
            get
            {
                return this.BlockStartIndex;
            }
        }

        public List<InstructionBlock> PreviousInstructions
        {
            get
            {
                if (this.PreviousFlowInstructionDirty)
                {
                    this.ComputePreviousFlowInstructions();
                    this.PreviousFlowInstructionDirty = false;
                }

                return this.previousFlowInstructions;
            }
        }

        public List<InstructionBlock> NextInstructions
        {
            get
            {
                if (this.NextFlowInstructionsDirty)
                {
                    this.ComputeNextFlowInstructionBlocks();
                    this.NextFlowInstructionsDirty = false;
                }

                return this.nextFlowInstructions;
            }
        }
        #endregion

        #region public functions
        public override string ToString()
        {
            return this.Instruction.ToString();
        }

        public override void DeleteInstruction(int instructionIndex)
        {
            if (this.BlockStartIndex == instructionIndex)
            {
                if (this.ContainsBranchPredecossor())
                {
                    throw new InvalidOperationException("Can't delete instruction with branch predecessor");
                }

                if (this.Instruction.Previous != null)
                {
                    this.Instruction.Previous.Next = this.Instruction.Next;
                }
                if (this.Instruction.Next != null)
                {
                    this.Instruction.Next.Previous = this.Instruction.Previous;
                }

                for (int i = instructionIndex; i < this.Context.Instructions.Count; i++)
                {
                    this.Context.Instructions[i].Index--;
                }

                this.Context.LabelToInstruction.Remove(this.Instruction.Label);
                this.Context.InstructionToBlock.Remove(this.Instruction);
                this.Context.Instructions.RemoveAt(instructionIndex);

                base.Dissolve();
            }
        }

        /// <summary>
        /// Dissolve is NoOp since this instruction does not have any children to move to level up.
        /// </summary>
        public override void Dissolve()
        { }

        public override IList<IAstNode> ToExpressions(
            ExecutionContext executionContext,
            Stack<Expression> expressionStack)
        {
            Ast.Expression exp = null;

            if (expressionStack.Count > 0 &&
                expressionStack.Peek() is TemporaryAssignement)
            {
                switch (this.Instruction.CodeOp.Code)
                {
                    case IlOpCode.LoadLocal:
                    case IlOpCode.LoadLocal0:
                    case IlOpCode.LoadLocal1:
                    case IlOpCode.LoadLocal2:
                    case IlOpCode.LoadLocal3:
                        exp = expressionStack.Pop().DependentExpressions[0];
                        expressionStack.Push(exp);
                        return new[] { exp };
                    default:
                        throw new InvalidOperationException("Can't get to this case with only TempAssignement on the stack");
                }
            }

            switch (this.Instruction.CodeOp.Code)
            {
                case IlOpCode.Add:
                case IlOpCode.AddOvfSigned:
                case IlOpCode.AddOvfUnsigned:
                case IlOpCode.And:
                case IlOpCode.BranchIfEqual:
                case IlOpCode.BranchIfGreaterOrEqual:
                case IlOpCode.BranchIfGreaterOrEqualUnsigned:
                case IlOpCode.BranchIfGreater:
                case IlOpCode.BranchIfGreaterUnsigned:
                case IlOpCode.BranchIfLessOrEqual:
                case IlOpCode.BranchIfLessOrEqualUnsigned:
                case IlOpCode.BranchIfLessThan:
                case IlOpCode.BranchIfLessThanUnsigned:
                case IlOpCode.BranchIfNotEqualsUnsigned:
                case IlOpCode.CheckEquals:
                case IlOpCode.CheckGreater:
                case IlOpCode.CheckGreaterUnsigned:
                case IlOpCode.Ckfinite:
                case IlOpCode.CheckLesser:
                case IlOpCode.CheckLesserUnsigned:
                case IlOpCode.Div:
                case IlOpCode.DivUnsigned:
                case IlOpCode.Mul:
                case IlOpCode.MulOvf:
                case IlOpCode.Or:
                case IlOpCode.Remainder:
                case IlOpCode.RemainderUnsigned:
                case IlOpCode.ShiftLeft:
                case IlOpCode.ShiftRight:
                case IlOpCode.ShiftRightUnsigned:
                case IlOpCode.Sub:
                case IlOpCode.SubOvf:
                case IlOpCode.Xor:
                    exp = new BinaryExpression(
                        executionContext,
                        this,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            2));
                    expressionStack.Push(exp);
                    break;
                case IlOpCode.Arglist:
                    break;
                case IlOpCode.Branch:
                    break;
                case IlOpCode.Break:
                    break;
                case IlOpCode.BranchIfFalse:
                case IlOpCode.BranchIfTrue:
                    exp = new UnaryExpression(
                        executionContext,
                        this,
                        expressionStack.Pop());

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.Call:
                    exp = new MethodCall(
                        executionContext,
                        this,
                        false,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            this.Instruction.GetPops()));

                    if (!exp.IsStatement)
                    {
                        expressionStack.Push(exp);
                    }
                    break;
                case IlOpCode.Calli:
                    exp = new MethodCall(
                        executionContext,
                        this,
                        false,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            this.Instruction.GetPops()));

                    if (!exp.IsStatement)
                    {
                        expressionStack.Push(exp);
                    }
                    break;
                case IlOpCode.Callvirt:
                    exp = new MethodCall(
                        executionContext,
                        this,
                        true,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            this.Instruction.GetPops()));

                    if (!exp.IsStatement)
                    {
                        expressionStack.Push(exp);
                    }

                    break;
                case IlOpCode.ConvToByte:
                    break;
                case IlOpCode.ConvToShort:
                    break;
                case IlOpCode.CopyBulk:
                    break;
                case IlOpCode.Dup:
                    break;
                case IlOpCode.Endfilter:
                    break;
                case IlOpCode.Endfinally:
                    break;
                case IlOpCode.InitBulk:
                    break;
                case IlOpCode.Jmp:
                    break;
                case IlOpCode.LoadArgument:
                case IlOpCode.LoadArgument0:
                case IlOpCode.LoadArgument1:
                case IlOpCode.LoadArgument2:
                case IlOpCode.LoadArgument3:
                case IlOpCode.LoadArgumentAddress:
                    exp = new LoadArgument(
                        executionContext,
                        this);
                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadConstantInt:
                case IlOpCode.LoadConstantInt0:
                case IlOpCode.LoadConstantInt1:
                case IlOpCode.LoadConstantInt2:
                case IlOpCode.LoadConstantInt3:
                case IlOpCode.LoadConstantInt4:
                case IlOpCode.LoadConstantInt5:
                case IlOpCode.LoadConstantInt6:
                case IlOpCode.LoadConstantInt7:
                case IlOpCode.LoadConstantInt8:
                case IlOpCode.LoadConstantIntNeg1:
                case IlOpCode.LoadConstantLong:
                case IlOpCode.LoadConstantFloat:
                case IlOpCode.LoadConstantDouble:
                case IlOpCode.LoadNull:
                case IlOpCode.LoadString:
                    exp = new LoadConstant(
                        executionContext,
                        this);
                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadVirtualFunction:
                    exp = new LoadMethod(
                        executionContext,
                        this,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            1));

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadMethodPointer:
                    exp = new LoadMethod(
                        executionContext,
                        this);

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadIndirect:
                case IlOpCode.LoadObject:
                    exp = new LoadFromReference(
                        executionContext,
                        this,
                        expressionStack.Pop());

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadLocal:
                case IlOpCode.LoadLocal0:
                case IlOpCode.LoadLocal1:
                case IlOpCode.LoadLocal2:
                case IlOpCode.LoadLocal3:
                case IlOpCode.LoadLocalAddress:
                case IlOpCode.LoadLocalAddress0:
                case IlOpCode.LoadLocalAddress1:
                case IlOpCode.LoadLocalAddress2:
                case IlOpCode.LoadLocalAddress3:
                    exp = new LoadLocalVariable(
                        executionContext,
                        this);
                    expressionStack.Push(exp);
                    break;
                case IlOpCode.Leave:
                    break;
                case IlOpCode.Localalloc:
                    break;
                case IlOpCode.Neg:
                    break;
                case IlOpCode.Nop:
                    break;
                case IlOpCode.Not:
                    break;
                case IlOpCode.Pop:
                    exp = expressionStack.Pop();
                    exp.IsStatement = true;
                    break;
                case IlOpCode.Return:
                    if (expressionStack.Count > 0)
                    {
                        exp = new Return(
                            executionContext,
                            this,
                            expressionStack.Pop());
                    }
                    else
                    {
                        exp = new Return(
                            executionContext,
                            this);
                    }
                    break;
                case IlOpCode.StoreArgument:
                    exp = new ArgumentAssignment(
                        executionContext,
                        this,
                        expressionStack.Count == 1,
                        expressionStack.Pop());

                    break;
                case IlOpCode.StoreIndirect:
                case IlOpCode.StoreObject:
                    exp = new StoreToReference(
                        executionContext,
                        this,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            2));
                    break;
                case IlOpCode.StoreLocal:
                case IlOpCode.StoreLocal0:
                case IlOpCode.StoreLocal1:
                case IlOpCode.StoreLocal2:
                case IlOpCode.StoreLocal3:
                    if (this.Instruction.IsTempVariableStore)
                    {
                        exp = new TemporaryAssignement(
                            executionContext,
                            this,
                            expressionStack.Pop());

                        expressionStack.Push(exp);
                    }
                    else
                    {
                        exp = new LocalAssignment(
                            executionContext,
                            this,
                            expressionStack.Count == 1,
                            expressionStack.Pop());
                    }
                    break;
                case IlOpCode.Switch:
                    break;
                case IlOpCode.Box:
                    break;
                case IlOpCode.Castclass:
                    break;
                case IlOpCode.Cpobj:
                    break;
                case IlOpCode.Initobj:
                    break;
                case IlOpCode.Isinst:
                    break;
                case IlOpCode.LoadArrayElement:
                case IlOpCode.LoadArrayElementByte:
                case IlOpCode.LoadArrayElementShort:
                case IlOpCode.LoadArrayElementInt:
                case IlOpCode.LoadArrayElementLong:
                case IlOpCode.LoadArrayElementUnsignedByte:
                case IlOpCode.LoadArrayElementUnsignedShort:
                case IlOpCode.LoadArrayElementUnsignedInt:
                case IlOpCode.LoadArrayElementUnsignedLong:
                case IlOpCode.LoadArrayElementFloat:
                case IlOpCode.LoadArrayElementDouble:
                case IlOpCode.LoadArrayElementObject:
                case IlOpCode.LoadArrayElementAddress:
                    exp = new LoadArrayElement(
                        executionContext,
                        this,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            2));

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadField:
                case IlOpCode.LoadFieldAddress:
                    exp = new LoadField(
                        executionContext,
                        this,
                        expressionStack.Pop());
                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadStaticField:
                case IlOpCode.LoadStaticFieldAddress:
                    exp = new LoadField(
                        executionContext,
                        this);
                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadArrayLength:
                    exp = new LoadArrayLength(
                        executionContext,
                        this,
                        expressionStack.Pop());

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.LoadToken:
                    break;
                case IlOpCode.MakeReferenceAny:
                    break;
                case IlOpCode.NewArray:
                    exp = new NewArray(
                        executionContext,
                        this,
                        expressionStack.Pop());

                    expressionStack.Push(exp);
                    break;
                case IlOpCode.NewObject:
                    exp = new ObjectConstructor(
                        executionContext,
                        this,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            this.Instruction.GetPops()));

                    expressionStack.Push(exp);

                    break;
                case IlOpCode.Refanytype:
                    break;
                case IlOpCode.Refanyval:
                    break;
                case IlOpCode.Rethrow:
                    break;
                case IlOpCode.Sizeof:
                    break;
                case IlOpCode.StoreArrayElement:
                case IlOpCode.StoreByteArrayElement:
                case IlOpCode.StoreShortArrayElement:
                case IlOpCode.StoreIntArrayElement:
                case IlOpCode.StoreLongArrayElement:
                case IlOpCode.StoreFloatArrayElement:
                case IlOpCode.StoreDoubleArrayElement:
                case IlOpCode.StoreObjectArrayElement:
                    exp = new StoreArrayElement(
                        executionContext,
                        this,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            3))
                        {IsStatement = expressionStack.Count == 0};

                    break;
                case IlOpCode.StoreField:
                    exp = new FieldAssignement(
                        executionContext,
                        this,
                        expressionStack.Count == 2,
                        InstructionBlock.GetPopedExpressions(
                            expressionStack,
                            2));
                    break;
                case IlOpCode.StoreStaticField:
                    exp = new FieldAssignement(
                        executionContext,
                        this,
                        expressionStack.Count == 1,
                        expressionStack.Pop());
                    break;
                case IlOpCode.Throw:
                    break;
                case IlOpCode.Unbox:
                    break;
                case IlOpCode.UnboxAny:
                    break;
                default:
                    break;
            }

            if (exp != null)
            {
                return new[] { exp };
            }

            return null;
        }

        public static Expression[] GetPopedExpressions(
            Stack<Expression> stack,
            int pops)
        {
            Expression[] returnValue = new Expression[pops];

            for (int i = 0; i < pops; i++)
            {
                returnValue[pops - i - 1] = stack.Pop();
            }

            return returnValue;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        private bool NextFlowInstructionsDirty
        {
            get { return this.nextFlowInstructionDirty; }
            set
            {
                this.nextFlowInstructionDirty = value;
                if (this.nextFlowInstructionDirty)
                {
                    this.SetSuccessorDirty();
                }
            }
        }

        private bool PreviousFlowInstructionDirty
        {
            get { return this.previousFlowInstructionDirty; }
            set
            {
                this.previousFlowInstructionDirty = value;
                if (this.previousFlowInstructionDirty)
                {
                    this.SetPredecessorDirty();
                }
            }
        }
        #endregion

        #region private functions
        protected override void ComputeSuccessors()
        {
            base.ComputeSuccessors();

            foreach (var instructio in this.NextInstructions)
            {
                this.AddAdjecent(instructio, false);
            }
        }

        protected override void ComputePredecessors()
        {
            base.ComputePredecessors();

            foreach (var instruction in this.PreviousInstructions)
            {
                this.AddAdjecent(instruction, true);
            }
        }

        private void ComputeNextFlowInstructionBlocks()
        {
            this.nextFlowInstructions.Clear();

            if (this.Instruction.Next != null)
            {
                switch (this.Instruction.CodeOp.Flow)
                {
                    case FlowType.Branch:
                        this.nextFlowInstructions.Add(
                            this.Context.InstructionToBlock[
                                this.Context.LabelToInstruction[
                                (string)this.Instruction.OpCodeArgument]]);
                        break;
                    case FlowType.ConditionalBranch:
                        this.nextFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Instruction.Next]);
                        this.nextFlowInstructions.Add(
                            this.Context.InstructionToBlock[
                                this.Context.LabelToInstruction[
                                (string)this.Instruction.OpCodeArgument]]);
                        break;
                    case FlowType.Switch:
                        this.nextFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Instruction.Next]);
                        foreach (var label in (string[])this.Instruction.OpCodeArgument)
                        {
                            this.nextFlowInstructions.Add(
                                this.Context.InstructionToBlock[
                                    this.Context.LabelToInstruction[label]]);
                        }
                        break;
                    case FlowType.MethodCall:
                    case FlowType.Next:
                        this.nextFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Instruction.Next]);
                        break;
                }
            }
        }

        private void ComputePreviousFlowInstructions()
        {
            this.previousFlowInstructions.Clear();

            for (int i = 0; i < this.Context.Instructions.Count; i++)
            {
                if (this.Context.Instructions[i].CodeOp.Flow == FlowType.ConditionalBranch ||
                    this.Context.Instructions[i].CodeOp.Flow == FlowType.Branch)
                {
                    if (this.Instruction.Label.Equals(this.Context.Instructions[i].OpCodeArgument))
                    {
                        this.previousFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Context.Instructions[i]]);
                    }
                }
                else if (this.Context.Instructions[i].CodeOp.Flow == FlowType.Switch)
                {
                    if (((IList<string>)this.Context.Instructions[i].OpCodeArgument).IndexOf(this.Instruction.Label) >= 0)
                    {
                        this.previousFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Context.Instructions[i]]);
                    }
                }
            }

            if (this.Instruction.Previous != null)
            {
                switch (this.Instruction.Previous.CodeOp.Flow)
                {
                    case FlowType.ConditionalBranch:
                    case FlowType.MethodCall:
                    case FlowType.Next:
                        this.previousFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Instruction.Previous]);
                        break;
                }
            }
        }

        private bool ContainsBranchPredecossor()
        {
            for (int i = 0; i < this.PreviousInstructions.Count; i++)
            {
                if (this.PreviousInstructions[i].Instruction.CodeOp.Flow == FlowType.ConditionalBranch ||
                    this.PreviousInstructions[i].Instruction.CodeOp.Flow == FlowType.Branch)
                {
                    if (this.Instruction.Label.Equals(this.PreviousInstructions[i].Instruction.OpCodeArgument))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void WriteBodyToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine(
                "{0}.   {1}: {2} {3}",
                linePrefix,
                this.Instruction.Label,
                this.Instruction.CodeOp.Code,
                this.Instruction.OpCodeArgument);
        }
        #endregion
    }
}
