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
    public class HtmlNodeInfo : NodeInfo, IHtmlNodeGenerator
    {
        /// <summary>
        /// The generated node.
        /// </summary>
        HtmlNode generatedNode;

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

        /// <summary>
        /// Gets or sets the generated node.
        /// </summary>
        /// <value>
        /// The generated node.
        /// </value>
        public HtmlNode GeneratedNode
        {
            get { return this.generatedNode; }

            set
            {
                if (this.generatedNode != null)
                {
                    throw new InvalidOperationException("Can't set generated node more than once");
                }

                this.generatedNode = value;
            }
        }
    }
}
