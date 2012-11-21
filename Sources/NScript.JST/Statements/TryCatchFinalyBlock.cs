//-----------------------------------------------------------------------
// <copyright file="TryCatchFinalyBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for TryCatchFinalyBlock
    /// </summary>
    public class TryCatchFinalyBlock : Statement
    {
        /// <summary>
        /// Backing field for Try.
        /// </summary>
        private ScopeBlock tryStatement;

        /// <summary>
        /// Backing field for Catch
        /// </summary>
        private CatchHandler catchHandler;

        /// <summary>
        /// Backing field for Finally.
        /// </summary>
        private ScopeBlock finallyStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TryCatchFinalyBlock"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="tryStatement">The try statement.</param>
        /// <param name="catchHandler">The catch handler.</param>
        /// <param name="finallyStatement">The finally statement.</param>
        public TryCatchFinalyBlock(
            IdentifierScope scope,
            ScopeBlock tryStatement,
            CatchHandler catchHandler,
            ScopeBlock finallyStatement)
            :base (null, scope)
        {
            this.tryStatement = tryStatement;
            this.catchHandler = catchHandler;
            this.finallyStatement = finallyStatement;
        }

        /// <summary>
        /// Gets the try.
        /// </summary>
        public ScopeBlock Try
        {
            get { return this.tryStatement; }
        }

        /// <summary>
        /// Gets the catch.
        /// </summary>
        public CatchHandler Catch
        {
            get { return this.catchHandler; }
        }

        /// <summary>
        /// Gets the finally.
        /// </summary>
        public ScopeBlock Finally
        {
            get { return this.finallyStatement; }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("try", this.Try);
            serializer.AddValue("catch", this.Catch);
            serializer.AddValue("finally", this.Finally);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .Write(Keyword.Try);

            ((ScopeBlock)this.Try).Write(writer, true);

            if (this.Catch != null)
            {
                writer.Write(this.Catch);
            }

            if (this.Finally != null)
            {
                writer.Write(Keyword.Finally);
                this.Finally.Write(
                    writer, true);
            }
        }
    }
}
