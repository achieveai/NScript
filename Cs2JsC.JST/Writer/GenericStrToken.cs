//-----------------------------------------------------------------------
// <copyright file="GenericStrToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST.Writer
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for GenericStrToken
    /// </summary>
    internal class GenericStrToken :TokenBase
    {
        /// <summary>
        /// Backing field for String.
        /// </summary>
        private readonly string str;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericStrToken"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="str">The string.</param>
        /// <param name="tokenType">Type of the token.</param>
        public GenericStrToken(Location location, string str, TokenType tokenType)
            :base(tokenType, location)
        {
            this.str = str;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <value>The string.</value>
        public string String
        {
            get { return this.str; }
        }
    }
}
