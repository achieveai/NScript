//-----------------------------------------------------------------------
// <copyright file="DoubleLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    /// <summary>
    /// Double literal expression.
    /// </summary>
    public class DoubleLiteralExpression : LiteralExpression
    {
        /// <summary>
        /// Backing field for number literal.
        /// </summary>
        private double doubleLiteral;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleLiteralExpression"/> class.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="scope">The scope.</param>
        public DoubleLiteralExpression(
            IdentifierScope scope,
            double number)
            : base(number.ToString(), scope)
        {
            this.doubleLiteral = number;
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <value>The number.</value>
        public double Double
        {
            get
            {
                return this.doubleLiteral;
            }
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            if (this.Double < 0)
            {
                writer.Write(Symbols.UnaryMinus)
                    .Write(-this.Double);
            }
            else
            {
                writer.Write(this.Double);
            }
        }
    }
}