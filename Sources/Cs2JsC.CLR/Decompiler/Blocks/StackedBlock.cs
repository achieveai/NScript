namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cs2JsC.CLR.AST;
    using Mono.Cecil;
    using Cs2JsC.Utils;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    class StackedBlock : BasicBlock
    {
        #region member variables

        #endregion

        #region constructors/Factories
        public StackedBlock(InstructionBlock rootBlock, params Block[] blocks)
            : base(StackedBlock.MergeArray(blocks, rootBlock))
        {
            this.RootBlock = rootBlock;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public InstructionBlock RootBlock
        { get; private set; }
        #endregion

        #region public functions
        /// <summary>
        /// Gets the dependent.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Dependent block at given index.</returns>
        public Block GetDependent(int index)
        {
            if (index < 0
                || index >= this.Children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.Children[index];
        }

        /// <summary>
        /// Dependents the count.
        /// </summary>
        /// <returns>Number of dependents</returns>
        public int DependentCount()
        {
            return this.Children.Count - 1;
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            switch (this.RootBlock.Instruction.OpCode)
            {
                case OpCode.Nop:
                    return null;
                case OpCode.Pop:
                    return new ExpressionStatement(
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver));
                case OpCode.Dup:
                    return (Expression)this.GetDependent(0).ToAstNode(variableResolver);
                case OpCode.Jmp:
                case OpCode.CallIndirect:
                    break;
                case OpCode.Call:
                case OpCode.CallVirtual:
                case OpCode.Newobj:
                    return this.GetMethodCallExpression(variableResolver);

                case OpCode.Return:
                    if (this.DependentCount() > 0)
                    {
                        return new ReturnStatement(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            this.GetCorrectTypedPrimitive(
                                (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                                this.Context.MethodDefinition.ReturnType));
                    }
                    else
                    {
                        return new ReturnStatement(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            null);
                    }
                // For branches, we branch when condition is met. This means we continue if the
                // condition does not match. So all the operators are ! operators.
                case OpCode.BranchFalse_s:
                case OpCode.BranchFalse:
                    return GetBoolEquivalentExpression(
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver));
                case OpCode.BranchTrue_s:
                case OpCode.BranchTrue:
                    return new UnaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        GetBoolEquivalentExpression(
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver)),
                        UnaryOperator.LogicalNot);
                case OpCode.BranchEq_s:
                case OpCode.BranchEq:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.NotEquals);
                case OpCode.BranchGe_s:
                case OpCode.BranchUGe_s:
                case OpCode.BranchGe:
                case OpCode.BranchUGe:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.LessThan);
                case OpCode.BranchGt_s:
                case OpCode.BranchUGt_s:
                case OpCode.BranchGt:
                case OpCode.BranchUGt:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.LessThanOrEqual);
                case OpCode.BranchLe_s:
                case OpCode.BranchULe_s:
                case OpCode.BranchLe:
                case OpCode.BranchULe:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.GreaterThan);
                case OpCode.BranchLt_s:
                case OpCode.BranchULt_s:
                case OpCode.BranchLt:
                case OpCode.BranchULt:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.GreaterThanOrEqual);
                case OpCode.BranchUNe_s:
                case OpCode.BranchUNe:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Equals);
                case OpCode.Branch_s:
                case OpCode.Branch:
                case OpCode.Switch:
                    break;
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
                case OpCode.LoadObject:
                    return new VariableAddressReference(
                        this.Context.ClrContext,
                        null,
                        variableResolver.ResolveParameter(
                            (ParameterDefinition)this.GetDependent(0).GetInstructionAt(0).OpCodeArgument));

                case OpCode.StoreObject:
                case OpCode.StoreIndirect_ref:
                case OpCode.StoreIndirectInt8:
                case OpCode.StoreIndirectInt16:
                case OpCode.StoreIndirectInt32:
                case OpCode.StoreIndirectInt64:
                case OpCode.StoreIndirectSingle:
                case OpCode.StoreIndirectDouble:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new VariableAddressReference(
                            this.Context.ClrContext,
                            null,
                            variableResolver.ResolveParameter(
                                (ParameterDefinition)this.GetDependent(0).GetInstructionAt(0).OpCodeArgument)),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Assignment);

                case OpCode.Add:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Plus);
                case OpCode.Sub:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Minus);
                case OpCode.Mul:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Mul);
                case OpCode.Div:
                case OpCode.Div_un:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Div);
                case OpCode.Rem:
                case OpCode.Rem_un:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Mod);
                case OpCode.And:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.BitwiseAnd);
                case OpCode.Or:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.BitwiseOr);
                case OpCode.Xor:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.BitwiseOr);
                case OpCode.Shl:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.LeftShift);
                case OpCode.Shr:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.RightShift);
                case OpCode.Shr_un:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.UnsignedRightShift);
                case OpCode.Neg:
                    return new UnaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        UnaryOperator.UnaryMinus);
                case OpCode.Not:
                    return new UnaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        UnaryOperator.LogicalNot);
                // case OpCode.LoadObject:
                //    return (Expression)this.GetDependent(0).ToAstNode(variableResolver);
                case OpCode.CopyObject:
                case OpCode.LoadString:
                    break;
                case OpCode.Castclass:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (TypeReference)this.RootBlock.Instruction.OpCodeArgument,
                        TypeCheckType.CastType);
                case OpCode.Isinst:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (TypeReference)this.RootBlock.Instruction.OpCodeArgument,
                        TypeCheckType.AsType);
                case OpCode.Conv_r_un:
                case OpCode.Unbox:
                case OpCode.Unbox_any:
                    return new UnboxExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (TypeReference)this.RootBlock.Instruction.OpCodeArgument);
                case OpCode.Throw:
                    return new ThrowStatement(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver));
                case OpCode.LoadField:
                case OpCode.LoadFieldAddress:
                    {
                        Expression rv = new FieldReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            (FieldReference)this.RootBlock.Instruction.OpCodeArgument,
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver));

                        if (this.RootBlock.Instruction.OpCode == OpCode.LoadFieldAddress)
                        {
                            rv = new LoadAddressExpression(
                                this.Context.ClrContext,
                                rv.Location,
                                rv);
                        }

                        return rv;
                    }
                case OpCode.LoadStaticField:
                case OpCode.LoadStaticFieldAddress:
                    break;
                case OpCode.StoreField:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new FieldReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            (FieldReference)this.RootBlock.Instruction.OpCodeArgument,
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver)),
                        this.GetCorrectTypedPrimitive(
                            (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                            ((FieldReference)this.RootBlock.Instruction.OpCodeArgument).Resolve().FieldType),
                        BinaryOperator.Assignment);

                case OpCode.StoreStaticField:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new FieldReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            (FieldReference)this.RootBlock.Instruction.OpCodeArgument),
                        this.GetCorrectTypedPrimitive(
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            ((FieldReference)this.RootBlock.Instruction.OpCodeArgument).Resolve().FieldType),
                        BinaryOperator.Assignment);
                // case OpCode.StoreObject:
                //     break;
                case OpCode.Box:
                    return new BoxExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        this.GetCorrectTypedPrimitive(
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            (TypeReference)this.RootBlock.Instruction.OpCodeArgument));
                case OpCode.Newarr:
                    return new NewArrayExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (TypeReference)this.RootBlock.Instruction.OpCodeArgument,
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver));
                case OpCode.LoadLength:
                    return new PropertyReferenceExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new InternalPropertyReference(this.KnownReferences.ArrayLengthGetter, null),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver));

                case OpCode.LoadElementAddress:
                    return
                        new LoadAddressExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            new ArrayElementExpression(
                                this.Context.ClrContext,
                                this.ComputeLocation(),
                                (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                                (Expression)this.GetDependent(1).ToAstNode(variableResolver)));
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
                    return new ArrayElementExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver));
                case OpCode.StoreElementIntPtr:
                case OpCode.StoreElementInt8:
                case OpCode.StoreElementInt16:
                case OpCode.StoreElementInt32:
                case OpCode.StoreElementInt64:
                case OpCode.StoreElementSingle:
                case OpCode.StoreElementDouble:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new ArrayElementExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            (Expression)this.GetDependent(1).ToAstNode(variableResolver)),
                        (Expression)this.GetDependent(2).ToAstNode(variableResolver),
                        BinaryOperator.Assignment);
                case OpCode.LoadElement_ref:
                case OpCode.LoadElement:
                    return new ArrayElementExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver));
                case OpCode.StoreElement_ref:
                case OpCode.StoreElement:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new ArrayElementExpression(
                        this.Context.ClrContext,
                            this.ComputeLocation(),
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            (Expression)this.GetDependent(1).ToAstNode(variableResolver)),
                        (Expression)this.GetDependent(2).ToAstNode(variableResolver),
                        BinaryOperator.Assignment);
                case OpCode.Refanyval:
                case OpCode.Ckfinite:
                case OpCode.Mkrefany:
                case OpCode.Ldtoken:
                    break;
                case OpCode.ConvertToInt8:
                case OpCode.ConvertUnsignedOverflowToInt8:
                case OpCode.ConvertOverflowToInt8:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.SByte,
                        TypeCheckType.CastType);
                case OpCode.ConvertToInt16:
                case OpCode.ConvertUnsignedOverflowToInt16:
                case OpCode.ConvertOverflowToInt16:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.Short,
                        TypeCheckType.CastType);
                case OpCode.ConvertToInt32:
                case OpCode.ConvertUnsignedOverflowToInt32:
                case OpCode.ConvertOverflowToInt32:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.Int32,
                        TypeCheckType.CastType);
                case OpCode.ConvertToInt64:
                case OpCode.ConvertUnsignedOverflowToInt64:
                case OpCode.ConvertOverflowToInt64:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.Int64,
                        TypeCheckType.CastType);
                case OpCode.ConvertToUInt8:
                case OpCode.ConvertUnsignedOverflowToUInt8:
                case OpCode.ConvertOverflowToUInt8:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.Byte,
                        TypeCheckType.CastType);
                case OpCode.ConvertToUInt16:
                case OpCode.ConvertUnsignedOverflowToUInt16:
                case OpCode.ConvertOverflowToUInt16:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.UShort,
                        TypeCheckType.CastType);
                case OpCode.ConvertToUInt32:
                case OpCode.ConvertUnsignedOverflowToUInt32:
                case OpCode.ConvertOverflowToUInt32:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.UInt32,
                        TypeCheckType.CastType);
                case OpCode.ConvertToUInt64:
                case OpCode.ConvertUnsignedOverflowToUInt64:
                case OpCode.ConvertOverflowToUInt64:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.UInt64,
                        TypeCheckType.CastType);
                case OpCode.ConvertToIntPtr:
                case OpCode.ConvertUnsignedOverflowToIntPtr:
                case OpCode.ConvertOverflowToIntPtr:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.IntPtr,
                        TypeCheckType.CastType);
                case OpCode.ConvertUnsignedOverflowToUIntPtr:
                case OpCode.ConvertOverflowToUIntPtr:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.UIntPtr,
                        TypeCheckType.CastType);
                case OpCode.ConvertToSingle:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.Single,
                        TypeCheckType.CastType);
                case OpCode.ConvertToDouble:
                    return new TypeCheckExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        this.KnownReferences.Double,
                        TypeCheckType.CastType);
                case OpCode.Add_ovf:
                case OpCode.Add_ovf_un:
                case OpCode.Mul_ovf:
                case OpCode.Mul_ovf_un:
                case OpCode.Sub_ovf:
                case OpCode.Sub_ovf_un:
                case OpCode.Endfinally:
                case OpCode.Leave:
                case OpCode.Leave_s:
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
                    break;
                case OpCode.CompareUGt:
                    if (this.GetDependent(1).InstructionCount == 1 &&
                        (this.GetDependent(1).GetInstructionAt(0).OpCode == OpCode.LoadNull ||
                        (this.GetDependent(1).GetInstructionAt(0).OpCode == OpCode.LoadConstantInt32
                         && 0 == (int)this.GetDependent(1).GetInstructionAt(0).OpCodeArgument)))
                    {
                        // if 1st argument is 0 then UGt really means that it's not equal to 0.
                        return new BinaryExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                            BinaryOperator.NotEquals);
                    }
                    else
                    {
                        return new BinaryExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                            BinaryOperator.GreaterThan);
                    }
                case OpCode.CompareULt:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.LessThan);
                case OpCode.CompareEq:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.Equals);
                case OpCode.CompareGt:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.GreaterThan);
                case OpCode.CompareLt:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                        (Expression)this.GetDependent(1).ToAstNode(variableResolver),
                        BinaryOperator.LessThan);
                case OpCode.LoadFunction:
                case OpCode.LoadVirtualFunction:
                case OpCode.LoadArgument:
                case OpCode.LoadArgumentAddress:
                    break;
                case OpCode.StoreArgument:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new VariableReference(
                            this.Context.ClrContext,
                            null,
                            variableResolver.ResolveParameter(
                                (ParameterDefinition)this.RootBlock.Instruction.OpCodeArgument)),
                        this.GetCorrectTypedPrimitive(
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            variableResolver.ResolveParameter(
                                (ParameterDefinition)this.RootBlock.Instruction.OpCodeArgument).Type),
                        BinaryOperator.Assignment);
                case OpCode.LoadLocal:
                case OpCode.LoadLocalAddress:
                    break;
                case OpCode.StoreLocal:
                    return new BinaryExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        new VariableReference(
                            this.Context.ClrContext,
                            null,
                            variableResolver.ResolveLocal(
                                (VariableDefinition)this.RootBlock.Instruction.OpCodeArgument)),
                        this.GetCorrectTypedPrimitive(
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            variableResolver.ResolveLocal(
                                (VariableDefinition)this.RootBlock.Instruction.OpCodeArgument).Type),
                        BinaryOperator.Assignment);
                case OpCode.Constrained:
                    // Ideally we need to box the object and then call virtual method on the boxed variable.
                    // since we are not worrying much about this,
                    // L_0000: ldarga.s i
                    // L_0002: constrained int32
                    // L_0008: callvirt instance string [mscorlib]System.Object::ToString()
                    // To void loading of address, we need to convert this expression to non loadAddress
                    // load expression.
                    //
                    // We have added code in MethodExecution to clean OpCodes and it converts load address
                    // instruction to load instruction so that we don't have to worry about
                    // converting address instruction to normal instruction.
                    return new BoxExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        this.GetCorrectTypedPrimitive(
                            (Expression)this.GetDependent(0).ToAstNode(variableResolver),
                            (TypeReference)this.RootBlock.Instruction.OpCodeArgument));

                case OpCode.Initobj:
                    {
                        Expression nestedExpression =
                            LoadAddressExpression.GetLoadAddressExpression((Expression)this.GetDependent(0).ToAstNode(variableResolver));
                        // if (nestedExpression is VariableAddressReference)
                        // {
                            return new BinaryExpression(
                                this.Context.ClrContext,
                                this.ComputeLocation(),
                                LoadAddressExpression.GetLoadAddressExpression((Expression)this.GetDependent(0).ToAstNode(variableResolver)),
                                new DefaultValueExpression(
                                    this.Context.ClrContext,
                                    this.ComputeLocation(),
                                    (TypeReference)this.RootBlock.Instruction.OpCodeArgument),
                                BinaryOperator.Assignment);
                        // }
                        // else
                        // {
                        //     return new InitObjectWithDefaultValue(
                        //         this.Context.ClrContext,
                        //         this.ComputeLocation(),
                        //         (TypeReference)this.RootBlock.Instruction.OpCodeArgument,
                        //         LoadAddressExpression.GetLoadAddressExpression(this.GetDependent(0).ToAstNode(variableResolver)));
                        // }
                    }

                case OpCode.Localloc:
                case OpCode.Endfilter:
                case OpCode.Unaligned:
                case OpCode.Volatile:
                case OpCode.Tailcall:
                case OpCode.Cpblk:
                case OpCode.Initblk:
                case OpCode.Rethrow:
                case OpCode.Sizeof:
                case OpCode.Refanytype:
                case OpCode.Readonly:
                case OpCode.Break:
                case OpCode.LoadArgument_0:
                case OpCode.LoadArgument_1:
                case OpCode.LoadArgument_2:
                case OpCode.LoadArgument_3:
                case OpCode.LoadLocal_0:
                case OpCode.LoadLocal_1:
                case OpCode.LoadLocal_2:
                case OpCode.LoadLocal_3:
                case OpCode.LoadArgument_s:
                case OpCode.LoadArgumentAddress_s:
                case OpCode.LoadLocal_s:
                case OpCode.LoadLocalAddress_s:
                case OpCode.StoreLocal_0:
                case OpCode.StoreLocal_1:
                case OpCode.StoreLocal_2:
                case OpCode.StoreLocal_3:
                case OpCode.StoreArgument_s:
                case OpCode.StoreLocal_s:
                case OpCode.LoadNull:
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
                case OpCode.LoadConstantInt32:
                case OpCode.LoadConstantInt64:
                case OpCode.LoadConstantSingle:
                case OpCode.LoadConstantDouble:
                default:
                    break;
            }

            throw new NotSupportedException(
                string.Format(
                    "OpCode: {0} not supported",
                    this.RootBlock.Instruction.OpCode));
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static Block[] MergeArray(Block[] array1, Block block)
        {
            Block[] array2 = new Block[array1.Length + 1];
            array1.CopyTo(array2, 0);
            array2[array1.Length] = block;

            return array2;
        }

        /// <summary>
        /// Gets the bool equivalent expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static Expression GetBoolEquivalentExpression(Expression expression)
        {
            if (expression.ResultType.IsBoolean())
            {
                return expression;
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.SByte)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new IntLiteral(
                        expression.Context,
                        null,
                        (sbyte) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.Byte)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new UIntLiteral(
                        expression.Context,
                        null,
                        (byte) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.Int16)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new IntLiteral(
                        expression.Context,
                        null,
                        (short) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.UInt16)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new UIntLiteral(
                        expression.Context,
                        null,
                        (ushort) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.Int32)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new IntLiteral(
                        expression.Context,
                        null,
                        (int) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.UInt32)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new UIntLiteral(
                        expression.Context,
                        null,
                        (uint) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.Int64)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new IntLiteral(
                        expression.Context,
                        null,
                        (long) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.GetTypeCode() == TypeCode.UInt64)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new UIntLiteral(
                        expression.Context,
                        null,
                        (ulong) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.MetadataType == MetadataType.IntPtr)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new IntLiteral(
                        expression.Context,
                        null,
                        (IntPtr) 0),
                    BinaryOperator.NotEquals);
            }

            if (expression.ResultType.MetadataType == MetadataType.UIntPtr)
            {
                return new BinaryExpression(
                    expression.Context,
                    expression.Location,
                    expression,
                    new UIntLiteral(
                        expression.Context,
                        null,
                        (UIntPtr) 0),
                    BinaryOperator.NotEquals);
            }

            return new BinaryExpression(
                expression.Context,
                expression.Location,
                expression,
                new NullLiteral(
                    expression.Context,
                    null),
                BinaryOperator.NotEquals);
        }

        /// <summary>
        /// Gets the method call expression.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>Appropriate call expression.</returns>
        private Expression GetMethodCallExpression(VariableResolver variableResolver)
        {
            MethodReference methodReference = (MethodReference) this.RootBlock.Instruction.OpCodeArgument;
            MethodDefinition methodDefinition = methodReference.Resolve();
            PropertyDefinition propertyDefinition = methodDefinition.GetPropertyDefinition();
            Expression callExpression;

            if (propertyDefinition != null)
            {
                return this.GetPropertyReference(
                    propertyDefinition,
                    methodReference,
                    variableResolver);
            }

            // This is call to typeof function.
            if (methodReference.DeclaringType.IsSame(this.KnownReferences.TypeType)
                && methodReference.Name == "GetTypeFromHandle")
            {
                return new AST.TypeofExpression(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    (AST.TypeReferenceExpression)this.GetDependent(0).ToAstNode(variableResolver));
            }

            if (methodDefinition.GetParentDefinition().BaseType != null
                && this.KnownReferences.MulticastDelegate.IsSame(
                    methodDefinition.GetParentDefinition().BaseType)
                && this.RootBlock.Instruction.OpCode == OpCode.Newobj)
            {
                return this.ProcessDelegateMethodExpression(variableResolver);
            }

            Location location = this.ComputeLocation();
            List<Expression> nestedExpressions = new List<Expression>(this.DependentCount());

            for (int depedentIndex = 0; depedentIndex < this.DependentCount(); depedentIndex++)
            {
                Expression nestedExpression =
                    (Expression)this.GetDependent(depedentIndex).ToAstNode(variableResolver);

                int argumentIndex = methodDefinition.IsStatic
                    ? depedentIndex
                    : depedentIndex - 1;

                if (argumentIndex >= 0)
                {
                    nestedExpression = this.GetCorrectTypedPrimitive(
                        nestedExpression,
                        methodDefinition.Parameters[argumentIndex].ParameterType);
                }

                nestedExpressions.Add(
                    (Expression) this.GetDependent(depedentIndex).ToAstNode(variableResolver));
            }

            if (methodDefinition.GetParentDefinition().BaseType != null
                && this.KnownReferences.MulticastDelegate.IsSame(methodDefinition.GetParentDefinition().BaseType)
                && methodReference.Name == "Invoke")
            {
                callExpression = nestedExpressions[0];
                nestedExpressions.RemoveAt(0);

                // Return Method call on delegate
                return new MethodCallExpression(
                    this.Context.ClrContext,
                    location,
                    callExpression,
                    nestedExpressions.ToArray());
            }

            if (this.RootBlock.Instruction.OpCode == OpCode.CallVirtual
                && methodReference.Resolve().IsVirtual)
            {
                callExpression = nestedExpressions[0];
                nestedExpressions.RemoveAt(0);

                return new MethodCallExpression(
                    this.Context.ClrContext,
                    location,
                    new VirtualMethodReferenceExpression(
                        this.Context.ClrContext,
                        callExpression.Location,
                        methodReference,
                        callExpression),
                    nestedExpressions.ToArray());
            }

            if (this.RootBlock.Instruction.OpCode == OpCode.Newobj)
            {
                return new NewObjectExpression(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    methodReference,
                    nestedExpressions.ToArray());
            }

            if (methodReference.Resolve().IsStatic)
            {
                return new MethodCallExpression(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    new MethodReferenceExpression(
                        this.Context.ClrContext,
                        location,
                        methodReference),
                    nestedExpressions.ToArray());
            }

            callExpression = nestedExpressions[0];
            nestedExpressions.RemoveAt(0);

            return new MethodCallExpression(
                this.Context.ClrContext,
                location,
                new MethodReferenceExpression(
                    this.Context.ClrContext,
                    callExpression.Location,
                    methodReference,
                    callExpression),
                nestedExpressions.ToArray());
        }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <param name="propertyDefinition">The property definition.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>PropertyReference Expression</returns>
        private Expression GetPropertyReference(
            PropertyDefinition propertyDefinition,
            MethodReference methodReference,
            VariableResolver variableResolver)
        {
            MethodDefinition methodDefinition = methodReference.Resolve();
            List<Expression> arguments = new List<Expression>();

            int readArguments = propertyDefinition.Parameters.Count + (propertyDefinition.IsStatic() ? 0 : 1);

            for (int dependentIndex = 0; dependentIndex < readArguments; dependentIndex++)
            {
                Expression argument = (Expression) this.GetDependent(dependentIndex).ToAstNode(variableResolver);

                int argumentIndex = propertyDefinition.IsStatic()
                    ? dependentIndex
                    : dependentIndex - 1;

                if (argumentIndex >= 0)
                {
                    argument = this.GetCorrectTypedPrimitive(
                        argument,
                        methodDefinition.Parameters[argumentIndex].ParameterType);
                }

                arguments.Add(argument);
            }

            Expression value = null;
            Expression objectExpression = null;
            if (!propertyDefinition.IsStatic())
            {
                objectExpression = arguments[0];
                arguments.RemoveAt(0);
            }

            if (propertyDefinition.SetMethod == methodDefinition)
            {
                value = this.GetCorrectTypedPrimitive(
                        (Expression) this.GetDependent(readArguments).ToAstNode(variableResolver),
                        propertyDefinition.PropertyType);
            }

            PropertyReferenceExpression propertyReferenceExpression = new PropertyReferenceExpression(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    new InternalPropertyReference(
                        methodDefinition.IsGetter ? methodReference : null,
                        methodDefinition.IsSetter ? methodReference : null),
                    objectExpression,
                    arguments);

            if (propertyDefinition.SetMethod == methodDefinition)
            {

                // We can be here only in the assignment case.
                return new BinaryExpression(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    propertyReferenceExpression,
                    value,
                    BinaryOperator.Assignment);
            }

            return propertyReferenceExpression;
        }

        /// <summary>
        /// Processes the delegate method expression.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>Delegate method expression</returns>
        private Expression ProcessDelegateMethodExpression(
            VariableResolver variableResolver)
        {
            if (this.RootBlock.Instruction.OpCode == OpCode.Newobj)
            {
                var loadFunction = this.GetDependent(1);
                var loadInstruction = loadFunction.GetInstructionAt(loadFunction.InstructionCount - 1);
                MethodReference methodReference = (MethodReference)loadInstruction.OpCodeArgument;
                Expression methodAccessorExpression = (Expression)this.GetDependent(0).ToAstNode(variableResolver);
                MethodReferenceExpression methodExpression;

                if (loadInstruction.OpCode == OpCode.LoadFunction)
                {
                    if (methodReference.Resolve().IsStatic)
                    {
                        methodExpression = new MethodReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            methodReference);
                    }
                    else
                    {
                        methodExpression = new MethodReferenceExpression(
                            this.Context.ClrContext,
                            this.ComputeLocation(),
                            methodReference,
                            methodAccessorExpression);
                    }
                }
                else
                {
                    methodExpression = new VirtualMethodReferenceExpression(
                        this.Context.ClrContext,
                        this.ComputeLocation(),
                        methodReference,
                        methodAccessorExpression);
                }

                return new DelegateMethodExpression(
                    this.Context.ClrContext,
                    methodExpression.Location,
                    methodExpression,
                    ((MethodReference) this.RootBlock.Instruction.OpCodeArgument).DeclaringType);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Converts to address value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// value of addressed expression
        /// </returns>
        private Expression ConvertToAddressValue(Expression expression)
        {
            if (expression is LoadAddressExpression)
            {
                return ((LoadAddressExpression)expression).NestedExpression;
            }
            else if (expression is VariableAddressReference)
            {
                return new VariableReference(
                    this.Context.ClrContext,
                    expression.Location,
                    ((VariableAddressReference)expression).Variable);
            }

            return expression;
        }

        /// <summary>
        /// Gets the correct typed primitive.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>Casted expression for primitive types.</returns>
        private Expression GetCorrectTypedPrimitive(
            Expression expression,
            TypeReference typeReference)
        {
            if (expression is IntLiteral
                && typeReference is TypeReference)
            {
                // Here we try to handle only Int/UInt => bool/char/UInt
                switch (((TypeReference)typeReference).FullName)
                {
                    case "System.Boolean":
                    case "System.Char":
                    case "System.Byte":
                    case "System.SByte":
                    case "System.Int16":
                    case "System.UInt16":
                    case "System.Int32":
                    case "System.UInt32":
                    case "System.Int64":
                    case "System.UInt64":
                    case "System.IntPtr":
                    case "System.UIntPtr":
                        return new AST.TypeCheckExpression(
                            this.Context.ClrContext,
                            expression.Location,
                            expression,
                            typeReference,
                            TypeCheckType.CastType);
                }
            }

            return expression;
        }
        #endregion
    }
}
