//-----------------------------------------------------------------------
// <copyright file="VirtualMethodReferenceExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Definition for VirtualMethodReferenceExpression
    /// </summary>
    public class VirtualMethodReferenceExpression : MethodReferenceExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMethodReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="leftExpression">The left expression.</param>
        public VirtualMethodReferenceExpression(
            ClrContext context,
            Location location,
            MethodReference methodReference,
            Expression leftExpression)
            : base(context, location, methodReference, leftExpression)
        {
            if (!methodReference.Resolve().IsVirtual)
            {
                throw new ArgumentException("MethodReference should be for virtual method");
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            VirtualMethodReferenceExpression right = obj as VirtualMethodReferenceExpression;

            return right != null
                && base.Equals(right);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() + typeof(VirtualMethodReferenceExpression).GetHashCode();
        }
    }
}
