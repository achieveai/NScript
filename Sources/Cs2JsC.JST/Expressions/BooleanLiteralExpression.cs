//-----------------------------------------------------------------------
// <copyright file="BooleanLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
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
        public BooleanLiteralExpression(
            IdentifierScope scope,
            bool value)
            : base(value ? "true" : "false", scope)
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
