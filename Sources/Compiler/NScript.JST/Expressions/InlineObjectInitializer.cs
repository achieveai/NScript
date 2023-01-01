//-----------------------------------------------------------------------
// <copyright file="InlineObjectInitializer.cs" company="">
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
    /// Definition for InlineObjectInitializer
    /// </summary>
    public class InlineObjectInitializer : Expression
    {
        /// <summary>
        /// Backing collection for initializers.
        /// </summary>
        private readonly List<Tuple<Expression, Expression>> initializers =
            new List<Tuple<Expression,Expression>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineObjectInitializer"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">the scope</param>
        public InlineObjectInitializer(
            Location location,
            IdentifierScope scope)
            : base(location, scope)
        {
            Initializers = new ReadOnlyCollection<Tuple<Expression,Expression>>(initializers);
        }

        /// <summary>
        /// Adds the initializer.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="expression">The expression.</param>
        public void AddInitializer(
            IIdentifier identifier,
            Expression expression)
        {
            this.initializers.Add(
                new Tuple<Expression, Expression>(
                    new IdentifierExpression(identifier, expression.Scope),
                    expression));
        }

        /// <summary>
        /// Adds the initializer.
        /// </summary>
        /// <param name="strIdentifier">The STR identifier.</param>
        /// <param name="expression">The expression.</param>
        public void AddInitializer(
            string strIdentifier,
            Expression expression)
        {
            this.initializers.Add(
                Tuple.Create<Expression, Expression>(
                    new StringLiteralExpression(null, strIdentifier),
                    expression));
        }

        /// <summary>
        /// Gets the initializer count.
        /// </summary>
        public int InitializerCount
        {
            get { return this.initializers.Count; }
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>The precendence.</value>
        public override Precedence Precedence
        {
            get
            {
                return JST.Precedence.Primary;
            }
        }

        public IReadOnlyList<Tuple<Expression, Expression>> Initializers
        { get; }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue(
                "initializers",
                this.initializers,
                (s, e) => s.AddValue("field", e.Item1));
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            if (this.initializers.Count > 0)
            {
                _ = writer.Write(Symbols.BrackedOpenCurly)
                    .EnterScope();

                for (int initializerIndex = 0;
                    initializerIndex < this.initializers.Count;
                    initializerIndex++)
                {
                    if (initializerIndex > 0)
                    {
                        _ = writer.Write(Symbols.Comma);
                    }

                    var initializer = this.initializers[initializerIndex];
                    _ = writer.WriteNewLine()
                        .Write(initializer.Item1)
                        .Write(Symbols.Colon);
                    if (initializer.Item2.Precedence == Precedence.Comma)
                    {
                        _ = writer
                            .Write(Symbols.BracketOpenRound)
                            .Write(initializer.Item2)
                            .Write(Symbols.BracketCloseRound);
                    }
                    else
                    {
                        _ = writer.Write(initializer.Item2);
                    }
                }

                _ = writer.ExitScope()
                    .Write(Symbols.BracketCloseCurly);
            }
            else
            {
                _ = writer.Write(Symbols.BrackedOpenCurly)
                    .Write(Symbols.BracketCloseCurly);
            }
        }
    }
}
