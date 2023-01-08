//-----------------------------------------------------------------------
// <copyright file="ScriptLiteralExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NScript.Utils;

    /// <summary>
    /// Definition for ScriptLiteralExpression
    /// </summary>
    public class ScriptLiteralExpression : Expression
    {
        /// <summary>
        /// Backing field for Literal.
        /// </summary>
        readonly string literal;

        /// <summary>
        /// Backing field for LiteralArguments.
        /// </summary>
        IList<Expression> literalArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptLiteralExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="literal">The literal.</param>
        /// <param name="literalArguments">The literal arguments.</param>
        public ScriptLiteralExpression(
            Location location,
            IdentifierScope scope,
            string literal,
            params Expression[] literalArguments)
            : base(location, scope)
        {
            this.literal = literal;
            this.literalArguments = literalArguments;
            LiteralArguments = new ReadOnlyCollection<Expression>(literalArguments);
        }

        public string Literal => literal;

        public ReadOnlyCollection<Expression> LiteralArguments { get; }

        public override Precedence Precedence
        {
            get
            {
                return Precedence.Primary;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("literal", this.literal);
            serializer.AddValue("arguments", this.literalArguments);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteIdentifier(this.literal);
        }
    }
}
