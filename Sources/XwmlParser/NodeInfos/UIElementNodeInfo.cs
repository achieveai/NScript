//-----------------------------------------------------------------------
// <copyright file="UIElementNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for UIElementNodeInfo
    /// </summary>
    public class UIElementNodeInfo : ContextBindableNodeInfo, IHtmlNodeGenerator
    {
        /// <summary>
        /// The generated node.
        /// </summary>
        private HtmlNode generatedNode;

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

        /// <summary>
        /// Gets or sets the generated node.
        /// </summary>
        /// <value>
        /// The generated node.
        /// </value>
        public HtmlNode GeneratedNode
        {
            get { return this.generatedNode; }
        }
        public IIdentifier[] ClassNames
        { get; set; }

        /// <summary>
        /// Sets new node and path.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="nodePath"> Full pathname of the node file. </param>
        public void SetNewNodeAndPath(
            HtmlNode node)
        {
            this.generatedNode = node;
        }

        public void FinalizeGeneratedNode(SkinCodeGenerator codeGenerator)
        {
            throw new NotImplementedException();
        }
    }
}
