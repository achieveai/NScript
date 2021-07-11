//-----------------------------------------------------------------------
// <copyright file="UnboxExpression.cs" company="">
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
    /// Definition for UnboxExpression
    /// </summary>
    public class UnboxExpression : Expression
    {
        /// <summary>
        /// Backing field for innerExpression.
        /// </summary>
        private Expression innerExpression;

        /// <summary>
        /// Backing field for returnType.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnboxExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="innerExpression">The inner expression.</param>
        /// <param name="paramDef">The type reference.</param>
        public UnboxExpression(
            ClrContext context,
            Location location,
            Expression innerExpression,
            TypeReference typeReference)
            : base(context, location)
        {
            this.innerExpression = innerExpression;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Gets the inner expression.
        /// </summary>
        public Expression InnerExpression
        {
            get { return this.innerExpression; }
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
                return this.typeReference;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.innerExpression = (Expression)processor.Process(this.innerExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("unboxType", this.typeReference.ToString());
            serializationInfo.AddValue("innerExpression", this.innerExpression);
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
            UnboxExpression right = obj as UnboxExpression;

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
            return typeof(UnboxExpression).GetHashCode() + this.InnerExpression.GetHashCode();
        }
    }
}
