using NScript.CLR.AST;
using NScript.Converter.TypeSystemConverter;
using System.Collections.Generic;
using System.Linq;

namespace NScript.Converter.ExpressionsConverter
{
    public static class SwitchExpressionConverter
    {
        public static JST.Expression Convert(IMethodScopeConverter methodConverter, SwitchExpression expression)
        {
            // 1. Convert switch value. If evaluation of switchValue leads to side effects,
            //    assign it to a variable after conversion for reuse
            // 2. Convert each switch arm

            var switchValue = ExpressionConverterBase.Convert(
                methodConverter,
                expression.SwitchValue);

            var needsSwitchValueAssignment = !(
                expression.SwitchValue is VariableReference
                || expression.SwitchValue is LiteralExpression);

            var switchVarExpression = needsSwitchValueAssignment
                ? JST.IdentifierExpression.Create(
                    null,
                    methodConverter.Scope,
                    new List<JST.IIdentifier>
                    { methodConverter.GetTempVariable() })
                : switchValue;

            var switchVarInitialization = needsSwitchValueAssignment
                ? new JST.BinaryExpression(
                    null,
                    methodConverter.Scope,
                    JST.BinaryOperator.Assignment,
                    switchVarExpression,
                    switchValue)
                : null;

            // TODO: If there are too many cases, we should generate a jump table instead.
            var exprCondList = expression.Patterns
                .Zip(expression.Expressions)
                .Select(tupl =>
                {
                    var (label, expr) = tupl;
                    // All pattern shapes share lowering through PatternMatcher (ADR 0026).
                    // The arm's `when` clause is attached to DeclarationPattern.WhenExpressionOpt
                    // by the BondToAst serializer and threaded in automatically.
                    var jsCond = PatternMatcher.LowerToCondition(
                        methodConverter,
                        label,
                        switchVarExpression,
                        expression.SwitchValue.ResultType);

                    var jsExpr = ExpressionConverterBase.Convert(methodConverter, expr);

                    return (jsExpr, jsCond);
                })
                .ToList();

            JST.Expression rv = new JST.NullLiteralExpression(methodConverter.Scope);

            // Reverse this to process the last switch arm first.
            // Ultimately, the last arm goes to the end of the conditional expression.
            exprCondList.Reverse();

            exprCondList.ForEach(
                exprCond => rv = new JST.ConditionalOperatorExpression(
                    null,
                    methodConverter.Scope,
                    exprCond.jsCond,
                    exprCond.jsExpr,
                    rv));

            return switchVarInitialization == null
                ? new JST.ExpressionsList(null, methodConverter.Scope, rv)
                : new JST.ExpressionsList(null, methodConverter.Scope, switchVarInitialization, rv);
        }

    }
}