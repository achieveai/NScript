//-----------------------------------------------------------------------
// <copyright file="TupleLiteralConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using JsCsc.Lib.Serialization;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;

    /// <summary>
    /// Definition for TupleLiteralConverter
    /// </summary>
    public static class TupleLiteralConverter
    {
        internal static Expression Convert(
            IMethodScopeConverter scopeConverter,
            TupleLiteral tupleLiteral)
        {
            // Create new Object, where each property/field is 
            // using field identifier of ValueTuple type's respective field
            // to generate the field name. Initialize the value to the expression
            // from tupleLiteral.
            throw new NotImplementedException();
        }
    }
}
