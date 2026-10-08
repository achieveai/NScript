//-----------------------------------------------------------------------
// <copyright file="JavaScript.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for JavaScript
    /// </summary>
    partial class JavaScriptLexer
    {
        // The grammar matches SkipSpace tokens itself, so "hidden" tokens stay on the default channel.
        const int HIDDEN = 0;

        /// <summary>
        /// Returns comments as SkipSpace. No grammar rule accepts a comment token, so a comment
        /// anywhere in a [Script] body failed the parse; whitespace is accepted wherever a comment
        /// may appear.
        /// </summary>
        public override Antlr.Runtime.IToken NextToken()
        {
            var token = base.NextToken();
            if (token.Type == Comment || token.Type == LineComment)
            {
                token.Type = SkipSpace;
            }

            return token;
        }
    }
}
