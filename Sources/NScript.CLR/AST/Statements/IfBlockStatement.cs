//-----------------------------------------------------------------------
// <copyright file="IfBlockStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;

    /// <summary>
    /// Definition for IfBlockStatement
    /// </summary>
    public class IfBlockStatement : Statement
    {
        /// <summary>
        /// Backing collection for TrueCaseStatements.
        /// </summary>
        private ScopeBlock trueCaseStatements;

        /// <summary>
        /// Backing collection for FalseCaseStatements.
        /// </summary>
        private ScopeBlock falseCaseStatements;

        /// <summary>
        /// Backing field for condition expression.
        /// </summary>
        private Expression conditionExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfBlockStatement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="trueCaseStatements">The true case statements.</param>
        /// <param name="falseCaseStatements">The false case statements.</param>
        public IfBlockStatement(
            ClrContext context,
            Location location,
            Expression condition,
            ScopeBlock trueCaseStatements,
            ScopeBlock falseCaseStatements)
            : base(context, location)
        {
            foreach (var trueCaseStatement in trueCaseStatements.Statements)
            {
                if (trueCaseStatement == null)
                {
                    throw new ArgumentNullException("trueCaseStatements has null element");
                }
            }

            if (falseCaseStatements != null)
            {
                foreach (var falseCaseStatement in falseCaseStatements.Statements)
                {
                    if (falseCaseStatement == null)
                    {
                        throw new ArgumentException("falseCaseStatement has null element");
                    }
                }
            }

            this.conditionExpression = condition;
            this.trueCaseStatements = trueCaseStatements;
            this.falseCaseStatements = falseCaseStatements;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Expression Condition
        {
            get
            {
                return this.conditionExpression;
            }
        }

        /// <summary>
        /// Gets the true case statements.
        /// </summary>
        /// <value>The true case statements.</value>
        public ScopeBlock TrueCaseStatements
        {
            get
            {
                return this.trueCaseStatements;
            }
        }

        /// <summary>
        /// Gets the false case statements.
        /// </summary>
        /// <value>The false case statements.</value>
        public ScopeBlock FalseCaseStatements
        {
            get
            {
                return this.falseCaseStatements;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.conditionExpression = (Expression)processor.Process(this.conditionExpression);
            this.trueCaseStatements = (ScopeBlock)processor.Process(this.trueCaseStatements);
            if (this.falseCaseStatements != null)
            {
                this.falseCaseStatements = (ScopeBlock)processor.Process(this.falseCaseStatements);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("condition", this.conditionExpression);
            serializationInfo.AddValue("trueBlock", this.trueCaseStatements);
            serializationInfo.AddValue("falseBlock", this.falseCaseStatements);
        }
    }
}
