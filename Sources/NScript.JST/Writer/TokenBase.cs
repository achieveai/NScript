//-----------------------------------------------------------------------
// <copyright file="TokenBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST.Writer
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for TokenBase
    /// </summary>
    internal abstract class TokenBase
    {
        /// <summary>
        /// Backing field for Type.
        /// </summary>
        private readonly TokenType type;

        /// <summary>
        /// Backing field for Location.
        /// </summary>
        private readonly Location location;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBase"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="location">The location.</param>
        protected TokenBase(TokenType type, Location location)
        {
            this.type = type;
            this.location = location;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public TokenType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location Location
        {get { return this.location; }}
    }
}
