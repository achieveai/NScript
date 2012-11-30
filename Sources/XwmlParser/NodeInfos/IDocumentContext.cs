//-----------------------------------------------------------------------
// <copyright file="IDocumentContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

	/// <summary>
	/// Definition for IDocumentContext
	/// </summary>
	public interface IDocumentContext
	{
        ParserContext ParserContext
        { get; }

        IResolver Resolver
        { get; }

        void ProcessNode(HtmlNode node, NodeInfo parentNode, Action<HtmlNode, NodeInfo, NodeType> callback);

        Tuple<string, string> GetFullName(string name);
	}
}
