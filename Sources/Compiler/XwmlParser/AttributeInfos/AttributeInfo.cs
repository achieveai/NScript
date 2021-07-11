//-----------------------------------------------------------------------
// <copyright file="AttributeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.AttributeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for AttributeInfo
    /// </summary>
    public abstract class AttributeInfo
    {
        HtmlAttribute attribute;

        public AttributeInfo(
            HtmlAttribute attribute)
        {
            this.attribute = attribute;
        }
    }
}
