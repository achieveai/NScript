using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NScript.JST;

namespace NScript.JSParser
{
    public class Parser
    {
        public readonly static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Parses the specified js.
        /// </summary>
        /// <param name="js">The js.</param>
        /// <returns></returns>
        public static JST.ScopeBlock Parse(
            string js,
            IdentifierScope parentScope,
            IResolver resolver)
        {
            stopWatch.Start();
            ANTLRStringStream input = new ANTLRStringStream(js);
            JavaScriptLexer lexer = new JavaScriptLexer(input);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            JavaScriptParser parser = new JavaScriptParser(tokenStream);
            CommonTree tree = parser.program().Tree;

            var rv = Parser.WalkTree(tree, parentScope, resolver);
            stopWatch.Stop();
            return rv;
        }

        /// <summary>
        /// Walks the tree.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <returns>Walk the tree</returns>
        private static JST.ScopeBlock WalkTree(
            CommonTree tree,
            IdentifierScope parentScope,
            IResolver resolver)
        {
            List<JST.Statement> statements = new List<Statement>();
            ScopeResolver identifierResolver = new ScopeResolver(
                parentScope,
                resolver);

            foreach (CommonTree treeNode in tree.Children)
            {
                JST.Statement statement = Parser.ParseStatementNode(
                    treeNode,
                    identifierResolver);

                if (statement != null)
                {
                    statements.Add(statement);
                }
            }

            return new JST.ScopeBlock(
                null,
                parentScope,
                statements);
        }

        /// <summary>
        /// Parses the statement node.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static JST.Statement ParseStatementNode(
            CommonTree tree,
            ScopeResolver resolver)
        {
            if (tree is CommonErrorNode)
            {
                CommonErrorNode error = (CommonErrorNode)tree;
                throw new ApplicationException(
                    string.Format(
                        "Error hit parsing start:{0}#{1} stop:{2}#{3}",
                        error.start.Line,
                        error.start.CharPositionInLine,
                        error.stop.Line,
                        error.stop.CharPositionInLine));
            }

            switch (tree.token.Type)
            {
                case JavaScriptLexer.RETURN_STATEMENT:
                    return new JST.ReturnStatement(
                        null,
                        resolver.Scope,
                        tree.Children == null
                            ? null
                            : Parser.ParseExpression(
                                (CommonTree)tree.Children[0],
                                resolver));
                case JavaScriptLexer.IF_BLOCK:
                    return new JST.IfBlockStatement(
                        null,
                        resolver.Scope,
                        Parser.ParseExpression(
                            (CommonTree)tree.Children[0],
                            resolver),
                        Parser.WrapInBlock(
                            resolver.Scope,
                            Parser.ParseStatementNode(
                                (CommonTree)tree.Children[1],
                                resolver)),
                        tree.Children.Count == 3
                            ? Parser.WrapInBlock(
                                resolver.Scope,
                                Parser.ParseStatementNode(
                                    (CommonTree)tree.Children[2],
                                    resolver))
                            : null);
                case JavaScriptLexer.EMPTY_STATEMENT:
                    return null;
                case JavaScriptParser.FOR_IN_BLOCK:
                    return Parser.ParseForInBlock(
                        tree,
                        resolver);
                case JavaScriptParser.FOR_BLOCK:
                    return Parser.ParseForBlock(
                        tree,
                        resolver);
                case JavaScriptParser.WHILE_BLOCK:
                    return Parser.ParseWhileBlock(
                        tree,
                        resolver);
                case JavaScriptParser.STATEMENT_BLOCK:
                    return Parser.ParseStatementBlock(
                        tree,
                        resolver);
                case JavaScriptParser.THROW:
                    return new JST.ThrowStatement(
                        null,
                        resolver.Scope,
                        Parser.ParseExpression(
                            (CommonTree)tree.Children[0],
                            resolver));
                case JavaScriptLexer.BREAK:
                    return new JST.BreakStatement(null, resolver.Scope);
                default:
                    {
                        JST.Expression expr =
                            Parser.ParseExpression(
                                tree,
                                resolver);

                        if (expr != null)
                        {
                            return new JST.ExpressionStatement(
                                null,
                                resolver.Scope,
                                expr);
                        }

                        return null;
                    }
            }
        }

        private static Statement ParseWhileBlock(CommonTree tree, ScopeResolver resolver)
        {
            Expression condition = Parser.ParseExpression(
                (CommonTree)tree.Children[0],
                resolver);

            return new WhileLoop(
                null,
                resolver.Scope,
                condition,
                Parser.ParseStatementNode(
                    (CommonTree)tree.Children[1],
                    resolver));
        }

