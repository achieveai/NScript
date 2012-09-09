//-----------------------------------------------------------------------
// <copyright file="IHtmlNodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for IHtmlNodeGenerator
    /// </summary>
    public interface IHtmlNodeGenerator
    {
        HtmlNode GeneratedNode
        { get; }
    }
}
