
namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using NScript.CLR.AST;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

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
            IlInstruction instruction)
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
        public IlInstruction Instruction
        {
            get;
            private set;
        }

        public override int StackBefore
        { get { return this.Instruction.StackLevelBefore; } }

        public override int StackAfter
        { get { return this.Instruction.StackLevelAfter; } }

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
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}, Instruction: ({1})",
                base.ToString(),
                this.Instruction.ToString());
        }

        /// <summary>
        /// Deletes the instruction.
        /// </summary>
        /// <param name="instructionIndex">Index of the instruction.</param>
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

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            switch (this.Instruction.OpCode)
            {
                case OpCode.Nop:
                    return null;
                case OpCode.Break:
                    return new DebuggerBreakStatement(
                        this.Context.ClrContext,
                        this.Instruction.Location);
                case OpCode.LoadArgument_0:
                case OpCode.LoadArgument_1:
                case OpCode.LoadArgument_2:
                case OpCode.LoadArgument_3:
                case OpCode.LoadLocal_0:
                case OpCode.LoadLocal_1:
                case OpCode.LoadLocal_2:
                case OpCode.LoadLocal_3:
                case OpCode.StoreLocal_0:
                case OpCode.StoreLocal_1:
                case OpCode.StoreLocal_2:
                case OpCode.StoreLocal_3:
                    break;
                case OpCode.LoadNull:
                    return new NullLiteral(
                        this.Context.ClrContext,
                        this.Instruction.Location);
                case OpCode.LoadConstantInt32_m1:
                case OpCode.LoadConstantInt32_0:
                case OpCode.LoadConstantInt32_1:
                case OpCode.LoadConstantInt32_2:
                case OpCode.LoadConstantInt32_3:
                case OpCode.LoadConstantInt32_4:
                case OpCode.LoadConstantInt32_5:
                case OpCode.LoadConstantInt32_6:
                case OpCode.LoadConstantInt32_7:
                case OpCode.LoadConstantInt32_8:
                case OpCode.LoadConstantInt32_s:
                    break;
                case OpCode.LoadConstantInt32:
                    return new IntLiteral(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (int)this.Instruction.OpCodeArgument);
                case OpCode.LoadConstantInt64:
                    return new IntLiteral(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (long)this.Instruction.OpCodeArgument);
                case OpCode.LoadConstantSingle:
                    return new DoubleLiteral(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (float)this.Instruction.OpCodeArgument);
                case OpCode.LoadConstantDouble:
                    return new DoubleLiteral(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (double)this.Instruction.OpCodeArgument);
                case OpCode.Dup:
                case OpCode.Pop:
                case OpCode.Jmp:
                case OpCode.Call:
                    {
                        MethodReference methodReference = (MethodReference) this.Instruction.OpCodeArgument;
                        MethodDefinition methodDefinition = methodReference.Resolve();
                        PropertyDefinition propertyDefinition = methodDefinition.GetPropertyDefinition();

                        if (propertyDefinition != null)
                        {
                            if (!methodDefinition.IsStatic ||
                                propertyDefinition.GetMethod != methodDefinition)
                            {
                                throw new InvalidOperationException("Can't have anything else than StaticPropertyGetter.");
                            }

                            return new PropertyReferenceExpression(
                                this.Context.ClrContext,
                                this.ComputeLocation(),
                                new InternalPropertyReference(
                                    methodReference,
                                    null));
                        }

                        return new MethodCallExpression(
                            this.Context.ClrContext,
                            this.Instruction.Location,
                            new MethodReferenceExpression(
                                this.Context.ClrContext,
                                this.Instruction.Location,
                                (MethodReference)this.Instruction.OpCodeArgument));
                    }
                case OpCode.CallIndirect:
                    break;
                case OpCode.Return:
                    return new ReturnStatement(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        null);
                case OpCode.Branch_s:
                case OpCode.BranchFalse_s:
                case OpCode.BranchTrue_s:
                case OpCode.BranchEq_s:
                case OpCode.BranchGe_s:
                case OpCode.BranchGt_s:
                case OpCode.BranchLe_s:
                case OpCode.BranchLt_s:
                case OpCode.BranchUNe_s:
                case OpCode.BranchUGe_s:
                case OpCode.BranchUGt_s:
                case OpCode.BranchULe_s:
                case OpCode.BranchULt_s:
                case OpCode.Branch:
                case OpCode.BranchFalse:
                case OpCode.BranchTrue:
                case OpCode.BranchEq:
                case OpCode.BranchGe:
                case OpCode.BranchGt:
                case OpCode.BranchLe:
                case OpCode.BranchLt:
                case OpCode.BranchUNe:
                case OpCode.BranchUGe:
                case OpCode.BranchUGt:
                case OpCode.BranchULe:
                case OpCode.BranchULt:
                case OpCode.Switch:
                case OpCode.LoadIndirectInt8:
                case OpCode.LoadIndirectUInt8:
                case OpCode.LoadIndirectInt16:
                case OpCode.LoadIndirectUInt16:
                case OpCode.LoadIndirectInt32:
                case OpCode.LoadIndirectUInt32:
                case OpCode.LoadIndirectInt64:
                case OpCode.LoadIndirectIntPtr:
                case OpCode.LoadIndirectSingle:
                case OpCode.LoadIndirectDouble:
                case OpCode.LoadIndirect_ref:
                case OpCode.StoreIndirect_ref:
                case OpCode.StoreIndirectInt8:
                case OpCode.StoreIndirectInt16:
                case OpCode.StoreIndirectInt32:
                case OpCode.StoreIndirectInt64:
                case OpCode.StoreIndirectSingle:
                case OpCode.StoreIndirectDouble:
                case OpCode.Add:
                case OpCode.Sub:
                case OpCode.Mul:
                case OpCode.Div:
                case OpCode.Div_un:
                case OpCode.Rem:
                case OpCode.Rem_un:
                case OpCode.And:
                case OpCode.Or:
                case OpCode.Xor:
                case OpCode.Shl:
                case OpCode.Shr:
                case OpCode.Shr_un:
                case OpCode.Neg:
                case OpCode.Not:
                case OpCode.ConvertToInt8:
                case OpCode.ConvertToInt16:
                case OpCode.ConvertToInt32:
                case OpCode.ConvertToInt64:
                case OpCode.ConvertToSingle:
                case OpCode.ConvertToDouble:
                case OpCode.ConvertToUInt32:
                case OpCode.ConvertToUInt64:
                case OpCode.CallVirtual:
                case OpCode.CopyObject:
                case OpCode.LoadObject:
                    break;
                case OpCode.LoadString:
                    return new StringLiteral(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (string)this.Instruction.OpCodeArgument);
                case OpCode.Newobj:
                    return new NewObjectExpression(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (MethodReference)this.Instruction.OpCodeArgument);
                case OpCode.Castclass:
                case OpCode.Isinst:
                case OpCode.Conv_r_un:
                case OpCode.Unbox:
                case OpCode.Throw:
                case OpCode.LoadField:
                case OpCode.LoadFieldAddress:
                    break;
                case OpCode.LoadStaticField:
                    return new FieldReferenceExpression(
                            this.Context.ClrContext,
                        this.ComputeLocation(),
                        (FieldReference)this.Instruction.OpCodeArgument);
                case OpCode.LoadStaticFieldAddress:
                    return new LoadAddressExpression(
                            this.Context.ClrContext,
                            this.Instruction.Location,
                            new FieldReferenceExpression(
                                this.Context.ClrContext,
                                this.Instruction.Location,
                                (FieldReference) this.Instruction.OpCodeArgument));
                case OpCode.StoreField:
                case OpCode.StoreStaticField:
                case OpCode.StoreObject:
                case OpCode.ConvertUnsignedOverflowToInt8:
                case OpCode.ConvertUnsignedOverflowToInt16:
                case OpCode.ConvertUnsignedOverflowToInt32:
                case OpCode.ConvertUnsignedOverflowToInt64:
                case OpCode.ConvertUnsignedOverflowToUInt8:
                case OpCode.ConvertUnsignedOverflowToUInt16:
                case OpCode.ConvertUnsignedOverflowToUInt32:
                case OpCode.ConvertUnsignedOverflowToUInt64:
                case OpCode.ConvertUnsignedOverflowToIntPtr:
                case OpCode.ConvertUnsignedOverflowToUIntPtr:
                case OpCode.Box:
                case OpCode.Newarr:
                case OpCode.LoadLength:
                case OpCode.LoadElementAddress:
                case OpCode.LoadElementInt8:
                case OpCode.LoadElementUInt8:
                case OpCode.LoadElementInt16:
                case OpCode.LoadElementUInt16:
                case OpCode.LoadElementInt32:
                case OpCode.LoadElementUInt32:
                case OpCode.LoadElementInt64:
                case OpCode.LoadElementIntPtr:
                case OpCode.LoadElementSingle:
                case OpCode.LoadElementDouble:
                case OpCode.LoadElement_ref:
                case OpCode.StoreElementIntPtr:
                case OpCode.StoreElementInt8:
                case OpCode.StoreElementInt16:
                case OpCode.StoreElementInt32:
                case OpCode.StoreElementInt64:
                case OpCode.StoreElementSingle:
                case OpCode.StoreElementDouble:
                case OpCode.StoreElement_ref:
                case OpCode.LoadElement:
                case OpCode.StoreElement:
                case OpCode.Unbox_any:
                case OpCode.ConvertOverflowToInt8:
                case OpCode.ConvertOverflowToUInt8:
                case OpCode.ConvertOverflowToInt16:
                case OpCode.ConvertOverflowToUInt16:
                case OpCode.ConvertOverflowToInt32:
                case OpCode.ConvertOverflowToUInt32:
                case OpCode.ConvertOverflowToInt64:
                case OpCode.ConvertOverflowToUInt64:
                case OpCode.Refanyval:
                case OpCode.Ckfinite:
                case OpCode.Mkrefany:
                case OpCode.ConvertToUInt16:
                case OpCode.ConvertToUInt8:
                case OpCode.ConvertToIntPtr:
                case OpCode.ConvertOverflowToIntPtr:
                case OpCode.ConvertOverflowToUIntPtr:
                case OpCode.Add_ovf:
                case OpCode.Add_ovf_un:
                case OpCode.Mul_ovf:
                case OpCode.Mul_ovf_un:
                case OpCode.Sub_ovf:
                case OpCode.Sub_ovf_un:
                case OpCode.StoreIndirectIntPtr:
                case OpCode.Conv_u:
                case OpCode.Prefix7:
                case OpCode.Prefix6:
                case OpCode.Prefix5:
                case OpCode.Prefix4:
                case OpCode.Prefix3:
                case OpCode.Prefix2:
                case OpCode.Prefix1:
                case OpCode.Prefixref:
                case OpCode.Arglist:
                case OpCode.CompareEq:
                case OpCode.CompareGt:
                case OpCode.CompareUGt:
                case OpCode.CompareLt:
                case OpCode.CompareULt:
                    return new VariableReference(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)this.Instruction.OpCodeArgument));
                case OpCode.Ldtoken:
                    return new TypeReferenceExpression(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (TypeReference)this.Instruction.OpCodeArgument);
                case OpCode.LoadFunction:
                    return new MethodReferenceExpression(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (MethodReference)this.Instruction.OpCodeArgument);
                case OpCode.LoadVirtualFunction:
                case OpCode.LoadArgument_s:
                case OpCode.LoadArgument:
                    return new VariableReference(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)this.Instruction.OpCodeArgument));
                case OpCode.LoadArgumentAddress_s:
                case OpCode.LoadArgumentAddress:
                    return new LoadAddressExpression(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        new VariableReference(
                            this.Context.ClrContext,
                            this.Instruction.Location,
                            variableResolver.ResolveParameter(
                                (ParameterDefinition)this.Instruction.OpCodeArgument)));
                case OpCode.StoreArgument_s:
                case OpCode.StoreArgument:
                    break;
                case OpCode.LoadLocal_s:
                case OpCode.LoadLocal:
                    return new VariableReference(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        variableResolver.ResolveLocal(
                            (VariableDefinition)this.Instruction.OpCodeArgument));
                case OpCode.LoadLocalAddress_s:
                case OpCode.LoadLocalAddress:
                    return new LoadAddressExpression(
                            this.Context.ClrContext,
                            this.Instruction.Location,
                            new VariableReference(
                                this.Context.ClrContext,
                                this.Instruction.Location,
                                variableResolver.ResolveLocal(
                                    (VariableDefinition) this.Instruction.OpCodeArgument)));
                case OpCode.LoadDefaultValue:
                    return new DefaultValueExpression(
                        this.Context.ClrContext,
                        this.Instruction.Location,
                        (TypeReference)this.Instruction.OpCodeArgument);
                case OpCode.Endfinally:
                    return null;
                case OpCode.StoreLocal_s:
                case OpCode.StoreLocal:
                case OpCode.Localloc:
                case OpCode.Endfilter:
                case OpCode.Unaligned:
                case OpCode.Volatile:
                case OpCode.Tailcall:
                case OpCode.Initobj:
                case OpCode.Constrained:
                case OpCode.Cpblk:
                case OpCode.Initblk:
                case OpCode.Rethrow:
                case OpCode.Sizeof:
                case OpCode.Refanytype:
                case OpCode.Readonly:
                case OpCode.Leave:
                case OpCode.Leave_s:
                default:
                    break;
            }

            throw new NotSupportedException(this.Instruction.OpCode.ToString());
        }

        /*
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
         */
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

            switch (this.Instruction.FlowType)
            {
                case FlowType.Branch:
                case FlowType.Leave:
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
                    if (this.Instruction.Next != null)
                    {
                        this.nextFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Instruction.Next]);
                    }
                    break;
            }
        }

        private void ComputePreviousFlowInstructions()
        {
            this.previousFlowInstructions.Clear();

            for (int i = 0; i < this.Context.Instructions.Count; i++)
            {
                if (this.Context.Instructions[i].FlowType == FlowType.ConditionalBranch ||
                    this.Context.Instructions[i].FlowType == FlowType.Branch ||
                    this.Context.Instructions[i].FlowType == FlowType.Leave)
                {
                    if (this.Instruction.Label.Equals(this.Context.Instructions[i].OpCodeArgument))
                    {
                        this.previousFlowInstructions.Add(
                            this.Context.InstructionToBlock[this.Context.Instructions[i]]);
                    }
                }
                else if (this.Context.Instructions[i].FlowType == FlowType.Switch)
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
                switch (this.Instruction.Previous.FlowType)
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
                FlowType previousFlowType = this.PreviousInstructions[i].Instruction.FlowType;
                if (previousFlowType == FlowType.ConditionalBranch
                    || previousFlowType == FlowType.Branch
                    || previousFlowType == FlowType.Leave)
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
                this.Instruction.FlowType,
                this.Instruction.OpCodeArgument);
        }
        #endregion
    }
}
