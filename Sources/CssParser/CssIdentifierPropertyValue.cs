//-----------------------------------------------------------------------
// <copyright file="CssIdentifierPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssIdentifierPropertyValue
    /// </summary>
    public class CssIdentifierPropertyValue : CssPropertyValue
    {
        public CssIdentifierPropertyValue(string identifier)
        {
            this.Identifier = identifier;
        }

        public string Identifier { get; private set; }

        public override string ToString()
        {

            return this.Identifier;
        }
    }
}
