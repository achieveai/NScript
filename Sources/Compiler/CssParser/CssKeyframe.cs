//-----------------------------------------------------------------------
// <copyright file="CssKeyframe.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Definition for CssKeyframe
    /// </summary>
    public class CssKeyframe
    {
        public CssKeyframe(
            List<string> selectors,
            List<CssProperty> properties)
        {
            this.Selectors = selectors;
            this.Properties = properties;
        }

        public List<string> Selectors { get; private set; }

        public List<CssProperty> Properties { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int iSelector = 0; iSelector < this.Selectors.Count; iSelector++)
            {
                if (iSelector > 0)
                {
                    sb.Append(',');
                }

                sb.Append(this.Selectors[iSelector]);
            }

            sb.Append('{');
            for (int iProperty = 0; iProperty < this.Properties.Count; iProperty++)
            {
                if (iProperty > 0)
                {
                    sb.Append(';');
                }

                sb.Append(this.Properties[iProperty]);
            }

            sb.Append('}');

            return sb.ToString();
        }
    }
}
