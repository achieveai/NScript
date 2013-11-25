//-----------------------------------------------------------------------
// <copyright file="CssStringPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssStringPropertyValue
    /// </summary>
    public class CssStringPropertyValue : CssPropertyValue
    {
        public CssStringPropertyValue(
            string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
