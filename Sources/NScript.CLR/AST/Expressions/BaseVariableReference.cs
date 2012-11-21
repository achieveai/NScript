//-----------------------------------------------------------------------
// <copyright file="BaseVariableReference.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using NScript.Utils;

namespace NScript.CLR.AST
{
    /// <summary>
    /// Definition for BaseVariableReference
    /// </summary>
    public class BaseVariableReference : VariableReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReference"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="variable">The variable.</param>
        public BaseVariableReference(
            ClrContext context,
            Location location,
            ThisVariable variable)
            : base(context, location, variable)
        {
        }
    }
}