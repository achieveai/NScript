namespace CssParser.Test
{
    using System;
    using CssParser;
    using NUnit.Framework;

    [TestFixture]
    public class CssGrammerUnitTest
    {
        [Test]
        public void BasicTest()
        {
            CssGrammer grammer = new CssGrammer(".body { border: #ddeeff thin 123px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            Assert.AreEqual(1, grammer.Rules[0].Selectors.Count);
            Assert.IsInstanceOf<CssClassName>(grammer.Rules[0].Selectors[0]);

            Assert.AreEqual(1, grammer.Rules[0].Properties.Count);
            Assert.AreEqual("border", grammer.Rules[0].Properties[0].PropertyName);
            var propertyArgs = new string[] { "#ddeeff", "thin", "123px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[i].ToString());
            }
        }

        [Test]
        public void TestCssSelector()
        {
            CssGrammer grammer = new CssGrammer(".list > div { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<CssRuleSelector>(selectors[0]);
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOf<CssClassName>(ruleSelector.Selectors[0]);
            Assert.IsInstanceOf<CssTagName>(ruleSelector.Selectors[1]);
            Assert.AreEqual(SelectorOp.ParentOf, ruleSelector.Ops[0]);
        }

        [Test]
        public void TestCssRuleSelector3Deep()
        {
            CssGrammer grammer = new CssGrammer(".list > a > #div { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<CssRuleSelector>(selectors[0]);
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(3, ruleSelector.Selectors.Count);
            Assert.IsInstanceOf<CssClassName>(ruleSelector.Selectors[0]);
            Assert.IsInstanceOf<CssTagName>(ruleSelector.Selectors[1]);
            Assert.IsInstanceOf<CssId>(ruleSelector.Selectors[2]);
        }

        [Test]
        public void PropertyOnlyTest()
        {
            CssGrammer grammer = new CssGrammer("border: #ddeeff thin 1px solid;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            Assert.AreEqual("border", grammer.Properties[0].PropertyName);

            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[i]);
            }
        }

        [Test]
        public void PropertiesOnlyTest()
        {
            CssGrammer grammer = new CssGrammer("border: #ddeeff thin 1px solid; background-color: #ffeedd;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(2, grammer.Properties.Count);

            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[i]);
            }

            propertyArgs = new string[] { "#ffeedd" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[1].PropertyArgs[i]);
            }
        }
    }
}
