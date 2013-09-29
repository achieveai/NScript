//-----------------------------------------------------------------------
// <copyright file="IDocumentContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
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

        Tuple<string, string> GetFullName(string name);
    }
}
