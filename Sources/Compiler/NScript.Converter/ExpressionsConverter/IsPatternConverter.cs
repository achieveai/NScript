using NScript.CLR.AST;
using NScript.Converter.TypeSystemConverter;

namespace NScript.Converter.ExpressionsConverter
{
    public static class IsPatternConverter
    {
        public static JST.Expression Convert(IMethodScopeConverter converter, IsPatternExpression isPattern)
        {
            var lhs = ExpressionConverterBase.Convert(converter, isPattern.Lhs);
            return PatternMatcher.LowerToCondition(
                converter,
                isPattern.Pattern,
                lhs,
                isPattern.Lhs.ResultType);
        }
    }
}
