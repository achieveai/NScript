//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;
    using HtmlAgilityPack;

    public enum NodeType
    {
        ExtensibleObject,
        Property,
        Html,
        Text,
        Template,
        Factory,
        Style
    }

    /// <summary>
    /// Definition for Parser
    /// </summary>
    public class Parser
    {
        HtmlNode node;

        public HtmlNode ParseNode(
            HtmlNode node)
        {
            return null;
        }

        public NodeType GetNodeType(HtmlNode node)
        {
            return NodeType.Html;
        }
    }
}