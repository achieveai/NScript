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

    /// <summary>
    /// Definition for SwitchStatementConverter
    /// </summary>
    public static class SwitchStatementConverter
    {
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            SwitchStatement statement)
        {
            JST.Expression switchValue = ExpressionConverterBase.Convert(
                converter,
                statement.SwitchValue);
            converter.PushScopeBlock(statement);

            JST.Expression switchValueOrTempVar = switchValue;

            var tempVar = new JST.IdentifierExpression(converter.GetTempVariable(), converter.Scope);

            switchValue = new JST.ExpressionsList(switchValue.Location, converter.Scope, new JST.Expression[] {
                    new JST.BinaryExpression(switchValue.Location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        tempVar,
                        switchValue),
                    new JST.BooleanLiteralExpression(null, true)
                });
            switchValueOrTempVar = tempVar;

            var conversionVariant = GetSwitchConversionVariant(statement);

            var caseBlocks =
                new List<KeyValuePair<List<JST.Expression>, JST.Statement>>(statement.CaseBlocks.Count);

            foreach (var keyValuePair in statement.CaseBlocks)
            {
                List<JST.Expression> cases = new List<JST.Expression>(keyValuePair.Key.Count);

                for (int literalIndex = 0; literalIndex < keyValuePair.Key.Count; literalIndex++)
                {
                    if (keyValuePair.Key[literalIndex] != null)
                    {
                        cases.Add(
                            Convert(
                                converter,
                                keyValuePair.Key[literalIndex], switchValueOrTempVar, conversionVariant));
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
                            keyValuePair.Value)));
            }

            converter.PopScopeBlock();

            return new JST.SwitchBlockStatement(
                statement.Location,
                converter.Scope,
                switchValue,
                caseBlocks);
        }

        private static JST.Expression Convert(
            IMethodScopeConverter converter,
            CaseLabel label,
            JST.Expression switchValueOrTempVar,
            ConversionVariant conversionVariant)
        {
            switch (label)
            {
                case ConstCaseLabel ccl:
                    var constantExpression = ExpressionConverterBase.Convert(converter, ccl.ConstantExpression);
                    return conversionVariant == ConversionVariant.RegularSwitchValue
                        ?  constantExpression
                        : new JST.BinaryExpression(
                            null,
                            converter.Scope.ParentScope,
                            JST.BinaryOperator.Equals,
                            switchValueOrTempVar,
                            constantExpression);

                case DeclarationCaseLabel dcl:
                    var variableOpt = dcl.VariableOpt != null
                        ? (JST.IdentifierExpression)ExpressionConverterBase.Convert(converter, dcl.VariableOpt)
                        : null;
                    var ty = dcl.TypeReference.Resolve();
                    var methodReference = converter.KnownReferences.AsTypeMethod;
                    var typeRefExpr = JST.IdentifierExpression.Create(null, converter.Scope,
                        converter.Resolve(ty));
                    var asType = MethodCallExpressionConverter.CreateMethodCallExpression(
                        new MethodCallContext(typeRefExpr, methodReference, false),
                        new JST.Expression[] { switchValueOrTempVar },
                        converter,
                        converter.RuntimeManager
                    );

                    var whenExprOpt = dcl.WhenExpressionOpt != null
                        ? ExpressionConverterBase.Convert(converter, dcl.WhenExpressionOpt)
                        : null;

                    var binding = variableOpt != null
                        ? new JST.BinaryExpression(null, converter.Scope, JST.BinaryOperator.Assignment, variableOpt, asType)
                        : asType;

                    var typeCheckExpr = new JST.BinaryExpression(
                        null,
                        converter.Scope,
                        JST.BinaryOperator.NotEquals,
                        binding,
                        new JST.NullLiteralExpression(converter.Scope));

                    return whenExprOpt != null
                        ? new JST.BinaryExpression(null, converter.Scope, JST.BinaryOperator.LogicalAnd, typeCheckExpr, whenExprOpt)
                        : typeCheckExpr;

                default:
                    throw new NotImplementedException();
            }
        }

        private static bool ShouldRequireTempVariable(Expression switchValue)
        {
            switch (switchValue)
            {
                case LiteralExpression:
                case VariableReference:
                    return false;

                default:
                    return true;
            }
        }

        private enum ConversionVariant
        {
            BooleanSwitchValue,
            RegularSwitchValue
        }

        private static ConversionVariant GetSwitchConversionVariant(SwitchStatement statement)
        {
            foreach (var block in statement.CaseBlocks)
            {
                var cases = block.Key;
                foreach (var @case in cases)
                {
                    var dcl = @case as DeclarationCaseLabel;
                    if (dcl?.VariableOpt != null)
                    {
                        return ConversionVariant.BooleanSwitchValue;
                    }
                }
            }

            return ConversionVariant.RegularSwitchValue;
        }
    }
}