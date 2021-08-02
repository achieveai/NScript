//-----------------------------------------------------------------------
// <copyright file="TupleDeconstructConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using NScript.CLR.AST;
    using NScript.CLR.AST.Statements;
    using NScript.Converter.TypeSystemConverter;
    using System;

    /// <summary>
    /// Definition for TupleDeconstructConverter
    /// </summary>
    public static class TupleDeconstructConverter
    {
        internal static JST.Statement Convert(
            IMethodScopeConverter scopeConverter,
            TupleDeconstructStatement statement)
        {
            // Create assignment operators for each of left
            // expression and match it to corresponding property on tuple
            // E.g. idx: 1, should be matched with Item2 field on ValueTuple
            throw new NotImplementedException();
        }
    }
}
