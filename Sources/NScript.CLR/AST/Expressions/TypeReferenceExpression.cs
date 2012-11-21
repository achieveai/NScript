//-----------------------------------------------------------------------
// <copyright file="TypeReferenceExpression.cs" company="">
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
    /// Definition for TypeReferenceExpression
    /// </summary>
    public class TypeReferenceExpression : Expression
    {
        /// <summary>
        /// Backing field for Type.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="paramDef">The type reference.</param>
        public TypeReferenceExpression(
            ClrContext context,
            Location location,
            TypeReference typeReference)
            : base(context, location)
        {
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public TypeReference Type
        {
            get { return this.typeReference; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public override TypeReference ResultType
        {
            get
            {
                return this.KnownReferences.RuntimeTypeHandle;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("paramDef", this.Type.ToString());
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
            TypeReferenceExpression right = obj as TypeReferenceExpression;

            return right != null
                && this.Type.Equals(right.Type);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(TypeReferenceExpression).GetHashCode() + this.Type.GetHashCode();
        }
    }
}
