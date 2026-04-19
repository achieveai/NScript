//-----------------------------------------------------------------------
// <copyright file="NullLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using NScript.Utils;

    /// <summary>
    /// Null literal expression.
    /// </summary>
    public class NullLiteralExpression : LiteralExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullLiteralExpression"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="location">Optional source location for the literal.</param>
        public NullLiteralExpression(IdentifierScope scope, Location location = null)
            : base("null", scope, location)
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