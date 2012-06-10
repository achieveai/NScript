//-----------------------------------------------------------------------
// <copyright file="NewArrayExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Utils;


    /// <summary>
    /// Definition for NewArrayExpression
    /// </summary>
    public class NewArrayExpression : Expression
    {
        /// <summary>
        /// Backing field for Size.
        /// </summary>
        private readonly Expression arraySizeExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewArrayExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="sizeExpression">The size expression.</param>
        public NewArrayExpression(
            Location location,
            IdentifierScope scope,
            Expression sizeExpression)
            : base(location, scope)
        {
            this.arraySizeExpression = sizeExpression;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Expression Size
        {
            get
            {
                return this.arraySizeExpression;
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
            serializer.AddValue("size", this.Size);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            if (this.Size != null
                && (!(this.Size is NumberLiteralExpression)
                    || ((NumberLiteralExpression)this.Size).Number != 0))
            {
                writer.Write(Keyword.New)
                    .WriteIdentifier("Array")
                    .Write(Symbols.BracketOpenRound)
                    .Write(this.Size)
                    .Write(Symbols.BracketCloseRound);
            }
            else
            {
                writer.Write(Symbols.BrackedOpenSquare)
                    .Write(Symbols.BracketCloseSquare);
            }
        }
    }
}
