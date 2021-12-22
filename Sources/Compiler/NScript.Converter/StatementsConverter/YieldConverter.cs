using NScript.CLR;
using NScript.Converter.ExpressionsConverter;
using NScript.Converter.TypeSystemConverter;
using NScript.Utils;

namespace NScript.Converter.StatementsConverter
{
    public static class YieldConverter
    {
        public static JST.Statement Convert(
            IMethodScopeConverter methodScopeConverter,
            CLR.AST.YieldStatement yieldStatement)
        {
            if (yieldStatement.IsBreak)
            {
                return new JST.ReturnStatement(
                    yieldStatement.Location,
                    methodScopeConverter.Scope,
                    null);
            }

            var yieldExpression = new JST.YieldExpression(
                yieldStatement.Location,
                methodScopeConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodScopeConverter,
                    yieldStatement.YieldValue));

            return new JST.ExpressionStatement(
                yieldStatement.Location,
                methodScopeConverter.Scope,
                yieldExpression);
        }

    }
}
