//-----------------------------------------------------------------------
// <copyright file="ForLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ForLoop
    /// </summary>
    public class ForLoop : ScopeBlock
    {
        /// <summary>
        /// Backing field for condition
        /// </summary>
        private Expression condition;

        /// <summary>
        /// Backing field for initializeStatement;
        /// </summary>
        private Statement initializeStatement;

        /// <summary>
        /// Backing field for incrementStatement
        /// </summary>
        private Statement incrementStatement;

        /// <summary>
        /// Backing field for Loop.
        /// </summary>
        private Statement loop;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForLoop"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="initializeStatement">The initialize statement.</param>
        /// <param name="incrementStatement">The increment statement.</param>
        /// <param name="loop">The loop.</param>
        public ForLoop(
            ClrContext context,
            Location location,
            Expression condition,
            Statement initializeStatement,
            Statement incrementStatement,
            ScopeBlock loop,
            List<(LocalVariable localVariable, bool isUsed)> variables,
            List<LocalFunctionVariable> localFunctionNames)
            : base(context, location, variables, localFunctionNames)
        {
            this.condition = condition;
            this.initializeStatement = initializeStatement;
            this.incrementStatement = incrementStatement;
            this.loop = loop;

            // if (this.initializeStatement != null)
            // {
            //     base.AddStatement(this.initializeStatement);
            // }

            base.AddStatement(loop);

            // if (this.incrementStatement != null)
            // {
            //     base.AddStatement(this.incrementStatement);
            // }
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
        /// Gets the initialize statement.
        /// </summary>
        /// <value>The initialize statement.</value>
        public Statement InitializeStatement
        {
            get { return this.initializeStatement; }
        }

        /// <summary>
        /// Gets the increment statement.
        /// </summary>
        /// <value>The increment statement.</value>
        public Statement IncrementStatement
        {
            get { return this.incrementStatement; }
        }

        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <value>The loop.</value>
        public Statement Loop
        {
            get { return this.loop; }
        }

        /// <summary>
        /// Adds the statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public override void AddStatement(Statement statement)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.initializeStatement = (Statement) processor.Process(this.initializeStatement);
            this.condition = (Expression) processor.Process(this.condition);
            this.incrementStatement = (Statement) processor.Process(this.incrementStatement);
            this.loop = (Statement) processor.Process(this.loop);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("condition", this.condition);
            serializationInfo.AddValue("initialize", this.initializeStatement);
            serializationInfo.AddValue("increment", this.incrementStatement);
        }
    }
}
