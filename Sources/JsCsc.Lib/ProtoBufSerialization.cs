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
    //  MaxId = 208;

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ModuleSpecSer
    {
        public string Name { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(100, typeof(GenericParamSer))]
    [ProtoInclude(101, typeof(ArrayTypeSer))]
    [ProtoInclude(102, typeof(GenericInstanceTypeSer))]
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
        public TypeSpecSer DeclaringType { get; set; }

        public TypeSpecSer MemberType { get; set; }

        public string Name { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class EventSpecSer
    {
        public TypeSpecSer DeclaringType { get; set; }

        public TypeSpecSer MemberType { get; set; }

        public string Name { get; set; }
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
    [ProtoInclude(103, typeof(LocalVariableSer))]
    [ProtoInclude(105, typeof(ExpressionSer))]
    [ProtoInclude(106, typeof(StatementSer))]
    [ProtoInclude(110, typeof(MethodCallArg))]
    [ProtoInclude(169, typeof(SwitchSectionSer))]
    [ProtoInclude(114, typeof(ObjectInitilaizer))]
    [ProtoInclude(179, typeof(MethodBody))]
    public class AstBase
    {
        public LocationSer Location { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(104, typeof(ParameterSer))]
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
    public class ExpressionSer
        : AstBase
    {
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(140, typeof(ThrowExpression))]
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
    public class StatementSer
        : AstBase
    {
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(172, typeof(ExplicitBlockSer))]
    public class BlockSer
        : StatementSer
    {
        public List<StatementSer> Statements { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NullExpression
        : ExpressionSer
    {
        
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
    {
        public bool IsByRef { get; set; }

        public ExpressionSer Value { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(112, typeof(StrCatExpression))]
    [ProtoInclude(113, typeof(NewExpression))]
    [ProtoInclude(124, typeof(ConstructorInitializerExpression))]
    [ProtoInclude(127, typeof(UserCastExpression))]
    [ProtoInclude(208, typeof(UserDefinedBinaryOrUnaryOpExpression))]
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
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(115, typeof(NewInitializerExpression))]
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

        public int? Field { get; set; }

        public ExpressionSer Value { get; set; }

        public MethodCallExpression MethodCall { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class NewInitializerExpression
        : NewExpression
    {
        public List<ObjectInitilaizer> Initializers { get; set; }
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
    [ProtoInclude(120, typeof(IndexExpression))]
    public class PropertyExpression
        : ExpressionSer
    {
        public int Getter { get; set; }

        public int Setter { get; set; }

        public ExpressionSer Instance { get; set; }

        public bool IsNotVirtual { get; set; }

        public int Property { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class IndexExpression
        : PropertyExpression
    {
        public List<MethodCallArg> Arguments { get; set; }
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
    [ProtoInclude(132, typeof(BaseThisExpression))]
    public class ThisExpression
        : ExpressionSer
    {
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class BaseThisExpression
        : ThisExpression
    {
        
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
    [ProtoInclude(136, typeof(CompoundAssignExpression))]
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
    { }

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
    [ProtoInclude(151, typeof(TempVariableRefExpression))]
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
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ParameterReferenceExpression
        : ExpressionSer
    {
        public ParameterSer Parameter { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [ProtoInclude(154, typeof(YieldStatement))]
    public class StatementExpressionSer
        : StatementSer
    {
        public ExpressionSer Expression { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class YieldStatement
        : StatementExpressionSer
    {
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class YieldBreakStatement
        : StatementSer
    {
        
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
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ContinueStatement
        : StatementSer
    {
        
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class EmptyStatementSer
        : StatementSer
    {
        
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
    [ProtoInclude(164, typeof(DoStatement))]
    [ProtoInclude(165, typeof(WhileStatement))]
    [ProtoInclude(166, typeof(ForStatement))]
    [ProtoInclude(167, typeof(ForEachStatement))]
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

        new public StatementSer Loop { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class WhileStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }

        new public StatementSer Loop { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ForStatement
        : LoopStatement
    {
        public ExpressionSer Condition { get; set; }

        public StatementSer Initializer { get; set; }

        public StatementSer Iterator { get; set; }

        new public StatementSer Loop { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class ForEachStatement
        : LoopStatement
    {
        public ExpressionSer Collection { get; set; }

        public string LocalVariableName { get; set; }

        new public StatementSer Loop { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SwitchCaseLabel
    {
        public ExpressionSer LabelValue { get; set; }
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SwitchSectionSer
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
    [ProtoInclude(175, typeof(ParameterBlock))]
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
    }

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class FullAst
    {
        public LinkedList<MethodBody> Methods { get; set; }

        public TypeInfoSer TypeInfo { get; set; }
    }
}
