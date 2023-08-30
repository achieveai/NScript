using NScript.CLR.AST;
using NScript.Converter.TypeSystemConverter;
using System;

namespace NScript.Converter.ExpressionsConverter
{
    public static class IsPatternConverter
    {
        public static JST.Expression Convert(IMethodScopeConverter converter, IsPatternExpression isPattern)
        {
            var lhs = ExpressionConverterBase.Convert(converter, isPattern.Lhs);

            if (isPattern.Pattern is ConstantPattern constantPattern)
            {
                var constantExpression = ExpressionConverterBase.Convert(
                    converter,
                    constantPattern.ConstantExpression);

                return new JST.BinaryExpression(
                    lhs.Location,
                    converter.Scope,
                    JST.BinaryOperator.Equals,
                    lhs,
                    constantExpression);
            }
            else if (isPattern.Pattern is DeclarationPattern declarationPattern)
            {
                // (x is Type2 y) ---- ((y = Type.AsType(Type2, typeof x)) != null)

                var variableAccess = ExpressionConverterBase.Convert(
                    converter,
                    declarationPattern.VariableAccess);

                var ty = declarationPattern.Type;

                // Generate call to Type.AsType

                var methodReference = converter.KnownReferences.AsTypeMethod;

                var typeRefExpr = JST.IdentifierExpression.Create(
                    null,
                    converter.Scope,
                    converter.Resolve(ty));

                var asTypeCall = MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(typeRefExpr, methodReference, false),
                    new JST.Expression[] { lhs },
                    converter,
                    converter.RuntimeManager);

                var assignment = new JST.BinaryExpression(
                    isPattern.Location,
                    converter.Scope,
                    JST.BinaryOperator.Assignment,
                    variableAccess,
                    asTypeCall);

                return new JST.BinaryExpression(
                    isPattern.Location,
                    converter.Scope,
                    JST.BinaryOperator.NotEquals,
                    assignment,
                    new JST.NullLiteralExpression(converter.Scope));
            }

            throw new NotImplementedException($"IsPattern: {isPattern.Pattern.GetType().Name}");
        }
    }
}
