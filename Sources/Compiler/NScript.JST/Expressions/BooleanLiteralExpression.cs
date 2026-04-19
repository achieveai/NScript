//-----------------------------------------------------------------------
// <copyright file="BooleanLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using NScript.Utils;

    /// <summary>
    /// Boolean literal expression.
    /// </summary>
    public class BooleanLiteralExpression : LiteralExpression
    {
        /// <summary>
        /// Backing field for number literal.
        /// </summary>
        private bool booleanLiteral;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanLiteralExpression"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="value">The boolean.</param>
        /// <param name="location">Optional source location for the literal.</param>
        public BooleanLiteralExpression(
            IdentifierScope scope,
            bool value,
            Location location = null)
            : base(value ? "true" : "false", scope, location)
        {
            this.booleanLiteral = value;
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <value>The number.</value>
        public bool Value
        {
            get
            {
                return this.booleanLiteral;
            }
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(this.Value ? Keyword.True : Keyword.False);
        }
    }
}
