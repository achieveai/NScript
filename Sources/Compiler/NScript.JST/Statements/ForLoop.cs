//-----------------------------------------------------------------------
// <copyright file="ForLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ForLoop
    /// </summary>
    public class ForLoop : Statement
    {
        /// <summary>
        /// Backing field for Condition
        /// </summary>
        private readonly Expression condition;

        /// <summary>
        /// Backing field for InitialziationBlock
        /// </summary>
        private readonly Statement initializationBlock;

        /// <summary>
        /// Backing field for IncrementBlock
        /// </summary>
        private readonly Statement incrementBlock;

        /// <summary>
        /// Backing field for Loop
        /// </summary>
        private readonly Statement loopBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForLoop"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="initializationBlock">The initialization block.</param>
        /// <param name="incrementBlock">The increment block.</param>
        /// <param name="loopBlock">The loop block.</param>
        public ForLoop(
            Location location,
            IdentifierScope scope,
            Expression condition,
            Statement initializationBlock,
            Statement incrementBlock,
            Statement loopBlock)
            : base(location, scope)
        {
            this.condition = condition;
            this.initializationBlock = initializationBlock;
            this.incrementBlock = incrementBlock;
            this.loopBlock = loopBlock;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Expression Condition
        {
            get { return this.condition; }
        }

        /// <summary>
        /// Gets the initialization block.
        /// </summary>
        /// <value>The initialization block.</value>
        public Statement InitializationBlock
        {
            get { return this.initializationBlock; }
        }

        /// <summary>
        /// Gets the increment block.
        /// </summary>
        /// <value>The increment block.</value>
        public Statement IncrementBlock
        {
            get { return this.incrementBlock; }
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
            serializer.AddValue("condition", this.Condition);
            serializer.AddValue("initBlock", this.InitializationBlock);
            serializer.AddValue("incrementBlock", this.IncrementBlock);
            serializer.AddValue("loop", this.Loop);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .EnterLocation(this.Location)
                .Write(Keyword.For)
                .Write(Symbols.BracketOpenRound);

            if (this.InitializationBlock is InitializerStatement)
            {
                writer.EnterLocation(this.initializationBlock.Location)
                    .Write(this.initializationBlock)
                    .LeaveLocation();
            }
            else
            {
                ForLoop.WriteBlockWithCommaSeperator(
                    writer,
                    this.InitializationBlock);
                writer.Write(Symbols.SemiColon);
            }

            if (this.Condition != null)
            {
                writer.EnterLocation(this.Condition.Location)
                    .Write(this.Condition)
                    .LeaveLocation()
                    .Write(Symbols.SemiColon);
            }
            else
            {
                writer.Write(Symbols.SemiColon);
            }

            ForLoop.WriteBlockWithCommaSeperator(
                writer,
                this.IncrementBlock);

            writer.Write(Symbols.BracketCloseRound);

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

            writer.LeaveLocation();
        }

        /// <summary>
        /// Writes the block with comma seperator.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="block">The block.</param>
        private static void WriteBlockWithCommaSeperator(
            JSWriter writer,
            Statement block)
        {
            if (block != null)
            {
                if (block is ScopeBlock)
                {
                    ScopeBlock scopeBlock = (ScopeBlock) block;

                    for (int statementIndex = 0; statementIndex < scopeBlock.Statements.Count; statementIndex++)
                    {
                        if (statementIndex > 0)
                        {
                            writer.Write(Symbols.Comma);
                        }

                        ForLoop.WriteStatement(
                            writer,
                            (ExpressionStatement) scopeBlock.Statements[statementIndex]);
                    }
                }
                else
                {
                    ForLoop.WriteStatement(
                        writer,
                        (ExpressionStatement)block);
                }
            }
        }

        /// <summary>
        /// Writes the statement.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="statement">The statement.</param>
        private static void WriteStatement(
            JSWriter writer,
            ExpressionStatement statement)
        {
            writer.EnterLocation(statement.Location)
                .Write(statement.Expression)
                .LeaveLocation();
        }
    }
}
