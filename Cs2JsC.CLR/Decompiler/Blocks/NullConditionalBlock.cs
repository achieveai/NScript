//-----------------------------------------------------------------------
// <copyright file="NullConditionalBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for NullConditionalBlock
    /// </summary>
    internal class NullConditionalBlock : BasicBlock
    {
        /// <summary>
        /// Backing field for FirstValueChoice.
        /// </summary>
        private readonly BasicBlock value1;

        /// <summary>
        /// Backing field for SecondValueChoice.
        /// </summary>
        private readonly BasicBlock value2;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullConditionalBlock"/> class.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="childBlock">The child block.</param>
        public NullConditionalBlock(
            BasicBlock value1,
            BasicBlock value2,
            SerialBlock childBlock)
            : base(new BasicBlock[] {childBlock})
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            return new AST.NullConditional(
                this.Context.ClrContext,
                this.ComputeLocation(),
                (AST.Expression) this.value1.ToAstNode(variableResolver),
                (AST.Expression) this.value2.ToAstNode(variableResolver));
        }
    }
}
