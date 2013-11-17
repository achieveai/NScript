//-----------------------------------------------------------------------
// <copyright file="FunctionExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using NScript.Utils;

    /// <summary>
    /// Function expression.
    /// </summary>
    public class FunctionExpression : Expression
    {
        /// <summary>
        /// Backing field for Scope.
        /// </summary>
        private readonly IdentifierScope innerScope;

        /// <summary>
        /// Backing field for parameters.
        /// </summary>
        private List<IIdentifier> parameters;

        /// <summary>
        /// Backing field for Statements.
        /// </summary>
        private List<Statement> statements;

        /// <summary>
        /// Backing field for identifier.
        /// </summary>
        private readonly ReadOnlyCollection<IIdentifier> readonlyIdentifiers;

        /// <summary>
        /// Backing field for statements.
        /// </summary>
        private readonly ReadOnlyCollection<Statement> readonlyStatements;

        /// <summary>
        /// Name of the function.
        /// </summary>
        private readonly IIdentifier name;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="outerScope">The outer scope.</param>
        /// <param name="innerScope">The inner scope.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="name">The name.</param>
        public FunctionExpression(
            Location location,
            IdentifierScope outerScope,
            IdentifierScope innerScope,
            IEnumerable<IIdentifier> parameters,
            IIdentifier name)
            : base(location, outerScope)
        {
            this.innerScope = innerScope;
            this.name = name;
            if (name != null)
            {
                name.MarkAsFunctionName();
            }

            this.parameters = new List<IIdentifier>(parameters);
            this.statements = new List<Statement>();
            this.readonlyStatements = new ReadOnlyCollection<Statement>(this.statements);
            this.readonlyIdentifiers = new ReadOnlyCollection<IIdentifier>(this.parameters);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public IIdentifier Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IList<IIdentifier> Parameters
        {
            get
            {
                return this.readonlyIdentifiers;
            }
        }

        /// <summary>
        /// Gets the statements.
        /// </summary>
        /// <value>The statements.</value>
        public IList<Statement> Statements
        {
            get
            {
                return this.readonlyStatements;
            }
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>
        /// The precendence.
        /// </value>
        public override Precedence Precedence
        {
            get
            {
                return Precedence.Assignment;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is left to right.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is left to right; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeftToRight
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public void AddStatement(Statement statement)
        {
            if (statement == null)
            {
                throw new System.ArgumentNullException("statement");
            }

            this.statements.Add(statement);
        }

        /// <summary>
        /// Adds the statements.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public void AddStatements(IList<Statement> statements)
        {
            this.statements.AddRange(statements);
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("name", this.name.SuggestedName);
            serializer.AddValue("parameters", this.Parameters.Select((param) => param.SuggestedName));
            serializer.AddValue("statements", this.Statements);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.Function);

            if (this.name != null)
            {
                writer.Write(this.name);
            }

            writer.Write(Symbols.BracketOpenRound);
            for (int argumentIndex = 0; argumentIndex < this.parameters.Count; argumentIndex++)
            {
                if (argumentIndex > 0)
                {
                    writer.Write(Symbols.Comma);
                }

                writer.Write(this.parameters[argumentIndex]);
            }

            writer.Write(Symbols.BracketCloseRound)
                .Write(Symbols.BrackedOpenCurly)
                .EnterScope();

            if (this.innerScope.UsedLocalIdentifiers.Count > 0)
            {
                int realIdentifiers = 0;
                for (int identifierIndex = 0; identifierIndex < this.innerScope.UsedLocalIdentifiers.Count; identifierIndex++)
                {
                    if (!this.innerScope.UsedLocalIdentifiers[identifierIndex].IsFunctionName)
                    { realIdentifiers++; }
                }

                if (realIdentifiers > 0)
                {
                    writer.WriteNewLine()
                        .Write(Keyword.Var);

                    for (int identifierIndex = 0, writtenIdentifiers = 0; identifierIndex < this.innerScope.UsedLocalIdentifiers.Count; identifierIndex++)
                    {
                        if (this.innerScope.UsedLocalIdentifiers[identifierIndex].IsFunctionName)
                        { continue; }

                        if (writtenIdentifiers++ > 0)
                        {
                            writer.Write(Symbols.Comma);
                        }

                        writer.Write(this.innerScope.UsedLocalIdentifiers[identifierIndex]);
                    }

                    writer.Write(Symbols.SemiColon);
                }
            }

            foreach (Statement statement in this.Statements)
            {
                writer.Write(statement);
            }

            writer.ExitScope().Write(Symbols.BracketCloseCurly);
        }
    }
}
