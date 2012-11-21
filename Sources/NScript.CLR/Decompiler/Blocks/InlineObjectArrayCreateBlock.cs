//-----------------------------------------------------------------------
// <copyright file="InlineObjectArrayCreateBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler.Blocks
{
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Definition for InlineObjectArrayCreateBlock
    /// </summary>
    internal class InlineObjectArrayCreateBlock : BasicBlock
    {
        /// <summary>
        /// Backing field for size;
        /// </summary>
        private int size;

        /// <summary>
        /// Backing field for ElementInitBlocks.
        /// </summary>
        private Block[] elementInitBlocks;

        /// <summary>
        /// Backing field for ArrayInitBlock.
        /// </summary>
        private StackedBlock arrayInitBlock;

        /// <summary>
        /// Backing field for ArrayLoadBlock.
        /// </summary>
        private InstructionBlock arrayLoadBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineObjectArrayCreateBlock"/> class.
        /// </summary>
        /// <param name="blockToIndexMapping">The block to index mapping.</param>
        /// <param name="childBlocks">The child blocks.</param>
        public InlineObjectArrayCreateBlock(
            int[] blockToIndexMapping,
            BasicBlock[] childBlocks)
            : base(childBlocks)
        {
            // Child block contains array creation block and final array load block and all
            // the element initialization blocks.
            this.size = blockToIndexMapping.Length;
            this.elementInitBlocks = new Block[this.size];
            this.arrayInitBlock = (StackedBlock)childBlocks[0];
            this.arrayLoadBlock = (InstructionBlock)childBlocks[childBlocks.Length - 1];

            for (int blockToIndex = 0; blockToIndex < blockToIndexMapping.Length; blockToIndex++)
            {
                if (blockToIndexMapping[blockToIndex] > 0)
                {
                    this.elementInitBlocks[blockToIndex] =
                        ((StackedBlock) childBlocks[blockToIndexMapping[blockToIndex]]).GetDependent(2);
                }
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// Gets the element init blocks.
        /// </summary>
        /// <value>The element init blocks.</value>
        public IList<Block> ElementInitBlocks
        {
            get { return this.elementInitBlocks; }
        }

        /// <summary>
        /// Gets the array init block.
        /// </summary>
        /// <value>The array init block.</value>
        public StackedBlock ArrayInitBlock
        {
            get { return this.arrayInitBlock; }
        }

        /// <summary>
        /// Gets the array load block.
        /// </summary>
        /// <value>The array load block.</value>
        public InstructionBlock ArrayLoadBlock
        {
            get { return this.arrayLoadBlock; }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            Location location = this.ComputeLocation();
            TypeReference typeReference =
                (TypeReference)((StackedBlock)this.ArrayInitBlock.GetDependent(0)).RootBlock.Instruction.OpCodeArgument;

            Expression[] expressions = new Expression[this.Size];

            for (int elementIndex = 0; elementIndex < this.Size; elementIndex++)
            {
                if (this.elementInitBlocks[elementIndex] == null)
                {
                    expressions[elementIndex] = new DefaultValueExpression(
                        this.Context.ClrContext,
                        location,
                        typeReference);
                }
                else
                {
                    expressions[elementIndex] =
                        (Expression) this.ElementInitBlocks[elementIndex].ToAstNode(variableResolver);
                }
            }

            return new AST.InlineArrayInitialization(
                this.Context.ClrContext,
                location,
                typeReference,
                expressions);
        }
    }
}
