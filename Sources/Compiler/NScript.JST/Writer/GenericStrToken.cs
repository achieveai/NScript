//-----------------------------------------------------------------------
// <copyright file="GenericStrToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST.Writer
{
    using NScript.Utils;

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
        /// Backing field for OriginalName — the pre-minification source-level name that
        /// this identifier token represents. Null for non-identifier tokens or when the
        /// emitted name already equals the original (nothing to resolve back to).
        /// </summary>
        private readonly string originalName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericStrToken"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="str">The string.</param>
        /// <param name="tokenType">Type of the token.</param>
        public GenericStrToken(Location location, string str, TokenType tokenType)
            :this(location, str, tokenType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericStrToken"/> class with an
        /// original source-level name for V3 source-map <c>names</c>-array population.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="str">The emitted text (possibly minified for identifiers).</param>
        /// <param name="tokenType">Type of the token.</param>
        /// <param name="originalName">Original pre-minification name, or null if unavailable.</param>
        public GenericStrToken(Location location, string str, TokenType tokenType, string originalName)
            :base(tokenType, location)
        {
            this.str = str;
            this.originalName = originalName;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <value>The string.</value>
        public string String
        {
            get { return this.str; }
        }

        /// <summary>
        /// Gets the original pre-minification name for this identifier token.
        /// Null when the token is not an identifier or when the emitted text already
        /// equals the original name.
        /// </summary>
        public string OriginalName
        {
            get { return this.originalName; }
        }
    }
}
