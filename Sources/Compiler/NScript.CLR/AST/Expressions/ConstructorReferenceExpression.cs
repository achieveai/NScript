//-----------------------------------------------------------------------
// <copyright file="ConstructorReferenceExpression.cs" company="">
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
    /// Definition for ConstructorReferenceExpression
    /// </summary>
    public class ConstructorReferenceExpression : Expression
    {
        /// <summary>
        /// Backing field for Constructor.
        /// </summary>
        private readonly MethodReference methodReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="methodReference">The method reference.</param>
        public ConstructorReferenceExpression(
            ClrContext context,
            Location location,
            MethodReference methodReference)
            : base(context, location)
        {
            this.methodReference = methodReference;

            if (this.methodReference.Name != ".ctor")
            {
                throw new ArgumentException("Method is not a constructor");
            }
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <value>The constructor.</value>
        public MethodReference Constructor
        {
            get
            {
                return this.methodReference;
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("methodReference", this.methodReference.ToString());
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
            ConstructorReferenceExpression right = obj as ConstructorReferenceExpression;

            return right != null
                && this.Constructor.Equals(right.Constructor);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(ConstructorReferenceExpression).GetHashCode()
                ^ this.Constructor.GetHashCode();
        }
    }
}
