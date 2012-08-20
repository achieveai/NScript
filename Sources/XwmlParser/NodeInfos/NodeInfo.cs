//-----------------------------------------------------------------------
// <copyright file="NodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for NodeInfo
    /// </summary>
    public abstract class NodeInfo
    {
        /// <summary>
        /// The node.
        /// </summary>
        private HtmlNode node;

        /// <summary>
        /// Information describing the tag.
        /// </summary>
        private Tuple<string, string> tagInfo;

        /// <summary>
        /// Specialised constructor for use only by derived classes.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        protected NodeInfo(HtmlNode node, Tuple<string, string> tagInfo)
        {
            this.node = node;
            this.tagInfo = tagInfo;
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public HtmlNode Node
        { get { return this.node; } }

        /// <summary>
        /// Gets the information describing the tag.
        /// </summary>
        /// <value>
        /// Information describing the tag.
        /// </value>
        public Tuple<string, string> TagInfo
        { get { return this.tagInfo; } }
    }
}
