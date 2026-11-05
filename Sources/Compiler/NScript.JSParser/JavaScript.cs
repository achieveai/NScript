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

            if (token.Type != SkipSpace)
            {
                this.lastSignificantToken = token;
                this.lastClosedImport = false;
                if (token.Type == IMPORT_START)
                {
                    this.openImports++;
                }
                else if (this.openImports > 0 && token.Text == "}")
                {
                    this.openImports--;
                    this.lastClosedImport = true;
                }
            }

            return token;
        }

        /// <summary>
        /// Count of '@{' imports whose closing '}' has not been seen yet.
        /// </summary>
        private int openImports;

        /// <summary>
        /// Whether the last significant token was the '}' that closes an '@{...}' import,
        /// which is a value (so a following '/' is division), unlike a block's '}'.
        /// </summary>
        private bool lastClosedImport;

        /// <summary>
        /// The last token that was not whitespace or a comment.
        /// </summary>
        private Antlr.Runtime.IToken lastSignificantToken;

        /// <summary>
        /// Whether a '/' here starts a regex literal. After a value (a name, literal, ')' or ']'),
        /// '/' is division; everywhere else (start, after an operator, '(', ',', 'return') it is a regex.
        /// </summary>
        protected bool RegexAllowed
        {
            get
            {
                if (this.lastSignificantToken == null)
                {
                    return true;
                }

                if (this.lastClosedImport)
                {
                    return false;
                }

                switch (this.lastSignificantToken.Type)
                {
                    case Identifier:
                    case NumericLiteral:
                    case StringLiteral:
                    case BoolLiteral:
                    case RegularExpressionLiteral:
                    case NULL:
                    case THIS:
                    case PLUS_INC:
                    case MINUS_INC:
                        return false;
                }

                string text = this.lastSignificantToken.Text;
                return text != ")" && text != "]";
            }
        }
    }
}
