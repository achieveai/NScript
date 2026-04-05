using System;
using System.Collections.Generic;
using NScript.JST;

namespace NScript.RazorSkin
{
    /// <summary>
    /// Replaces StringLiteralExpression nodes in the JST tree with
    /// IdentifierStringExpression nodes for CSS class name minification.
    /// Uses immutable clone approach — creates new nodes where CSS strings are found,
    /// leaving the original tree unchanged for branches without matches.
    /// </summary>
    internal class CssLiteralReplacer
    {
        private readonly Dictionary<string, IIdentifier> _cssClassMap;

        public CssLiteralReplacer(Dictionary<string, IIdentifier> cssClassMap)
        {
            _cssClassMap = cssClassMap ?? throw new ArgumentNullException(nameof(cssClassMap));
        }

        /// <summary>
        /// Walks a list of statements and replaces CSS string literals
        /// with IdentifierStringExpression nodes.
        /// Returns the original list if no replacements were made.
        /// </summary>
        public List<Statement> TransformStatements(List<Statement> statements)
        {
            if (_cssClassMap.Count == 0 || statements == null) return statements;

            bool changed = false;
            var result = new List<Statement>(statements.Count);

            for (int i = 0; i < statements.Count; i++)
            {
                var transformed = TransformStatement(statements[i]);
                result.Add(transformed);
                if (!ReferenceEquals(transformed, statements[i]))
                    changed = true;
            }

            return changed ? result : statements;
        }

        private Statement TransformStatement(Statement stmt)
        {
            if (stmt is ReturnStatement ret)
            {
                if (ret.ReturnExpression == null) return stmt;
                var newExpr = TransformExpression(ret.ReturnExpression);
                if (!ReferenceEquals(newExpr, ret.ReturnExpression))
                    return new ReturnStatement(ret.Location, ret.Scope, newExpr);
                return stmt;
            }

            if (stmt is ExpressionStatement exprStmt)
            {
                var newExpr = TransformExpression(exprStmt.Expression);
                if (!ReferenceEquals(newExpr, exprStmt.Expression))
                    return new ExpressionStatement(exprStmt.Location, exprStmt.Scope, newExpr);
                return stmt;
            }

            if (stmt is VarInitializerStatement varInit)
            {
                return TransformInitializerStatement(varInit, isVar: true);
            }

            if (stmt is InitializerStatement initStmt)
            {
                return TransformInitializerStatement(initStmt, isVar: false);
            }

            if (stmt is IfBlockStatement ifBlock)
            {
                return TransformIfBlock(ifBlock);
            }

            if (stmt is ScopeBlock scopeBlock)
            {
                return TransformScopeBlock(scopeBlock);
            }

            if (stmt is ForLoop forLoop)
            {
                return TransformForLoop(forLoop);
            }

            return stmt;
        }

        private Statement TransformInitializerStatement(InitializerStatement initStmt, bool isVar)
        {
            bool changed = false;
            var inits = initStmt.Initializers;
            var newInits = new List<Expression>(inits.Count);

            for (int i = 0; i < inits.Count; i++)
            {
                var newInit = TransformExpression(inits[i]);
                newInits.Add(newInit);
                if (!ReferenceEquals(newInit, inits[i]))
                    changed = true;
            }

            if (!changed) return initStmt;

            if (isVar)
                return new VarInitializerStatement(initStmt.Location, initStmt.Scope, newInits);
            return new InitializerStatement(initStmt.Location, initStmt.Scope, newInits);
        }

        private Statement TransformIfBlock(IfBlockStatement ifBlock)
        {
            var newCond = TransformExpression(ifBlock.Condition);
            var newTrue = TransformScopeBlock(ifBlock.TrueBlock);
            var newFalse = ifBlock.FalseBlock != null ? TransformScopeBlock(ifBlock.FalseBlock) : null;

            if (!ReferenceEquals(newCond, ifBlock.Condition) ||
                !ReferenceEquals(newTrue, ifBlock.TrueBlock) ||
                (newFalse != null && !ReferenceEquals(newFalse, ifBlock.FalseBlock)))
            {
                return new IfBlockStatement(
                    ifBlock.Location, ifBlock.Scope,
                    newCond, (ScopeBlock)newTrue, (ScopeBlock)(newFalse ?? ifBlock.FalseBlock));
            }

            return ifBlock;
        }

