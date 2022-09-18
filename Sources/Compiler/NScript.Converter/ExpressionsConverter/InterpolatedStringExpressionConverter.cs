namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System.Linq;

    public class InterpolatedStringExpressionConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            InterpolatedStringExpression expression)
        {
            var res = ExpressionConverterBase.Convert(
                converter,
                expression.Parts.First());

            foreach (var part in expression.Parts.Skip(1))
            {
                var rhs = ExpressionConverterBase.Convert(
                    converter,
                    part);

                res = new JST.BinaryExpression(
                    part.Location,
                    converter.Scope,
                    JST.BinaryOperator.Plus,
                    res,
                    rhs);
            }

            return res;
        }
    }
}
