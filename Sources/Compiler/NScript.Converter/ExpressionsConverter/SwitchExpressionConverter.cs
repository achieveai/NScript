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

            var exprCondList = expression.CaseLabels
                .Zip(expression.Expressions)
                .Select(tupl =>
                {
                    var (label, expr) = tupl;
                    var jsCond = (label) switch
                    {
                        ConstCaseLabel constCaseLabel => MakeConditionalExpression(
                            constCaseLabel,
                            methodConverter,
                            switchVarExpression),

                        DeclarationCaseLabel dcl => MakeConditionalExpression(
                            dcl,
                            methodConverter,
                            switchVarExpression),

                        DiscardCaseLabel discardCaseLabel => new JST.BooleanLiteralExpression(methodConverter.Scope, true)
                    };

                    var jsExpr = ExpressionConverterBase.Convert(methodConverter, expr);

                    return (jsExpr, jsCond);
                })
                .ToList();

            JST.Expression rv = new JST.NullLiteralExpression(methodConverter.Scope);

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

        public static JST.Expression MakeConditionalExpression(
            ConstCaseLabel discardCaseLabel,
            IMethodScopeConverter methodConverter,
            JST.Expression switchVar)
        {
            return new JST.BinaryExpression(
                discardCaseLabel.Location,
                methodConverter.Scope,
                JST.BinaryOperator.StrictEquals,
                switchVar,
                ExpressionConverterBase.Convert(methodConverter, discardCaseLabel.ConstantExpression));
        }

        public static JST.Expression MakeConditionalExpression(
            DeclarationCaseLabel dcl,
            IMethodScopeConverter converter,
            JST.Expression reusableSwitchValue )
        {
                var variableOpt = dcl.VariableOpt != null
                    ? (JST.IdentifierExpression)ExpressionConverterBase.Convert(converter, dcl.VariableOpt)
                    : null;
                var ty = dcl.TypeReference.Resolve();
                var methodReference = converter.KnownReferences.AsTypeMethod;
                var typeRefExpr = JST.IdentifierExpression.Create(null, converter.Scope,
                    converter.Resolve(ty));
                // JS: Type__AsType(Type, ident)
                var asType = MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(typeRefExpr, methodReference, false),
                    new JST.Expression[] { reusableSwitchValue },
                    converter,
                    converter.RuntimeManager
                );

                var binding = variableOpt != null
                    ? new JST.BinaryExpression(null, converter.Scope, JST.BinaryOperator.Assignment, variableOpt, asType)
                    : asType;

                 // JS: Type__AsType(Type, ident) != null
                var typeCheckExpr = new JST.BinaryExpression(
                    null,
                    converter.Scope,
                    JST.BinaryOperator.NotEquals,
                    binding,
                    new JST.NullLiteralExpression(converter.Scope));

                var whenExprOpt = dcl.WhenExpressionOpt != null
                    ? ExpressionConverterBase.Convert(converter, dcl.WhenExpressionOpt)
                    : null;

                return whenExprOpt != null
                    ? new JST.BinaryExpression(null, converter.Scope, JST.BinaryOperator.LogicalAnd, typeCheckExpr, whenExprOpt)
                    : typeCheckExpr;
        }
    }
}