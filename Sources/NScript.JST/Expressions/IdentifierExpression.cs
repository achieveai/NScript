//-----------------------------------------------------------------------
// <copyright file="IdentifierExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NScript.Utils;
namespace NScript.JST
{
    /// <summary>
    /// Identifier expression that contains identifier.
    /// </summary>
    public class IdentifierExpression : Expression
    {
        /// <summary>
        /// Backing field for idnetifier.
        /// </summary>
        private readonly IIdentifier identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierExpression"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="scope">The scope.</param>
        public IdentifierExpression(
            IIdentifier identifier,
            IdentifierScope scope,
            Location loc = null)
            : base(loc, scope)
        {
            this.identifier = identifier;
            this.identifier.AddUsage(this.Scope);
        }

        /// <summary>
        /// Creates the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="identifiers">The identifiers.</param>
        /// <returns>Expression representing the full set of indexes.</returns>
        public static Expression Create(
            Location location,
            IdentifierScope scope,
            IList<IIdentifier> identifiers,
            int lastIndex = -1)
        {
            if (lastIndex == -1)
            {
                lastIndex = identifiers.Count;
            }

            if (lastIndex > 1)
            {
                return new IndexExpression(
                    location,
                    scope,
                    identifiers,
                    lastIndex);
            }

            if (lastIndex == 1)
            {
                return new IdentifierExpression(
                    identifiers[0],
                    scope);
            }

            throw new ArgumentException("identifiers");
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public IIdentifier Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>The precendence.</value>
        public override Precedence Precedence
        {
            get
            {
                return Precedence.Primary;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("name", this.Identifier.SuggestedName);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(this.identifier);
        }
    }
}
