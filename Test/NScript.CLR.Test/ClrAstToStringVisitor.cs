namespace NScript.CLR.Test
{
    using Mono.Cecil;
    using NScript.CLR.AST;
    using System;
    using System.Linq;

    public class ClrAstToStringVisitor : AstVisitor<int, string>
    {
        public override string VisitExpression(AnonymousMethodBodyExpression node, int arg)
        {
            return $"{GetSpaces(arg)}({node.ResultType}){this.Visit(node.ParameterBlock, arg + 1)}";
        }

        public override string VisitExpression(AnonymousNewExpression node, int arg)
        {
            var rv = GetSpaces(arg) + "new {";
            foreach (var value in node.Values)
            { rv += value.Item1 + " :" + this.Visit(value.Item2, arg + 1) + ","; }

            return rv + GetSpaces(arg) + "}";
        }

        public override string VisitExpression(ArrayElementExpression node, int arg)
        {
            return this.Visit(node.Array, arg) + "["
                + this.Visit(node.Index, arg + 1) + GetSpaces(arg) + "]";
        }

        public override string VisitExpression(BaseVariableReference node, int arg)
        { return GetSpaces(arg) + "this"; }

        public override string VisitExpression(BinaryExpression node, int arg)
        {
            return GetSpaces(arg)
                + $"{node.Operator} l:{node.IsLifted} rt:{node.ResultType}"
                + this.Visit(node.Left, arg + 1)
                + GetSpaces(arg) + ","
                + this.Visit(node.Right, arg + 1);
        }

        public override string VisitExpression(BooleanLiteral node, int arg)
        {
            return GetSpaces(arg) + "bool " + node.Value;
        }

        public override string VisitExpression(BoxExpression node, int arg)
        {
            return GetSpaces(arg) + "Box("
                + this.Visit(node.BoxedExpression, arg + 1)
                + GetSpaces(arg) + ")";
        }

        public override string VisitExpression(CharLiteral node, int arg)
        {
            return GetSpaces(arg) + "char " + node.Value;
        }

        public override string VisitExpression(ConditionalOperatorExpression node, int arg)
        {
            return GetSpaces(arg) + "rt:" + node.ResultType
                + this.Visit(node.Condition, arg + 1)
                + GetSpaces(arg) + "?"
                + this.Visit(node.TrueCase, arg + 1)
                + GetSpaces(arg) + ":"
                + this.Visit(node.FalseCase, arg + 1);
        }

        public override string VisitExpression(ConstructorReferenceExpression node, int arg)
        { return GetSpaces(arg) + node.Constructor; }

        public override string VisitExpression(ConvertTypeExpression node, int arg)
        {
            return GetSpaces(arg) + node.ResultType
                + this.Visit(node.NestedExpression, arg + 1);
        }

        public override string VisitExpression(DefaultValueExpression node, int arg)
        {
            return GetSpaces(arg) + $"default({node.ResultType})";
        }

        public override string VisitExpression(DoubleLiteral node, int arg)
        {
            return GetSpaces(arg) + (node.IsSingle ? "float " : "double " ) + node.Value;
        }

        public override string VisitExpression(DynamicIndexAccessor node, int arg)
        {
            return GetSpaces(arg)
                + this.Visit(node.InstanceExpression, arg + 1)
                + GetSpaces(arg) + "["
                + this.Visit(node.IndexExpression, arg + 1)
                + GetSpaces(arg) + "]";
        }

        public override string VisitExpression(DynamicMemberAccessor node, int arg)
        {
            return GetSpaces(arg) + "DynamicMemberAccessor"
                + this.Visit(node.InstanceExpression, arg + 1)
                + GetSpaces(arg) + "." + node.MemberName;
        }

        public override string VisitExpression(FieldReferenceExpression node, int arg)
        {
            return node.LeftExpression == null
                ? GetSpaces(arg) + "static? FieldReference " + node.FieldReference.Name
                : GetSpaces(arg) + "FieldReference"
                    + this.Visit(node.LeftExpression, arg + 1)
                    + GetSpaces(arg + 1) + "." + node.FieldReference.Name;
        }

        public override string VisitExpression(InitObjectWithDefaultValue node, int arg)
        {
            throw new NotImplementedException();
        }

        public override string VisitExpression(InlineIteratorExpression node, int arg)
        {
            return GetSpaces(arg) + node.ResultType
                + this.Visit(node.IteratorBlock, arg + 1);
        }

        public override string VisitExpression(InlinePropertyInitilizationExpression node, int arg)
        {
            var rv = GetSpaces(arg) + node.ResultType
                + this.Visit(node.Constructor, arg + 1)
                + GetSpaces(arg) + "{";

            foreach (var tupl in node.Setters)
            {
                rv += this.Visit(tupl.Item1, arg + 1) + ":";
                if (tupl.Item2.Length == 1)
                {
                    rv += this.Visit(tupl.Item2[0], arg + 1) + ",";
                }
                else
                {
                    foreach (var expr in tupl.Item2)
                    {
                        rv += this.Visit(expr, arg + 1)
                            + GetSpaces(arg) + ",";
                    }
                }
            }

            rv += GetSpaces(arg) + "}";

            return rv;
        }

        public override string VisitExpression(IntLiteral node, int arg)
        {
            return GetSpaces(arg) + node.Size + " " + node.Value;
        }

        public override string VisitExpression(LoadAddressExpression node, int arg)
        {
            return GetSpaces(arg) + "ref/out"
                + this.Visit(node.NestedExpression, arg + 1);
        }

        public override string VisitExpression(MemberReferenceExpression node, int arg)
        {
            return node.LeftExpression == null
                ? GetSpaces(arg) + "static? MemberReference " + node.MemberReference.Name
                : GetSpaces(arg) + "MemberReference"
                    + this.Visit(node.LeftExpression, arg + 1)
                    + GetSpaces(arg + 1) + "." + node.MemberReference.Name;
        }

        public override string VisitExpression(MethodCallExpression node, int arg)
        {
            var rv = this.Visit(node.MethodReference, arg) + "("
                + string.Join(",",
                    Enumerable.Range(0, node.Parameters.Count)
                        .Select(_ => this.Visit(node.Parameters[_] , arg + 1))
                        .ToArray());

            if (node.Parameters.Count > 0)
            { rv += GetSpaces(arg) + ")"; }
            else
            { rv += ")"; }

            return rv;
        }

        public override string VisitExpression(MethodReferenceExpression node, int arg)
        {
            return node.LeftExpression == null
                ? GetSpaces(arg) + "static? MethodReference " + node.MethodReference.Name
                : GetSpaces(arg) + "MethodReference"
                    + this.Visit(node.LeftExpression, arg + 1)
                    + GetSpaces(arg + 1) + "." + node.MethodReference.Name;
        }

        public override string VisitExpression(NewArrayExpression node, int arg)
        { return GetSpaces(arg) + "new " + node.ResultType.GetElementType() + "["
                + this.Visit(node.Size, arg + 1)
                + GetSpaces(arg) + "]"; }

        public override string VisitExpression(NewObjectExpression node, int arg)
        {
            var rv = GetSpaces(arg) + node.ResultType + "(";
            if (node.Parameters.Count > 0)
            {
                foreach (var par in node.Parameters)
                { rv += this.Visit(par, arg + 1) + ","; }

                rv += GetSpaces(arg + 1) + ")";
            }
            else
            { rv += ")"; }

            return rv;
        }

        public override string VisitExpression(NullConditional node, int arg)
        {
            return GetSpaces(arg) + node.ResultType
                + this.Visit(node.FirstChoice, arg + 1)
                + GetSpaces(arg) + "??"
                + this.Visit(node.Alternate, arg + 1);
        }

        public override string VisitExpression(NullLiteral node, int arg)
        {
            return GetSpaces(arg) + "null";
        }

        public override string VisitExpression(PropertyReferenceExpression node, int arg)
        {
            return node.LeftExpression == null
                ? GetSpaces(arg) + "static? PropertyReference " + node.PropertyReference.Name
                : GetSpaces(arg) + "PropertyReference"
                    + this.Visit(node.LeftExpression, arg + 1)
                    + GetSpaces(arg + 1) + "." + node.PropertyReference.Name;
        }

        public override string VisitExpression(StringLiteral node, int arg)
        {
            return GetSpaces(arg) + "string \"" + node.String + '"';
        }

        public override string VisitExpression(ToNullable node, int arg)
        {
            return GetSpaces(arg) + node.ResultType
                + this.Visit(node.InnerExpression, arg + 1);
        }

        public override string VisitExpression(TypeCheckExpression node, int arg)
        {
            return GetSpaces(arg) + node.Type + " " + node.CheckType
                + this.Visit(node.Expression, arg + 1);
        }

        public override string VisitExpression(TypeofExpression node, int arg)
        {
            return GetSpaces(arg) + $"typeof({node.InnerExpression.Type})";
        }

        public override string VisitExpression(TypeReferenceExpression node, int arg)
        {
            return GetSpaces(arg) + node.Type;
        }

        public override string VisitExpression(UIntLiteral node, int arg)
        {
            return GetSpaces(arg) + "U" + node.Size + " " + node.Value;
        }

        public override string VisitExpression(UnaryExpression node, int arg)
        {
            return GetSpaces(arg) + $"{node.Operator}, is:{node.IsLifted}"
                + this.Visit(node.Expression, arg + 1);
        }

        public override string VisitExpression(UnboxExpression node, int arg)
        {
            return GetSpaces(arg) + "Unbox("
                + this.Visit(node.InnerExpression, arg + 1)
                + GetSpaces(arg + 1) +  ")";
        }

        public override string VisitExpression(VariableAddressReference node, int arg)
        {
            return GetSpaces(arg) + "ref"
                + this.Visit(node.NestedExpression, arg + 1);
        }

        public override string VisitExpression(VariableReference node, int arg)
        {
            return GetSpaces(arg) + node.Variable.Name;
        }

        public override string VisitExpression(VirtualMethodReferenceExpression node, int arg)
        {
            return GetSpaces(arg) + "virtual"
                + this.Visit(node.LeftExpression, arg + 1)
                + GetSpaces(arg + 1) + "." + node.MethodReference;
        }

        public override string VisitNull(int arg)
        { return string.Empty; }

        public override string VisitStatement(BreakStatement node, int arg)
        {
            return GetSpaces(arg) + "break";
        }

        public override string VisitStatement(ContinueStatement node, int arg)
        {
            return GetSpaces(arg) + "continue";
        }

        public override string VisitStatement(DebuggerBreakStatement node, int arg)
        {
            return GetSpaces(arg) + "DebugBreak";
        }

        public override string VisitStatement(DoWhileLoop node, int arg)
        {
            return GetSpaces(arg) + "doWhile("
                + this.Visit(node.Condition, arg + 1)
                + GetSpaces(arg) + ") {"
                + this.Visit(node.LoopBlock, arg + 1)
                + GetSpaces(arg) + "}";
        }

        public override string VisitStatement(ExplicitBlock node, int arg)
        {
            return GetSpaces(arg) + "ExplicitBlock {"
                + string.Concat(node.Statements.Select(_ => this.Visit(_, arg + 1)).ToArray())
                + GetSpaces(arg, true) + "}";
        }

        public override string VisitStatement(ExpressionStatement node, int arg)
        { return this.Visit(node.Expression, arg) + ";"; }

        public override string VisitStatement(ForEachLoop node, int arg)
        {
            return GetSpaces(arg) + "foreach("
                + node.Variable.Name + " in "
                + this.Visit(node.Collection, arg + 1)
                + GetSpaces(arg) + ") {"
                + this.Visit(node.Scope, arg + 1)
                + GetSpaces(arg) + "}";
        }

        public override string VisitStatement(ForLoop node, int arg)
        {
            return GetSpaces(arg) + "for("
                + this.Visit(node.InitializeStatement, arg + 1)
                + GetSpaces(arg) + ";"
                + this.Visit(node.Condition, arg + 1)
                + GetSpaces(arg) + ";"
                + this.Visit(node.IncrementStatement, arg + 1)
                + GetSpaces(arg) + ")"
                + this.VisitStatement((ExplicitBlock)node, arg);
        }

        public override string VisitStatement(HandlerBlock node, int arg)
        {
            if (node.IsCatchBlock)
            {
                return GetSpaces(arg) + $"catch({node.CatchType} {node.CatchVariable?.Variable.Name}) {{"
                    + this.Visit(node.Block, arg + 1)
                    + GetSpaces(arg) + "}";
            }
            else
            {
                return GetSpaces(arg) + "finally {"
                    + this.Visit(node.Block, arg + 1)
                    + GetSpaces(arg) + "}";
            }
        }

        public override string VisitStatement(IfBlockStatement node, int arg)
        {
            var rv = GetSpaces(arg) + "if ("
                + this.Visit(node.Condition, arg + 1)
                + GetSpaces(arg) + ")"
                + this.Visit(node.TrueCaseStatements, arg + 1);

            if (node.FalseCaseStatements != null)
            {
                rv += GetSpaces(arg) + "else "
                    + this.Visit(node.FalseCaseStatements, arg + 1);
            }

            return rv;
        }

        public override string VisitStatement(InitializerStatement node, int arg)
        {
            return GetSpaces(arg) + "initializers: "
                + string.Concat(node.Initializers.Select(_ => this.Visit(_, arg + 1)).ToArray());
        }

        public override string VisitStatement(ParameterBlock node, int arg)
        {
            return GetSpaces(arg) + "ParamBlock("
                + string.Join(
                    ", ",
                    node.Parameters.Select(_ => $"{ToString(_.ParameterType)}{_.Type} {_.ParameterReference.Name}")) + ") "
                + this.VisitStatement((ExplicitBlock)node, arg);
        }

        public override string VisitStatement(ReturnStatement node, int arg)
        {
            if (node.ReturnExpression != null)
            {
                return GetSpaces(arg) + "return"
                    + this.Visit(node.ReturnExpression, arg + 1);
            }

            return GetSpaces(arg) + "return";
        }

        public override string VisitStatement(ScopeBlock node, int arg)
        {
            return GetSpaces(arg) + "ScopeBlock("
                + string.Join(", ", node.LocalVariables.Select(_ => _.Name).ToList()) + ") {"
                + string.Concat(node.Statements.Select(_ => this.Visit(_, arg + 1)).ToArray())
                + GetSpaces(arg) + "}";
        }

        public override string VisitStatement(SwitchStatement node, int arg)
        {
            return GetSpaces(arg) + "switch("
                + this.Visit(node.SwitchValue, arg + 1)
                + GetSpaces(arg) + ") {"
                + string.Concat(
                    node
                        .CaseBlocks
                        .Select(_ =>
                            string.Concat(
                                _.Key
                                    .Select(_k => GetSpaces(arg) + (_k == null ? "default" : ("case " + this.Visit(_k, arg + 1))))
                                    .ToArray()))
                        .ToArray())
                + GetSpaces(arg) + "}";
        }

        public override string VisitStatement(ThrowStatement node, int arg)
        {
            if (node.Expression != null)
            {
                return GetSpaces(arg) + "throw"
                    + this.Visit(node.Expression, arg + 1);
            }

            return GetSpaces(arg) + "throw";
        }

        public override string VisitStatement(TryCatchFinally node, int arg)
        {
            return GetSpaces(arg) + "try"
                + this.Visit(node.TryBlock, arg + 1)
                + string.Concat(
                    node.Handlers
                        .Select(_ => this.Visit(_, arg))
                        .ToArray());
        }

        public override string VisitStatement(WhileLoop node, int arg)
        {
            return GetSpaces(arg) + "While("
                + this.Visit(node.Condition, arg + 1)
                + GetSpaces(arg) + ") {"
                + this.Visit(node.LoopBlock, arg + 1)
                + GetSpaces(arg) + "}";
        }

        public override string VisitStatement(YieldStatement node, int arg)
        {
            if (node.IsBreak)
            { return GetSpaces(arg) + "yield break"; }

            return GetSpaces(arg) + "yield return"
                + this.Visit(node.YieldValue, arg + 1);
        }

        private static string GetSpaces(int i, bool forceNewLine = false)
            => i == 0 && !forceNewLine ? string.Empty : "\r\n" + string.Join("", Enumerable.Range(0, i).Select(_ => "  ").ToArray());

        public static string ToString(ParameterAttributes attrs)
        {
            if (attrs == ParameterAttributes.None)
            { return string.Empty; }
            else if (attrs == (ParameterAttributes.In | ParameterAttributes.Out))
            { return "ref "; }
            else if (attrs == ParameterAttributes.Out)
            { return "out "; }
            else
            {
                return attrs.ToString() + " ";
            }
        }

        public override string VisitExpression(InlineArrayInitialization node, int arg)
        {
            return GetSpaces(arg) + $"{node.ElementType}["
                + (node.SizeExpression != null ? (this.Visit(node.SizeExpression, arg + 1) + GetSpaces(arg)) : string.Empty) + "]"
                + GetSpaces(arg) + "{"
                + string.Join(
                    ",",
                    node.ElementInitValues
                        .Select(_ => this.Visit(_, arg + 1))
                        .ToList())
                + GetSpaces(arg) + "}";
        }
    }
}