        private ScopeBlock TransformScopeBlock(ScopeBlock scopeBlock)
        {
            if (scopeBlock == null) return null;
            var stmts = new List<Statement>(scopeBlock.Statements);
            var newStmts = TransformStatements(stmts);
            if (ReferenceEquals(newStmts, stmts)) return scopeBlock;

            return new ScopeBlock(scopeBlock.Location, scopeBlock.Scope, newStmts);
        }

        private Statement TransformForLoop(ForLoop forLoop)
        {
            var newCond = forLoop.Condition != null ? TransformExpression(forLoop.Condition) : null;
            var newInit = forLoop.InitializationBlock != null ? TransformStatement(forLoop.InitializationBlock) : null;
            var newIncr = forLoop.IncrementBlock != null ? TransformStatement(forLoop.IncrementBlock) : null;
            var newLoop = forLoop.Loop != null ? TransformStatement(forLoop.Loop) : null;

            if (!ReferenceEquals(newCond, forLoop.Condition) ||
                !ReferenceEquals(newInit, forLoop.InitializationBlock) ||
                !ReferenceEquals(newIncr, forLoop.IncrementBlock) ||
                !ReferenceEquals(newLoop, forLoop.Loop))
            {
                return new ForLoop(
                    forLoop.Location, forLoop.Scope,
                    newCond ?? forLoop.Condition,
                    newInit ?? forLoop.InitializationBlock,
                    newIncr ?? forLoop.IncrementBlock,
                    newLoop ?? forLoop.Loop);
            }

            return forLoop;
        }

        /// <summary>
        /// Recursively transforms an expression, replacing CSS string literals.
        /// Returns the original expression if no changes needed.
        /// </summary>
        private Expression TransformExpression(Expression expr)
        {
            if (expr == null) return null;

            if (expr is StringLiteralExpression strLit)
            {
                var cssExpr = TryCreateCssExpression(strLit);
                return cssExpr ?? expr;
            }

            if (expr is ConditionalOperatorExpression cond)
            {
                var newCond = TransformExpression(cond.Condition);
                var newTrue = TransformExpression(cond.TrueExpression);
                var newFalse = TransformExpression(cond.FalseExpression);

                if (!ReferenceEquals(newCond, cond.Condition) ||
                    !ReferenceEquals(newTrue, cond.TrueExpression) ||
                    !ReferenceEquals(newFalse, cond.FalseExpression))
                {
                    return new ConditionalOperatorExpression(
                        cond.Location, cond.Scope, newCond, newTrue, newFalse);
                }
                return expr;
            }

            if (expr is BinaryExpression bin)
            {
                var newLeft = TransformExpression(bin.Left);
                var newRight = TransformExpression(bin.Right);

                if (!ReferenceEquals(newLeft, bin.Left) ||
                    !ReferenceEquals(newRight, bin.Right))
                {
                    return new BinaryExpression(
                        bin.Location, bin.Scope, bin.Operator, newLeft, newRight);
                }
                return expr;
            }

            if (expr is UnaryExpression unary)
            {
                var newOperand = TransformExpression(unary.NestedExpression);
                if (!ReferenceEquals(newOperand, unary.NestedExpression))
                    return new UnaryExpression(
                        unary.Location, unary.Scope, unary.Operator, newOperand);
                return expr;
            }

            if (expr is MethodCallExpression methodCall)
            {
                return TransformMethodCall(methodCall);
            }

            if (expr is ArrayLiteralExpression arrayLit)
            {
                return TransformArrayLiteral(arrayLit);
            }

            if (expr is FunctionExpression funcExpr)
            {
                return TransformFunctionExpression(funcExpr);
            }

            if (expr is InlineObjectInitializer objInit)
            {
                return TransformInlineObject(objInit);
            }

            if (expr is InlineNewArrayInitialization newArrayInit)
            {
                return TransformNewArrayInitialization(newArrayInit);
            }

            // IdentifierExpression, IndexExpression, NewObjectExpression, etc.
            // — unlikely to contain CSS string literals. Skip for now.
            return expr;
        }

