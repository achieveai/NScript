//-----------------------------------------------------------------------
// <copyright file="ProtoBufSerialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib.Serialization
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(103, typeof(LocalVariableSer))]
    [ProtoInclude(105, typeof(ExpressionSer))]
    [ProtoInclude(106, typeof(StatementSer))]
    [ProtoInclude(110, typeof(MethodCallArg))]
    [ProtoInclude(169, typeof(SwitchSectionSer))]
    [ProtoInclude(114, typeof(ObjectInitilaizer))]
    [ProtoInclude(179, typeof(MethodBody))]
    [Serializable]
    public class AstBase
    {
        public LocationSer Location { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(104, typeof(ParameterSer))]
    [Serializable]
    public class LocalVariableSer
        : AstBase
    {
        public int Type { get; set; }

        public string Name { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ParameterSer
        : LocalVariableSer
    {
        public int Modifier { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(108, typeof(NullExpression))]
    [ProtoInclude(180, typeof(ByteLiteralExpression))]
    [ProtoInclude(181, typeof(SByteLiteralExpression))]
    [ProtoInclude(182, typeof(ShortLiteralExpression))]
    [ProtoInclude(183, typeof(UShortLiteralExpression))]
    [ProtoInclude(184, typeof(IntLiteralExpression))]
    [ProtoInclude(185, typeof(UIntLiteralExpression))]
    [ProtoInclude(186, typeof(LongLiteralExpression))]
    [ProtoInclude(187, typeof(ULongLiteralExpression))]
    [ProtoInclude(188, typeof(FloatLiteralExpression))]
    [ProtoInclude(189, typeof(DoubleLiteralExpression))]
    [ProtoInclude(190, typeof(BoolLiteralExpression))]
    [ProtoInclude(191, typeof(StringLiteralExpression))]
    [ProtoInclude(192, typeof(DecimalLiteralExpression))]
    [ProtoInclude(193, typeof(CharLiteralExpression))]
    [ProtoInclude(194, typeof(BoolConstantExpression))]
    [ProtoInclude(195, typeof(ByteConstantExpression))]
    [ProtoInclude(196, typeof(SbyteConstantExpression))]
    [ProtoInclude(197, typeof(ShortConstantExpression))]
    [ProtoInclude(198, typeof(UshortConstantExpression))]
    [ProtoInclude(199, typeof(IntConstantExpression))]
    [ProtoInclude(200, typeof(UintConstantExpression))]
    [ProtoInclude(201, typeof(LongConstantExpression))]
    [ProtoInclude(202, typeof(UlongConstantExpression))]
    [ProtoInclude(203, typeof(FloatConstantExpression))]
    [ProtoInclude(204, typeof(DoubleConstantExpression))]
    [ProtoInclude(205, typeof(StringConstantExpression))]
    [ProtoInclude(206, typeof(DecimalConstantExpression))]
    [ProtoInclude(207, typeof(CharConstantExpression))]
    [ProtoInclude(111, typeof(MethodCallExpression))]
    [ProtoInclude(116, typeof(NewAnonymoustype))]
    [ProtoInclude(117, typeof(MethodExpression))]
    [ProtoInclude(118, typeof(FieldExpression))]
    [ProtoInclude(119, typeof(PropertyExpression))]
    [ProtoInclude(121, typeof(DynamicIndexBinderExpression))]
    [ProtoInclude(122, typeof(DynamicMethodBinderExpression))]
    [ProtoInclude(123, typeof(DelegateInvocationExpression))]
    [ProtoInclude(125, typeof(DefaultValueExpr))]
    [ProtoInclude(126, typeof(TypeExpressionSer))]
    [ProtoInclude(128, typeof(TypeCastExpression))]
    [ProtoInclude(129, typeof(DelegateCreationExpression))]
    [ProtoInclude(130, typeof(ArrayCreationExpression))]
    [ProtoInclude(131, typeof(ThisExpression))]
    [ProtoInclude(133, typeof(TypeOfExpression))]
    [ProtoInclude(134, typeof(ElementAccessExpression))]
    [ProtoInclude(135, typeof(AssignExpression))]
    [ProtoInclude(137, typeof(BoxCastExpression))]
    [ProtoInclude(138, typeof(NullConstantExpression))]
    [ProtoInclude(141, typeof(UnwrapExpression))]
    [ProtoInclude(142, typeof(WrapExpression))]
    [ProtoInclude(143, typeof(LiftedNullExpression))]
    [ProtoInclude(144, typeof(NullCoalescingOperatorSer))]
    [ProtoInclude(145, typeof(UnaryExpression))]
    [ProtoInclude(146, typeof(BinaryExpression))]
    [ProtoInclude(147, typeof(IsExpression))]
    [ProtoInclude(148, typeof(AsExpression))]
    [ProtoInclude(149, typeof(ConditionalExpression))]
    [ProtoInclude(150, typeof(LocalVariableRefExpression))]
    [ProtoInclude(152, typeof(ParameterReferenceExpression))]
    [ProtoInclude(177, typeof(AnonymousMethodBodyExpr))]
    [ProtoInclude(178, typeof(AnonymousMethodExpr))]
    [ProtoInclude(176, typeof(IteratorBlock))]
    [ProtoInclude(208, typeof(NullableToNormal))]
    [ProtoInclude(209, typeof(EventExpression))]
    [ProtoInclude(210, typeof(DynamicMemberExpression))]
    [ProtoInclude(211, typeof(DynamicMethodInvocationExpression))]
    [ProtoInclude(213, typeof(ConditionalAccess))]
    [ProtoInclude(214, typeof(ConditionalReceiver))]
    [ProtoInclude(216, typeof(LocalMethodCallExpression))]
    [ProtoInclude(217, typeof(LocalMethodExpression))]
    [ProtoInclude(140, typeof(ThrowExpression))]
    [ProtoInclude(218, typeof(TupleLiteral))]
    [Serializable]
    public abstract class ExpressionSer
        : AstBase
    {
        public bool NoUse_ { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(153, typeof(StatementExpressionSer))]
    [ProtoInclude(155, typeof(YieldBreakStatement))]
    [ProtoInclude(156, typeof(StatementListSer))]
    [ProtoInclude(157, typeof(ReturnStatement))]
    [ProtoInclude(158, typeof(BreakStatement))]
    [ProtoInclude(159, typeof(ContinueStatement))]
    [ProtoInclude(160, typeof(EmptyStatementSer))]
    [ProtoInclude(161, typeof(VariableBlockDeclaration))]
    [ProtoInclude(162, typeof(IfStatement))]
    [ProtoInclude(163, typeof(LoopStatement))]
    [ProtoInclude(170, typeof(SwitchStatement))]
    [ProtoInclude(171, typeof(TryFinallyBlockSer))]
    [ProtoInclude(173, typeof(CatchBlock))]
    [ProtoInclude(174, typeof(TryCatchBlock))]
    [ProtoInclude(107, typeof(BlockSer))]
    [ProtoInclude(215, typeof(LocalMethodStatement))]
    [ProtoInclude(219, typeof(DeconstructTupleAssignment))]
    [Serializable]
    public abstract class StatementSer
        : AstBase
    {
        public bool NoUse_ { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(172, typeof(ExplicitBlockSer))]
    [Serializable]
    public class BlockSer
        : StatementSer
    {
        public List<StatementSer> Statements { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LocalMethodStatement
        : StatementSer
    {
        public LocalMethodIdentitySer MethodId { get; set; }

        public ParameterBlock Block { get; set; }

        public int ScopeBlockId { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LocalMethodIdentitySer
    {
        public string MethodName
        { get; set; }

        public int ReturnType
        { get; set; }

        public int GenericParameters
        { get; set; }

        public List<ParameterSer> Parameters
        { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NullExpression
        : ExpressionSer
    {
        public bool None { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ByteLiteralExpression : ExpressionSer
    { public byte Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SByteLiteralExpression : ExpressionSer
    { public sbyte Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class CharLiteralExpression : ExpressionSer
    { public char Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ShortLiteralExpression : ExpressionSer
    { public short Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UShortLiteralExpression : ExpressionSer
    { public ushort Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IntLiteralExpression : ExpressionSer
    { public int Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UIntLiteralExpression : ExpressionSer
    { public uint Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LongLiteralExpression : ExpressionSer
    { public long Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ULongLiteralExpression : ExpressionSer
    { public ulong Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class FloatLiteralExpression : ExpressionSer
    { public float Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DoubleLiteralExpression : ExpressionSer
    { public double Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class BoolLiteralExpression : ExpressionSer
    { public bool Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class StringLiteralExpression : ExpressionSer
    { public string Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DecimalLiteralExpression : ExpressionSer
    { public decimal Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class MethodCallArg
        : AstBase
    {
        public bool IsByRef { get; set; }

        public ExpressionSer Value { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(112, typeof(StrCatExpression))]
    [ProtoInclude(113, typeof(NewExpression))]
    [ProtoInclude(124, typeof(ConstructorInitializerExpression))]
    [ProtoInclude(127, typeof(UserCastExpression))]
    [ProtoInclude(208, typeof(UserDefinedBinaryOrUnaryOpExpression))]
    [Serializable]
    public class MethodCallExpression
        : ExpressionSer
    {
        public int Method { get; set; }

        public ExpressionSer Instance { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LocalMethodCallExpression
        : ExpressionSer
    {
        public string MethodName { get; set; }

        public int ReturnType { get; set; }

        public List<int> TypeParameters { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UserDefinedBinaryOrUnaryOpExpression
        : MethodCallExpression
    {
        public int Operator
        { get; set; }

        public bool IsLifted
        { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class StrCatExpression
        : MethodCallExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(115, typeof(NewInitializerExpression))]
    [ProtoInclude(212, typeof(NewCollectionInitializerExpression))]
    [Serializable]
    public class NewExpression
        : MethodCallExpression
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ObjectInitilaizer
        : AstBase
    {
        public int? Setter { get; set; }

        public int? Getter { get; set; }

        public int? Field { get; set; }

        public ExpressionSer Value { get; set; }

        public MethodCallExpression MethodCall { get; set; }

        public int? Property { get; set; }

        public List<MethodCallArg> PropertyArgs { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NewInitializerExpression
        : NewExpression
    {
        public List<ObjectInitilaizer> Initializers { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NewCollectionInitializerExpression
        : NewExpression
    {
        public List<MethodCallExpression> ItemInitializers { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NewAnonymoustype
        : ExpressionSer
    {
        public Dictionary<string, ExpressionSer> Initializers { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class MethodExpression
        : ExpressionSer
    {
        public int Method { get; set; }

        public List<int> GenericParameters { get; set; }

        public ExpressionSer Instance { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LocalMethodExpression
        : ExpressionSer
    {
        public string MethodName { get; set; }

        public int ReturnType { get; set; }

        public List<int> GenericParameters { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TupleLiteral
        : ExpressionSer
    {
        public List<ExpressionSer> TupleArgs { get; set; }

        public int TupleType { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DeconstructTupleAssignment
        : ExpressionSer
    {
        public List<ExpressionSer> LHSArgs { get; set; }

        public ExpressionSer RightTuple { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class FieldExpression
        : ExpressionSer
    {
        public int Field { get; set; }

        public ExpressionSer Instance { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(120, typeof(IndexExpression))]
    [Serializable]
    public class PropertyExpression
        : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public bool IsNotVirtual { get; set; }

        public int Property { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class EventExpression : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public bool IsNotVirtual { get; set; }

        public int Event { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IndexExpression
        : PropertyExpression
    {
        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DynamicMemberExpression
        : ExpressionSer
    {
        public string MemberName { get; set; }

        public ExpressionSer Instance { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DynamicIndexBinderExpression
        : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public ExpressionSer Index { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DynamicMethodBinderExpression
        : ExpressionSer
    {
        public string Name { get; set; }

        public ExpressionSer Instance { get; set; }

        public ExpressionSer Value { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DynamicMethodInvocationExpression
        : ExpressionSer
    {
        public ExpressionSer Method { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DelegateInvocationExpression
        : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ConstructorInitializerExpression
        : MethodCallExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DefaultValueExpr
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TypeExpressionSer
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UserCastExpression
        : MethodCallExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TypeCastExpression
        : ExpressionSer
    {
        public int Type { get; set; }

        public ExpressionSer Expression { get; set; }

        public bool IsUnbox { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NullableToNormal : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DelegateCreationExpression
        : ExpressionSer
    {
        public ExpressionSer Method { get; set; }

        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ArrayCreationExpression
        : ExpressionSer
    {
        public int ArrayType { get; set; }

        public int ElementType { get; set; }

        public List<ExpressionSer> Initializers { get; set; }

        public List<ExpressionSer> Arguments { get; set; }
    }

    public class TupleCreationExpression
        : ExpressionSer
    {
        public List<int> tupleType { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(132, typeof(BaseThisExpression))]
    [Serializable]
    public class ThisExpression
        : ExpressionSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class BaseThisExpression
        : ThisExpression
    {
        public bool NoUse2 { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TypeOfExpression
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ElementAccessExpression
        : ExpressionSer
    {
        public ExpressionSer Left { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(136, typeof(CompoundAssignExpression))]
    [Serializable]
    public class AssignExpression
        : ExpressionSer
    {
        public ExpressionSer Left { get; set; }

        public ExpressionSer Right { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class CompoundAssignExpression
        : AssignExpression
    {
        public int Operator { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class BoxCastExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NullConstantExpression
        : ExpressionSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class BoolConstantExpression
        : ExpressionSer
    { public bool Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ByteConstantExpression
        : ExpressionSer
    { public byte Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SbyteConstantExpression
        : ExpressionSer
    { public sbyte Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class CharConstantExpression
        : ExpressionSer
    { public char Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ShortConstantExpression
        : ExpressionSer
    { public short Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UshortConstantExpression
        : ExpressionSer
    { public ushort Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IntConstantExpression
        : ExpressionSer
    { public int Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UintConstantExpression
        : ExpressionSer
    { public uint Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LongConstantExpression
        : ExpressionSer
    { public long Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UlongConstantExpression
        : ExpressionSer
    { public ulong Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class FloatConstantExpression
        : ExpressionSer
    { public float Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DoubleConstantExpression
        : ExpressionSer
    { public double Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class StringConstantExpression
        : ExpressionSer
    { public string Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DecimalConstantExpression
        : ExpressionSer
    { public decimal Value { get; set; } }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ThrowExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UnwrapExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class WrapExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class LiftedNullExpression
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class NullCoalescingOperatorSer
        : ExpressionSer
    {
        public ExpressionSer Left { get; set; }

        public ExpressionSer Right { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class UnaryExpression
        : ExpressionSer
    {
        public int Operator { get; set; }

        public ExpressionSer Expression { get; set; }

        public bool IsLifted { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class BinaryExpression
        : ExpressionSer
    {
        public int Operator { get; set; }

        public ExpressionSer Left { get; set; }

        public ExpressionSer Right { get; set; }

        public bool IsLifted { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IsExpression
        : ExpressionSer
    {
        public int Type { get; set; }

        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class AsExpression
        : ExpressionSer
    {
        public int Type { get; set; }

        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ConditionalExpression
        : ExpressionSer
    {
        public ExpressionSer Condition { get; set; }

        public ExpressionSer TrueExpression { get; set; }

        public ExpressionSer FalseExpression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ConditionalAccess
        : ExpressionSer
    {
        public ExpressionSer Receiver
        { get; set; }

        public ExpressionSer AccessExpression
        { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ConditionalReceiver
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(151, typeof(TempVariableRefExpression))]
    [Serializable]
    public class LocalVariableRefExpression
        : ExpressionSer
    {
        public bool IsAddressReference { get; set; }

        public LocalVariableSer LocalVariable { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TempVariableRefExpression
        : LocalVariableRefExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ParameterReferenceExpression
        : ExpressionSer
    {
        public ParameterSer Parameter { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class AwaitExpression
        : ExpressionSer
    {
        public MethodCallExpression GetAwaiterMethodCall { get; set; }
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(154, typeof(YieldStatement))]
    [Serializable]
    public class StatementExpressionSer
        : StatementSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class YieldStatement
        : StatementExpressionSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class YieldBreakStatement
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class StatementListSer
        : StatementSer
    {
        public List<StatementSer> Statements { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ReturnStatement
        : StatementSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class BreakStatement
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ContinueStatement
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class EmptyStatementSer
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class VariableBlockDeclaration
        : StatementSer
    {
        public List<ExpressionSer> Initializers { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IfStatement
        : StatementSer
    {
        public ExpressionSer Condition { get; set; }

        public StatementSer TrueStatement { get; set; }

        public StatementSer FalseStatement { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(164, typeof(DoStatement))]
    [ProtoInclude(165, typeof(WhileStatement))]
    [ProtoInclude(166, typeof(ForStatement))]
    [ProtoInclude(167, typeof(ForEachStatement))]
    [Serializable]
    public class LoopStatement
        : StatementSer
    {
        public StatementSer Loop { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class DoStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class WhileStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ForStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }

        public StatementSer Initializer { get; set; }

        public StatementSer Iterator { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ForEachStatement
        : LoopStatement
    {
        public ExpressionSer Collection { get; set; }

        public string LocalVariableName { get; set; }

        new public StatementSer Loop { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(220, typeof(SwitchConstCaseLabel))]
    [ProtoInclude(221, typeof(SwitchDeclarationCaseLabel))]
    [ProtoInclude(222, typeof(SwitchDiscardCaseLabel))]
    [Serializable]
    public class SwitchCaseLabel: AstBase
    {
        public bool NoUse_ { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SwitchDiscardCaseLabel : SwitchCaseLabel
    {
        public bool NoUse_2 { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SwitchConstCaseLabel : SwitchCaseLabel
    {
        public ExpressionSer LabelValue { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SwitchDeclarationCaseLabel : SwitchCaseLabel
    {
        public LocalVariableSer? LocalVariableOpt { get; set; }
        public ExpressionSer When { get; set; }

        public int? DeclaredTypeOpt { get; set; }
    }


    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SwitchSectionSer
        : BlockSer
    {
        public List<SwitchCaseLabel> Labels { get; set; }

        public StatementSer Block { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class SwitchStatement
        : StatementSer
    {
        public bool IsIfElseStatement { get; set; }

        public ExpressionSer SwitchExpression { get; set; }

        public List<SwitchSectionSer> Blocks { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TryFinallyBlockSer
        : StatementSer
    {
        public StatementSer TryBlock { get; set; }

        public StatementSer FinallyBlock { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(175, typeof(ParameterBlock))]
    [Serializable]
    public class ExplicitBlockSer
        : BlockSer
    {
        public int Id { get; set; }

        public List<string> LocalFunctions { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class CatchBlock
        : StatementSer
    {
        public int? CatchType { get; set; }

        public LocalVariableSer LocalVariable { get; set; }

        public ExplicitBlockSer Block { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class TryCatchBlock
        : StatementSer
    {
        public StatementSer TryBlock { get; set; }

        public List<CatchBlock> CatchBlocks { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class ParameterBlock
        : ExplicitBlockSer
    {
        public List<ParameterSer> Parameters { get; set; }

        public bool IsMethodOwned { get; set; }

        public BlockKind BlockKind { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IteratorBlock
        : ExpressionSer
    {
        public int Type { get; set; }

        public ParameterBlock Block { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class AnonymousMethodBodyExpr
        : ExpressionSer
    {
        public int Type { get; set; }

        public ParameterBlock Block { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class AnonymousMethodExpr
        : ExpressionSer
    {
        public ParameterBlock Block { get; set; }
    }

    [Flags]
    [Serializable]
    public enum BlockKind
    {
        Regular = 0,
        Iterator = 1 << 1,
        Async = 1 << 2
    }


    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class MethodBody
        : AstBase
    {
        public int MethodId { get; set; }

        public BlockKind BlockKind { get; set; }

        public string FileName { get; set; }

        public ParameterBlock Body { get; set; }

        public LocationSer ScriptBlockLocation { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class FullAst
    {
        public List<MethodBody> Methods { get; set; }

        public TypeInfoSer TypeInfo { get; set; }
    }
}