//-----------------------------------------------------------------------
// <copyright file="SymbolToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST.Writer
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for SymbolToken
    /// </summary>
    internal class SymbolToken : TokenBase
    {
        /// <summary>
        /// Backing field for Symbol.
        /// </summary>
        private readonly Symbols symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolToken"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="symbol">The symbol.</param>
        public SymbolToken(Location location, Symbols symbol)
            :base(TokenType.Symbol, location)
        {
            this.symbol = symbol;
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        /// <value>The symbol.</value>
        public Symbols Symbol
        {get { return this.symbol; }}
    }
}
