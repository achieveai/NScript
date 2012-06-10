//-----------------------------------------------------------------------
// <copyright file="ArrayElementExpression.cs" company="">
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
    /// Definition for ArrayElementLoadExpression
    /// </summary>
    public class ArrayElementExpression : Expression
    {
        /// <summary>
        /// Backing field for ArrayExpression.
        /// </summary>
        private Expression arrayExpression;

        /// <summary>
        /// Backing field for IndexExpression.
        /// </summary>
        private Expression indexExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayElementExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="arrayExpression">The array expression.</param>
        /// <param name="indexExpression">The index expression.</param>
        public ArrayElementExpression(
            ClrContext context,
            Location location,
            Expression arrayExpression,
            Expression indexExpression)
            : base(context, location)
        {
            this.arrayExpression = arrayExpression;
            this.indexExpression = indexExpression;
        }

        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <value>The array.</value>
        public Expression Array
        {
            get
            {
                return this.arrayExpression;
            }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public Expression Index
        {
            get
            {
                return this.indexExpression;
            }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return ((ArrayType)this.arrayExpression.ResultType).ElementType;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.arrayExpression = (Expression)processor.Process(this.arrayExpression);
            this.indexExpression = (Expression)processor.Process(this.indexExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("arrayExpression", arrayExpression);
            serializationInfo.AddValue("indexExpression", indexExpression);
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
            ArrayElementExpression right = obj as ArrayElementExpression;
            return right != null
                && this.Array.Equals(right.Array)
                && this.Index.Equals(right.Index);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(ArrayElementExpression).GetHashCode()
                ^ this.Index.GetHashCode();
        }
    }
}
