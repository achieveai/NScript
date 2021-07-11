//-----------------------------------------------------------------------
// <copyright file="CssUnitPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssUnitPropertyValue
    /// </summary>
    public class CssUnitPropertyValue : CssPropertyValue
    {
        public CssUnitPropertyValue(
            double value,
            string unit)
        {
            this.Value = value;
            this.Unit = unit;
        }

        public string Unit { get; private set; }

        public double Value { get; private set; }

        public override string ToString()
        {
            return this.Value.ToString() + this.Unit;
        }
    }
}
