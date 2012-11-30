//-----------------------------------------------------------------------
// <copyright file="FactoryNodeInfo.cs" company="">
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
    /// Definition for FactoryNodeInfo
    /// </summary>
    public class FactoryNodeInfo : NodeInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controlType">  The type. </param>
        /// <param name="node">         The node. </param>
        /// <param name="tagInfo">      Information describing the tag. </param>
        public FactoryNodeInfo(
            TypeReference controlType,
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(node, tagInfo)
        {
            this.ControlType = controlType;
        }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>
        /// The type of the control.
        /// </value>
        public TypeReference ControlType
        { get; private set; }

        public void ParseNode(TemplateParser parser)
        {
        }
    }
}
