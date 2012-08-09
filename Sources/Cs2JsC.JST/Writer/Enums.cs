//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST.Writer
{
    /// <summary>
    /// Token types for TokenPoint.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// Keyword token
        /// </summary>
        Keyword,

        /// <summary>
        /// Symbol token
        /// </summary>
        Symbol,

        /// <summary>
        /// space token
        /// </summary>
        Space,

        /// <summary>
        /// Newline token
        /// </summary>
        Newline,

        /// <summary>
        /// Any generic string
        /// </summary>
        StrToken,

        /// <summary>
        /// Number token
        /// </summary>
        NumToken,

        /// <summary>
        /// Identifier token.
        /// </summary>
        IdentifierToken,

        /// <summary>
        /// Scope Token.
        /// </summary>
        ScopeToken,
    }
}
