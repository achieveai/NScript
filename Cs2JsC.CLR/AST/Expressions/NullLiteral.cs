//-----------------------------------------------------------------------
// <copyright file="NullLiteral.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for NullLiteral
    /// </summary>
    public class NullLiteral : LiteralExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        public NullLiteral(
            ClrContext context,
            Location location)
            : base(context, location)
        {
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.KnownReferences.Object;
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
            NullLiteral right = obj as NullLiteral;

            return right != null;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(NullLiteral).GetHashCode();
        }
    }
}
