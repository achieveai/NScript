//-----------------------------------------------------------------------
// <copyright file="ToNullable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
using NScript.Utils;
using Mono.Cecil;

    /// <summary>
    /// Definition for ToNullable
    /// </summary>
    public class ToNullable : Expression
    {
        /// <summary>
        /// Backing field for InnerExpression.
        /// </summary>
        private Expression innerExpression;

        private GenericInstanceType typeReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="locationInfo">The location info.</param>
        /// <param name="innerExpression">The inner expression.</param>
        public ToNullable(
            ClrContext context,
            Location locationInfo,
            Expression innerExpression)
            : base(context, locationInfo)
        {
            this.innerExpression = innerExpression;
            this.typeReference = new GenericInstanceType(
                context.KnownReferences.NullableType);
            this.typeReference.GenericArguments.Add(
                innerExpression.ResultType);
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
        /// <value>The type of the result.</value>
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
            this.innerExpression = (Expression) processor.Process(this.innerExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("inner", this.innerExpression);
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
            ToNullable right = obj as ToNullable;

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
            return typeof(ToNullable).GetHashCode()
                ^ this.innerExpression.GetHashCode();
        }
    }

    /// <summary>
    /// Definition for FromNullable
    /// </summary>
    public class FromNullable : Expression
    {
        /// <summary>
        /// Backing field for InnerExpression.
        /// </summary>
        private Expression innerExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="locationInfo">The location info.</param>
        /// <param name="innerExpression">The inner expression.</param>
        public FromNullable(
            ClrContext context,
            Location locationInfo,
            Expression innerExpression)
            : base(context, locationInfo)
        {
            this.innerExpression = innerExpression;
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
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return ((GenericInstanceType)this.innerExpression.ResultType).GenericArguments[0];
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.innerExpression = (Expression) processor.Process(this.innerExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("inner", this.innerExpression);
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
            ToNullable right = obj as ToNullable;

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
            return typeof(ToNullable).GetHashCode()
                ^ this.innerExpression.GetHashCode();
        }
    }
}
