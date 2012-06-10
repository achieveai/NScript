//-----------------------------------------------------------------------
// <copyright file="NullLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    /// <summary>
    /// Null literal expression.
    /// </summary>
    public class NullLiteralExpression : LiteralExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullLiteralExpression"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public NullLiteralExpression(IdentifierScope scope)
            : base("null", scope)
        {
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.Null);
        }
    }
}