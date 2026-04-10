using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class CssLiteralReplacerTests
    {
        private IdentifierScope _scope;
        private Dictionary<string, IIdentifier> _cssMap;
        private CssLiteralReplacer _replacer;

        [TestInitialize]
        public void Setup()
        {
            _scope = new IdentifierScope(false);
            _cssMap = new Dictionary<string, IIdentifier>
            {
                ["header"] = SimpleIdentifier.CreateScopeIdentifier(_scope, "header", false, true),
                ["footer"] = SimpleIdentifier.CreateScopeIdentifier(_scope, "footer", false, true),
                ["active"] = SimpleIdentifier.CreateScopeIdentifier(_scope, "active", false, true),
            };
            _replacer = new CssLiteralReplacer(_cssMap);
        }

        // --- TryCreateCssExpression / basic string replacement ---

        [TestMethod]
        public void TransformStatements_ReturnsOriginalList_WhenNoMatches()
        {
            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "no-match"))
            };

            var result = _replacer.TransformStatements(stmts);

            result.Should().BeSameAs(stmts, "no CSS strings matched, original list returned");
        }

        [TestMethod]
        public void TransformStatements_ReturnsOriginalList_WhenEmptyMap()
        {
            var emptyReplacer = new CssLiteralReplacer(new Dictionary<string, IIdentifier>());
            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "header"))
            };

            var result = emptyReplacer.TransformStatements(stmts);

            result.Should().BeSameAs(stmts);
        }

        [TestMethod]
        public void TransformStatements_ReturnsNull_WhenNull()
        {
            _replacer.TransformStatements(null).Should().BeNull();
        }

        [TestMethod]
        public void TransformStatements_ReplacesSingleCssString()
        {
            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "header"))
            };

            var result = _replacer.TransformStatements(stmts);

            result.Should().NotBeSameAs(stmts);
            var retStmt = result[0].Should().BeOfType<ReturnStatement>().Subject;
            retStmt.ReturnExpression.Should().BeOfType<IdentifierStringExpression>();
        }

        [TestMethod]
        public void TransformStatements_ReplacesMultiTokenCssString()
        {
            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "header active"))
            };

            var result = _replacer.TransformStatements(stmts);

            result.Should().NotBeSameAs(stmts);
            var retStmt = result[0].Should().BeOfType<ReturnStatement>().Subject;
            retStmt.ReturnExpression.Should().BeOfType<IdentifierStringExpression>();
        }

        [TestMethod]
        public void TransformStatements_LeavesPartialCssMatch_Unchanged()
        {
            // "header unknown" — "unknown" not in map, so leave entire string unchanged
            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "header unknown"))
            };

            var result = _replacer.TransformStatements(stmts);

            result.Should().BeSameAs(stmts, "partial match means no replacement");
        }

        // --- ExpressionStatement ---

        [TestMethod]
        public void TransformStatements_ReplacesExpressionStatement()
        {
            var assign = new BinaryExpression(null, _scope, BinaryOperator.Assignment,
                new IdentifierExpression(_cssMap["header"], _scope),
                new StringLiteralExpression(_scope, "footer"));
            var stmts = new List<Statement>
            {
                new ExpressionStatement(null, _scope, assign)
            };

            var result = _replacer.TransformStatements(stmts);

            result.Should().NotBeSameAs(stmts);
            var exprStmt = result[0].Should().BeOfType<ExpressionStatement>().Subject;
            var bin = exprStmt.Expression.Should().BeOfType<BinaryExpression>().Subject;
            bin.Right.Should().BeOfType<IdentifierStringExpression>();
        }

        // --- VarInitializerStatement ---

        [TestMethod]
        public void TransformStatements_ReplacesVarInitializer()
        {
            var inits = new List<Expression>
            {
                new BinaryExpression(null, _scope, BinaryOperator.Assignment,
                    new IdentifierExpression(_cssMap["active"], _scope),
                    new StringLiteralExpression(_scope, "active"))
            };
            var stmts = new List<Statement>
            {
                new VarInitializerStatement(null, _scope, inits)
            };

            var result = _replacer.TransformStatements(stmts);

            result.Should().NotBeSameAs(stmts);
        }

        // --- ConditionalOperatorExpression ---

        [TestMethod]
        public void TransformExpression_ReplacesInTernary()
        {
            var ternary = new ConditionalOperatorExpression(null, _scope,
                new IdentifierExpression(_cssMap["header"], _scope),
                new StringLiteralExpression(_scope, "active"),
                new StringLiteralExpression(_scope, "no-match"));

            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, ternary)
            };

            var result = _replacer.TransformStatements(stmts);
            var retStmt = result[0].Should().BeOfType<ReturnStatement>().Subject;
            var cond = retStmt.ReturnExpression.Should().BeOfType<ConditionalOperatorExpression>().Subject;
            cond.TrueExpression.Should().BeOfType<IdentifierStringExpression>();
            cond.FalseExpression.Should().BeOfType<StringLiteralExpression>("no-match not in CSS map");
        }

        // --- BinaryExpression ---

        [TestMethod]
        public void TransformExpression_ReplacesInBinaryComparison()
        {
            var binary = new BinaryExpression(null, _scope, BinaryOperator.StrictEquals,
                new IdentifierExpression(_cssMap["header"], _scope),
                new StringLiteralExpression(_scope, "header"));

            var stmts = new List<Statement>
            {
                new ExpressionStatement(null, _scope, binary)
            };

            var result = _replacer.TransformStatements(stmts);
            var exprStmt = result[0].Should().BeOfType<ExpressionStatement>().Subject;
            var bin = exprStmt.Expression.Should().BeOfType<BinaryExpression>().Subject;
            bin.Right.Should().BeOfType<IdentifierStringExpression>();
        }

        // --- MethodCallExpression ---

        [TestMethod]
        public void TransformExpression_ReplacesInMethodCallArguments()
        {
            var call = new MethodCallExpression(null, _scope,
                new IdentifierExpression(_cssMap["header"], _scope),
                new List<Expression> { new StringLiteralExpression(_scope, "footer") });

            var stmts = new List<Statement>
            {
                new ExpressionStatement(null, _scope, call)
            };

            var result = _replacer.TransformStatements(stmts);
            var exprStmt = result[0].Should().BeOfType<ExpressionStatement>().Subject;
            var mc = exprStmt.Expression.Should().BeOfType<MethodCallExpression>().Subject;
            mc.Arguments[0].Should().BeOfType<IdentifierStringExpression>();
        }

        // --- FunctionExpression (IIFE body) ---

        [TestMethod]
        public void TransformExpression_ReplacesInsideFunctionExpression()
        {
            var innerScope = new IdentifierScope(_scope);
            var funcExpr = new FunctionExpression(null, _scope, innerScope,
                new List<IIdentifier>(), null, false, false);
            funcExpr.AddStatements(new List<Statement>
            {
                new ReturnStatement(null, innerScope, new StringLiteralExpression(innerScope, "active"))
            });

            var stmts = new List<Statement>
            {
                new ExpressionStatement(null, _scope,
                    new MethodCallExpression(null, _scope, funcExpr, new List<Expression>()))
            };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
        }

        // --- InlineNewArrayInitialization ---

        [TestMethod]
        public void TransformExpression_ReplacesInsideNewArrayInitialization()
        {
            var values = new List<Expression>
            {
                new StringLiteralExpression(_scope, "header"),
                new StringLiteralExpression(_scope, "no-match"),
                new StringLiteralExpression(_scope, "footer"),
            };
            var arrayInit = new InlineNewArrayInitialization(null, _scope, values);

            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, arrayInit)
            };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
            var retStmt = result[0].Should().BeOfType<ReturnStatement>().Subject;
            var arr = retStmt.ReturnExpression.Should().BeOfType<InlineNewArrayInitialization>().Subject;
            arr.Values[0].Should().BeOfType<IdentifierStringExpression>("'header' is in CSS map");
            arr.Values[1].Should().BeOfType<StringLiteralExpression>("'no-match' stays unchanged");
            arr.Values[2].Should().BeOfType<IdentifierStringExpression>("'footer' is in CSS map");
        }

        // --- InlineObjectInitializer ---

        [TestMethod]
        public void TransformExpression_ReplacesInsideInlineObject()
        {
            var objInit = new InlineObjectInitializer(null, _scope);
            var keyId = SimpleIdentifier.CreateScopeIdentifier(_scope, "cls", false, true);
            objInit.AddInitializer(keyId, new StringLiteralExpression(_scope, "active"));

            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, objInit)
            };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
        }

        // --- ForLoop ---

        [TestMethod]
        public void TransformStatement_ReplacesInsideForLoop()
        {
            var loopBody = new ScopeBlock(null, _scope, new List<Statement>
            {
                new ExpressionStatement(null, _scope,
                    new BinaryExpression(null, _scope, BinaryOperator.StrictEquals,
                        new IdentifierExpression(_cssMap["header"], _scope),
                        new StringLiteralExpression(_scope, "header")))
            });

            var forLoop = new ForLoop(null, _scope,
                new IdentifierExpression(_cssMap["header"], _scope),
                null, null, loopBody);

            var stmts = new List<Statement> { forLoop };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
        }

        // --- IfBlockStatement ---

        [TestMethod]
        public void TransformStatement_ReplacesInsideIfBlock()
        {
            var trueBlock = new ScopeBlock(null, _scope, new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "active"))
            });
            var falseBlock = new ScopeBlock(null, _scope, new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, "footer"))
            });
            var ifStmt = new IfBlockStatement(null, _scope,
                new StringLiteralExpression(_scope, "header"),
                trueBlock, falseBlock);

            var stmts = new List<Statement> { ifStmt };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
        }

        // --- ArrayLiteralExpression ---

        [TestMethod]
        public void TransformExpression_ReplacesInsideArrayLiteral()
        {
            var elements = new List<Expression>
            {
                new StringLiteralExpression(_scope, "active"),
                new StringLiteralExpression(_scope, "no-match"),
            };
            var arrayLit = new ArrayLiteralExpression(null, _scope, elements);

            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, arrayLit)
            };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
            var retStmt = result[0].Should().BeOfType<ReturnStatement>().Subject;
            var arr = retStmt.ReturnExpression.Should().BeOfType<ArrayLiteralExpression>().Subject;
            arr.Elements[0].Should().BeOfType<IdentifierStringExpression>();
            arr.Elements[1].Should().BeOfType<StringLiteralExpression>();
        }

        // --- UnaryExpression ---

        [TestMethod]
        public void TransformExpression_ReplacesInsideUnaryExpression()
        {
            var unary = new UnaryExpression(null, _scope, UnaryOperator.LogicalNot,
                new StringLiteralExpression(_scope, "active"));

            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, unary)
            };

            var result = _replacer.TransformStatements(stmts);
            result.Should().NotBeSameAs(stmts);
        }

        // --- Empty string edge case ---

        [TestMethod]
        public void TransformStatements_IgnoresEmptyStringLiteral()
        {
            var stmts = new List<Statement>
            {
                new ReturnStatement(null, _scope, new StringLiteralExpression(_scope, ""))
            };

            var result = _replacer.TransformStatements(stmts);
            result.Should().BeSameAs(stmts);
        }
    }
}
