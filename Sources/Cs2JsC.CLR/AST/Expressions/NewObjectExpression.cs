//-----------------------------------------------------------------------
// <copyright file="NewObjectExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for NewObjectExpression
    /// </summary>
    public class NewObjectExpression : MethodCallExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewObjectExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="constructor">The constructor.</param>
        /// <param name="constructorArguments">The constructor arguments.</param>
        public NewObjectExpression(
            ClrContext context,
            Location location,
            MethodReference constructor,
            params Expression[] constructorArguments)
            : base(
                context,
                location,
                new ConstructorReferenceExpression(context, location, constructor),
                constructorArguments)
        {
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
            NewObjectExpression right = obj as NewObjectExpression;

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
            int rv = typeof(NewObjectExpression).GetHashCode()
                ^ this.MethodReference.GetHashCode();

            foreach (var item in this.Parameters)
            {
                rv ^= item.GetHashCode();
            }

            return rv;
        }
    }
}
