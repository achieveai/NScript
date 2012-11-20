//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Definition for ExceptionHandlerBlock
    /// </summary>
    internal class ExceptionHandlerBlock : Block
    {
        /// <summary>
        /// Backing field for IsTryCatch.
        /// </summary>
        private bool isTryCatch;

        /// <summary>
        /// Backing field for ExceptionType
        /// </summary>
        private TypeReference exceptionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerBlock"/> class.
        /// </summary>
        /// <param name="isTryCatch">if set to <c>true</c> [is try catch].</param>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="tryBlock">The try block.</param>
        /// <param name="handlerBlock">The handler block.</param>
        public ExceptionHandlerBlock(
            bool isTryCatch,
            TypeReference exceptionType,
            Block tryBlock,
            Block handlerBlock)
            : base(new Block[] { tryBlock, handlerBlock })
        {
            this.isTryCatch = isTryCatch;
            this.exceptionType = exceptionType;
        }

        /// <summary>
        /// Gets the try block.
        /// </summary>
        public Block TryBlock
        {
            get { return this.Children[0]; }
        }

        /// <summary>
        /// Gets the handler block.
        /// </summary>
        public Block HandlerBlock
        {
            get { return this.Children[1]; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is try catch.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is try catch; otherwise, <c>false</c>.
        /// </value>
        public bool IsTryCatch
        {
            get { return this.isTryCatch; }
        }

        /// <summary>
        /// Gets the type of the exception.
        /// </summary>
        /// <value>
        /// The type of the exception.
        /// </value>
        public TypeReference ExceptionType
        {
            get { return this.exceptionType; }
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
            /*
            HashSet<Block> ignoreLeavesFor = new HashSet<Block>();
            Block tempH = this.HandlerBlock;
            Block tempT = this;

            while (tempH != null && tempT != null && tempT is ExceptionHandler)
            {
                ignoreLeavesFor.Add(tempT.Parent.Children[tempT.Parent.Children.IndexOf(tempT) + 1]);

                tempH = tempT.Parent;
                tempT = tempH.Parent != null
                    ? tempH.Parent
                    : null;
            }
             */

            AST.Node tryBlockNode = this.TryBlockToAst(variableResolver);
            AST.TryCatchFinally innerTryBlock = tryBlockNode as AST.TryCatchFinally;

            if (innerTryBlock != null
                && innerTryBlock.Handlers[innerTryBlock.Handlers.Count-1].IsCatchBlock)
            {
                // We can't append to finally block so go normal route if there is a finally block inside.
                innerTryBlock.AddHandler(
                    this.HandlerToAst(variableResolver));

                return innerTryBlock;
            }
            else
            {
                return new AST.TryCatchFinally(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    (AST.ScopeBlock)tryBlockNode,
                    this.HandlerToAst(variableResolver));
            }
        }

        /// <summary>
        /// Tries the block to ast.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>Try scope block</returns>
        private AST.Node TryBlockToAst(VariableResolver variableResolver)
        {
            AST.ScopeBlock scopeBlock = new AST.ScopeBlock(
                this.Context.ClrContext,
                this.HandlerBlock.ComputeLocation());

            // Since last block is always going to be leave/goto block, let's ignore that block.
            for (int childIndex = 0; childIndex < this.TryBlock.Children.Count; childIndex++)
            {
                Block child = this.TryBlock.Children[childIndex];

                AST.Statement statement = AST.Statement.ToStatement(child.ToAstNode(variableResolver));

                if (statement != null)
                {
                    scopeBlock.AddStatement(statement);
                }
            }

            if (scopeBlock.Statements.Count == 1
                && scopeBlock.Statements[0] is AST.TryCatchFinally)
            {
                return scopeBlock.Statements[0];
            }

            return scopeBlock;
        }

        /// <summary>
        /// Handlers to ast.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>Handler scope block.</returns>
        private AST.HandlerBlock HandlerToAst(VariableResolver variableResolver)
        {
            int startIndex = 0;
            AST.VariableReference variableReference = null;

            if (this.isTryCatch)
            {
                startIndex = 1;
                IlInstruction instruction = ((StackedBlock)this.HandlerBlock.Children[0]).RootBlock.Instruction;
                if (instruction.OpCode == OpCode.StoreLocal)
                {
                    variableReference = new AST.VariableReference(
                        this.Context.ClrContext,
                        instruction.Location,
                        variableResolver.ResolveLocal((VariableDefinition)instruction.OpCodeArgument));
                }
            }

            AST.ScopeBlock scopeBlock = new AST.ScopeBlock(
                this.Context.ClrContext,
                this.HandlerBlock.ComputeLocation());

            // Since last block is always going to be leave/goto block, let's ignore that block.
            for (int childIndex = startIndex; childIndex < this.HandlerBlock.Children.Count; childIndex++)
            {
                Block child = this.HandlerBlock.Children[childIndex];

                AST.Statement statement = AST.Statement.ToStatement(child.ToAstNode(variableResolver));

                if (statement != null)
                {
                    scopeBlock.AddStatement(statement);
                }
            }

            return new AST.HandlerBlock(
                this.Context.ClrContext,
                scopeBlock.Location,
                this.isTryCatch
                    ? (this.ExceptionType ??
                        this.Context.ClrContext.KnownReferences.Exception)
                    : null,
                variableReference,
                scopeBlock);
        }
    }
}
