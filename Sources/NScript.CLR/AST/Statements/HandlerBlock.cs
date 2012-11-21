//-----------------------------------------------------------------------
// <copyright file="HandlerBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for HandlerBlock
    /// </summary>
    public class HandlerBlock : Statement
    {
        /// <summary>
        /// Backing field for catchVariable.
        /// </summary>
        private VariableReference catchVariable;

        /// <summary>
        /// backing field for Block.
        /// </summary>
        private ScopeBlock block;

        /// <summary>
        /// Backing field for catchType.
        /// </summary>
        private TypeReference catchType;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerBlock"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="catchTypeReference">The catch type reference.</param>
        /// <param name="catchVariableExpression">The catch variable expression.</param>
        /// <param name="block">The block.</param>
        public HandlerBlock(
            ClrContext context,
            Location location,
            TypeReference catchTypeReference,
            VariableReference catchVariableExpression,
            ScopeBlock block)
            : base(context, location)
        {
            this.catchType = catchTypeReference;
            this.catchVariable = catchVariableExpression;
            this.block = block;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is catch block.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is catch block; otherwise, <c>false</c>.
        /// </value>
        public bool IsCatchBlock
        {
            get { return this.catchType != null; }
        }

        /// <summary>
        /// Gets the catch variable.
        /// </summary>
        public VariableReference CatchVariable
        {
            get { return this.catchVariable; }
            set
            {
                if (this.IsCatchBlock && this.catchVariable == null)
                {
                    this.catchVariable = value;
                    return;
                }

                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the type of the catch.
        /// </summary>
        /// <value>
        /// The type of the catch.
        /// </value>
        public TypeReference CatchType
        {
            get { return this.catchType; }
        }

        /// <summary>
        /// Gets the block.
        /// </summary>
        public ScopeBlock Block
        {
            get { return this.block; }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("isCatch", this.IsCatchBlock);
            if (this.IsCatchBlock)
            {
                serializationInfo.AddValue("catchType", this.CatchType.ToString());
            }

            serializationInfo.AddValue("catchVar", this.CatchVariable);
            serializationInfo.AddValue("block", this.Block);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.block = (ScopeBlock)processor.Process(this.block);
        }
    }
}