        private Expression TransformMethodCall(MethodCallExpression methodCall)
        {
            bool changed = false;
            var args = methodCall.Arguments;
            var newArgs = new List<Expression>(args.Count);

            for (int i = 0; i < args.Count; i++)
            {
                var newArg = TransformExpression(args[i]);
                newArgs.Add(newArg);
                if (!ReferenceEquals(newArg, args[i]))
                    changed = true;
            }

            var newMethod = TransformExpression(methodCall.MethodExpression);
            if (!ReferenceEquals(newMethod, methodCall.MethodExpression))
                changed = true;

            if (!changed) return methodCall;

            return new MethodCallExpression(
                methodCall.Location, methodCall.Scope,
                newMethod, newArgs);
        }

        private Expression TransformArrayLiteral(ArrayLiteralExpression arrayLit)
        {
            bool changed = false;
            var elements = arrayLit.Elements;
            var newElements = new List<Expression>(elements.Count);

            for (int i = 0; i < elements.Count; i++)
            {
                var newElem = TransformExpression(elements[i]);
                newElements.Add(newElem);
                if (!ReferenceEquals(newElem, elements[i]))
                    changed = true;
            }

            if (!changed) return arrayLit;

            return new ArrayLiteralExpression(arrayLit.Location, arrayLit.Scope, newElements);
        }

        private Expression TransformFunctionExpression(FunctionExpression funcExpr)
        {
            var stmts = new List<Statement>(funcExpr.Statements);
            var newStmts = TransformStatements(stmts);
            if (ReferenceEquals(newStmts, stmts)) return funcExpr;

            var newFunc = new FunctionExpression(
                funcExpr.Location, funcExpr.Scope, funcExpr.InnerScope,
                funcExpr.Parameters, funcExpr.Name,
                funcExpr.IsAsync, funcExpr.IsGenerator);
            newFunc.AddStatements(newStmts);
            return newFunc;
        }

        private Expression TransformInlineObject(InlineObjectInitializer objInit)
        {
            bool changed = false;
            var newObj = new InlineObjectInitializer(objInit.Location, objInit.Scope);

            foreach (var init in objInit.Initializers)
            {
                var newValue = TransformExpression(init.Item2);
                if (!ReferenceEquals(newValue, init.Item2))
                    changed = true;

                if (init.Item1 is IdentifierExpression idExpr)
                    newObj.AddInitializer(idExpr.Identifier, newValue);
                else if (init.Item1 is StringLiteralExpression strKey)
                    newObj.AddInitializer(strKey.StringLiteral, newValue);
                else
                    newObj.AddInitializer(init.Item1.ToString(), newValue);
            }

            return changed ? newObj : objInit;
        }

        private Expression TransformNewArrayInitialization(InlineNewArrayInitialization arrayInit)
        {
            bool changed = false;
            var values = arrayInit.Values;
            var newValues = new List<Expression>(values.Count);

            for (int i = 0; i < values.Count; i++)
            {
                var newVal = TransformExpression(values[i]);
                newValues.Add(newVal);
                if (!ReferenceEquals(newVal, values[i]))
                    changed = true;
            }

            if (!changed) return arrayInit;

            return new InlineNewArrayInitialization(arrayInit.Location, arrayInit.Scope, newValues);
        }

        /// <summary>
        /// Tries to create an IdentifierStringExpression from a CSS string literal.
        /// Splits the string on spaces and resolves each token against the CSS class map.
        /// Returns null if any token doesn't match (not a CSS string).
        /// </summary>
        private Expression TryCreateCssExpression(StringLiteralExpression strLit)
        {
            var value = strLit.StringLiteral;
            if (string.IsNullOrEmpty(value)) return null;

            var tokens = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return null;

            // Try to resolve ALL tokens — if any fails, this isn't a CSS string
            var identifiers = new IIdentifier[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                IIdentifier id;
                if (!_cssClassMap.TryGetValue(tokens[i], out id))
                    return null; // Not all tokens are CSS classes — leave unchanged
                identifiers[i] = id;
            }

            // All tokens matched! Build IdentifierStringExpression
            var scope = strLit.Scope;
            var firstIdExpr = new IdentifierExpression(identifiers[0], scope);
            var cssExpr = new IdentifierStringExpression(null, scope, firstIdExpr);

            for (int i = 1; i < identifiers.Length; i++)
            {
                cssExpr.Append(new StringLiteralExpression(scope, " "));
                cssExpr.Append(new IdentifierExpression(identifiers[i], scope));
            }

            return cssExpr;
        }
    }
}
