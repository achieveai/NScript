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
    public class ThrowStatement : Statement
    {
        /// <summary>
        /// Backing field for InnerExpression.
        /// </summary>
        private Expression innerExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="innerExpression">The inner expression.</param>
        public ThrowStatement(
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
            writer.WriteNewLine()
                .Write(Keyword.Throw)
                .Write(this.Expression)
                .Write(Symbols.SemiColon);
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
