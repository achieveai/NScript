//-----------------------------------------------------------------------
// <copyright file="SkinNodeInfo.cs" company="">
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
    /// Definition for SkinNodeInfo
    /// </summary>
    public class SkinNodeInfo : NodeInfo, IHtmlNodeGenerator
    {
        HtmlNode generatedNode;

        public SkinNodeInfo(
            TypeReference dataContextType,
            TypeReference controlType,
            HtmlNode node)
            : base(node, null)
        {
            this.DataContextType = dataContextType;
            this.ControlType = controlType;
        }

        /// <summary>
        /// Gets or sets the type of the data context.
        /// </summary>
        /// <value>
        /// The type of the data context.
        /// </value>
        public TypeReference DataContextType
        { get; private set; }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>
        /// The type of the control.
        /// </value>
        public TypeReference ControlType
        { get; private set; }

        public override void ParseNode(TemplateParser parser)
        {
            this.generatedNode = parser.GeneratedDocument.CreateElement("div");
        }

        public override bool ProcessChildNode(NodeInfo childNode)
        {
            IHtmlNodeGenerator nodeGenerator = childNode as IHtmlNodeGenerator;

            if (nodeGenerator == null)
            {
                return false;
            }

            this.AddChildNodeInfo(childNode);
            return true;
        }

        public HtmlAgilityPack.HtmlNode GeneratedNode
        {
            get { return this.generatedNode; }
        }

        public void SetNewNodeAndPath(HtmlAgilityPack.HtmlNode node)
        {
            throw new InvalidOperationException();
        }

        public void FinalizeGeneratedNode(SkinCodeGenerator codeGenerator)
        {
        }
    }
}
