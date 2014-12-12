//-----------------------------------------------------------------------
// <copyright file="SerializeTokens.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    /// <summary>
    /// Definition for SerializeTokens
    /// </summary>
    public static class SerializeTokens
    {
    }

    public static class TypeTokens
    {
        public const string ModuleDefinition = "moduleName";
        public const string TypeSpec = "typeSpec";
        public const string MethodSpec = "methodSpec";
        public const string FieldSpec = "fieldSpec";
        public const string PropertySpec = "propertySpec";
        public const string NullLiteral = "nullLiteral";
        public const string BoolLiteral = "boolLiteral";
        public const string CharLiteral = "charLiteral";
        public const string LongLiteral = "longLiteral";
        public const string IntLiteral = "intLiteral";
        public const string UIntLiteral = "uintLiteral";
        public const string StringLiteral = "strLiteral";
        public const string DecimalLiteral = "decimalLiteral";
        public const string DoubleLiteral = "doubleLiteral";
        public const string FloatLiteral = "floatLiteral";
        public const string Assignment = "assignment";
        public const string MethodCall = "methodCall";
        public const string BinaryExpr = "binaryExpr";
        public const string UnaryExpr = "unaryExpr";
        public const string TypeCast = "typeCast";
        public const string ByteLiteral = "byteLiteral";
        public const string SByteLiteral = "sbyteLiteral";
        public const string ShortLiteral = "shortLiteral";
        public const string UShortConstant = "ushortConstant";
        public const string EmptyStatement = "emptyStatement";
        public const string StatementExpr = "statementExpr";
        public const string StatementList = "statementList";
        public const string ReturnStatement = "returnStatement";
        public const string ThrowStatment = "throwStatment";
        public const string BreakExpression = "breakExpression";
        public const string ContinueExpression = "continueExpression";
        public const string IfStatement = "ifStatement";
        public const string DoWhileStatement = "doWhileStatement";
        public const string WhileStatement = "whileStatement";
        public const string ForStatement = "forStatement";
        public const string ULongLiteral = "ulongLiteral";
        public const string ForEachStatement = "forEachStatement";
        public const string SwitchStatement = "switchStatement";
        public const string SwitchSection = "switchSection";
        public const string Default = "default";
        public const string LabelLiteral = "labelLiteral";
        public const string ScopeBlock = "scopeBlock";
        public const string ParameterBlock = "parameterBlock";
        public const string ParameterType = "parameterType";
        public const string TryFinally = "tryFinally";
        public const string TryCatch = "tryCatch";
        public const string CatchBlock = "catchBlock";
        public const string NullCoalascing = "nullCoalascing";
        public const string Yield = "yield";
        public const string Iterator = "iterator";
        public const string TypeCheck = "typeCheck";
        public const string IsExpr = "isExpr";
        public const string AsExpr = "asExpr";
        public const string Conditional = "conditional";
        public const string VariableAddressReference = "varaibleAddressReference";
        public const string VariableReference = "variableReference";
        public const string ParameterReference = "parameterReference";
        public const string Invocation = "invocation";
        public const string New = "new";
        public const string ArrayCreation = "arrayCreation";
        public const string ThisExpr = "thisExpr";
        public const string TypeOf = "typeOf";
        public const string ArrayElementAccess = "arrayElementAccess";
        public const string BaseExpr = "baseExpr";
        public const string NewInitializer = "newInitializer";
        public const string MethodExpr = "methodExpr";
        public const string FieldExpr = "fieldExpr";
        public const string PropertyExpr = "propertyExpr";
        public const string IndexerExpr = "indexerExpr";
        public const string DelegateInvocation = "delegateInvocation";
        public const string DefaultValue = "defaultValue";
        public const string TempVariableAddressReference = "tempVariableAddressReference";
        public const string TempVariableReference = "tempVariableReference";
        public const string TypeExpr = "typeExpr";
        public const string DelegateCreation = "delegateCreation";
        public const string LocalVariable = "localVariable";
        public const string Argument = "argument";
        public const string MethodBody = "methodBody";
        public const string BoxExpr = "boxExpr";
        public const string UnBoxExpr = "unBoxExpr";
        public const string VariableInitializer = "varInitializer";
        public const string AnonymousMethod = "anonymousMethod";
        public const string WrapToNullable = "wrap";
        public const string UnwrapFromNullable = "unwrap";
        public const string DynamicMemberBinder = "dynamicMB";
        public const string DynamicIndexBinder = "dynamicIB";
        public const string NewAnonymousType = "nat";
        public const string StrCatExpr = "strcat";
    }

    public static class NameTokens
    {
        public const string Name = "name";
        public const string TypeName = "_t";
        public const string Type = "type";
        public const string Position = "position";
        public const string IsMethodOwned = "isMethodOwned";
        public const string ElementType = "elementType";
        public const string NameSpace = "nameSpace";
        public const string DeclaringType = "declaringType";
        public const string TypeParams = "typeParams";
        public const string ReturnType = "returnType";
        public const string IsStatic = "isStatic";
        public const string Arity = "arity";
        public const string ModFlags = "modFlags";
        public const string Parameters = "parameters";
        public const string MemberType = "memberType";
        public const string Value = "value";
        public const string Loc = "loc";
        public const string LeftExpr = "leftExpr";
        public const string RightExpr = "rightExpr";
        public const string Method = "method";
        public const string Instance = "instance";
        public const string Index = "index";
        public const string Arguments = "arguments";
        public const string Operator = "operator";
        public const string Expr = "expr";
        public const string Condition = "condition";
        public const string TrueStatement = "trueStatement";
        public const string FalseCondition = "falseCondition";
        public const string Loop = "loop";
        public const string Initializer = "initializer";
        public const string Iterator = "iterator";
        public const string LocalVariable = "localVariable";
        public const string Blocks = "blocks";
        public const string Labels = "labels";
        public const string Block = "block";
        public const string Id = "id";
        public const string HasThis = "hasThis";
        public const string TryBlock = "tryBlock";
        public const string FinallyBlock = "finallyBlock";
        public const string CatchBlocks = "catchBlocks";
        public const string Field = "field";
        public const string Getter = "getter";
        public const string Setter = "setter";
        public const string FileName = "fileName";
        public const string IsByRef = "isByRef";
        public const string Module = "module";
        public const string IsLifted = "isLifted";
    }

    public static class ValueTokens
    {
        public const string GenericParam = "genericParam";
        public const string Array = "array";
        public const string GenericInstance = "genericInstance";
    }
}