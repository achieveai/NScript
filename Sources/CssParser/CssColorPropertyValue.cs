//-----------------------------------------------------------------------
// <copyright file="CssColorPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssColorPropertyValue
    /// </summary>
    public class CssColorPropertyValue : CssPropertyValue
    {
        public CssColorPropertyValue(
            string hexValue)
        {
            this.HexValue = hexValue;
        }

        public string HexValue { get; private set; }

        public override string ToString()
        {
            return this.HexValue;
        }
    }
}
