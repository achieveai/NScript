namespace NScript.JSParser.Test
{
    using MoreLinq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NScript.JST.Visitors;
    using System.Linq;
    using NScript.JST;

    [TestClass]
    public class JstOptimizerTests
    {
        const string InlineResourceNS = "NScript.JSParser.Test.InlineOptimizers.";

        [TestMethod]
        public void JstInlinableVisitorTest()
        {
            var scopeBlock = JSParserAndGeneratorHelper.ReadJstFromResourceScript(
                InlineResourceNS + "SimpleInlinableMethods.js");

            var inlinableVisitor = new InlineableVisitor();
            var jsVisitor = inlinableVisitor as IJstVisitor;
            jsVisitor.DispatchStatement(scopeBlock);

            var dict = inlinableVisitor.Functions
                .Select(kv => (kv.Key.SuggestedName, kv.Value))
                .ToDictionary(kv => kv.SuggestedName, kv => kv.Value);

            Assert.IsTrue(dict["add"].SimpleInlinableMethod);
            Assert.IsTrue(dict["isMatch"].SimpleInlinableMethod);
            Assert.IsTrue(dict["add2"].SimpleMethodProxy);
            Assert.IsTrue(dict["add2"].SimpleInlinableMethod);
            Assert.IsTrue(dict["add3"].SimpleInlinableMethod);
            Assert.IsFalse(dict["add"].SimpleMethodProxy);
            Assert.IsFalse(dict["isMatch"].SimpleMethodProxy);
            Assert.IsFalse(dict["add3"].SimpleMethodProxy);
        }

        [TestMethod]
        public void JstIdentifierCounterVisitorTest()
        {
            var matchers = ScopeIdentifierUsageMatcher.ListReadYamlFromResource(
                InlineResourceNS + "SimpleInlinableUsageCountTest.yaml");
            var scopeBlock = JSParserAndGeneratorHelper.ReadJstFromResourceScript(
                InlineResourceNS + "SimpleInlinableMethods.js");

            matchers[0].AssertUsage(scopeBlock.Scope);

            scopeBlock.Scope.ResetUsageCounter();
            matchers[1].AssertUsage(scopeBlock.Scope);

            var identCounter = new IdentifierCounterVisitor();
            ((IJstVisitor)identCounter).DispatchStatement(scopeBlock);

            matchers[2].AssertUsage(scopeBlock.Scope);
        }

        [DataTestMethod]
        [DataRow("SimpleInlinableMethods.js", "SimpleProxyFixerUsageCount.yaml")]
        [DataRow("ProxyMethodRegressions.js", "ProxyMethodRegressions.yaml", "ProxyMethodRegressions_Post.js")]
        public void JstProxyFixerVisitorTest(
            string jsFileName,
            string verifyFileName,
            string postJsFileName = null)
        {
            var identCounter = new IdentifierCounterVisitor();
            var unusedMethodRemover = new UnusedMethodRemover();
            var inlinableVisitor = new InlineableVisitor();

            var scopeBlock = JSParserAndGeneratorHelper.ReadJstFromResourceScript(
                InlineResourceNS + jsFileName);
            ((IJstVisitor)inlinableVisitor).DispatchStatement(scopeBlock);

            var proxyFixer = new ProxyFixer(inlinableVisitor.Functions);
            var block = proxyFixer.DispatchStatementExt(scopeBlock);

            block.Scope.ResetUsageCounter();

            ((IJstVisitor)identCounter).DispatchStatement(block);

            var matcher = ScopeIdentifierUsageMatcher.ListReadYamlFromResource(
                InlineResourceNS + verifyFileName);

            matcher[0].AssertUsage(block.Scope);
            block = unusedMethodRemover.DispatchStatementExt(block);
            block.Scope.ResetUsageCounter();
            ((IJstVisitor)identCounter).DispatchStatement(block);
            matcher[1].AssertUsage(block.Scope);

            if (postJsFileName != null)
            {
                JSParserAndGeneratorHelper.CompareJstTokens(
                    block as ScopeBlock,
                    InlineResourceNS + postJsFileName);
            }
        }
    }
}
