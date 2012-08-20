//-----------------------------------------------------------------------
// <copyright file="TypeNodeInfo.cs" company="">
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
    /// Definition for TypeNodeInfo
    /// </summary>
    public class TypeNodeInfo : NodeInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">     The type. </param>
        /// <param name="node">     The node. </param>
        /// <param name="tagName">  Name of the tag. </param>
        public TypeNodeInfo(
            TypeReference type,
            HtmlNode node,
            Tuple<string, string> tagName)
            : base(node, tagName)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public TypeReference Type
        { get; private set; }
    }
}
