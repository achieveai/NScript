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
        /// Full pathname of the node file.
        /// </summary>
        List<int> nodePath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        public HtmlNodeInfo(
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(node, tagInfo)
        {
        }

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

        /// <summary>
        /// Gets the full pathname of the node file.
        /// </summary>
        /// <value>
        /// The full pathname of the node file.
        /// </value>
        public List<int> NodePath
        { get { return this.nodePath; } }

        /// <summary>
        /// Sets new node and path.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="nodePath"> Full pathname of the node file. </param>
        public void SetNewNodeAndPath(
            HtmlNode node,
            List<int> nodePath)
        {
            this.generatedNode = node;
            this.nodePath = nodePath;
        }

        public override void ParseNode(
            TemplateParser parser)
        {
            if (this.Node.NodeType == HtmlNodeType.Text)
            {
            }
            else if (this.Node.NodeType == HtmlNodeType.Element)
            {
                /*
                // TOOD: add id tracking and munging.
                HtmlNodeInfo htmlNodeInfo = new HtmlNodeInfo(
                    node,
                    this.context.GetFullName(node.OriginalName));
                HtmlNode rv = this.document.CreateElement(node.OriginalName);
                foreach (var attr in node.Attributes)
                {
                    rv.SetAttributeValue(attr.OriginalName, attr.Value);
                }

                foreach (var childNode in node.ChildNodes)
                {
                    var tmpNode = this.ParseNode(childNode, null);

                    IHtmlNodeGenerator generatorNode = tmpNode as IHtmlNodeGenerator;

                    if (generatorNode != null)
                    {
                        rv.AppendChild(generatorNode.GeneratedNode);
                    }
                }

                return new HtmlNodeInfo(
                    node,
                    this.context.GetFullName(node.OriginalName))
                    { GeneratedNode = rv };
                */
            }
        }
    }
}
