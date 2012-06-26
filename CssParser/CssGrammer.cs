//-----------------------------------------------------------------------
// <copyright file="CssGrammer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

    /// <summary>
    /// Definition for CssGrammer
    /// </summary>
    public partial class CssGrammer
    {
        public CssGrammer(string css)
        {
            ANTLRStringStream input = new ANTLRStringStream(css);
            CssGrammerLexer lexer = new CssGrammerLexer(input);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            CssGrammerParser parser = new CssGrammerParser(tokenStream);
            CommonTree tree = parser.stylesheet().Tree;
        }
    }
}