//-----------------------------------------------------------------------
// <copyright file="LocalFunctionReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using NScript.Converter.TypeSystemConverter;

namespace NScript.Converter.ExpressionsConverter
{
    /// <summary>
    /// Definition for LocalFunctionReferenceConverter
    /// </summary>
    public static class LocalFunctionReferenceConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            CLR.AST.LocalFunctionReference localFunction)
            => new JST.IdentifierExpression(
                converter.ResolveLocalFunction(
                    localFunction.Variable.Name),
                converter.Scope);
    }
}