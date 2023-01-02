namespace NScript.JSParser.Test
{
    using MoreLinq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NScript.JST.Visitors;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
            var scopeBlock = JSParserAndGeneratorHelper.ReadJstFromResourceScript(
                InlineResourceNS + "SimpleInlinableMethods.js");

            scopeBlock.Scope.ScopedIdentifiers
                .ForEach(si =>
                {
                    switch (si.SuggestedName)
                    {
                        case "add":
                            Assert.AreEqual(1, si.UsageCount);
                            break;           
                        case "add2":         
                            Assert.AreEqual(2, si.UsageCount);
                            break;           
                        case "add3":         
                            Assert.AreEqual(0, si.UsageCount);
                            break;           
                        case "isMatch":      
                            Assert.AreEqual(0, si.UsageCount);
                            break;
                    }
                });

            scopeBlock.Scope.ChildScopes
                .SelectMany(cs => cs.ParameterIdentifiers)
                .ForEach(si => Assert.AreEqual(1, si.UsageCount));

            scopeBlock.Scope.ResetUsageCounter();
            scopeBlock.Scope.ScopedIdentifiers
                .ForEach(si => Assert.AreEqual(0, si.UsageCount));

            scopeBlock.Scope.ChildScopes
                .SelectMany(cs => cs.ParameterIdentifiers)
                .ForEach(si => Assert.AreEqual(0, si.UsageCount));

            var identCounter = new IdentifierCounterVisitor();
            ((IJstVisitor)identCounter).DispatchStatement(scopeBlock);

            scopeBlock.Scope.ScopedIdentifiers
                .ForEach(si =>
                {
                    switch (si.SuggestedName)
                    {
                        case "add":
                            Assert.AreEqual(2, si.UsageCount);
                            break;           
                        case "add2":         
                            Assert.AreEqual(3, si.UsageCount);
                            break;           
                        case "add3":         
                            Assert.AreEqual(1, si.UsageCount);
                            break;           
                        case "isMatch":      
                            Assert.AreEqual(1, si.UsageCount);
                            break;
                    }
                });

            scopeBlock.Scope.ChildScopes
                .SelectMany(cs => cs.ParameterIdentifiers)
                .ForEach(si => Assert.AreEqual(2, si.UsageCount));

        }
    }
}
