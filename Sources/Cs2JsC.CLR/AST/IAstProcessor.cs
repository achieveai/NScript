//-----------------------------------------------------------------------
// <copyright file="IAstProcessor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System.Collections.Generic;

    /// <summary>
    /// Definition for IAstProcessor
    /// </summary>
    public interface IAstProcessor
    {
        /// <summary>
        /// Processes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Processed Node.</returns>
        Node Process(Node node);

        /// <summary>
        /// Processes the specified statements.
        /// </summary>
        /// <param name="statements">The statements.</param>
        /// <returns>Processed Statements.</returns>
        List<Statement> Process(List<Statement> statements);

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        TopLevelBlock Context { get; }
    }
}
