//-----------------------------------------------------------------------
// <copyright file="LiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    /// <summary>
    /// Literal expression is used for all literals.
    /// </summary>
    public abstract class LiteralExpression : Expression
    {
        /// <summary>
        /// Backing field for literalString.
        /// </summary>
        private readonly string literalString;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralExpression"/> class.
        /// </summary>
        /// <param name="literalString">The literal string.</param>
        /// <param name="scope">The scope.</param>
        public LiteralExpression(
            string literalString,
            IdentifierScope scope)
            : base(null, scope)
        {
            this.literalString = literalString;
        }

        /// <summary>
        /// Gets the literal string.
        /// </summary>
        /// <value>The literal string.</value>
        public string LiteralString
        {
            get
            {
                return this.literalString;
            }
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
        public override void Serialize(Cs2JsC.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("value", this.LiteralString);
        }
    }
}
