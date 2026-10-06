using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NScript.JST;
using NScript.Utils;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Converts a Razor binding expression (C# source text) into a JST expression.
    /// Accepted syntax: literals, parentheses, conditionals, <c>! -</c>,
    /// <c>&amp;&amp; || == != &lt; &lt;= &gt; &gt;= + - * / %</c>, null-coalescing (<c>??</c>),
    /// dotted name paths of any depth, and instance method invocations (<c>recv.Method(args)</c>).
    /// Every name path is handed to <c>resolvePath</c> and every invocation to
    /// <c>resolveInvocation</c>; both must return a fully resolved JST expression (scope-system
    /// identifiers only) or <c>null</c> when they cannot. Anything unsupported or unresolvable
    /// throws a <see cref="RazorSubControlDiagnosticException"/> so the build fails with the
    /// expression text instead of emitting a getter that silently reads <c>undefined</c>.
    /// </summary>
    internal static class BindingExpressionConverter
    {
        /// <summary>
        /// Resolves an instance method invocation: given the receiver's dotted segments, the
        /// method name, and the already-converted argument expressions, returns the resolved
        /// JST call expression, or <c>null</c> when the method cannot be resolved.
        /// </summary>
        internal delegate Expression InvocationResolver(
            IReadOnlyList<string> receiverSegments, string methodName, IReadOnlyList<Expression> arguments);

        internal static Expression Convert(
            string csharpExpression,
            IdentifierScope scope,
            Func<IReadOnlyList<string>, Expression> resolvePath,
            InvocationResolver resolveInvocation,
            Location location)
        {
            if (resolvePath == null) throw new ArgumentNullException(nameof(resolvePath));
            if (string.IsNullOrWhiteSpace(csharpExpression))
                throw Unsupported(csharpExpression, location, "the expression is empty");

            var syntax = SyntaxFactory.ParseExpression(csharpExpression);
            if (syntax.ContainsDiagnostics)
                throw Unsupported(csharpExpression, location, "it is not a valid C# expression");

            return new Converter(csharpExpression, scope, resolvePath, resolveInvocation, location).Visit(syntax);
        }

        /// <summary>
        /// Returns every maximal dotted name path read by <paramref name="csharpExpression"/>
        /// (e.g. <c>"c-" + Control.Label</c> yields <c>[Control, Label]</c>). Returns an empty
        /// list when the text does not parse; <see cref="Convert"/> reports that case.
        /// </summary>
        internal static List<IReadOnlyList<string>> CollectMemberPaths(string csharpExpression)
        {
            var paths = new List<IReadOnlyList<string>>();
            if (string.IsNullOrWhiteSpace(csharpExpression))
                return paths;

            var syntax = SyntaxFactory.ParseExpression(csharpExpression);
            if (syntax.ContainsDiagnostics)
                return paths;

            foreach (var node in syntax.DescendantNodesAndSelf().OfType<ExpressionSyntax>())
            {
                // Only the outermost node of a chain: skip `Model` in `Model.Flag`
                // and the `Flag` name part itself.
                if (node.Parent is MemberAccessExpressionSyntax parent
                    && parent.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                    continue;

                var segments = new List<string>();
                if (Converter.TryFlatten(node, segments))
                    paths.Add(segments);
            }
            return paths;
        }

        /// <summary>
        /// An instance method call found in a binding expression: the receiver's dotted segments,
        /// the method name, and its argument count. Used to retain invoked methods for emission.
        /// </summary>
        internal readonly struct InvocationRef
        {
            internal InvocationRef(IReadOnlyList<string> receiverSegments, string methodName, int argumentCount)
            {
                ReceiverSegments = receiverSegments;
                MethodName = methodName;
                ArgumentCount = argumentCount;
            }

            internal IReadOnlyList<string> ReceiverSegments { get; }
            internal string MethodName { get; }
            internal int ArgumentCount { get; }
        }

        /// <summary>
        /// Returns every instance method invocation <c>recv.Method(args)</c> whose receiver is a
        /// dotted name path (e.g. <c>Model.Decorate(Model.Name)</c> yields receiver <c>[Model]</c>,
        /// method <c>Decorate</c>, one argument). Returns an empty list when the text does not parse.
        /// Lets the plugin retain invoked methods against dead-code elimination (ADR-0022).
        /// </summary>
        internal static List<InvocationRef> CollectInvocations(string csharpExpression)
        {
            var invocations = new List<InvocationRef>();
            if (string.IsNullOrWhiteSpace(csharpExpression))
                return invocations;

            var syntax = SyntaxFactory.ParseExpression(csharpExpression);
            if (syntax.ContainsDiagnostics)
                return invocations;

            foreach (var invocation in syntax.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                if (!(invocation.Expression is MemberAccessExpressionSyntax callee
                        && callee.Kind() == SyntaxKind.SimpleMemberAccessExpression
                        && callee.Name is IdentifierNameSyntax methodName))
                    continue;

                var segments = new List<string>();
                if (Converter.TryFlatten(callee.Expression, segments))
                    invocations.Add(new InvocationRef(
                        segments, methodName.Identifier.ValueText, invocation.ArgumentList.Arguments.Count));
            }
            return invocations;
        }

        private static RazorSubControlDiagnosticException Unsupported(
            string expression, Location location, string reason)
            => new RazorSubControlDiagnosticException(location,
                "Unsupported Razor binding expression '" + expression + "': " + reason
                + "; precompute it in C#.");

        private sealed class Converter
        {
            private readonly string _text;
            private readonly IdentifierScope _scope;
            private readonly Func<IReadOnlyList<string>, Expression> _resolvePath;
            private readonly InvocationResolver _resolveInvocation;
            private readonly Location _location;
            private int _coalesceTemp;

            internal Converter(string text, IdentifierScope scope,
                Func<IReadOnlyList<string>, Expression> resolvePath,
                InvocationResolver resolveInvocation, Location location)
            {
                _text = text;
                _scope = scope;
                _resolvePath = resolvePath;
                _resolveInvocation = resolveInvocation;
                _location = location;
            }

            internal Expression Visit(ExpressionSyntax node)
            {
                switch (node)
                {
                    case ParenthesizedExpressionSyntax parenthesized:
                        // The JST writer re-adds parentheses from operator precedence.
                        return Visit(parenthesized.Expression);

                    case ConditionalExpressionSyntax conditional:
                        return new ConditionalOperatorExpression(null, _scope,
                            Visit(conditional.Condition),
                            Visit(conditional.WhenTrue),
                            Visit(conditional.WhenFalse));

                    case BinaryExpressionSyntax binary:
                        return VisitBinary(binary);

                    case PrefixUnaryExpressionSyntax unary:
                        return VisitUnary(unary);

                    case LiteralExpressionSyntax literal:
                        return VisitLiteral(literal);

                    case InvocationExpressionSyntax invocation:
                        return VisitInvocation(invocation);

                    case IdentifierNameSyntax _:
                    case MemberAccessExpressionSyntax _:
                        return VisitPath(node);

                    default:
                        throw Fail(Describe(node) + " is not supported");
                }
            }

            private Expression VisitBinary(BinaryExpressionSyntax binary)
            {
                // `??` has no JST operator; desugar it to a conditional (handled separately).
                if (binary.Kind() == SyntaxKind.CoalesceExpression)
                    return VisitCoalesce(binary);

                BinaryOperator op;
                switch (binary.Kind())
                {
                    case SyntaxKind.LogicalAndExpression: op = BinaryOperator.LogicalAnd; break;
                    case SyntaxKind.LogicalOrExpression: op = BinaryOperator.LogicalOr; break;
                    // Loose equality: C# operands of `==` share a type, so `==` and `===`
                    // differ only on null vs undefined, where loose matches C# null semantics.
                    case SyntaxKind.EqualsExpression: op = BinaryOperator.Equals; break;
                    case SyntaxKind.NotEqualsExpression: op = BinaryOperator.NotEquals; break;
                    case SyntaxKind.LessThanExpression: op = BinaryOperator.LessThan; break;
                    case SyntaxKind.LessThanOrEqualExpression: op = BinaryOperator.LessThanOrEqual; break;
                    case SyntaxKind.GreaterThanExpression: op = BinaryOperator.GreaterThan; break;
                    case SyntaxKind.GreaterThanOrEqualExpression: op = BinaryOperator.GreaterThanOrEqual; break;
                    case SyntaxKind.AddExpression: op = BinaryOperator.Plus; break;
                    case SyntaxKind.SubtractExpression: op = BinaryOperator.Minus; break;
                    case SyntaxKind.MultiplyExpression: op = BinaryOperator.Mul; break;
                    case SyntaxKind.DivideExpression: op = BinaryOperator.Div; break;
                    case SyntaxKind.ModuloExpression: op = BinaryOperator.Mod; break;
                    default:
                        throw Fail("operator '" + binary.OperatorToken.Text + "' is not supported");
                }

                return new BinaryExpression(null, _scope, op, Visit(binary.Left), Visit(binary.Right));
            }

            /// <summary>
            /// Desugars <c>a ?? b</c> to <c>(t = a) != null ? t : b</c> with a fresh temp so the
            /// left operand is evaluated once. <c>!= null</c> is loose, matching nullish semantics
            /// (both null and undefined fall through to <c>b</c>). The temp is a scoped local that
            /// the enclosing getter function declares as <c>var</c>.
            /// </summary>
            private Expression VisitCoalesce(BinaryExpressionSyntax binary)
            {
                var temp = SimpleIdentifier.CreateScopeIdentifier(_scope, "nc" + _coalesceTemp++, true);
                var left = Visit(binary.Left);
                var right = Visit(binary.Right);

                var assign = new BinaryExpression(null, _scope, BinaryOperator.Assignment,
                    new IdentifierExpression(temp, _scope), left);
                var test = new BinaryExpression(null, _scope, BinaryOperator.NotEquals,
                    assign, new NullLiteralExpression(_scope));
                return new ConditionalOperatorExpression(null, _scope, test,
                    new IdentifierExpression(temp, _scope), right);
            }

            private Expression VisitUnary(PrefixUnaryExpressionSyntax unary)
            {
                switch (unary.Kind())
                {
                    case SyntaxKind.LogicalNotExpression:
                        return new UnaryExpression(null, _scope, UnaryOperator.LogicalNot, Visit(unary.Operand));
                    case SyntaxKind.UnaryMinusExpression:
                        return new UnaryExpression(null, _scope, UnaryOperator.UnaryMinus, Visit(unary.Operand));
                    default:
                        throw Fail("operator '" + unary.OperatorToken.Text + "' is not supported");
                }
            }

            private Expression VisitLiteral(LiteralExpressionSyntax literal)
            {
                switch (literal.Kind())
                {
                    case SyntaxKind.StringLiteralExpression:
                        return new StringLiteralExpression(_scope, literal.Token.ValueText);
                    case SyntaxKind.TrueLiteralExpression:
                        return new BooleanLiteralExpression(_scope, true);
                    case SyntaxKind.FalseLiteralExpression:
                        return new BooleanLiteralExpression(_scope, false);
                    case SyntaxKind.NullLiteralExpression:
                        return new NullLiteralExpression(_scope);
                    case SyntaxKind.NumericLiteralExpression:
                        switch (literal.Token.Value)
                        {
                            case int i: return new NumberLiteralExpression(_scope, i);
                            case uint u: return new NumberLiteralExpression(_scope, u);
                            case long l: return new NumberLiteralExpression(_scope, l);
                            case double d: return new DoubleLiteralExpression(_scope, d);
                            case float f: return new DoubleLiteralExpression(_scope, f);
                        }
                        throw Fail("numeric literal '" + literal.Token.Text + "' is not supported");
                    default:
                        throw Fail(Describe(literal) + " is not supported");
                }
            }

            /// <summary>
            /// Resolves an instance method call <c>recv.Method(args)</c>. The receiver must be a
            /// dotted member path; the method is resolved (with its arguments) through
            /// <c>resolveInvocation</c>. Lambdas, named/ref arguments, generic methods and
            /// receiver-less calls are rejected.
            /// </summary>
            private Expression VisitInvocation(InvocationExpressionSyntax invocation)
            {
                if (_resolveInvocation == null)
                    throw Fail("method invocations are not supported here");

                if (!(invocation.Expression is MemberAccessExpressionSyntax callee
                        && callee.Kind() == SyntaxKind.SimpleMemberAccessExpression
                        && callee.Name is IdentifierNameSyntax methodName))
                    throw Fail(Describe(invocation) + " is not supported");

                var receiverSegments = new List<string>();
                if (!TryFlatten(callee.Expression, receiverSegments))
                    throw Fail("the invocation target '" + callee.Expression + "' is not a simple member path");

                var arguments = new List<Expression>();
                foreach (var argument in invocation.ArgumentList.Arguments)
                {
                    if (argument.NameColon != null || argument.RefKindKeyword.Kind() != SyntaxKind.None)
                        throw Fail("named or ref/out arguments are not supported");
                    arguments.Add(Visit(argument.Expression));
                }

                var resolved = _resolveInvocation(
                    receiverSegments, methodName.Identifier.ValueText, arguments);
                if (resolved == null)
                    throw Fail("'" + invocation + "' cannot be resolved to an instance method on "
                        + "Model, Control or the loop variable");
                return resolved;
            }

            private Expression VisitPath(ExpressionSyntax node)
            {
                var segments = new List<string>();
                if (!TryFlatten(node, segments))
                    throw Fail("'" + node + "' is not a simple member path");

                var resolved = _resolvePath(segments);
                if (resolved == null)
                    throw Fail("'" + node + "' cannot be resolved (supported: Model, Control or the loop "
                        + "variable, an instance property path on any of them, Root.Property, or a static Type.Member)");
                return resolved;
            }

            internal static bool TryFlatten(ExpressionSyntax node, List<string> segments)
            {
                switch (node)
                {
                    case IdentifierNameSyntax identifier:
                        segments.Add(identifier.Identifier.ValueText);
                        return true;
                    case MemberAccessExpressionSyntax member
                        when member.Kind() == SyntaxKind.SimpleMemberAccessExpression
                            && member.Name is IdentifierNameSyntax name:
                        if (!TryFlatten(member.Expression, segments)) return false;
                        segments.Add(name.Identifier.ValueText);
                        return true;
                    default:
                        return false;
                }
            }

            private static string Describe(ExpressionSyntax node)
                => node.Kind() + " '" + node + "'";

            private RazorSubControlDiagnosticException Fail(string reason)
                => Unsupported(_text, _location, reason);
        }
    }
}
