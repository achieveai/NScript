//-----------------------------------------------------------------------
// <copyright file="InlineDelegateBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for InlineDelegateBlock
    /// </summary>
    internal class InlineDelegateBlock : Block
    {
        /// <summary>
        /// Backing field for delegateBlock.
        /// </summary>
        StackedBlock delegateBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDelegateBlock"/> class.
        /// </summary>
        /// <param name="delegateBlock">The delegate block.</param>
        /// <param name="children">The children.</param>
        public InlineDelegateBlock(
            StackedBlock delegateBlock,
            Block[] children)
            : base(children)
        {
            this.delegateBlock = delegateBlock;
        }

        /// <summary>
        /// Gets the delegate block.
        /// </summary>
        public StackedBlock DelegateBlock
        {
            get { return this.delegateBlock; }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>
        /// AST Node representing current block.
        /// </returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            return new AST.InlineDelegateExpression(
                this.Context.ClrContext,
                (AST.DelegateMethodExpression)this.DelegateBlock.ToAstNode(variableResolver));
        }
    }
}
