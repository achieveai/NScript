namespace NScript.CLR.AST
{
    using System.Runtime.Serialization;
    using NScript.Utils;

    /// <summary>
    /// Return statement, may or may not have Return expression.
    /// </summary>
    public class ReturnStatement : Statement
    {
        /// <summary>
        /// Backing field for returnExpression.
        /// </summary>
        private Expression returnExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnStatement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="returnExpression">The return expression.</param>
        public ReturnStatement(
            ClrContext context,
            Location location,
            Expression returnExpression)
            : base(context, location)
        {
            this.returnExpression = returnExpression;
        }

        /// <summary>
        /// Gets the return expression.
        /// </summary>
        /// <value>The return expression.</value>
        public Expression ReturnExpression
        {
            get
            {
                return this.returnExpression;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            if (this.returnExpression != null)
            {
                this.returnExpression = (Expression)processor.Process(this.returnExpression);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("returnExpression", this.returnExpression);
        }
    }
}
