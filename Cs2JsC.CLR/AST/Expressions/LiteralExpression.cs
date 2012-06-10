//-----------------------------------------------------------------------
// <copyright file="LiteralExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Base for all the literal expressions.
    /// </summary>
    public abstract class LiteralExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        protected LiteralExpression(
            ClrContext context,
            Location location)
            : base(context, location)
        {
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
        }
    }
}
