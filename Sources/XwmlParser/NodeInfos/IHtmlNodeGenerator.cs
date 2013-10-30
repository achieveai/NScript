//-----------------------------------------------------------------------
// <copyright file="IHtmlNodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using NScript.JST;
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

        IIdentifier[] ClassNames
        { get; set; }

        /// <summary>
        /// Finalize generated node.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        void FinalizeGeneratedNode(
            SkinCodeGenerator codeGenerator);

        void MarkAsPart(bool isDomPart);
    }
}