        /// <summary>
        /// Parses for block.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static Statement ParseForBlock(
            CommonTree tree,
            ScopeResolver resolver)
        {
            Expression[] forParts = new Expression[3];
            Statement block;

            int iPart = 0;
            for (int iChild = 0; iChild < tree.ChildCount-1; iChild++)
            {
                if (tree.Children[iChild].Text == ";")
                {
                    iPart++;
                    continue;
                }

                forParts[iPart] = Parser.ParseExpression(
                    (CommonTree)tree.Children[iChild],
                    resolver);
            }

            block = Parser.ParseStatementNode(
                (CommonTree)tree.Children[tree.ChildCount - 1],
                resolver);

            return new ForLoop(
                null,
                resolver.Scope,
                forParts[1],
                new ExpressionStatement(
                    null,
                    resolver.Scope,
                    forParts[0]),
                new ExpressionStatement(
                    null,
                    resolver.Scope,
                    forParts[2]),
                block);
        }

        /// <summary>
        /// Wraps the specified statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>Wrap</returns>
        private static JST.ScopeBlock WrapInBlock(
            IdentifierScope scope,
            JST.Statement statement)
        {
            if (statement is JST.ScopeBlock)
            {
                return (JST.ScopeBlock)statement;
            }

            List<JST.Statement> statements = new List<Statement>();
            if (statement != null)
            {
                statements.Add(statement);
            }

            return new JST.ScopeBlock(
                null,
                scope,
                statements);
        }

        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>scope</returns>
        private static JST.Expression ParseExpression(
            CommonTree tree,
            ScopeResolver resolver)
        {
            switch (tree.Token.Type)
            {
                case JavaScriptLexer.INSTANCE_JSNI:
                    return Parser.ParseMemberAccessor(
                        tree,
                        resolver);
                case JavaScriptParser.TYPE_SEPERATOR:
                    return Parser.ParseImportedMember(tree, resolver);
                case JavaScriptParser.TYPENAME:
                    return Parser.ParseImportedTypeName(tree, resolver);
                case JavaScriptParser.Identifier:
                    if (((CommonTree)tree.Parent).Token.Type == JavaScriptParser.DOT_ACCESSOR_EXPR)
                    {
                        return new StringLiteralExpression(
                            resolver.Scope,
                            tree.token.Text);
                    }
                    else
                    {
                        return resolver.Resolve(tree.token.Text);
                    }
                case JavaScriptParser.NumericLiteral:
                    return new NumberLiteralExpression(
                        resolver.Scope,
                        long.Parse(
                            tree.token.Text));
                case JavaScriptParser.StringLiteral:
                    return new StringLiteralExpression(
                        resolver.Scope,
                        tree.Token.Text.Substring(1, tree.token.Text.Length-2));
                case JavaScriptParser.BoolLiteral:
                    return new BooleanLiteralExpression(
                        resolver.Scope,
                        bool.Parse(tree.token.Text));
                case JavaScriptParser.NULL:
                    return new NullLiteralExpression(
                        resolver.Scope);
                case JavaScriptParser.BINARY_EXPR:
                    return Parser.ParseBinaryExpression(
                        tree,
                        resolver);
                case JavaScriptParser.CONDITIONAL_EXPR:
                    return Parser.ParseConditionalExpression(
                        tree,
                        resolver);
                case JavaScriptParser.ACCESSOR_EXPR:
                    return Parser.ParseMemberAccessor(
                        tree,
                        resolver);
                case JavaScriptParser.METHOD_CALL:
                    return Parser.ParseMemberAccessor(
                        tree,
                        resolver);
                case JavaScriptParser.VAR:
                    return Parser.ParseVarDecl(
                        tree,
                        resolver);
                case JavaScriptParser.INLINE_OBJ_INIT:
                    return Parser.ParseInlineObjectInitializers(
                        tree,
                        resolver);
                case JavaScriptParser.POSTFIX:
                    return new UnaryExpression(
                        null,
                        resolver.Scope,
                        ((CommonTree)tree.Children[0]).Token.Type == JavaScriptParser.PLUS_INC
                            ? UnaryOperator.PostIncrement
                            : UnaryOperator.PostDecrement,
                        Parser.ParseExpression(
                            (CommonTree)tree.Children[1],
                            resolver));
                case JavaScriptParser.PREFIX:
                    return Parser.ParseUnaryExpression(
                        tree,
                        resolver);
                case JavaScriptParser.THIS:
                    return new ThisExpression(null, resolver.Scope);
                case JavaScriptParser.NEW_OBJ_EXPR:
                    return Parser.ParseNewExpression(tree, resolver);
                case JavaScriptParser.FUNCTION:
                    return Parser.ParseFunction(tree, resolver);
                case JavaScriptParser.INLINE_ARRAY_INIT:
                    return Parser.ParseInlineArray(tree, resolver);
                default:
                    throw new NotSupportedException(
                        string.Format(
                        "Expression for \"{0}\" is not supported",
                        tree.Token.Text));
            }
        }

        /// <summary>
        /// Parses the inline array.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Inline array initialization.</returns>
        private static Expression ParseInlineArray(CommonTree tree, ScopeResolver resolver)
        {
            List<JST.Expression> expressions = new List<Expression>();
            if (tree.Children != null)
            {
                foreach (CommonTree child in tree.Children)
                {
                    expressions.Add(Parser.ParseExpression(child, resolver));
                }
            }

            return new JST.InlineNewArrayInitialization(
                null,
                resolver.Scope,
                expressions);
        }

        /// <summary>
        /// Parses the function.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static Expression ParseFunction(
            CommonTree tree,
            ScopeResolver resolver)
        {
            IIdentifier identifier = null;
            if (tree.ChildCount == 3)
            {
                identifier = resolver.CreateIdentifier(((CommonTree)tree.Children[0]).Text);
            }

            List<string> args = new List<string>();
            var argsTree = (CommonTree)tree.Children[tree.ChildCount - 2];

            if (argsTree.Children != null)
            {
                foreach (var arg in ((CommonTree)tree.Children[tree.ChildCount-2]).Children)
                {
                    args.Add(arg.Text);
                }
            }

            resolver.PushScope(args);
            IdentifierScope scope = resolver.Scope;
            ScopeBlock scopeBlock = (ScopeBlock)Parser.ParseStatementBlock(
                (CommonTree)tree.Children[tree.ChildCount-1],
                resolver);
            resolver.PopScope();

            var functionExpression = new FunctionExpression(
                null,
                resolver.Scope,
                scope,
                scope.ParameterIdentifiers,
                identifier);

            functionExpression.AddStatements(scopeBlock.Statements);

            return functionExpression;
        }

        /// <summary>
        /// Parses the new expression.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>New Expression</returns>
        private static Expression ParseNewExpression(
            CommonTree tree,
            ScopeResolver resolver)
        {
            return new NewObjectExpression(
                null,
                resolver.Scope,
                Parser.ParseExpression((CommonTree)tree.Children[0], resolver),
                Parser.ParseArgs((CommonTree)tree.Children[1], resolver));
        }

        /// <summary>
        /// Parses the unary expression.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static Expression ParseUnaryExpression(
            CommonTree tree,
            ScopeResolver resolver)
        {
            int type = ((CommonTree)tree.Children[0]).Token.Type;
            UnaryOperator op = UnaryOperator.Void;
            switch (type)
            {
                case JavaScriptParser.DELETE:
                    op = UnaryOperator.Delete;
                    break;
                case JavaScriptParser.VOID:
                    op = UnaryOperator.Void;
                    break;
                case JavaScriptParser.TYPE_OF:
                    op = UnaryOperator.TypeOf;
                    break;
                case JavaScriptParser.PLUS_INC:
                    op = tree.Token.Type == JavaScriptParser.POSTFIX
                        ? UnaryOperator.PostIncrement
                        : UnaryOperator.PreIncrement;
                    break;
                case JavaScriptParser.MINUS_INC:
                    op = tree.Token.Type == JavaScriptParser.POSTFIX
                        ? UnaryOperator.PostDecrement
                        : UnaryOperator.PreDecrement;
                    break;
                case JavaScriptParser.PLUS:
                    op = UnaryOperator.UnaryPlus;
                    break;
                case JavaScriptParser.MINUS:
                    op = UnaryOperator.UnaryMinus;
                    break;
                case JavaScriptParser.BIT_NOT:
                    op = UnaryOperator.BitwiseNot;
                    break;
                case JavaScriptParser.NOT:
                    op = UnaryOperator.LogicalNot;
                    break;
                default:
                    break;
            }

            return new UnaryExpression(
                null,
                resolver.Scope,
                op,
                Parser.ParseExpression((CommonTree)tree.Children[1], resolver));
        }

        /// <summary>
        /// Parses the statement block.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static Statement ParseStatementBlock(CommonTree tree, ScopeResolver resolver)
        {
            ScopeBlock rv = new ScopeBlock(null, resolver.Scope, new List<Statement>());

            for (int iStatment = 0; iStatment < tree.ChildCount; iStatment++)
            {
                rv.AddStatement(
                    Parser.ParseStatementNode(
                        (CommonTree)tree.Children[iStatment],
                        resolver));
            }

            return rv;
        }

        /// <summary>
        /// Parses for in block.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static Statement ParseForInBlock(
            CommonTree tree,
            ScopeResolver resolver)
        {
            IdentifierExpression key;
            Expression expression;
            Statement loopBlock;

            if (tree.ChildCount == 4)
            {
                key = new IdentifierExpression(resolver.CreateIdentifier(tree.Children[1].Text), resolver.Scope);
            }
            else
            {
                key = (IdentifierExpression)resolver.Resolve(tree.Children[0].Text);
            }

            expression = Parser.ParseExpression((CommonTree)tree.Children[tree.ChildCount-2], resolver);
            loopBlock = Parser.ParseStatementNode((CommonTree)tree.Children[tree.ChildCount - 1], resolver);

            return new ForInLoop(
                null,
                resolver.Scope,
                expression,
                key,
                loopBlock);
        }

        /// <summary>
        /// Parses the inline object initializers.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static JST.Expression ParseInlineObjectInitializers(
            CommonTree tree,
            ScopeResolver resolver)
        {
            var rv = new InlineObjectInitializer(null, resolver.Scope);

            if (tree.ChildCount > 0)
            {
                for (int iChild = 0; iChild < tree.ChildCount; iChild++)
                {
                    CommonTree nameValuePair = (CommonTree)tree.Children[iChild];

                    if (nameValuePair.Token.Type == JavaScriptParser.PROP_NAME_VALUE)
                    {
                        rv.AddInitializer(
                            nameValuePair.Children[0].Text,
                            Parser.ParseExpression(
                                (CommonTree)nameValuePair.Children[1],
                                resolver));
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// Parses the imported member.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Resolve expression for member.</returns>
        private static JST.Expression ParseImportedMember(
            CommonTree tree,
            ScopeResolver resolver,
            bool isInstance = false)
        {
            string memberName = tree.Children[1].Text;
            Tuple<string, string> typeName = Parser.ParseTypeName(
                (CommonTree)tree.Children[0]);

            if (tree.ChildCount == 3)
            {
                List<Tuple<string, string>> arguments = new List<Tuple<string, string>>();
                if (tree.Children[2].ChildCount > 0)
                {
                    foreach (var item in ((CommonTree)tree.Children[2]).Children)
                    {
                        arguments.Add(
                            Parser.ParseTypeName((CommonTree)item));
                    }
                }

                return resolver.ResolveMethod(
                    typeName,
                    isInstance,
                    memberName,
                    arguments.ToArray());
            }
            else
            {
                return resolver.ResolveField(
                    typeName,
                    isInstance,
                    memberName);
            }
        }

        /// <summary>
        /// Parses the name of the imported type.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Resolved expression for given type.</returns>
        private static JST.Expression ParseImportedTypeName(
            CommonTree tree,
            ScopeResolver resolver)
        {
            return resolver.ResolveType(
                Parser.ParseTypeName(tree));
        }

        /// <summary>
        /// Parses the name of the type.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <returns>Type name in Tuple form.</returns>
        private static Tuple<string, string> ParseTypeName(
            CommonTree tree)
        {
            StringBuilder[] parts = new StringBuilder[2];

            if (tree.ChildCount > 2)
            {
                throw new ApplicationException(
                    string.Format(
                        "Can't resolve typeName at {0}#{1}",
                        tree.Line,
                        tree.CharPositionInLine));
            }
            if (tree.ChildCount == 1)
            {
                parts[1] = new StringBuilder();
                parts[1].Append(tree.Children[0].Text);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    parts[i] = new StringBuilder();
                    CommonTree nameTree = (CommonTree)tree.Children[i];
                    foreach (var item in nameTree.Children)
                    {
                        if (parts[i].Length > 0
                            && item.Text[0] != '`')
                        {
                            parts[i].Append('.');
                        }

                        parts[i].Append(item.Text);
                    }
                }
            }

            return Tuple.Create(
                parts[0] != null ? parts[0].ToString() : null,
                parts[1].ToString());
        }

        /// <summary>
        /// Parses the var decl.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static JST.Expression ParseVarDecl(
            CommonTree tree,
            ScopeResolver resolver)
        {
            JST.Expression rv = null;
            if (tree.ChildCount > 0)
            {
                foreach (CommonTree identifier in tree.Children)
                {
                    if (identifier.Token.Type == JavaScriptParser.BINARY_EXPR)
                    {
                        resolver.CreateIdentifier(
                            identifier.Children[0].Text);

                        JST.Expression expr =
                            new JST.BinaryExpression(
                                null,
                                resolver.Scope,
                                BinaryOperator.Assignment,
                                resolver.Resolve(identifier.Children[0].Text),
                                Parser.ParseExpression(
                                    (CommonTree)identifier.Children[2],
                                    resolver));

                        if (rv == null)
                        {
                            rv = expr;
                        }
                        else
                        {
                            rv = new JST.BinaryExpression(
                                null,
                                resolver.Scope,
                                BinaryOperator.Comma,
                                rv,
                                expr);
                        }
                    }
                    else
                    {
                        resolver.CreateIdentifier(identifier.Text);
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// Parses the member accessor.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Returns Index accessor or methodCall</returns>
        private static JST.Expression ParseMemberAccessor(
            CommonTree tree,
            ScopeResolver resolver)
        {
            CommonTree child0 = (CommonTree)tree.Children[0];
            CommonTree child1 = (CommonTree)tree.Children[1];

            if (child1.Token.Type == JavaScriptLexer.INSTANCE_JSNI)
            {
                return new JST.IndexExpression(
                    null,
                    resolver.Scope,
                    Parser.ParseExpression(
                        child0,
                        resolver),
                    Parser.ParseImportedMember(
                        (CommonTree)child1.Children[0],
                        resolver,
                        true));
            }

            if (child1.Token.Type == JavaScriptParser.DOT_ACCESSOR_EXPR
                || child1.Token.Type == JavaScriptParser.INDEX_EXPR)
            {
                return new JST.IndexExpression(
                    null,
                    resolver.Scope,
                    Parser.ParseExpression(
                        child0,
                        resolver),
                    Parser.ParseExpression(
                        (CommonTree)child1.Children[0],
                        resolver),
                    child1.Token.Type == JavaScriptParser.INDEX_EXPR);
            }
            else
            {
                return new JST.MethodCallExpression(
                    null,
                    resolver.Scope,
                    Parser.ParseExpression(
                        child0,
                        resolver),
                    Parser.ParseArgs(child1, resolver));
            }
        }

        /// <summary>
        /// Parses the args.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        private static Expression[] ParseArgs(CommonTree tree, ScopeResolver resolver)
        {
            List<JST.Expression> arguments = new List<Expression>();

            if (tree.Children != null)
            {
                foreach (CommonTree arg in tree.Children)
                {
                    arguments.Add(
                        Parser.ParseExpression(arg, resolver));
                }
            }

            return arguments.ToArray();
        }

        /// <summary>
        /// Parses the conditional expression.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Conditional expression.</returns>
        private static JST.ConditionalOperatorExpression ParseConditionalExpression(
            CommonTree tree,
            ScopeResolver resolver)
        {
            return new JST.ConditionalOperatorExpression(
                null,
                resolver.Scope,
                Parser.ParseExpression(
                    (CommonTree)tree.Children[0],
                    resolver),
                Parser.ParseExpression(
                    (CommonTree)tree.Children[1],
                    resolver),
                Parser.ParseExpression(
                    (CommonTree)tree.Children[2],
                    resolver));

        }

        /// <summary>
        /// Parses the binary expression.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>a binary expression.</returns>
        private static JST.BinaryExpression ParseBinaryExpression(
            CommonTree tree,
            ScopeResolver resolver)
        {
            BinaryOperator binaryOperator;

            switch (((CommonTree)tree.Children[0]).Token.Type)
            {
                case JavaScriptLexer.ASN:
                    binaryOperator = BinaryOperator.Assignment;
                    break;
                case JavaScriptLexer.BIT_AND:
                    binaryOperator = BinaryOperator.BitwiseAnd;
                    break;
                case JavaScriptLexer.BIT_AND_ASN:
                    binaryOperator = BinaryOperator.BitwiseAndAssignment;
                    break;
                case JavaScriptLexer.BIT_OR:
                    binaryOperator = BinaryOperator.BitwiseOr;
                    break;
                case JavaScriptLexer.BIT_OR_ASN:
                    binaryOperator = BinaryOperator.BitwiseOrAssignment;
                    break;
                case JavaScriptLexer.BIT_XOR:
                    binaryOperator = BinaryOperator.BitwiseXor;
                    break;
                case JavaScriptLexer.BIT_XOR_ASN:
                    binaryOperator = BinaryOperator.BitwiseXorAssignment;
                    break;
                case JavaScriptLexer.PLUS:
                    binaryOperator = BinaryOperator.Plus;
                    break;
                case JavaScriptLexer.PLUS_ASN:
                    binaryOperator = BinaryOperator.PlusAssignment;
                    break;
                case JavaScriptLexer.DIV:
                    binaryOperator = BinaryOperator.Div;
                    break;
                case JavaScriptLexer.DIV_ASN:
                    binaryOperator = BinaryOperator.DivAssignment;
                    break;
                case JavaScriptLexer.MUL:
                    binaryOperator = BinaryOperator.Mul;
                    break;
                case JavaScriptLexer.MUL_ASN:
                    binaryOperator = BinaryOperator.MulAssignment;
                    break;
                case JavaScriptLexer.MINUS:
                    binaryOperator = BinaryOperator.Minus;
                    break;
                case JavaScriptLexer.MINUS_ASN:
                    binaryOperator = BinaryOperator.MinusAssignment;
                    break;
                case JavaScriptLexer.MOD:
                    binaryOperator = BinaryOperator.Mod;
                    break;
                case JavaScriptLexer.MOD_ASN:
                    binaryOperator = BinaryOperator.ModAssignment;
                    break;
                case JavaScriptLexer.SHL:
                    binaryOperator = BinaryOperator.LeftShift;
                    break;
                case JavaScriptLexer.SHL_ASN:
                    binaryOperator = BinaryOperator.LeftShiftAssignment;
                    break;
                case JavaScriptLexer.SHR:
                    binaryOperator = BinaryOperator.RightShift;
                    break;
                case JavaScriptLexer.SHR_ASN:
                    binaryOperator = BinaryOperator.RightShiftAssignment;
                    break;
                case JavaScriptLexer.USHR:
                    binaryOperator = BinaryOperator.UnsignedRightShift;
                    break;
                case JavaScriptLexer.USHR_ASN:
                    binaryOperator = BinaryOperator.UnsignedRightShiftAssignment;
                    break;
                case JavaScriptLexer.AND:
                    binaryOperator = BinaryOperator.LogicalAnd;
                    break;
                case JavaScriptLexer.OR:
                    binaryOperator = BinaryOperator.LogicalOr;
                    break;
                case JavaScriptLexer.EQ:
                    binaryOperator = BinaryOperator.Equals;
                    break;
                case JavaScriptLexer.NEQ:
                    binaryOperator = BinaryOperator.NotEquals;
                    break;
                case JavaScriptLexer.REQ:
                    binaryOperator = BinaryOperator.StrictEquals;
                    break;
                case JavaScriptLexer.RNQ:
                    binaryOperator = BinaryOperator.StrictNotEquals;
                    break;
                case JavaScriptLexer.GT:
                    binaryOperator = BinaryOperator.GreaterThan;
                    break;
                case JavaScriptLexer.GTE:
                    binaryOperator = BinaryOperator.GreaterThanOrEqual;
                    break;
                case JavaScriptLexer.LT:
                    binaryOperator = BinaryOperator.LessThan;
                    break;
                case JavaScriptLexer.LTE:
                    binaryOperator = BinaryOperator.LessThanOrEqual;
                    break;
                case JavaScriptLexer.INST_OF:
                    binaryOperator = BinaryOperator.InstanceOf;
                    break;
                case JavaScriptLexer.IN:
                    binaryOperator = BinaryOperator.In;
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format(
                        "BinaryExpression for {0} is not supported",
                        ((CommonTree)tree.Children[0]).Token.Text));
            }

            return new BinaryExpression(
                null,
                resolver.Scope,
                binaryOperator,
                Parser.ParseExpression(
                    (CommonTree)tree.Children[1],
                    resolver),
                Parser.ParseExpression(
                    (CommonTree)tree.Children[2],
                    resolver));
        }
    }
}
