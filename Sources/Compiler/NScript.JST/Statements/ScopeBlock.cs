//-----------------------------------------------------------------------
// <copyright file="ScopeBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;

    /// <summary>
    /// Definition for ScopeBlock
    /// </summary>
    public class ScopeBlock : Statement
    {
        /// <summary>
        /// Backing collection for Statements.
        /// </summary>
        private readonly List<Statement> statements;

        /// <summary>
        /// Backing field for Statements.
        /// </summary>
        private readonly ReadOnlyCollection<Statement> readonlyStatements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeBlock"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="statements">The statements.</param>
        public ScopeBlock(
            Location location,
            IdentifierScope scope,
            List<Statement> statements) 
            : base(location, scope)
        {
            this.statements = statements;
            this.readonlyStatements = new ReadOnlyCollection<Statement>(this.statements);
        }

        /// <summary>
        /// Gets the statements.
        /// </summary>
        /// <value>The statements.</value>
        public IList<Statement> Statements
        {
            get { return this.readonlyStatements; }
        }

        /// <summary>
        /// Adds the statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public void AddStatement(JST.Statement statement)
        {
            if (statement != null)
            {
                this.statements.Add(statement);
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("statements", this.Statements);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            this.Write(writer, false);
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="forceBraces">if set to <c>true</c> [force braces].</param>
        public void Write(JSWriter writer, bool forceBraces)
        {
            if (this.Statements.Count > 1
                || forceBraces)
            {
                writer.Write(Symbols.BracketOpenCurly)
                    .EnterScope();

                foreach (var statement in this.Statements)
                {
                    writer.Write(statement);
                }

                writer.ExitScope()
                    .Write(Symbols.BracketCloseCurly);
            }

            else if (this.statements.Count == 1)
            {
                writer.EnterScope()
                    .Write(this.statements[0])
                    .ExitScope();
            }
            else
            {
                writer.Write(Symbols.SemiColon);
            }
        }
    }
}
