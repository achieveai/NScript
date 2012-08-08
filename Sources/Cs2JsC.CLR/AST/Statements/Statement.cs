namespace Cs2JsC.CLR.AST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Base class for all the statements.
    /// </summary>
    public abstract class Statement : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Statement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        public Statement(
            ClrContext context,
            Location location)
            : base(context, location)
        {
        }

        /// <summary>
        /// Toes the statement.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Node itself if it is statement else if it is expresion, Expression statement is retunred.</returns>
        public static Statement ToStatement(AST.Node node)
        {
            if (node is Expression)
            {
                return new ExpressionStatement((Expression) node);
            }
            else
            {
                return (Statement) node;
            }
        }
    }
}
