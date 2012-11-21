//-----------------------------------------------------------------------
// <copyright file="NumberLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    /// <summary>
    /// Number literal expression.
    /// </summary>
    public class NumberLiteralExpression : LiteralExpression
    {
        /// <summary>
        /// Backing field for number literal.
        /// </summary>
        private readonly long numberLiteral;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberLiteralExpression"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="number">The number.</param>
        public NumberLiteralExpression(
            IdentifierScope scope,
            long number)
            : base(number.ToString(), scope)
        {
            this.numberLiteral = number;
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <value>The number.</value>
        public long Number
        {
            get
            {
                return this.numberLiteral;
            }
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            if (this.Number < 0)
            {
                writer.Write(Symbols.UnaryMinus)
                    .Write(-this.Number);
            }
            else
            {
                writer.Write(this.Number);
            }
        }
    }
}