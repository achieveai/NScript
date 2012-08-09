//-----------------------------------------------------------------------
// <copyright file="TryCatchFinally.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for TryCatchFinally
    /// </summary>
    public class TryCatchFinally : Statement
    {
        /// <summary>
        /// Backing field for tryBlocks.
        /// </summary>
        private ScopeBlock tryBlock;

        /// <summary>
        /// Backing field for Handlerblocks
        /// </summary>
        private List<HandlerBlock> handlerBlocks = new List<HandlerBlock>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TryCatchFinally"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="tryBlock">The try block.</param>
        /// <param name="handlerBlock">The handler block.</param>
        public TryCatchFinally(
            ClrContext context,
            Location location,
            ScopeBlock tryBlock,
            HandlerBlock handlerBlock)
            : base(context, location)
        {
            this.tryBlock = tryBlock;
            this.handlerBlocks.Add(handlerBlock);
        }

        /// <summary>
        /// Gets the try block.
        /// </summary>
        public ScopeBlock TryBlock
        {
            get { return this.tryBlock; }
        }

        /// <summary>
        /// Gets the handlers.
        /// </summary>
        public IList<HandlerBlock> Handlers
        {
            get { return this.handlerBlocks; }
        }

        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void AddHandler(HandlerBlock handler)
        {
            this.handlerBlocks.Add(handler);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("try", this.tryBlock);
            serializationInfo.AddValue("handlers", this.handlerBlocks);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.tryBlock = (ScopeBlock)processor.Process(this.TryBlock);
            for (int handlerIndex = 0; handlerIndex < this.handlerBlocks.Count; handlerIndex++)
            {
                this.handlerBlocks[handlerIndex] =
                    (HandlerBlock)processor.Process(this.handlerBlocks[handlerIndex]);
            }
        }
    }
}
