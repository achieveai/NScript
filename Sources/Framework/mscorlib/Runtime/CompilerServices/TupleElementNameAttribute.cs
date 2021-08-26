//-----------------------------------------------------------------------
// <copyright file="TupleNameAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for TupleNameAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class TupleElementNamesAttribute : Attribute
    {
        private readonly string[] _transformNames;

        //
        // Summary:
        //     Specifies, in a pre-order depth-first traversal of a type's construction, which
        //     value tuple elements are meant to carry element names.
        //
        // Returns:
        //     An array that indicates which value tuple elements are meant to carry element
        //     names.
        public IList<string> TransformNames => _transformNames;

        //
        // Summary:
        //     Initializes a new instance of the System.Runtime.CompilerServices.TupleElementNamesAttribute
        //     class.
        //
        // Parameters:
        //   transformNames:
        //     A string array that specifies, in a pre-order depth-first traversal of a type's
        //     construction, which value tuple occurrences are meant to carry element names.
        public TupleElementNamesAttribute(string[] transformNames)
        {
            _transformNames = transformNames;
        }
    }
}
