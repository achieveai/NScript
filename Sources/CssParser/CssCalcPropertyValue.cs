//-----------------------------------------------------------------------
// <copyright file="CssCalcPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Definition for CssCalcPropertyValue
    /// </summary>
    public class CssCalcPropertyValue : CssPropertyValue
    {
        private CssUnitPropertyValue[] propertyValues;
        private char[] operators;

        public CssCalcPropertyValue(
            CssUnitPropertyValue[] propertyValues,
            char[] operators)
        {
            this.propertyValues = propertyValues;
            this.operators = operators;
        }

        public CssUnitPropertyValue[] PropertyValues
        { get { return this.propertyValues; } }

        public char[] Operators
        { get { return this.operators; } }

        public override string ToString()
        {
            StringBuilder rv = new StringBuilder();

            rv.Append("calc(");
            rv.Append(this.propertyValues[0].ToString());

            for (int i = 0; i < this.operators.Length; i++)
            {
                rv.Append(' ')
                    .Append(this.operators[i])
                    .Append(' ')
                    .Append(this.propertyValues[i + 1].ToString());
            }

            rv.Append(')');
            return rv.ToString();
        }
    }
}
