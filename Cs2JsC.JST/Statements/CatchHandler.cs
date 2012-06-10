//-----------------------------------------------------------------------
// <copyright file="CatchHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CatchHandler
    /// </summary>
    public class CatchHandler : Statement
    {
        /// <summary>
        /// Backing field for CatchIdentifier;
        /// </summary>
        IdentifierExpression catchIdentifier;

        /// <summary>
        /// Backing field for CatchBlock.
        /// </summary>
        ScopeBlock catchBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatchHandler"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="catchIdentifier">The catch identifier.</param>
        /// <param name="catchBlock">The catch block.</param>
        public CatchHandler(
            IdentifierScope scope,
            IdentifierExpression catchIdentifier,
            ScopeBlock catchBlock)
            : base(null, scope)
        {
            this.catchIdentifier = catchIdentifier;
            this.catchBlock = catchBlock;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public IdentifierExpression Identifier
        {
            get { return this.catchIdentifier; }
        }

        /// <summary>
        /// Gets the catch block.
        /// </summary>
        public ScopeBlock CatchBlock
        {
            get { return this.catchBlock; }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(Cs2JsC.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("identifier", this.Identifier);
            serializer.AddValue("block", this.CatchBlock);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.Catch);

            if (this.Identifier != null)
            {
                writer.Write(this.Identifier, true);
            }
            else
            {
                writer.Write(Symbols.BracketOpenRound)
                    .Write(Symbols.BracketCloseRound);
            }

            this.CatchBlock.Write(writer, true);
        }
    }
}
