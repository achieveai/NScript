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
    /// Only a small whitelist of syntax is accepted: literals, parentheses, conditionals,
    /// <c>! -</c>, <c>&amp;&amp; || == != &lt; &lt;= &gt; &gt;= + - *</c>, and dotted name paths.
    /// Every name path is handed to <c>resolvePath</c>, which must return a fully resolved
    /// JST expression (scope-system identifiers only). Anything else throws a
    /// <see cref="RazorSubControlDiagnosticException"/> so the build fails with the
    /// expression text instead of emitting a getter that silently reads <c>undefined</c>.
    /// </summary>
    internal static class BindingExpressionConverter
    {
        internal static Expression Convert(
            string csharpExpression,
            IdentifierScope scope,
            Func<IReadOnlyList<string>, Expression> resolvePath,
            Location location)
        {
            if (resolvePath == null) throw new ArgumentNullException(nameof(resolvePath));
            if (string.IsNullOrWhiteSpace(csharpExpression))
                throw Unsupported(csharpExpression, location, "the expression is empty");

            var syntax = SyntaxFactory.ParseExpression(csharpExpression);
            if (syntax.ContainsDiagnostics)
                throw Unsupported(csharpExpression, location, "it is not a valid C# expression");

            return new Converter(csharpExpression, scope, resolvePath, location).Visit(syntax);
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
            private readonly Location _location;

            internal Converter(string text, IdentifierScope scope,
                Func<IReadOnlyList<string>, Expression> resolvePath, Location location)
            {
                _text = text;
                _scope = scope;
                _resolvePath = resolvePath;
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
                        // ConditionalOperatorExpression.Write does not parenthesise a
                        // conditional in condition position, so `(a ? b : c) ? d : e`
                        // would be written as `a ? b : c ? d : e`.
                        if (Unwrap(conditional.Condition) is ConditionalExpressionSyntax)
                            throw Fail("a conditional expression used as the condition of another conditional is not supported");
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

                    case IdentifierNameSyntax _:
                    case MemberAccessExpressionSyntax _:
                        return VisitPath(node);

                    default:
                        throw Fail(Describe(node) + " is not supported");
                }
            }

            private Expression VisitBinary(BinaryExpressionSyntax binary)
            {
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
                    default:
                        throw Fail("operator '" + binary.OperatorToken.Text + "' is not supported");
                }

                // BinaryExpression.Write does not parenthesise a left operand of equal
                // precedence, and `?:` shares precedence with `||`.
                if (op == BinaryOperator.LogicalOr && Unwrap(binary.Left) is ConditionalExpressionSyntax)
                    throw Fail("a conditional expression as the left operand of '||' is not supported");

                return new BinaryExpression(null, _scope, op, Visit(binary.Left), Visit(binary.Right));
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

            private Expression VisitPath(ExpressionSyntax node)
            {
                var segments = new List<string>();
                if (!TryFlatten(node, segments))
                    throw Fail("'" + node + "' is not a simple member path");

                var resolved = _resolvePath(segments);
                if (resolved == null)
                    throw Fail("'" + node + "' cannot be resolved (supported: Model, Control or the loop "
                        + "variable alone, Root.Property, or a static Type.Member)");
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

            private static ExpressionSyntax Unwrap(ExpressionSyntax node)
            {
                while (node is ParenthesizedExpressionSyntax parenthesized)
                    node = parenthesized.Expression;
                return node;
            }

            private static string Describe(ExpressionSyntax node)
                => node.Kind() + " '" + node + "'";

            private RazorSubControlDiagnosticException Fail(string reason)
                => Unsupported(_text, _location, reason);
        }
    }
}
