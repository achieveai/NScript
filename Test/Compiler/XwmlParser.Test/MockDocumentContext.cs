//-----------------------------------------------------------------------
// <copyright file="MockDocumentContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MockDocumentContext
    /// </summary>
    public class MockDocumentContext : IDocumentContext
    {
        TypeResolver resolver;
        ParserContext parserContext;
        Dictionary<string, string> xmlNsMapping = new Dictionary<string, string>();
        public MockDocumentContext(
            ParserContext parserContext,
            TypeResolver resolver)
        {
            this.parserContext = parserContext;
            this.resolver = resolver;
        }

        public void AddNsMapping(string abbr, string full)
        {
            this.xmlNsMapping.Add(abbr, full);
        }

        public ParserContext ParserContext
        { get { return this.parserContext; } }

        public IClrResolver Resolver
        { get { return this.resolver; } }

        public Tuple<string, string> GetFullName(string name)
        {
            string[] parts = name.Split(':');
            if (parts.Length == 2)
            {
                return Tuple.Create(this.xmlNsMapping[parts[0]], parts[1]);
            }
            else
            {
                return Tuple.Create((string)null, name);
            }
        }

        public string GetCssString(List<string> usedCssClasses)
        {
            throw new NotImplementedException();
        }
    }
}
