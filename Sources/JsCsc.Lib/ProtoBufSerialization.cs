//-----------------------------------------------------------------------
// <copyright file="ProtoBufSerialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib.Serialization
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf;
    using NetJSON;
    using Newtonsoft.Json;
    //  MaxId = 214;

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ModuleSpecSer
    {
        public string Name { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(GenericParamSer)), ProtoInclude(100, typeof(GenericParamSer))]
    [NetJSONKnownType(typeof(ArrayTypeSer)), ProtoInclude(101, typeof(ArrayTypeSer))]
    [NetJSONKnownType(typeof(GenericInstanceTypeSer)), ProtoInclude(102, typeof(GenericInstanceTypeSer))]
    [NetJSONKnownType(typeof(PointerTypeSer)), ProtoInclude(209, typeof(PointerTypeSer))]
    public class TypeSpecSer
    {
        public string Name { get; set; }

        public ModuleSpecSer Module { get; set; }

        public TypeSpecSer NestedParent { get; set; }

        public string Namespace { get; set; }

        public int Arity { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class GenericParamSer
        : TypeSpecSer
    {
        public int Position { get; set; }

        public bool IsMethodOwned { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ArrayTypeSer
        : TypeSpecSer
    {
        public TypeSpecSer ElementType { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class PointerTypeSer
        : TypeSpecSer
    {
        public TypeSpecSer PointedAtType { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class GenericInstanceTypeSer
        : TypeSpecSer
    {
        public List<TypeSpecSer> TypeParams { get; set; }
   }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ParamSer
    {
        public TypeSpecSer ParamType { get; set; }

        public int ModFlags { get; set; }

        public string Name { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class FieldSpecSer
    {
        public TypeSpecSer DeclaringType { get; set; }

        public TypeSpecSer MemberType { get; set; }

        public string Name { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class PropertySpecSer
    {
        public int? Setter { get; set; }

        public int? Getter { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class EventSpecSer
    {
        public int? AddOn { get; set; }

        public int? RemoveOn { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class MethodSpecSer
    {
        public TypeSpecSer DeclaringType { get; set; }

        public TypeSpecSer ReturnType { get; set; }

        public string Name { get; set; }

        public bool IsStatic { get; set; }

        public int Arity { get; set; }

        public List<ParamSer> Parameters { get; set; }

        public List<TypeSpecSer> TypeArgs { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TypeInfoSer
    {
        public Dictionary<int, TypeSpecSer> Types { get; set; }

        public Dictionary<int, MethodSpecSer> Methods { get; set; }

        public Dictionary<int, FieldSpecSer> Fields { get; set; }

        public Dictionary<int, PropertySpecSer> Properties { get; set; }

        public Dictionary<int, EventSpecSer> Events { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class LocationSer
    {
        public int StartLine { get; set; }

        public int StartColumn { get; set; }

        public int EndLine { get; set; }

        public int EndColumn { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(LocalVariableSer)), ProtoInclude(103, typeof(LocalVariableSer))]
    [NetJSONKnownType(typeof(ExpressionSer)), ProtoInclude(105, typeof(ExpressionSer))]
    [NetJSONKnownType(typeof(StatementSer)), ProtoInclude(106, typeof(StatementSer))]
    [NetJSONKnownType(typeof(MethodCallArg)), ProtoInclude(110, typeof(MethodCallArg))]
    [NetJSONKnownType(typeof(SwitchSectionSer)), ProtoInclude(169, typeof(SwitchSectionSer))]
    [NetJSONKnownType(typeof(ObjectInitilaizer)), ProtoInclude(114, typeof(ObjectInitilaizer))]
    [NetJSONKnownType(typeof(MethodBody)), ProtoInclude(179, typeof(MethodBody))]
    public class AstBase
    {
        public LocationSer Location { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(ParameterSer)), ProtoInclude(104, typeof(ParameterSer))]
    public class LocalVariableSer
        : AstBase
    {
        public int Type { get; set; }

        public string Name { get; set; }

        public int BlockId { get; set; }
   }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ParameterSer
        : LocalVariableSer
    {
        public int Modifier { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(NullExpression)), ProtoInclude(108, typeof(NullExpression))]
    [NetJSONKnownType(typeof(ByteLiteralExpression)), ProtoInclude(180, typeof(ByteLiteralExpression))]
    [NetJSONKnownType(typeof(SByteLiteralExpression)), ProtoInclude(181, typeof(SByteLiteralExpression))]
    [NetJSONKnownType(typeof(ShortLiteralExpression)), ProtoInclude(182, typeof(ShortLiteralExpression))]
    [NetJSONKnownType(typeof(UShortLiteralExpression)), ProtoInclude(183, typeof(UShortLiteralExpression))]
    [NetJSONKnownType(typeof(IntLiteralExpression)), ProtoInclude(184, typeof(IntLiteralExpression))]
    [NetJSONKnownType(typeof(UIntLiteralExpression)), ProtoInclude(185, typeof(UIntLiteralExpression))]
    [NetJSONKnownType(typeof(LongLiteralExpression)), ProtoInclude(186, typeof(LongLiteralExpression))]
    [NetJSONKnownType(typeof(ULongLiteralExpression)), ProtoInclude(187, typeof(ULongLiteralExpression))]
    [NetJSONKnownType(typeof(FloatLiteralExpression)), ProtoInclude(188, typeof(FloatLiteralExpression))]
    [NetJSONKnownType(typeof(DoubleLiteralExpression)), ProtoInclude(189, typeof(DoubleLiteralExpression))]
    [NetJSONKnownType(typeof(BoolLiteralExpression)), ProtoInclude(190, typeof(BoolLiteralExpression))]
    [NetJSONKnownType(typeof(StringLiteralExpression)), ProtoInclude(191, typeof(StringLiteralExpression))]
    [NetJSONKnownType(typeof(DecimalLiteralExpression)), ProtoInclude(192, typeof(DecimalLiteralExpression))]
    [NetJSONKnownType(typeof(CharLiteralExpression)), ProtoInclude(193, typeof(CharLiteralExpression))]
    [NetJSONKnownType(typeof(BoolConstantExpression)), ProtoInclude(194, typeof(BoolConstantExpression))]
    [NetJSONKnownType(typeof(ByteConstantExpression)), ProtoInclude(195, typeof(ByteConstantExpression))]
    [NetJSONKnownType(typeof(SbyteConstantExpression)), ProtoInclude(196, typeof(SbyteConstantExpression))]
    [NetJSONKnownType(typeof(ShortConstantExpression)), ProtoInclude(197, typeof(ShortConstantExpression))]
    [NetJSONKnownType(typeof(UshortConstantExpression)), ProtoInclude(198, typeof(UshortConstantExpression))]
    [NetJSONKnownType(typeof(IntConstantExpression)), ProtoInclude(199, typeof(IntConstantExpression))]
    [NetJSONKnownType(typeof(UintConstantExpression)), ProtoInclude(200, typeof(UintConstantExpression))]
    [NetJSONKnownType(typeof(LongConstantExpression)), ProtoInclude(201, typeof(LongConstantExpression))]
    [NetJSONKnownType(typeof(UlongConstantExpression)), ProtoInclude(202, typeof(UlongConstantExpression))]
    [NetJSONKnownType(typeof(FloatConstantExpression)), ProtoInclude(203, typeof(FloatConstantExpression))]
    [NetJSONKnownType(typeof(DoubleConstantExpression)), ProtoInclude(204, typeof(DoubleConstantExpression))]
    [NetJSONKnownType(typeof(StringConstantExpression)), ProtoInclude(205, typeof(StringConstantExpression))]
    [NetJSONKnownType(typeof(DecimalConstantExpression)), ProtoInclude(206, typeof(DecimalConstantExpression))]
    [NetJSONKnownType(typeof(CharConstantExpression)), ProtoInclude(207, typeof(CharConstantExpression))]
    [NetJSONKnownType(typeof(MethodCallExpression)), ProtoInclude(111, typeof(MethodCallExpression))]
    [NetJSONKnownType(typeof(NewAnonymoustype)), ProtoInclude(116, typeof(NewAnonymoustype))]
    [NetJSONKnownType(typeof(MethodExpression)), ProtoInclude(117, typeof(MethodExpression))]
    [NetJSONKnownType(typeof(FieldExpression)), ProtoInclude(118, typeof(FieldExpression))]
    [NetJSONKnownType(typeof(PropertyExpression)), ProtoInclude(119, typeof(PropertyExpression))]
    [NetJSONKnownType(typeof(DynamicIndexBinderExpression)), ProtoInclude(121, typeof(DynamicIndexBinderExpression))]
    [NetJSONKnownType(typeof(DynamicMethodBinderExpression)), ProtoInclude(122, typeof(DynamicMethodBinderExpression))]
    [NetJSONKnownType(typeof(DelegateInvocationExpression)), ProtoInclude(123, typeof(DelegateInvocationExpression))]
    [NetJSONKnownType(typeof(DefaultValueExpr)), ProtoInclude(125, typeof(DefaultValueExpr))]
    [NetJSONKnownType(typeof(TypeExpressionSer)), ProtoInclude(126, typeof(TypeExpressionSer))]
    [NetJSONKnownType(typeof(TypeCastExpression)), ProtoInclude(128, typeof(TypeCastExpression))]
    [NetJSONKnownType(typeof(DelegateCreationExpression)), ProtoInclude(129, typeof(DelegateCreationExpression))]
    [NetJSONKnownType(typeof(ArrayCreationExpression)), ProtoInclude(130, typeof(ArrayCreationExpression))]
    [NetJSONKnownType(typeof(ThisExpression)), ProtoInclude(131, typeof(ThisExpression))]
    [NetJSONKnownType(typeof(TypeOfExpression)), ProtoInclude(133, typeof(TypeOfExpression))]
    [NetJSONKnownType(typeof(ElementAccessExpression)), ProtoInclude(134, typeof(ElementAccessExpression))]
    [NetJSONKnownType(typeof(AssignExpression)), ProtoInclude(135, typeof(AssignExpression))]
    [NetJSONKnownType(typeof(BoxCastExpression)), ProtoInclude(137, typeof(BoxCastExpression))]
    [NetJSONKnownType(typeof(NullConstantExpression)), ProtoInclude(138, typeof(NullConstantExpression))]
    [NetJSONKnownType(typeof(UnwrapExpression)), ProtoInclude(141, typeof(UnwrapExpression))]
    [NetJSONKnownType(typeof(WrapExpression)), ProtoInclude(142, typeof(WrapExpression))]
    [NetJSONKnownType(typeof(LiftedNullExpression)), ProtoInclude(143, typeof(LiftedNullExpression))]
    [NetJSONKnownType(typeof(NullCoalescingOperatorSer)), ProtoInclude(144, typeof(NullCoalescingOperatorSer))]
    [NetJSONKnownType(typeof(UnaryExpression)), ProtoInclude(145, typeof(UnaryExpression))]
    [NetJSONKnownType(typeof(BinaryExpression)), ProtoInclude(146, typeof(BinaryExpression))]
    [NetJSONKnownType(typeof(IsExpression)), ProtoInclude(147, typeof(IsExpression))]
    [NetJSONKnownType(typeof(AsExpression)), ProtoInclude(148, typeof(AsExpression))]
    [NetJSONKnownType(typeof(ConditionalExpression)), ProtoInclude(149, typeof(ConditionalExpression))]
    [NetJSONKnownType(typeof(LocalVariableRefExpression)), ProtoInclude(150, typeof(LocalVariableRefExpression))]
    [NetJSONKnownType(typeof(ParameterReferenceExpression)), ProtoInclude(152, typeof(ParameterReferenceExpression))]
    [NetJSONKnownType(typeof(AnonymousMethodBodyExpr)), ProtoInclude(177, typeof(AnonymousMethodBodyExpr))]
    [NetJSONKnownType(typeof(AnonymousMethodExpr)), ProtoInclude(178, typeof(AnonymousMethodExpr))]
    [NetJSONKnownType(typeof(IteratorBlock)), ProtoInclude(176, typeof(IteratorBlock))]
    [NetJSONKnownType(typeof(NullableToNormal)), ProtoInclude(208, typeof(NullableToNormal))]
    [NetJSONKnownType(typeof(EventExpression)), ProtoInclude(209, typeof(EventExpression))]
    [NetJSONKnownType(typeof(DynamicMemberExpression)), ProtoInclude(210, typeof(DynamicMemberExpression))]
    [NetJSONKnownType(typeof(DynamicMethodInvocationExpression)), ProtoInclude(211, typeof(DynamicMethodInvocationExpression))]
    [NetJSONKnownType(typeof(ConditionalAccess)), ProtoInclude(213, typeof(ConditionalAccess))]
    [NetJSONKnownType(typeof(ConditionalReceiver)), ProtoInclude(214, typeof(ConditionalReceiver))]
    public abstract class ExpressionSer
        : AstBase
    {
        public bool NoUse_ { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(ThrowExpression)), ProtoInclude(140, typeof(ThrowExpression))]
    [NetJSONKnownType(typeof(StatementExpressionSer)), ProtoInclude(153, typeof(StatementExpressionSer))]
    [NetJSONKnownType(typeof(YieldBreakStatement)), ProtoInclude(155, typeof(YieldBreakStatement))]
    [NetJSONKnownType(typeof(StatementListSer)), ProtoInclude(156, typeof(StatementListSer))]
    [NetJSONKnownType(typeof(ReturnStatement)), ProtoInclude(157, typeof(ReturnStatement))]
    [NetJSONKnownType(typeof(BreakStatement)), ProtoInclude(158, typeof(BreakStatement))]
    [NetJSONKnownType(typeof(ContinueStatement)), ProtoInclude(159, typeof(ContinueStatement))]
    [NetJSONKnownType(typeof(EmptyStatementSer)), ProtoInclude(160, typeof(EmptyStatementSer))]
    [NetJSONKnownType(typeof(VariableBlockDeclaration)), ProtoInclude(161, typeof(VariableBlockDeclaration))]
    [NetJSONKnownType(typeof(IfStatement)), ProtoInclude(162, typeof(IfStatement))]
    [NetJSONKnownType(typeof(LoopStatement)), ProtoInclude(163, typeof(LoopStatement))]
    [NetJSONKnownType(typeof(SwitchStatement)), ProtoInclude(170, typeof(SwitchStatement))]
    [NetJSONKnownType(typeof(TryFinallyBlockSer)), ProtoInclude(171, typeof(TryFinallyBlockSer))]
    [NetJSONKnownType(typeof(CatchBlock)), ProtoInclude(173, typeof(CatchBlock))]
    [NetJSONKnownType(typeof(TryCatchBlock)), ProtoInclude(174, typeof(TryCatchBlock))]
    [NetJSONKnownType(typeof(BlockSer)), ProtoInclude(107, typeof(BlockSer))]
    public abstract class StatementSer
        : AstBase
    {
        public bool NoUse_ { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(ExplicitBlockSer)), ProtoInclude(172, typeof(ExplicitBlockSer))]
    public class BlockSer
        : StatementSer
    {
        public List<StatementSer> Statements { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NullExpression
        : ExpressionSer
    {
        public bool None { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class ByteLiteralExpression : ExpressionSer
	{ public byte Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class SByteLiteralExpression : ExpressionSer
	{ public sbyte Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class CharLiteralExpression : ExpressionSer
	{ public char Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class ShortLiteralExpression : ExpressionSer
	{ public short Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class UShortLiteralExpression : ExpressionSer
	{ public ushort Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class IntLiteralExpression : ExpressionSer
	{ public int Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class UIntLiteralExpression : ExpressionSer
	{ public uint Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class LongLiteralExpression : ExpressionSer
	{ public long Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class ULongLiteralExpression : ExpressionSer
	{ public ulong Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class FloatLiteralExpression : ExpressionSer
	{ public float Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class DoubleLiteralExpression : ExpressionSer
	{ public double Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class BoolLiteralExpression : ExpressionSer
	{ public bool Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class StringLiteralExpression : ExpressionSer
	{ public string Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
	public class DecimalLiteralExpression : ExpressionSer
	{ public decimal Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class MethodCallArg
        : AstBase
    {
        public bool IsByRef { get; set; }

        public ExpressionSer Value { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(StrCatExpression)), ProtoInclude(112, typeof(StrCatExpression))]
    [NetJSONKnownType(typeof(NewExpression)), ProtoInclude(113, typeof(NewExpression))]
    [NetJSONKnownType(typeof(ConstructorInitializerExpression)), ProtoInclude(124, typeof(ConstructorInitializerExpression))]
    [NetJSONKnownType(typeof(UserCastExpression)), ProtoInclude(127, typeof(UserCastExpression))]
    [NetJSONKnownType(typeof(UserDefinedBinaryOrUnaryOpExpression)), ProtoInclude(208, typeof(UserDefinedBinaryOrUnaryOpExpression))]
    public class MethodCallExpression
        : ExpressionSer
    {
        public int Method { get; set; }

        public ExpressionSer Instance { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    public class UserDefinedBinaryOrUnaryOpExpression
        : MethodCallExpression
    {
        public int Operator
        { get; set; }

        public bool IsLifted
        { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class StrCatExpression
        : MethodCallExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(NewInitializerExpression)), ProtoInclude(115, typeof(NewInitializerExpression))]
    [NetJSONKnownType(typeof(NewCollectionInitializerExpression)), ProtoInclude(212, typeof(NewCollectionInitializerExpression))]
    public class NewExpression
        : MethodCallExpression
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ObjectInitilaizer
        : AstBase
    {
        public int? Setter { get; set; }

        public int? Getter { get; set; }

        public int? Field { get; set; }

        public ExpressionSer Value { get; set; }

        public MethodCallExpression MethodCall { get; set; }

        public int? Property { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NewInitializerExpression
        : NewExpression
    {
        public List<ObjectInitilaizer> Initializers { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NewCollectionInitializerExpression
        : NewExpression
    {
        public List<MethodCallExpression> ItemInitializers { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NewAnonymoustype
        : ExpressionSer
    {
        public Dictionary<string, ExpressionSer> Initializers { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class MethodExpression
        : ExpressionSer
    {
        public int Method { get; set; }

        public List<int> GenericParameters { get; set; }

        public ExpressionSer Instance { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class FieldExpression
        : ExpressionSer
    {
        public int Field { get; set; }

        public ExpressionSer Instance { get; set; }
   }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(IndexExpression)), ProtoInclude(120, typeof(IndexExpression))]
    public class PropertyExpression
        : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public bool IsNotVirtual { get; set; }

        public int Property { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class EventExpression : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public bool IsNotVirtual { get; set; }

        public int Event { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class IndexExpression
        : PropertyExpression
    {
        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DynamicMemberExpression
        : ExpressionSer
    {
        public string MemberName { get; set; }

        public ExpressionSer Instance { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DynamicIndexBinderExpression
        : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public ExpressionSer Index { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DynamicMethodBinderExpression
        : ExpressionSer
    {
        public string Name { get; set; }

        public ExpressionSer Instance { get; set; }

        public ExpressionSer Value { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DynamicMethodInvocationExpression
        : ExpressionSer
    {
        public ExpressionSer Method { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DelegateInvocationExpression
        : ExpressionSer
    {
        public ExpressionSer Instance { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ConstructorInitializerExpression
        : MethodCallExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DefaultValueExpr
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TypeExpressionSer
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class UserCastExpression
        : MethodCallExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TypeCastExpression
        : ExpressionSer
    {
        public int Type { get; set; }

        public ExpressionSer Expression { get; set; }

        public bool IsUnbox { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NullableToNormal : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DelegateCreationExpression
        : ExpressionSer
    {
        public ExpressionSer Method { get; set; }

        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ArrayCreationExpression
        : ExpressionSer
    {
        public int ArrayType { get; set; }

        public int ElementType { get; set; }

        public List<ExpressionSer> Initializers { get; set; }

        public List<ExpressionSer> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(BaseThisExpression)), ProtoInclude(132, typeof(BaseThisExpression))]
    public class ThisExpression
        : ExpressionSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class BaseThisExpression
        : ThisExpression
    {
        public bool NoUse2 { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TypeOfExpression
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ElementAccessExpression
        : ExpressionSer
    {
        public ExpressionSer Left { get; set; }

        public List<MethodCallArg> Arguments { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(CompoundAssignExpression)), ProtoInclude(136, typeof(CompoundAssignExpression))]
    public class AssignExpression
        : ExpressionSer
    {
        public ExpressionSer Left { get; set; }

        public ExpressionSer Right { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class CompoundAssignExpression
        : AssignExpression
    {
        public int Operator { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class BoxCastExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NullConstantExpression
        : ExpressionSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class BoolConstantExpression
        : ExpressionSer
    { public bool Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ByteConstantExpression
        : ExpressionSer
    { public byte Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SbyteConstantExpression
        : ExpressionSer
    { public sbyte Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class CharConstantExpression
        : ExpressionSer
    { public char Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ShortConstantExpression
        : ExpressionSer
    { public short Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class UshortConstantExpression
        : ExpressionSer
    { public ushort Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class IntConstantExpression
        : ExpressionSer
    { public int Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class UintConstantExpression
        : ExpressionSer
    { public uint Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class LongConstantExpression
        : ExpressionSer
    { public long Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class UlongConstantExpression
        : ExpressionSer
    { public ulong Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class FloatConstantExpression
        : ExpressionSer
    { public float Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DoubleConstantExpression
        : ExpressionSer
    { public double Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class StringConstantExpression
        : ExpressionSer
    { public string Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DecimalConstantExpression
        : ExpressionSer
    { public decimal Value { get; set; } }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ThrowExpression
        : StatementSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class UnwrapExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class WrapExpression
        : ExpressionSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class LiftedNullExpression
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NullCoalescingOperatorSer
        : ExpressionSer
    {
        public ExpressionSer Left { get; set; }

        public ExpressionSer Right { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class UnaryExpression
        : ExpressionSer
    {
        public int Operator { get; set; }

        public ExpressionSer Expression { get; set; }

        public bool IsLifted { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class BinaryExpression
        : ExpressionSer
    {
        public int Operator { get; set; }

        public ExpressionSer Left { get; set; }

        public ExpressionSer Right { get; set; }

        public bool IsLifted { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class IsExpression
        : ExpressionSer
    {
        public int Type { get; set; }

        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class AsExpression
        : ExpressionSer
    {
        public int Type { get; set; }

        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ConditionalExpression
        : ExpressionSer
    {
        public ExpressionSer Condition { get; set; }

        public ExpressionSer TrueExpression { get; set; }

        public ExpressionSer FalseExpression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ConditionalAccess
        : ExpressionSer
    {
        public ExpressionSer Receiver
        { get; set; }

        public ExpressionSer AccessExpression
        { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ConditionalReceiver
        : ExpressionSer
    {
        public int Type { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(TempVariableRefExpression)), ProtoInclude(151, typeof(TempVariableRefExpression))]
    public class LocalVariableRefExpression
        : ExpressionSer
    {
        public bool IsAddressReference { get; set; }

        public LocalVariableSer LocalVariable { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TempVariableRefExpression
        : LocalVariableRefExpression
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ParameterReferenceExpression
        : ExpressionSer
    {
        public ParameterSer Parameter { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(YieldStatement)), ProtoInclude(154, typeof(YieldStatement))]
    public class StatementExpressionSer
        : StatementSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class YieldStatement
        : StatementExpressionSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class YieldBreakStatement
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class StatementListSer
        : StatementSer
    {
        public List<StatementSer> Statements { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ReturnStatement
        : StatementSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class BreakStatement
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ContinueStatement
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class EmptyStatementSer
        : StatementSer
    {
        public bool NoUse { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class VariableBlockDeclaration
        : StatementSer
    {
        public List<ExpressionSer> Initializers { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class IfStatement
        : StatementSer
    {
        public ExpressionSer Condition { get; set; }

        public StatementSer TrueStatement { get; set; }

        public StatementSer FalseStatement { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(DoStatement)), ProtoInclude(164, typeof(DoStatement))]
    [NetJSONKnownType(typeof(WhileStatement)), ProtoInclude(165, typeof(WhileStatement))]
    [NetJSONKnownType(typeof(ForStatement)), ProtoInclude(166, typeof(ForStatement))]
    [NetJSONKnownType(typeof(ForEachStatement)), ProtoInclude(167, typeof(ForEachStatement))]
    public class LoopStatement
        : StatementSer
    {
        public StatementSer Loop { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class DoStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class WhileStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ForStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }

        public StatementSer Initializer { get; set; }

        public StatementSer Iterator { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ForEachStatement
        : LoopStatement
    {
        public ExpressionSer Collection { get; set; }

        public string LocalVariableName { get; set; }

        new public StatementSer Loop { get; set; }

        public int BlockId { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SwitchCaseLabel
    {
        public ExpressionSer LabelValue { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SwitchSectionSer
        : AstBase
    {
        public List<SwitchCaseLabel> Labels { get; set; }

        public StatementSer Block { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SwitchStatement
        : StatementSer
    {
        public ExpressionSer SwitchExpression { get; set; }

        public List<SwitchSectionSer> Blocks { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TryFinallyBlockSer
        : StatementSer
    {
        public StatementSer TryBlock { get; set; }

        public StatementSer FinallyBlock { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [NetJSONKnownType(typeof(ParameterBlock)), ProtoInclude(175, typeof(ParameterBlock))]
    public class ExplicitBlockSer
        : BlockSer
    {
        public int Id { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class CatchBlock
        : StatementSer
    {
        public int? CatchType { get; set; }

        public LocalVariableSer LocalVariable { get; set; }

        public ExplicitBlockSer Block { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class TryCatchBlock
        : StatementSer
    {
        public StatementSer TryBlock { get; set; }

        public List<CatchBlock> CatchBlocks { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ParameterBlock
        : ExplicitBlockSer
    {
        public List<ParameterSer> Parameters { get; set; }

        public bool IsMethodOwned { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class IteratorBlock
        : ExpressionSer
    {
        public int Type { get; set; }

        public ParameterBlock Block { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class AnonymousMethodBodyExpr
        : ExpressionSer
    {
        public int Type { get; set; }

        public ParameterBlock Block { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class AnonymousMethodExpr
        : ExpressionSer
    {
        public ParameterBlock Block { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class MethodBody
        : AstBase
    {
        public int MethodId { get; set; }

        public string FileName { get; set; }

        public ParameterBlock Body { get; set; }

        public LocationSer ScriptBlockLocation { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class FullAst
    {
        public LinkedList<MethodBody> Methods { get; set; }

        public TypeInfoSer TypeInfo { get; set; }
    }
}
