//-----------------------------------------------------------------------
// <copyright file="Node.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Javascript AST node.
    /// </summary>
    public abstract class Node : ICustomSerializable
    {
        /// <summary>
        /// Backing field for location.
        /// </summary>
        private readonly Location location;

        /// <summary>
        /// Backging field for Scope.
        /// </summary>
        private readonly IdentifierScope scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        public Node(
            Location location,
            IdentifierScope scope)
        {
            this.location = location;
            this.scope = scope;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public IdentifierScope Scope
        {
            get
            {
                return this.scope;
            }
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void Write(JSWriter writer)
        {
            throw new System.NotSupportedException(this.GetType().Name);
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public virtual void Serialize(ICustomSerializer serializer)
        {
            throw new System.NotImplementedException(
                string.Format("Serialize not implemented for {0}", this.GetType().Name));
        }
    }
}
