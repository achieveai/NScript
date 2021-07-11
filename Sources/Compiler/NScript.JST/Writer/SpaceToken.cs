//-----------------------------------------------------------------------
// <copyright file="SpaceToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using NScript.Utils;
namespace NScript.JST.Writer
{
    /// <summary>
    /// Definition for SpaceToken
    /// </summary>
    internal class SpaceToken : TokenBase
    {
        /// <summary>
        /// Backing field for SpaceCount.
        /// </summary>
        private int spaceCount = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceToken"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public SpaceToken(Location location)
            :base(TokenType.Space, location)
        {
        }

        /// <summary>
        /// Gets the space count.
        /// </summary>
        /// <value>The space count.</value>
        public int SpaceCount
        {
            get { return this.spaceCount; }
            set { this.spaceCount = value; }
        }
    }
}
