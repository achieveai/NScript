//-----------------------------------------------------------------------
// <copyright file="CssNumberPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssNumberPropertyValue
    /// </summary>
    public class CssNumberPropertyValue : CssPropertyValue
    {
        public CssNumberPropertyValue(double value)
        {
            this.Value = value;
        }

        public double Value { get; private set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
