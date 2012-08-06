//-----------------------------------------------------------------------
// <copyright file="KeywordToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST.Writer
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for KeywordToken
    /// </summary>
    internal class KeywordToken : TokenBase
    {
        /// <summary>
        /// Backing field for Keyword;
        /// </summary>
        private readonly Keyword keyword;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordToken"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="keyword">The keyword.</param>
        public KeywordToken(Location location, Keyword keyword)
            :base(TokenType.Keyword, location)
        {
            this.keyword = keyword;
        }

        /// <summary>
        /// Gets the keyword.
        /// </summary>
        /// <value>The keyword.</value>
        public Keyword Keyword
        {get { return this.keyword; }}
    }
}
