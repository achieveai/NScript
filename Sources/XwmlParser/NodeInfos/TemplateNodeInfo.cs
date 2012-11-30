//-----------------------------------------------------------------------
// <copyright file="TemplateNodeInfo.cs" company="">
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
    /// Definition for TemplateNodeInfo
    /// </summary>
    public class TemplateNodeInfo : NodeInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controlType">  Type of the control. </param>
        /// <param name="dataType">     Type of the data. </param>
        /// <param name="node">         The node. </param>
        /// <param name="tagInfo">      Information describing the tag. </param>
        public TemplateNodeInfo(
            TypeReference controlType,
            TypeReference dataType,
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(node, tagInfo)
        {
            this.ControlType = controlType;
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>
        /// The type of the control.
        /// </value>
        public TypeReference ControlType
        { get; private set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public TypeReference DataType
        { get; private set; }

        public void ParseNode(TemplateParser parser)
        {
        }
    }
}
