//-----------------------------------------------------------------------
// <copyright file="InlinePropertyInitializerBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Cs2JsC.CLR.AST;

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for InlinePropertyInitializerBlock
    /// </summary>
    internal class InlinePropertyInitializerBlock : BasicBlock
    {
        /// <summary>
        /// Backing field for InitBlock.
        /// </summary>
        private readonly StackedBlock initBlock;

        /// <summary>
        /// Backing field for Setters.
        /// </summary>
        private readonly List<StackedBlock> setters =
            new List<StackedBlock>();

        /// <summary>
        /// Backing field for LoadBlock.
        /// </summary>
        private readonly BasicBlock loadBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinePropertyInitializerBlock"/> class.
        /// </summary>
        /// <param name="childBlocks">The child blocks.</param>
        public InlinePropertyInitializerBlock(
            BasicBlock[] childBlocks)
            : base(childBlocks)
        {
            this.initBlock = (StackedBlock)childBlocks[0];

            for (int childIndex = 1; childIndex < childBlocks.Length-1; childIndex++)
            {
                this.setters.Add((StackedBlock) childBlocks[childIndex]);
            }

            this.loadBlock = childBlocks[childBlocks.Length - 1];
        }

        /// <summary>
        /// Gets the init block.
        /// </summary>
        /// <value>The init block.</value>
        public StackedBlock InitBlock
        {
            get { return this.initBlock; }
        }

        /// <summary>
        /// Gets the setters.
        /// </summary>
        /// <value>The setters.</value>
        public IList<StackedBlock> Setters
        {
            get { return this.setters; }
        }

        /// <summary>
        /// Gets the load block.
        /// </summary>
        /// <value>The load block.</value>
        public BasicBlock LoadBlock
        {
            get { return this.loadBlock; }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.NewObjectExpression newObjectExpression =
                (AST.NewObjectExpression) this.InitBlock.GetDependent(0).ToAstNode(variableResolver);

            List<Tuple<AST.MemberReferenceExpression, Expression[]>> setters = new List<Tuple<MemberReferenceExpression, Expression[]>>();

            foreach (StackedBlock setter in this.Setters)
            {
                var binaryExpression = (BinaryExpression) setter.ToAstNode(variableResolver);

                if (binaryExpression.Left is FieldReferenceExpression)
                {
                    setters.Add(
                        Tuple.Create<MemberReferenceExpression, Expression[]>(
                            new FieldReferenceExpression(
                                this.Context.ClrContext,
                                binaryExpression.Left.Location,
                                ((FieldReferenceExpression)binaryExpression.Left).FieldReference,
                                null),
                            new Expression[] { binaryExpression.Right }));
                }
                else
                {
                    setters.Add(
                        Tuple.Create<MemberReferenceExpression, Expression[]>(
                            new PropertyReferenceExpression(
                                this.Context.ClrContext,
                                binaryExpression.Left.Location,
                                ((PropertyReferenceExpression) binaryExpression.Left).PropertyReference,
                                null),
                            new Expression[] { binaryExpression.Right }));
                }
            }

            return new InlinePropertyInitilizationExpression(
                this.Context.ClrContext,
                this.ComputeLocation(),
                newObjectExpression,
                setters);
        }
    }
}
