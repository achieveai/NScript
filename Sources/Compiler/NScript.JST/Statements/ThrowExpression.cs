//-----------------------------------------------------------------------
// <copyright file="ThrowStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ThrowExpression
    /// </summary>
    public class ThrowExpression : Expression
    {
        /// <summary>
        /// Backing field for InnerExpression.
        /// </summary>
        private Expression innerExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="innerExpression">The inner expression.</param>
        public ThrowExpression(
            Location location,
            IdentifierScope scope,
            Expression innerExpression)
            : base(location, scope)
        {
            this.innerExpression = innerExpression;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression
        {
            get { return this.innerExpression; }
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer
                .EnterLocation(this.Location)
                .Write(Keyword.Throw)
                .Write(this.Expression)
                .LeaveLocation();
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("innerExpression", this.Expression);
        }
    }
}
