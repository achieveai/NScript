//-----------------------------------------------------------------------
// <copyright file="SkinnableNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for SkinnableNodeInfo
    /// </summary>
    public class SkinnableNodeInfo : UIElementNodeInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">     The type. </param>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        public SkinnableNodeInfo(
            TypeReference type,
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(type, node, tagInfo)
        { }
    }
}
