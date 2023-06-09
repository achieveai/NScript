//-----------------------------------------------------------------------
// <copyright file="SwitchStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for SwitchStatementConverter
    /// </summary>
    public static class SwitchStatementConverter
    {
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            SwitchStatement statement)
        {
            var conversionVariant = GetSwitchConversionVariant(statement);
            var (jsSwitchValue, reusableSwitchValue) = ConvertSwitchValue(
                converter,
                statement.SwitchValue,
                conversionVariant);

            converter.PushScopeBlock(statement);

            var caseBlocks =
                new List<KeyValuePair<List<JST.Expression>, JST.Statement>>(statement.CaseBlocks.Count);

            foreach (var keyValuePair in statement.CaseBlocks)
            {
                List<JST.Expression> cases = new(keyValuePair.Item1.Count);

                for (int literalIndex = 0; literalIndex < keyValuePair.Item1.Count; literalIndex++)
                {
                    if (keyValuePair.Item1[literalIndex] != null)
                    {
                        cases.Add(
                            Convert(
                                converter,
                                keyValuePair.Item1[literalIndex], reusableSwitchValue, conversionVariant));
                    }
                    else
                    {
                        cases.Add(null);
                    }
                }

                caseBlocks.Add(
                    new KeyValuePair<List<JST.Expression>, JST.Statement>(
                        cases,
                        StatementConverterBase.Convert(
                            converter,
                            keyValuePair.Item2)));
            }

            converter.PopScopeBlock();

            return new JST.SwitchBlockStatement(
                statement.Location,
                converter.Scope,
                jsSwitchValue,
                caseBlocks);
        }

        private static JST.Expression Convert(
            IMethodScopeConverter converter,
            CaseLabel label,
            JST.Expression reusableSwitchValue,
            ConversionVariant conversionVariant)
        {
            switch (label)
            {
                case ConstCaseLabel ccl:
                    var constantExpression = ExpressionConverterBase.Convert(converter, ccl.ConstantExpression);
                    return conversionVariant == ConversionVariant.RegularSwitchValue
                        ? constantExpression
                        : new JST.BinaryExpression(
                            null,
                            converter.Scope.ParentScope,
                            JST.BinaryOperator.Equals,
                            reusableSwitchValue,
                            constantExpression);

                case DeclarationCaseLabel dcl:
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

                default:
                    throw new NotImplementedException();
            }
        }

        private enum ConversionVariant
        {
            BooleanSwitchValue,
            RegularSwitchValue
        }

        private static (JST.Expression, JST.Expression) ConvertSwitchValue(
            IMethodScopeConverter converter,
            Expression switchValue,
            ConversionVariant conversionVariant)
        {
            var jsSwitchValue = ExpressionConverterBase.Convert(converter, switchValue);

            if (conversionVariant == ConversionVariant.RegularSwitchValue)
            {
                // JS: switch(12), switch(o), switch(f()), etc.
                return (jsSwitchValue, jsSwitchValue);
            }

            var shouldRequireTempVariable = switchValue switch
            {
                LiteralExpression => false,
                VariableReference => false,
                _ => true
            };

            JST.Expression reusableSwitchValue;
            if (shouldRequireTempVariable)
            {
                var tempVarExpr = new JST.IdentifierExpression(converter.GetTempVariable(), converter.Scope);
                var assignmentExpr = new JST.BinaryExpression(switchValue.Location, converter.Scope, JST.BinaryOperator.Assignment, tempVarExpr, jsSwitchValue);

                // JS: switch(temp0 = switchValue, true)
                jsSwitchValue = new JST.ExpressionsList(
                    switchValue.Location,
                    converter.Scope,
                    new JST.Expression[]
                    {
                        assignmentExpr, new JST.BooleanLiteralExpression(converter.Scope, true)
                    });

                reusableSwitchValue = tempVarExpr;
            }
            else
            {
                reusableSwitchValue = jsSwitchValue;
                // JS: switch(true)
                jsSwitchValue = new JST.BooleanLiteralExpression(converter.Scope, true);
            }

            return (jsSwitchValue, reusableSwitchValue);
        }

        private static ConversionVariant GetSwitchConversionVariant(SwitchStatement statement)
        {
            return statement.CaseBlocks
                .SelectMany(cb => cb.cases)
                .Any(cs => cs is DeclarationCaseLabel)
                    ? ConversionVariant.BooleanSwitchValue
                    : ConversionVariant.RegularSwitchValue;
        }
    }
}