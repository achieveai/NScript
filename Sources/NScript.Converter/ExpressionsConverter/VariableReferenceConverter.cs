//-----------------------------------------------------------------------
// <copyright file="VariableReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using NScript.Converter.StatementsConverter;

    /// <summary>
    /// Definition for VariableReferenceConverter
    /// </summary>
    public static class VariableReferenceConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            VariableReference expression)
        {
            if (expression.Variable is LocalVariable)
            {
                LocalVariable localVariable = (LocalVariable)expression.Variable;

                return new JST.IdentifierExpression(
                    converter.ResolveLocal(localVariable.Name),
                    converter.Scope);
            }
            else if (expression.Variable is ThisVariable)
            {
                return converter.ResolveThis(converter.Scope);
            }
            else if (expression.Variable is ParameterVariable)
            {
                ParameterVariable localVariable = (ParameterVariable)expression.Variable;

                return new JST.IdentifierExpression(
                    converter.ResolveArgument(localVariable.Name),
                    converter.Scope);
            }

            throw new NotImplementedException();
        }
    }
}
