//-----------------------------------------------------------------------
// <copyright file="CssMultiplePropertyValues.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Definition for CssMultiplePropertyValues
    /// </summary>
    public class CssPropertyValueSet: CssPropertyValue
    {
        public CssPropertyValueSet(IList<CssPropertyValue> values)
        {
            this.Values = values;
        }

        public IList<CssPropertyValue> Values { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int iValue = 0; iValue < this.Values.Count; iValue++)
            {
                if (iValue > 0)
                {
                    sb.Append(',');
                }

                sb.Append(this.Values[iValue].ToString());
            }

            return sb.ToString();
        }
    }
}
