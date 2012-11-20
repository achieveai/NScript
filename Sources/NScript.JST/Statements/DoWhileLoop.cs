//-----------------------------------------------------------------------
// <copyright file="DoWhileLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for DoWhileLoop
    /// </summary>
    public class DoWhileLoop : Statement
    {
        /// <summary>
        /// Backing field for condition.
        /// </summary>
        private readonly Expression condition;

        /// <summary>
        /// Backing field for Loop
        /// </summary>
        private readonly Statement loop;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoWhileLoop"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="loop">The loop.</param>
        public DoWhileLoop(
            Location location,
            IdentifierScope scope,
            Expression condition,
            Statement loop)
            : base(location, scope)
        {
            this.condition = condition;
            this.loop = loop;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Expression Condition
        { get { return this.condition; } }

        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <value>The loop.</value>
        public Statement Loop
        { get { return this.loop; } }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("condition", this.Condition);
            serializer.AddValue("loop", this.loop);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .Write(Keyword.Do);

            if (!(this.Loop is ScopeBlock))
            {
                writer.EnterScope()
                    .Write(this.Loop)
                    .ExitScope()
                    .WriteNewLine()
                    .Write(Keyword.While);
            }
            else
            {
                writer.Write(this.Loop)
                    .Write(Keyword.While);
            }

            writer.Write(this.Condition, true)
                .Write(Symbols.SemiColon);
        }
    }
}
