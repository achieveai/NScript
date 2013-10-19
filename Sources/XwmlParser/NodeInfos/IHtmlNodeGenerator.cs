//-----------------------------------------------------------------------
// <copyright file="IHtmlNodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for IHtmlNodeGenerator
    /// </summary>
    public interface IHtmlNodeGenerator
    {
        /// <summary>
        /// Gets the generated node.
        /// </summary>
        /// <value>
        /// The generated node.
        /// </value>
        HtmlNode GeneratedNode
        { get; }

        /// <summary>
        /// Sets new node and path.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="nodePath"> Full pathname of the node file. </param>
        void SetNewNodeAndPath(
            HtmlNode node);

        /// <summary>
        /// Finalize generated node.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        void FinalizeGeneratedNode(
            SkinCodeGenerator codeGenerator);
    }
}
