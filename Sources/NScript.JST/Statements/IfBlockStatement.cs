//-----------------------------------------------------------------------
// <copyright file="IfBlockStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for IfBlockStatement
    /// </summary>
    public class IfBlockStatement : Statement
    {
        /// <summary>
        /// Backing field conditionExpression.
        /// </summary>
        private Expression conditionExpression;

        /// <summary>
        /// Backing field for TrueBlock
        /// </summary>
        private ScopeBlock trueBlock;

        /// <summary>
        /// Backing field for FalseBlock.
        /// </summary>
        private ScopeBlock falseBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfBlockStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="trueBlock">The true block.</param>
        /// <param name="falseBlock">The false block.</param>
        public IfBlockStatement(
            Location location,
            IdentifierScope scope,
            Expression condition,
            ScopeBlock trueBlock,
            ScopeBlock falseBlock)
            : base(location, scope)
        {
            this.conditionExpression = condition;
            this.trueBlock = trueBlock;
            this.falseBlock = falseBlock;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Expression Condition
        {
            get { return this.conditionExpression; }
        }

        /// <summary>
        /// Gets the true block.
        /// </summary>
        /// <value>The true block.</value>
        public ScopeBlock TrueBlock
        {
            get { return this.trueBlock; }
        }

        /// <summary>
        /// Gets the false block.
        /// </summary>
        /// <value>The false block.</value>
        public ScopeBlock FalseBlock
        {
            get { return this.falseBlock; }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("conditon", this.Condition);
            serializer.AddValue("trueCase", this.TrueBlock);
            serializer.AddValue("falseCase", this.FalseBlock);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine();

            this.WriteNestedIf(writer);
        }

        /// <summary>
        /// Writes the nested if.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void WriteNestedIf(JSWriter writer, bool nested = false)
        {
            writer.EnterLocation(this.conditionExpression.Location);
            if (nested)
            {
                writer.Write(Keyword.Else);
            }

            writer.Write(Keyword.If)
                .Write(this.conditionExpression, true)
                .LeaveLocation();

            if (this.FalseBlock != null
                && this.ShouldForceIfBlockEncapsulation(this.TrueBlock))
            {
                writer.Write(Symbols.BrackedOpenCurly)
                    .Write(this.trueBlock)
                    .Write(Symbols.BracketCloseCurly);
            }
            else
            {
                    writer.Write(this.trueBlock);
            }

            if (this.FalseBlock != null)
            {
                if (this.FalseBlock.Statements.Count == 1 &&
                    this.FalseBlock.Statements[0] is IfBlockStatement)
                {
                    writer.WriteNewLine();
                    ((IfBlockStatement)this.FalseBlock.Statements[0]).WriteNestedIf(writer, true);
                }
                else
                {
                    writer.WriteNewLine()
                        .Write(Keyword.Else)
                        .Write(this.falseBlock);
                }
            }
        }

        /// <summary>
        /// Shoulds the force if block encapsulation.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>true if we need to force block encapsulation.</returns>
        private bool ShouldForceIfBlockEncapsulation(Statement statement)
        {
            ScopeBlock scopeblock = statement as ScopeBlock;
            if (scopeblock != null)
            {
                if (scopeblock.Statements.Count == 1)
                {
                    return this.ShouldForceIfBlockEncapsulation(scopeblock.Statements[0]);
                }

                return false;
            }

            IfBlockStatement ifBlock = statement as IfBlockStatement;
            if (ifBlock != null)
            {
                if (ifBlock.FalseBlock == null)
                {
                    return this.ShouldForceIfBlockEncapsulation2(ifBlock.TrueBlock);
                }

                return this.ShouldForceIfBlockEncapsulation2(ifBlock.falseBlock);
            }

            ForLoop forLoop = statement as ForLoop;
            if (forLoop != null)
            {
                return this.ShouldForceIfBlockEncapsulation2(forLoop.Loop);
            }

            ForInLoop forInLoop = statement as ForInLoop;
            if (forInLoop != null)
            {
                return this.ShouldForceIfBlockEncapsulation2(forInLoop.Loop);
            }

            WhileLoop whileLoop = statement as WhileLoop;
            if (whileLoop != null)
            {
                return this.ShouldForceIfBlockEncapsulation2(whileLoop.Loop);
            }

            return false;
        }

        /// <summary>
        /// Shoulds the force if block encapsulation.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>true if we need to force block encapsulation.</returns>
        private bool ShouldForceIfBlockEncapsulation2(Statement statement)
        {
            ScopeBlock scopeblock = statement as ScopeBlock;
            if (scopeblock != null)
            {
                if (scopeblock.Statements.Count == 1)
                {
                    return this.ShouldForceIfBlockEncapsulation2(scopeblock.Statements[0]);
                }

                return false;
            }

            IfBlockStatement ifBlock = statement as IfBlockStatement;
            if (ifBlock != null)
            {
                if (ifBlock.FalseBlock == null)
                {
                    return this.ShouldForceIfBlockEncapsulation2(ifBlock.TrueBlock);
                }

                return this.ShouldForceIfBlockEncapsulation2(ifBlock.falseBlock);
            }

            ForLoop forLoop = statement as ForLoop;
            if (forLoop != null)
            {
                return this.ShouldForceIfBlockEncapsulation2(forLoop.Loop);
            }

            ForInLoop forInLoop = statement as ForInLoop;
            if (forInLoop != null)
            {
                return this.ShouldForceIfBlockEncapsulation2(forInLoop.Loop);
            }

            WhileLoop whileLoop = statement as WhileLoop;
            if (whileLoop != null)
            {
                return this.ShouldForceIfBlockEncapsulation2(whileLoop.Loop);
            }

            return statement is ExpressionStatement
                || statement is ReturnStatement;
        }
    }
}
