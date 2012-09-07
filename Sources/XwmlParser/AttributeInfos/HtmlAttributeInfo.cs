//-----------------------------------------------------------------------
// <copyright file="HtmlAttributeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.AttributeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for HtmlAttributeInfo
    /// </summary>
    public class HtmlAttributeInfo : AttributeInfo
    {
        /// <summary>
        /// The generated attribute.
        /// </summary>
        HtmlAttribute generatedAttribute;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attribute"> The attribute. </param>
        public HtmlAttributeInfo(
            HtmlAttribute attribute)
            : base(attribute)
        {
        }

        public HtmlAttribute GeneratedAttribute
        {
            get { return this.generatedAttribute; }
            set
            {
                if (this.generatedAttribute != null)
                {
                    throw new InvalidOperationException("Can't add generated attribute more than once");
                }

                this.generatedAttribute = value;
            }
        }
    }
}
