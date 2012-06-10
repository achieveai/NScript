//-----------------------------------------------------------------------
// <copyright file="ForEachBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for ForEachBlock
    /// </summary>
    internal class ForEachBlock : Block
    {
        /// <summary>
        /// Backing field for loopVariable.
        /// </summary>
        private VariableDefinition loopVariableIndex;

        /// <summary>
        /// Backing field for Colleciton Block.
        /// </summary>
        private Block collectionBlock;

        /// <summary>
        /// Backing field for TypeReference.
        /// </summary>
        private TypeReference typeReferenceBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForEachBlock"/> class.
        /// </summary>
        /// <param name="collectionBlock">The collection block.</param>
        /// <param name="loopVariable">The loop variable.</param>
        /// <param name="typeReferenceBase">The type reference base.</param>
        /// <param name="children">The children.</param>
        public ForEachBlock(
            Block collectionBlock,
            VariableDefinition loopVariable,
            TypeReference typeReferenceBase,
            Block[] children)
            : base(children)
        {
            this.loopVariableIndex = loopVariable;
            this.collectionBlock = collectionBlock;
            this.typeReferenceBase = typeReferenceBase;
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
            ScopeBlock scopeBlock = new ScopeBlock(
                this.Context.ClrContext,
                null);
            AST.Expression collection = (AST.Expression)this.collectionBlock.ToAstNode(variableResolver);
            AST.LocalVariable localVariable = variableResolver.ResolveLocal(this.loopVariableIndex);

            WhileBlock whileLoop = (WhileBlock)((ExceptionHandlerBlock)this.Children[1]).TryBlock.Children[0];

            // We start from 1 so that we can skip foreach item variable assignment.
            for (int i = 1; i < whileLoop.Loop.Children.Count; i++)
            {
                Block block = whileLoop.Loop.Children[i];

                AST.Statement statement = AST.Statement.ToStatement(
                    block.ToAstNode(variableResolver));

                if (statement != null)
                {
                    scopeBlock.AddStatement(statement);
                }
            }

            return new AST.ForEachLoop(
                this.Context.ClrContext,
                this.ComputeLocation(),
                localVariable,
                collection,
                scopeBlock);
        }
    }
}
