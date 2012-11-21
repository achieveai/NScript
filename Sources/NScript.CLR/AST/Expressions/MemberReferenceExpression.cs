//-----------------------------------------------------------------------
// <copyright file="MemberReferenceExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for MemberReferenceExpression
    /// </summary>
    public class MemberReferenceExpression : Expression
    {
        /// <summary>
        /// Backing field for memberReference;
        /// </summary>
        private readonly MemberReference memberReference;

        /// <summary>
        /// Backing field for LeftExpression.
        /// </summary>
        private Expression leftExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="memberReference">The member reference.</param>
        /// <param name="leftExpression">The left expression.</param>
        public MemberReferenceExpression(
            ClrContext context,
            Location location,
            MemberReference memberReference,
            Expression leftExpression)
            : base(context, location)
        {
            this.memberReference = memberReference;
            this.leftExpression = leftExpression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="memberReference">The member reference.</param>
        public MemberReferenceExpression(
            ClrContext context,
            Location location,
            MemberReference memberReference)
            : base(context, location)
        {
            this.memberReference = memberReference;
        }

        /// <summary>
        /// Gets the member reference.
        /// </summary>
        /// <value>The member reference.</value>
        public MemberReference MemberReference
        {
            get
            {
                return this.memberReference;
            }
        }

        /// <summary>
        /// Gets the left expression.
        /// </summary>
        /// <value>The left expression.</value>
        public Expression LeftExpression
        {
            get
            {
                return this.leftExpression;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            if (this.leftExpression != null)
            {
                this.leftExpression = (Expression)processor.Process(this.leftExpression);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("leftExpression", this.LeftExpression);
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
            MemberReferenceExpression right = obj as MemberReferenceExpression;

            return right != null
                && object.Equals(this.LeftExpression, right.LeftExpression)
                && this.MemberReference.Equals(right.MemberReference);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(MemberReferenceExpression).GetHashCode()
                ^ this.LeftExpression.GetHashCode()
                ^ this.MemberReference.GetHashCode();
        }
    }
}
