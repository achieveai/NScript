//-----------------------------------------------------------------------
// <copyright file="TypeofExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for TypeofExpression
    /// </summary>
    public class TypeofExpression : Expression
    {
        /// <summary>
        /// Backing field for Expression.
        /// </summary>
        private readonly TypeReferenceExpression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeofExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="expression">The expression.</param>
        public TypeofExpression(
            ClrContext context,
            Location location,
            TypeReferenceExpression expression)
            : base(context, location)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Gets the inner expression.
        /// </summary>
        public TypeReferenceExpression InnerExpression
        {
            get { return this.expression; }
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
                return this.Context.KnownReferences.TypeType;
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
            serializationInfo.AddValue("paramDef", this.InnerExpression.Type.ToString());
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
            TypeofExpression right = obj as TypeofExpression;

            return right != null
                && this.InnerExpression.Equals(right.InnerExpression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(TypeofExpression).GetHashCode() + this.InnerExpression.GetHashCode();
        }
    }
}
