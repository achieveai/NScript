using System;

namespace NScript.JST
{
    using System.Collections.Generic;
    using System.IO;
    using NScript.JST.Writer;
    using System.Text;
    using NScript.Utils;

    /// <summary>
    /// JSWriter is used to write javascript file.
    /// </summary>
    public class JSWriter
    {
        /// <summary>
        /// Backing field for tokens.
        /// </summary>
        private readonly LinkedList<TokenBase> tokens = new LinkedList<TokenBase>();

        /// <summary>
        /// Backing field for isOptimized.
        /// </summary>
        private readonly bool isOptimized;

        /// <summary>
        /// Stack for all the locations on the stack.
        /// </summary>
        private readonly Stack<Location> locationStack = new Stack<Location>();

        /// <summary>
        /// tracking field for scope depth.
        /// </summary>
        private int scopeDepth = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSWriter"/> class.
        /// </summary>
        /// <param name="isIndented">if set to <c>true</c> [is indented].</param>
        /// <param name="isOptimized">if set to <c>true</c> [is optimized].</param>
        public JSWriter(
            bool isIndented,
            bool isOptimized)
        {
            this.isOptimized = isOptimized;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is optimized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is optimized; otherwise, <c>false</c>.
        /// </value>
        public bool IsOptimized
        {
            get
            {
                return this.isOptimized;
            }
        }

        /// <summary>
        /// Writes the new line.
        /// </summary>
        public JSWriter WriteNewLine()
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new NewlineToken(
                        this.GetTopLocation())));

