//-----------------------------------------------------------------------
// <copyright file="ParserContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ParserContext
    /// </summary>
    public class ParserContext
    {
        IResolver resolver;
        List<Tuple<string, string>> abbrToNamespaceMapping = new List<Tuple<string, string>>();

        public ParserContext(
            IResolver resolver)
        {
        }

        public IResolver Resolver
        { get { return this.resolver; } }
    }
}