//-----------------------------------------------------------------------
// <copyright file="BoxExpression.cs" company="">
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
    /// Definition for BoxExpression
    /// </summary>
    public class BoxExpression : Expression
    {
        /// <summary>
        /// Backing field for BoxedExpression.
        /// </summary>
        private Expression boxedExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="locationInfo">The location info.</param>
        /// <param name="innerExpression">The inner expression.</param>
        public BoxExpression(
            ClrContext context,
            Location locationInfo,
            Expression innerExpression)
            : base(context, locationInfo)
        {
            this.boxedExpression = innerExpression;
        }

        /// <summary>
        /// Gets the boxed expression.
        /// </summary>
        /// <value>The boxed expression.</value>
        public Expression BoxedExpression
        {
            get { return this.boxedExpression; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.Context.KnownReferences.Object;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.boxedExpression = (Expression) processor.Process(this.boxedExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("box", this.boxedExpression);
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
            BoxExpression right = obj as BoxExpression;

            return right != null
                && this.BoxedExpression.Equals(right.BoxedExpression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(BoxExpression).GetHashCode()
                ^ this.BoxedExpression.GetHashCode();
        }
    }
}
