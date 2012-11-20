//-----------------------------------------------------------------------
// <copyright file="ThisExpressions.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using NScript.Utils;
namespace NScript.JST
{
    /// <summary>
    /// This expression.
    /// </summary>
    public class ThisExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThisExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">the scope</param>
        public ThisExpression(Location location, IdentifierScope scope)
            : base(location, scope)
        {
        }

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <value>The precedence.</value>
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
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.This);
        }
    }
}
