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

        IClrResolver Resolver
        { get; }

        string GetCssString(List<string> usedCssClasses);

        Tuple<string, string> GetFullName(string name);
    }
}
