//-----------------------------------------------------------------------
// <copyright file="StringLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using NScript.Utils;

    /// <summary>
    /// String literal expression.
    /// </summary>
    public class StringLiteralExpression : LiteralExpression
    {
        /// <summary>
        /// Backing store for string literal.
        /// </summary>
        private string stringLiteral;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLiteralExpression"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="stringLiteral">The string literal.</param>
        /// <param name="location">Optional source location for the literal.</param>
        public StringLiteralExpression(
            IdentifierScope scope,
            string stringLiteral,
            Location location = null)
            : base(stringLiteral, scope, location)
        {
            this.stringLiteral = stringLiteral;
        }

        /// <summary>
        /// Gets the string literal.
        /// </summary>
        /// <value>The string literal.</value>
        public string StringLiteral
        {
            get
            {
                return this.stringLiteral;
            }
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteStr(this.LiteralString);
        }
    }
}
