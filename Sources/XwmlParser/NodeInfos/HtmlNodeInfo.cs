//-----------------------------------------------------------------------
// <copyright file="HtmlNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for HtmlNodeInfo
    /// </summary>
    public class HtmlNodeInfo : NodeInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        public HtmlNodeInfo(
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(node, tagInfo)
        { }
    }
}
