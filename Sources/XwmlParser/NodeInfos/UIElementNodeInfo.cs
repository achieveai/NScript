//-----------------------------------------------------------------------
// <copyright file="UIElementNodeInfo.cs" company="">
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
    /// Definition for UIElementNodeInfo
    /// </summary>
    public class UIElementNodeInfo : TypeNodeInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">     The type. </param>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        public UIElementNodeInfo(
            TypeReference type,
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(type, node, tagInfo)
        { }
    }
}
