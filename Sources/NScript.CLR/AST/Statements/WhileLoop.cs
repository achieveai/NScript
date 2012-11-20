//-----------------------------------------------------------------------
// <copyright file="WhileLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;

    /// <summary>
    /// Definition for WhileLoop
    /// </summary>
    public class WhileLoop : Statement
    {
        /// <summary>
        /// Backing field for conditionExpression.
        /// </summary>
        private Expression conditionExpression;

        /// <summary>
        /// Backing field for scopeBlock.
        /// </summary>
        private ExplicitBlock loopBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoWhileLoop"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <param name="loopBlock">The loop block.</param>
        public WhileLoop(
            ClrContext context,
            Location location,
            Expression conditionExpression,
            ExplicitBlock loopBlock)
            : base(context, location)
        {
            this.conditionExpression = conditionExpression;
            this.loopBlock = loopBlock;
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
        /// Gets the loop block.
        /// </summary>
        /// <value>The loop block.</value>
        public ExplicitBlock LoopBlock
        {
            get { return this.loopBlock; }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.conditionExpression = (Expression)processor.Process(this.conditionExpression);
            this.loopBlock = (ScopeBlock)processor.Process(this.loopBlock);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("condition", this.conditionExpression);
            serializationInfo.AddValue("loopBlock", this.loopBlock);
        }
    }
}