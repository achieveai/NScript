//-----------------------------------------------------------------------
// <copyright file="IfBlockConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for IfBlockConverter
    /// </summary>
    public static class IfBlockConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>IfBlockStatement</returns>
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            IfBlockStatement statement)
        {
            JST.Expression condition = ExpressionsConverter.ExpressionConverterBase.Convert(
                converter,
                statement.Condition);

            List<JST.Statement> jstStatements = new List<JST.Statement>();

            JST.Statement trueStatement = ScopeBlockConverter.Convert(converter, statement.TrueCaseStatements);
            JST.ScopeBlock trueBlock = trueStatement as JST.ScopeBlock;
            if (trueBlock == null)
            {
                if (trueStatement != null)
                {
                    jstStatements.Add(trueStatement);
                }

                trueBlock = new JST.ScopeBlock(
                    trueStatement != null ? trueStatement.Location : null,
                    converter.Scope,
                    jstStatements);
            }

            JST.ScopeBlock falseBlock = null;

            if (statement.FalseCaseStatements != null)
            {
                JST.Statement falseStatement = ScopeBlockConverter.Convert(converter, statement.FalseCaseStatements);
                falseBlock = falseStatement as JST.ScopeBlock;

                if (falseBlock == null)
                {
                    jstStatements = new List<JST.Statement>();
                    jstStatements.Add(falseStatement);
                    falseBlock = new JST.ScopeBlock(
                        falseStatement.Location,
                        converter.Scope,
                        jstStatements);
                }
            }

            return new JST.IfBlockStatement(
                statement.Location,
                condition.Scope,
                condition,
                trueBlock,
                falseBlock);
        }
    }
}
