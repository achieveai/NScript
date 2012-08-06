//-----------------------------------------------------------------------
// <copyright file="NewlineToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST.Writer
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for NewlineToken
    /// </summary>
    internal class NewlineToken : TokenBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewlineToken"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public NewlineToken(Location location):base(TokenType.Newline, location)
        {
        }
    }
}
