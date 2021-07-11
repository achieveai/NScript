//-----------------------------------------------------------------------
// <copyright file="ForInLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;


    /// <summary>
    /// Definition for ForInLoop
    /// </summary>
    public class ForInLoop : Statement
    {
        /// <summary>
        /// Backing field for Collection
        /// </summary>
        private readonly Expression collection;

        /// <summary>
        /// Backing field for Key.
        /// </summary>
        private readonly IdentifierExpression key;

        /// <summary>
        /// Backing field for Loop
        /// </summary>
        private readonly Statement loopBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForLoop"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="key">The key.</param>
        /// <param name="loopBlock">The loop block.</param>
        public ForInLoop(
            Location location,
            IdentifierScope scope,
            Expression collection,
            IdentifierExpression key,
            Statement loopBlock)
            : base(location, scope)
        {
            this.collection = collection;
            this.key = key;
            this.loopBlock = loopBlock;
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public Expression Collection
        {
            get { return this.collection; }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public IdentifierExpression Key
        {
            get { return this.key; }
        }

        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <value>The loop.</value>
        public Statement Loop
        {
            get { return this.loopBlock; }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("collection", this.Collection);
            serializer.AddValue("key", this.Key);
            serializer.AddValue("loop", this.Loop);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .Write(Keyword.For)
                .Write(Symbols.BracketOpenRound)
                .Write(this.Key)
                .Write(Keyword.In)
                .Write(this.collection, this.collection.Precedence <= Precedence.Relational)
                .Write(Symbols.BracketCloseRound);

            if (this.loopBlock is ScopeBlock)
            {
                writer.Write(this.loopBlock);
            }
            else
            {
                if (this.loopBlock != null)
                {
                    writer.EnterScope()
                        .Write(this.loopBlock)
                        .ExitScope();
                }
                else
                {
                    writer.Write(Symbols.SemiColon);
                }
            }
        }
    }
}
