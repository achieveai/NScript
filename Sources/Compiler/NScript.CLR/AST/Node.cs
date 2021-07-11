//-----------------------------------------------------------------------
// <copyright file="IsTypeProcessor.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;

    public abstract class Node : ICustomSerializable
    {
        /// <summary>
        /// Backing field for Context
        /// </summary>
        private readonly ClrContext clrContext;

        /// <summary>
        /// Backing field for Location.
        /// </summary>
        private readonly Location location;

        /// <summary>
        /// Backing field for parentNode.
        /// </summary>
        private Node parentNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        protected Node(
            ClrContext clrContext,
            Location location)
        {
            this.location = location;
            this.clrContext = clrContext;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ClrContext Context
        {
            get { return this.clrContext; }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        /// 
        public Location Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public Node ParentNode
        {
            get { return this.parentNode; }
            private set { this.parentNode = value; }
        }

        /// <summary>
        /// Gets the known references.
        /// </summary>
        protected ClrKnownReferences KnownReferences
        { get { return this.clrContext.KnownReferences; } }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public virtual void ProcessThroughPipeline(
            IAstProcessor processor)
        {
            throw new NotImplementedException(this.GetType().Name);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        public virtual void Serialize(ICustomSerializer serializationInfo)
        {
        }
    }
}
