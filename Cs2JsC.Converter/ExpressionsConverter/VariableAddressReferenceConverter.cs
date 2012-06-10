//-----------------------------------------------------------------------
// <copyright file="VariableAddressReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for VariableAddressReferenceConverter
    /// </summary>
    public static class VariableAddressReferenceConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression to read address variable.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            VariableAddressReference expression)
        {
            if (expression.Variable is ThisVariable)
            {
                if (!expression.Variable.Type.Resolve().IsValueType)
                {
                    throw new System.InvalidOperationException("Don't know how to converter '&this'");
                }

                return ExpressionConverterBase.Convert(converter, expression.NestedExpression);
            }

            return new JST.MethodCallExpression(
                expression.Location,
                converter.Scope,
                VariableAddressReferenceConverter.Convert(
                    converter,
                    expression,
                    true),
                new List<JST.Expression>());
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="isReader">if set to <c>true</c> is reader.</param>
        /// <returns>Expression to read address variable.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            VariableAddressReference expression,
            bool isReader)
        {
            // Since only way to access addrss variable is through argument variable,
            // we are assuming this fact with resolving the argument.
            return new JST.IndexExpression(
                expression.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(converter, expression.NestedExpression),
                new JST.IdentifierExpression(
                    isReader
                        ? converter.RuntimeManager.ReferenceManager.ReaderIdentifier
                        : converter.RuntimeManager.ReferenceManager.WriterIdentifier));
        }
    }
}