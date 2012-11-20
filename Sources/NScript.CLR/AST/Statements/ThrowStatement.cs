//-----------------------------------------------------------------------
// <copyright file="ThrowStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ThrowExpression
    /// </summary>
    public class ThrowStatement : Statement
    {
        private Expression innerExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowStatement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="innerExpression">The inner expression.</param>
        public ThrowStatement(
            ClrContext context,
            Location location,
            Expression innerExpression)
            : base(context, location)
        {
            this.innerExpression = innerExpression;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression
        {
            get { return this.innerExpression; }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("innerExpression", this.Expression);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.innerExpression = (Expression)processor.Process(this.Expression);
        }
    }
}
