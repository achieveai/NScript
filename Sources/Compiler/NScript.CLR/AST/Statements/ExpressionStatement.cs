namespace NScript.CLR.AST
{
    using System;

    /// <summary>
    /// Statement that is based on expression.
    /// </summary>
    public class ExpressionStatement : Statement
    {
        /// <summary>
        /// Backing field for Expression.
        /// </summary>
        private Expression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionStatement"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public ExpressionStatement(Expression expression)
            : base(expression.Context, expression.Location)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.expression = (Expression)processor.Process(this.expression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("expression", this.Expression);
        }
    }
}
