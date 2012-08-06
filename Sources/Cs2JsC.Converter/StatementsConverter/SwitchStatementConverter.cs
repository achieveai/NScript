//-----------------------------------------------------------------------
// <copyright file="SwitchStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.ExpressionsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;

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

            List<KeyValuePair<List<JST.LiteralExpression>, JST.Statement>> caseBlocks =
                new List<KeyValuePair<List<JST.LiteralExpression>, JST.Statement>>(statement.CaseBlocks.Count);

            foreach (var keyValuePair in statement.CaseBlocks)
            {
                List<JST.LiteralExpression> cases = new List<JST.LiteralExpression>(keyValuePair.Key.Count);

                for (int literalIndex = 0; literalIndex < keyValuePair.Key.Count; literalIndex++)
                {
                    if (keyValuePair.Key[literalIndex] != null)
                    {
                        cases.Add(
                            (JST.LiteralExpression)
                            ExpressionConverterBase.Convert(
                                converter,
                                keyValuePair.Key[literalIndex]));
                    }
                    else
                    {
                        cases.Add(null);
                    }
                }

                caseBlocks.Add(
                    new KeyValuePair<List<JST.LiteralExpression>, JST.Statement>(
                        cases,
                        StatementConverterBase.Convert(
                            converter,
                            keyValuePair.Value)));
            }

            return new JST.SwitchBlockStatement(
                statement.Location,
                converter.Scope,
                switchValue,
                caseBlocks);
        }
    }
}