            return this;
        }

        /// <summary>
        /// Enters the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Self</returns>
        public JSWriter EnterLocation(Location location)
        {
            this.locationStack.Push(location);
            return this;
        }

        /// <summary>
        /// Leaves the location.
        /// </summary>
        public void LeaveLocation()
        {
            this.locationStack.Pop();
        }

        /// <summary>
        /// Enters the scope.
        /// </summary>
        /// <returns>Self</returns>
        public JSWriter EnterScope()
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new ScopeToken(++this.scopeDepth, false)));

            return this;
        }

        /// <summary>
        /// Exits the scope.
        /// </summary>
        /// <returns>Self</returns>
        public JSWriter ExitScope()
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new ScopeToken(--this.scopeDepth, true)));

            return this;
        }

        /// <summary>
        /// Writes the arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>Self</returns>
        public JSWriter WriteArguments(IList<Expression> arguments)
        {
            this.Write(Symbols.BracketOpenRound);
            for (int argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
            {
                if (argumentIndex > 0)
                {
                    this.Write(Symbols.Comma);
                }

                if (arguments[argumentIndex].Precedence <= Precedence.Comma)
                {
                    this.Write(Symbols.BracketOpenRound);
                }

                arguments[argumentIndex].Write(this);
                if (arguments[argumentIndex].Precedence <= Precedence.Comma)
                {
                    this.Write(Symbols.BracketCloseRound);
                }
            }

            this.Write(Symbols.BracketCloseRound);

            return this;
        }

        /// <summary>
        /// Writes the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>Self</returns>
        public JSWriter Write(Symbols symbol)
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new SymbolToken(
                        this.GetTopLocation(),
                        symbol)));

            return this;
        }

        /// <summary>
        /// Writers the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>Self</returns>
        public JSWriter Write(Keyword keyword)
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new KeywordToken(
                        this.GetTopLocation(),
                        keyword)));

            return this;
        }

        /// <summary>
        /// Writes the STR.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Self</returns>
        public JSWriter Write(double number)
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new GenericStrToken(
                        this.GetTopLocation(),
                        number.ToString(),
                        TokenType.NumToken)));

            return this;
        }

        /// <summary>
        /// Writes the STR.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Self</returns>
        public JSWriter Write(long number)
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new GenericStrToken(
                        this.GetTopLocation(),
                        number.ToString(),
                        TokenType.NumToken)));

            return this;
        }

        /// <summary>
        /// Writes the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Self</returns>
        public JSWriter Write(IIdentifier identifier)
        {
            SimpleIdentifier simpleIdentifier = identifier as SimpleIdentifier;
            if (simpleIdentifier != null)
            {
                this.tokens.AddLast(
                    new LinkedListNode<TokenBase>(
                        new GenericStrToken(
                            this.GetTopLocation(),
                            identifier.GetName(),
                            TokenType.IdentifierToken)));
            }
            else
            {
                CompoundIdentifier compoundIdentifier = (CompoundIdentifier)identifier;
                this.Write(compoundIdentifier.Identifiers[0]);
                for (int iIdentifier = 1; iIdentifier < compoundIdentifier.Identifiers.Count; iIdentifier++)
                {
                    this.tokens.AddLast(
                        new LinkedListNode<TokenBase>(
                            new SymbolToken(this.GetTopLocation(), Symbols.Dot)));

                    this.Write(compoundIdentifier.Identifiers[iIdentifier]);
                }
            }

            return this;
        }

        /// <summary>
        /// Writes the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Self</returns>
        public JSWriter WriteIdentifier(string identifier)
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new GenericStrToken(
                        this.GetTopLocation(),
                        identifier,
                        TokenType.IdentifierToken)));

            return this;
        }

        /// <summary>
        /// Writes the STR.
        /// </summary>
        /// <param name="strToken">The STR token.</param>
        /// <returns>Self</returns>
        public JSWriter WriteStr(string strToken)
        {
            this.tokens.AddLast(
                new LinkedListNode<TokenBase>(
                    new GenericStrToken(
                        this.GetTopLocation(),
                        strToken,
                        TokenType.StrToken)));

            return this;
        }

        /// <summary>
        /// Writes the specified string.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="writeInParentheses">if set to <c>true</c> write in parentheses.</param>
        /// <returns>Self</returns>
        public JSWriter Write(
            JST.Node node,
            bool writeInParentheses)
        {
            if (writeInParentheses)
            {
                this.Write(Symbols.BracketOpenRound)
                    .Write(node)
                    .Write(Symbols.BracketCloseRound);
            }
            else
            {
                node.Write(this);
            }

            return this;
        }

        /// <summary>
        /// Writes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Self</returns>
        public JSWriter Write(JST.Node node)
        {
            if (node != null)
            {
                if (node is JST.Expression
                    && node.Location != null)
                {
                    this.EnterLocation(node.Location);
                    node.Write(this);
                    this.LeaveLocation();
                }
                else
                {
                    node.Write(this);
                }
            }

            return this;
        }

        /// <summary>
        /// Writes script to jsFileName and map file to jsFileName.map with given sourceRoot.
        /// </summary>
        /// <param name="jsFileName"> Filename of the js file. </param>
        /// <param name="sourceRoot"> Source root. </param>
        public void Write(string jsFileName, string sourceRoot)
        {
            using (StreamWriter streamWriter = new StreamWriter(jsFileName, false, System.Text.Encoding.UTF8))
            {
                this.Write(streamWriter, Path.GetFileName(jsFileName), sourceRoot, jsFileName + ".map");
            }
        }

        /// <summary>
        /// Writes to the given TextWriter.
        /// </summary>
        /// <param name="writer"> The TextWriter to write. </param>
        public void Write(TextWriter writer)
        {
            this.Write(writer, null, null, null);
        }

        /// <summary>
        /// Writes javascript to given writer with mapping file generated using jsFileName, sourceRoot
        /// and mapFileName.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
        ///     the required range. </exception>
        /// <param name="writer">      The writer. </param>
        /// <param name="jsFileName">  Filename of the js file. </param>
        /// <param name="sourceRoot">  Source root. </param>
        /// <param name="mapFileName"> Filename of the map file. </param>
        private void Write(TextWriter writer, string jsFileName, string sourceRoot, string mapFileName)
        {
            this.ArrangeSpaces();

            int scopeDepth = 0;
            Location lastLocation = null;
            int curLine = 0;
            int curCol = 0;
            var sourceMapping = new OwaSourceMapper.SourceMap();
            if (jsFileName != null)
            {
                sourceMapping.File = jsFileName;
                sourceMapping.SourceRoot = sourceRoot.Replace("\\", "\\\\");
            }

            sourceMapping.AddMapping(
                curLine,
                0,
                curLine,
                0,
                jsFileName);

            if (jsFileName != null)
            {
                writer.Write("(function(){");
            }

            foreach (var token in this.tokens)
            {
                string str = string.Empty;

                if (token.Type != TokenType.Space
                    && token.Type != TokenType.Newline
                    && token.Location != lastLocation)
                {
                    if (lastLocation != null
                        && lastLocation.EndLine != int.MaxValue)
                    {
                        sourceMapping.AddMapping(
                            curLine,
                            curCol,
                            lastLocation.EndLine - 1,
                            lastLocation.EndColumn - 1,
                            lastLocation.FileName);
                    }

                    lastLocation = token.Location;

                    if (lastLocation == null
                        || lastLocation.StartLine < 0
                        || string.IsNullOrWhiteSpace(lastLocation.FileName))
                    {
                        sourceMapping.AddMapping(
                            curLine,
                            curCol,
                            curLine,
                            curCol,
                            jsFileName);
                    }
                    else
                    {
                        sourceMapping.AddMapping(
                            curLine,
                            curCol,
                            lastLocation.StartLine - 1,
                            lastLocation.StartColumn - 1,
                            lastLocation.FileName);
                    }
                }

                switch (token.Type)
                {
                    case TokenType.Keyword:
                        str = GetString(((KeywordToken) token).Keyword);
                        break;
                    case TokenType.Symbol:
                        str = GetString(((SymbolToken) token).Symbol);
                        break;
                    case TokenType.Space:
                        str = " ";
                        break;
                    case TokenType.Newline:
                        str = this.GetNewLineString(scopeDepth);
                        break;
                    case TokenType.StrToken:
                    case TokenType.NumToken:
                    case TokenType.IdentifierToken:
                        str = GetString((GenericStrToken) token);
                        break;
                    case TokenType.ScopeToken:
                        if (!this.IsOptimized)
                        {
                            ScopeToken scopeToken = (ScopeToken) token;

                            if (scopeToken.IsExit)
                            {
                                scopeDepth--;
                            }
                            else
                            {
                                scopeDepth++;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (token.Type == TokenType.Newline)
                {
                    curLine++;
                    curCol = str.Length - 2;
                    lastLocation = null;
                    sourceMapping.AddMapping(
                        curLine,
                        0,
                        curLine,
                        0,
                        jsFileName);
                }
                else if (!string.IsNullOrEmpty(str))
                {
                    curCol += str.Length;
                }

                if (!string.IsNullOrEmpty(str))
                {
                    writer.Write(str);
                }
            }

            if (mapFileName != null)
            {
                writer.WriteLine();
                writer.Write("//@ sourceMappingURL={0}", Path.GetFileName(mapFileName));
                sourceMapping.AddMapping(
                    ++curLine,
                    0,
                    curLine,
                    0,
                    jsFileName);
            }

            if (jsFileName != null)
            {
                writer.Write("\r\n})();");
                sourceMapping.AddMapping(
                    ++curLine,
                    0,
                    curLine,
                    0,
                    jsFileName);
            }

            if (mapFileName != null)
            {
                using (StreamWriter mapWriter = new StreamWriter(mapFileName, false, System.Text.Encoding.ASCII))
                    mapWriter.Write(sourceMapping.ToString());
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>stringified version of keyword</returns>
        private static string GetString(Keyword keyword)
        {
            switch (keyword)
            {
                case Keyword.Break:
                    return "break";
                case Keyword.Case:
                    return "case";
                case Keyword.Catch:
                    return "catch";
                case Keyword.Const:
                    return "const";
                case Keyword.Continue:
                    return "continue";
                case Keyword.Default:
                    return "default";
                case Keyword.Delete:
                    return "delete";
                case Keyword.Do:
                    return "do";
                case Keyword.Else:
                    return "else";
                case Keyword.Export:
                    return "export";
                case Keyword.False:
                    return "false";
                case Keyword.Finally:
                    return "finally";
                case Keyword.For:
                    return "for";
                case Keyword.Function:
                    return "function";
                case Keyword.If:
                    return "if";
                case Keyword.Import:
                    return "import";
                case Keyword.In:
                    return "in";
                case Keyword.InstanceOf:
                    return "instanceof";
                case Keyword.Label:
                    return "label";
                case Keyword.Let:
                    return "let";
                case Keyword.New:
                    return "new";
                case Keyword.Null:
                    return "null";
                case Keyword.Return:
                    return "return";
                case Keyword.Switch:
                    return "switch";
                case Keyword.This:
                    return "this";
                case Keyword.Throw:
                    return "throw";
                case Keyword.True:
                    return "true";
                case Keyword.Try:
                    return "try";
                case Keyword.TypeOf:
                    return "typeof";
                case Keyword.Var:
                    return "var";
                case Keyword.Void:
                    return "void";
                case Keyword.While:
                    return "while";
                case Keyword.With:
                    return "with";
                case Keyword.Yield:
                    return "yield";
                default:
                    throw new ArgumentOutOfRangeException("keyword");
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>Strigified version of symbol</returns>
        private static string GetString(Symbols symbol)
        {
            switch (symbol)
            {
                case Symbols.Assign:
                    return "=";
                case Symbols.And:
                    return "&";
                case Symbols.AndEquals:
                    return "&=";
                case Symbols.BrackedOpenCurly:
                    return "{";
                case Symbols.BracketCloseCurly:
                    return "}";
                case Symbols.BracketOpenRound:
                    return "(";
                case Symbols.BracketCloseRound:
                    return ")";
                case Symbols.BrackedOpenSquare:
                    return "[";
                case Symbols.BracketCloseSquare:
                    return "]";
                case Symbols.Colon:
                    return ":";
                case Symbols.CommentStart:
                    return "/*";
                case Symbols.CommentEnd:
                    return "*/";
                case Symbols.Comma:
                    return ",";
                case Symbols.Conditional:
                    return "?";
                case Symbols.ConditionalElse:
                    return ":";
                case Symbols.Divide:
                    return "/";
                case Symbols.DivideEquals:
                    return "/=";
                case Symbols.Dot:
                    return ".";
                case Symbols.Equals:
                    return "==";
                case Symbols.EqualsReally:
                    return "===";
                case Symbols.GreaterThan:
                    return ">";
                case Symbols.GreaterThanEquals:
                    return ">=";
                case Symbols.Inverse:
                    return "~";
                case Symbols.LessThan:
                    return "<";
                case Symbols.LessThanEquals:
                    return "<=";
                case Symbols.LogicalAnd:
                    return "&&";
                case Symbols.LogicalOr:
                    return "||";
                case Symbols.Modulus:
                    return "%";
                case Symbols.ModulusEquals:
                    return "%=";
                case Symbols.Minus:
                    return "-";
                case Symbols.MinusEquals:
                    return "-=";
                case Symbols.Multiply:
                    return "*";
                case Symbols.MultiplyEquals:
                    return "*=";
                case Symbols.Not:
                    return "!";
                case Symbols.NotEquals:
                    return "!=";
                case Symbols.NotEqualsReally:
                    return "!==";
                case Symbols.Or:
                    return "|";
                case Symbols.OrEquals:
                    return "|=";
                case Symbols.Plus:
                    return "+";
                case Symbols.PlusEquals:
                    return "+=";
                case Symbols.PostDecrement:
                    return "--";
                case Symbols.PostIncrement:
                    return "++";
                case Symbols.PreDecrement:
                    return "--";
                case Symbols.PreIncrement:
                    return "++";
                case Symbols.SemiColon:
                    return ";";
                case Symbols.ShiftRight:
                    return ">>";
                case Symbols.ShiftRightEquals:
                    return ">>=";
                case Symbols.ShiftLeft:
                    return "<<";
                case Symbols.ShiftLeftEquals:
                    return "<<=";
                case Symbols.SingleLineComment:
                    return "//";
                case Symbols.UnaryMinus:
                    return "-";
                case Symbols.UnsignedShiftRight:
                    return ">>>";
                case Symbols.UnsignedShiftRightEquals:
                    return ">>>=";
                case Symbols.Xor:
                    return "^";
                case Symbols.XorEquals:
                    return "^=";
                default:
                    throw new ArgumentOutOfRangeException("symbol");
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>string for token</returns>
        private static string GetString(GenericStrToken token)
        {
            switch (token.Type)
            {
                case TokenType.StrToken:
                    return string.Format("\"{0}\"", Utils.ToJSString(token.String));
                case TokenType.NumToken:
                case TokenType.IdentifierToken:
                    return token.String;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the top location.
        /// </summary>
        /// <returns>Location from top of stack, or null if stack is empty</returns>
        private Location GetTopLocation()
        {
            if (this.locationStack.Count > 0)
            {
                return this.locationStack.Peek();
            }

            return null;
        }

        /// <summary>
        /// Arranges the spaces.
        /// </summary>
        private void ArrangeSpaces()
        {
            LinkedListNode<TokenBase> token = this.tokens.First;

            while (token != null)
            {
                switch (token.Value.Type)
                {
                    case TokenType.Keyword:
                        this.ArrangeKeywordSpaces(token);
                        break;
                    case TokenType.Symbol:
                        this.ArrangeSymbolSpaces(token);
                        break;
                    case TokenType.Space:
                        break;
                    case TokenType.Newline:
                        break;
                    case TokenType.StrToken:
                    case TokenType.NumToken:
                    case TokenType.IdentifierToken:
                        this.ArrangeGenericTokenSpaces(token);
                        break;
                }

                token = token.Next;
            }
        }

        /// <summary>
        /// Arranges the keyword spaces.
        /// </summary>
        /// <param name="node">The node.</param>
        private void ArrangeKeywordSpaces(
            LinkedListNode<TokenBase> node)
        {
            if (this.IsOptimized)
            {
                var prevToken = this.GetNonOptimizableTokenBefore(node);

                if (prevToken != null &&
                    prevToken.Type != TokenType.Symbol &&
                    prevToken.Type != TokenType.Newline &&
                    prevToken.Type != TokenType.Space)
                {
                    this.InsertSpace(node, false);
                }
            }
            else
            {
                var prevToken = this.GetNonOptimizableTokenBefore(node);

                if (prevToken != null &&
                    prevToken.Type != TokenType.Newline &&
                    prevToken.Type != TokenType.Space &&
                    prevToken.Type != TokenType.Symbol)
                {
                    this.InsertSpace(node, true);
                }

                var nextToken = this.GetNonOptimizableTokenAfter(node);
                if (nextToken == null)
                {
                    return;
                }

                KeywordToken keywordToken = (KeywordToken) node.Value;

                switch (keywordToken.Keyword)
                {
                    case Keyword.Break:
                    case Keyword.Continue:
                    case Keyword.False:
                    case Keyword.Null:
                    case Keyword.This:
                    case Keyword.True:
                    case Keyword.Void:
                        return;
                    case Keyword.Return:
                        if (nextToken.Type == TokenType.Symbol &&
                            ((SymbolToken)nextToken).Symbol != Symbols.SemiColon)
                        {
                            this.InsertSpace(node, false);
                        }
                        break;
                    case Keyword.TypeOf:
                        if (nextToken.Type == TokenType.Symbol &&
                            ((SymbolToken)nextToken).Symbol == Symbols.BracketOpenRound)
                        {
                            return;
                        }
                        break;
                    case Keyword.If:
                    case Keyword.While:
                    case Keyword.Do:
                    case Keyword.For:
                    case Keyword.Catch:
                        this.InsertSpace(node, false);
                        break;
                }

                if (nextToken.Type != TokenType.Space &&
                    nextToken.Type != TokenType.Newline &&
                    nextToken.Type != TokenType.Symbol)
                {
                    this.InsertSpace(node, false);
                }
            }
        }

        /// <summary>
        /// Arranges the symbol spaces.
        /// </summary>
        /// <param name="node">The node.</param>
        private void ArrangeSymbolSpaces(
            LinkedListNode<TokenBase> node)
        {
            SymbolToken symbolToken = (SymbolToken)node.Value;

            if (!this.HasSpaceBefore(node))
            {
                if (symbolToken.Symbol == Symbols.PreDecrement ||
                    symbolToken.Symbol == Symbols.UnaryMinus ||
                    symbolToken.Symbol == Symbols.PreIncrement)
                {
                    this.InsertSpace(node, true);
                }
            }

            if (!this.HasSpaceAfter(node))
            {
                if (symbolToken.Symbol == Symbols.PreDecrement ||
                    symbolToken.Symbol == Symbols.PreIncrement)
                {
                    // this.InsertSpace(node, false);
                }
            }

            if (!this.isOptimized)
            {
                switch (symbolToken.Symbol)
                {
                    case Symbols.Assign:
                    case Symbols.And:
                    case Symbols.AndEquals:
                    case Symbols.Conditional:
                    case Symbols.ConditionalElse:
                    case Symbols.Divide:
                    case Symbols.DivideEquals:
                    case Symbols.Equals:
                    case Symbols.EqualsReally:
                    case Symbols.GreaterThan:
                    case Symbols.GreaterThanEquals:
                    case Symbols.LessThan:
                    case Symbols.LessThanEquals:
                    case Symbols.LogicalAnd:
                    case Symbols.LogicalOr:
                    case Symbols.Modulus:
                    case Symbols.ModulusEquals:
                    case Symbols.Minus:
                    case Symbols.MinusEquals:
                    case Symbols.Multiply:
                    case Symbols.MultiplyEquals:
                    case Symbols.NotEquals:
                    case Symbols.NotEqualsReally:
                    case Symbols.Or:
                    case Symbols.OrEquals:
                    case Symbols.Plus:
                    case Symbols.PlusEquals:
                    case Symbols.ShiftRight:
                    case Symbols.ShiftRightEquals:
                    case Symbols.ShiftLeft:
                    case Symbols.ShiftLeftEquals:
                    case Symbols.UnsignedShiftRight:
                    case Symbols.UnsignedShiftRightEquals:
                    case Symbols.Xor:
                    case Symbols.XorEquals:
                        if (!this.HasSpaceBefore(node))
                        {
                            this.InsertSpace(node, true);
                        }

                        if (!this.HasSpaceAfter(node))
                        {
                            this.InsertSpace(node, false);
                        }

                        break;
                    case Symbols.SemiColon:
                    case Symbols.Colon:
                        {
                            var token = this.GetNonOptimizableTokenAfter(node);

                            if (token != null &&
                                token.Type != TokenType.Space &&
                                token.Type != TokenType.Newline &&
                                !(token.Type == TokenType.Symbol && ((SymbolToken)token).Symbol == Symbols.BracketCloseCurly))
                            {
                                this.InsertSpace(node, false);
                            }
                        }

                        break;

                    case Symbols.BrackedOpenCurly:
                        if (!this.HasSpaceBefore(node))
                        {
                            this.InsertSpace(node, true);
                        }

                        break;
                    case Symbols.BracketCloseCurly:
                        this.tokens.AddBefore(
                            node,
                            new LinkedListNode<TokenBase>(
                                new NewlineToken(null)));

                        {
                            var token = this.GetNonOptimizableTokenAfter(node);

                            if (token != null &&
                                token.Type != TokenType.Space &&
                                token.Type != TokenType.Newline &&
                                token.Type != TokenType.Symbol)
                            {
                                this.InsertSpace(node, false);
                            }
                        }

                        break;
                    case Symbols.BracketOpenRound:
                    case Symbols.BracketCloseRound:
                    case Symbols.BrackedOpenSquare:
                    case Symbols.BracketCloseSquare:
                        break;
                    case Symbols.Comma:
                        if (!this.HasSpaceAfter(node))
                        {
                            this.tokens.AddAfter(
                                node,
                                new LinkedListNode<TokenBase>(
                                    new SpaceToken(null)));
                        }

                        break;
                    case Symbols.Inverse:
                    case Symbols.Not:
                        break;
                }
            }
        }

        /// <summary>
        /// Arranges the generic token spaces.
        /// </summary>
        /// <param name="node">The node.</param>
        private void ArrangeGenericTokenSpaces(
            LinkedListNode<TokenBase> node)
        {
            if (this.IsOptimized)
            {
                var prevToken = this.GetNonOptimizableTokenBefore(node);

                if (prevToken != null &&
                    prevToken.Type != TokenType.Symbol &&
                    prevToken.Type != TokenType.Newline &&
                    prevToken.Type != TokenType.Space)
                {
                    this.InsertSpace(node, false);
                }
            }
            else
            {
                var prevToken = this.GetNonOptimizableTokenBefore(node);

                if (prevToken != null &&
                    prevToken.Type != TokenType.Newline &&
                    prevToken.Type != TokenType.Space &&
                    prevToken.Type != TokenType.Symbol)
                {
                    this.InsertSpace(node, false);
                }

                var nextToken = this.GetNonOptimizableTokenAfter(node);

                if (nextToken != null &&
                    nextToken.Type != TokenType.Space &&
                    nextToken.Type != TokenType.Newline &&
                    nextToken.Type != TokenType.Symbol)
                {
                    this.InsertSpace(node, false);
                }
            }
        }

        /// <summary>
        /// Inserts the space.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="before">if set to <c>true</c> [before].</param>
        /// <param name="spaceCount">The space count.</param>
        private void InsertSpace(LinkedListNode<TokenBase> node, bool before, int spaceCount = 1)
        {
            if (before)
            {
                var newNode =
                    new LinkedListNode<TokenBase>(
                        new SpaceToken(node.Previous != null ? node.Previous.Value.Location : null) {SpaceCount = spaceCount});

                this.tokens.AddBefore(
                    node,
                    newNode);
            }
            else
            {
                var newNode =
                    new LinkedListNode<TokenBase>(
                        new SpaceToken(node.Value.Location) {SpaceCount = spaceCount});

                this.tokens.AddAfter(
                    node,
                    newNode);
            }
        }

        /// <summary>
        /// Determines whether tokesn have space before the specified node].
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// <c>true</c> if tokens list has space before the specified node; otherwise, <c>false</c>.
        /// </returns>
        private bool HasSpaceBefore(LinkedListNode<TokenBase> node)
        {
            TokenBase prevT = this.GetNonOptimizableTokenBefore(node);

            return prevT == null || prevT.Type == TokenType.Space || prevT.Type == TokenType.Newline;
        }

        /// <summary>
        /// Determines whether tokesn have space after the specified node].
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// <c>true</c> if tokens list has space after the specified node; otherwise, <c>false</c>.
        /// </returns>
        private bool HasSpaceAfter(LinkedListNode<TokenBase> node)
        {
            TokenBase nextT = this.GetNonOptimizableTokenAfter(node);

            return nextT == null || nextT.Type == TokenType.Space || nextT.Type == TokenType.Newline;
        }

        /// <summary>
        /// Gets the non optimizable token before.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Token</returns>
        private TokenBase GetNonOptimizableTokenBefore(LinkedListNode<TokenBase> node)
        {
            while (node.Previous != null)
            {
                node = node.Previous;

                switch (node.Value.Type)
                {
                    case TokenType.Keyword:
                    case TokenType.Symbol:
                    case TokenType.StrToken:
                    case TokenType.NumToken:
                    case TokenType.IdentifierToken:
                    case TokenType.Space:
                        return node.Value;
                    case TokenType.Newline:
                        if (!this.isOptimized)
                        {
                            return node.Value;
                        }
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the non optimizable token after.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Token</returns>
        private TokenBase GetNonOptimizableTokenAfter(LinkedListNode<TokenBase> node)
        {
            while (node.Next != null)
            {
                node = node.Next;

                switch (node.Value.Type)
                {
                    case TokenType.Keyword:
                    case TokenType.Symbol:
                    case TokenType.StrToken:
                    case TokenType.NumToken:
                    case TokenType.IdentifierToken:
                    case TokenType.Space:
                        return node.Value;
                    case TokenType.Newline:
                        if (!this.isOptimized)
                        {
                            return node.Value;
                        }
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the new line string.
        /// </summary>
        /// <param name="scopeDepth">The scope depth.</param>
        /// <returns>stringified version of the line.</returns>
        private string GetNewLineString(int scopeDepth)
        {
            if (!this.isOptimized)
            {
                StringBuilder strBuilder = new StringBuilder();

                strBuilder.AppendLine();
                scopeDepth *= 2;

                for (int i = 0; i < scopeDepth; i++)
                {
                    strBuilder.Append(' ');
                }

                return strBuilder.ToString();
            }

            return string.Empty;
        }
    }
}
