//-----------------------------------------------------------------------
// <copyright file="DoWhileLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for DoWhileLoop
    /// </summary>
    public class DoWhileLoop : Statement
    {
        /// <summary>
        /// Backing field for conditionExpression.
        /// </summary>
        private Expression conditionExpression;

        /// <summary>
        /// Backing field for scopeBlock.
        /// </summary>
        private ScopeBlock loopBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoWhileLoop"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <param name="loopBlock">The loop block.</param>
        public DoWhileLoop(
            ClrContext context,
            Location location,
            Expression conditionExpression,
            ScopeBlock loopBlock)
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
        public ScopeBlock LoopBlock
        {
            get { return this.loopBlock; }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.conditionExpression = (Expression) processor.Process(this.conditionExpression);
            this.loopBlock = (ScopeBlock) processor.Process(this.loopBlock);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("loopBlock", this.loopBlock);
            serializationInfo.AddValue("condition", this.conditionExpression);
        }
    }
}
