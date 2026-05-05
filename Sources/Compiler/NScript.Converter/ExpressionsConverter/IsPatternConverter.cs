using NScript.CLR.AST;
using NScript.Converter.TypeSystemConverter;
using System.Collections.Generic;

namespace NScript.Converter.ExpressionsConverter
{
    public static class IsPatternConverter
    {
        public static JST.Expression Convert(IMethodScopeConverter converter, IsPatternExpression isPattern)
        {
            var lhs = ExpressionConverterBase.Convert(converter, isPattern.Lhs);

            // Binary (and/or) patterns reference the scrutinee in both branches.
            // Reusing a JST node with side effects would emit it twice and re-evaluate.
            // Mirror SwitchExpressionConverter: hoist non-trivial scrutinees to a temp.
            var lhsNeedsHoisting =
                ContainsBinaryPattern(isPattern.Pattern)
                && !(isPattern.Lhs is VariableReference || isPattern.Lhs is LiteralExpression);

            if (!lhsNeedsHoisting)
            {
                return PatternMatcher.LowerToCondition(
                    converter,
                    isPattern.Pattern,
                    lhs,
                    isPattern.Lhs.ResultType);
            }

            var tmpIdent = JST.IdentifierExpression.Create(
                null,
                converter.Scope,
                new List<JST.IIdentifier> { converter.GetTempVariable() });
            var assign = new JST.BinaryExpression(
                null,
                converter.Scope,
                JST.BinaryOperator.Assignment,
                tmpIdent,
                lhs);
            var condition = PatternMatcher.LowerToCondition(
                converter,
                isPattern.Pattern,
                tmpIdent,
                isPattern.Lhs.ResultType);

            return new JST.ExpressionsList(null, converter.Scope, assign, condition);
        }

        private static bool ContainsBinaryPattern(Pattern pattern) => pattern switch
        {
            BinaryPattern _ => true,
            NegatedPattern n => ContainsBinaryPattern(n.Inner),
            _ => false,
        };
    }
}
